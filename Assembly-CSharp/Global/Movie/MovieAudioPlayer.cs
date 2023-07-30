using System;
using Memoria;

public class MovieAudioPlayer : MusicPlayer
{
	public SoundProfile GetActiveSoundProfile()
	{
		return this.activeSoundProfile;
	}

	public override Single Volume => FF9StateSystem.Settings.cfg.IsSoundEnabled ? (Configuration.VoiceActing.Enabled ? Configuration.VoiceActing.MovieVolume / 100f : Configuration.Audio.MusicVolume / 100f) : 0f;

}
