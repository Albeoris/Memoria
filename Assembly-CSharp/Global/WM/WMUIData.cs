using System;
using UnityEngine;

public static class WMUIData
{
	public static WMWorld World
	{
		get
		{
			return Singleton<WMWorld>.Instance;
		}
	}

	public static Single MainCharacterRotationY
	{
		get
		{
			if (ff9.w_moveActorPtr == (UnityEngine.Object)null)
			{
				return 0f;
			}
			return ff9.w_moveActorPtr.rot1;
		}
	}

	public static Vector2 MainCharacterPosition
	{
		get
		{
			Single x = ff9.w_moveActorPtr.RealPosition.x;
			Single z = ff9.w_moveActorPtr.RealPosition.z;
			Int32 mode = (Int32)ff9.w_naviMapno * 2;
			mode = 1;
			Single x2;
			Single y;
			ff9.w_naviGetPos(x, z, out x2, out y, mode);
			return new Vector2(x2, y);
		}
	}

	public static Single CameraRotationY
	{
		get
		{
			return ff9.w_cameraSysDataCamera.rotation;
		}
	}

	public static Vector2 CameraPosition
	{
		get
		{
			Single x = ff9.w_moveActorPtr.RealPosition.x;
			Single z = ff9.w_moveActorPtr.RealPosition.z;
			Int32 mode = (Int32)ff9.w_naviMapno * 2;
			Single x2;
			Single y;
			ff9.w_naviGetPos(x, z, out x2, out y, mode);
			return new Vector2(x2, y);
		}
	}

	public static Single ChocoboPositionX
	{
		get
		{
			if (ff9.w_moveChocoboPtr == (UnityEngine.Object)null)
			{
				return 0f;
			}
			return ff9.w_moveChocoboPtr.RealPosition.x / WMUIData.World.Width;
		}
	}

	public static Single ChocoboPositionZ
	{
		get
		{
			if (ff9.w_moveChocoboPtr == (UnityEngine.Object)null)
			{
				return 0f;
			}
			return 1f - -ff9.w_moveChocoboPtr.RealPosition.z / WMUIData.World.Height;
		}
	}

	public static Vector2 ChocoboPosition
	{
		get
		{
			if (ff9.w_moveChocoboPtr == (UnityEngine.Object)null)
			{
				return Vector2.zero;
			}
			Single x = ff9.w_moveChocoboPtr.RealPosition.x;
			Single z = ff9.w_moveChocoboPtr.RealPosition.z;
			Int32 mode = (Int32)ff9.w_naviMapno * 2;
			mode = 1;
			Single x2;
			Single y;
			ff9.w_naviGetPos(x, z, out x2, out y, mode);
			return new Vector2(x2, y);
		}
	}

	public static Single PlanePositionX
	{
		get
		{
			if (ff9.w_movePlanePtr == (UnityEngine.Object)null)
			{
				return 0f;
			}
			return ff9.w_movePlanePtr.RealPosition.x / WMUIData.World.Width;
		}
	}

	public static Single PlanePositionZ
	{
		get
		{
			if (ff9.w_movePlanePtr == (UnityEngine.Object)null)
			{
				return 0f;
			}
			return 1f - -ff9.w_movePlanePtr.RealPosition.z / WMUIData.World.Height;
		}
	}

	public static Vector2 PlanePosition
	{
		get
		{
			if (ff9.w_movePlanePtr == (UnityEngine.Object)null)
			{
				return Vector2.zero;
			}
			Single x = ff9.w_movePlanePtr.RealPosition.x;
			Single z = ff9.w_movePlanePtr.RealPosition.z;
			Int32 mode = (Int32)ff9.w_naviMapno * 2;
			mode = 1;
			Single x2;
			Single y;
			ff9.w_naviGetPos(x, z, out x2, out y, mode);
			return new Vector2(x2, y);
		}
	}

	public static ff9.navipos[,] NavigationLocaition
	{
		get
		{
			return ff9.w_naviLocationPos;
		}
	}

	public static Int32 ActiveMapNo
	{
		get
		{
			return (Int32)ff9.w_naviMapno;
		}
	}

	public static Int32 ControlNo
	{
		get
		{
			return (Int32)ff9.w_moveCHRControl_No;
		}
	}

	public static Byte StatusNo
	{
		get
		{
			return (Byte)((!(ff9.w_moveActorPtr == (UnityEngine.Object)null)) ? ff9.w_moveActorPtr.originalActor.index : 0);
		}
	}

	public static Boolean ChocoboIsAvailable
	{
		get
		{
			return !(ff9.w_moveChocoboPtr == (UnityEngine.Object)null) && ff9.w_moveChocoboPtr.originalActor.isEnableRenderer;
		}
	}

	public static Boolean PlaneIsAvailable
	{
		get
		{
			return ff9.w_movePlanePtr != (UnityEngine.Object)null;
		}
	}

	public static Boolean AutoPilotIsOn()
	{
		return ff9.w_moveAutoPilot != 0;
	}

	public static Boolean LocationAvailable(Int32 no)
	{
		return FF9StateSystem.World.IsBeeScene || ff9.w_naviLocationAvailable(no);
	}
}
