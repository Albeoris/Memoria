using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// GTE MVMVA (RT * V0, no translation, sf=1, lm=0): MAC1..3 = (RT * V0) >> 12, IR1..3 = MAC saturated to signed
    /// 16 bit. The return value is a register leftover of the original, the constant 0xFFFF8000.
    /// </summary>
    public static long GteRtV0()
    {
        Recompiler.Runtime.Tracer.Enter("GteRtV0()");
        short* rt = (short*)xmmword_180211F40;
        short rt33 = (short)dword_180211F50;
        short* v = (short*)Unsafe.AsPointer(ref qword_180211FC0);

        int mac1 = unchecked(v[0] * rt[0] + v[2] * rt[2] + v[1] * rt[1]) >> 12;
        dword_180212024 = mac1;
        int mac2 = unchecked(v[0] * rt[3] + v[2] * rt[5] + v[1] * rt[4]) >> 12;
        dword_180212028 = mac2;
        int mac3 = unchecked(v[0] * rt[6] + v[2] * rt33 + v[1] * rt[7]) >> 12;
        dword_18021202C = mac3;

        *(short*)Unsafe.AsPointer(ref dword_180211FE4) = Saturate(mac1);
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        ir23[0] = Saturate(mac2);
        ir23[2] = Saturate(mac3);
        return 4294934528L;

        static short Saturate(int value) =>
            value > 0x7FFF ? (short)0x7FFF : value < -32768 ? short.MinValue : (short)value;
    }
}
