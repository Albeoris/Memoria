using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct Int32toSByteConverter
{
    public Int32toSByteConverter(Int32 value)
    {
        this.SByte1 = (this.SByte2 = (this.SByte3 = (this.SByte4 = 0)));
        this.Value = value;
    }

    public static implicit operator Int32(Int32toSByteConverter value)
    {
        return value.Value;
    }

    public static implicit operator Int32toSByteConverter(Int32 value)
    {
        return new Int32toSByteConverter(value);
    }

    [FieldOffset(0)]
    public Int32 Value;

    [FieldOffset(0)]
    public SByte SByte1;

    [FieldOffset(1)]
    public SByte SByte2;

    [FieldOffset(2)]
    public SByte SByte3;

    [FieldOffset(3)]
    public SByte SByte4;
}
