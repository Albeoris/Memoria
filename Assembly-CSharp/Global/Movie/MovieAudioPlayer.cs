using System;

public class MovieAudioPlayer : MusicPlayer
{
	public void PlayMusic(String soundName, Int32 fadeIn = 0)
	{
		Int32 soundIndex = SoundMetaData.GetSoundIndex(soundName, SoundProfileType.MovieAudio);
		SoundLib.Log("PlayMuvieAudio movieName: " + soundName);
		if (soundIndex != -1)
		{
			base.PlayMusic(soundIndex, fadeIn, SoundProfileType.MovieAudio);
		}
	}

	public SoundProfile GetActiveSoundProfile()
	{
		return this.activeSoundProfile;
	}
}
