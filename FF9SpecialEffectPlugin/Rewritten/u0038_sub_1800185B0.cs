using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Summon-model accessors (Hi_GetSummonBonePos, Hi_GetSummonBoneMatrix, Hi_Show/HideSummonModelMesh,
/// Hi_Start/StopSummonTexAnim). Model record: +32 hidden-mesh bit mask, +56 bone matrices (32 bytes each),
/// +112 texture animations (24 bytes each: +8 flags, +16 u32, +22 u16).
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Hi_GetSummonBonePos: a3[0..2] = low halves of the translation (+20/+24/+28) of bone a2; returns the bone table.</summary>
    public static long GetSummonBonePos(int a1, int a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("GetSummonBonePos(a1, a2, a3)", a1, a2, (nint)a3);
        long model = SummonModelOrHang(a1, "Hi_GetSummonBonePos () \n id:{0} can not use!");
        long bone = 32L * a2;
        a3[0] = *(ushort*)(bone + *(long*)(model + 56) + 20);
        a3[1] = *(ushort*)(bone + *(long*)(model + 56) + 24);
        long result = *(long*)(model + 56);
        a3[2] = *(ushort*)(bone + result + 28);
        return result;
    }

    /// <summary>Hi_GetSummonBoneMatrix: copies the 32-byte bone matrix a2 to a3; returns the bone table.</summary>
    public static long GetSummonBoneMatrix(int a1, int a2, void* a3)
    {
        Recompiler.Runtime.Tracer.Enter("GetSummonBoneMatrix(a1, a2, a3)", a1, a2, (nint)a3);
        long model = SummonModelOrHang(a1, "Hi_GetSummonBoneMatrix () \n id:{0} can not use!");
        long result = *(long*)(model + 56);
        long bone = 32L * a2;
        System.Buffer.MemoryCopy((void*)(bone + result), a3, 32, 32);
        return result;
    }

    /// <summary>Hi_ShowSummonModelMesh: clears bit a2 of the hidden-mesh mask; returns the mask applied.</summary>
    public static long ShowSummonModelMesh(int a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("ShowSummonModelMesh(a1, a2)", a1, a2);
        long model = SummonModelOrHang(a1, "Hi_ShowSummonModelMesh () \n id:{0} can not use!");
        uint mask = ~(1u << a2);
        *(uint*)(model + 32) &= mask;
        return mask;
    }

    /// <summary>Hi_HideSummonModelMesh: sets bit a2 of the hidden-mesh mask; returns the bit.</summary>
    public static long HideSummonModelMesh(int a1, sbyte a2)
    {
        Recompiler.Runtime.Tracer.Enter("HideSummonModelMesh(a1, a2)", a1, a2);
        long model = SummonModelOrHang(a1, "Hi_HideSummonModelMesh () \n id:{0} can not use!");
        uint bit = 1u << a2;
        *(uint*)(model + 32) |= bit;
        return bit;
    }

    /// <summary>Hi_StartSummonTexAnim: texture animation a2 flags |= (a3 != 0 ? 3 : 1), timer (+16) = 0, +22 = 4096.</summary>
    public static long StartSummonTexAnim(int a1, int a2, sbyte a3)
    {
        Recompiler.Runtime.Tracer.Enter("StartSummonTexAnim(a1, a2, a3)", a1, a2, a3);
        long model = SummonModelOrHang(a1, "Hi_StartSummonTexAnim () \n id:{0} can not use!");
        long entry = 24L * a2;
        byte* flags = (byte*)(*(long*)(model + 112) + entry + 8);
        *flags |= a3 != 0 ? (byte)3 : (byte)1;
        *(uint*)(*(long*)(model + 112) + entry + 16) = 0;
        *(ushort*)(*(long*)(model + 112) + entry + 22) = 4096;
        return 4096;
    }

    /// <summary>Hi_StopSummonTexAnim: clears the two low flag bits of texture animation a2; returns the table.</summary>
    public static long StopSummonTexAnim(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("StopSummonTexAnim(a1, a2)", a1, a2);
        long model = SummonModelOrHang(a1, "Hi_StopSummonTexAnim () \n id:{0} can not use!");
        long result = *(long*)(model + 112);
        *(byte*)(result + 24L * a2 + 8) &= 0xFC;
        return result;
    }

    /// <summary>Model of summon slot <paramref name="id"/>; an unused slot formats the message into Buffer and hangs.</summary>
    private static long SummonModelOrHang(int id, string format)
    {
        Recompiler.Runtime.Tracer.Enter("SummonModelOrHang(id, format)", id, format);
        var slot = (SummonSlot*)Unsafe.AsPointer(ref qword_180220830) + id;
        long model = 0;
        if (slot->inUse == 0 || (model = (long)slot->model) == 0)
        {
            WriteAnsi(Buffer, string.Format(System.Globalization.CultureInfo.InvariantCulture, format, id));
            HiraishiFatalHang();
        }

        return model;
    }
}
