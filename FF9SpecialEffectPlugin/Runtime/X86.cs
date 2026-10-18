namespace Recompiler.Runtime;

/// <summary>
/// Instruction-level semantics the C# casts do not reproduce. The decompiler prints <c>(int)someDouble</c> for
/// <c>cvttsd2si</c>, but a .NET cast saturates while the instruction returns the "integer indefinite" value
/// (<c>0x80000000</c> / <c>0x8000000000000000</c>) for NaN and out-of-range inputs; a port that must match the
/// original bit for bit calls these instead.
/// </summary>
public static class X86
{
    /// <summary><c>cvttsd2si r32, xmm</c>: truncating double to int32, integer indefinite on NaN or overflow.</summary>
    public static int Cvttsd2si(double value) =>
        double.IsNaN(value) || value >= 2147483648.0 || value <= -2147483649.0 ? int.MinValue : (int)value;

    /// <summary><c>cvttsd2si r64, xmm</c>: truncating double to int64, integer indefinite on NaN or overflow.</summary>
    public static long Cvttsd2si64(double value) =>
        double.IsNaN(value) || value >= 9223372036854775808.0 || value <= -9223372036854777856.0 ? long.MinValue : (long)value;

    /// <summary><c>cvttss2si r32, xmm</c>: truncating float to int32, integer indefinite on NaN or overflow.</summary>
    public static int Cvttss2si(float value) =>
        float.IsNaN(value) || value >= 2147483648.0f || value <= -2147483904.0f ? int.MinValue : (int)value;

    /// <summary><c>cvttss2si r64, xmm</c>: truncating float to int64, integer indefinite on NaN or overflow.</summary>
    public static long Cvttss2si64(float value) =>
        float.IsNaN(value) || value >= 9223372036854775808.0f || value <= -9223373136366403584.0f ? long.MinValue : (long)value;

    /// <summary><c>cvtsd2si</c> with the default MXCSR rounding (round to nearest even), integer indefinite on NaN or overflow.</summary>
    public static int Cvtsd2si(double value) =>
        double.IsNaN(value) || value >= 2147483647.5 || value <= -2147483648.5 ? int.MinValue : (int)Math.Round(value, MidpointRounding.ToEven);
}
