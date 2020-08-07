using System;
using System.Collections.Generic;
using Memoria;
using UnityEngine;
using Object = System.Object;
using MLog = Memoria.Prime.Log;

public class SoundLib : MonoBehaviour
{
	public static void PlayMovieMusic(String movieName, Int32 offsetTimeMSec = 0)
	{
		if (SoundLib.instance == (UnityEngine.Object)null)
		{
		    LogWarning("The instance is null.");
			return;
		}
		if (SoundLib.MusicIsMute)
		{
            return;
		}
		Int32 movieSoundIndex = SoundLib.GetMovieSoundIndex(movieName);
		if (movieSoundIndex != -1)
		{
			SoundLib.movieAudioPlayer.PlayMusic(movieSoundIndex, offsetTimeMSec, SoundProfileType.MovieAudio);
		}
		else
		{
			SoundLib.LogError(movieName + " not found!");
		}
	}

	public static SoundProfile GetActiveMovieAudioSoundProfile()
	{
		return SoundLib.movieAudioPlayer.GetActiveSoundProfile();
	}

	public static Int32 GetMovieSoundIndex(String movieName)
	{
		String soundName = String.Empty;
		if (String.Equals(movieName, "FMV000"))
		{
			soundName = "Sounds01/BGM_/music033";
		}
		else if (String.Equals(movieName, "FMV059"))
		{
			soundName = "Sounds02/Movie_/FMV059A";
		}
		else if (String.Equals(movieName, "FMV060"))
		{
			String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
			String str = "FMV059C";
			if (currentLanguage == "Japanese")
			{
				str = "FMV059B";
			}
			soundName = "Sounds02/Movie_/" + str;
		}
		else if (String.Equals(movieName, "mbg102"))
		{
			soundName = "Sounds02/song_/song0504_0";
		}
		else if (String.Equals(movieName, "mbg103"))
		{
			soundName = "Sounds02/song_/song0505_0";
		}
		else if (String.Equals(movieName, "mbg105"))
		{
			soundName = "Sounds02/song_/song0503_0";
		}
		else if (String.Equals(movieName, "mbg106"))
		{
			soundName = "Sounds02/song_/song0507_0";
		}
		else if (String.Equals(movieName, "mbg107"))
		{
			soundName = "Sounds02/song_/song0506_0";
		}
		else if (String.Equals(movieName, "mbg108"))
		{
			soundName = "Sounds02/song_/song0501_0";
		}
		else if (String.Equals(movieName, "mbg110"))
		{
			soundName = "Sounds02/song_/song0502_0";
		}
		else if (String.Equals(movieName, "mbg111"))
		{
			soundName = "Sounds02/song_/song0509_0";
		}
		else if (String.Equals(movieName, "mbg112"))
		{
			soundName = "Sounds02/song_/song0510_0";
		}
		else if (String.Equals(movieName, "mbg113"))
		{
			soundName = "Sounds02/song_/song0508_0";
		}
		else if (String.Equals(movieName, "mbg115"))
		{
			soundName = "Sounds02/song_/song0511_0";
		}
		else if (String.Equals(movieName, "mbg116"))
		{
			soundName = "Sounds02/song_/song1040_0";
		}
		else if (String.Equals(movieName, "mbg117"))
		{
			soundName = "Sounds02/song_/song0512_0";
		}
		else if (String.Equals(movieName, "mbg118"))
		{
			soundName = "Sounds02/song_/song0513_0";
		}
		else
		{
			soundName = "Sounds02/Movie_/" + movieName;
		}
		return SoundMetaData.GetSoundIndex(soundName, SoundProfileType.MovieAudio);
	}

	public static void PauseMovieMusic(String movieName)
	{
		if (SoundLib.instance == (UnityEngine.Object)null)
		{
			return;
		}
		SoundLib.movieAudioPlayer.PauseMusic();
	}

