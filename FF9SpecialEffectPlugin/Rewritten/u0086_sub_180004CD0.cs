using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// GTE DPCT emulation (depth cue colour triple) over the emulated GTE registers in module data: for each of the
    /// three FIFO colours RGB0 (0x180212010): MAC = ((C &lt;&lt; 16) + IR0 * (FC - 16*C)) &gt;&gt; 12 with far colour
    /// RFC/GFC/BFC (0x180211F94/98/9C) and IR0 (low word of 0x180211FE0); IR1..3 = MAC saturated to int16; the FIFO
    /// shifts (RGB0 = RGB1, RGB1 = RGB2) and RGB2 = MAC/16 saturated to 0..255 with CODE from 0x180211FDB.
    /// FLAG is not updated. Returns the saturated blue of the last colour.
    /// </summary>
    public static long GteDpct()
    {
        Recompiler.Runtime.Tracer.Enter("GteDpct()");
        byte* rgb0 = (byte*)Unsafe.AsPointer(ref dword_180212010);
        byte* rgb2 = (byte*)Unsafe.AsPointer(ref dword_180212018);
        int fifoNext = dword_180212014;
        long result = 0;
        for (int pass = 0; pass < 3; pass++)
        {
            int ir0 = (short)dword_180211FE0;
            byte r = rgb0[0];
            byte g = rgb0[1];
            byte b = rgb0[2];
            long mac1 = ((long)(r << 16) + ir0 * (long)(int)(qword_180211F94 - 16 * r)) >> 12;
            dword_180212024 = (int)mac1;
            // movsxd r8,eax at 0x180004D59 sign-extends the 32-bit green-channel difference.
            long mac2 = ((long)(g << 16) + ir0 * (long)unchecked(*(int*)unk_180211F98 - 16 * g)) >> 12;
            dword_180212028 = (int)mac2;
            long mac3 = ((long)(b << 16) + ir0 * (long)(dword_180211F9C - 16 * b)) >> 12;
            dword_18021202C = (int)mac3;

            *(short*)Unsafe.AsPointer(ref dword_180211FE4) = IntplSatS16((int)mac1);
            *(short*)Unsafe.AsPointer(ref qword_180211FE8) = IntplSatS16((int)mac2);
            short ir3 = IntplSatS16((int)mac3);

            dword_180212010 = fifoNext;
            fifoNext = dword_180212018;
            *(short*)unk_180211FEC = ir3;

            rgb2[0] = IntplSatU8((int)mac1 >> 4);
            rgb2[1] = IntplSatU8((int)mac2 >> 4);
            int blue = (int)mac3 >> 4;
            result = blue > 255 ? 255 : blue < 0 ? 0 : (uint)blue;
            rgb2[2] = (byte)result;
            rgb2[3] = *unk_180211FDB;
        }

        dword_180212014 = fifoNext;
        return result;
    }
}
