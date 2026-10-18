using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also SaturateS16 (Rewritten/u0012_sub_1800032C0.cs), GteOpClampIr (Rewritten/u0050_ComputeLocalYawDistance.cs), SaturateIr (Rewritten/u0086_sub_180004CD0.cs).
    private static short IntplSatS16(int value)
    {
        Recompiler.Runtime.Tracer.Enter("IntplSatS16(value)", value);
        return value > 0x7FFF ? (short)0x7FFF : value < -32768 ? short.MinValue : (short)value;
    }
}
