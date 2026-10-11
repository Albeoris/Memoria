namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Scaled motion step of a PSX-side particle record with its damping s16 at +42; the original is an instruction
    /// clone of ParticleStepScaledMotion32 apart from that offset, see <see cref="ParticleStepScaledMotion32Body"/>.
    /// </summary>
    public static long ParticleStepScaledMotion42(ulong a1, int a2, ulong a3, ulong a4)
    {
        Recompiler.Runtime.Tracer.Enter("ParticleStepScaledMotion42(a1, a2, a3, a4)", a1, a2, a3, a4);
        return ParticleStepScaledMotion32Body(a1, a2, a3, a4, 42);
    }
}
