using System;

public partial class EventEngine
{
    public Int32 DoEventCodeWorld(PosObj po, Int32 code)
    {
        Int32 num1;
        Int32 num2;
        switch ((EBin.event_code_binary)code)
        {
            case EBin.event_code_binary.SHADOWON:
                return 0;
            case EBin.event_code_binary.SHADOWOFF:
                return 0;
            case EBin.event_code_binary.SHADOWSCALE:
                num1 = this.getv1();
                num2 = this.getv1();
                return 0;
            case EBin.event_code_binary.SHADOWOFFSET:
                num1 = this.getv2();
                num2 = this.getv2();
                return 0;
            case EBin.event_code_binary.SHADOWLOCK:
                num1 = this.getv1();
                return 0;
            case EBin.event_code_binary.SHADOWUNLOCK:
                return 0;
            case EBin.event_code_binary.SHADOWAMP:
                num1 = this.getv1();
                return 0;
            case EBin.event_code_binary.RAIN:
                Int32 strength = this.getv1();
                Int32 speed = this.getv1();
                ff9.rainRenderer.SetRainParam(strength, speed);
                this._ff9.btl_rain = (Byte)strength;
                return 0;
            default:
                return 1;
        }
    }
}