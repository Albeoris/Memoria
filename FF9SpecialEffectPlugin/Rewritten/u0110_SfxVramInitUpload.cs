namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Assembles the 64x192 16-bit VRAM block at 0x1802208D0 from three .data templates (2048 + 8192 + 14336 bytes),
    /// points qword_1803231E0 / qword_180323260 at their tables, uploads the block at (768, 320) through the host,
    /// restores the 67584-byte page buffer from its template, uploads the four pages, uploads the 256x2 strip at
    /// (0, 240) and clears int[9] of the context at off_180066C68, which it returns.
    /// </summary>
    public static int* SfxVramInitUpload()
    {
        Recompiler.Runtime.Tracer.Enter("SfxVramInitUpload()");
        byte* block = unk_1802208D0;
        System.Buffer.MemoryCopy(unk_18004F020, block, 2048, 2048);
        System.Buffer.MemoryCopy(unk_180064C60, unk_1802210D0, 8192, 8192);
        System.Buffer.MemoryCopy(unk_180060060, unk_1802230D0, 14336, 14336);
        qword_1803231E0 = (long)unk_180063860;

        var upload = (BattleCallbackFn)BattleCallbackAddress;
        if (upload != null)
        {
            Native.InvokeBattleCallback((long)upload, HostCommand.LoadImage, 768, 320, 64, 192, (ulong)block);
        }

        dword_180220888 = 1;
        qword_180323260 = (long)unk_18004F820;
        System.Buffer.MemoryCopy(unk_18004F860, block, 67584, 67584);

        while (SfxVramPagesUploadOrRestore(1) == 0)
        {
        }

        upload = (BattleCallbackFn)BattleCallbackAddress;
        if (upload != null)
        {
            Native.InvokeBattleCallback((long)upload, HostCommand.LoadImage, 0, 240, 256, 2, (ulong)unk_1802308D0);
        }

        int* result = (int*)off_180066C68;
        result[9] = 0;
        return result;
    }
}
