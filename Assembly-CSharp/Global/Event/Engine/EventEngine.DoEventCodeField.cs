using System;

public partial class EventEngine
{
	public Int32 DoEventCodeField(PosObj po, Int32 code)
	{
		switch ((EBin.event_code_binary)code)
		{
			case EBin.event_code_binary.SHADOWON:
				ff9shadow.FF9ShadowOnField((Int32)po.uid);
				return 0;
			case EBin.event_code_binary.SHADOWOFF:
				ff9shadow.FF9ShadowOffField((Int32)po.uid);
				po.isShadowOff = true;
				return 0;
			case EBin.event_code_binary.SHADOWSCALE:
				Int32 xScale = this.getv1();
				Int32 zScale = this.getv1();
				ff9shadow.FF9ShadowSetScaleField((Int32)po.uid, xScale, zScale);
				return 0;
			case EBin.event_code_binary.SHADOWOFFSET:
				Int32 num1 = this.getv2();
				Int32 num2 = this.getv2();
				ff9shadow.FF9ShadowSetOffsetField((Int32)po.uid, (Single)num1, (Single)num2);
				return 0;
			case EBin.event_code_binary.SHADOWLOCK:
				Int32 num3 = this.getv1();
				ff9shadow.FF9ShadowLockYRotField((Int32)po.uid, (Single)(num3 << 4));
				return 0;
			case EBin.event_code_binary.SHADOWUNLOCK:
				ff9shadow.FF9ShadowUnlockYRotField((Int32)po.uid);
				return 0;
			case EBin.event_code_binary.SHADOWAMP:
				Int32 num4 = this.getv1();
				ff9shadow.FF9ShadowSetAmpField((Int32)po.uid, (Int32)(Byte)num4);
				return 0;
			case EBin.event_code_binary.RAIN:
				Int32 strength = this.getv1();
				Int32 speed = this.getv1();
				this.fieldmap.rainRenderer.SetRainParam(strength, speed);
				this._ff9.btl_rain = (Byte)strength;
				return 0;
			default:
				return 1;
		}
	}
}
