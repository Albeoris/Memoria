using System.Runtime.InteropServices;

namespace Recompiler.Runtime.Com;

/// <summary><c>VARTYPE</c> values used by <see cref="PropVariant"/>.</summary>
public enum VarType : ushort
{
    Empty = 0,
    Null = 1,
    I2 = 2,
    I4 = 3,
    R4 = 4,
    R8 = 5,
    Bstr = 8,
    Error = 10,
    Bool = 11,
    I1 = 16,
    UI1 = 17,
    UI2 = 18,
    UI4 = 19,
    I8 = 20,
    UI8 = 21,
    Int = 22,
    UInt = 23,
    FileTime = 64,
    Lpstr = 30,
    Lpwstr = 31,
}

/// <summary>
/// Blittable <c>PROPVARIANT</c> (16 bytes on x64) as used by property-based COM interfaces such as
/// 7-Zip's <c>IInArchive::GetProperty</c>. Only the scalar/BSTR members are modelled.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
public unsafe struct PropVariant
{
    [FieldOffset(0)] public VarType Type;
    [FieldOffset(2)] public ushort Reserved1;
    [FieldOffset(4)] public ushort Reserved2;
    [FieldOffset(6)] public ushort Reserved3;
    [FieldOffset(8)] public sbyte I1;
    [FieldOffset(8)] public byte UI1;
    [FieldOffset(8)] public short I2;
    [FieldOffset(8)] public ushort UI2;
    [FieldOffset(8)] public int I4;
    [FieldOffset(8)] public uint UI4;
    [FieldOffset(8)] public long I8;
    [FieldOffset(8)] public ulong UI8;
    [FieldOffset(8)] public float R4;
    [FieldOffset(8)] public double R8;
    [FieldOffset(8)] public short BoolValue;
    [FieldOffset(8)] public char* BstrValue;
    [FieldOffset(8)] public void* Pointer;

    public bool IsEmpty => Type == VarType.Empty;

    public static PropVariant FromUInt32(uint value) => new() { Type = VarType.UI4, UI4 = value };
    public static PropVariant FromUInt64(ulong value) => new() { Type = VarType.UI8, UI8 = value };
    public static PropVariant FromBool(bool value) => new() { Type = VarType.Bool, BoolValue = value ? (short)-1 : (short)0 };

    /// <summary>Allocates a BSTR copy of <paramref name="value"/>; the receiver frees it with <c>PropVariantClear</c>/<c>SysFreeString</c>.</summary>
    public static PropVariant FromString(string value) => new()
    {
        Type = VarType.Bstr,
        BstrValue = (char*)Marshal.StringToBSTR(value),
    };

    /// <summary>Releases owned memory (BSTR) and resets the variant to <see cref="VarType.Empty"/>.</summary>
    public void Clear()
    {
        if (Type == VarType.Bstr && BstrValue is not null)
        {
            Marshal.FreeBSTR((nint)BstrValue);
        }

        this = default;
    }
}
