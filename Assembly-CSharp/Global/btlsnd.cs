using System;

public class btlsnd
{
	public static void ff9btlsnd_song_load(Int32 _songid)
	{
		btlsnd.FF9BattleSoundArg0(1792, _songid);
	}

	public static void ff9btlsnd_song_play(Int32 _songid)
	{
		btlsnd.FF9BattleSoundArg0(0, _songid);
	}

	public static void ff9btlsnd_song_stop(Int32 _songid)
	{
		btlsnd.FF9BattleSoundArg0(256, _songid);
	}

	public static void ff9btlsnd_song_vol(Int32 _songid, Int32 _vol)
	{
		btlsnd.FF9BattleSoundArg1(16897, _songid, _vol);
	}

	public static void ff9btlsnd_song_vol_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(33537, _songid, _ticks, _to);
	}

	public static void ff9btlsnd_song_vol_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static void ff9btlsnd_song_vol_intplall(Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(34305, 0, _ticks, _to);
	}

	public static void ff9btlsnd_song_tempo(Int32 _songid, Int32 _tempo)
	{
		btlsnd.FF9BattleSoundArg1(16898, _songid, _tempo);
	}

	public static void ff9btlsnd_song_tempo_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(33538, _songid, _ticks, _to);
	}

	public static void ff9btlsnd_song_tempo_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static void ff9btlsnd_song_pitch(Int32 _songid, Int32 _pitch)
	{
		btlsnd.FF9BattleSoundArg1(16899, _songid, _pitch);
	}

	public static void ff9btlsnd_song_pitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(33539, _songid, _ticks, _to);
	}

	public static void ff9btlsnd_song_pitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(50177, _songid, _ticks, _from, _to);
	}

	public static void ff9btlsnd_song_tempopitch(Int32 _songid, Int32 _tempopitch)
	{
		btlsnd.FF9BattleSoundArg1(16900, _songid, _tempopitch);
	}

	public static void ff9btlsnd_song_tempopitch_intpl(Int32 _songid, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(33540, _songid, _ticks, _to);
	}

	public static void ff9btlsnd_song_tempopitch_fade(Int32 _songid, Int32 _ticks, Int32 _from, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(1028, _songid, _ticks, _from, _to);
	}

	public static void ff9btlsnd_sndeffect_play(Int32 _sndeffectid, Int32 _attr, Int32 _vol, Int32 _pos)
	{
		btlsnd.FF9BattleSoundArg3(53248, _sndeffectid, _attr, _pos, _vol);
	}

	public static void ff9btlsnd_sndeffect_stop(Int32 _sndeffectid, Int32 _attr)
	{
		btlsnd.FF9BattleSoundArg1(20736, _sndeffectid, _attr);
	}

	public static void ff9btlsnd_sndeffect_vol(Int32 _sndeffectid, Int32 _attr, Int32 _vol)
	{
		btlsnd.FF9BattleSoundArg2(37377, _sndeffectid, _attr, _vol);
	}

	public static void ff9btlsnd_sndeffect_vol_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(54017, _sndeffectid, _attr, _ticks, _to);
	}

	public static void ff9btlsnd_sndeffect_vol_all(Int32 _vol)
	{
		btlsnd.FF9BattleSoundArg1(21761, -1, _vol);
	}

	public static void ff9btlsnd_sndeffect_vol_intplall(Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(38401, -1, _ticks, _to);
	}

	public static void ff9btlsnd_sndeffect_pos(Int32 _sndeffectid, Int32 _attr, Int32 _pos)
	{
		btlsnd.FF9BattleSoundArg2(37381, _sndeffectid, _attr, _pos);
	}

	public static void ff9btlsnd_sndeffect_pos_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(54021, _sndeffectid, _attr, _ticks, _to);
	}

	public static void ff9btlsnd_sndeffect_pos_all(Int32 _pos)
	{
		btlsnd.FF9BattleSoundArg1(21765, -1, _pos);
	}

	public static void ff9btlsnd_sndeffect_pos_intplall(Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(38405, -1, _ticks, _to);
	}

	public static void ff9btlsnd_sndeffect_pitch(Int32 _sndeffectid, Int32 _attr, Int32 _pitch)
	{
		btlsnd.FF9BattleSoundArg2(37379, _sndeffectid, _attr, _pitch);
	}

	public static void ff9btlsnd_sndeffect_pitch_intpl(Int32 _sndeffectid, Int32 _attr, Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg3(54019, _sndeffectid, _attr, _ticks, _to);
	}

	public static void ff9btlsnd_sndeffect_pitch_all(Int32 _pitch)
	{
		btlsnd.FF9BattleSoundArg1(21763, -1, _pitch);
	}

	public static void ff9btlsnd_sndeffect_pitch_intplall(Int32 _ticks, Int32 _to)
	{
		btlsnd.FF9BattleSoundArg2(38403, -1, _ticks, _to);
	}

	public static void ff9btlsnd_instr_load(Int32 _instrid, Int32 _areano)
	{
		btlsnd.FF9BattleSoundArg1(30464, _instrid, _areano);
	}

	public static Int32 ff9btlsnd_sync()
	{
		return btlsnd.FF9BattleSoundArg0(3072, 0);
	}

	public static Int32 FF9BattleSoundArg0(Int32 _parmtype, Int32 _objno)
	{
		return FF9Snd.FF9BattleSoundDispatch(_parmtype, _objno, 0, 0, 0);
	}

	public static void FF9BattleSoundArg1(Int32 _parmtype, Int32 _objno, Int32 _arg1)
	{
		FF9Snd.FF9BattleSoundDispatch(_parmtype, _objno, _arg1, 0, 0);
	}

	public static void FF9BattleSoundArg2(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2)
	{
		FF9Snd.FF9BattleSoundDispatch(_parmtype, _objno, _arg1, _arg2, 0);
	}

	public static void FF9BattleSoundArg3(Int32 _parmtype, Int32 _objno, Int32 _arg1, Int32 _arg2, Int32 _arg3)
	{
		FF9Snd.FF9BattleSoundDispatch(_parmtype, _objno, _arg1, _arg2, _arg3);
	}

	public static Int32 ff9btlsnd_weapon_sfx(Int32 _partycharno, FF9BatteSoundWeaponSndEffectType _sfxtype)
	{
		return FF9Snd.FF9BattleSoundGetWeaponSndEffect(_partycharno, _sfxtype);
	}

	public static void ff9btlsnd_weapon_instr(Int32 _partycharno)
	{
		FF9Snd.FF9BattleSoundGetWeaponInstr(_partycharno);
	}

	public const Int32 FF9BATTLESOUND_WEAPON_COUNT = 85;

	public const Int32 FF9BATTLESOUND_WEAPONSNDEFFECT_COUNT = 2;

	public const Int32 FF9BATTLESOUND_DEFAULTSNDEFFECT_COUNT = 7;

	public const Int32 FF9BATTLESOUND_IDATTR_INSTR_MONSTER = 2;

	public const Int32 FF9BATTLESOUND_IDATTR_INSTR_WEAPON = 3;

	public static Int16[] ff9battleDefaultSndEffect = new Int16[]
	{
		121,
		120,
		1110,
		2752,
		2906,
		2907,
		2908
	};
}
