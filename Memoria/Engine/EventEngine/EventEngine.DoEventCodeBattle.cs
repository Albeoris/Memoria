public partial class EventEngine
{
    public int DoEventCodeBattle(PosObj po, int code)
    {
        switch ((EBin.event_code_binary)code)
        {
            case EBin.event_code_binary.SHADOWON:
                ff9shadow.FF9ShadowOnBattle((int)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWOFF:
                ff9shadow.FF9ShadowOffBattle((int)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWSCALE:
                int xScale = this.getv1();
                int zScale = this.getv1();
                ff9shadow.FF9ShadowSetScaleBattle((int)po.uid, xScale, zScale);
                return 0;
            case EBin.event_code_binary.SHADOWOFFSET:
                int num1 = this.getv2();
                int num2 = this.getv2();
                ff9shadow.FF9ShadowSetOffsetBattle((int)po.uid, (float)num1, (float)num2);
                return 0;
            case EBin.event_code_binary.SHADOWLOCK:
                int num3 = this.getv1();
                ff9shadow.FF9ShadowLockYRotBattle((int)po.uid, (float)(num3 << 4));
                return 0;
            case EBin.event_code_binary.SHADOWUNLOCK:
                ff9shadow.FF9ShadowUnlockYRotBattle((int)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWAMP:
                int amp = this.getv1();
                ff9shadow.FF9ShadowSetAmpBattle((int)po.uid, amp);
                return 0;
            case EBin.event_code_binary.RAIN:
                int num4 = this.getv1();
                this.getv1();
                FF9StateSystem.Common.FF9.btl_rain = (byte)num4;
                return 0;
            default:
                return 1;
        }
    }
}