using System;
using Memoria;

public class MovieAudioPlayer : MusicPlayer
{
	public SoundProfile GetActiveSoundProfile()
	{
		return this.activeSoundProfile;
	}

	public override Single Volume => Configuration.Audio.MovieVolume / 100f;

}