	public static void StopMovieMusic(String movieName, Boolean isForceStop = false)
	{
		if (SoundLib.instance == (UnityEngine.Object)null)
		{
			return;
		}
		if (!isForceStop)
		{
			if (String.Equals(movieName, "FMV000"))
			{
				SoundLib.Log("Don't stop sound for " + movieName + " if it is NOT forced to stop.");
			}
			else
			{
				SoundLib.movieAudioPlayer.StopMusic();
			}
		}
		else if (String.Equals(movieName, "FMV000"))
		{
			Int32 ticks = 90;
			Int32 fadeOut = AllSoundDispatchPlayer.ConvertTickToMillisec(ticks);
			SoundLib.movieAudioPlayer.StopMusic(fadeOut);
		}
		else
		{
			SoundLib.movieAudioPlayer.StopMusic();
		}
	}

	public static void AddNewSound(String fileName, Int32 soundId, AudioSource source)
	{
		SoundLib.Log("No implementation");
	}

	public static void SeekMovieAudio(String movieName, Single time)
	{
		SoundLib.movieAudioPlayer.SeekActiveSound((Int32)(time * 1000f));
	}

	public static void SeekMusic(Single time)
	{
		SoundLib.musicPlayer.SeekActiveSound((Int32)(time * 1000f));
	}

	public static void LoadMovieResources(String basePath, String[] movies)
	{
		SoundLib.Log("No implementation");
	}

	public static void UnloadMovieResources()
	{
		SoundLib.Log("No implementation");
	}

	public static void Log(Object message)
	{
        //MLog.Message("[SoundLib] " + message);
    }

	public static void LogError(Object message)
	{
        MLog.Error("[SoundLib] " + message);
    }

	public static void LogWarning(Object message)
	{
	    MLog.Warning("[SoundLib] " + message);
	}

    public static Boolean MusicIsMute { get; private set; }

	public static Boolean SoundEffectIsMute { get; private set; }

