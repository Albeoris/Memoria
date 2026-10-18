using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Fetches the position (a2, 3 shorts) and orientation (a3, 3 shorts) of the anchor selected by the low 5 bits
    /// of a1, plus a distance bias (a4). Ids 21..31 read a fixed table; 11 reads a stored position; the others ask
    /// the host callback (0x03/0x16/0x01 messages) about a HostObject and finish with HostCallbackSend(obj, 0x200000).
    /// </summary>
    public static void GetAnchorTransform(sbyte a1, long a2, long a3, uint* a4)
    {
        Recompiler.Runtime.Tracer.Enter("GetAnchorTransform(a1, a2, a3, a4)", a1, a2, a3, (nint)a4);
        Shape_GetAnchorTransform_a2* shape_GetAnchorTransform_a2 = (Shape_GetAnchorTransform_a2*)a2;
        Shape_GetAnchorTransform_a2* shape_GetAnchorTransform_a3 = (Shape_GetAnchorTransform_a2*)a3;
        ushort* pos = (ushort*)a2;
        ushort* rot = (ushort*)a3;
        shape_GetAnchorTransform_a2->union_0_4 = 0;
        pos[2] = 0;
        shape_GetAnchorTransform_a3->union_0_4 = 0;
        rot[2] = 0;
        *a4 = 0;

        int id = a1 & 0x1F;
        if (id == 0)
        {
            return;
        }

        if (id > 20)
        {
            ushort* entry = (ushort*)(unk_180220060 + 8 * (id - 21));
            pos[0] = entry[0];
            pos[1] = entry[1];
            pos[2] = entry[2];
            if (id - 21 < 4)
            {
                rot[0] = entry[24];
                rot[1] = entry[25];
                rot[2] = entry[26];
            }

            return;
        }

        if (id == 11)
        {
            pos[0] = (ushort)word_18006971C;
            pos[2] = (ushort)word_18006971E;
            return;
        }

        short* defaults = (short*)unk_18021FF4A;
        long obj;
        switch (id)
        {
            case 1:
                obj = qword_18021FFF0;
                break;
            case 10:
                obj = qword_180220130;
                if (obj == 0)
                {
                    return;
                }

                break;
            case 12:
                obj = CameraCaster;
                break;
            case 15:
                obj = qword_180220058;
                defaults = (short*)unk_18021FF54;
                break;
            default:
                obj = 0;
                break;
        }

        bool alternate = (a1 & 0x20) != 0;
        if (obj != 0)
        {
            var callback = HostCallbackPtr;
            if (callback != null)
            {
                Native.InvokeBattleCallback((long)callback, ((HostObject*)obj)->HostId | HostCommand.GetRotate, (ulong)(alternate ? 2 : 1), 0, 0, 0, unchecked((ulong)a3));
            }
        }
        else
        {
            defaults = (short*)unk_18021FF5E;
            if (id == 5 || id == 9)
            {
                obj = qword_18021FFF8;
            }

            if ((uint)(id - 13) <= 1)
            {
                obj = CameraTarget;
            }

            if (obj == 0)
            {
                obj = BattleObjectList;
            }

            var callback = HostCallbackPtr;
            if (callback != null)
            {
                ulong code = obj != 0 ? ((HostObject*)obj)->HostId | HostCommand.GetRotate : HostCommand.GetRotate;
                Native.InvokeBattleCallback((long)callback, code, 3, 0, 0, 0, unchecked((ulong)a3));
            }

            // The original dereferences the object without a null check here.
            rot[1] = (ushort)(((HostObject*)obj)->bi_player != 0 ? 2048 : 0);
        }

        HostObject* host = (HostObject*)obj;
        var query = HostCallbackPtr;
        bool answered = query != null && Native.InvokeBattleCallback((long)query, host->HostId | HostCommand.GetSlave, 0, 0, 0, 0, 0) != 0;
        if (answered)
        {
            pos[0] = (ushort)defaults[3];
            pos[2] = (ushort)defaults[4];
        }
        else
        {
            var fetch = HostCallbackPtr;
            if (fetch != null)
            {
                Native.InvokeBattleCallback((long)fetch, host->HostId | HostCommand.GetPosition, (ulong)(alternate ? 3 : 2), 0, 0, 0, unchecked((ulong)a2));
            }
        }

        if (id == 9)
        {
            pos[0] = (ushort)word_18021FFD8;
            pos[2] = (ushort)word_18021FFDC;
        }
        else if (id == 14)
        {
            pos[0] = (ushort)CameraTargetX;
            pos[2] = (ushort)CameraTargetZ;
        }

        uint bias = host->geo_radius;
        pos[1] = 0;
        *a4 = bias;
        switch (a1 & 0xC0)
        {
            case 0:
                pos[1] = (ushort)defaults[1];
                break;
            case 0x40:
                pos[1] = (ushort)defaults[2];
                break;
            case 0xC0:
                pos[1] = (ushort)defaults[0];
                break;
        }

        HostCallbackSend(obj, 0x200000u);
    }

    /// <summary>
    /// Computes a world position (a4, 3 ushorts) at distance a3[3] (+ bias) and angles a3[0]/a3[1] (4096 = full turn,
    /// fixed point 12) around the anchor a1, optionally averaged with the anchor a2; adds the anchor roll to a3[2].
    /// </summary>
    public static long ComputeOrbitPosition(long a1, sbyte a2, short* a3, ushort* a4)
    {
        Recompiler.Runtime.Tracer.Enter("ComputeOrbitPosition(a1, a2, a3, a4)", a1, a2, (nint)a3, (nint)a4);
        const double Scale = 4096.8;
        const double Pi = 3.141592653589793;
        sbyte anchor = (sbyte)a1;
        Span<short> pos = stackalloc short[4];
        short* posAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(pos));
        Span<ushort> rot = stackalloc ushort[4];
        ushort* rotAddress = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rot));
        uint bias;
        GetAnchorTransform(anchor, (long)posAddress, (long)rotAddress, &bias);

        short x;
        short y;
        short z;
        if ((a2 & 0x1F) != 0)
        {
            Span<ushort> pos2 = stackalloc ushort[4];
            ushort* pos2Address = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(pos2));
            byte* rot2 = stackalloc byte[8];
            uint bias2;
            GetAnchorTransform((sbyte)((a2 & 0x3F) | (anchor & 0xC0)), (long)pos2Address, (long)rot2, &bias2);
            x = (short)((short)(pos[0] + pos2[0]) >> 1);
            y = (short)((short)(pos[1] + pos2[1]) >> 1);
            z = (short)((short)(pos[2] + pos2[2]) >> 1);
        }
        else
        {
            x = pos[0];
            y = pos[1];
            z = pos[2];
        }

        int distance = a2 >= 0 ? a3[3] + (int)bias : a3[3];
        int pitch = a3[0];
        long result;
        if ((a2 & 0x40) != 0)
        {
            int horizontal = (distance * (int)(Math.Cos((double)pitch / Scale * 2.0 * Pi) * Scale)) >> 12;
            a4[0] = (ushort)((horizontal * (int)(Math.Sin((double)a3[1] / Scale * 2.0 * Pi) * -Scale)) >> 12);
            a4[2] = (ushort)((horizontal * (int)(Math.Cos((double)a3[1] / Scale * 2.0 * Pi) * -Scale)) >> 12);
            result = (uint)((distance * (int)(Math.Sin((double)a3[0] / Scale * 2.0 * Pi) * Scale)) >> 12);
            a4[1] = (ushort)result;
        }
        else
        {
            double c = Math.Cos((double)(rot[0] + pitch) / Scale * 2.0 * Pi);
            int id = anchor & 0x1F;
            int yaw = rot[1] + a3[1];
            long obj = 0;
            int horizontal = (distance * (int)(c * Scale)) >> 12;
            if (id == 5 || id == 9)
            {
                obj = qword_18021FFF8;
            }

            if ((uint)(id - 13) <= 1)
            {
                obj = CameraTarget;
            }

            if (obj != 0 && ((HostObject*)obj)->bi_player != 0)
            {
                yaw ^= 0xFFF;
            }

            double angle = (double)yaw / Scale * 2.0 * Pi;
            a4[0] = (ushort)((horizontal * (int)(Math.Sin(angle) * -Scale)) >> 12);
            a4[2] = (ushort)((horizontal * (int)(Math.Cos(angle) * -Scale)) >> 12);
            a4[1] = (ushort)((distance * (int)(Math.Sin((double)(rot[0] + a3[0]) / Scale * 2.0 * Pi) * Scale)) >> 12);
            result = rot[2];
            a3[2] += (short)rot[2];
        }

        a4[0] += (ushort)x;
        a4[1] += (ushort)y;
        a4[2] += (ushort)z;
        return result;
    }

    /// <summary>The host callback installed by SFX_InitSystem, read from the shared module data (qword_1801C1DE8).</summary>
    private static BattleCallbackFn HostCallbackPtr =>
        (BattleCallbackFn)BattleCallbackAddress;
}
