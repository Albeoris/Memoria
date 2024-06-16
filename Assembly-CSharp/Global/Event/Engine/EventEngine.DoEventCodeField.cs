using System;

public partial class EventEngine
{
    public Int32 DoEventCodeField(PosObj po, Int32 code)
    {
        switch ((EBin.event_code_binary)code)
        {
            case EBin.event_code_binary.SHADOWON: // 0x7F, "EnableShadow", "Enable the shadow for the entry's object."
                ff9shadow.FF9ShadowOnField((Int32)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWOFF: // 0x80, "DisableShadow", "Disable the shadow for the entry's object."
                ff9shadow.FF9ShadowOffField((Int32)po.uid);
                po.isShadowOff = true;
                return 0;
            case EBin.event_code_binary.SHADOWSCALE: // 0x81, "SetShadowSize", "Set the entry's object shadow size", true, 2, { 1, 1 }, { "Size X", "Size Y" }, { AT_SPIN, AT_SPIN }, 0
                Int32 xScale = this.getv1(); // arg1: size X
                Int32 zScale = this.getv1(); // arg2: size Z
                ff9shadow.FF9ShadowSetScaleField((Int32)po.uid, xScale, zScale);
                return 0;
            case EBin.event_code_binary.SHADOWOFFSET: // 0x82, "SetShadowOffset", "Change the offset between the entry's object and its shadow...", true, 2, { 2, 2 }, { "Offset X", "Offset Y" }, { AT_SPIN, AT_SPIN }, 0
                Int32 xOffset = this.getv2(); // arg1: offset X
                Int32 zOffset = this.getv2(); // arg2: offset Z
                ff9shadow.FF9ShadowSetOffsetField((Int32)po.uid, (Single)xOffset, (Single)zOffset);
                return 0;
            case EBin.event_code_binary.SHADOWLOCK: // 0x83, "LockShadowRotation", "Stop updating the shadow rotation by the object's rotation", true, 1, { 1 }, { "Locked Rotation" }, { AT_SPIN }, 0
                Int32 rotY = this.getv1(); // arg1: locked rotation
                ff9shadow.FF9ShadowLockYRotField((Int32)po.uid, (Single)(rotY << 4));
                return 0;
            case EBin.event_code_binary.SHADOWUNLOCK: // 0x84, "UnlockShadowRotation", "Make the shadow rotate accordingly with its object."
                ff9shadow.FF9ShadowUnlockYRotField((Int32)po.uid);
                return 0;
            case EBin.event_code_binary.SHADOWAMP: // 0x85, "SetShadowAmplifier", "Amplify or reduce the shadow transparancy", true, 1, { 1 }, { "Amplification Factor" }, { AT_USPIN }, 0
                Int32 ampFactor = this.getv1(); // arg1: amplification factor
                ff9shadow.FF9ShadowSetAmpField((Int32)po.uid, (Int32)(Byte)ampFactor);
                return 0;
            case EBin.event_code_binary.RAIN: // 0xD8, "SetWeather", "Add a raining effect (works on the world maps, fields and in battles)", true, 2, { 1, 1 }, { "Strength", "Speed" }, { AT_USPIN, AT_USPIN }, 0
                Int32 strength = this.getv1(); // arg1: strength of rain
                Int32 speed = this.getv1(); // arg2: speed of rain (unused for a battle rain)
                this.fieldmap.rainRenderer.SetRainParam(strength, speed);
                this._ff9.btl_rain = (Byte)strength;
                return 0;
            default:
                return 1;
        }
    }
}
