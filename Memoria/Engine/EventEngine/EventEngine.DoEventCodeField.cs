public partial class EventEngine
{
    public int DoEventCodeField(PosObj po, int code)
    {
        switch ((EBin.event_code_binary)code)
        {
            case EBin.event_code_binary.SHADOWON:
                ff9shadow.FF9ShadowOnField((int)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWOFF:
                ff9shadow.FF9ShadowOffField((int)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWSCALE:
                int xScale = this.getv1();
                int zScale = this.getv1();
                ff9shadow.FF9ShadowSetScaleField((int)po.uid, xScale, zScale);
                return 0;
            case EBin.event_code_binary.SHADOWOFFSET:
                int num1 = this.getv2();
                int num2 = this.getv2();
                ff9shadow.FF9ShadowSetOffsetField((int)po.uid, (float)num1, (float)num2);
                return 0;
            case EBin.event_code_binary.SHADOWLOCK:
                int num3 = this.getv1();
                ff9shadow.FF9ShadowLockYRotField((int)po.uid, (float)(num3 << 4));
                return 0;
            case EBin.event_code_binary.SHADOWUNLOCK:
                ff9shadow.FF9ShadowUnlockYRotField((int)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWAMP:
                int num4 = this.getv1();
                ff9shadow.FF9ShadowSetAmpField((int)po.uid, (int)(byte)num4);
                return 0;
            case EBin.event_code_binary.RAIN:
                int strength = this.getv1();
                int speed = this.getv1();
                this.fieldmap.rainRenderer.SetRainParam(strength, speed);
                this._ff9.btl_rain = (byte)strength;
                return 0;
            default:
                return 1;
        }
    }
}