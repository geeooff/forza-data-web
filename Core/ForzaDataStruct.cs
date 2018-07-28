using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForzaData.Core
{
    public struct ForzaDataStruct
    {
		// = 1 when race is on. = 0 when in menus/race stopped …
		public int IsRaceOn;

		//Can overflow to 0 eventually
		public uint TimestampMS;

		public float EngineMaxRpm;
		public float EngineIdleRpm;
		public float CurrentEngineRpm;

		//In the car's local space; X = right, Y = up, Z = forward
		public float AccelerationX;
		public float AccelerationY;
		public float AccelerationZ;

		//In the car's local space; X = right, Y = up, Z = forward
		public float VelocityX;
		public float VelocityY;
		public float VelocityZ;

		//In the car's local space; X = pitch, Y = yaw, Z = roll
		public float AngularVelocityX; 
		public float AngularVelocityY;
		public float AngularVelocityZ;

		public float Yaw;
		public float Pitch;
		public float Roll;

		// Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
		public float NormalizedSuspensionTravelFrontLeft;
		public float NormalizedSuspensionTravelFrontRight;
		public float NormalizedSuspensionTravelRearLeft;
		public float NormalizedSuspensionTravelRearRight;

		// Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
		public float TireSlipRatioFrontLeft;
		public float TireSlipRatioFrontRight;
		public float TireSlipRatioRearLeft;
		public float TireSlipRatioRearRight;

		// Wheel rotation speed radians/sec.
		public float WheelRotationSpeedFrontLeft;
		public float WheelRotationSpeedFrontRight;
		public float WheelRotationSpeedRearLeft;
		public float WheelRotationSpeedRearRight;

		// = 1 when wheel is on rumble strip, = 0 when off.
		public int WheelOnRumbleStripFrontLeft;
		public int WheelOnRumbleStripFrontRight;
		public int WheelOnRumbleStripRearLeft;
		public int WheelOnRumbleStripRearRight;

		// = from 0 to 1, where 1 is the deepest puddle
		public float WheelInPuddleDepthFrontLeft;
		public float WheelInPuddleDepthFrontRight;
		public float WheelInPuddleDepthRearLeft;
		public float WheelInPuddleDepthRearRight;

		// Non-dimensional surface rumble values passed to controller force feedback
		public float SurfaceRumbleFrontLeft;
		public float SurfaceRumbleFrontRight;
		public float SurfaceRumbleRearLeft;
		public float SurfaceRumbleRearRight;

		// Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
		public float TireSlipAngleFrontLeft;
		public float TireSlipAngleFrontRight;
		public float TireSlipAngleRearLeft;
		public float TireSlipAngleRearRight;

		// Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
		public float TireCombinedSlipFrontLeft;
		public float TireCombinedSlipFrontRight;
		public float TireCombinedSlipRearLeft;
		public float TireCombinedSlipRearRight;

		// Actual suspension travel in meters
		public float SuspensionTravelMetersFrontLeft;
		public float SuspensionTravelMetersFrontRight;
		public float SuspensionTravelMetersRearLeft;
		public float SuspensionTravelMetersRearRight;

		/// <summary>
		/// Unique ID of the car make/model
		/// </summary>
		public int CarOrdinal;

		/// <summary>
		/// Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive 
		/// </summary>
		public int CarClass;

		/// <summary>
		/// Between 100 (slowest car) and 999 (fastest car) inclusive
		/// </summary>
		public int CarPerformanceIndex;

		/// <summary>
		/// Corresponds to EDrivetrainType; 0 = FWD, 1 = RWD, 2 = AWD
		/// </summary>
		public int DrivetrainType;

		/// <summary>
		/// Number of cylinders in the engine
		/// </summary>
		public int NumCylinders;
	}
}
