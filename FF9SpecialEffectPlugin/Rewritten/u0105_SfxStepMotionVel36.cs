using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// <see cref="SfxStepMotion"/> with the s16 vertical velocity at a1+36 instead of a1+16: maps a1, a3, a4 through
    /// PsxAddrMapperRegister; speed s8 at a1+30 scales the direction of <paramref name="a3"/> through GPF; velocity
    /// += (a2 * speed) &gt;&gt; 7; 20.12 position at a1+0/+4/+8 += MAC1 &lt;&lt; 6, (velocity + MAC2) &lt;&lt; 6,
    /// MAC3 &lt;&lt; 6; position &gt;&gt; 12 stored as three s16 at <paramref name="a4"/>. Returns the z part as a u16.
    /// </summary>
    public static long SfxStepMotionVel36(ulong a1, int a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxStepMotionVel36(a1, a2, a3, a4)", a1, a2, a3, a4);
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

        int velocity = (accel >> 7) + *(short*)PsxAddrToPtr(mapper, record + 36);
        *(ushort*)PsxAddrToPtr(mapper, record + 36) = (ushort)velocity;

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
