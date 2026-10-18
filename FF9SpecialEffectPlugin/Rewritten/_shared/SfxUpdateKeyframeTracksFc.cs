using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also EmitInterpolatedSpriteTrailSetFc (Rewritten/u0092_EmitInterpolatedSpriteTrail.cs).
    private static void SfxUpdateKeyframeTracksFc(int rfc, int gfc, int bfc)
    {
        Recompiler.Runtime.Tracer.Enter("SfxUpdateKeyframeTracksFc(rfc, gfc, bfc)", rfc, gfc, bfc);
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = rfc;
        *(int*)unk_180211F98 = gfc;
        dword_180211F9C = bfc;
    }
}
