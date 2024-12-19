using System;
using UnityEngine;

public class FF9CFG
{
    public FF9CFG()
    {
        this.sound = 0UL;
        this.sound_effect = 0UL;
        this.control = 0UL;
        this.cursor = 0UL;
        this.atb = 1UL;
        this.camera = 0UL;
        this.move = 1UL; // Run by default
        this.vibe = FF9CFG_VIBE_ON; // Vibration ON by default
        this.btl_speed = 1UL;
        this.skip_btl_camera = 1UL;
        this.btl_msg = 3UL;
        this.fld_msg = 3UL;
        this.here_icon = 0UL;
        this.win_type = 0UL;
        this.target_win = 0UL;
        this.control_data = 0UL;
        this.control_data_keyboard = new KeyCode[HonoInputManager.DefaultInputKeysCount];
        this.control_data_joystick = new String[HonoInputManager.DefaultInputKeysCount];
        for (Int32 i = 0; i < (Int32)this.control_data_joystick.Length; i++)
        {
            this.control_data_joystick[i] = String.Empty;
        }
    }

    public UInt64 sound;

    public UInt64 sound_effect;

    public UInt64 control;

    public UInt64 cursor;

    public UInt64 atb;

    public UInt64 camera;

    public UInt64 move;

    public UInt64 vibe;

    public UInt64 btl_speed;

    public UInt64 skip_btl_camera;

    public UInt64 btl_msg;

    public UInt64 fld_msg;

    public UInt64 here_icon;

    public UInt64 win_type;

    public UInt64 target_win;

    public UInt64 control_data;

    public KeyCode[] control_data_keyboard;

    public String[] control_data_joystick;

    public static Byte FF9CFG_VIBE_ON;

    public static Byte FF9CFG_VIBE_OFF = 1;

    public Boolean IsMusicEnabled => sound == 0L;
    public Boolean IsSoundEnabled => sound_effect == 0L;
}
