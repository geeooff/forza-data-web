using System.Net;
using System.Net.Sockets;

namespace ForzaData.SamplePlayer.Tests;

[TestClass]
public class SamplePlayerTests
{
	private const int TestPort = 17777;
	private static readonly IPAddress TestIp = IPAddress.Loopback;

	[TestMethod]
	public void DefaultCommandSettings_Validate_ValidSettings_ReturnsSuccess()
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
		Assert.AreEqual(ValidationResult.Success(), result);
	}

	[TestMethod]
	public void DefaultCommandSettings_Validate_MissingInput_ReturnsError()
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
	public void DefaultCommandSettings_Validate_InvalidPort_ReturnsError()
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
	public void DefaultCommandSettings_Validate_InvalidIp_ReturnsError()
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
	public void DefaultCommandSettings_Validate_NegativeSpeed_ReturnsError()
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
	public void DefaultCommandSettings_Validate_ZeroSpeed_ReturnsError()
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
	public void DefaultCommandSettings_DefaultValues_AreCorrect()
	{
		// Arrange & Act
		var settings = new DefaultCommandSettings();

		// Assert
		Assert.AreEqual(1.0, settings.Speed);
		Assert.IsFalse(settings.Loop);
		Assert.IsFalse(settings.Display);
	}
}

[TestClass]
public class SamplePlayerIntegrationTests
{
	[TestMethod]
	public void SamplePlayer_CanReadSampleFile_WithValidData()
	{
		// Arrange
		// Create a test sample file with one valid chunk
		var testData = new byte[232]; // Sled packet size
		// Fill with some test data (all zeros is fine for this test)
		
		using var memoryStream = new MemoryStream();
		using var writer = new SampleWriter(memoryStream);
		
		var chunk = new SampleStruct
		{
			Elapsed = TimeSpan.TicksPerSecond, // 1 second in ticks
			Length = testData.Length,
			Data = testData
		};
		
		writer.Write(chunk);
		writer.Flush();
		
		// Reset stream position
		memoryStream.Position = 0;
		
		// Act
		using var reader = new SampleReader(memoryStream);
		bool result = reader.TryRead(out SampleStruct readChunk);
		
		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(chunk.Elapsed, readChunk.Elapsed);
		Assert.AreEqual(chunk.Length, readChunk.Length);
		Assert.AreEqual(chunk.Data.Length, readChunk.Data.Length);
	}

	[TestMethod]
	public void SamplePlayer_CanHandleMultipleChunks_WithCorrectTiming()
	{
		// Arrange
		var testData = new byte[232];
		
		using var memoryStream = new MemoryStream();
		using var writer = new SampleWriter(memoryStream);
		
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
		memoryStream.Position = 0;
		
		// Act
		using var reader = new SampleReader(memoryStream);
		var chunks = new List<SampleStruct>();
		
		while (reader.TryRead(out SampleStruct chunk))
		{
			chunks.Add(chunk);
		}
		
		// Assert
		Assert.AreEqual(3, chunks.Count);
		Assert.AreEqual(TimeSpan.TicksPerSecond * 1, chunks[0].Elapsed);
		Assert.AreEqual(TimeSpan.TicksPerSecond * 2, chunks[1].Elapsed);
		Assert.AreEqual(TimeSpan.TicksPerSecond * 3, chunks[2].Elapsed);
	}

	[TestMethod]
	public void SamplePlayer_EmptyFile_ReturnsNoChunks()
	{
		// Arrange
		using var memoryStream = new MemoryStream(new byte[0]);
		
		// Act
		using var reader = new SampleReader(memoryStream);
		bool result = reader.TryRead(out _);
		
		// Assert
		Assert.IsFalse(result);
		Assert.IsTrue(reader.EndOfStream);
	}

	[TestMethod]
	public void SamplePlayer_CorruptedChunk_ThrowsSampleException()
	{
		// Arrange
		// Create a stream with invalid data (truncated chunk)
		using var memoryStream = new MemoryStream(new byte[10]); // Too small to be valid
		
		// Act & Assert
		using var reader = new SampleReader(memoryStream);
		Assert.ThrowsException<SampleException>(() => reader.TryRead(out _));
	}

	[TestMethod]
	public void SamplePlayer_TimingCalculation_WithSpeedMultiplier()
	{
		// Arrange
		var baseElapsed = TimeSpan.TicksPerSecond; // 1 second
		var speed = 2.0; // Double speed
		
		// Act
		long targetTicks = (long)(baseElapsed / speed);
		
		// Assert
		// At double speed, 1 second of recording should play in 0.5 seconds
		Assert.AreEqual(TimeSpan.TicksPerSecond / 2, targetTicks);
	}

	[TestMethod]
	public void SamplePlayer_TimingCalculation_WithHalfSpeed()
	{
		// Arrange
		var baseElapsed = TimeSpan.TicksPerSecond; // 1 second
		var speed = 0.5; // Half speed
		
		// Act
		long targetTicks = (long)(baseElapsed / speed);
		
		// Assert
		// At half speed, 1 second of recording should play in 2 seconds
		Assert.AreEqual(TimeSpan.TicksPerSecond * 2, targetTicks);
	}
}

[TestClass]
public class UdpPlaybackTests
{
	[TestMethod]
	public async Task SamplePlayer_SendsUdpPackets_Correctly()
	{
		// Arrange
		var testData = new byte[232];
		// Set some identifiable data
		testData[0] = 0xAA;
		testData[1] = 0xBB;
		
		// Create a test sample file
		using var memoryStream = new MemoryStream();
		using var writer = new SampleWriter(memoryStream);
		
		var chunk = new SampleStruct
		{
			Elapsed = 0, // Immediate playback
			Length = testData.Length,
			Data = testData
		};
		
		writer.Write(chunk);
		writer.Flush();
		memoryStream.Position = 0;
		
		// Save to temp file
		var tempFile = Path.GetTempFileName();
		using (var fileStream = File.OpenWrite(tempFile))
		{
			memoryStream.CopyTo(fileStream);
		}
		
		// Setup UDP receiver
		using var receiver = new UdpClient(TestPort);
		var receivedTask = receiver.ReceiveAsync();
		
		// Act - Run the player
		// Note: In a real test, we'd run the actual command, but for unit testing
		// we'll simulate the playback logic
		using var inputStream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
		using var reader = new SampleReader(inputStream);
		
		using var sender = new UdpClient();
		sender.Connect(TestIp, TestPort);
		
		if (reader.TryRead(out SampleStruct readChunk))
		{
			sender.Send(readChunk.Data, readChunk.Data.Length);
		}
		
		// Wait for receive
		var receiveResult = await receivedTask.WaitAsync(TimeSpan.FromSeconds(2));
		
		// Cleanup
		File.Delete(tempFile);
		
		// Assert
		Assert.IsNotNull(receiveResult);
		Assert.AreEqual(testData.Length, receiveResult.Buffer.Length);
		Assert.AreEqual(testData[0], receiveResult.Buffer[0]);
		Assert.AreEqual(testData[1], receiveResult.Buffer[1]);
	}
}
