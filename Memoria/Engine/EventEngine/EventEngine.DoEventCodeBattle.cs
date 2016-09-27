using System;

public partial class EventEngine
{
    public Int32 DoEventCodeBattle(PosObj po, Int32 code)
    {
        switch ((EBin.event_code_binary)code)
        {
            case EBin.event_code_binary.SHADOWON:
                ff9shadow.FF9ShadowOnBattle((Int32)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWOFF:
                ff9shadow.FF9ShadowOffBattle((Int32)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWSCALE:
                Int32 xScale = this.getv1();
                Int32 zScale = this.getv1();
                ff9shadow.FF9ShadowSetScaleBattle((Int32)po.uid, xScale, zScale);
                return 0;
            case EBin.event_code_binary.SHADOWOFFSET:
                Int32 num1 = this.getv2();
                Int32 num2 = this.getv2();
                ff9shadow.FF9ShadowSetOffsetBattle((Int32)po.uid, (Single)num1, (Single)num2);
                return 0;
            case EBin.event_code_binary.SHADOWLOCK:
                Int32 num3 = this.getv1();
                ff9shadow.FF9ShadowLockYRotBattle((Int32)po.uid, (Single)(num3 << 4));
                return 0;
            case EBin.event_code_binary.SHADOWUNLOCK:
                ff9shadow.FF9ShadowUnlockYRotBattle((Int32)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWAMP:
                Int32 amp = this.getv1();
                ff9shadow.FF9ShadowSetAmpBattle((Int32)po.uid, amp);
                return 0;
            case EBin.event_code_binary.RAIN:
                Int32 num4 = this.getv1();
                this.getv1();
                FF9StateSystem.Common.FF9.btl_rain = (Byte)num4;
                return 0;
            default:
                return 1;
        }
    }
}