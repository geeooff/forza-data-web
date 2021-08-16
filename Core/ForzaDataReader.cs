using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ForzaData.Core
{
	public partial class ForzaDataReader
	{
		internal const int SledPacketLength = 232;
		internal const int CarDashPacketLength = 311;
		internal const int HorizonCarDashPacketLength = 324;

		internal static readonly Range SledRange = 0..232;
		internal static readonly Range CarDashRange = 232..311;
		internal static readonly Range HorizonCarDashRange = 244..323;

		public ForzaDataReader()
		{
			
		}

		public bool TryRead(in ReadOnlySpan<byte> input, out ForzaDataStruct output)
		{
			try
			{
				output = Read(input);
				return true;
			}
			catch
			{
				output = new ForzaDataStruct()
				{
					Version = ForzaDataVersion.Unknown
				};
				return false;
			}
		}

		public ForzaDataStruct Read(in ReadOnlySpan<byte> input)
		{
			ForzaDataStruct output = new ForzaDataStruct()
			{
				Version = GetVersion(input.Length)
			};

			if (output.Version != ForzaDataVersion.Unknown)
			{
				output.Sled = ReadSledData(input[SledRange]);

				if (output.Version == ForzaDataVersion.CarDash)
				{
					output.CarDash = ReadCarDashData(input[CarDashRange]);
				}
				else if (output.Version == ForzaDataVersion.HorizonCarDash)
				{
					output.CarDash = ReadCarDashData(input[HorizonCarDashRange]);
				}
			}

			return output;
		}

		private ForzaDataVersion GetVersion(int length) => length switch
		{
			SledPacketLength => ForzaDataVersion.Sled,
			CarDashPacketLength => ForzaDataVersion.CarDash,
			HorizonCarDashPacketLength => ForzaDataVersion.HorizonCarDash,
			_ => ForzaDataVersion.Unknown,
		};
	}
}
