using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// One motion step of the record <paramref name="a1"/> (host pointers are mapped to PSX addresses through
    /// PsxAddrMapperRegister on unk_180576A10, in the order a1, a3, a4). With speed s = s8 at a1+30 and the direction
    /// vector d of <paramref name="a3"/> (s16 x, y at +0 and z at +4): GPF computes MAC = (s &lt;&lt; 6) * d &gt;&gt; 12;
    /// the s16 vertical velocity at a1+16 grows by (a2 * s) &gt;&gt; 7; the 20.12 position ints at a1+0/+4/+8 grow by
    /// MAC1 &lt;&lt; 6, (velocity + MAC2) &lt;&lt; 6 and MAC3 &lt;&lt; 6. The integer parts (position &gt;&gt; 12) are stored as
    /// three s16 at <paramref name="a4"/>. Returns the z integer part as a u16.
    /// </summary>
    public static long SfxStepMotion(ulong a1, int a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxStepMotion(a1, a2, a3, a4)", a1, a2, a3, a4);
        long mapper = (long)unk_180576A10;
        uint record = (uint)PsxAddrMapperRegister(mapper, a1);
        uint direction = (uint)PsxAddrMapperRegister(mapper, a3);
        uint output = (uint)PsxAddrMapperRegister(mapper, a4);

        int speed = *(sbyte*)PsxAddrToPtr(mapper, record + 30);
        int dxy = *(int*)PsxAddrToPtr(mapper, direction);
        int accel = a2 * speed;
        int dz = *(short*)PsxAddrToPtr(mapper, direction + 4);
        dword_180211FE4 = (short)dxy;
        dword_180211FE0 = speed << 6;
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        ir23[0] = dxy >> 16;
        ir23[1] = dz;
        GteGpf();

        int velocity = (accel >> 7) + *(short*)PsxAddrToPtr(mapper, record + 16);
        *(ushort*)PsxAddrToPtr(mapper, record + 16) = (ushort)velocity;

        int x = *(int*)PsxAddrToPtr(mapper, record);
        int y = *(int*)PsxAddrToPtr(mapper, record + 4);
        int z = *(int*)PsxAddrToPtr(mapper, record + 8);
        int mac2 = dword_180212028;
        int mac3 = dword_18021202C;

        x += dword_180212024 << 6;
        *(int*)PsxAddrToPtr(mapper, record) = x;
        y += (velocity + mac2) << 6;
        *(int*)PsxAddrToPtr(mapper, record + 4) = y;
        z += mac3 << 6;
        *(int*)PsxAddrToPtr(mapper, record + 8) = z;

        *(ushort*)PsxAddrToPtr(mapper, output) = (ushort)(x >> 12);
        *(ushort*)PsxAddrToPtr(mapper, output + 2) = (ushort)(y >> 12);
        *(ushort*)PsxAddrToPtr(mapper, output + 4) = (ushort)(z >> 12);
        return (ushort)(z >> 12);
    }
}
