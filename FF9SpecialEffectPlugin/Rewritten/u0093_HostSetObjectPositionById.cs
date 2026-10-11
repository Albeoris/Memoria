using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Object queries by context index (PsxGetContextQword returns the HostObject): host vector queries 0x01/0x02/0x03/
/// 0x0E/0x05/0x06/0x07 (each preceded by the 0x16 "host owns it" probe where the original has it), distance and size
/// fields, and the aim/depth helpers built on HostQueryObjPosition. Return values are the raw rax of the original.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Clears context word 3 (the active object's PSX address); returns the context table.</summary>
    public static int* ClearActiveObjectAddr()
    {
        Recompiler.Runtime.Tracer.Enter("ClearActiveObjectAddr()");
        int* context = (int*)off_180066C68;
        context[3] = 0;
        return context;
    }

    /// <summary>
    /// geo_radius (+56) of object <paramref name="a1"/> when it is player or bit 0x100 of a1 is set (then the
    /// object index is the low byte), else the u32 at +40.
    /// </summary>
    public static long ObjSelectDistanceById(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("ObjSelectDistanceById(a1)", a1);
        var obj = ObjSelectDistanceObject(a1, out bool forced);
        return obj->bi_player != 0 || forced ? obj->geo_radius : obj->enemy_radius;
    }

    /// <summary>geo_height (+60) of object <paramref name="a1"/>, zero-extended.</summary>
    public static long ObjSizeById(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("ObjSizeById(a1)", a1);
        return (uint)((HostObject*)PsxGetContextQword(a1))->geo_height;
    }

    /// <summary>
    /// Host query 0x01 | id into <paramref name="position"/>: sub-id 0 when the 0x16 probe answers nonzero for a
    /// enemy object, else sub-id 1. Returns the callback result (0 without a callback).
    /// </summary>
    public static long HostGetObjectPositionById(int objectIndex, long position)
    {
        Recompiler.Runtime.Tracer.Enter("HostGetObjectPositionById(objectIndex, position)", objectIndex, position);
        long obj = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        if ((int)HostObjCallback(ObjCommand(obj, HostCommand.GetSlave), 0, 0, 0, 0, 0) != 0 && ((HostObject*)obj)->bi_player == 0)
        {
            if (BattleCallbackAddress == null)
            {
                return 0;
            }

            return HostObjCallback(((HostObject*)obj)->HostId | HostCommand.GetPosition, 0, 0, 0, 0, (ulong)position);
        }

        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.GetPosition), 1, 0, 0, 0, (ulong)position);
    }

    /// <summary>HostQueryObjPosition for object <paramref name="a1"/>.</summary>
    public static long HostQueryObjPositionById(int a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryObjPositionById(a1, a2)", a1, a2);
        return HostQueryObjPosition(PsxGetContextQword(a1), a2);
    }

    /// <summary>
    /// Anchor position of object <paramref name="a1"/> into the s16 vector <paramref name="a3"/>. Host-owned (0x16 probe
    /// nonzero, not player): host 0x0E | id (0, byte +26, 0, 0, a3). Otherwise HostQueryObjPosition, then a player
    /// object lowers a3[1] by geo_height (halved when <paramref name="a2"/> is 0, minus 128 when the 0x200000 send succeeds);
    /// a enemy one asks 0x0E again and, for a2 == 1, stores the host's answer to 0x0E | id (1, byte +47) in a3[1].
    /// Returns al: the last callback/send result, or the low byte of the callback pointer.
    /// </summary>
    public static sbyte HostQueryObjAnchorPos(long a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryObjAnchorPos(a1, a2, a3)", a1, a2, a3);
        var obj = (HostObject*)a1;
        if (BattleCallbackAddress != null && (int)HostObjCallback(ObjCommand(a1, HostCommand.GetSlave), 0, 0, 0, 0, 0) != 0 && obj->bi_player == 0)
        {
            ulong kind = obj->tar_bone;
            if (BattleCallbackAddress == null)
            {
                return 0;
            }

            return (sbyte)HostObjCallback(obj->HostId | HostCommand.GetMatrix, 0, kind, 0, 0, (ulong)a3);
        }

        HostQueryObjPosition(a1, a3);
        if (obj->bi_player != 0)
        {
            int size = obj->geo_height;
            if (a2 == 0)
            {
                size >>= 1;
            }

            byte sent = HostCallbackSend(a1, 0x200000);
            if (sent != 0)
            {
                size -= 128;
            }

            *(ushort*)(a3 + 2) -= (ushort)size;
            return (sbyte)sent;
        }

        long callback = (long)BattleCallbackAddress;
        if (callback != 0)
        {
            HostObjCallback(obj->HostId | HostCommand.GetMatrix, 0, obj->tar_bone, 0, 0, (ulong)a3);
            callback = (long)BattleCallbackAddress;
        }

        if (a2 != 1)
        {
            return (sbyte)(byte)callback;
        }

        if (callback == 0)
        {
            *(ushort*)(a3 + 2) = 0;
            return 0;
        }

        long answer = Native.InvokeBattleCallback(callback, obj->HostId | HostCommand.GetMatrix, 1, *((byte*)obj + 47), 0, 0, 0);
        *(ushort*)(a3 + 2) = (ushort)answer;
        return (sbyte)answer;
    }

    /// <summary>HostQueryObjAnchorPos for object <paramref name="a1"/>.</summary>
    public static sbyte HostQueryObjAnchorPosById(int a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryObjAnchorPosById(a1, a2, a3)", a1, a2, a3);
        return HostQueryObjAnchorPos(PsxGetContextQword(a1), a2, a3);
    }

    /// <summary>
    /// Host query 0x02 | id into <paramref name="position"/> unless the 0x16 probe answers nonzero for a enemy object
    /// (then that answer is returned). 0 without a callback.
    /// </summary>
    public static long HostSetObjectPositionById(int objectIndex, long position)
    {
        Recompiler.Runtime.Tracer.Enter("HostSetObjectPositionById(objectIndex, position)", objectIndex, position);
        long obj = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        long probe = HostObjCallback(ObjCommand(obj, HostCommand.GetSlave), 0, 0, 0, 0, 0);
        if ((int)probe != 0 && ((HostObject*)obj)->bi_player == 0)
        {
            return probe;
        }

        if (BattleCallbackAddress == null)
        {
            return 0;
        }

        return HostObjCallback(ObjCommand(obj, HostCommand.SetPosition), 0, 0, 0, 0, (ulong)position);
    }

    /// <summary>Host query 0x03 | id (1, 0, 0, 0, a2) when a callback exists; then a2[1] += 2048. Returns 2048.</summary>
    public static long HostGetObjectRotationById(int objectIndex, long rotation)
    {
        Recompiler.Runtime.Tracer.Enter("HostGetObjectRotationById(objectIndex, rotation)", objectIndex, rotation);
        long obj = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress != null)
        {
            HostObjCallback(ObjCommand(obj, HostCommand.GetRotate), 1, 0, 0, 0, (ulong)rotation);
        }

        *(ushort*)(rotation + 2) += 2048;
        return 2048;
    }

    /// <summary>1 when object <paramref name="a1"/> is not player, else 0.</summary>
    public static long IsObjNotFlippedById(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("IsObjNotFlippedById(a1)", a1);
        return ((HostObject*)PsxGetContextQword(a1))->bi_player == 0 ? 1 : 0;
    }

    /// <summary>
    /// Scale of object <paramref name="objectIndex"/>: host flags 0x07 | id; bit 1 set → host query 0x05 | id into
    /// <paramref name="scale"/> (returns its result), else a2 = (4096, 4096, 4096) and the flags are returned.
    /// </summary>
    public static long HostQueryObjScaleById(int objectIndex, uint* scale)
    {
        Recompiler.Runtime.Tracer.Enter("HostQueryObjScaleById(objectIndex, scale)", objectIndex, (nint)scale);
        long obj = PsxGetContextQword(objectIndex);
        long callback = (long)BattleCallbackAddress;
        long flags = 0;
        if (callback != 0)
        {
            flags = HostObjCallback(ObjCommand(obj, HostCommand.GetGeoFlag), 0, 0, 0, 0, 0);
            callback = (long)BattleCallbackAddress;
        }

        if ((flags & 2) == 0)
        {
            scale[2] = 4096;
            scale[1] = 4096;
            scale[0] = 4096;
            return flags;
        }

        if (callback == 0)
        {
            return flags;
        }

        return Native.InvokeBattleCallback(callback, ObjCommand(obj, HostCommand.GetScale), 0, 0, 0, 0, (ulong)scale);
    }

    /// <summary>
    /// Sends the scale <paramref name="scale"/> of object <paramref name="objectIndex"/> with host 0x06 | id: (0, 0, 0, 0) for
    /// unit scale, else (0x81, x, y, z). Skipped when the 0x16 probe answers nonzero for a enemy object (returns
    /// that answer). Without a callback returns the object pointer (unit scale) or z.
    /// </summary>
    public static long HostSetObjScaleById(int objectIndex, uint* scale)
    {
        Recompiler.Runtime.Tracer.Enter("HostSetObjScaleById(objectIndex, scale)", objectIndex, (nint)scale);
        long obj = PsxGetContextQword(objectIndex);
        long callback = (long)BattleCallbackAddress;
        long result = obj;
        if (callback != 0)
        {
            result = HostObjCallback(ObjCommand(obj, HostCommand.GetSlave), 0, 0, 0, 0, 0);
            if ((int)result != 0 && ((HostObject*)obj)->bi_player == 0)
            {
                return result;
            }

            callback = (long)BattleCallbackAddress;
        }

        var send = (BattleCallbackFn)callback;
        uint x = scale[0];
        if (x == 0x1000 && scale[1] == 0x1000 && scale[2] == 0x1000)
        {
            if (callback == 0)
            {
                return result;
            }

            return Native.InvokeBattleCallback((long)send, ObjCommand(obj, HostCommand.SetScale), 0, 0, 0, 0, 0);
        }

        uint z = scale[2];
        uint y = scale[1];
        if (callback == 0)
        {
            return z;
        }

        return Native.InvokeBattleCallback((long)send, ObjCommand(obj, HostCommand.SetScale), 0x81, x, y, z, 0);
    }

    /// <summary>
    /// Point of view from object <paramref name="a2"/> towards object <paramref name="a1"/>: the same object returns
    /// its position in <paramref name="a3"/>. Otherwise yaw = -atan2(dx, dz) / 2π * -4096 and pitch =
    /// -1024 - (-atan2((int)sqrt(dx² + dz²) via GTE SQR, dy) / 2π * 4096) (d = a1 - a2) feed SfxRotTransPointZ from
    /// a2's position at ObjSelectDistanceById(a2).
    /// </summary>
    public static long ObjAimPointById(int a1, int a2, short* a3)
    {
        Recompiler.Runtime.Tracer.Enter("ObjAimPointById(a1, a2, a3)", a1, a2, (nint)a3);
        long target = PsxGetContextQword(a2);
        if (target == PsxGetContextQword(a1))
        {
            return HostQueryObjPosition(PsxGetContextQword(a1), (long)a3);
        }

        Span<short> from = stackalloc short[4];
        short* fromAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(from));
        Span<short> to = stackalloc short[4];
        short* toAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(to));
        HostQueryObjPosition(PsxGetContextQword(a1), (long)fromAddress);
        HostQueryObjPosition(PsxGetContextQword(a2), (long)toAddress);
        int dx = from[0] - to[0];
        int dz = from[2] - to[2];
        int toY = to[1];
        int fromY = from[1];
        int* ir = (int*)Unsafe.AsPointer(ref dword_180211FE4);
        ir[0] = dx;
        ir[1] = dz;
        GteSqr();

        double twoPi = BitConverter.Int64BitsToDouble(qword_18004B6A8);
        double yaw = -Math.Atan2(dx, dz);
        int horizontalSq = unchecked(dword_180212024 + dword_180212028);
        Span<short> angles = stackalloc short[4];
        short* anglesAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(angles));
        angles[2] = 0;
        angles[1] = (short)Recompiler.Runtime.X86.Cvttsd2si(yaw / twoPi * BitConverter.Int64BitsToDouble(qword_18004B6E0));
        double horizontal = Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt(horizontalSq));
        double pitch = -Math.Atan2(horizontal, fromY - toY);
        angles[0] = (short)(-1024 - Recompiler.Runtime.X86.Cvttsd2si(pitch / twoPi * BitConverter.Int64BitsToDouble(qword_18004B6C0)));

        var obj = ObjSelectDistanceObject(a2, out bool forced);
        int distance = (int)(obj->bi_player != 0 || forced ? obj->geo_radius : obj->enemy_radius);
        return SfxRotTransPointZ(toAddress, anglesAddress, distance, (ushort*)a3);
    }

    /// <summary>
    /// 1 when object <paramref name="a1"/> is not deeper than object <paramref name="a2"/> (z of the positions), negated
    /// (0xFFFFFFFF) when the context-word-11 camera's angle at +0x50 faces away ((angle - 1024) &amp; 0xFFF &gt; 0x800).
    /// </summary>
    public static long ObjDepthOrderSignById(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ObjDepthOrderSignById(a1, a2)", a1, a2);
        Span<short> first = stackalloc short[4];
        short* firstAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(first));
        Span<short> second = stackalloc short[4];
        short* secondAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(second));
        HostQueryObjPosition(PsxGetContextQword(a1), (long)firstAddress);
        HostQueryObjPosition(PsxGetContextQword(a2), (long)secondAddress);
        int sign = first[2] <= second[2] ? 1 : 0;
        byte* camera = ResolveGlobalPsxAddress(((uint*)off_180066C68)[11]);
        if (((*(short*)(camera + 80) - 1024) & 0xFFF) > 0x800)
        {
            sign = -sign;
        }

        return (uint)sign;
    }

    /// <summary><paramref name="a2"/> + geo_radius / 6 (unsigned) of object <paramref name="a1"/>, as u32.</summary>
    public static long ObjDistanceSixthPlusById(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("ObjDistanceSixthPlusById(a1, a2)", a1, a2);
        var obj = (HostObject*)PsxGetContextQword(a1);
        return (uint)(a2 + (int)(obj->geo_radius / 6));
    }

    /// <summary>Object of ObjSelectDistanceById: bit 0x100 of the id forces geo_radius and keeps only the low byte.</summary>
    private static HostObject* ObjSelectDistanceObject(int id, out bool forced)
    {
        Recompiler.Runtime.Tracer.Enter("ObjSelectDistanceObject(id)", id);
        forced = (id & 0x100) != 0;
        if (forced)
        {
            id = (byte)id;
        }

        return (HostObject*)PsxGetContextQword(id);
    }
}
