using Assets.Scripts.Common;
using System;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable UnusedMember.Global

public class BattleSwirl : MonoBehaviour
{
    private const Single FRAME_LENGTH = 1f / 30f;

    private Single _time;
    private Single _cumulativeTime;
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
        _cumulativeTime += Time.deltaTime;
        if (_time >= 1.3f && !_hasPlayEncounterSound)
        {
            RequestPlayBattleEncounterSong();
            _hasPlayEncounterSound = true;
        }

        while (_cumulativeTime >= FRAME_LENGTH)
        {
            _cumulativeTime -= FRAME_LENGTH;
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
            RequestPlayBattleEncounterSongForField();
        else if (_eventEngineGmode == 3)
            RequestPlayBattleEncounterSongForWorld();
    }

    private void RequestPlayBattleEncounterSongForField()
    {
        FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle = true;
        Int32 songid = FF9SndMetaData.GetMusicForBattle(FF9SndMetaData.BtlBgmMapperForFieldMap, FF9StateSystem.Common.FF9.fldMapNo, FF9StateSystem.Field.FF9Field.loc.map.nextMapNo);
        Int32 currentMusicId = FF9Snd.GetCurrentMusicId();
        if (songid == -1 || songid == currentMusicId)
            return;

        btlsnd.ff9btlsnd_song_play(songid);
        FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle = false;
    }

    private void RequestPlayBattleEncounterSongForWorld()
    {
        Int32 songid = FF9SndMetaData.GetMusicForBattle(FF9SndMetaData.BtlBgmMapperForWorldMap, FF9StateSystem.Common.FF9.wldMapNo, FF9StateSystem.World.FF9World.map.nextMapNo);
        Int32 currentMusicId = FF9Snd.GetCurrentMusicId();
        if (songid == -1 || songid == currentMusicId)
            return;

        SoundLib.GetAllSoundDispatchPlayer().FF9SOUND_SONG_SUSPEND(currentMusicId, true);
        btlsnd.ff9btlsnd_song_play(songid);
    }
}
