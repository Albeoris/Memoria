using System;

public static class EBG
{
    public const Byte EBG_OVERLAY_SCREEN_ANCHORED = 1;

    public const Byte EBG_OVERLAY_ACTIVE = 2;

    public const Byte EBG_OVERLAY_LOOP = 4;

    public const Byte EBG_OVERLAY_PARALLAX = 8;

    public const Byte EBG_OVERLAY_OFFSET_SCROLL = 128;

    public const Byte EBG_OVERLAY_LAYER = 16;

    public const Byte EBG_OVERLAY_ANIM_FRAME = 32;

    public const Byte EBG_OVERLAY_PALETTE_ANIM = 64;

    public const Int16 EBG_OVERLAY_LOOP_LOCK = 32767;

    public const Byte EBG_ANIM_INIT = 1;

    public const Byte EBG_ANIM_ACTIVE = 2;

    public const Byte EBG_ANIM_PLAY_SET = 4;

    public const Byte EBG_ANIM_LOOPING = 8;

    public const Byte EBG_ANIM_LOOP = 16;

    public const Byte EBG_ANIM_WRAP = 32;

    public const SByte EBG_ANIM_FRAME_STOP = -1;

    public const Int32 OVERLAY_ATTACH_MAX = 10;

    public const Byte EBG_STATE_SCROLL_STATUS_MASK = 7;

    public const Byte EBG_STATE_SCROLL_STATUS_IDLE = 0;

    public const Byte EBG_STATE_SCROLL_STATUS_INIT = 1;

    public const Byte EBG_STATE_SCROLL_STATUS_RUNNING = 2;

    public const Byte EBG_STATE_SCROLL_STATUS_RELEASE = 3;

    public const Byte EBG_STATE_SCROLL_STATUS_DONE = 4;

    public const Byte EBG_STATE_SCROLL_TYPE_MASK = 8;

    public const Byte EBG_STATE_SCROLL_TYPE_LINEAR = 0;

    public const Byte EBG_STATE_SCROLL_TYPE_EASE = 8;

    public const Byte EBG_STATE_SCROLL_RELEASE_TIME = 30;

    public const Byte EBG_STATE_VRPSTATUS_MASK = 224;

    public const Byte EBG_STATE_VRPSTATUS_LOCKED = 32;

    public const Byte EBG_STATE_VRPSTATUS_ACTIVE = 64;

    public const Byte EBG_STATE_VRPSTATUS_INACTIVE = 0;

    public const Byte EBG_STATE_VRPSTATUS_UPDATE = 128;

    public const Int16 EBG_STATE_CHAR_AIM_HEIGHT = 324;

    public const Int16 EBG_DEPTH_MIN = 8;

    public const Int16 EBG_DEPTH_MAX = 4088;

    public const Byte EBG_VIEWPORT_COUNT = 4;
}
