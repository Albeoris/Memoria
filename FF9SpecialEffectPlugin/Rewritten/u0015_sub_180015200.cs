namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_DebugPSGData: PSG slot <paramref name="a1"/> (48-byte entries at 0x180220230: +0 model, +32 in-use byte) must be
    /// in use with a model, otherwise the message is formatted into Buffer and the module hangs. Builds a
    /// <see cref="PsgUvBounds"/> on the stack (mins 0xFFFF, maxes 0, SkipBounds = a2) and runs
    /// <see cref="AccumulatePsgUvBounds"/> for every part (count = byte +3 of the header at the model's PSX address +8).
    /// Returns the last helper result, or the value the address resolution left in rax when there are no parts.
    /// </summary>
    public static long HiDebugPsgData(int a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("HiDebugPsgData(a1, a2)", a1, a2);
        byte* slot = unk_180220230 + 48 * a1;
        long model = 0;
        if (slot[32] == 0 || (model = *(long*)slot) == 0)
        {
            WriteAsciiZ(Buffer, $"Hi_DebugPSGData()\nid:{a1.ToString(System.Globalization.CultureInfo.InvariantCulture)} cannot use!");
            HiraishiFatalHang();
        }

        PsgUvBounds bounds = default;
        bounds.SkipBounds = (byte)a2;
        bounds.Id = a1;
        ushort* minU = (ushort*)((ushort*)&bounds.MinU);
        ushort* minV = (ushort*)((ushort*)&bounds.MinV);
        ushort* maxU = (ushort*)((ushort*)&bounds.MaxU);
        ushort* maxV = (ushort*)((ushort*)&bounds.MaxV);
        for (int i = 0; i < 8; i++)
        {
            minU[i] = 0xFFFF;
            minV[i] = 0xFFFF;
            maxU[i] = 0;
            maxV[i] = 0;
        }

        uint address = *(uint*)(model + 8);
        long result;
        byte* header = ResolvePsgHeader(address, out result);
        int parts = header[3];
        if (parts != 0)
        {
            int part = 0;
            do
            {
                result = AccumulatePsgUvBounds(*(long*)slot, (byte)part++, (long)&bounds);
            }
            while (part < parts);
        }

        return result;
    }

    /// <summary>
    /// The inlined PsxAddrToPtr of HiDebugPsgData, also reporting the value it leaves in rax (returned by the original
    /// when the model has no parts): the masked offset for RAM, 32 * segment for a segment, the scratchpad base for the
    /// scratchpad. Null results are dereferenced by the caller, like the original.
    /// </summary>
    private static byte* ResolvePsgHeader(uint address, out long rax)
    {
        Recompiler.Runtime.Tracer.Enter("ResolvePsgHeader(address)", address);
        rax = 0;
        if (address == 0)
        {
            return null;
        }

        uint segment = address >> 24;
        if (segment == 128)
        {
            rax = address & 0xFFFFFFF;
            if ((uint)rax < 0x200000)
            {
                return (byte*)(qword_1805789E0 + address - (uint)dword_1805789D8);
            }
        }
        else
        {
            rax = address & 0xC00000;
            if ((uint)rax == 0xC00000)
            {
                rax = 32L * segment;
                return (byte*)(*(long*)(unk_180576A18 + rax) + (address & 0x3FFFFF));
            }
        }

        uint offset = address - 0x1F800000u;
        if (offset <= 0x3FF)
        {
            rax = (long)unk_1805789E8;
            return unk_1805789E8 + offset;
        }

        return null;
    }

    /// <summary>sprintf into a fixed char buffer: ASCII bytes of <paramref name="text"/> plus the terminating NUL.</summary>
    private static void WriteAsciiZ(byte* destination, string text)
    {
        Recompiler.Runtime.Tracer.Enter("WriteAsciiZ(destination, text)", (nint)destination, text);
        for (int i = 0; i < text.Length; i++)
        {
            destination[i] = (byte)text[i];
        }

        destination[text.Length] = 0;
    }
}
