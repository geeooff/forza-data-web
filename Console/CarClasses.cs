namespace ForzaData.Console;

public static class CarClasses
{
	private static class Motorsport
	{
		internal static bool IsMotorsportCarClass(int carClass, int carPerformance) => carClass switch
		{
			0 => carPerformance is >= 100 and <= 300, // E
			1 => carPerformance is >= 301 and <= 400, // D
			2 => carPerformance is >= 401 and <= 500, // C
			3 => carPerformance is >= 501 and <= 600, // B
			4 => carPerformance is >= 601 and <= 700, // A
			5 => carPerformance is >= 701 and <= 800, // S
			6 => carPerformance is >= 801 and <= 900, // R
			7 => carPerformance is >= 901 and <= 998, // P
			8 => carPerformance == 999, // X
			_ => false
		};

		internal static string? GetMotorsportCarClassCode(int carClass) => carClass switch
		{
			0 => "E",
			1 => "D",
			2 => "C",
			3 => "B",
			4 => "A",
			5 => "S",
			6 => "R",
			7 => "P",
			8 => "X",
			_ => null
		};
	}

	private static class Horizon
	{
		internal static bool IsHorizonCarClass(int carClass, int carPerformance) => carClass switch
		{
			0 => carPerformance is >= 100 and <= 500, // D
			1 => carPerformance is >= 501 and <= 600, // C
			2 => carPerformance is >= 601 and <= 700, // B
			3 => carPerformance is >= 701 and <= 800, // A
			4 => carPerformance is >= 801 and <= 900, // S1
			5 => carPerformance is >= 901 and <= 998, // S2
			6 => carPerformance == 999, // X
			_ => false
		};

		internal static string? GetHorizonCarClassCode(int carClass) => carClass switch
		{
			0 => "D",
			1 => "C",
			2 => "B",
			3 => "A",
			4 => "S1",
			5 => "S2",
			6 => "X",
			_ => null
		};
	}

	public static string? GetCarClassCode(int carClass, int carPerformance)
	{
		string? carClassCode = null;

		if (Motorsport.IsMotorsportCarClass(carClass, carPerformance))
			carClassCode = Motorsport.GetMotorsportCarClassCode(carClass);
		else if (Horizon.IsHorizonCarClass(carClass, carPerformance))
			carClassCode = Horizon.GetHorizonCarClassCode(carClass);

		return carClassCode;
	}
}
