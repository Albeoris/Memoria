namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    // Shared helper (recomp fn dedupe-helpers): the same body was also PsgPsxPtr (Rewritten/u0022_PsgDrawMeshPart.cs), SplitPsxPtr (Rewritten/u0034_sub_18000CA80.cs), BsqPtr (Rewritten/u0076_BuildSpriteQuads.cs), SfxFramePsxPtr (Rewritten/u0107_SFX_Update_0.cs).
    /// <summary>
    /// The inlined PsxAddrToPtr over the global PSX context at 0x180576A10 (segment table 0x180576A18, RAM PSX base
    /// 0x1805789D8, RAM host pointer 0x1805789E0, scratchpad 0x1805789E8); null for 0 or an unmapped address.
    /// </summary>
    private static byte* ResolveGlobalPsxAddress(uint address)
    {
        Recompiler.Runtime.Tracer.Enter("ResolveGlobalPsxAddress(address)", address);
        return (byte*)PsxAddrToPtr((long)unk_180576A10, address);
    }
}
