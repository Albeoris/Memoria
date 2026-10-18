namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_FreeEffModel: clears the in-use flag of effect-model slot <paramref name="a1"/> (no bound check). When the
    /// slot is not in use or has no model, formats "Hi_FreeEffModel()\nid:%d can not free!" into Buffer and hangs in
    /// HiraishiFatalHang. Returns the slot table address.
    /// </summary>
    public static void* FreeEffModelSlot(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("FreeEffModelSlot(a1)", a1);
        var slot = (EffModelSlot*)unk_180220230 + a1;
        if (slot->inUse == 0 || slot->model == 0)
        {
            WriteAnsi(Buffer, $"Hi_FreeEffModel()\nid:{a1} can not free!");
            HiraishiFatalHang();
        }

        slot->inUse = 0;
        return unk_180220230;
    }

    /// <summary>sprintf-equivalent store of an ASCII message plus the terminating NUL.</summary>
    private static void WriteAnsi(byte* destination, string text)
    {
        Recompiler.Runtime.Tracer.Enter("WriteAnsi(destination, text)", (nint)destination, text);
        for (int i = 0; i < text.Length; i++)
        {
            destination[i] = (byte)text[i];
        }

        destination[text.Length] = 0;
    }
}
