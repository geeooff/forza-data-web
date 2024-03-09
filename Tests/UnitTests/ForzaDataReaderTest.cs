using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ForzaData.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ForzaData.Tests.UnitTests
{
	[TestClass]
	public class ForzaDataReaderTest
	{
        private const int SledPacketLength = 232;
		private const int CarDashPacketLength = 311;
		private const int HorizonCarDashPacketLength = 324;
        private const int UnknownPacketLength = 666;
        private const int ZeroPacketLength = 0;

        [TestMethod]
        [DataTestMethod]
        [DataRow(SledPacketLength, ForzaDataVersion.Sled)]
        [DataRow(CarDashPacketLength, ForzaDataVersion.CarDash)]
        [DataRow(HorizonCarDashPacketLength, ForzaDataVersion.HorizonCarDash)]
        [DataRow(UnknownPacketLength, ForzaDataVersion.Unknown)]
        [DataRow(ZeroPacketLength, ForzaDataVersion.Unknown)]
		public void VersionResolution(int length, ForzaDataVersion expectedVersion)
        {
            // TODO get the version from the length
            var actualVersion = ForzaDataVersion.Unknown;
            Assert.AreEqual(expectedVersion, actualVersion);
        }
	}
}
