using System;
using UnityXInput;
using XInputDotNetPure;

public static class GamePad
{
	public static GamePadState GetState(PlayerIndex playerIndex)
	{
		return PersistenSingleton<XInputManager>.Instance.CurrentState;
	}

	public static void SetVibration(PlayerIndex playerIndex, Single leftMotor, Single rightMotor)
	{
		PersistenSingleton<XInputManager>.Instance.SetVibration(leftMotor, rightMotor);
	}
}
