using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Scaled motion step of a PSX-side particle record with its damping s16 at +32; see
    /// <see cref="ParticleStepScaledMotion32Body"/>.
    /// </summary>
    public static long ParticleStepScaledMotion32(ulong a1, int a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("ParticleStepScaledMotion32(a1, a2, a3, a4)", a1, a2, a3, a4);
        return ParticleStepScaledMotion32Body(a1, a2, a3, a4, 32);
    }

    /// <summary>
    /// Shared body of ParticleStepScaledMotion32/42 (the two originals differ only in the damping offset).
    /// Record a1: position s32 x/y/z at +0/+4/+8 (20.12), velocity s16 x/y/z at +12/+14/+16, s8 scale at +30,
    /// damping s16 at <paramref name="dampingOffset"/>. GPF damps the velocity (the 32-bit read at +16 carries the
    /// following word into IR3), the damped velocity is written back with (a2 * scale) &gt;&gt; 7 added to y, GPL with
    /// IR0 = 64 adds it to the position; then GPF with IR0 = scale &lt;&lt; 6 scales the s16 vector a3 and the result
    /// &lt;&lt; 6 is added to the position. a4 receives the position &gt;&gt; 12 as s16 x/y/z; returns the z result
    /// zero-extended from 16 bits.
    /// </summary>
    private static long ParticleStepScaledMotion32Body(ulong a1, int a2, ulong a3, ulong a4, uint dampingOffset)
    {
        Recompiler.Runtime.Tracer.Enter("ParticleStepScaledMotion32Body(a1, a2, a3, a4, dampingOffset)", a1, a2, a3, a4, dampingOffset);
        uint obj = (uint)PsxAddrMapperRegister((long)unk_180576A10, a1);
        uint add = (uint)PsxAddrMapperRegister((long)unk_180576A10, a3);
        uint output = (uint)PsxAddrMapperRegister((long)unk_180576A10, a4);

        int* ir2 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        int* ir3 = (int*)word_180211FEC;

        int damping = *(short*)ResolveGlobalPsxAddress(unchecked(obj + dampingOffset));
        int velocityXy = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 12));
        dword_180211FE0 = damping;
        dword_180211FE4 = (short)velocityXy;
        *ir2 = velocityXy >> 16;
        *ir3 = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 16));
        int scale = *(sbyte*)ResolveGlobalPsxAddress(unchecked(obj + 30));
        GteGpf();

        int velocityX = (short)dword_180211FE4;
        dword_180211FE4 = velocityX;
        int velocityY = *(short*)ir2;
        *ir2 = velocityY;
        dword_180211FE0 = 64;
        int velocityZ = *(short*)ir3;
        *ir3 = velocityZ;
        int pushedY = unchecked(((a2 * scale) >> 7) + velocityY);

        dword_180212024 = *(int*)ResolveGlobalPsxAddress(obj);
        dword_180212028 = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 4));
        dword_18021202C = *(int*)ResolveGlobalPsxAddress(unchecked(obj + 8));
        GteGplSf0();

        *(short*)ResolveGlobalPsxAddress(unchecked(obj + 12)) = (short)velocityX;
        *(short*)ResolveGlobalPsxAddress(unchecked(obj + 14)) = (short)pushedY;
        *(short*)ResolveGlobalPsxAddress(unchecked(obj + 16)) = (short)velocityZ;
        int mac1 = dword_180212024;
        int mac2 = dword_180212028;
        int mac3 = dword_18021202C;

        int addXy = *(int*)ResolveGlobalPsxAddress(add);
        *ir3 = *(short*)ResolveGlobalPsxAddress(unchecked(add + 4));
        dword_180211FE0 = scale << 6;
        dword_180211FE4 = (short)addXy;
        *ir2 = addXy >> 16;
        GteGpf();

        int x = unchecked(mac1 + (dword_180212024 << 6));
        *(int*)ResolveGlobalPsxAddress(obj) = x;
        int y = unchecked(mac2 + (dword_180212028 << 6));
        *(int*)ResolveGlobalPsxAddress(unchecked(obj + 4)) = y;
        int z = unchecked(mac3 + (dword_18021202C << 6));
        *(int*)ResolveGlobalPsxAddress(unchecked(obj + 8)) = z;

        *(short*)ResolveGlobalPsxAddress(output) = (short)(x >> 12);
        *(short*)ResolveGlobalPsxAddress(unchecked(output + 2)) = (short)(y >> 12);
        *(short*)ResolveGlobalPsxAddress(unchecked(output + 4)) = (short)(z >> 12);
        return (ushort)(z >> 12);
    }
}
