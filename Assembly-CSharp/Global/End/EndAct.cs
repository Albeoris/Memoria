public class EndAct
{
	public static EndAct.ENDACT_MODE_ENUM Endact_GetMode()
	{
		return EndAct.mode;
	}

	public static void Endact_SetMode(EndAct.ENDACT_MODE_ENUM mode)
	{
		EndAct.mode = mode;
	}

	public static EndAct.ENDACT_MODE_ENUM mode;

	public enum ENDACT_MODE_ENUM
	{
		ENDACT_MODE_STAND,
		ENDACT_MODE_HIT,
		ENDACT_MODE_DOUBLE,
		ENDACT_MODE_SPLIT,
		ENDACT_MODE_MAX
	}
}
