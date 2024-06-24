using System;

public class fldfmv : Singleton<fldfmv>
{
	public static Int32 ff9fieldfmv_fmvid(Int32 _discno, Int32 _fmvno)
	{
		return (_discno & 255) << 8 | (_fmvno & 255);
	}

	public static Int32 FF9FieldFMVInit(Int32 _fmvid, Int32 _bitdepth)
	{
		return Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(0, _fmvid, _bitdepth);
	}

	public static void FF9FieldFMVShutdown()
	{
		Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(1, 0, 0);
	}

	public static Int32 FF9FieldFMVSync()
	{
		return Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(3, 0, 0);
	}

	public static void FF9FieldFMVPlay()
	{
		Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(2, 0, 0);
	}

	public static Int32 FF9FieldFMVGetFrame()
	{
		return Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(4, 0, 0);
	}

	public static Int32 FF9FieldFMV24BitMode(Int32 _active)
	{
		return Singleton<fldfmv>.Instance.FF9FieldFMVDispatch(5, _active, 0);
	}

	protected override void Awake()
	{
		base.Awake();
		this.FF9Sys = PersistenSingleton<FF9StateSystem>.Instance;
		this.FF9 = FF9StateSystem.Common.FF9;
		this.FF9Field = FF9StateSystem.Field.FF9Field;
		this.mbg = MBG.Instance;
		this.mbg.SetFinishCallback(new Action(this.ff9fieldFMVShutdown));
	}

	public Int32 FF9FieldFMVDispatch(Int32 ParmType, Int32 Arg1, Int32 Arg2)
	{
		switch (ParmType & 15)
		{
		case 0:
		{
			Int32 fmvdisc = Arg1 >> 8 & 255;
			Int32 fmvno = Arg1 & 255;
			this.ff9fieldFMVInit(fmvdisc, fmvno, Arg2);
			break;
		}
		case 1:
			this.ff9fieldFMVShutdown();
			break;
		case 2:
			fldfmv.ff9fieldFMVAttr |= 1;
			break;
		case 3:
		{
			Int32 result;
			if (fldfmv.fmvStatus == 8)
			{
				result = 1;
			}
			else if (fldfmv.fmvStatus >= 5)
			{
				result = 0;
			}
			else
			{
				result = 16;
			}
			return result;
		}
		case 4:
			return this.mbg.GetFrame;
		case 5:
			fldfmv.fmvDepth = Arg1;
			this.mbg.Set24BitMode(fldfmv.fmvDepth);
			break;
		}
		return 0;
	}

	public void Update()
	{
	}

	public unsafe void ff9fieldFMVService()
	{
		if (fldfmv.fmvStatus == 1 || fldfmv.fmvStatus == 2)
		{
			fldfmv.ff9fieldFMVAttr |= 8;
			fldfmv.fmvStatus = 3;
		}
		if ((fldfmv.fmvStatus == 3 && (fldfmv.ff9fieldFMVAttr & 8) != 0) || fldfmv.fmvStatus == 4)
		{
			fldfmv.fmvStatus = 5;
		}
		switch (fldfmv.fmvStatus)
		{
		case 5:
			if ((fldfmv.ff9fieldFMVAttr & 1) != 0 || FF9StateSystem.Common.FF9.fldMapNo == 100)
			{
				Int32 num = 1;
				if (FF9StateSystem.Common.FF9.id != 0)
				{
					num++;
				}
				FieldMap.FF9FieldAttr.field[0, num - 1] = 1536;
				FieldMap.FF9FieldAttr.ff9[0, num - 1] = 25;
				if (!this.mbg.isFMV045)
				{
					FieldMap.FF9FieldAttr.field[0, num - 1] |= (UInt16)2048;
				}
				if (fldfmv.fmvDepth != 0)
				{
					FieldMap.FF9FieldAttr.ff9[0, num - 1] |= (UInt16)68;
					FieldMap.FF9FieldAttr.field[0, num - 1] |= (UInt16)1;
				}
				FieldMap.FF9FieldAttr.fmv[0, num - 1] = 64;
				FieldMap.FF9FieldAttr.fmv[1, num] = 64;
				FieldMap.FF9FieldAttr.fmv[0, num] = 2;
				this.mbg.SetFadeInParameters(2, 4, 1);
				this.mbg.Set24BitMode(fldfmv.fmvDepth);
				this.mbg.Play();
				fldfmv.fmvStatus = 6;
			}
			break;
		case 6:
		{
			UInt64 num2 = this.mbg.IsPlaying();
			Int32 getFrame = this.mbg.GetFrame;
			if ((fldfmv.ff9fieldFMVAttr & 2) != 0 && (num2 & 2UL) != 0UL && getFrame >= this.mbg.GetFrameCount)
			{
				if ((fldfmv.ff9fieldFMVAttr & 256) == 0)
				{
					if (getFrame <= fldfmv.ff9fieldFMVLastFrame)
					{
						FF9StateSystem.Common.FF9.attr &= 4294967266u;
						FF9StateSystem.Field.FF9Field.attr &= 4294963710u;
					}
					else
					{
						FieldMap.FF9FieldAttr.ff9[1, 0] = 29;
						FieldMap.FF9FieldAttr.field[1, 0] = 7681;
					}
				}
				fldfmv.ff9fieldFMVAttr |= 256;
				this.ff9fieldFMVRestoreState();
			}
			fldfmv.ff9fieldFMVLastFrame = getFrame;
			break;
		}
		case 7:
			if ((fldfmv.ff9fieldFMVAttr & 4) != 0)
			{
				this.ff9fieldFMVRestoreState();
				fldfmv.fmvStatus = 8;
			}
			break;
		}
	}

