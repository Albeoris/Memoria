namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Scale of the trigonometric curves (.rdata qword_18004B6C8 = 0x40B000CCCCCCCCCD).</summary>
    private const double SfxCurveScale = 4096.8;

    /// <summary>
    /// Fixed-point (1.0 = 4096) curve value of progress a1 / a2: mode 0 linear (a1 &lt;&lt; 12) / a2; modes 1 and 2
    /// take x = (a1 &lt;&lt; 11) / a2 and return 4096 - (int)(cos(x / 4096.8 * 2 * pi) * 4096.8) and
    /// (int)(sin(x / 4096.8 * 2 * pi) * 4096.8); any other mode returns 0. Results are zero-extended 32-bit values.
    /// </summary>
    public static long SfxCurveValue(int a1, int a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxCurveValue(a1, a2, a3)", a1, a2, a3);
        if (a3 == 0)
        {
            return (uint)((a1 << 12) / a2);
        }

        if (a3 == 1)
        {
            double angle = (double)((a1 << 11) / a2) / SfxCurveScale * 2.0 * Math.PI;
            return (uint)(4096 - (int)(Math.Cos(angle) * SfxCurveScale));
        }

        if (a3 == 2)
        {
            double angle = (double)((a1 << 11) / a2) / SfxCurveScale * 2.0 * Math.PI;
            return (uint)(int)(Math.Sin(angle) * SfxCurveScale);
        }

        return 0;
    }
}
