using Recompiler.Runtime;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Loads size bytes of the effect binary, seeds the random generator, resolves the caster and targets from
    /// the 72-byte S_Eff_Req_Org, resets the work area and starts effnum's sequencer.
    /// Memoria pins both buffers for the duration of this call; the effect bytes are copied into module storage.
    /// Evidence: ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L1975-L1986.
    /// </summary>
    public static void SFX_Play_0(int effnum, byte* bin, int size, S_Eff_Req_Org* req)
    {
        Tracer.Enter("SFX_Play_0(effnum, bin, size, req)", effnum, (nint)bin, size, (nint)req);
        if (size != 0)
        {
            System.Buffer.MemoryCopy(bin, unk_1803678F0, (long)size, (long)size);
        }

        Ports.Random.Seed((uint)Ports.Clock.UtcNow.ToUnixTimeSeconds());
        CurrentEffectId = effnum;
        EffectFrameIndex = 0;
        SfxResolveActors(req);
        SfxResetAndRegisterGlobals();
        ((uint*)off_180066C68)[3] = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)unk_180211E80);
        SfxStateReset();
        SeqStateBegin((short)effnum);
    }
}
