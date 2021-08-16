using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ForzaData.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ForzaData.Tests
{
	[TestClass]
	public class ForzaDataReaderTests
	{
		[TestMethod]
		[DataTestMethod]
		[DataRow("FM7_Sled_2019-04-25_Xbox.bin")]
		[DataRow("FM7_CarDash_2019-04-25_Xbox.bin")]
		public void ActualVsLegacy(string sampleFileName)
		{
			var sampleFolder = new DirectoryInfo("Samples");
			var sampleFile = new FileInfo(Path.Combine(sampleFolder.FullName, sampleFileName));

			using FileStream sampleStream = sampleFile.OpenRead(); 
			using SampleReader sampleReader = new SampleReader(sampleStream);

			var legacyDataReader = new LegacyForzaDataReader();
			var actualDataReader = new ForzaDataReader();

			var sledFields = typeof(ForzaSledDataStruct).GetFields();
			var carDashFields = typeof(ForzaCarDashDataStruct).GetFields();

			// iterate every chunk
			while (sampleReader.TryRead(out SampleStruct chunk))
			{
				// read chunk with old and new readers
				var expectedForzaData = legacyDataReader.Read(chunk.Data);
				var actualForzaData = actualDataReader.Read(chunk.Data);

				// comparing read version
				Assert.AreEqual(
					expectedForzaData.Version,
					actualForzaData.Version,
					$"{nameof(ForzaDataStruct.Version)} comparison failed at chunk Elapsed = {chunk.Elapsed}"
				);

				// comparing each sled field
				foreach (var sledField in sledFields)
				{
					Assert.AreEqual(
						sledField.GetValue(expectedForzaData.Sled),
						sledField.GetValue(actualForzaData.Sled),
						$"{sledField.Name} comparison failed at chunk Elapsed = {chunk.Elapsed}"
					);
				}

				// comparing each car dash field
				foreach (var carDashField in carDashFields)
				{
					Assert.AreEqual(
						carDashField.GetValue(expectedForzaData.CarDash),
						carDashField.GetValue(actualForzaData.CarDash),
						$"{carDashField.Name} comparison failed at chunk Elapsed = {chunk.Elapsed}"
					);
				}

				//Trace.WriteLine($"Chunk #{chunk.Elapsed}/byte {sampleStream.Position}: success");
			}
		}
	}
}
