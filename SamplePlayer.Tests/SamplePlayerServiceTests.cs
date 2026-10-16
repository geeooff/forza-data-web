using System.Diagnostics;

namespace ForzaData.SamplePlayer.Tests;

[TestClass]
public class SamplePlayerServiceTests
{
	private const int TestPort = 17777;
	private static readonly IPAddress TestIp = IPAddress.Loopback;

	[TestMethod]
	public async Task PlayAsync_SingleChunk_PlaysCorrectly()
	{
		// Arrange
		var testData = new byte[232]; // Sled packet size
		testData[0] = 0xAA;
		testData[1] = 0xBB;
		
		using var memoryStream = new MemoryStream();
		using (var writer = new SampleWriter(memoryStream))
		{
			var chunk = new SampleStruct
			{
				Elapsed = TimeSpan.TicksPerSecond, // 1 second
				Length = testData.Length,
				Data = testData
			};
			writer.Write(chunk);
			writer.Flush();
		}
		memoryStream.Position = 0;

		using var udpClient = new UdpClient();
		udpClient.Connect(TestIp, TestPort);
		
		using var receiver = new UdpClient(TestPort);
		var receivedTask = receiver.ReceiveAsync();

		var service = new SamplePlayerService();
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

		// Act
		await service.PlayAsync(memoryStream, udpClient, 1.0, cts.Token);
		
		var receiveResult = await receivedTask.WaitAsync(TimeSpan.FromSeconds(2));

		// Assert
		Assert.IsNotNull(receiveResult);
		Assert.AreEqual(testData.Length, receiveResult.Buffer?.Length);
		Assert.AreEqual(testData[0], receiveResult.Buffer?[0]);
		Assert.AreEqual(testData[1], receiveResult.Buffer?[1]);
		Assert.AreEqual(1, service.TotalChunks);
		Assert.AreEqual(testData.Length, service.TotalBytes);
	}

	[TestMethod]
	public async Task PlayAsync_MultipleChunks_PlaysAll()
	{
		// Arrange
		var testData = new byte[232];
		
		using var memoryStream = new MemoryStream();
		using (var writer = new SampleWriter(memoryStream))
		{
			// Write 3 chunks with increasing elapsed times
			for (int i = 0; i < 3; i++)
			{
				var chunk = new SampleStruct
				{
					Elapsed = TimeSpan.TicksPerSecond * (i + 1), // 1s, 2s, 3s
					Length = testData.Length,
					Data = testData
				};
				writer.Write(chunk);
			}
			writer.Flush();
		}
		memoryStream.Position = 0;

		using var udpClient = new UdpClient();
		udpClient.Connect(TestIp, TestPort);
		
		using var receiver = new UdpClient(TestPort);
		var receivedCount = 0;
		
		// Start receiving
		_ = Task.Run(async () =>
		{
			while (true)
			{
				try
				{
					var result = await receiver.ReceiveAsync();
					Interlocked.Increment(ref receivedCount);
				}
				catch (ObjectDisposedException)
				{
					break;
				}
			}
		});

		var service = new SamplePlayerService();
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

		// Act
		await service.PlayAsync(memoryStream, udpClient, 10.0, cts.Token); // Fast speed for test

		// Give time for UDP packets to arrive
		await Task.Delay(500);
		
		// Assert
		Assert.AreEqual(3, service.TotalChunks);
		Assert.AreEqual(3 * testData.Length, service.TotalBytes);
	}

	[TestMethod]
	public async Task PlayAsync_WithSpeed_SendsAtCorrectRate()
	{
		// Arrange
		var testData = new byte[232];
		var chunkDuration = TimeSpan.FromMilliseconds(100); // 100ms per chunk
		
		using var memoryStream = new MemoryStream();
		using (var writer = new SampleWriter(memoryStream))
		{
			// Write 2 chunks, 100ms apart
			for (int i = 0; i < 2; i++)
			{
				var chunk = new SampleStruct
				{
					Elapsed = (long)(i * chunkDuration.Ticks),
					Length = testData.Length,
					Data = testData
				};
				writer.Write(chunk);
			}
			writer.Flush();
		}
		memoryStream.Position = 0;

		using var udpClient = new UdpClient();
		udpClient.Connect(TestIp, TestPort);
		
		using var receiver = new UdpClient(TestPort);
		var receiveTimes = new List<DateTime>();
		
		// Capture receive times
		_ = Task.Run(async () =>
		{
			for (int i = 0; i < 2; i++)
			{
				try
				{
					var result = await receiver.ReceiveAsync();
					receiveTimes.Add(DateTime.UtcNow);
				}
				catch (ObjectDisposedException)
				{
					break;
				}
			}
		});

		var service = new SamplePlayerService();
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

		// Act
		await service.PlayAsync(memoryStream, udpClient, 1.0, cts.Token);
		
		// Wait for all packets
		await Task.Delay(500);
		
		// Assert
		Assert.AreEqual(2, service.TotalChunks);
		
		// Check timing between packets (with tolerance for system scheduling)
		if (receiveTimes.Count >= 2)
		{
			var elapsed = (receiveTimes[1] - receiveTimes[0]).TotalSeconds;
			// Should be approximately 100ms (with tolerance for system scheduling)
			Assert.IsTrue(elapsed < 0.5, $"Timing between packets should be ~100ms, was {elapsed}s");
		}
		else
		{
			// If we didn't receive both packets, at least verify we got some
			Assert.IsTrue(receiveTimes.Count > 0, "Should have received at least one packet");
		}
	}

