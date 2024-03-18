namespace ForzaData.Core;

public partial class ForzaDataReader
{
	/// <summary>Forza Motorsport 7 and Forza Motorsport (2023) sled only</summary>
	internal const int SledPacketLength = 232;

	/// <summary>Forza Motorsport 7 sled + car dash</summary>
	internal const int CarDashV1PacketLength = 311;

	/// <summary>Forza Horizon 4/5 sled + car dash</summary>
	internal const int CarDashV2PacketLength = 324;

	/// <summary>Forza Motorsport (2023) sled + car dash</summary>
	internal const int CarDashV3PacketLength = 331;

	/// <summary>Range of sled data (for all games)</summary>
	internal static readonly Range SledRange = 0..232;

	/// <summary>Range of car dash data (for Forza Motorsport 7 and Forza Motorsport (2023))</summary>
	internal static readonly Range CarDashRange = 232..311;

	/// <summary>Range of Horizon extras data (for Forza Horizon 4 and 5)</summary>
	internal static readonly Range HorizonExtrasRange = 232..244;

	/// <summary>Range of car dash data (for Forza Horizon 4 and 5)</summary>
	internal static readonly Range CarDashAfterHorizonExtrasRange = 244..323;

	/// <summary>Range of Motorsport extras data (for Forza Motorsport (2023))</summary>
	internal static readonly Range MotorsportExtrasRange = 311..331;

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
		ForzaDataStruct output = new()
		{
			Version = GetVersion(input.Length)
		};

		if (output.Version != ForzaDataVersion.Unknown)
		{
			output.Sled = ReadSledData(input[SledRange]);

			if (output.Version == ForzaDataVersion.CarDashV1)
			{
				output.CarDash = ReadCarDashData(input[CarDashRange]);
			}
			else if (output.Version == ForzaDataVersion.CarDashV2)
			{
				output.HorizonExtras = ReadHorizonExtrasData(input[HorizonExtrasRange]);
				output.CarDash = ReadCarDashData(input[CarDashAfterHorizonExtrasRange]);
			}
			else if (output.Version == ForzaDataVersion.CarDashV3)
			{
				output.CarDash = ReadCarDashData(input[CarDashRange]);
				output.MotorsportExtras = ReadMotorsportExtrasData(input[MotorsportExtrasRange]);
			}
		}

		return output;
	}

	private static ForzaDataVersion GetVersion(int length) => length switch
	{
		SledPacketLength => ForzaDataVersion.Sled,
		CarDashV1PacketLength => ForzaDataVersion.CarDashV1,
		CarDashV2PacketLength => ForzaDataVersion.CarDashV2,
		CarDashV3PacketLength => ForzaDataVersion.CarDashV3,
		_ => ForzaDataVersion.Unknown,
	};
}
