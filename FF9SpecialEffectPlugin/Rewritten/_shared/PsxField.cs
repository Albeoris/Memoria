using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also SpriteField (Rewritten/u0074_sub_18001FA80.cs).
    /// <summary>Host pointer of the PSX address <paramref name="address"/> + <paramref name="offset"/> (32-bit wrap).</summary>
    private static byte* PsxField(uint address, uint offset)
    {
        Recompiler.Runtime.Tracer.Enter("PsxField(address, offset)", address, offset);
        return ResolveGlobalPsxAddress(unchecked(address + offset));
    }
}
