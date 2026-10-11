using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// One motion step of a PSX-side particle record a1 (mapped through PsxAddrMapperRegister): position s32 x/y/z at
    /// +0/+4/+8 (20.12), velocity s16 x/y/z at +12/+14/+16, damping s16 at +18. GPF damps the velocity
    /// (IR0 = damping, IR = velocity, the 32-bit read at +16 also carries the damping word into IR3), the damped
    /// velocity is written back with a2 added to y, then GPL (IR0 = 4096) adds it to the position, and the s16 vector
    /// a3 is added shifted left by 6 (x += a3[2], y += a3[0], z += a3[1]). a4 receives the position &gt;&gt; 12 as
    /// s16 x/y/z. Returns the z result zero-extended from 16 bits.
    /// </summary>
    public static long ParticleStepMotion(ulong a1, short a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("ParticleStepMotion(a1, a2, a3, a4)", a1, a2, a3, a4);
        uint obj = (uint)PsxAddrMapperRegister((long)unk_180576A10, a1);
        uint add = (uint)PsxAddrMapperRegister((long)unk_180576A10, a3);
        uint output = (uint)PsxAddrMapperRegister((long)unk_180576A10, a4);

        int* ir2 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        int* ir3 = (int*)word_180211FEC;

        int damping = *(short*)ResolveGlobalPsxAddress(unchecked(obj + 18));
        int velocityXy = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 12));
        dword_180211FE0 = damping;
        dword_180211FE4 = (short)velocityXy;
        *ir2 = velocityXy >> 16;
        *ir3 = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 16));
        GteGpf();

        *ir2 = *(short*)ir2;
        int velocityX = (short)dword_180211FE4;
        dword_180211FE4 = velocityX;
        int velocityZ = *(short*)ir3;
        *ir3 = velocityZ;
        short velocityY = unchecked((short)(a2 + *(short*)ir2));
        dword_180211FE0 = 4096;

        dword_180212024 = *(int*)ResolveGlobalPsxAddress(obj);
        dword_180212028 = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 4));
        dword_18021202C = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 8));
        GteGplSf0();

        *(short*)ResolveGlobalPsxAddress(unchecked(obj + 12)) = (short)velocityX;
        *(short*)ResolveGlobalPsxAddress(unchecked(obj + 14)) = velocityY;
        *(short*)ResolveGlobalPsxAddress(unchecked(obj + 16)) = (short)velocityZ;
        int mac2 = dword_180212028;
        int mac3 = dword_18021202C;

        int addXy = *(int*)ResolveGlobalPsxAddress(add);
        int addZ = *(short*)ResolveGlobalPsxAddress(unchecked(add + 4));
        int x = unchecked(dword_180212024 + (addZ << 6));
        *(int*)ResolveGlobalPsxAddress(obj) = x;
        int y = unchecked(mac2 + ((short)addXy << 6));
        *(int*)ResolveGlobalPsxAddress(unchecked(obj + 4)) = y;
        int z = unchecked(mac3 + ((addXy >> 16) << 6));
        *(int*)ResolveGlobalPsxAddress(unchecked(obj + 8)) = z;

        *(short*)ResolveGlobalPsxAddress(output) = (short)(x >> 12);
        *(short*)ResolveGlobalPsxAddress(unchecked(output + 2)) = (short)(y >> 12);
        *(short*)ResolveGlobalPsxAddress(unchecked(output + 4)) = (short)(z >> 12);
        return (ushort)(z >> 12);
    }
}
