using System;

public static class SharedSerializerEvent
{
	public static void WillAutosave()
	{
		try
		{
			if (FF9Snd.HasJustChangedBetweenWorldAndField)
			{
				FF9StateSystem.Sound.auto_save_bgm_id = -1;
			}
			else
			{
				FF9StateSystem.Sound.auto_save_bgm_id = FF9Snd.GetCurrentMusicId();
			}
			FF9Snd.HasJustChangedBetweenWorldAndField = false;
		}
		catch (Exception arg)
		{
			ISharedDataLog.LogWarning("GetCurrentMusicId: Exception: " + arg);
		}
	}

	public static void WillAutosaveDidParse()
	{
		FF9StateSystem.Sound.auto_save_bgm_id = -1;
	}

	public static void DidAutoload()
	{
		if (FF9StateSystem.Sound.auto_save_bgm_id != -1)
		{
			if (FF9StateSystem.Settings.cfg.sound == 0UL)
			{
				SoundLib.EnableMusic();
			}
			else
			{
				SoundLib.DisableMusic();
			}
			AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
			allSoundDispatchPlayer.FF9SOUND_SONG_PLAY(FF9StateSystem.Sound.auto_save_bgm_id, 127, 0);
			allSoundDispatchPlayer.FF9SOUND_SONG_VOL_FADE(FF9StateSystem.Sound.auto_save_bgm_id, 30, 0, 127);
			FF9StateSystem.Sound.auto_save_bgm_id = -1;
		}
	}
}
