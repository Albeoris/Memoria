namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_ModifyEffModelAbr: unless <paramref name="a2"/> is 255, PSG slot <paramref name="a1"/> (48-byte entries at
    /// 0x180220230: +0 model, +32 in-use byte) must be in use with a model, otherwise the message is formatted into
    /// Buffer and the module hangs; then the model gets blend mode a2 &lt;&lt; 5 (<see cref="PsxObjectSetBlendMode"/>).
    /// </summary>
    public static void HiModifyEffModelAbr(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("HiModifyEffModelAbr(a1, a2)", a1, a2);
        if (a2 == 255)
        {
            return;
        }

        byte* slot = unk_180220230 + 48 * a1;
        long model = 0;
        if (slot[32] == 0 || (model = *(long*)slot) == 0)
        {
            WriteAsciiZ(
                Buffer,
                $"Hi_ModifyEffModelAbr () \n id:{a1.ToString(System.Globalization.CultureInfo.InvariantCulture)} can not use!");
            HiraishiFatalHang();
        }

        PsxObjectSetBlendMode(model, (ushort)(32 * a2));
    }
}
