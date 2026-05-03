using System;
using UnityEngine;

public static class BGI
{
    public static Byte BGI_TRI_BITS_GET(UInt16 f)
    {
        return (Byte)(f >> 8);
    }

    public static Int64 BGI_charGetInfo(Int32 uid, ref Int16 triNdx, ref Int16 floorNdx)
    {
        Actor activeActorByUID = PersistenSingleton<EventEngine>.Instance.getActiveActorByUID(uid);
        if (activeActorByUID != null)
        {
            triNdx = (Int16)activeActorByUID.fieldMapActorController.activeTri;
            floorNdx = (Int16)activeActorByUID.fieldMapActorController.activeFloor;
        }
        return 1L;
    }

    public static Int64 BGI_charGetPitchAndRoll(Int32 uid, ref Int16 thetaX, ref Int16 thetaZ)
    {
        // TODO: Maybe actually use this to rotate the character (notably for the field "Memoria/World Fusion")
        Actor activeActorByUID = PersistenSingleton<EventEngine>.Instance.getActiveActorByUID(uid);
        if (activeActorByUID != null)
        {
            Int16 index = (Int16)activeActorByUID.fieldMapActorController.activeTri;
            BGI_TRI_DEF bgi_TRI_DEF = PersistenSingleton<EventEngine>.Instance.fieldmap.bgi.triList[(Int32)index];
            thetaX = (Int16)(-bgi_TRI_DEF.thetaZ);
            thetaZ = bgi_TRI_DEF.thetaX;
        }
        return 1L;
    }

    public static Single BGI_rcos(Int32 a)
    {
        Single f = (Single)a / 4096f * 360f * 0.0174532924f;
        Single num = Mathf.Cos(f);
        return num * 4096f;
    }

    public const UInt32 BGI_MAGIC = 2900156077u;

    public const Single BGI_PI = 3.14159274f;

    public const Int32 BGI_ANGLE_THRESHOLD = 20724;

    public const UInt16 BGI_CHAR_ACTIVE = 1;

    public const UInt16 BGI_CHAR_HEIGHT = 432;

    public const UInt16 BGI_CHAR_STEP_HEIGHT = 432;

    public const Single BGI_CHAR_DEFAULT_RAD = 96f;

    public const Single BGI_CHAR_DEFAULT_RADIUS = 9216f;

    public const Int32 BGI_CHAR_POS_MODIFIED = 1;

    public const Int32 BGI_CHAR_ON_FLOOR = 2;

    public const Int32 BGI_CHAR_IDLE = 4;

    public const Int32 BGI_CHAR_SLIDING = 8;

    public const Int32 BGI_CHAR_SLOPE_MODIFIED = 0x10;

    public const Int32 BGI_CHAR_NDX_OUT_OF_RANGE = 0x4000;

    public const Int32 BGI_CHAR_SYS_UNINITIALIZED = 0x8000;

    public const UInt16 BGI_FLOOR_ACTIVE = 1;

    public const UInt16 BGI_TRI_ACTIVE = 1;

    public const UInt16 BGI_TRI_ANIM_FRAME = 2;

    public const UInt16 BGI_TRI_SELECT = 0x80;

    public const UInt16 BGI_TRI_VISIT = 0x80;

    public const Int16 BGI_SIM_ACTIVE = 1;

    public const Int16 BGI_SIM_INIT = 2;

    public const Int16 BGI_SIM_PLAY_SET = 4;

    public const Int16 BGI_SIM_LOOP = 8;

    public const Int16 BGI_SIM_WRAP = 0x10;

    public const Int16 BGI_SIM_MAX = 0x1E;

    public const UInt16 BGI_SIM_LINEAR = 0;

    public const UInt16 BGI_SIM_SINUSOIDAL = 1;

    public const Int16 BGI_SIM_XAXIS = 0;

    public const Int16 BGI_SIM_YAXIS = 1;

    public const Int16 BGI_SIM_ZAXIS = 2;

    public const Byte BGI_ATTR_CPC = 0x80;

    public const Byte BGI_ATTR_NPC = 0x40;

    public const Byte BGI_ATTR_STEP1 = 0x20;

    public const Byte BGI_ATTR_STEP2 = 0x10;

    public const Byte BGI_ATTR_GROUND1 = 8;

    public const Byte BGI_ATTR_GROUND2 = 4;

    public const Byte BGI_ATTR_SPECIAL1 = 2;

    public const Byte BGI_ATTR_SPECIAL2 = 1;

    public const Int32 BGI_FORCE_MAX = 16;

    public const Int32 BGI_CHAR_MAX = 24;
}
