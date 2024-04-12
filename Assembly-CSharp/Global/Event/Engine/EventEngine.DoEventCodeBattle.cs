using System;

public partial class EventEngine
{
	public Int32 DoEventCodeBattle(PosObj po, Int32 code)
	{
		switch ((EBin.event_code_binary)code)
		{
			case EBin.event_code_binary.SHADOWON:
				ff9shadow.FF9ShadowOnBattle(po.uid);
				return 0;
			case EBin.event_code_binary.SHADOWOFF:
				ff9shadow.FF9ShadowOffBattle(po.uid);
				return 0;
			case EBin.event_code_binary.SHADOWSCALE:
				Int32 xScale = this.getv1();
				Int32 zScale = this.getv1();
				ff9shadow.FF9ShadowSetScaleBattle(po.uid, xScale, zScale);
				return 0;
			case EBin.event_code_binary.SHADOWOFFSET:
				Int32 offsetX = this.getv2();
				Int32 offsetZ = this.getv2();
				ff9shadow.FF9ShadowSetOffsetBattle(po.uid, offsetX, offsetZ);
				return 0;
			case EBin.event_code_binary.SHADOWLOCK:
				Int32 rotY = this.getv1();
				ff9shadow.FF9ShadowLockYRotBattle(po.uid, rotY << 4);
				return 0;
			case EBin.event_code_binary.SHADOWUNLOCK:
				ff9shadow.FF9ShadowUnlockYRotBattle(po.uid);
				return 0;
			case EBin.event_code_binary.SHADOWAMP:
				Int32 amp = this.getv1();
				ff9shadow.FF9ShadowSetAmpBattle(po.uid, amp);
				return 0;
			case EBin.event_code_binary.RAIN:
				Byte strength = (Byte)this.getv1();
				Byte speed = (Byte)this.getv1();
				FF9StateSystem.Common.FF9.btl_rain = strength;
				return 0;
			default:
				return 1;
		}
	}
}
