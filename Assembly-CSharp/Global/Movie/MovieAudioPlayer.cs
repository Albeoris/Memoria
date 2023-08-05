using System;
using Memoria;

public class MovieAudioPlayer : MusicPlayer
{
	public SoundProfile GetActiveSoundProfile()
	{
		return this.activeSoundProfile;
	}

	public override Single Volume => Configuration.VoiceActing.Enabled ? Configuration.VoiceActing.MovieVolume / 100f : (FF9StateSystem.Settings.cfg.IsMusicEnabled ? Configuration.Audio.MusicVolume / 100f : 0f);

}