	public void ff9fieldFMVRestoreState()
	{
		this.FF9.attr &= 4294967202u;
		this.FF9Field.attr &= 4294959614u;
		fldfmv.ff9fieldFMVAttr &= -512;
	}

	private void ff9fieldFMVInit(Int32 FMVDisc, Int32 FMVNo, Int32 In24Bit)
	{
		fldfmv.ff9fieldFMVAttr = 0;
		fldfmv.fmvDisc = FMVDisc;
		fldfmv.fmvNo = FMVNo;
		fldfmv.fmvDepth = In24Bit;
		this.mbg.Seek(fldfmv.fmvDisc, fldfmv.fmvNo);
		fldfmv.fmvStatus = 1;
	}

	private void ff9fieldFMVShutdown()
	{
		if (this.mbg.isFastForwardOnBeforePlayingMBG)
		{
			FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.HighSpeedMode, true);
			PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.HighSpeedMode, true);
		}
		this.mbg.Purge();
		fldfmv.ff9fieldFMVAttr |= 4;
		fldfmv.fmvStatus = 7;
		ff9fieldFMVService();
		UIManager.Field.MovieHitArea.SetActive(false);
		MBG.Instance.ResetFlags();
	}

	public Boolean ff9fieldFMVIsActive()
	{
		FF9FieldFMVServiceStatus ff9FieldFMVServiceStatus = (FF9FieldFMVServiceStatus)fldfmv.fmvStatus;
		return ff9FieldFMVServiceStatus >= FF9FieldFMVServiceStatus.FF9FIELD_FMVSERVICE_SERVICE && ff9FieldFMVServiceStatus < FF9FieldFMVServiceStatus.FF9FIELD_FMVSERVICE_COMPLETE && (fldfmv.ff9fieldFMVAttr & 2) != 0 && (this.mbg.IsPlaying() & 2UL) != 0UL;
	}

	private void OnDestroy()
	{
		this.ff9fieldFMVRestoreState();
		fldfmv.fmvStatus = 8;
	}

	public const Byte FLDFMV_DISC_MASK = 255;

	public const Byte FLDFMV_DISC_SHIFT = 8;

	public const Byte FLDFMV_NO_MASK = 255;

	public const Byte FLDFMV_NO_SHIFT = 0;

	public const Byte FLDFMV_OP_INIT = 0;

	public const Byte FLDFMV_OP_SHUTDOWN = 1;

	public const Byte FLDFMV_OP_PLAY = 2;

	public const Byte FLDFMV_OP_SYNC = 3;

	public const Byte FLDFMV_OP_GETFRAME = 4;

	public const Byte FLDFMV_OP_24BITMODE = 5;

	public const Byte FLDFMV_OP_MASK = 15;

	public const Byte FLDFMV_OP_SHIFT = 0;

	public const Byte FLDFMV_OP_FLAG_INIT = 0;

	public const Byte FLDFMV_OP_FLAG_SHUTDOWN = 1;

	public const Byte FLDFMV_OP_FLAG_PLAY = 2;

	public const Byte FLDFMV_OP_FLAG_SYNC = 3;

	public const Byte FLDFMV_OP_FLAG_GETFRAME = 4;

	public const Byte FLDFMV_OP_FLAG_24BITMODE = 5;

	public const Byte FLDFMV_INIT = 0;

	public const Byte FLDFMV_SHUTDOWN = 1;

	public const Byte FLDFMV_SYNC = 3;

	public const Byte FLDFMV_PLAY = 2;

	public const Byte FLDFMV_GETFRAME = 4;

	public const Byte FLDFMV_24BITMODE = 5;

	public const Byte FLDFMV_SOUNDSYNC_CYCLE_FRAMES = 50;

	public const Byte FLDFMV_SOUNDSYNC_LOSSAGE = 1;

	public const Byte FLDFMV_FADE_FRAMECOUNT = 3;

	public const Byte FLDFMV_STATUS_MASK = 127;

	public const Byte FLDFMV_STATUS_INITIALIZED = 0;

	public const Byte FLDFMV_STATUS_COMPLETE = 1;

	public const Byte FLDFMV_STATUS_UNINITIALIZED = 16;

	public const Byte FLDFMV_STATUS_BADDISC = 17;

	public const Byte FLDFMV_STATUS_DISABLED = 128;

	public const Byte FLDFMV_DEF_STREAM_SETCOUNT = 2;

	public const Byte FLDFMV_DEF_STREAM_FADEINCOUNT = 4;

	public const Int32 FF9_ATTR_FMV_SET = 1;

	public const Int32 FF9_ATTR_FMV_START = 2;

	public const Int32 FF9_ATTR_FMV_DONE = 4;

	public const Int32 FF9_ATTR_FMV_OVERLAY = 8;

	public const Int32 FF9_ATTR_FMV_STREAMFADEIN = 16;

	public const Int32 FF9_ATTR_FMV_STREAMFADEOUT = 32;

	public const Int32 FF9_ATTR_FMV_CAMERAFIRSTFRAME = 64;

	public const Int32 FF9_ATTR_FMV_SCREENFADEOUT = 128;

	public const Int32 FF9_ATTR_FMV_END = 256;

	public static Int32 ff9fieldFMVAttr;

	public static Int32 ff9fieldFMVLastFrame;

	public static Int32 fmvDisc;

	public static Int32 fmvNo;

	public static Int32 fmvDepth;

	public static Int32 fmvStatus;

	private FF9StateSystem FF9Sys;

	private FF9StateFieldSystem FF9Field;

	private FF9StateGlobal FF9;

	private MBG mbg;
}
