using System;
using System.Collections.Generic;
using UnityEngine;
using FF9;
using Memoria.Data;

public static class FF9Snd
{
	static FF9Snd()
	{
		// Note: this type is marked as 'beforefieldinit'.
		FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9AllSoundDispatch);
	}

	public static Int32 ff9snd_get_argc(Int32 _cmd)
	{
		return _cmd >> 14;
	}

	public static Int32 ff9snd_get_datatype(Int32 _parm)
	{
		return (Int32)(((_parm >> 12 & 3) == 0) ? 7 : ((Int32)(((_parm & 8192) == 0) ? 8 : 24)));
	}

	public static Int32 ff9snd_song_play(Int32 _songid)
	{
		return FF9Snd.FF9SoundArg0(0, _songid);
	}

	public static Int32 ff9snd_song_stop(Int32 _songid)
	{
		return FF9Snd.FF9SoundArg0(256, _songid);
	}

	public static Int32 ff9snd_song_vol(Int32 _songid, Int32 _vol)
	{
		return FF9Snd.FF9SoundArg1(16897, _songid, _vol);
	}

	public static Int32 ff9snd_song_vol_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(33537, _songid, _ticks, _to);
	}

	public static Int32 ff9snd_song_vol_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9snd_song_tempo(Int32 _songid, Int32 _tempo)
	{
		return FF9Snd.FF9SoundArg1(16898, _songid, _tempo);
	}

	public static Int32 ff9snd_song_tempo_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(33538, _songid, _ticks, _to);
	}

	public static Int32 ff9snd_song_tempo_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9snd_song_pitch(Int32 _songid, Int32 _pitch)
	{
		return FF9Snd.FF9SoundArg1(16899, _songid, _pitch);
	}

	public static Int32 ff9snd_song_pitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(33539, _songid, _ticks, _to);
	}

	public static Int32 ff9snd_song_pitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9snd_song_tempopitch(Int32 _songid, Int32 _tempopitch)
	{
		return FF9Snd.FF9SoundArg1(16900, _songid, _tempopitch);
	}

	public static Int32 ff9snd_song_tempopitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(33540, _songid, _ticks, _to);
	}

	public static Int32 ff9snd_song_tempopitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(1028, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9snd_sndeffect_play(Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		return FF9Snd.FF9SoundArg3(53248, _sndeffectid, _attr, _pos, _vol);
	}

	public static Int32 ff9snd_sndeffect_stop(Int32 _sndeffectid, Int32 _attr)
	{
		return FF9Snd.FF9SoundArg1(20736, _sndeffectid, _attr);
	}

	public static Int32 ff9snd_sndeffect_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		return FF9Snd.FF9SoundArg2(37377, _sndeffectid, _attr, _vol);
	}

	public static Int32 ff9snd_sndeffect_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(54017, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9snd_sndeffect_vol_all(Int32 _vol)
	{
		return FF9Snd.FF9SoundArg1(21761, -1, _vol);
	}

	public static Int32 ff9snd_sndeffect_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(38401, -1, _ticks, _to);
	}

	public static Int32 ff9snd_sndeffect_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		return FF9Snd.FF9SoundArg2(37381, _sndeffectid, _attr, _pos);
	}

	public static Int32 ff9snd_sndeffect_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(54021, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9snd_sndeffect_pos_all(Int32 _pos)
	{
		return FF9Snd.FF9SoundArg1(21765, -1, _pos);
	}

	public static Int32 ff9snd_sndeffect_pos_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(38405, -1, _ticks, _to);
	}

	public static Int32 ff9snd_sndeffect_pitch(Int32 _sndeffectid, Int32 _attr, Int32 _pitch)
	{
		return FF9Snd.FF9SoundArg2(37379, _sndeffectid, _attr, _pitch);
	}

	public static Int32 ff9snd_sndeffect_pitch_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg3(54019, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9snd_sndeffect_pitch_all(Int32 _pitch)
	{
		return FF9Snd.FF9SoundArg1(21763, -1, _pitch);
	}

	public static Int32 ff9snd_sndeffect_pitch_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9SoundArg2(38403, -1, _ticks, _to);
	}

	public static Int32 FF9SoundArg0(Int32 _parmtype, Int32 _objno)
	{
		return FF9Snd.sndFuncPtr(_parmtype, _objno, 0, 0, 0);
	}

	public static Int32 FF9SoundArg1(Int32 _parmtype, Int32 _objno, Int32 _arg1)
	{
		return FF9Snd.sndFuncPtr(_parmtype, _objno, _arg1, 0, 0);
	}

	public static Int32 FF9SoundArg2(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2)
	{
		return FF9Snd.sndFuncPtr(_parmtype, _objno, _arg1, _arg2, 0);
	}

	public static Int32 FF9SoundArg3(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		return FF9Snd.sndFuncPtr(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	private static Int32 DMSMakeID(FF9Snd.DMS_DATATYPE type, Int32 ObjNo)
	{
		return 99;
	}

	private static Int64 DMSGetIDNo(Int64 _dmsid)
	{
		return _dmsid & 65535L;
	}

	public static Int32 GetCurrentMusicId()
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		return allSoundDispatchPlayer.GetCurrentMusicId();
	}

	public static void ParameterChanger(ref Int32 ParmType, ref Int32 ObjNo, ref Int32 Arg1, ref Int32 Arg2, ref Int32 Arg3)
	{
		if ((ParmType >> 12 & 3) == 0)
		{
			if (ObjNo == 93)
				ObjNo = 5;
			else if (ObjNo == 137)
				ObjNo = 10;
			else if (ObjNo == 78)
				ObjNo = 22;
		}
		if ((ParmType >> 12 & 3) == 1)
		{
			if (ObjNo == 3067 || ObjNo == 3068)
				ObjNo = 1748;
			else if (ObjNo == 293)
				ObjNo = 633;
//			else if (ObjNo == 1545) [DV] => When Vivi cast fire on Garnet's hood in CD1 (field n°67)
//				ObjNo = 58; => Third sound file of SFX Fire_Sword (se020006)
			else if (ObjNo == 679)
				ObjNo = 9010197;
			else if (ObjNo == 1230 || ObjNo == 1231)
			{
				FF9Snd.FF9AllSoundDispatch(ParmType, 9000023, Arg1, Arg2, Arg3);
				FF9Snd.FF9AllSoundDispatch(ParmType, 9000024, Arg1, Arg2, Arg3);
				FF9Snd.FF9AllSoundDispatch(ParmType, 9000025, Arg1, Arg2, Arg3);
			}
			else if (ObjNo == 681)
				ObjNo = 3092;
			else if (ObjNo == 672)
				ObjNo = 9010138;
//			else if (ObjNo == 678) // [DV] => Battle Sound Slam (ex : Edge from Tantarian)
//				ObjNo = 725; => Battle Sound Slap (ex : Strike from Zombie)
            else if (ObjNo == 3084)
				ObjNo = 370;
		}
	}

	public static Boolean GetIsExtEnvObjNo(Int32 ObjNo)
	{
		return ObjNo == 642 || ObjNo == 618 || ObjNo == 347 || ObjNo == 347 || ObjNo == 656 || ObjNo == 650 || ObjNo == 650 || ObjNo == 9060080 || ObjNo == 9060084 || ObjNo == 9060087 || ObjNo == 1495 || ObjNo == 9070035 || ObjNo == 9080033 || ObjNo == 1397 || ObjNo == 9080075 || ObjNo == 9090033 || ObjNo == 9090097 || ObjNo == 9100097 || ObjNo == 9100105 || ObjNo == 9100107 || ObjNo == 9110004 || ObjNo == 9110015 || ObjNo == 9120016 || ObjNo == 1887 || ObjNo == 410 || ObjNo == 1461;
	}

	public static Boolean IsShiftPitchInFastForwardMode(Int32 ObjNo)
	{
		return ObjNo != 103 && ObjNo != 1044 && ObjNo != 636 && ObjNo != 635 && ObjNo != 634 && ObjNo != 103 && ObjNo != 101 && ObjNo != 105 && ObjNo != 109 && ObjNo != 683 && ObjNo != 1043 && ObjNo != 108 && ObjNo != 107 && ObjNo != 106 && ObjNo != 1044 && ObjNo != 1046 && ObjNo != 1045 && ObjNo != 1047 && ObjNo != 1261 && ObjNo != 3096 && ObjNo != 102 && ObjNo != 101 && ObjNo != 103 && ObjNo != 682 && ObjNo != 682 && ObjNo != 1362 && ObjNo != 1853 && ObjNo != 1854 && ObjNo != 1855 && ObjNo != 1856 && ObjNo != 1857 && ObjNo != 1858 && ObjNo != 1859 && ObjNo != 1852 && ObjNo != 1861 && ObjNo != 1862 && ObjNo != 1863 && ObjNo != 1864 && ObjNo != 2331 && ObjNo != 2332 && ObjNo != 2333 && !FF9Snd.GetIsExtEnvObjNo(ObjNo);
	}

	public static Int32 GetExtEnvObjNo(Int32 ObjNo)
	{
		if (ObjNo == 643)
		{
			return 642;
		}
		if (ObjNo == 255)
		{
			return 618;
		}
		if (ObjNo == 355)
		{
			return 347;
		}
		if (ObjNo == 275)
		{
			return 347;
		}
		if (ObjNo == 661)
		{
			return 656;
		}
		if (ObjNo == 652)
		{
			return 650;
		}
		if (ObjNo == 736)
		{
			return 650;
		}
		if (ObjNo == 1490)
		{
			return 9060080;
		}
		if (ObjNo == 1493)
		{
			return 9060084;
		}
		if (ObjNo == 1511)
		{
			return 9060087;
		}
		if (ObjNo == 1569)
		{
			return 1495;
		}
		if (ObjNo == 1321)
		{
			return 9070035;
		}
		if (ObjNo == 1366)
		{
			return 9080033;
		}
		if (ObjNo == 1396)
		{
			return 1397;
		}
		if (ObjNo == 1406)
		{
			return 9080075;
		}
		if (ObjNo == 1431)
		{
			return 9090033;
		}
		if (ObjNo == 1993)
		{
			return 9090097;
		}
		if (ObjNo == 2287)
		{
			return 9100097;
		}
		if (ObjNo == 2292)
		{
			return 9100105;
		}
		if (ObjNo == 2293)
		{
			return 9100107;
		}
		if (ObjNo == 1877)
		{
			return 9110004;
		}
		if (ObjNo == 2648)
		{
			return 9110015;
		}
		if (ObjNo == 2193)
		{
			return 9120016;
		}
		if (ObjNo == 1888)
		{
			return 1887;
		}
		return -1;
	}

	private static Int32 GetOffsetMillisecondFromFF9SOUND_SONG_SKIPPHRASE(Int32 ObjNo, Int32 _phrase)
	{
		Int32 result = -1;
		Int32 gMode = PersistenSingleton<EventEngine>.Instance.gMode;
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		if (gMode == 1 && (fldMapNo == 613 || fldMapNo == 615) && ObjNo == 113 && _phrase == 14)
		{
			result = 48000;
		}
		else if (gMode == 1 && fldMapNo == 3010 && ObjNo == 145 && _phrase == 11)
		{
			result = 16500;
		}
		return result;
	}

	private static void SetSfxResDelegate(AllSoundDispatchPlayer player)
	{
		if (player.onSndEffectResPlay == null)
		{
			player.onSndEffectResPlay = new AllSoundDispatchPlayer.OnSndEffectResPlay(FF9Snd.OnSndEffectResPlay);
			player.onSndEffectResStop = new AllSoundDispatchPlayer.OnSndEffectResStop(FF9Snd.OnSndEffectResStop);
			player.onSndEffectResVol = new AllSoundDispatchPlayer.OnSndEffectResVol(FF9Snd.OnSndEffectResVol);
			player.onSndEffectResVolIntpl = new AllSoundDispatchPlayer.OnSndEffectResVolIntpl(FF9Snd.OnSndEffectResVolIntpl);
			player.onSndEffectResStopCurrent = new AllSoundDispatchPlayer.OnSndEffectResStopCurrent(FF9Snd.OnSndEffectResStopCurrent);
			player.onSndEffectResSuspend = new AllSoundDispatchPlayer.OnSndEffectResSuspend(FF9Snd.OnSndEffectResSuspend);
			player.onSndEffectResRestore = new AllSoundDispatchPlayer.OnSndEffectResRestore(FF9Snd.OnSndEffectResRestore);
			player.onSndEffectResVolAll = new AllSoundDispatchPlayer.OnSndEffectResVolAll(FF9Snd.OnSndEffectResVolAll);
			player.onSndEffectResVolIntplAll = new AllSoundDispatchPlayer.OnSndEffectResVolIntplAll(FF9Snd.OnSndEffectResVolIntplAll);
		}
	}

	private static void OnSndEffectResPlay(Int32 slot, Int32 ObjNo, Int32 attr, Int32 pos, Int32 vol)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(ObjNo);
		if (extEnvObjNo != -1)
		{
			SoundLib.Log("OnSndEffectResPlay " + extEnvObjNo);
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(extEnvObjNo, attr);
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(extEnvObjNo, attr, pos, vol);
			if (extEnvObjNo == 618)
			{
				SoundLib.Log("PLAY " + 410);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(410, attr);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(410, attr, pos, vol);
			}
		}
	}

	private static void OnSndEffectResStop(Int32 slot, Int32 ObjNo, Int32 attr)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(ObjNo);
		if (extEnvObjNo != -1)
		{
			SoundLib.Log("OnSndEffectResStop " + extEnvObjNo);
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(extEnvObjNo, attr);
			if (extEnvObjNo == 618)
			{
				SoundLib.Log("STOP " + 410);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(410, attr);
			}
		}
	}

	private static void OnSndEffectResVol(Int32 slot, Int32 ObjNo, Int32 attr, Int32 vol)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(ObjNo);
		if (extEnvObjNo != -1)
		{
			SoundLib.Log("OnSndEffectResVol " + extEnvObjNo);
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL(extEnvObjNo, attr, vol);
			if (extEnvObjNo == 618)
			{
				SoundLib.Log("VOL " + 410);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL(410, attr, vol);
			}
		}
	}

	private static void OnSndEffectResVolIntpl(Int32 slot, Int32 ObjNo, Int32 attr, Int32 ticks, Int32 to)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(ObjNo);
		if (extEnvObjNo != -1)
		{
			SoundLib.Log("OnSndEffectResVolIntpl " + extEnvObjNo);
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL_INTPL(extEnvObjNo, attr, ticks, to);
			if (extEnvObjNo == 618)
			{
				SoundLib.Log("INTPL " + 410);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL_INTPL(410, attr, ticks, to);
			}
		}
	}

	private static void OnSndEffectResStopCurrent()
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		for (Int32 i = 0; i < 2; i++)
		{
			if (allSoundDispatchPlayer.sfxResSlot[i] != null)
			{
				Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(allSoundDispatchPlayer.sfxResSlot[i].ObjNo);
				if (extEnvObjNo != -1)
				{
					SoundLib.Log("OnSndEffectResStopCurrent " + extEnvObjNo);
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(extEnvObjNo, 0);
					if (extEnvObjNo == 618)
					{
						SoundLib.Log("STOP CURRENT " + 410);
						allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(410, 0);
					}
				}
			}
		}
	}

	private static void OnSndEffectResSuspend(Int32 slot)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		if (allSoundDispatchPlayer.sfxResSlot[slot] != null)
		{
			Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(allSoundDispatchPlayer.sfxResSlot[slot].ObjNo);
			if (extEnvObjNo != -1)
			{
				SoundLib.Log("OnSndEffectResSuspend " + extEnvObjNo);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(extEnvObjNo, 0);
				if (extEnvObjNo == 618)
				{
					SoundLib.Log("SUSPEND " + 410);
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(410, 0);
				}
			}
		}
	}

	private static void OnSndEffectResRestore(Int32 slot)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		if (allSoundDispatchPlayer.sfxResSlot[slot] != null)
		{
			Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(allSoundDispatchPlayer.sfxResSlot[slot].ObjNo);
			if (extEnvObjNo != -1)
			{
				SoundLib.Log("OnSndEffectResRestore " + extEnvObjNo);
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(extEnvObjNo, 0, allSoundDispatchPlayer.sfxResSlot[slot].SndEffectPos, allSoundDispatchPlayer.sfxResSlot[slot].SndEffectVol);
				if (extEnvObjNo == 618)
				{
					SoundLib.Log("RESTORE " + 410);
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(410, 0, allSoundDispatchPlayer.sfxResSlot[slot].SndEffectPos, allSoundDispatchPlayer.sfxResSlot[slot].SndEffectVol);
				}
			}
		}
	}

	private static void OnSndEffectResVolAll(Int32 vol)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		for (Int32 i = 0; i < 2; i++)
		{
			if (allSoundDispatchPlayer.sfxResSlot[i] != null)
			{
				Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(allSoundDispatchPlayer.sfxResSlot[i].ObjNo);
				if (extEnvObjNo != -1)
				{
					SoundLib.Log("OnSndEffectResVolAll " + extEnvObjNo);
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL(extEnvObjNo, 0, vol);
				}
			}
		}
	}

	private static void OnSndEffectResVolIntplAll(Int32 ticks, Int32 to)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		for (Int32 i = 0; i < 2; i++)
		{
			if (allSoundDispatchPlayer.sfxResSlot[i] != null)
			{
				Int32 extEnvObjNo = FF9Snd.GetExtEnvObjNo(allSoundDispatchPlayer.sfxResSlot[i].ObjNo);
				if (extEnvObjNo != -1)
				{
					SoundLib.Log("OnSndEffectResVolIntplAll " + extEnvObjNo);
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL_INTPL(extEnvObjNo, 0, ticks, to);
				}
			}
		}
	}

	public static void ff9fieldSoundSuspendAllResidentSndEffect()
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		for (Int32 i = 0; i < 2; i++)
			if (allSoundDispatchPlayer.sfxResSlot[i] != null)
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_SUSPEND(i);
	}

	public static void ff9fieldSoundRestoreAllResidentSndEffect()
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		for (Int32 i = 0; i < 2; i++)
			if (allSoundDispatchPlayer.sfxResSlot[i] != null)
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_RESTORE(i);
	}

	public static Int32 FF9AllSoundDispatch(Int32 ParmType, Int32 ObjNo, Int32 Arg1, Int32 Arg2, Int32 Arg3)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		FF9Snd.SetSfxResDelegate(allSoundDispatchPlayer);
		try
		{
			ParmType &= 65535;
			Int32 slot = ParmType >> 6 & 1;
			SoundLib.Log($"ParmType: {ParmType} ObjNo: {ObjNo}, Arg1: {Arg1}, Arg2: {Arg2}, Arg3: {Arg3}");
			FF9Snd.ParameterChanger(ref ParmType, ref ObjNo, ref Arg1, ref Arg2, ref Arg3);
			Int32 num = ParmType & 65471;
			switch (num)
			{
				case FF9SOUND_SONG_PLAY: // 0
				{
					SoundLib.Log("FF9SOUND_SONG_PLAY");
					allSoundDispatchPlayer.FF9SOUND_SONG_PLAY(ObjNo, 127, 0);
					break;
				}
				case FF9SOUND_SONG_STOP: // 256
				{
					SoundLib.Log("FF9SOUND_SONG_STOP");
					allSoundDispatchPlayer.FF9SOUND_SONG_STOP(ObjNo);
					break;
				}
				case FF9SOUND_SONG_STOPCURRENT: // 265
				{
					SoundLib.Log("FF9SOUND_SONG_STOPCURRENT");
					allSoundDispatchPlayer.FF9SOUND_SONG_STOPCURRENT();
					break;
				}
				case FF9SOUND_SONG_NULL: // 520
				{
					SoundLib.Log("FF9SOUND_SONG_NULL means ignore");
					break;
				}
				case FF9SOUND_SONG_TEMPOPITCH_FADE: // 1028
				{
					SoundLib.Log("Call FF9SOUND_SONG_TEMPOPITCH_FADE stubbed case");
					break;
				}
				case FF9SOUND_SONG_LOAD: // 1792
				{
					SoundLib.Log("FF9SOUND_SONG_LOAD");
					allSoundDispatchPlayer.FF9SOUND_SONG_LOAD(ObjNo);
					break;
				}
				case FF9SOUND_SONG_SUSPEND: // 2048
				{
					SoundLib.Log("FF9SOUND_SONG_SUSPEND");
					allSoundDispatchPlayer.FF9SOUND_SONG_SUSPEND(ObjNo, false);
					break;
				}
				case FF9SOUND_SONG_RESTORE: // 2304
				{
					SoundLib.Log("FF9SOUND_SONG_RESTORE");
					allSoundDispatchPlayer.FF9SOUND_SONG_RESTORE();
					break;
				}
				case FF9SOUND_SONG_JUMPPOINT: // 2566
				{
					SoundLib.Log("FF9SOUND_SONG_JUMPPOINT");
					SoundLib.Log("ObjNo: " + ObjNo);
					allSoundDispatchPlayer.FF9SOUND_SONG_JUMPPOINT();
					break;
				}
				case FF9SOUND_SYNC: // 3072
				{
					SoundLib.Log("Call SOUND_SYNC, stub");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_STOPCURRENT: // 4489
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_STOPCURRENT");
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
					break;
				}
				case FF9SOUND_SNDEFFECT_NULL: // 4616
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_NULL means ignore");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_SUSPEND: // 6272
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_SUSPEND");
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_SUSPEND(slot);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_RESTORE: // 6528
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_RESTORE");
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_RESTORE(slot);
					break;
				}
				case FF9SOUND_STREAM_STOP: // 8448
				{
					SoundLib.Log("FF9SOUND_STREAM_STOP");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_STREAM_STOP should be 0, but ObjNo: " + ObjNo);

					allSoundDispatchPlayer.FF9SOUND_STREAM_STOP();
					break;
				}
				case FF9SOUND_STREAM_NULL: // 8712
				{
					SoundLib.Log("Call FF9SOUND_STREAM_NULL, stub");
					break;
				}
				case FF9SOUND_SONG_VOL: // 16897
				{
					SoundLib.Log("FF9SOUND_SONG_VOL");
					Int32 vol = Arg1;
					allSoundDispatchPlayer.FF9SOUND_SONG_VOL(ObjNo, vol);
					break;
				}
				case FF9SOUND_SONG_TEMPO: // 16898
				{
					SoundLib.Log("Call FF9SOUND_SONG_TEMPO stubbed case");
					break;
				}
				case FF9SOUND_SONG_PITCH: // 16899
				{
					SoundLib.Log("FF9SOUND_SONG_PITCH");
					Int32 pitch = Arg1;
					allSoundDispatchPlayer.FF9SOUND_SONG_PITCH(ObjNo, pitch);
					break;
				}
				case FF9SOUND_SONG_TEMPOPITCH: // 16900
				{
					SoundLib.Log("Call FF9SOUND_SONG_TEMPOPITCH stubbed case");
					break;
				}
				case FF9SOUND_SONG_SKIPPHRASE: // 16903
				{
					SoundLib.Log("FF9SOUND_SONG_SKIPPHRASE");
					Int32 num10 = Arg1;
					SoundLib.Log("_phrase: " + num10);
					Int32 offsetMillisecondFromFF9SOUND_SONG_SKIPPHRASE = FF9Snd.GetOffsetMillisecondFromFF9SOUND_SONG_SKIPPHRASE(ObjNo, num10);
					if (offsetMillisecondFromFF9SOUND_SONG_SKIPPHRASE != -1)
					{
						allSoundDispatchPlayer.FF9SOUND_SONG_SKIPPHRASE_MILLISEC(ObjNo, offsetMillisecondFromFF9SOUND_SONG_SKIPPHRASE);
					}

					break;
				}
				case FF9SOUND_SNDEFFECT_STOP: // 20736
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_STOP");
					Int32 attr6 = Arg1;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(ObjNo, attr6);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_STOP: // 20864
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_STOP");
					Int32 timeMsec = Arg1;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOP(slot, ObjNo, timeMsec);
					break;
				}
				case FF9SOUND_SNDEFFECT_VOL_ALL: // 21761
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_VOL_ALL");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_SNDEFFECT_VOL_ALL should be 0, but ObjNo: " + ObjNo);

					Int32 vol2 = Arg1;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL_ALL(vol2);
					break;
				}
				case FF9SOUND_SNDEFFECT_PITCH_ALL: // 21763
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_PITCH_ALL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECT_POS_ALL: // 21765
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_POS_ALL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_VOL_ALL: // 21889
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_VOL_ALL");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_SNDEFFECTRES_VOL_ALL should be 0, but ObjNo: " + ObjNo);

					Int32 vol3 = Arg1;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_VOL_ALL(vol3);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_PITCH_ALL: // 21891
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_PITCH_ALL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_POS_ALL: // 21893
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_POS_ALL stubbed case");
					break;
				}
				case FF9SOUND_STREAM_VOL: // 25089
				{
					SoundLib.Log("FF9SOUND_STREAM_VOL");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_STREAM_VOL should be 0, but ObjNo: " + ObjNo);

					Int32 vol7 = Arg1;
					allSoundDispatchPlayer.FF9SOUND_STREAM_VOL(vol7);
					break;
				}
				case FF9SOUND_STREAM_POS: // 25093
				{
					SoundLib.Log("FF9SOUND_STREAM_POS");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_STREAM_POS should be 0, but ObjNo: " + ObjNo);

					SoundLib.Log("No implementation!");
					break;
				}
				case FF9SOUND_STREAMFMV_VOL: // 25098
				{
					SoundLib.Log("FF9SOUND_STREAMFMV_VOL");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_STREAMFMV_VOL should be 0, but ObjNo: " + ObjNo);

					SoundLib.Log("No implementation!");
					break;
				}
				case FF9SOUND_STREAM_REVERB: // 25100
				{
					SoundLib.Log("FF9SOUND_STREAM_REVERB");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_STREAM_REVERB should be 0, but ObjNo: " + ObjNo);

					SoundLib.Log("No implementation!");
					break;
				}
				case FF9SOUND_INSTR_LOAD: // 30464
				{
					SoundLib.Log("Call FF9SOUND_INSTR_LOAD, stub");
					break;
				}
				case FF9SOUND_SONG_VOL_INTPL: // 33537
				{
					SoundLib.Log("FF9SOUND_SONG_VOL_INTPL");
					Int32 ticks4 = Arg1;
					Int32 to4 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SONG_VOL_INTPL(ObjNo, ticks4, to4);
					break;
				}
				case FF9SOUND_SONG_TEMPO_INTPL: // 33538
				{
					SoundLib.Log("Call FF9SOUND_SONG_TEMPO_INTPL stubbed case");
					break;
				}
				case FF9SOUND_SONG_PITCH_INTPL: // 33539
				{
					SoundLib.Log("FF9SOUND_SONG_PITCH_INTPL");
					Int32 tick = Arg1;
					Int32 to5 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SONG_PITCH_INTPL(ObjNo, tick, to5);
					break;
				}
				case FF9SOUND_SONG_TEMPOPITCH_INTPL: // 33540
				{
					SoundLib.Log("Call FF9SOUND_SONG_TEMPOPITCH_INTPL stubbed case");
					break;
				}
				case FF9SOUND_SONG_VOL_INTPLALL: // 34305
				{
					SoundLib.Log("FF9SOUND_SONG_VOL_INTPLALL");
					Int32 ticks7 = Arg1;
					Int32 to9 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SONG_VOL_INTPLALL(ticks7, to9);
					break;
				}
				case FF9SOUND_SNDEFFECT_VOL: // 37377
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_VOL");
					Int32 attr = Arg1;
					Int32 vol4 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL(ObjNo, attr, vol4);
					break;
				}
				case FF9SOUND_SNDEFFECT_PITCH: // 37379
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_PITCH");
					Int32 attr7 = Arg1;
					Int32 pitch2 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PITCH(ObjNo, attr7, pitch2);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_VOL: // 37505
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_VOL");
					Int32 attr2 = Arg1;
					Int32 vol5 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_VOL(slot, ObjNo, attr2, vol5);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_PITCH: // 37507
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_PITCH stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_POS: // 37509
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_POS stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECT_POS: // 37381
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_POS stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECT_VOL_INTPLALL: // 38401
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_VOL_INTPLALL");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_SNDEFFECT_VOL_INTPLALL should be 0, but ObjNo: " + ObjNo);

					Int32 ticks = Arg1;
					Int32 to = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL_INTPLALL(ticks, to);
					break;
				}
				case FF9SOUND_SNDEFFECT_PITCH_INTPLALL: // 38403
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_PITCH_INTPLALL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECT_POS_INTPLALL: // 38405
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_POS_INTPLALL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_VOL_INTPLALL: // 38529
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_VOL_INTPLALL");
					if (ObjNo != -1)
						SoundLib.Log("ObjNo for FF9SOUND_SNDEFFECTRES_VOL_INTPLALL should be 0, but ObjNo: " + ObjNo);

					Int32 ticks2 = Arg1;
					Int32 to2 = Arg2;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_VOL_INTPLALL(ticks2, to2);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_PITCH_INTPLALL: // 38531
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_PITCH_INTPLALL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_POS_INTPLALL: // 38533
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_POS_INTPLALL stubbed case");
					break;
				}
				case FF9SOUND_STREAM_PLAY: // 40960
				{
					SoundLib.Log("FF9SOUND_STREAM_PLAY");
					Int32 streamid = ObjNo;
					Int32 num8 = Arg1;
					Int32 num9 = Arg2;
					SoundLib.Log($"ObjNo: {ObjNo},  pos: {num8}, reverb: {num9}");
					if (!SoundLib.SoundEffectIsMute)
						allSoundDispatchPlayer.FF9SOUND_STREAM_PLAY(streamid, num8, num9);

					break;
				}
				case FF9SOUND_SONG_VOL_FADE: // 50177
				{
					SoundLib.Log("FF9SOUND_SONG_VOL_FADE");
					Int32 ticks5 = Arg1;
					Int32 from = Arg2;
					Int32 to6 = Arg3;
					allSoundDispatchPlayer.FF9SOUND_SONG_VOL_FADE(ObjNo, ticks5, from, to6);
					break;
				}
				case FF9SOUND_SONG_TEMPO_FADE: // 50178
				{
					SoundLib.Log("Call FF9SOUND_SONG_TEMPO_FADE stubbed case");
					break;
				}
				case FF9SOUND_SONG_PITCH_FADE: // 50179
				{
					SoundLib.Log("FF9SOUND_SONG_PITCH_FADE");
					Int32 tick2 = Arg1;
					Int32 from2 = Arg2;
					Int32 to7 = Arg3;
					allSoundDispatchPlayer.FF9SOUND_SONG_PITCH_FADE(ObjNo, tick2, from2, to7);
					break;
				}
				case FF9SOUND_SONG_VOL_FADEALL: // 51969
				{
					SoundLib.Log("FF9SOUND_SONG_VOL_FADEALL");
					Int32 ticks6 = Arg1;
					Int32 from3 = Arg2;
					Int32 to8 = Arg3;
					allSoundDispatchPlayer.FF9SOUND_SONG_VOL_FADEALL(ticks6, from3, to8);
					break;
				}
				case FF9SOUND_SNDEFFECT_PLAY: // 53248
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_PLAY");
					Int32 attr = Arg1;
					Int32 pos = Arg2;
					Int32 volume = Arg3;
					SoundLib.Log($"ObjNo: {ObjNo}, attr: {attr}  pos: {pos}, vol: {volume}");
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(ObjNo, attr, pos, volume);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_PLAY: // 53376
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_PLAY");
					Int32 timeMsec = Arg1;
					Int32 pos = Arg2;
					Int32 volume = Arg3;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_PLAY(slot, ObjNo, timeMsec, pos, volume);
					break;
				}
				case FF9SOUND_SNDEFFECT_VOL_INTPL: // 54017
				{
					SoundLib.Log("FF9SOUND_SNDEFFECT_VOL_INTPL");
					Int32 attr3 = Arg1;
					Int32 ticks3 = Arg2;
					Int32 to3 = Arg3;
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_VOL_INTPL(ObjNo, attr3, ticks3, to3);
					break;
				}
				case FF9SOUND_SNDEFFECT_PITCH_INTPL: // 54019
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_PITCH_INTPL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECT_POS_INTPL: // 54021
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECT_POS_INTPL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_VOL_INTPL: // 54145
				{
					SoundLib.Log("FF9SOUND_SNDEFFECTRES_VOL_INTPL");
					Int32 unused = Arg1;
					Int32 ticks = Arg2;
					Int32 volume = Arg3;
					SoundLib.Log($"ObjNo: {ObjNo}, attr: {unused}  ticks: {ticks}, to: {volume}");
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_VOL_INTPL(slot, ObjNo, unused, ticks, volume);
					break;
				}
				case FF9SOUND_SNDEFFECTRES_PITCH_INTPL: // 54147
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_PITCH_INTPL stubbed case");
					break;
				}
				case FF9SOUND_SNDEFFECTRES_POS_INTPL: // 54149
				{
					SoundLib.Log("Call FF9SOUND_SNDEFFECTRES_POS_INTPL stubbed case");
					break;
				}
				case 16901:
				case 16902:
				case 21762:
				case 21764:
				case 21890:
				case 21892:
				case 25099:
				case 37378:
				case 37380:
				case 37506:
				case 37508:
				case 38402:
				case 38404:
				case 38530:
				case 38532:
				case 54018:
				case 54020:
				case 54146:
				case 54148:
				default:
				{
					SoundLib.Log($"Not founded case with ParmType: {ParmType}\nParmType Value: {Convert.ToString(ParmType, 2)}");
					break;
				}
			}
		}
		catch (Exception ex)
		{
			SoundLib.LogWarning($"Music Id: {ObjNo} has Exception: {ex}");
		}

		return 0;
	}

	public static Int32 ff9endsnd_song_play(Int32 _songid)
	{
		return FF9Snd.FF9EndingSoundArg0(0, _songid);
	}

	public static Int32 ff9endsnd_song_stop(Int32 _songid)
	{
		return FF9Snd.FF9EndingSoundArg0(256, _songid);
	}

	public static Int32 ff9endsnd_song_vol(Int32 _songid, Int32 _vol)
	{
		return FF9Snd.FF9EndingSoundArg1(16897, _songid, _vol);
	}

	public static Int32 ff9endsnd_song_vol_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9EndingSoundArg2(33537, _songid, _ticks, _to);
	}

	public static Int32 ff9endsnd_song_vol_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9EndingSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9endsnd_sndeffect_play(Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		return FF9Snd.FF9EndingSoundArg3(53248, _sndeffectid, _attr, _pos, _vol);
	}

	public static Int32 ff9endsnd_sndeffect_stop(Int32 _sndeffectid, Int32 _attr)
	{
		return FF9Snd.FF9EndingSoundArg1(20736, _sndeffectid, _attr);
	}

	public static Int32 ff9endsnd_sndeffect_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		return FF9Snd.FF9EndingSoundArg2(37377, _sndeffectid, _attr, _vol);
	}

	public static Int32 ff9endsnd_sndeffect_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9EndingSoundArg3(54017, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9endsnd_sndeffect_vol_all(Int32 _vol)
	{
		return FF9Snd.FF9EndingSoundArg1(21761, -1, _vol);
	}

	public static Int32 ff9endsnd_sndeffect_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9EndingSoundArg2(38401, -1, _ticks, _to);
	}

	public static Int32 ff9endsnd_sndeffect_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		return FF9Snd.FF9EndingSoundArg2(37381, _sndeffectid, _attr, _pos);
	}

	public static Int32 ff9endsnd_sndeffect_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9EndingSoundArg3(54021, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9endsnd_sndeffect_pos_all(Int32 _pos)
	{
		return FF9Snd.FF9EndingSoundArg1(21765, -1, _pos);
	}

	public static Int32 ff9endsnd_sndeffect_pos_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9EndingSoundArg2(38405, -1, _ticks, _to);
	}

	public static Int32 FF9EndingSoundArg0(Int32 _parmtype, Int32 _objno)
	{
		return FF9Snd.ff9endingSoundDispatch(_parmtype, _objno, 0, 0, 0);
	}

	public static Int32 FF9EndingSoundArg1(Int32 _parmtype, Int32 _objno, Int32 _arg1)
	{
		return FF9Snd.ff9endingSoundDispatch(_parmtype, _objno, _arg1, 0, 0);
	}

	public static Int32 FF9EndingSoundArg2(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2)
	{
		return FF9Snd.ff9endingSoundDispatch(_parmtype, _objno, _arg1, _arg2, 0);
	}

	public static Int32 FF9EndingSoundArg3(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		return FF9Snd.ff9endingSoundDispatch(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	public static Int32 ff9endingSoundDispatch(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		return FF9Snd.FF9MiniGameSoundDispatch(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	public static Int32 FF9BattleSoundDispatch(Int32 ParmType, Int32 ObjNo, Int32 Arg1, Int32 Arg2, Int32 Arg3)
	{
		return FF9Snd.FF9AllSoundDispatch(ParmType, ObjNo, Arg1, Arg2, Arg3);
	}

	public static Int32 FF9BattleSoundGetWeaponSndEffect(Int32 PartyCharNo, FF9BatteSoundWeaponSndEffectType type)
	{
		return FF9Snd.FF9BattleSoundGetWeaponSndEffect02(PartyCharNo, type);
	}

	public static Int32 FF9BattleSoundGetWeaponSndEffect01(Int32 PartyCharNo, FF9BatteSoundWeaponSndEffectType type)
	{
		Int32 weapModel = ff9item.GetItemWeapon(FF9StateSystem.Common.FF9.party.member[PartyCharNo].equip[0]).ModelId;
		for (Int32 i = 0; i < FF9Snd.ff9battleSoundWeaponSndEffect01.GetLength(0); i++)
			if (FF9Snd.ff9battleSoundWeaponSndEffect01[i, 2] == weapModel)
				return FF9Snd.ff9battleSoundWeaponSndEffect01[i, (Int32)type];
		return -1;
	}

	public static Int32 FF9BattleSoundGetWeaponSndEffect02(Int32 PartyCharNo, FF9BatteSoundWeaponSndEffectType type)
	{
		CharacterBattleParameter param = btl_mot.BattleParameterList[FF9StateSystem.Common.FF9.party.member[PartyCharNo].info.serial_no];
		if ((Int32)type >= param.WeaponSound.Length) // For unregistered serial numbers, use weapon sounds directly linked to weapon models
			return FF9BattleSoundGetWeaponSndEffect01(PartyCharNo, type);
		return param.WeaponSound[(Int32)type];
	}

	public static Int32 FF9BattleSoundGetWeaponInstr(Int32 PartyCharNo)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundLoadWeaponSndEffectPC(Int32 PartyCharNo, FF9BattleLoadState[] StatePtr)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundUnloadWeaponSndEffectPC(Int32 PartyCharNo)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundGetSongInstrIDPC(Int32 SongID)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundRemoveSongHdrPC(Byte[] LoadAddr)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int64 ff9battleSoundGetFDPC(Int32 ObjID, Int64[] ObjSizePtr)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1L;
	}

	public static void ff9battleSoundFreeFDPC(Int64 FD)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
	}

	public static Int32 ff9battleSoundLoadCallbackPC()
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundLoadWeaponSndEffectPack(Int32 PartyCharNo, FF9BattleLoadState[] StatePtr)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundGetSongInstrIDPack(Int32 SongID)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundRemoveSongHdrPack(Byte[] LoadAddr)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static Int32 ff9battleSoundLoadCallbackPack()
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	public static void ff9battleSoundMarkFade(Int32 TargetVol, Int32 FadeTime)
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
	}

	public static Int32 ff9battleSoundIsFadeInProgress()
	{
		SoundLib.LogError("Call stubbed function, btlSnd");
		return -1;
	}

	private static Int64 ff9charptr_attr_test(FF9Char _charptr, Int32 _attr)
	{
		return (Int64)((UInt64)_charptr.attr & (UInt64)((Int64)_attr));
	}

	private static UInt16 BGI_TRI_BITS_GET(UInt16 f)
	{
		return (UInt16)(f >> 8);
	}

	private static Int32 DMSMakeID(Int32 _datatype, Int32 _no)
	{
		return (_datatype & 255) << 16 | (_no & 65535);
	}

	public static FF9FieldCharSound ff9fieldSoundGetChar(FF9Char CharPtr, Int32 AnmNo, Int32 Frame)
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		FF9FieldCharSound[] charSoundArray = map.charSoundArray;
		Int32 num = map.charSoundUse - 1;
		for (Int32 i = num; i >= 0; i--)
		{
			FF9FieldCharSound ff9FieldCharSound = charSoundArray[i];
			if (ff9FieldCharSound.uid == CharPtr.evt.uid && ff9FieldCharSound.anmID == (UInt16)AnmNo && ff9FieldCharSound.frame == (Int16)Frame)
			{
				return ff9FieldCharSound;
			}
		}
		return (FF9FieldCharSound)null;
	}

	public static FF9FieldCharSound ff9fieldSoundNewChar(FF9Char CharPtr, Int32 AnmNo, Int32 Frame)
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		FF9FieldCharSound[] charSoundArray = map.charSoundArray;
		if (map.charSoundUse >= FF9Snd.FLDINT_CHAR_SOUNDCOUNT)
		{
			global::Debug.LogError("ff9StateFieldMap.charSoundUse >= FLDINT_CHAR_SOUNDCOUNT, " + map.charSoundUse);
			global::Debug.Log("Fallback, clear CharSoundArray");
			for (Int32 i = 0; i < FF9Snd.FLDINT_CHAR_SOUNDCOUNT; i++)
			{
				charSoundArray[i] = new FF9FieldCharSound();
			}
			global::Debug.Log("Reset charSoundUse");
			map.charSoundUse = 0;
		}
		FF9FieldCharSound ff9FieldCharSound = charSoundArray[map.charSoundUse];
		map.charSoundUse++;
		ff9FieldCharSound.uid = CharPtr.evt.uid;
		ff9FieldCharSound.geoID = CharPtr.evt.model;
		ff9FieldCharSound.anmID = (UInt16)AnmNo;
		ff9FieldCharSound.frame = (Int16)Frame;
		return ff9FieldCharSound;
	}

	public static Int32 ff9fieldSoundDeleteChar(FF9Char CharPtr, Int32 AnmNo, Int32 Frame)
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		FF9FieldCharSound[] charSoundArray = map.charSoundArray;
		Int32 num = map.charSoundUse - 1;
		Int32 i = num;
		FF9FieldCharSound ff9FieldCharSound = map.charSoundArray[num];
		while (i >= 0)
		{
			FF9FieldCharSound ff9FieldCharSound2 = charSoundArray[i];
			if (ff9FieldCharSound2.uid == CharPtr.evt.uid && ff9FieldCharSound2.anmID == (UInt16)AnmNo && ff9FieldCharSound2.frame == (Int16)Frame)
			{
				if (ff9FieldCharSound2 != ff9FieldCharSound)
				{
					ff9FieldCharSound = new FF9FieldCharSound();
				}
				else
				{
					ff9FieldCharSound = new FF9FieldCharSound();
				}
				map.charSoundUse--;
			}
			i--;
		}
		return -1;
	}

	public static void ff9fieldSoundCharService(List<FF9Char> CharArray, Int32 CharCount)
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		FF9FieldCharSound[] charSoundArray = map.charSoundArray;
		Int32 charSoundUse = map.charSoundUse;
		for (Int32 i = CharCount - 1; i >= 0; i--)
		{
			FF9Char ff9Char = CharArray[i];
			if (ff9Char != null)
			{
				if (FF9Snd.ff9charptr_attr_test(ff9Char, 64) == 0L)
				{
					for (Int32 j = charSoundUse - 1; j >= 0; j--)
					{
						FF9FieldCharSound ff9FieldCharSound = charSoundArray[j];
						if (ff9FieldCharSound != null)
						{
							if (ff9FieldCharSound.uid == ff9Char.evt.uid && ff9FieldCharSound.anmID == ff9Char.evt.anim && ff9FieldCharSound.frame == (Int16)ff9Char.evt.animFrame)
							{
								Int16 num = 0;
								Int16 num2 = 0;
								UInt16 num3 = 0;
								Int32 num4 = 0;
								Int32 pos = 128;
								Int32 num5 = 127;
								BGI.BGI_charGetInfo((Int32)ff9Char.evt.uid, ref num, ref num2);
								if (num != -1)
								{
									BGI_TRI_DEF bgi_TRI_DEF = PersistenSingleton<EventEngine>.Instance.fieldmap.bgi.triList[(Int32)num];
									num3 = bgi_TRI_DEF.triFlags;
									num3 = FF9Snd.BGI_TRI_BITS_GET(num3);
								}
								Int64 num6 = (Int64)ff9FieldCharSound.sndEffectID[(Int32)((((Int32)num3 & FF9Snd.BGI_ATTR_STEP2) == 0) ? 0 : 1)];
								Int32 attr = (Int32)(ff9FieldCharSound.uid & 3);
								if ((Int32)ff9FieldCharSound.pitchRand == 1)
								{
									Int32 num7 = UnityEngine.Random.Range(0, 255);
									num4 = (num7 >> 4) - 8;
									num4 *= 3;
								}
								FF9Snd.FF9FieldSoundGetPositionVolume((Int32)ff9Char.evt.pos[0], (Int32)ff9Char.evt.pos[1], (Int32)ff9Char.evt.pos[2], out pos, out num5);
								num5 = (Int32)((Single)num5 * 0.5f);
								FF9Snd.ff9fldsnd_sndeffect_play((Int32)num6, attr, num5, pos);
								if (num4 != 0)
								{
									FF9Snd.ff9fldsnd_sndeffect_pitch((Int32)num6, attr, num4);
								}
							}
						}
					}
				}
			}
		}
	}

	public static Int32 FF9FieldSoundDispatch(Int32 ParmType, Int32 ObjNo, Int32 Arg1, Int32 Arg2, Int32 Arg3)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		ParmType &= 65535;
		FF9Snd.ParameterChanger(ref ParmType, ref ObjNo, ref Arg1, ref Arg2, ref Arg3);
		Int32 num = ParmType & 65471;
		if (num != 0 && num != 1792)
		{
			if (num == 53248)
			{
				if (!FF9Snd.PlayedFieldSoundEffect.Contains(ObjNo))
					FF9Snd.PlayedFieldSoundEffect.Add(ObjNo);
				Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
				if ((fldMapNo == 1911 || fldMapNo == 911) && ObjNo == 1395)
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(1394, 0);
				if (fldMapNo == 157 && ObjNo == 3064)
				{
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(9050122, 0, 0, Arg3);
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_PLAY(9050123, 0, 0, Arg3);
					Arg3 /= 2;
					if (!FF9Snd.PlayedFieldSoundEffect.Contains(9050122))
						FF9Snd.PlayedFieldSoundEffect.Add(9050122);
					if (!FF9Snd.PlayedFieldSoundEffect.Contains(9050123))
						FF9Snd.PlayedFieldSoundEffect.Add(9050123);
				}
				if ((fldMapNo == 302 || fldMapNo == 303 || fldMapNo == 304) && ObjNo == 2491)
					ObjNo = 77;
				//if (ObjNo == 2727) // Choco's beak sound in the lagoon
				//	ObjNo = 2728; // Choco's normal beak sound (grass/ground)
			}
		}
		else
		{
			FF9Snd.BGMFieldSongCounter++;
		}
		return FF9Snd.FF9AllSoundDispatch(ParmType, ObjNo, Arg1, Arg2, Arg3);
	}

	public static void ff9fieldsound_stopall_mapsndeffect(Int32 _mapno)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		foreach (Int32 objNo in FF9Snd.PlayedFieldSoundEffect)
		{
			allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP(objNo, 0);
		}
		FF9Snd.PlayedFieldSoundEffect.Clear();
	}

	public static Int32 ff9fldsnd_song_load(Int32 _songid)
	{
		return FF9Snd.FF9FieldSoundArg0(1792, _songid);
	}

	public static Int32 ff9fldsnd_song_play(Int32 _songid)
	{
		return FF9Snd.FF9FieldSoundArg0(0, _songid);
	}

	public static Int32 ff9fldsnd_song_suspend(Int32 _songid)
	{
		return FF9Snd.FF9FieldSoundArg0(2048, _songid);
	}

	public static Int32 ff9fldsnd_song_restore()
	{
		return FF9Snd.FF9FieldSoundArg0(2304, -1);
	}

	public static Int32 ff9fldsnd_song_stop(Int32 _songid)
	{
		return FF9Snd.FF9FieldSoundArg0(256, _songid);
	}

	public static Int32 ff9fldsnd_song_stopcurrent()
	{
		return FF9Snd.FF9FieldSoundArg0(265, -1);
	}

	public static Int32 ff9fldsnd_song_vol(Int32 _songid, Int32 _vol)
	{
		return FF9Snd.FF9FieldSoundArg1(16897, _songid, _vol);
	}

	public static Int32 ff9fldsnd_song_vol_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(33537, _songid, _ticks, _to);
	}

	public static Int32 ff9fldsnd_song_vol_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9fldsnd_song_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(34305, 0, _ticks, _to);
	}

	public static Int32 ff9fldsnd_song_vol_fadeall(Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(51969, 0, _ticks, _from, _to);
	}

	public static Int32 ff9fldsnd_song_tempo(Int32 _songid, Int32 _tempo)
	{
		return FF9Snd.FF9FieldSoundArg1(16898, _songid, _tempo);
	}

	public static Int32 ff9fldsnd_song_tempo_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(33538, _songid, _ticks, _to);
	}

	public static Int32 ff9fldsnd_song_tempo_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9fldsnd_song_pitch(Int32 _songid, Int32 _pitch)
	{
		return FF9Snd.FF9FieldSoundArg1(16899, _songid, _pitch);
	}

	public static Int32 ff9fldsnd_song_pitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(33539, _songid, _ticks, _to);
	}

	public static Int32 ff9fldsnd_song_pitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9fldsnd_song_tempopitch(Int32 _songid, Int32 _tempopitch)
	{
		return FF9Snd.FF9FieldSoundArg1(16900, _songid, _tempopitch);
	}

	public static Int32 ff9fldsnd_song_tempopitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(33540, _songid, _ticks, _to);
	}

	public static Int32 ff9fldsnd_song_tempopitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(1028, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9fldsnd_song_jumppoint(Int32 _point)
	{
		return FF9Snd.FF9FieldSoundArg0(2566, _point);
	}

	public static Int32 ff9fldsnd_song_skipphrase(Int32 _songid, Int32 _phrase)
	{
		return FF9Snd.FF9FieldSoundArg1(16903, _songid, _phrase);
	}

	public static Int32 ff9fldsnd_sndeffect_play(Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		return FF9Snd.FF9FieldSoundArg3(53248, _sndeffectid, _attr, _pos, _vol);
	}

	public static Int32 ff9fldsnd_sndeffect_stop(Int32 _sndeffectid, Int32 _attr)
	{
		return FF9Snd.FF9FieldSoundArg1(20736, _sndeffectid, _attr);
	}

	public static Int32 ff9fldsnd_sndeffect_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		return FF9Snd.FF9FieldSoundArg2(37377, _sndeffectid, _attr, _vol);
	}

	public static Int32 ff9fldsnd_sndeffect_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(54017, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffect_vol_all(Int32 _vol)
	{
		return FF9Snd.FF9FieldSoundArg1(21761, -1, _vol);
	}

	public static Int32 ff9fldsnd_sndeffect_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(38401, -1, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffect_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		return FF9Snd.FF9FieldSoundArg2(37381, _sndeffectid, _attr, _pos);
	}

	public static Int32 ff9fldsnd_sndeffect_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(54021, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffect_pos_all(Int32 _pos)
	{
		return FF9Snd.FF9FieldSoundArg1(21765, -1, _pos);
	}

	public static Int32 ff9fldsnd_sndeffect_pos_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(38405, -1, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffect_pitch(Int32 _sndeffectid, Int32 _attr, Int32 _pitch)
	{
		return FF9Snd.FF9FieldSoundArg2(37379, _sndeffectid, _attr, _pitch);
	}

	public static Int32 ff9fldsnd_sndeffect_pitch_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(54019, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffect_pitch_all(Int32 _pitch)
	{
		return FF9Snd.FF9FieldSoundArg1(21763, -1, _pitch);
	}

	public static Int32 ff9fldsnd_sndeffect_pitch_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg2(38403, -1, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffectres_slot(Int32 _slot)
	{
		return (_slot & 1) << 6;
	}

	public static Int32 ff9fldsnd_sndeffectres_play(Int32 _slot, Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		return FF9Snd.FF9FieldSoundArg3(53376 | FF9Snd.ff9fldsnd_sndeffectres_slot(_slot), _sndeffectid, _attr, _pos, _vol);
	}

	public static Int32 ff9fldsnd_sndeffectres_suspend(Int32 _slot)
	{
		return FF9Snd.FF9FieldSoundArg0(6272 | FF9Snd.ff9fldsnd_sndeffectres_slot(_slot), _slot);
	}

	public static Int32 ff9fldsnd_sndeffectres_restore(Int32 _slot)
	{
		return FF9Snd.FF9FieldSoundArg0(6528 | FF9Snd.ff9fldsnd_sndeffectres_slot(_slot), _slot);
	}

	public static Int32 ff9fldsnd_sndeffectres_stop(Int32 _sndeffectid, Int32 _attr)
	{
		return FF9Snd.FF9FieldSoundArg1(20864, _sndeffectid, _attr);
	}

	public static Int32 ff9fldsnd_sndeffectres_stopcurrent()
	{
		return FF9Snd.FF9FieldSoundArg0(4489, -1);
	}

	public static Int32 ff9fldsnd_sndeffectres_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		return FF9Snd.FF9FieldSoundArg2(37505, _sndeffectid, _attr, _vol);
	}

	public static Int32 ff9fldsnd_sndeffectres_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(54145, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffectres_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		return FF9Snd.FF9FieldSoundArg2(37509, _sndeffectid, _attr, _pos);
	}

	public static Int32 ff9fldsnd_sndeffectres_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(54149, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9fldsnd_sndeffectres_pitch(Int32 _sndeffectid, Int32 _attr, Int32 _pitch)
	{
		return FF9Snd.FF9FieldSoundArg2(37507, _sndeffectid, _attr, _pitch);
	}

	public static Int32 ff9fldsnd_sndeffectres_pitch_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9FieldSoundArg3(54147, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9fldsnd_stream_play(Int32 _streamid, Int32 _pos, Int32 _reverb)
	{
		return FF9Snd.FF9FieldSoundArg2(40960, _streamid, _pos, _reverb);
	}

	public static Int32 ff9fldsnd_stream_stop()
	{
		return FF9Snd.FF9FieldSoundArg0(8448, -1);
	}

	public static Int32 ff9fldsnd_stream_vol(Int32 _vol)
	{
		return FF9Snd.FF9FieldSoundArg1(25089, -1, _vol);
	}

	public static Int32 ff9fldsnd_stream_pos(Int32 _pos)
	{
		return FF9Snd.FF9FieldSoundArg1(25093, -1, _pos);
	}

	public static Int32 ff9fldsnd_stream_reverb(Int32 _reverb)
	{
		return FF9Snd.FF9FieldSoundArg1(25100, -1, _reverb);
	}

	public static Int32 ff9fldsnd_streamfmv_vol(Int32 _vol)
	{
		return FF9Snd.FF9FieldSoundArg1(25098, -1, _vol);
	}

	public static Int32 ff9fldsnd_instr_load(Int32 _instrid, Int32 _attr, Int32 _slot)
	{
		return FF9Snd.FF9FieldSoundArg2(30464, _instrid, _attr, _slot);
	}

	public static Int32 ff9fldsnd_sync()
	{
		return FF9Snd.FF9FieldSoundArg0(3072, -1);
	}

	public static Int32 FF9FieldSoundArg0(Int32 _parmtype, Int32 _objno)
	{
		return FF9Snd.FF9FieldSoundDispatch(_parmtype, _objno, 0, 0, 0);
	}

	public static Int32 FF9FieldSoundArg1(Int32 _parmtype, Int32 _objno, Int32 _arg1)
	{
		return FF9Snd.FF9FieldSoundDispatch(_parmtype, _objno, _arg1, 0, 0);
	}

	public static Int32 FF9FieldSoundArg2(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2)
	{
		return FF9Snd.FF9FieldSoundDispatch(_parmtype, _objno, _arg1, _arg2, 0);
	}

	public static Int32 FF9FieldSoundArg3(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		return FF9Snd.FF9FieldSoundDispatch(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	public static void FF9FieldSoundGetPositionVolume(Int32 PosX, Int32 PosY, Int32 PosZ, out Int32 PosPtr, out Int32 VolPtr)
	{
		FF9StateFieldMap map = FF9StateSystem.Field.FF9Field.loc.map;
		Int16[] array = new Int16[2];
		Matrix4x4 cam = FF9StateSystem.Common.FF9.cam;
		UInt16 proj = FF9StateSystem.Common.FF9.proj;
		Vector2 projectionOffset = FF9StateSystem.Common.FF9.projectionOffset;
		Vector3 vertex = new Vector3((Single)PosX, (Single)PosY, (Single)PosZ);
		Vector3 vector = PSX.CalculateGTE_RTPT_POS(vertex, Matrix4x4.identity, cam, (Single)proj, projectionOffset, true);
		array[0] = (Int16)vector.x;
		array[1] = (Int16)vector.y;
		Int64 num = (Int64)vector.z;
		num = num / 4L + (Int64)map.charOTOffset;
		if (num < 0L)
		{
			num = 0L;
		}
		else if (num >= 4096L)
		{
			num = 4095L;
		}
		Int32 num2 = (Int32)(num * 128L) / 4096;
		Int32 num3 = (128 - num2) * 2;
		Int32 num4;
		if (array[0] >= 0 && array[0] < FF9_SCREEN_WIDTH)
		{
			num4 = (Int32)array[0] * num3 / FF9_SCREEN_WIDTH;
			num4 = num2 + num4;
		}
		else if (array[0] < 0)
		{
			if (array[0] < -FF9_SCREEN_WIDTH)
			{
				array[0] = -FF9_SCREEN_WIDTH;
			}
			num4 = (Int32)(array[0] - -FF9_SCREEN_WIDTH) * num2 / FF9_SCREEN_WIDTH;
		}
		else
		{
			if (array[0] >= 640)
			{
				array[0] = 639;
			}
			num4 = (Int32)(array[0] - FF9_SCREEN_WIDTH) * num2 / FF9_SCREEN_WIDTH;
			num4 = 255 - num2 + num4;
		}
		PosPtr = (Int32)((UInt16)(num4 & 255));
		Int32 num5 = (Int32)((4096L - num) * 127L) / 4096;
		VolPtr = (Int32)((UInt16)num5);
	}

	public static Int32 FF9MiniGameSoundDispatch(Int32 ParmType, Int32 ObjNo, Int32 Arg1, Int32 Arg2, Int32 Arg3)
	{
		return FF9Snd.FF9AllSoundDispatch(ParmType, ObjNo, Arg1, Arg2, Arg3);
	}

	public static Int32 FF9MiniGameSoundArg0(Int32 _parmtype, Int32 _objno)
	{
		return FF9Snd.FF9MiniGameSoundDispatch(_parmtype, _objno, 0, 0, 0);
	}

	public static Int32 FF9MiniGameSoundArg1(Int32 _parmtype, Int32 _objno, Int32 _arg1)
	{
		return FF9Snd.FF9MiniGameSoundDispatch(_parmtype, _objno, _arg1, 0, 0);
	}

	public static Int32 FF9MiniGameSoundArg2(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2)
	{
		return FF9Snd.FF9MiniGameSoundDispatch(_parmtype, _objno, _arg1, _arg2, 0);
	}

	public static Int32 FF9MiniGameSoundArg3(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		return FF9Snd.FF9MiniGameSoundDispatch(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	public static Int32 ff9minisnd_song_play(Int32 _songid)
	{
		return FF9Snd.FF9MiniGameSoundArg0(0, _songid);
	}

	public static Int32 ff9minisnd_song_stop(Int32 _songid)
	{
		return FF9Snd.FF9MiniGameSoundArg0(256, _songid);
	}

	public static Int32 ff9minisnd_song_vol(Int32 _songid, Int32 _vol)
	{
		return FF9Snd.FF9MiniGameSoundArg1(16897, _songid, _vol);
	}

	public static Int32 ff9minisnd_song_vol_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9MiniGameSoundArg2(33537, _songid, _ticks, _to);
	}

	public static Int32 ff9minisnd_song_vol_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9MiniGameSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9minisnd_sndeffect_play(Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		return FF9Snd.FF9MiniGameSoundArg3(53248, _sndeffectid, _attr, _pos, _vol);
	}

	public static Int32 ff9minisnd_sndeffect_stop(Int32 _sndeffectid, Int32 _attr)
	{
		return FF9Snd.FF9MiniGameSoundArg1(20736, _sndeffectid, _attr);
	}

	public static Int32 ff9minisnd_sndeffect_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		return FF9Snd.FF9MiniGameSoundArg2(37377, _sndeffectid, _attr, _vol);
	}

	public static Int32 ff9minisnd_sndeffect_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9MiniGameSoundArg3(54017, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9minisnd_sndeffect_vol_all(Int32 _vol)
	{
		return FF9Snd.FF9MiniGameSoundArg1(21761, -1, _vol);
	}

	public static Int32 ff9minisnd_sndeffect_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9MiniGameSoundArg2(38401, -1, _ticks, _to);
	}

	public static Int32 ff9minisnd_sndeffect_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		return FF9Snd.FF9MiniGameSoundArg2(37381, _sndeffectid, _attr, _pos);
	}

	public static Int32 ff9minisnd_sndeffect_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9MiniGameSoundArg3(54021, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9minisnd_sndeffect_pos_all(Int32 _pos)
	{
		return FF9Snd.FF9MiniGameSoundArg1(21765, -1, _pos);
	}

	public static Int32 ff9minisnd_sndeffect_pos_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9MiniGameSoundArg2(38405, -1, _ticks, _to);
	}

	public static Int32 ff9wldsnd_song_load(Int32 _songid)
	{
		return FF9Snd.FF9WorldSoundArg0(1792, _songid);
	}

	public static Int32 ff9wldsnd_song_play(Int32 _songid)
	{
		return FF9Snd.FF9WorldSoundArg0(0, _songid);
	}

	public static Int32 ff9wldsnd_song_suspend(Int32 _songid)
	{
		return FF9Snd.FF9WorldSoundArg0(2048, _songid);
	}

	public static Int32 ff9wldsnd_song_restore()
	{
		return FF9Snd.FF9WorldSoundArg0(2304, 0);
	}

	public static Int32 ff9wldsnd_song_stop(Int32 _songid)
	{
		return FF9Snd.FF9WorldSoundArg0(256, _songid);
	}

	public static Int32 ff9wldsnd_song_vol(Int32 _songid, Int32 _vol)
	{
		return FF9Snd.FF9WorldSoundArg1(16897, _songid, _vol);
	}

	public static Int32 ff9wldsnd_song_vol_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(33537, _songid, _ticks, _to);
	}

	public static Int32 ff9wldsnd_song_vol_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9wldsnd_song_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(34305, 0, _ticks, _to);
	}

	public static Int32 ff9wldsnd_song_vol_fadeall(Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(51969, 0, _ticks, _from, _to);
	}

	public static Int32 ff9wldsnd_song_tempo(Int32 _songid, Int32 _tempo)
	{
		return FF9Snd.FF9WorldSoundArg1(16898, _songid, _tempo);
	}

	public static Int32 ff9wldsnd_song_tempo_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(33538, _songid, _ticks, _to);
	}

	public static Int32 ff9wldsnd_song_tempo_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9wldsnd_song_pitch(Int32 _songid, Int32 _pitch)
	{
		return FF9Snd.FF9WorldSoundArg1(16899, _songid, _pitch);
	}

	public static Int32 ff9wldsnd_song_pitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(33539, _songid, _ticks, _to);
	}

	public static Int32 ff9wldsnd_song_pitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9wldsnd_song_tempopitch(Int32 _songid, Int32 _tempopitch)
	{
		return FF9Snd.FF9WorldSoundArg1(16900, _songid, _tempopitch);
	}

	public static Int32 ff9wldsnd_song_tempopitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(33540, _songid, _ticks, _to);
	}

	public static Int32 ff9wldsnd_song_tempopitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(1028, _songid, _ticks, _from, _to);
	}

	public static Int32 ff9wldsnd_song_jumppoint(Int32 _point)
	{
		return FF9Snd.FF9WorldSoundArg0(2566, _point);
	}

	public static Int32 ff9wldsnd_sndeffect_play(Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		return FF9Snd.FF9WorldSoundArg3(53248, _sndeffectid, _attr, _pos, _vol);
	}

	public static Int32 ff9wldsnd_sndeffect_stop(Int32 _sndeffectid, Int32 _attr)
	{
		return FF9Snd.FF9WorldSoundArg1(20736, _sndeffectid, _attr);
	}

	public static Int32 ff9wldsnd_sndeffect_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		return FF9Snd.FF9WorldSoundArg2(37377, _sndeffectid, _attr, _vol);
	}

	public static Int32 ff9wldsnd_sndeffect_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(54017, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9wldsnd_sndeffect_vol_all(Int32 _vol)
	{
		return FF9Snd.FF9WorldSoundArg1(21761, -1, _vol);
	}

	public static Int32 ff9wldsnd_sndeffect_vol_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(38401, -1, _ticks, _to);
	}

	public static Int32 ff9wldsnd_sndeffect_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		return FF9Snd.FF9WorldSoundArg2(37381, _sndeffectid, _attr, _pos);
	}

	public static Int32 ff9wldsnd_sndeffect_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(54021, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9wldsnd_sndeffect_pos_all(Int32 _pos)
	{
		return FF9Snd.FF9WorldSoundArg1(21765, -1, _pos);
	}

	public static Int32 ff9wldsnd_sndeffect_pos_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(38405, -1, _ticks, _to);
	}

	public static Int32 ff9wldsnd_sndeffect_pitch(Int32 _sndeffectid, Int32 _attr, Int32 _pitch)
	{
		return FF9Snd.FF9WorldSoundArg2(37379, _sndeffectid, _attr, _pitch);
	}

	public static Int32 ff9wldsnd_sndeffect_pitch_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg3(54019, _sndeffectid, _attr, _ticks, _to);
	}

	public static Int32 ff9wldsnd_sndeffect_pitch_all(Int32 _pitch)
	{
		return FF9Snd.FF9WorldSoundArg1(21763, -1, _pitch);
	}

	public static Int32 ff9wldsnd_sndeffect_pitch_intplall(Int32 _ticks, Int32 _to)
	{
		return FF9Snd.FF9WorldSoundArg2(38403, -1, _ticks, _to);
	}

	public static Int32 ff9wldsnd_sync()
	{
		return FF9Snd.FF9WorldSoundArg0(3072, 0);
	}

	public static Int32 FF9WorldSoundArg0(Int32 _parmtype, Int32 _objno)
	{
		return FF9Snd.FF9WorldSoundDispatch(_parmtype, _objno, 0, 0, 0);
	}

	public static Int32 FF9WorldSoundArg1(Int32 _parmtype, Int32 _objno, Int32 _arg1)
	{
		return FF9Snd.FF9WorldSoundDispatch(_parmtype, _objno, _arg1, 0, 0);
	}

	public static Int32 FF9WorldSoundArg2(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2)
	{
		return FF9Snd.FF9WorldSoundDispatch(_parmtype, _objno, _arg1, _arg2, 0);
	}

	public static Int32 FF9WorldSoundArg3(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		return FF9Snd.FF9WorldSoundDispatch(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	public static Int32 FF9WorldSoundDispatch(Int32 ParmType, Int32 ObjNo, Int32 Arg1, Int32 Arg2, Int32 Arg3)
	{
		AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
		ParmType &= 65535;
		FF9Snd.ParameterChanger(ref ParmType, ref ObjNo, ref Arg1, ref Arg2, ref Arg3);
		Int32 num = ParmType & 65471;
		if (num == 53248)
		{
			if (ObjNo == 890)
			{
				ObjNo = 891;
			}
			if (ObjNo == 891)
			{
				ObjNo = 2728;
			}
		}
		return FF9Snd.FF9AllSoundDispatch(ParmType, ObjNo, Arg1, Arg2, Arg3);
	}

	public const Int32 FF9_SONG_COUNT = 2;

	public const Int32 FF9_SONG_TICKS_FADEOUT = 30;

	public const Int32 FF9_SNDEFFECT_COUNT = 2;

	public const Int32 FF9_SNDEFFECT_ATTR_SYSTEM = 8388608;

	public const Int32 FF9_SNDEFFECT_ATTR_OTHER = 0;

	public const Int32 FF9_SOUNDPACK_OFFSET_INSTR = 64;

	public const Int32 FF9SOUND_DEFAULTSNDEFFECT_COUNT = 21;

	public const Int32 FF9SOUND_ARGC_0 = 0;

	public const Int32 FF9SOUND_ARGC_1 = 1;

	public const Int32 FF9SOUND_ARGC_2 = 2;

	public const Int32 FF9SOUND_ARGC_3 = 3;

	public const Int32 FF9SOUND_ARGC_MASK = 3;

	public const Int32 FF9SOUND_ARGC_SHIFT = 14;

	public const Int32 FF9SOUND_ARGC_FLAG_0 = 0;

	public const Int32 FF9SOUND_ARGC_FLAG_1 = 16384;

	public const Int32 FF9SOUND_ARGC_FLAG_2 = 32768;

	public const Int32 FF9SOUND_ARGC_FLAG_3 = 49152;

	public const Int32 FF9SOUND_OBJ_SONG = 0;

	public const Int32 FF9SOUND_OBJ_SNDEFFECT = 1;

	public const Int32 FF9SOUND_OBJ_STREAM = 2;

	public const Int32 FF9SOUND_OBJ_INSTR = 3;

	public const Int32 FF9SOUND_OBJ_MASK = 3;

	public const Int32 FF9SOUND_OBJ_SHIFT = 12;

	public const Int32 FF9SOUND_OBJ_FLAG_SONG = 0;

	public const Int32 FF9SOUND_OBJ_FLAG_SNDEFFECT = 4096;

	public const Int32 FF9SOUND_OBJ_FLAG_STREAM = 8192;

	public const Int32 FF9SOUND_OBJ_FLAG_INSTR = 12288;

	public const Int32 FF9SOUND_OP_START = 0;

	public const Int32 FF9SOUND_OP_STOP = 1;

	public const Int32 FF9SOUND_OP_SET = 2;

	public const Int32 FF9SOUND_OP_INTPL = 3;

	public const Int32 FF9SOUND_OP_FADE = 4;

	public const Int32 FF9SOUND_OP_SETALL = 5;

	public const Int32 FF9SOUND_OP_INTPLALL = 6;

	public const Int32 FF9SOUND_OP_LOAD = 7;

	public const Int32 FF9SOUND_OP_SUSPEND = 8;

	public const Int32 FF9SOUND_OP_RESTORE = 9;

	public const Int32 FF9SOUND_OP_SETGLOBAL = 10;

	public const Int32 FF9SOUND_OP_FADEALL = 11;

	public const Int32 FF9SOUND_OP_SYNC = 12;

	public const Int32 FF9SOUND_OP_MASK = 15;

	public const Int32 FF9SOUND_OP_SHIFT = 8;

	public const Int32 FF9SOUND_OP_FLAG_START = 0;

	public const Int32 FF9SOUND_OP_FLAG_STOP = 256;

	public const Int32 FF9SOUND_OP_FLAG_SET = 512;

	public const Int32 FF9SOUND_OP_FLAG_INTPL = 768;

	public const Int32 FF9SOUND_OP_FLAG_FADE = 1024;

	public const Int32 FF9SOUND_OP_FLAG_SETALL = 1280;

	public const Int32 FF9SOUND_OP_FLAG_INTPLALL = 1536;

	public const Int32 FF9SOUND_OP_FLAG_LOAD = 1792;

	public const Int32 FF9SOUND_OP_FLAG_SUSPEND = 2048;

	public const Int32 FF9SOUND_OP_FLAG_RESTORE = 2304;

	public const Int32 FF9SOUND_OP_FLAG_SETGLOBAL = 2560;

	public const Int32 FF9SOUND_OP_FLAG_FADEALL = 2816;

	public const Int32 FF9SOUND_OP_FLAG_SYNC = 3072;

	public const Int32 FF9SOUND_MEM_HEAP = 0;

	public const Int32 FF9SOUND_MEM_RESIDENT = 1;

	public const Int32 FF9SOUND_MEM_MASK = 1;

	public const Int32 FF9SOUND_MEM_SHIFT = 7;

	public const Int32 FF9SOUND_MEM_FLAG_HEAP = 0;

	public const Int32 FF9SOUND_MEM_FLAG_RESIDENT = 128;

	public const Int32 FF9SOUND_SLOT_ZERO = 0;

	public const Int32 FF9SOUND_SLOT_ONE = 1;

	public const Int32 FF9SOUND_SLOT_MASK = 1;

	public const Int32 FF9SOUND_SLOT_SHIFT = 6;

	public const Int32 FF9SOUND_SLOT_FLAG_ZERO = 0;

	public const Int32 FF9SOUND_SLOT_FLAG_ONE = 64;

	public const Int32 FF9SOUND_PARM_OBJ = 0;

	public const Int32 FF9SOUND_PARM_VOL = 1;

	public const Int32 FF9SOUND_PARM_TEMPO = 2;

	public const Int32 FF9SOUND_PARM_PITCH = 3;

	public const Int32 FF9SOUND_PARM_TEMPOPITCH = 4;

	public const Int32 FF9SOUND_PARM_POS = 5;

	public const Int32 FF9SOUND_PARM_JUMPPOINT = 6;

	public const Int32 FF9SOUND_PARM_PHRASE = 7;

	public const Int32 FF9SOUND_PARM_NULL = 8;

	public const Int32 FF9SOUND_PARM_OBJCURRENT = 9;

	public const Int32 FF9SOUND_PARM_VOLFMV = 10;

	public const Int32 FF9SOUND_PARM_POSFMV = 11;

	public const Int32 FF9SOUND_PARM_REVERB = 12;

	public const Int32 FF9SOUND_PARM_MASK = 63;

	public const Int32 FF9SOUND_PARM_SHIFT = 0;

	public const Int32 FF9SOUND_PARM_FLAG_OBJ = 0;

	public const Int32 FF9SOUND_PARM_FLAG_VOL = 1;

	public const Int32 FF9SOUND_PARM_FLAG_TEMPO = 2;

	public const Int32 FF9SOUND_PARM_FLAG_PITCH = 3;

	public const Int32 FF9SOUND_PARM_FLAG_TEMPOPITCH = 4;

	public const Int32 FF9SOUND_PARM_FLAG_POS = 5;

	public const Int32 FF9SOUND_PARM_FLAG_JUMPPOINT = 6;

	public const Int32 FF9SOUND_PARM_FLAG_PHRASE = 7;

	public const Int32 FF9SOUND_PARM_FLAG_NULL = 8;

	public const Int32 FF9SOUND_PARM_FLAG_OBJCURRENT = 9;

	public const Int32 FF9SOUND_PARM_FLAG_VOLFMV = 10;

	public const Int32 FF9SOUND_PARM_FLAG_POSFMV = 11;

	public const Int32 FF9SOUND_PARM_FLAG_REVERB = 12;

	public const Int32 FF9SOUND_SONG_LOAD = 1792;

	public const Int32 FF9SOUND_SONG_PLAY = 0;

	public const Int32 FF9SOUND_SONG_SUSPEND = 2048;

	public const Int32 FF9SOUND_SONG_RESTORE = 2304;

	public const Int32 FF9SOUND_SONG_STOP = 256;

	public const Int32 FF9SOUND_SONG_STOPCURRENT = 265;

	public const Int32 FF9SOUND_SONG_VOL = 16897;

	public const Int32 FF9SOUND_SONG_VOL_INTPL = 33537;

	public const Int32 FF9SOUND_SONG_VOL_FADE = 50177;

	public const Int32 FF9SOUND_SONG_VOL_INTPLALL = 34305;

	public const Int32 FF9SOUND_SONG_VOL_FADEALL = 51969;

	public const Int32 FF9SOUND_SONG_TEMPO = 16898;

	public const Int32 FF9SOUND_SONG_TEMPO_INTPL = 33538;

	public const Int32 FF9SOUND_SONG_TEMPO_FADE = 50178;

	public const Int32 FF9SOUND_SONG_PITCH = 16899;

	public const Int32 FF9SOUND_SONG_PITCH_INTPL = 33539;

	public const Int32 FF9SOUND_SONG_PITCH_FADE = 50179;

	public const Int32 FF9SOUND_SONG_TEMPOPITCH = 16900;

	public const Int32 FF9SOUND_SONG_TEMPOPITCH_INTPL = 33540;

	public const Int32 FF9SOUND_SONG_TEMPOPITCH_FADE = 1028;

	public const Int32 FF9SOUND_SONG_JUMPPOINT = 2566;

	public const Int32 FF9SOUND_SONG_SKIPPHRASE = 16903;

	public const Int32 FF9SOUND_SONG_NULL = 520;

	public const Int32 FF9SOUND_SNDEFFECT_PLAY = 53248;

	public const Int32 FF9SOUND_SNDEFFECT_STOP = 20736;

	public const Int32 FF9SOUND_SNDEFFECT_VOL = 37377;

	public const Int32 FF9SOUND_SNDEFFECT_VOL_INTPL = 54017;

	public const Int32 FF9SOUND_SNDEFFECT_VOL_ALL = 21761;

	public const Int32 FF9SOUND_SNDEFFECT_VOL_INTPLALL = 38401;

	public const Int32 FF9SOUND_SNDEFFECT_POS = 37381;

	public const Int32 FF9SOUND_SNDEFFECT_POS_INTPL = 54021;

	public const Int32 FF9SOUND_SNDEFFECT_POS_ALL = 21765;

	public const Int32 FF9SOUND_SNDEFFECT_POS_INTPLALL = 38405;

	public const Int32 FF9SOUND_SNDEFFECT_PITCH = 37379;

	public const Int32 FF9SOUND_SNDEFFECT_PITCH_INTPL = 54019;

	public const Int32 FF9SOUND_SNDEFFECT_PITCH_ALL = 21763;

	public const Int32 FF9SOUND_SNDEFFECT_PITCH_INTPLALL = 38403;

	public const Int32 FF9SOUND_SNDEFFECT_NULL = 4616;

	public const Int32 FF9SOUND_SNDEFFECTRES_PLAY = 53376;

	public const Int32 FF9SOUND_SNDEFFECTRES_STOP = 20864;

	public const Int32 FF9SOUND_SNDEFFECTRES_STOPCURRENT = 4489;

	public const Int32 FF9SOUND_SNDEFFECTRES_SUSPEND = 6272;

	public const Int32 FF9SOUND_SNDEFFECTRES_RESTORE = 6528;

	public const Int32 FF9SOUND_SNDEFFECTRES_VOL = 37505;

	public const Int32 FF9SOUND_SNDEFFECTRES_VOL_INTPL = 54145;

	public const Int32 FF9SOUND_SNDEFFECTRES_VOL_ALL = 21889;

	public const Int32 FF9SOUND_SNDEFFECTRES_VOL_INTPLALL = 38529;

	public const Int32 FF9SOUND_SNDEFFECTRES_POS = 37509;

	public const Int32 FF9SOUND_SNDEFFECTRES_POS_INTPL = 54149;

	public const Int32 FF9SOUND_SNDEFFECTRES_POS_ALL = 21893;

	public const Int32 FF9SOUND_SNDEFFECTRES_POS_INTPLALL = 38533;

	public const Int32 FF9SOUND_SNDEFFECTRES_PITCH = 37507;

	public const Int32 FF9SOUND_SNDEFFECTRES_PITCH_INTPL = 54147;

	public const Int32 FF9SOUND_SNDEFFECTRES_PITCH_ALL = 21891;

	public const Int32 FF9SOUND_SNDEFFECTRES_PITCH_INTPLALL = 38531;

	public const Int32 FF9SOUND_STREAM_PLAY = 40960;

	public const Int32 FF9SOUND_STREAM_STOP = 8448;

	public const Int32 FF9SOUND_STREAM_VOL = 25089;

	public const Int32 FF9SOUND_STREAM_POS = 25093;

	public const Int32 FF9SOUND_STREAM_REVERB = 25100;

	public const Int32 FF9SOUND_STREAM_NULL = 8712;

	public const Int32 FF9SOUND_STREAMFMV_VOL = 25098;

	public const Int32 FF9SOUND_INSTR_LOAD = 30464;

	public const Int32 FF9SOUND_SYNC = 3072;

	public const Int32 FALSE = 0;

	public const Int32 TRUE = 1;

	public const Int32 NONE = -1;

	public const Int32 FF9_ATTR_GLOBAL_SOUNDRESET = 1048576;

	public const Int32 FF9_ATTR_GLOBAL_SONGSTOP = 2097152;

	public const Int32 FF9_ATTR_GLOBAL_SONGFADE = 4194304;

	public const Int32 FF9_ATTR_GLOBAL_SNDEFFECT0SET = 16777216;

	public const Int32 FF9_ATTR_GLOBAL_SNDEFFECT0PLAY = 33554432;

	public const Int32 FF9_ATTR_GLOBAL_SNDEFFECT1SET = 67108864;

	public const Int32 FF9_ATTR_GLOBAL_SNDEFFECT1PLAY = 134217728;

	public const UInt32 FF9_ATTR_GLOBAL_SONG0SET = 268435456u;

	public const UInt32 FF9_ATTR_GLOBAL_SONG0PLAY = 536870912u;

	public const UInt32 FF9_ATTR_GLOBAL_SONG1SET = 1073741824u;

	public const UInt32 FF9_ATTR_GLOBAL_SONG1PLAY = 2147483648u;

	public const Int32 ENDINT_DEFAULT_SONGID = 156;

	public const Int32 ENDGAME_PARM_SONG_FADEINFRAME = 90;

	public const Int32 FF9FLDSND_SNDEFFECT_ATTR_RESIDENT_COUNT = 2;

	public const Int32 FF9FLDSND_SNDEFFECT_ATTR_RESIDENT_BASE = 1048576;

	public const Int32 FF9FLDSND_SNDEFFECT_ATTR_RESIDENT_MASK = 3145728;

	public const Int32 FF9FLDSND_SNDEFFECT_ATTR_FOOT_COUNT = 4;

	public const Int32 FF9FLDSND_SNDEFFECT_ATTR_FOOT_BASE = 65536;

	public const Int32 FF9FLDSND_SNDEFFECT_ATTR_FOOT_MASK = 983040;

	public const Int32 FF9FLDSND_INSTR_DMSATTR_EVENT = 1;

	public const Int32 FF9FLDSND_INSTR_DMSATTR_FOOT = 4;

	public const Int32 FLDINT_SONG_FADEOUTTICK = 15;

	private const Int32 FF9_SCREEN_WIDTH = 320;

	private const Int32 FF9_SCREEN_HEIGHT = 224;

	public const Int32 FF9WORLDSOUND_DEFAULTSNDEFFECT_COUNT = 31;

	public static FF9Snd.SoundDispatchDelegate sndFuncPtr;

	public static Boolean HasJustChangedBetweenWorldAndField;

	private static Int16[,] ff9battleSoundWeaponSndEffect01 = new Int16[,] // Indexed by "WeaponItem"
	{
		{ 903, 44, 37 },    // Hammer
		{ 899, 21, 476 },	// Dagger
		{ 899, 21, 475 },	// MageMasher
		{ 899, 21, 501 },	// MythrilDagger
		{ 899, 21, 477 },	// Gladius
		{ 899, 21, 441 },	// ZorlinShape
		{ 899, 21, 527 },	// Orichalcon
		{ 900, 63, 522 },	// ButterflySword
		{ 900, 63, 521 },	// TheOgre
		{ 900, 63, 517 },	// Exploda
		{ 900, 63, 516 },	// RuneTooth
		{ 900, 63, 515 },	// AngelBless
		{ 900, 63, 514 },	// Sargatanas
		{ 900, 63, 520 },	// Masamune
		{ 900, 63, 519 },	// TheTower
		{ 900, 63, 518 },	// UltimaWeapon
		{ 902, 0, 366 },	// Broadsword
		{ 902, 0, 385 },	// IronSword
		{ 902, 0, 384 },	// MythrilSword
		{ 902, 0, 389 },	// BloodSword
		{ 902, 0, 388 },	// IceBrand
		{ 902, 0, 387 },	// CoralSword
		{ 902, 0, 386 },	// DiamondSword
		{ 902, 0, 392 },	// FlameSabre
		{ 902, 0, 391 },	// RuneBlade
		{ 902, 0, 629 },	// Defender
		{ 902, 0, 390 },	// SaveTheQueen
		{ 902, 0, 367 },	// UltimaSword
		{ 902, 0, 595 },	// Excalibur
		{ 902, 0, 383 },	// Ragnarok
		{ 902, 0, 594 },	// Excalibur2
		{ 901, 83, 465 },	// Javelin
		{ 901, 83, 496 },	// MythrilSpear
		{ 901, 83, 468 },	// Partisan
		{ 901, 83, 497 },	// IceLance
		{ 901, 83, 467 },	// Trident
		{ 901, 83, 466 },	// HeavyLance
		{ 901, 83, 470 },	// Obelisk
		{ 901, 83, 469 },	// HolyLance
		{ 901, 83, 605 },	// KainLance
		{ 901, 83, 413 },	// DragonHair
		{ 905, 897, 471 },	// CatClaws
		{ 905, 897, 498 },	// PoisonKnuckles
		{ 905, 897, 500 },	// MythrilClaws
		{ 905, 897, 472 },	// ScissorFangs
		{ 905, 897, 440 },	// DragonClaws
		{ 905, 897, 499 },	// TigerFangs
		{ 905, 897, 474 },	// Avenger
		{ 905, 897, 473 },	// KaiserKnuckles
		{ 905, 897, 456 },	// DuelClaws
		{ 905, 897, 455 },	// RuneClaws
		{ 906, 898, 454 },	// AirRacket
		{ 906, 898, 453 },	// MultinaRacket
		{ 906, 898, 460 },	// MagicRacket
		{ 906, 898, 459 },	// MythrilRacket
		{ 906, 898, 458 },	// PriestRacket
		{ 906, 898, 457 },	// TigerRacket
		{ 903, 44, 461 },	// Rod
		{ 903, 44, 491 },	// MythrilRod
		{ 903, 44, 493 },	// StardustRod
		{ 903, 44, 492 },	// HealingRod
		{ 903, 44, 463 },	// AsuraRod
		{ 903, 44, 462 },	// WizardRod
		{ 903, 44, 464 },	// WhaleWhisker
		{ 904, 22, 44 },	// GolemFlute
		{ 904, 22, 494 },	// LamiaFlute
		{ 904, 22, 43 },	// FairyFlute
		{ 904, 22, 264 },	// Hamelin
		{ 904, 22, 495 },	// SirenFlute
		{ 904, 22, 322 },	// AngelFlute
		{ 904, 22, 478 },	// MageStaff
		{ 904, 22, 503 },	// FlameStaff
		{ 904, 22, 502 },	// IceStaff
		{ 904, 22, 504 },	// LightningStaff
		{ 904, 22, 481 },	// OakStaff
		{ 904, 22, 480 },	// CypressPile
		{ 904, 22, 479 },	// OctagonRod
		{ 904, 22, 483 },	// HighMageStaff
		{ 904, 22, 482 },	// MaceofZeus
		{ 901, 83, 484 },	// Fork
		{ 901, 83, 507 },	// NeedleFork
		{ 901, 83, 506 },	// MythrilFork
		{ 901, 83, 505 },	// SilverFork
		{ 901, 83, 486 },	// BistroFork
		{ 901, 83, 485 }	// GastroFork
	};

	public static Dictionary<CharacterSerialNumber, Int32[]> ff9battleSoundWeaponSndEffect02 = new Dictionary<CharacterSerialNumber, Int32[]>
	{
		{ CharacterSerialNumber.ZIDANE_DAGGER,		new Int32[]{ 899, 21 } },
		{ CharacterSerialNumber.ZIDANE_SWORD,		new Int32[]{ 900, 83 } },
		{ CharacterSerialNumber.VIVI,				new Int32[]{ 904, 22 } },
		{ CharacterSerialNumber.GARNET_LH_ROD,		new Int32[]{ 904, 22 } },
		{ CharacterSerialNumber.GARNET_LH_KNIFE,	new Int32[]{ 906, 898 } },
		{ CharacterSerialNumber.GARNET_SH_ROD,		new Int32[]{ 904, 22 } },
		{ CharacterSerialNumber.GARNET_SH_KNIFE,	new Int32[]{ 906, 898 } },
		{ CharacterSerialNumber.STEINER_OUTDOOR,	new Int32[]{ 902, 70 } },
		{ CharacterSerialNumber.STEINER_INDOOR,		new Int32[]{ 902, 70 } },
		{ CharacterSerialNumber.KUINA,				new Int32[]{ 901, 20 } },
		{ CharacterSerialNumber.EIKO_FLUTE,			new Int32[]{ 904, 22 } },
		{ CharacterSerialNumber.EIKO_KNIFE,			new Int32[]{ 906, 898 } },
		{ CharacterSerialNumber.FREIJA,				new Int32[]{ 901, 20 } },
		{ CharacterSerialNumber.SALAMANDER,			new Int32[]{ 905, 897 } },
		{ CharacterSerialNumber.CINNA,				new Int32[]{ 900, 22 } },
		{ CharacterSerialNumber.MARCUS,				new Int32[]{ 902, 63 } },
		{ CharacterSerialNumber.BLANK,				new Int32[]{ 902, 63 } },
		{ CharacterSerialNumber.BLANK_ARMOR,		new Int32[]{ 902, 63 } },
		{ CharacterSerialNumber.BEATRIX,			new Int32[]{ 900, 70 } }
	};

	public static Int32 FLDINT_CHAR_SOUNDCOUNT = 20;

	public static Int32 BGI_ATTR_STEP2 = 16;

	public static HashSet<Int32> PlayedFieldSoundEffect = new HashSet<Int32>();

	public static Int32 BGMFieldSongCounter = 0;

	public static Int32 LatestWorldPlayedSong = -1;

	private enum FF9SOUNDSTATUS
	{
		IDLE,
		INIT,
		INSTR_LOADREQUEST,
		INSTR_LOADSERVICE,
		SEQ_LOADREQUEST,
		SEQ_LOADSERVICE,
		FADEWAIT,
		COUNT
	}

	private enum DMS_DATATYPE
	{
		GENERIC,
		MODULE,
		GEOMETRY,
		ANIMATION,
		IMAGE,
		SCRIPT,
		MESSAGE,
		SONG,
		SOUNDEFFECT,
		INSTRUMENT,
		FIELDBG,
		FIELDTERRAIN,
		BATTLEBG,
		FONTINFO,
		WMPACKTIM,
		WMSCRIPT,
		BATTLESCENETMP,
		BATTLESCENE,
		VRAMINFO,
		DEBUG,
		CDLINK,
		LIGHTINFO,
		SPS,
		SPSTEXTURE,
		SOUNDSTREAM,
		TEXTUREANIMATION,
		BATTLEBGINFO,
		PACKDATA,
		VRAMUSAGE,
		CLUTINFO,
		CONTROLLER,
		MAPCONFIGURATION,
		MOVIE,
		COUNT
	}

	private class FF9
	{
		public static Int32 prevSongID;

		public static Int32 prevSongMapID;

		public static Int16 fldMapNo;

		public static Int32 prevSongInstrID;

		public static Int32[] songVol = new Int32[2];

		public static Char prevSongVol;

		public static Int64 songSector;

		public static Int64 prevSongSector;

		public static Int16 songSize;

		public static Int16 prevSongSize;

		public static Int32 sndStatus;

		public static Int32[] songID = new Int32[2];

		public static Int32[] songInstrID = new Int32[2];

		public static Int32[] songSeqNo = new Int32[2];

		public static UInt64 attr;
	}

	public delegate Int32 SoundDispatchDelegate(Int32 ParmType, Int32 ObjNo, Int32 Arg1, Int32 Arg2, Int32 Arg3);
}
