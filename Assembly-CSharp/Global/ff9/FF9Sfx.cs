using System;

public class FF9Sfx
{
    public static void FF9SFX_Play(Int32 id)
    {
        if (!FF9Sfx.muteSfx)
        {
            FF9Snd.ff9snd_sndeffect_play(id, 8388608, 127, 128);
        }
    }

    public static void FF9SFX_PlayLoop(Int32 id)
    {
        if (!FF9Sfx.muteSfx)
        {
            FF9Snd.ff9snd_sndeffect_play(id, 0, 127, 128);
        }
    }

    public static void FF9SFX_Stop(Int32 id)
    {
        FF9Snd.ff9snd_sndeffect_stop(id, 8388608);
    }

    public static void FF9SFX_StopLoop(Int32 id)
    {
        FF9Snd.ff9snd_sndeffect_stop(id, 0);
    }

    public static Boolean muteSfx;
}
