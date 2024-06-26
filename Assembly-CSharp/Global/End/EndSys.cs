using System;

public class EndSys
{
	public static EndSys.ENDSYS_MODE_ENUM Endsys_GetMode()
	{
		return EndSys.mode;
	}

	public static void Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM mode)
	{
		EndSys.mode = mode;
	}

	private static EndSys.ENDSYS_MODE_ENUM mode;

	public enum ENDSYS_MODE_ENUM
	{
		ENDSYS_MODE_NONE,
		ENDSYS_MODE_SELECT,
		ENDSYS_MODE_BET,
		ENDSYS_MODE_ACTION,
		ENDSYS_MODE_QUIT,
		ENDSYS_MODE_MAX
	}
}
