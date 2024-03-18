using System.Runtime.InteropServices;

namespace ForzaData.Core;

/// <summary>
/// "Car dash" data structure
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct ForzaCarDashDataStruct
{
	//Position (meters)
	public float PositionX;
	public float PositionY;
	public float PositionZ;

	public float Speed; // meters per second
	public float Power; // watts
	public float Torque; // newton meter

	public float TireTempFrontLeft;
	public float TireTempFrontRight;
	public float TireTempRearLeft;
	public float TireTempRearRight;

	public float Boost;
	public float Fuel;
	public float DistanceTraveled;
	public float BestLap;
	public float LastLap;
	public float CurrentLap;
	public float CurrentRaceTime;

	public ushort LapNumber;
	public byte RacePosition;

	public byte Accel;
	public byte Brake;
	public byte Clutch;
	public byte HandBrake;
	public byte Gear;
	public sbyte Steer;

	public sbyte NormalizedDrivingLine;
	public sbyte NormalizedAIBrakeDifference;
}
