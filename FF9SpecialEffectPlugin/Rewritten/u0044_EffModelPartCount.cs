namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_SplitMdlVertex: part count of the model in effect slot <paramref name="a1"/>; a slot not in use or without
    /// a model formats the message into Buffer and hangs.
    /// </summary>
    public static long EffModelPartCount(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("EffModelPartCount(a1)", a1);
        EffModelSlot* slot = EffModelSlotOrHang(a1, "Hi_SplitMdlVertex () \n id:{0} can not use!");
        return PsgHandlePartCount((long)slot->model);
    }
}
