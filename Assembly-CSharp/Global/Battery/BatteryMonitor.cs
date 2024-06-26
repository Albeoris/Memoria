using System;

public class BatteryMonitor
{
	public static Int32 GetBatteryPercent()
	{
		return (Int32)(100f * BatteryMonitor.GetBatteryLevel());
	}

	public static Single GetBatteryLevel()
	{
		return 1f;
	}

	public static Boolean IsBatteryCharging()
	{
		return true;
	}
}