	public static void LoadGameSoundEffect(String jsonMetaData)
	{
		try
		{
			SoundLib.soundEffectPlayer.LoadGameSoundEffect(jsonMetaData);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void LoadSceneSoundEffect(String jsonMetaData)
	{
		try
		{
			SoundLib.soundEffectPlayer.LoadSceneSoundEffect(jsonMetaData);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void UnloadSoundEffect()
	{
		try
		{
			SoundLib.soundEffectPlayer.UnloadSoundEffect();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void UnloadAllSoundEffect()
	{
		try
		{
			SoundLib.soundEffectPlayer.UnloadAllSoundEffect();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static Boolean IsSoundEffectPlaying(Int32 soundIndex)
	{
		try
		{
			return SoundLib.soundEffectPlayer.IsSoundEffectPlaying(soundIndex);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
		return false;
	}

	public static void StopSoundEffect(Int32 soundIndex)
	{
		try
		{
			SoundLib.soundEffectPlayer.StopSoundEffect(soundIndex);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void StopAllSoundEffects()
	{
		try
		{
			SoundLib.soundEffectPlayer.StopAllSoundEffects();
			SoundLib.songPlayer.StopAllSoundEffects();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void PlaySoundEffect(Int32 soundIndex, Single soundVolume = 1f, Single panning = 0f, Single pitch = 1f)
	{
		try
		{
			SoundLib.soundEffectPlayer.PlaySoundEffect(soundIndex, soundVolume, panning, pitch);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static Int32 GetResidentSfxSoundCount()
	{
		return SoundMetaData.ResidentSfxSoundIndex[0].Count;
	}

	public static void LoadAllResidentSfxSoundData()
	{
		try
		{
			SoundLib.sfxSoundPlayer.LoadAllResidentSoundData();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void UnloadAllResidentSfxSoundData()
	{
		try
		{
			SoundLib.sfxSoundPlayer.UnloadAllResidentSoundData();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void LoadSfxSoundData(Int32 specialEffectID)
	{
		try
		{
			SoundLib.sfxSoundPlayer.LoadSoundData(specialEffectID);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static SoundProfile PlaySfxSound(Int32 soundIndexInSpecialEffect, Single soundVolume = 1f, Single panning = 0f, Single pitch = 1f)
	{
		SoundProfile result;
		try
		{
			result = SoundLib.sfxSoundPlayer.PlaySfxSound(soundIndexInSpecialEffect, soundVolume, panning, pitch);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
			result = (SoundProfile)null;
		}
		return result;
	}

	public static Boolean IsSfxSoundPlaying(Int32 soundIndexInSpecialEffect)
	{
		Boolean result;
		try
		{
			result = SoundLib.sfxSoundPlayer.IsPlaying(soundIndexInSpecialEffect);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
			result = false;
		}
		return result;
	}

	public static void StopSfxSound(Int32 soundIndexInSpecialEffect)
	{
		try
		{
			SoundLib.sfxSoundPlayer.StopSound(soundIndexInSpecialEffect);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void StopAllSfxSound()
	{
		try
		{
			SoundLib.sfxSoundPlayer.StopAllSounds();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void PlaySong(Int32 soundIndex)
	{
		try
		{
			SoundLib.songPlayer.PlaySong(soundIndex, 1f, 0f, 1f);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void StopSong(Int32 soundIndex)
	{
		try
		{
			SoundLib.songPlayer.StopSong(soundIndex);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void StopAllSongs()
	{
		try
		{
			SoundLib.songPlayer.StopAllSongs();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void PlaySong(Int32 soundIndex, Single soundVolume, Single panning, Single pitch)
	{
		try
		{
			SoundLib.songPlayer.PlaySong(soundIndex, soundVolume, panning, pitch);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void LazyLoadSoundResources()
	{
		try
		{
			if (!SoundLib.hasLoadedSoundResources)
			{
				SoundMetaData.LoadMetaData();
				FF9SndMetaData.LoadBattleEncountBgmMetaData();
				SoundLib.LoadAllResidentSfxSoundData();
				SoundLib.hasLoadedSoundResources = true;
			}
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void LoadMusic(String jsonMetaData)
	{
		try
		{
			SoundLib.musicPlayer.LoadMusic(jsonMetaData);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void UnloadMusic()
	{
		try
		{
			SoundLib.musicPlayer.UnloadMusic();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void LoadMovieAudio(String jsonMetaData)
	{
		try
		{
			SoundLib.movieAudioPlayer.LoadMusic(jsonMetaData);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void UnloadMovieAudio()
	{
		try
		{
			SoundLib.movieAudioPlayer.UnloadMusic();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void PlayMusic(Int32 soundIndex)
	{
		try
		{
			SoundLib.musicPlayer.PlayMusic(soundIndex);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void SetNextLoopRegion(Int32 soundIndex)
	{
		try
		{
			SoundLib.musicPlayer.NextLoopRegion(soundIndex);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void PlayMusic(Int32 soundIndex, Int32 fadeIn)
	{
		try
		{
			SoundLib.musicPlayer.PlayMusic(soundIndex, fadeIn, SoundProfileType.Music);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void PauseMusic()
	{
		try
		{
			SoundLib.musicPlayer.PauseMusic();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

    public static void ResumeMusic()
    {
        try
        {
            SoundLib.musicPlayer.ResumeMusic();
        }
        catch (Exception message)
        {
            SoundLib.LogError(message);
        }
    }

    public static Int32 GetActiveMusicSoundID()
	{
		try
		{
			return SoundLib.musicPlayer.GetActiveSoundID();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
		return -1;
	}

	public static void StopMusic()
	{
		try
		{
			SoundLib.musicPlayer.StopMusic();
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void StopMusic(Int32 fadeOut)
	{
		try
		{
			SoundLib.musicPlayer.StopMusic(fadeOut);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void SetMusicVolume(Single volume)
	{
		try
		{
			SoundLib.musicPlayer.SetMusicVolume(volume);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void SetMusicPanning(Single panning)
	{
		try
		{
			SoundLib.musicPlayer.SetMusicPanning(panning);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void SetMusicPitch(Single pitch)
	{
		try
		{
			SoundLib.musicPlayer.SetMusicPitch(pitch);
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void TryUpdateMusicVolume()
	{
		if (FF9StateSystem.Settings.cfg.IsMusicEnabled)
			EnableMusic();
	}

	public static void EnableMusic()
	{
		try
		{
			SoundLib.allSoundDispatchPlayer.SetMusicVolume(Configuration.Audio.MusicVolume);
			SoundLib.musicPlayer.SetOptionVolume(Configuration.Audio.MusicVolume);
			SoundLib.movieAudioPlayer.SetOptionVolume(Configuration.Audio.MusicVolume);
			SoundLib.MusicIsMute = false;
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void DisableMusic()
	{
		try
		{
			SoundLib.allSoundDispatchPlayer.SetMusicVolume(0);
			SoundLib.allSoundDispatchPlayer.SetMusicVolume(0);
			SoundLib.musicPlayer.SetOptionVolume(0);
			SoundLib.movieAudioPlayer.SetOptionVolume(0);
			SoundLib.MusicIsMute = true;
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}
	
		
	public static void TryUpdateSoundVolume()
	{
		if (FF9StateSystem.Settings.cfg.IsSoundEnabled)
			EnableSoundEffect();
	}

	public static void EnableSoundEffect()
	{
		try
		{
			SoundLib.allSoundDispatchPlayer.SetSoundEffectVolume(Configuration.Audio.SoundVolume);
			SoundLib.sfxSoundPlayer.SetVolume(Configuration.Audio.SoundVolume);
			SoundLib.soundEffectPlayer.SetVolume(Configuration.Audio.SoundVolume / 100f);
			SoundLib.songPlayer.SetVolume(Configuration.Audio.SoundVolume / 100f);
			SoundLib.SoundEffectIsMute = false;
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static void DisableSoundEffect()
	{
		try
		{
			SoundLib.allSoundDispatchPlayer.SetSoundEffectVolume(0);
			SoundLib.sfxSoundPlayer.SetVolume(0);
			SoundLib.soundEffectPlayer.SetVolume(0f);
			SoundLib.songPlayer.SetVolume(0f);
			SoundLib.SoundEffectIsMute = true;
		}
		catch (Exception message)
		{
			SoundLib.LogError(message);
		}
	}

	public static AllSoundDispatchPlayer GetAllSoundDispatchPlayer()
	{
		return SoundLib.allSoundDispatchPlayer;
	}

	public static void UpdatePlayingSoundEffectPitchByGameSpeed()
	{
		SoundLib.allSoundDispatchPlayer.UpdatePlayingSoundEffectPitchFollowingGameSpeed();
	}

	public static void StopAllSounds(Boolean isAll = true)
	{
		try
		{
			SoundLib.musicPlayer.StopMusic();
			SoundLib.soundEffectPlayer.StopAllSoundEffects();
			SoundLib.songPlayer.StopAllSoundEffects();
			SoundLib.movieAudioPlayer.StopMusic();
			if (isAll)
			{
				SoundLib.allSoundDispatchPlayer.StopAllSounds();
			}
			else
			{
				SoundLib.allSoundDispatchPlayer.PauseAllSounds();
			}
			SoundLib.sfxSoundPlayer.StopAllSounds();
		}
		catch (Exception message)
		{
			SoundLib.LogWarning(message);
		}
	}

	public static void ClearSuspendedSounds()
	{
		SoundLib.allSoundDispatchPlayer.ClearSuspendedSounds();
	}

	public static void SuspendSoundSystem()
	{
		if (!SoundLib.isSuspendAllSounds)
		{
			SoundLib.movieAudioPlayer.PauseMusic();
			SoundLib.allSoundDispatchPlayer.PauseAllSounds();
			SoundLib.sfxSoundPlayer.PauseAllSounds();
			SoundLib.isSuspendAllSounds = true;
		}
	}

	public static void ResumeSoundSystem()
	{
		if (SoundLib.isSuspendAllSounds)
		{
			SoundLib.movieAudioPlayer.ResumeMusic();
			SoundLib.allSoundDispatchPlayer.ResumeAllSounds();
			SoundLib.sfxSoundPlayer.ResumeAllSounds();
			SoundLib.isSuspendAllSounds = false;
		}
	}

	private void Awake()
	{
		this.InitializePlugin();
		this.InitializeSoundPlayer();
		SoundLib.instance = this;
	}

	private void Update()
	{
		if (this.soundPlayerList != null)
		{
			foreach (SoundPlayer soundPlayer in this.soundPlayerList)
			{
				soundPlayer.Update();
			}
		}
		this.UpdatePauseState();
	}

	private void UpdatePauseState()
	{
		Boolean flag = PersistenSingleton<UIManager>.Instance.IsPause || UIManager.Field.isShowSkipMovieDialog;
		if (flag != PersistenSingleton<UIManager>.Instance.IsPause || (UIManager.Field != (UnityEngine.Object)null && UIManager.Field.isShowSkipMovieDialog))
		{
			flag = (PersistenSingleton<UIManager>.Instance.IsPause || UIManager.Field.isShowSkipMovieDialog);
		}
		if (flag != this.isPauseLastFrame)
		{
			if (flag)
			{
				SoundLib.SuspendSoundSystem();
			}
			else
			{
				SoundLib.ResumeSoundSystem();
			}
		}
		this.isPauseLastFrame = flag;
	}

	private void OnDestroy()
	{
		this.FinalizePlugin();
	}

	private void OnApplicationPause(Boolean pause)
	{
		if (pause)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_Suspend();
		}
		else
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_Resume();
		}
	}

	private void OnQuit()
	{
		this.FinalizePlugin();
	}

	private Boolean InitializePlugin()
	{
		if (this.m_isInitialized)
		{
			return true;
		}
		SoundLib.Log("InitializePlugin()");
		Int32 num = ISdLibAPIProxy.Instance.SdSoundSystem_Create(String.Empty);
		if (num < 0)
		{
			return false;
		}
		this.m_isInitialized = true;
		GameObject gameObject = new GameObject("SoundLibWndProc");
		SoundLibWndProc soundLibWndProc = gameObject.AddComponent<SoundLibWndProc>();
		gameObject.transform.parent = base.transform;
		return true;
	}

	private void FinalizePlugin()
	{
		if (!this.m_isInitialized)
		{
			return;
		}
		SoundLib.Log("FinalizePlugin()");
		ISdLibAPIProxy.Instance.SdSoundSystem_Release();
		this.m_isInitialized = false;
	}

	private void InitializeSoundPlayer()
	{
		SoundLib.soundEffectPlayer = new SoundEffectPlayer();
		SoundLib.musicPlayer = new MusicPlayer();
		SoundLib.movieAudioPlayer = new MovieAudioPlayer();
		SoundLib.songPlayer = new SongPlayer();
		SoundLib.allSoundDispatchPlayer = new AllSoundDispatchPlayer();
		SoundLib.sfxSoundPlayer = new SfxSoundPlayer();
		this.soundPlayerList = new List<SoundPlayer>();
		this.soundPlayerList.Add(SoundLib.soundEffectPlayer);
		this.soundPlayerList.Add(SoundLib.musicPlayer);
		this.soundPlayerList.Add(SoundLib.movieAudioPlayer);
		this.soundPlayerList.Add(SoundLib.songPlayer);
		this.soundPlayerList.Add(SoundLib.allSoundDispatchPlayer);
		this.soundPlayerList.Add(SoundLib.sfxSoundPlayer);
	}

	private Boolean m_isInitialized;

	private static SoundEffectPlayer soundEffectPlayer;

	private static MusicPlayer musicPlayer;

	public static MovieAudioPlayer movieAudioPlayer;

	private static SongPlayer songPlayer;

	private static AllSoundDispatchPlayer allSoundDispatchPlayer;

	private static SfxSoundPlayer sfxSoundPlayer;

	private List<SoundPlayer> soundPlayerList;

	private static SoundLib instance;

	private static Boolean hasLoadedSoundResources;

	private static Boolean isSuspendAllSounds;

	private Boolean isPauseLastFrame;
}
