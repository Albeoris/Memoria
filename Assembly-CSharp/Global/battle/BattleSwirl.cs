using System;
using Assets.Scripts.Common;
using System.Collections.Generic;
using Memoria;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable UnusedMember.Global

public class BattleSwirl : MonoBehaviour
{
    private Single _time;
    private Boolean _hasPlayEncounterSound;
    private Boolean _isReplaceCalled;
    private Int32 _eventEngineGmode;
    private SFX_Rush _rush;

    private void Awake()
    {
        FF9StateSystem.Battle.isEncount = false;
        _hasPlayEncounterSound = false;
        _isReplaceCalled = false;
        _eventEngineGmode = PersistenSingleton<EventEngine>.Instance.gMode;
        FF9Snd.sndFuncPtr = FF9Snd.FF9BattleSoundDispatch;
        PlayBattleEncounterSoundEffect();
        _rush = new SFX_Rush();
    }

    private void OnDestroy()
    {
        _rush.ReleaseRenderTarget();
        _rush = null;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time >= 1.3f && !_hasPlayEncounterSound)
        {
            RequestPlayBattleEncounterSong();
            _hasPlayEncounterSound = true;
        }

        if (_rush.update() && !this._isReplaceCalled)
        {
            if (!_hasPlayEncounterSound)
            {
                _hasPlayEncounterSound = true;
                RequestPlayBattleEncounterSong();
            }

            if (!this._isReplaceCalled)
            {
                this._isReplaceCalled = true;
                SceneDirector.ReplacePending(SceneTransition.FadeOutToBlack_FadeIn, true);
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _rush.PostProcess(source, destination);
    }

    private void PlayBattleEncounterSoundEffect()
    {
        FF9Snd.ff9fldsnd_sndeffect_play(636, 0, SByte.MaxValue, 128);
        FF9Snd.ff9fldsnd_sndeffect_play(635, 0, SByte.MaxValue, 128);
        FF9Snd.ff9fldsnd_sndeffect_play(634, 0, SByte.MaxValue, 128);
    }


    private void RequestPlayBattleEncounterSong()
    {
        if (_eventEngineGmode == 1)
        {
            RequestPlayBattleEncounterSongForField();
        }
        else
        {
            if (_eventEngineGmode != 3)
                return;
            RequestPlayBattleEncounterSongForWorld();
        }
    }

    private void RequestPlayBattleEncounterSongForField()
    {
        AllSoundDispatchPlayer soundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
        FF9Snd.ff9fieldSoundSuspendAllResidentSndEffect();
        AllSoundDispatchPlayer.PlayingSfx[] residentSndEffectSlot1 = soundDispatchPlayer.GetResidentSndEffectSlot();
        Int32 num1 = residentSndEffectSlot1[0]?.SndEffectVol ?? 0;
        Int32 num2 = residentSndEffectSlot1[1]?.SndEffectVol ?? 0;
        soundDispatchPlayer.FF9SOUND_SNDEFFECTRES_VOL_INTPLALL(15, 0);
        AllSoundDispatchPlayer.PlayingSfx[] residentSndEffectSlot2 = soundDispatchPlayer.GetResidentSndEffectSlot();
        if (residentSndEffectSlot2[0] != null)
            residentSndEffectSlot2[0].SndEffectVol = num1;
        if (residentSndEffectSlot2[1] != null)
            residentSndEffectSlot2[1].SndEffectVol = num2;

        Int32 index = FF9StateSystem.Common.FF9.fldMapNo;
        FF9StateFieldMap ff9StateFieldMap = FF9StateSystem.Field.FF9Field.loc.map;
        Dictionary<Int32, Int32> dictionary = FF9SndMetaData.BtlBgmMapperForFieldMap[index];
        Int32 currentMusicId = FF9Snd.GetCurrentMusicId();
        FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle = true;
        if (dictionary.Count == 0 || !dictionary.ContainsKey(ff9StateFieldMap.nextMapNo))
            return;

        Int32 songid = dictionary[ff9StateFieldMap.nextMapNo];
        if (songid == currentMusicId)
            return;

        FF9Snd.ff9fldsnd_song_suspend(currentMusicId);
        btlsnd.ff9btlsnd_song_play(songid);
        FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle = false;
    }

    private void RequestPlayBattleEncounterSongForWorld()
    {
        Int32 index = FF9StateSystem.Common.FF9.wldMapNo;
        FF9StateWorldMap ff9StateWorldMap = FF9StateSystem.World.FF9World.map;
        Dictionary<Int32, Int32> dictionary = FF9SndMetaData.BtlBgmMapperForWorldMap[index];
        Int32 currentMusicId = FF9Snd.GetCurrentMusicId();
        if (dictionary.Count == 0 || !dictionary.ContainsKey(ff9StateWorldMap.nextMapNo))
            return;

        Int32 songid = dictionary[(Int32)ff9StateWorldMap.nextMapNo];
        if (songid == currentMusicId)
            return;

        SoundLib.GetAllSoundDispatchPlayer().FF9SOUND_SONG_SUSPEND(currentMusicId, true);
        btlsnd.ff9btlsnd_song_play(songid);
    }
}