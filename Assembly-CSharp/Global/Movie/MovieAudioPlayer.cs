using Memoria;
using System;

public class MovieAudioPlayer : MusicPlayer
{
    public SoundProfile GetActiveSoundProfile()
    {
        return this.activeSoundProfile;
    }

    public override Single Volume => Configuration.Audio.MovieVolume / 100f;

}
