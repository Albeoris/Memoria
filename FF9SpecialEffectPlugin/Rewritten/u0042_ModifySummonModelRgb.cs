namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Effect-model and summon-model modifiers (Hi_ModifyEffModelRGB, Hi_SetEffModelOffset, Hi_SetEffModelSlice,
/// Hi_ModifySummonModelAbr, Hi_ModifySummonModelRGB). An unused slot formats its message into Buffer and hangs.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Hi_ModifyEffModelRGB: tints every primitive of the model in effect slot <paramref name="a1"/>.</summary>
    public static void ModifyEffModelRgb(int a1, ushort a2, ushort a3, ushort a4)
    {
        Recompiler.Runtime.Tracer.Enter("ModifyEffModelRgb(a1, a2, a3, a4)", a1, a2, a3, a4);
        EffModelSlot* slot = EffModelSlotOrHang(a1, "Hi_ModifyEffModelRGB () \n id:{0} can not use!");
        TintObjectPrims((long)slot->model, a2, a3, a4);
    }

    /// <summary>Hi_SetEffModelOffset: stores <paramref name="a2"/> in the u16 at +38 of the slot; returns the slot table.</summary>
    public static void* SetEffModelOffset(int a1, short a2)
    {
        Recompiler.Runtime.Tracer.Enter("SetEffModelOffset(a1, a2)", a1, a2);
        EffModelSlot* slot = EffModelSlotOrHang(a1, "Hi_SetEffModelOffset () \n id:{0} can not use!");
        *(short*)((byte*)slot + 38) = a2;
        return unk_180220230;
    }

    /// <summary>Hi_SetEffModelSlice: enables slicing and stores <paramref name="a2"/> at +40; returns the slot table.</summary>
    public static void* SetEffModelSlice(int a1, short a2)
    {
        Recompiler.Runtime.Tracer.Enter("SetEffModelSlice(a1, a2)", a1, a2);
        EffModelSlot* slot = EffModelSlotOrHang(a1, "Hi_SetEffModelSlice () \n id:{0} can not use!");
        slot->sliceEnabled = 1;
        slot->field_40 = (ushort)a2;
        return unk_180220230;
    }

    /// <summary>Hi_ModifySummonModelAbr: blend mode 32 * <paramref name="a2"/> for the summon model; 255 leaves it unchanged.</summary>
    public static void ModifySummonModelAbr(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ModifySummonModelAbr(a1, a2)", a1, a2);
        if (a2 != 255)
        {
            long model = SummonModelOrHang(a1, "Hi_ModifySummonModelAbr () \n id:{0} can not use!");
            PsxObjectSetBlendMode(model, (ushort)(32 * a2));
        }
    }

    /// <summary>Hi_ModifySummonModelRGB: tints every primitive of the model in summon slot <paramref name="a1"/>.</summary>
    public static void ModifySummonModelRgb(int a1, ushort a2, ushort a3, ushort a4)
    {
        Recompiler.Runtime.Tracer.Enter("ModifySummonModelRgb(a1, a2, a3, a4)", a1, a2, a3, a4);
        long model = SummonModelOrHang(a1, "Hi_ModifySummonModelRGB () \n id:{0} can not use!");
        TintObjectPrims(model, a2, a3, a4);
    }

    /// <summary>Effect slot <paramref name="id"/> (no bound check); not in use or without a model formats the message into Buffer and hangs.</summary>
    private static EffModelSlot* EffModelSlotOrHang(int id, string format)
    {
        Recompiler.Runtime.Tracer.Enter("EffModelSlotOrHang(id, format)", id, format);
        var slot = (EffModelSlot*)unk_180220230 + id;
        if (slot->inUse == 0 || slot->model == 0)
        {
            WriteAnsi(Buffer, string.Format(System.Globalization.CultureInfo.InvariantCulture, format, id));
            HiraishiFatalHang();
        }

        return slot;
    }
}
