using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also SaturateColor (Rewritten/u0086_sub_180004CD0.cs).
    private static byte IntplSatU8(int value)
    {
        Recompiler.Runtime.Tracer.Enter("IntplSatU8(value)", value);
        return value > 255 ? (byte)255 : value < 0 ? (byte)0 : (byte)value;
    }
}
