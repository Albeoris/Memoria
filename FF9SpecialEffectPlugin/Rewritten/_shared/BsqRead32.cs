using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also SpriteU32 (Rewritten/u0078_EmitSpriteBatch.cs).
    private static uint BsqRead32(uint address)
    {
        Recompiler.Runtime.Tracer.Enter("BsqRead32(address)", address);
        return *(uint*)ResolveGlobalPsxAddress(address);
    }
}
