using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class AllSoundDispatchPlayer : SoundPlayer
{
	public static Single NormalizeVolume(Int32 originalVolume)
	{
		return (Single)originalVolume / 127f;
	}

	public static Int32 ReverseNormalizeVolume(Single normalizedVolume)
	{
		Int32 num = (Int32)(normalizedVolume * 127f);
		return (Int32)((num > 127) ? 127 : num);
	}

	private static Single NormalizePitch(Int32 original)
	{
		Int32 num = 255;
		Single num2 = 2f;
		Single num3 = (Single)original / (Single)num;
		return num3 * num2 + 1f;
	}
	
	public void SetMusicVolume(Int32 volume)
	{
		this.musicPlayerVolume = volume / 100f;
		this.UpdatePlayingMusicVolume(volume > 0);
	}

	public void SetSoundEffectVolume(Int32 volume)
	{
		this.soundEffectPlayerVolume = volume / 100f;
		this.UpdatePlayingSoundEffectVolume(volume > 0);
	}

	private void UpdatePlayingMusicVolume(Boolean isEnable)
	{
		this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, 0);
			}
		});
		AllSoundDispatchPlayer.PlayingSfx[] array = this.sfxResSlot;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = array[i];
			if (playingSfx != null)
			{
				if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx.SoundID) != 0)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(playingSfx.SndEffectVol) * this.musicPlayerVolume, 0);
				}
			}
		}
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx2 in this.sfxChanels)
		{
			if (playingSfx2 != null)
			{
				if (FF9Snd.GetIsExtEnvObjNo(playingSfx2.ObjNo))
				{
					if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx2.SoundID) != 0)
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx2.SoundID, AllSoundDispatchPlayer.NormalizeVolume(playingSfx2.SndEffectVol) * this.musicPlayerVolume, 0);
					}
				}
			}
		}
	}

	private void UpdatePlayingSoundEffectVolume(Boolean isEnable)
	{
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (playingSfx != null)
			{
				if (!FF9Snd.GetIsExtEnvObjNo(playingSfx.ObjNo))
				{
					if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx.SoundID) != 0)
					{
						if (playingSfx.ObjNo != 1261 && playingSfx.ObjNo != 3096)
						{
							ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(playingSfx.SndEffectVol) * this.soundEffectPlayerVolume, 0);
						}
					}
				}
			}
		}
		this.GetSoundProfileIfExist(this.currentSongID, SoundProfileType.Song, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.soundEffectPlayerVolume, 0);
			}
		});
	}

	public static Int32 ConvertTickToMillisec(Int32 ticks)
	{
		Single num = 1000f / (Single)Application.targetFrameRate;
		Single num2 = (Single)ticks * num;
		return (Int32)(((Int32)(num2 * 10f) % 10 > 5) ? ((Int32)num2 + 1) : ((Int32)num2));
	}

	public Int32 GetCurrentMusicId()
	{
		return this.currentMusicID;
	}

	public AllSoundDispatchPlayer.PlayingSfx[] GetResidentSndEffectSlot()
	{
		return this.sfxResSlot;
	}

	public void SetNextLoopRegion(Int32 ObjNo)
	{
		this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetNextLoopRegion(soundProfile.SoundID);
		});
	}

	public void FF9SOUND_SONG_LOAD(Int32 ObjNo)
	{
		if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
		{
			if (this.currentMusicID != ObjNo)
			{
				if (this.currentMusicID != -1)
				{
					this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
					{
						if (soundProfile != null)
						{
							ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
						}
						this.currentMusicID = -1;
					});
				}
				this.CreateSoundProfileIfNotExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
				{
					this.CreateSound(soundProfile);
					soundProfile.SoundVolume = 1f;
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
					Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
					if (fldMapNo == 503 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2970 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 11 && ObjNo == 35)
					{
						soundProfile.SoundVolume = 0f;
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume, 0);
					}
					this.currentMusicID = ObjNo;
					this.StopAndClearSuspendBGM(ObjNo);
				});
			}
		}
		else
		{
			this.CreateSoundProfileIfNotExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
			});
		}
	}
	
	public void StopAndClearSuspendBGM(int ObjNo)
	{
		if (this.suspendBgmNo != -1 && ObjNo != this.suspendBgmNo)
		{
			if (PersistenSingleton<EventEngine>.Instance.gMode == 1 && FF9Snd.sndFuncPtr == new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9FieldSoundDispatch))
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.suspendBgmID, 0);
				this.suspendBgmNo = -1;
				this.suspendBgmID = 0;
			}
			if (PersistenSingleton<EventEngine>.Instance.gMode == 3 && FF9Snd.sndFuncPtr == new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9WorldSoundDispatch))
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.suspendBgmID, 0);
				this.suspendBgmNo = -1;
				this.suspendBgmID = 0;
			}
		}
	}

	public void FF9SOUND_SONG_PLAY(Int32 ObjNo, Int32 vol = 127, int time = 0)
	{
		if (this.currentMusicID != ObjNo)
		{
			if (this.currentMusicID != -1)
			{
				this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
				{
					if (soundProfile != null)
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
					}
					this.currentMusicID = -1;
				});
			}
			this.CreateSoundProfileIfNotExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				this.CreateSound(soundProfile);
				soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(vol);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, time * 1000);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, 0);
				this.currentMusicID = ObjNo;
				this.StopAndClearSuspendBGM(ObjNo);
			});
		}
		else
		{
			this.CreateSoundProfileIfNotExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, time * 1000);
					soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(vol);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, 0);
					this.StopAndClearSuspendBGM(ObjNo);
				}
			});
		}
	}

	public Int32 GetSuspendSongID()
	{
		return this.suspendBgmNo;
	}

	public void FF9SOUND_SONG_SUSPEND(int ObjNo, bool isSkipPause = false)
	{
		this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				if (!isSkipPause)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
				}
				this.suspendBgmNo = ObjNo;
				this.suspendBgmID = soundProfile.SoundID;
				this.currentMusicID = -1;
			}
			else
			{
				this.suspendBgmNo = -1;
				this.suspendBgmID = 0;
			}
		});
	}

	public void FF9SOUND_SONG_RESTORE()
	{
		if (this.suspendBgmNo != -1)
		{
			this.currentMusicID = this.suspendBgmNo;
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(this.suspendBgmID, 0, 0);
			this.suspendBgmNo = -1;
			this.suspendBgmID = 0;
		}
		else
		{
			SoundLib.Log("FF9SOUND_SONG_RESTORE has suspendSongID = NONE");
		}
	}

	public void FF9SOUND_SONG_STOP(int ObjNo)
	{
		this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
			}
			if (ObjNo == this.suspendBgmNo)
			{
				this.suspendBgmNo = -1;
				this.suspendBgmID = 0;
			}
			if (ObjNo == this.currentMusicID)
			{
				this.currentMusicID = -1;
			}
		});
	}

	public void StopCurrentSong(Int32 ticks)
	{
		this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
			}
			this.currentMusicID = -1;
		});
	}

	public void FF9SOUND_SONG_STOPCURRENT()
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
				}
				this.currentMusicID = -1;
			});
		}
		else
		{
			SoundLib.Log("FF9SOUND_SONG_STOPCURRENT has currentMusicID = NONE");
		}
	}

	public void FF9SOUND_SONG_VOL(Int32 ObjNo, Int32 vol)
	{
		this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
				soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(vol);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, 0);
			}
		});
	}

	public void FF9SOUND_SONG_VOL_INTPL(Int32 ObjNo, Int32 ticks, Int32 to)
	{
		this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
				soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(to);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
			}
		});
	}

	public void FF9SOUND_SONG_VOL_FADE(Int32 ObjNo, Int32 ticks, Int32 from, Int32 to)
	{
		this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, AllSoundDispatchPlayer.NormalizeVolume(from), 0);
				soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(to);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
			}
		});
	}

	public void FF9SOUND_SONG_VOL_INTPLALL(Int32 ticks, Int32 to)
	{
		if (FF9StateSystem.Settings.cfg.sound == 1UL)
		{
			return;
		}
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
					soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(to);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.musicPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
				}
			});
		}
	}

	public void FF9SOUND_SONG_VOL_FADEALL(Int32 ticks, Int32 from, Int32 to)
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
					this.FF9SOUND_SONG_VOL_FADE(soundProfile.SoundIndex, ticks, from, to);
				}
			});
		}
	}

	public void FF9SOUND_SONG_PITCH(Int32 ObjNo, Int32 pitch)
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					Single pitch2 = AllSoundDispatchPlayer.NormalizePitch(pitch);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, pitch2, 0);
				}
			});
		}
	}

	public void FF9SOUND_SONG_PITCH_INTPL(Int32 ObjNo, Int32 tick, Int32 to)
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					Single pitch = AllSoundDispatchPlayer.NormalizePitch(to);
					Int32 transTimeMSec = AllSoundDispatchPlayer.ConvertTickToMillisec(tick);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, pitch, transTimeMSec);
				}
			});
		}
	}

	public void FF9SOUND_SONG_PITCH_FADE(Int32 ObjNo, Int32 tick, Int32 from, Int32 to)
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(ObjNo, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					Single pitch = AllSoundDispatchPlayer.NormalizePitch(from);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, pitch, 0);
					Single pitch2 = AllSoundDispatchPlayer.NormalizePitch(to);
					Int32 transTimeMSec = AllSoundDispatchPlayer.ConvertTickToMillisec(tick);
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, pitch2, transTimeMSec);
				}
			});
		}
	}

	public void FF9SOUND_SONG_JUMPPOINT()
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetNextLoopRegion(soundProfile.SoundID);
				}
			});
		}
	}

	public void FF9SOUND_SONG_SKIPPHRASE_MILLISEC(Int32 ObjNo, Int32 offsetTimeMSec)
	{
		this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
		{
			if (soundProfile != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
				soundProfile.SoundID = ISdLibAPIProxy.Instance.SdSoundSystem_CreateSound(soundProfile.BankID);
				Int32 num = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, offsetTimeMSec);
			}
		});
	}

	public void FF9SOUND_SNDEFFECT_PLAY(Int32 ObjNo, Int32 attr, Int32 pos, Int32 vol)
	{
		this.LimitPlayingSfxByObjNo(ObjNo, 2295, 1);
		this.LimitPlayingSfxByObjNo(ObjNo, 1966, 3);
		this.CreateSoundProfileIfNotExist(ObjNo, SoundProfileType.SoundEffect, delegate(SoundProfile soundProfile)
		{
			this.CreateSound(soundProfile);
			soundProfile.Pitch = 1f;
			if (this.TuneUpSoundEffectByObjNo(ObjNo, soundProfile) == 0)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, AllSoundDispatchPlayer.NormalizeVolume(vol) * this.soundEffectPlayerVolume, 0);
				this.ShiftPitchIfFastForward(soundProfile);
			}
			AllSoundDispatchPlayer.PlayingSfx playingSfx = new AllSoundDispatchPlayer.PlayingSfx();
			playingSfx.ObjNo = ObjNo;
			playingSfx.SoundID = soundProfile.SoundID;
			playingSfx.SndEffectVol = vol;
			playingSfx.Pitch = soundProfile.Pitch;
			playingSfx.IsFastForwardedPitch = (HonoBehaviorSystem.Instance.GetFastForwardFactor() != 1);
			playingSfx.StartPlayTime = Time.time;
			this.sfxChanels.Add(playingSfx);
		});
	}

	private void LimitPlayingSfxByObjNo(int ObjNo, int limitObjNo, int limitNumber)
	{
		if (ObjNo == limitObjNo)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = (AllSoundDispatchPlayer.PlayingSfx)null;
			float num = float.MaxValue;
			int num2 = 0;
			for (int i = 0; i < this.sfxChanels.Count; i++)
			{
				AllSoundDispatchPlayer.PlayingSfx playingSfx2 = this.sfxChanels[i];
				if (playingSfx2.ObjNo == ObjNo)
				{
					num2++;
					if (playingSfx2.StartPlayTime < num)
					{
						playingSfx = playingSfx2;
						num = playingSfx2.StartPlayTime;
					}
				}
			}
			if (num2 >= limitNumber)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, 0);
				this.sfxChanels.Remove(playingSfx);
			}
		}
	}

	private Int32 TuneUpSoundEffectByObjNo(Int32 ObjNo, SoundProfile soundProfile)
	{
		if (ObjNo == 1748)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, 0.6f, 0);
			soundProfile.Pitch *= 0.6f;
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
			return 1;
		}
		if (ObjNo == 58)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 900);
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, 0f, 0);
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, 0.7f, 300);
			soundProfile.Pitch *= 0.8f;
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
			return 1;
		}
		return 0;
	}

	private void ShiftPitchIfFastForward(SoundProfile soundProfile)
	{
		Int32 fastForwardFactor = HonoBehaviorSystem.Instance.GetFastForwardFactor();
		if (fastForwardFactor == 1)
		{
			return;
		}
		if (FF9Snd.IsShiftPitchInFastForwardMode(soundProfile.SoundIndex))
		{
			soundProfile.Pitch *= (Single)fastForwardFactor;
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, (Single)fastForwardFactor, 0);
		}
	}

	public void FF9SOUND_SNDEFFECT_STOP(Int32 ObjNo, Int32 attr)
	{
		List<AllSoundDispatchPlayer.PlayingSfx> list = new List<AllSoundDispatchPlayer.PlayingSfx>();
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (playingSfx.ObjNo == ObjNo)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, attr);
				list.Add(playingSfx);
			}
		}
		foreach (AllSoundDispatchPlayer.PlayingSfx item in list)
		{
			this.sfxChanels.Remove(item);
		}
	}

	public void FF9SOUND_SNDEFFECT_STOP_ALL(HashSet<Int32> exceptObjNo = null)
	{
		List<AllSoundDispatchPlayer.PlayingSfx> list = new List<AllSoundDispatchPlayer.PlayingSfx>();
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (!FF9Snd.GetIsExtEnvObjNo(playingSfx.ObjNo))
			{
				if (exceptObjNo == null || !exceptObjNo.Contains(playingSfx.ObjNo))
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, 0);
					list.Add(playingSfx);
				}
			}
		}
		foreach (AllSoundDispatchPlayer.PlayingSfx item in list)
		{
			this.sfxChanels.Remove(item);
		}
	}

	public void FF9SOUND_SNDEFFECT_VOL(Int32 ObjNo, Int32 attr, Int32 vol)
	{
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (playingSfx.ObjNo == ObjNo)
			{
				playingSfx.SndEffectVol = vol;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(vol) * this.soundEffectPlayerVolume, 0);
			}
		}
	}

	public void FF9SOUND_SNDEFFECT_VOL_INTPL(Int32 ObjNo, Int32 attr, Int32 ticks, Int32 to)
	{
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (playingSfx.ObjNo == ObjNo)
			{
				playingSfx.SndEffectVol = to;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(to) * this.soundEffectPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
			}
		}
	}

	public void FF9SOUND_SNDEFFECT_VOL_ALL(Int32 vol)
	{
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (!FF9Snd.GetIsExtEnvObjNo(playingSfx.ObjNo))
			{
				playingSfx.SndEffectVol = vol;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(vol) * this.soundEffectPlayerVolume, 0);
			}
		}
	}

	public void FF9SOUND_SNDEFFECT_VOL_INTPLALL(Int32 ticks, Int32 to)
	{
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (!FF9Snd.GetIsExtEnvObjNo(playingSfx.ObjNo) || playingSfx.ObjNo == 656)
			{
				playingSfx.SndEffectVol = to;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(to) * this.soundEffectPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
			}
		}
	}

	public void FF9SOUND_SNDEFFECT_PITCH(Int32 ObjNo, Int32 attr, Int32 pitch)
	{
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (playingSfx.ObjNo == ObjNo)
			{
				if (!FF9Snd.GetIsExtEnvObjNo(playingSfx.ObjNo))
				{
					playingSfx.Pitch = AllSoundDispatchPlayer.NormalizePitch(pitch);
					if (FF9Snd.IsShiftPitchInFastForwardMode(ObjNo))
					{
						Int32 fastForwardFactor = HonoBehaviorSystem.Instance.GetFastForwardFactor();
						playingSfx.Pitch = (Single)fastForwardFactor;
					}
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(playingSfx.SoundID, playingSfx.Pitch, 0);
				}
			}
		}
	}

	public Int32 GetSndEffectResSoundID(Int32 slot)
	{
		if (this.sfxResSlot == null)
		{
			return -1;
		}
		if (slot > 1)
		{
			return -1;
		}
		if (this.sfxResSlot[slot] == null)
		{
			return -1;
		}
		return this.sfxResSlot[slot].ObjNo;
	}

	public void FF9SOUND_SNDEFFECTRES_PLAY(Int32 slot, Int32 ObjNo, Int32 attr, Int32 pos, Int32 vol)
	{
		if (this.onSndEffectResPlay != null)
		{
			this.onSndEffectResPlay(slot, ObjNo, attr, pos, vol);
		}
		if (this.sfxResSlot[slot] != null)
		{
			this.GetSoundProfileIfExist(this.sfxResSlot[slot].ObjNo, SoundProfileType.SoundEffect, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.sfxResSlot[slot].SoundID, 0);
				}
				this.sfxResSlot[slot] = (AllSoundDispatchPlayer.PlayingSfx)null;
			});
		}
		this.CreateSoundProfileIfNotExist(ObjNo, SoundProfileType.SoundEffect, delegate(SoundProfile soundProfile)
		{
			this.CreateSound(soundProfile);
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, AllSoundDispatchPlayer.NormalizeVolume(vol) * this.musicPlayerVolume, 0);
			AllSoundDispatchPlayer.PlayingSfx playingSfx = new AllSoundDispatchPlayer.PlayingSfx();
			playingSfx.ObjNo = ObjNo;
			playingSfx.SoundID = soundProfile.SoundID;
			playingSfx.SndEffectPos = pos;
			playingSfx.SndEffectVol = vol;
			playingSfx.PrevSndEffectMapID = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			playingSfx.IsSuspend = false;
			this.sfxResSlot[slot] = playingSfx;
		});
	}

	public void FF9SOUND_SNDEFFECTRES_STOP(Int32 slot, Int32 ObjNo, Int32 attr)
	{
		if (this.onSndEffectResStop != null)
		{
			this.onSndEffectResStop(slot, ObjNo, attr);
		}
		for (Int32 i = 0; i < 2; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[i];
			if (playingSfx != null)
			{
				if (playingSfx.ObjNo == ObjNo)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, 0);
					this.sfxResSlot[i] = (AllSoundDispatchPlayer.PlayingSfx)null;
				}
			}
		}
	}

	public void FF9SOUND_SNDEFFECTRES_STOPCURRENT()
	{
		if (this.onSndEffectResStopCurrent != null)
		{
			this.onSndEffectResStopCurrent();
		}
		for (Int32 i = 0; i < 2; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[i];
			if (playingSfx != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, 0);
				this.sfxResSlot[i] = (AllSoundDispatchPlayer.PlayingSfx)null;
			}
		}
	}

	public void FF9SOUND_SNDEFFECTRES_SUSPEND(Int32 slot)
	{
		if (this.onSndEffectResSuspend != null)
		{
			this.onSndEffectResSuspend(slot);
		}
		AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[slot];
		if (playingSfx == null)
		{
			SoundLib.LogWarning("FF9SOUND_SNDEFFECTRES_SUSPEND: slot: " + slot + " is null!");
		}
		else
		{
			playingSfx.PrevSndEffectMapID = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			playingSfx.IsSuspend = true;
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, 0);
		}
	}

	public void FF9SOUND_SNDEFFECTRES_RESTORE(Int32 slot)
	{
		if (this.onSndEffectResRestore != null)
		{
			this.onSndEffectResRestore(slot);
		}
		AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[slot];
		if (playingSfx == null)
		{
			SoundLib.LogWarning("FF9SOUND_SNDEFFECTRES_RESTORE: slot: " + slot + " is null!");
		}
		else if (playingSfx.PrevSndEffectMapID == (Int32)FF9StateSystem.Common.FF9.fldMapNo)
		{
			this.FF9SOUND_SNDEFFECTRES_PLAY(slot, playingSfx.ObjNo, 0, 0, AllSoundDispatchPlayer.ReverseNormalizeVolume((Single)playingSfx.SndEffectVol));
		}
	}

	public void FF9SOUND_SNDEFFECTRES_VOL(Int32 slot, Int32 ObjNo, Int32 attr, Int32 vol)
	{
		if (this.onSndEffectResVol != null)
		{
			this.onSndEffectResVol(slot, ObjNo, attr, vol);
		}
		for (Int32 i = 0; i < 2; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[i];
			if (playingSfx != null)
			{
				if (playingSfx.ObjNo == ObjNo)
				{
					playingSfx.SndEffectVol = vol;
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(vol) * this.musicPlayerVolume, 0);
				}
			}
		}
	}

	public void FF9SOUND_SNDEFFECTRES_VOL_INTPL(Int32 slot, Int32 ObjNo, Int32 attr, Int32 ticks, Int32 to)
	{
		if (this.onSndEffectResVolIntpl != null)
		{
			this.onSndEffectResVolIntpl(slot, ObjNo, attr, ticks, to);
		}
		for (Int32 i = 0; i < 2; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[i];
			if (playingSfx != null)
			{
				if (playingSfx.ObjNo == ObjNo)
				{
					playingSfx.SndEffectVol = to;
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(to) * this.musicPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
				}
			}
		}
	}

	public void FF9SOUND_SNDEFFECTRES_VOL_ALL(Int32 vol)
	{
		if (this.onSndEffectResVolAll != null)
		{
			this.onSndEffectResVolAll(vol);
		}
		for (Int32 i = 0; i < 2; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[i];
			if (playingSfx != null)
			{
				playingSfx.SndEffectVol = vol;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(vol) * this.musicPlayerVolume, 0);
			}
		}
	}

	public void FF9SOUND_SNDEFFECTRES_VOL_INTPLALL(Int32 ticks, Int32 to)
	{
		if (this.onSndEffectResVolIntplAll != null)
		{
			this.onSndEffectResVolIntplAll(ticks, to);
		}
		for (Int32 i = 0; i < 2; i++)
		{
			AllSoundDispatchPlayer.PlayingSfx playingSfx = this.sfxResSlot[i];
			if (playingSfx != null)
			{
				playingSfx.SndEffectVol = to;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(playingSfx.SoundID, AllSoundDispatchPlayer.NormalizeVolume(to) * this.musicPlayerVolume, AllSoundDispatchPlayer.ConvertTickToMillisec(ticks));
			}
		}
	}

	public void FF9SOUND_STREAM_PLAY(Int32 streamid, Int32 pos, Int32 reverb)
	{
		try
		{
			this.FF9SOUND_STREAM_STOP();
		}
		catch (Exception message)
		{
			SoundLib.LogWarning(message);
		}
		try
		{
			this.CreateSoundProfileIfNotExist(streamid, SoundProfileType.Song, delegate(SoundProfile soundProfile)
			{
				base.CreateSound(soundProfile);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
				this.currentSongID = soundProfile.SoundIndex;
			});
		}
		catch (Exception message2)
		{
			SoundLib.LogWarning(message2);
		}
	}

	public void FF9SOUND_STREAM_STOP()
	{
		if (this.currentSongID != -1)
		{
			this.GetSoundProfileIfExist(this.currentSongID, SoundProfileType.Song, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
				}
				this.currentSongID = -1;
			});
		}
		else
		{
			SoundLib.Log("FF9SOUND_STREAM_STOP has currentSongID = NONE");
		}
	}

	public void FF9SOUND_STREAM_VOL(Int32 vol)
	{
		if (this.currentSongID != -1)
		{
			this.CreateSoundProfileIfNotExist(this.currentSongID, SoundProfileType.Song, delegate(SoundProfile soundProfile)
			{
				soundProfile.SoundVolume = AllSoundDispatchPlayer.NormalizeVolume(vol);
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.soundEffectPlayerVolume, 0);
			});
		}
		else
		{
			SoundLib.Log("FF9SOUND_STREAM_STOP has currentSongID = NONE");
		}
	}

	private void GetSoundProfileIfExist(Int32 soundIndex, SoundProfileType type, AllSoundDispatchPlayer.OnGetSoundProfileFinish onFinishDelegate)
	{
		SoundDatabase soundDatabase = (SoundDatabase)null;
		if (type == SoundProfileType.Music)
		{
			soundDatabase = this.musicImmediateDB;
		}
		else if (type == SoundProfileType.SoundEffect)
		{
			soundDatabase = this.sfxImmediateDB;
		}
		else if (type == SoundProfileType.Song)
		{
			soundDatabase = this.songImmediateDB;
		}
		SoundProfile soundProfile = soundDatabase.Read(soundIndex);
		if (soundProfile != null)
		{
			onFinishDelegate(soundProfile);
		}
		else
		{
			onFinishDelegate((SoundProfile)null);
		}
	}

	private void CreateSoundProfileIfNotExist(Int32 soundIndex, SoundProfileType type, AllSoundDispatchPlayer.OnCreateFinish onFinishDelegate)
	{
		this.onCreateDelegate = onFinishDelegate;
		SoundDatabase soundDatabase = (SoundDatabase)null;
		if (type == SoundProfileType.Music)
		{
			soundDatabase = this.musicImmediateDB;
		}
		else if (type == SoundProfileType.SoundEffect)
		{
			soundDatabase = this.sfxImmediateDB;
		}
		else if (type == SoundProfileType.Song)
		{
			soundDatabase = this.songImmediateDB;
		}
		SoundProfile soundProfile = soundDatabase.Read(soundIndex);
		if (soundProfile != null)
		{
			this.onCreateDelegate(soundProfile);
		}
		else
		{
			soundProfile = SoundMetaData.GetSoundProfile(soundIndex, type);
			this.immediateLoadedSoundProfile = soundProfile;
			this.DeleteImmediateSoundProfileIfUnused();
			base.LoadResource(soundProfile, soundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadImmediateSoundResourceCallback));
		}
	}

	private void DeleteImmediateSoundProfileIfUnused()
	{
		Dictionary<Int32, SoundProfile> dictionary = this.musicImmediateDB.ReadAll();
		if (dictionary.Count >= 4)
		{
			List<SoundProfile> list = new List<SoundProfile>();
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in dictionary)
			{
				SoundProfile value = keyValuePair.Value;
				if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(value.SoundID) == 0)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(value.SoundID, 0);
					list.Add(value);
				}
			}
			foreach (SoundProfile soundProfile in list)
			{
				SoundLib.Log(String.Concat(new Object[]
				{
					"Unload Sound ID: ",
					soundProfile.SoundID,
					" Type: ",
					soundProfile.SoundProfileType
				}));
				base.UnloadResource(soundProfile, this.musicImmediateDB);
			}
		}
		Dictionary<Int32, SoundProfile> dictionary2 = this.sfxImmediateDB.ReadAll();
		if (dictionary2.Count >= 20)
		{
			List<SoundProfile> list2 = new List<SoundProfile>();
			List<AllSoundDispatchPlayer.PlayingSfx> list3 = new List<AllSoundDispatchPlayer.PlayingSfx>();
			foreach (AllSoundDispatchPlayer.PlayingSfx item in this.sfxChanels)
			{
				list3.Add(item);
			}
			foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
			{
				if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx.SoundID) == 0)
				{
					list3.Remove(playingSfx);
				}
				else if (playingSfx.ObjNo == -1)
				{
					list3.Remove(playingSfx);
				}
			}
			this.sfxChanels.Clear();
			foreach (AllSoundDispatchPlayer.PlayingSfx item2 in list3)
			{
				this.sfxChanels.Add(item2);
			}
			List<AllSoundDispatchPlayer.PlayingSfx> list4 = new List<AllSoundDispatchPlayer.PlayingSfx>();
			AllSoundDispatchPlayer.PlayingSfx[] array = this.sfxResSlot;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				AllSoundDispatchPlayer.PlayingSfx item3 = array[i];
				list4.Add(item3);
			}
			for (Int32 j = 0; j < (Int32)this.sfxResSlot.Length; j++)
			{
				AllSoundDispatchPlayer.PlayingSfx playingSfx2 = this.sfxResSlot[j];
				if (playingSfx2 != null)
				{
					if (!playingSfx2.IsSuspend)
					{
						if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx2.SoundID) == 0)
						{
							list4.Remove(playingSfx2);
							this.sfxResSlot[j] = (AllSoundDispatchPlayer.PlayingSfx)null;
						}
					}
					else
					{
						SoundLib.Log("Slot: " + j + " is suspend!");
					}
				}
			}
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair2 in dictionary2)
			{
				SoundProfile value2 = keyValuePair2.Value;
				Boolean flag = false;
				foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx3 in list3)
				{
					if (playingSfx3.SoundID == value2.SoundID)
					{
						flag = true;
						break;
					}
				}
				Boolean flag2 = false;
				foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx4 in list4)
				{
					if (playingSfx4 != null && playingSfx4.SoundID == value2.SoundID)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag && !flag2)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(value2.SoundID, 0);
					list2.Add(value2);
				}
			}
			foreach (SoundProfile soundProfile2 in list2)
			{
				SoundLib.Log(String.Concat(new Object[]
				{
					"Unload Sound ID: ",
					soundProfile2.SoundID,
					" Type: ",
					soundProfile2.SoundProfileType
				}));
				base.UnloadResource(soundProfile2, this.sfxImmediateDB);
			}
		}
		Dictionary<Int32, SoundProfile> dictionary3 = this.songImmediateDB.ReadAll();
		if (dictionary3.Count >= 4)
		{
			List<SoundProfile> list5 = new List<SoundProfile>();
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair3 in dictionary3)
			{
				SoundProfile value3 = keyValuePair3.Value;
				if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(value3.SoundID) == 0)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(value3.SoundID, 0);
					list5.Add(value3);
				}
			}
			foreach (SoundProfile soundProfile3 in list5)
			{
				SoundLib.Log(String.Concat(new Object[]
				{
					"Unload Sound ID: ",
					soundProfile3.SoundID,
					" Type: ",
					soundProfile3.SoundProfileType
				}));
				base.UnloadResource(soundProfile3, this.songImmediateDB);
			}
		}
	}

	private void LoadImmediateSoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		if (!isError)
		{
			if (this.immediateLoadedSoundProfile != null)
			{
				this.onCreateDelegate(this.immediateLoadedSoundProfile);
			}
		}
		else
		{
			SoundLib.Log("LoadOnTheFlySoundResourceCallback is Error");
		}
	}

	public void ClearSuspendedSounds()
	{
		this.FF9SOUND_SONG_STOP(this.suspendBgmNo);
		for (Int32 i = 0; i < (Int32)this.sfxResSlot.Length; i++)
		{
			this.sfxResSlot[i] = (AllSoundDispatchPlayer.PlayingSfx)null;
		}
	}

	public void StopAllSounds()
	{
		try
		{
			this.FF9SOUND_SONG_STOP(this.currentMusicID);
		}
		catch (Exception arg)
		{
			SoundLib.LogWarning("Exception occur! " + arg);
		}
		List<Int32> list = new List<Int32>();
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			list.Add(playingSfx.ObjNo);
		}
		foreach (Int32 objNo in list)
		{
			try
			{
				this.FF9SOUND_SNDEFFECT_STOP(objNo, 0);
			}
			catch (Exception arg2)
			{
				SoundLib.LogWarning("Exception occur! " + arg2);
			}
		}
		try
		{
			this.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
		}
		catch (Exception arg3)
		{
			SoundLib.LogWarning("Exception occur! " + arg3);
		}
		try
		{
			this.FF9SOUND_STREAM_STOP();
		}
		catch (Exception arg4)
		{
			SoundLib.LogWarning("Exception occur! " + arg4);
		}
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		for (Int32 i = 0; i < FF9Snd.FLDINT_CHAR_SOUNDCOUNT; i++)
		{
			map.charSoundArray[i] = new FF9FieldCharSound();
		}
		map.charSoundUse = 0;
	}

	public void PauseAllSounds()
	{
		try
		{
			if (this.currentMusicID != -1)
			{
				this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
				{
					if (soundProfile != null)
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
					}
				});
			}
		}
		catch (Exception arg)
		{
			SoundLib.LogWarning("Exception occur! " + arg);
		}
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			try
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(playingSfx.SoundID, 1, 0);
			}
			catch (Exception arg2)
			{
				SoundLib.LogWarning("Exception occur! " + arg2);
			}
		}
		try
		{
			for (Int32 i = 0; i < 2; i++)
			{
				AllSoundDispatchPlayer.PlayingSfx playingSfx2 = this.sfxResSlot[i];
				if (playingSfx2 != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(playingSfx2.SoundID, 1, 0);
				}
			}
		}
		catch (Exception arg3)
		{
			SoundLib.LogWarning("Exception occur! " + arg3);
		}
		try
		{
			if (this.currentSongID != -1)
			{
				this.GetSoundProfileIfExist(this.currentSongID, SoundProfileType.Song, delegate(SoundProfile soundProfile)
				{
					if (soundProfile != null)
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
					}
				});
			}
		}
		catch (Exception arg4)
		{
			SoundLib.LogWarning("Exception occur! " + arg4);
		}
	}

	public void ResumeAllSounds()
	{
		try
		{
			if (this.currentMusicID != -1)
			{
				this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
				{
					if (soundProfile != null)
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
					}
				});
			}
		}
		catch (Exception arg)
		{
			SoundLib.LogWarning("Exception occur! " + arg);
		}
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			try
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(playingSfx.SoundID, 0, 0);
			}
			catch (Exception arg2)
			{
				SoundLib.LogWarning("Exception occur! " + arg2);
			}
		}
		try
		{
			for (Int32 i = 0; i < 2; i++)
			{
				AllSoundDispatchPlayer.PlayingSfx playingSfx2 = this.sfxResSlot[i];
				if (playingSfx2 != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(playingSfx2.SoundID, 0, 0);
				}
			}
		}
		catch (Exception arg3)
		{
			SoundLib.LogWarning("Exception occur! " + arg3);
		}
		try
		{
			if (this.currentSongID != -1)
			{
				this.GetSoundProfileIfExist(this.currentSongID, SoundProfileType.Song, delegate(SoundProfile soundProfile)
				{
					if (soundProfile != null)
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
					}
				});
			}
		}
		catch (Exception arg4)
		{
			SoundLib.LogWarning("Exception occur! " + arg4);
		}
	}

	public override void Update()
	{
		if (this.currentMusicID != -1)
		{
			this.GetSoundProfileIfExist(this.currentMusicID, SoundProfileType.Music, delegate(SoundProfile soundProfile)
			{
				if (soundProfile != null)
				{
					soundProfile.StartPlayTime += 20f;
					if (soundProfile.StartPlayTime > 245000f)
					{
						soundProfile.StartPlayTime %= 245000f;
					}

				}

			});

		}

		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx.SoundID) == 0)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(playingSfx.SoundID, 0);
				this.sfxChannelsRemoveList.Add(playingSfx);
			}
		}
		foreach (AllSoundDispatchPlayer.PlayingSfx item in this.sfxChannelsRemoveList)
		{
			this.sfxChanels.Remove(item);
		}
		this.sfxChannelsRemoveList.Clear();
	}

	public void UpdatePlayingSoundEffectPitchFollowingGameSpeed()
	{
		Int32 fastForwardFactor = HonoBehaviorSystem.Instance.GetFastForwardFactor();
		foreach (AllSoundDispatchPlayer.PlayingSfx playingSfx in this.sfxChanels)
		{
			if (playingSfx != null)
			{
				if (FF9Snd.IsShiftPitchInFastForwardMode(playingSfx.ObjNo))
				{
					if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(playingSfx.SoundID) != 0)
					{
						if (playingSfx.ObjNo != 1261 && playingSfx.ObjNo != 3096)
						{
							Boolean flag = false;
							if (fastForwardFactor == SettingsState.FastForwardGameSpeed && !playingSfx.IsFastForwardedPitch)
							{
								playingSfx.Pitch *= (Single)SettingsState.FastForwardGameSpeed;
								playingSfx.IsFastForwardedPitch = true;
								flag = true;
							}
							else if (fastForwardFactor != SettingsState.FastForwardGameSpeed && playingSfx.IsFastForwardedPitch)
							{
								playingSfx.Pitch /= (Single)SettingsState.FastForwardGameSpeed;
								playingSfx.IsFastForwardedPitch = false;
								flag = true;
							}
							if (flag)
							{
								ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(playingSfx.SoundID, playingSfx.Pitch, 0);
							}
						}
					}
				}
			}
		}
	}

	public const Int32 VOLUME_MAX = 127;

	public const Int32 SNDEFFECTRES_SLOT_MAX = 2;

	private Single musicPlayerVolume = 1f;

	private Single soundEffectPlayerVolume = 1f;

	private Int32 currentMusicID = -1;

	private Int32 suspendBgmNo = -1;

	private Int32 suspendBgmID;

	private List<AllSoundDispatchPlayer.PlayingSfx> sfxChanels = new List<AllSoundDispatchPlayer.PlayingSfx>();

	public AllSoundDispatchPlayer.OnSndEffectResPlay onSndEffectResPlay;

	public AllSoundDispatchPlayer.OnSndEffectResStop onSndEffectResStop;

	public AllSoundDispatchPlayer.OnSndEffectResStopCurrent onSndEffectResStopCurrent;

	public AllSoundDispatchPlayer.OnSndEffectResSuspend onSndEffectResSuspend;

	public AllSoundDispatchPlayer.OnSndEffectResRestore onSndEffectResRestore;

	public AllSoundDispatchPlayer.OnSndEffectResVol onSndEffectResVol;

	public AllSoundDispatchPlayer.OnSndEffectResVolIntpl onSndEffectResVolIntpl;

	public AllSoundDispatchPlayer.OnSndEffectResVolAll onSndEffectResVolAll;

	public AllSoundDispatchPlayer.OnSndEffectResVolIntplAll onSndEffectResVolIntplAll;

	public AllSoundDispatchPlayer.PlayingSfx[] sfxResSlot = new AllSoundDispatchPlayer.PlayingSfx[2];

	private Int32 currentSongID = -1;

	private SoundProfile immediateLoadedSoundProfile;

	private AllSoundDispatchPlayer.OnCreateFinish onCreateDelegate;

	private SoundDatabase musicImmediateDB = new SoundDatabase();

	private SoundDatabase sfxImmediateDB = new SoundDatabase();

	private SoundDatabase songImmediateDB = new SoundDatabase();

	private List<AllSoundDispatchPlayer.PlayingSfx> sfxChannelsRemoveList = new List<AllSoundDispatchPlayer.PlayingSfx>();

	public class PlayingSfx
	{
		public PlayingSfx()
		{
			this.ObjNo = 0;
			this.SoundID = 0;
			this.SndEffectPos = 0;
			this.SndEffectVol = 0;
			this.PrevSndEffectMapID = 0;
			this.IsSuspend = false;
			this.Pitch = 1f;
			this.IsFastForwardedPitch = false;
			this.StartPlayTime = 0f;
		}

		public Int32 ObjNo;

		public Int32 SoundID;

		public Int32 SndEffectPos;

		public Int32 SndEffectVol;

		public Int32 PrevSndEffectMapID;

		public Boolean IsSuspend;

		public Single Pitch;

		public Boolean IsFastForwardedPitch;

		public float StartPlayTime;
	}

	public delegate void OnSndEffectResPlay(Int32 slot, Int32 ObjNo, Int32 attr, Int32 pos, Int32 vol);

	public delegate void OnSndEffectResStop(Int32 slot, Int32 ObjNo, Int32 attr);

	public delegate void OnSndEffectResStopCurrent();

	public delegate void OnSndEffectResSuspend(Int32 slot);

	public delegate void OnSndEffectResRestore(Int32 slot);

	public delegate void OnSndEffectResVol(Int32 slot, Int32 ObjNo, Int32 attr, Int32 vol);

	public delegate void OnSndEffectResVolIntpl(Int32 slot, Int32 ObjNo, Int32 attr, Int32 ticks, Int32 to);

	public delegate void OnSndEffectResVolAll(Int32 vol);

	public delegate void OnSndEffectResVolIntplAll(Int32 ticks, Int32 to);

	private delegate void OnCreateFinish(SoundProfile soundProfile);

	private delegate void OnGetSoundProfileFinish(SoundProfile soundProfile);
}