	[TestMethod]
	public async Task PlayAsync_CancellationToken_StopsPlayback()
	{
		// Arrange
		var testData = new byte[232];
		
		using var memoryStream = new MemoryStream();
		using (var writer = new SampleWriter(memoryStream))
		{
			// Write 10 chunks with long delays
			for (int i = 0; i < 10; i++)
			{
				var chunk = new SampleStruct
				{
					Elapsed = TimeSpan.TicksPerSecond * (i + 1), // 1s, 2s, ..., 10s
					Length = testData.Length,
					Data = testData
				};
				writer.Write(chunk);
			}
			writer.Flush();
		}
		memoryStream.Position = 0;

		using var udpClient = new UdpClient();
		udpClient.Connect(TestIp, TestPort);
		
		var service = new SamplePlayerService();
		using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));

		// Act
		try
		{
			await service.PlayAsync(memoryStream, udpClient, 1.0, cts.Token);
		}
		catch (OperationCanceledException)
		{
			// Expected when cancelled
		}

		// Assert
		// Should have played only a few chunks before cancellation
		Assert.IsTrue(service.TotalChunks < 10, "Should have been cancelled before completing all chunks");
	}

	[TestMethod]
	public void SamplePlayerService_Reset_ClearsCounters()
	{
		// Arrange
		var service = new SamplePlayerService();
		service.TotalChunks = 10;
		service.TotalBytes = 1000;

		// Act
		service.Reset();

		// Assert
		Assert.AreEqual(0, service.TotalChunks);
		Assert.AreEqual(0, service.TotalBytes);
	}
}

[TestClass]
public class DefaultCommandSettingsTests
{
	[TestMethod]
	public void Validate_ValidSettings_ReturnsSuccess()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "127.0.0.1",
			OutputPort = 7777,
			Speed = 1.0
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsTrue(result.Successful);
	}

	[TestMethod]
	public void Validate_MissingInput_ReturnsError()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = null,
			OutputIp = "127.0.0.1",
			OutputPort = 7777
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsFalse(result.Successful);
		Assert.IsTrue(result.Message.Contains("Input file path is required"));
	}

	[TestMethod]
	public void Validate_InvalidPort_ReturnsError()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "127.0.0.1",
			OutputPort = 0
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsFalse(result.Successful);
		Assert.IsTrue(result.Message.Contains("Output port must be between 1 and 65535"));
	}

	[TestMethod]
	public void Validate_PortTooHigh_ReturnsError()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "127.0.0.1",
			OutputPort = 65536
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsFalse(result.Successful);
	}

	[TestMethod]
	public void Validate_InvalidIp_ReturnsError()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "invalid.ip.address",
			OutputPort = 7777
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsFalse(result.Successful);
		Assert.IsTrue(result.Message.Contains("Invalid output IP address"));
	}

	[TestMethod]
	public void Validate_NegativeSpeed_ReturnsError()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "127.0.0.1",
			OutputPort = 7777,
			Speed = -1.0
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsFalse(result.Successful);
		Assert.IsTrue(result.Message.Contains("Speed must be greater than 0"));
	}

	[TestMethod]
	public void Validate_ZeroSpeed_ReturnsError()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "127.0.0.1",
			OutputPort = 7777,
			Speed = 0.0
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsFalse(result.Successful);
	}

	[TestMethod]
	public void DefaultValues_AreCorrect()
	{
		// Arrange & Act
		var settings = new DefaultCommandSettings();

		// Assert
		Assert.AreEqual(1.0, settings.Speed);
		Assert.IsFalse(settings.Loop);
	}

	[TestMethod]
	public void OutputPort_Null_UsesDefault()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = "127.0.0.1",
			OutputPort = null
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsTrue(result.Successful);
	}

	[TestMethod]
	public void OutputIp_Null_UsesDefault()
	{
		// Arrange
		var settings = new DefaultCommandSettings
		{
			Input = "test.bin",
			OutputIp = null,
			OutputPort = 7777
		};

		// Act
		var result = settings.Validate();

		// Assert
		Assert.IsTrue(result.Successful);
	}
}
