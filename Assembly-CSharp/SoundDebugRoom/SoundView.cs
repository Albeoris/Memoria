using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using Memoria;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundDebugRoom
{
    public class SoundView : MonoBehaviour
    {
        private void Start()
        {
            SoundLib.SuspendSoundSystem();
            this.soundViewController = new SoundViewController(this);
        }

        private void Exit()
        {
            SoundLib.ResumeSoundSystem();
            SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
        }

        private void OnGUI()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            DebugGuiSkin.ApplySkin();
            GUILayout.BeginArea(fullscreenRect);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.soundPanelScrollPosition = GUILayout.BeginScrollView(this.soundPanelScrollPosition, new GUILayoutOption[]
            {
                GUILayout.Width(fullscreenRect.width),
                GUILayout.Height(fullscreenRect.height * 0.45f)
            });
            this.BuildSoundName();
            this.BuildPlayer();
            this.BuildVolumeSlider();
            this.BuildSeeker();
            this.BuildAdjustment();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button("Sound Effect", new GUILayoutOption[0]))
            {
                this.soundViewController.SetActiveSoundType(SoundProfileType.SoundEffect);
            }
            if (GUILayout.Button("BGM", new GUILayoutOption[0]))
            {
                this.soundViewController.SetActiveSoundType(SoundProfileType.Music);
                this.soundViewController.SetMusicPanning(this.PanningPosition);
                this.soundViewController.SetMusicPitch(this.PitchPosition);
                this.soundViewController.SetMusicVolume(this.SoundVolume);
            }
            if (GUILayout.Button("Song", new GUILayoutOption[0]))
            {
                this.soundViewController.SetActiveSoundType(SoundProfileType.Song);
            }
            if (GUILayout.Button("Sfx Sound", new GUILayoutOption[0]))
            {
                this.soundViewController.SetActiveSoundType(SoundProfileType.Sfx);
            }
            if (GUILayout.Button("Movie Audio", new GUILayoutOption[0]))
            {
                this.soundViewController.SetActiveSoundType(SoundProfileType.MovieAudio);
            }
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("<", new GUILayoutOption[0]))
            {
                this.soundViewController.PreviousPlayList();
            }
            GUILayout.Label(this.soundViewController.PlaylistInfo, new GUILayoutOption[0]);
            if (GUILayout.Button(">", new GUILayoutOption[0]))
            {
                this.soundViewController.NextPlayList();
            }
            GUILayout.EndHorizontal();
            GUILayout.Label(this.soundViewController.PlaylistDetail, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
            if (this.soundViewController.GetActiveSoundType() == SoundProfileType.Sfx)
            {
                this.BuildSfxSoundSelector();
            }
            else
            {
                this.BuildSoundSelector();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void BuildSoundName()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Back", new GUILayoutOption[0]))
            {
                this.Exit();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (this.soundViewController != null && this.soundViewController.GetActiveSound() != null)
            {
                GUILayout.Label(this.soundViewController.GetActiveSound().Name, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            if (this.isEnableSound)
            {
                if (GUILayout.Button("Suspend SEAD", new GUILayoutOption[0]))
                {
                    if (ISdLibAPIProxy.Instance.SdSoundSystem_Suspend() == 0)
                    {
                        this.isEnableSound = false;
                    }
                    else
                    {
                        SoundLib.LogWarning("SdSoundSystem_Suspend failure");
                    }
                }
            }
            else if (GUILayout.Button("Resume SEAD", new GUILayoutOption[0]))
            {
                if (ISdLibAPIProxy.Instance.SdSoundSystem_Resume() == 0)
                {
                    this.isEnableSound = true;
                }
                else
                {
                    SoundLib.LogWarning("SdSoundSystem_Resume failure");
                }
            }
            GUILayout.EndHorizontal();
        }

        private void BuildPlayer()
        {
            Boolean isPlay = this.soundViewController.IsPlay;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Previous", new GUILayoutOption[0]))
            {
                this.soundViewController.PreviousSound();
            }
            this.soundViewController.IsPlay = GUILayout.Toggle(this.soundViewController.IsPlay, this.playPauseToggleText, new GUILayoutOption[0]);
            if (GUILayout.Button("Stop", new GUILayoutOption[0]))
            {
                this.soundViewController.StopActiveSound();
                isPlay = this.soundViewController.IsPlay;
            }
            if (GUILayout.Button("Next", new GUILayoutOption[0]))
            {
                this.soundViewController.NextSound();
            }
            if (GUILayout.Button("Reload", new GUILayoutOption[0]))
            {
                // This goes into an infinite loop:
                //this.soundViewController.ReloadSoundMetaData();
            }
            GUILayout.EndHorizontal();
            if (this.soundViewController.IsPlay)
            {
                this.playPauseToggleText = "Pause";
                if (this.soundViewController.IsPlay != isPlay)
                {
                    this.soundViewController.PlayActiveSound();
                }
            }
            else
            {
                this.playPauseToggleText = "Play";
                if (this.soundViewController.IsPlay != isPlay)
                {
                    this.soundViewController.PauseActiveSound();
                }
            }
        }

        private void BuildVolumeSlider()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Volume", new GUILayoutOption[0]);
            Single soundVolume = this.SoundVolume;
            this.SoundVolume = GUILayout.HorizontalSlider(this.SoundVolume, 0f, 1f, new GUILayoutOption[0]);
            if (soundVolume != this.SoundVolume)
            {
                this.soundViewController.SetMusicVolume(this.SoundVolume);
            }
            GUILayout.EndHorizontal();
        }

        private void BuildSeeker()
        {
        }

        private void BuildAdjustment()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Panning", new GUILayoutOption[0]);
            Single panningPosition = this.PanningPosition;
            this.PanningPosition = GUILayout.HorizontalSlider(this.PanningPosition, -1f, 1f, new GUILayoutOption[0]);
            if (panningPosition != this.PanningPosition)
            {
                this.soundViewController.SetMusicPanning(this.PanningPosition);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Pitch", new GUILayoutOption[0]);
            Single pitchPosition = this.PitchPosition;
            this.PitchPosition = GUILayout.HorizontalSlider(this.PitchPosition, 0f, 2f, new GUILayoutOption[0]);
            if (pitchPosition != this.PitchPosition)
            {
                this.soundViewController.SetMusicPitch(this.PitchPosition);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Reset", new GUILayoutOption[0]))
            {
                this.SoundVolume = 1f;
                this.PitchPosition = 1f;
                this.PanningPosition = 0f;
                this.soundViewController.SetMusicVolume(this.SoundVolume);
                this.soundViewController.SetMusicPitch(this.PitchPosition);
                this.soundViewController.SetMusicPanning(this.PanningPosition);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (this.isSfxEnabled)
            {
                if (GUILayout.Button("Disable SFX", new GUILayoutOption[0]))
                {
                    SoundLib.DisableSoundEffect();
                    this.isSfxEnabled = false;
                }
            }
            else if (GUILayout.Button("Enable SFX", new GUILayoutOption[0]))
            {
                SoundLib.EnableSoundEffect();
                this.isSfxEnabled = true;
            }
            if (this.isMusicEnabled)
            {
                if (GUILayout.Button("Disable Music", new GUILayoutOption[0]))
                {
                    SoundLib.DisableMusic();
                    this.isMusicEnabled = false;
                }
            }
            else if (GUILayout.Button("Enable Music", new GUILayoutOption[0]))
            {
                SoundLib.EnableMusic();
                this.isMusicEnabled = true;
            }
            if (GUILayout.Button("Next Loop Region", new GUILayoutOption[0]))
            {
                SoundLib.SetNextLoopRegion(71);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void BuildSfxSoundSelector()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.soundSelectorScrollPosition = GUILayout.BeginScrollView(this.soundSelectorScrollPosition, new GUILayoutOption[]
            {
                GUILayout.Width(fullscreenRect.width * 2f / 3f),
                GUILayout.Height(fullscreenRect.height * 0.45f)
            });
            if (this.sfxSoundUIState == 0)
            {
                foreach (Int32 num in this.soundViewController.AllSfxGroupSongIndex)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Label("EFX ID: " + num, new GUILayoutOption[0]);
                    if (GUILayout.Button("Load", new GUILayoutOption[0]))
                    {
                        this.CurrentSpecialEffectID = num;
                        this.sfxSoundUIState = 1;
                        this.soundViewController.LoadSfxSoundGroup(num);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else if (this.sfxSoundUIState == 1)
            {
                List<String> sfxSoundPlaylist = this.soundViewController.GetSfxSoundPlaylist(this.CurrentSpecialEffectID);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                if (GUILayout.Button("Back", new GUILayoutOption[0]))
                {
                    this.sfxSoundUIState = 0;
                }
                for (Int32 i = 0; i < sfxSoundPlaylist.Count; i++)
                {
                    if (i == SoundLib.GetResidentSfxSoundCount())
                    {
                        GUILayout.Label("---- ---- ---- ----", new GUILayoutOption[0]);
                    }
                    String text = sfxSoundPlaylist[i];
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Label(text, new GUILayoutOption[0]);
                    if (GUILayout.Button("Play", new GUILayoutOption[0]))
                    {
                        this.soundViewController.SelectSound(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            else
            {
                SoundLib.LogError("sfxSoundUIState is invalid: " + this.sfxSoundUIState);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void BuildSoundSelector()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.soundSelectorScrollPosition = GUILayout.BeginScrollView(this.soundSelectorScrollPosition, new GUILayoutOption[]
            {
                GUILayout.Width(fullscreenRect.width * 2f / 3f),
                GUILayout.Height(fullscreenRect.height * 0.45f)
            });
            List<SoundProfile> playlist = this.soundViewController.GetPlaylist();
            if (playlist != null)
            {
                foreach (SoundProfile soundProfile in playlist)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Label(soundProfile.Name, new GUILayoutOption[0]);
                    if (GUILayout.Button("Play", new GUILayoutOption[0]))
                    {
                        this.soundViewController.SelectSound(soundProfile);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private const Int32 SFX_SOUND_UI_STATE_LOADER = 0;

        private const Int32 SFX_SOUND_UI_STATE_PLAYER = 1;

        private String playPauseToggleText = "Play";

        public Single SoundVolume = 1f;

        public Single SeekerPosition;

        public Single PitchPosition = 1f;

        public Single PanningPosition;

        private Vector2 soundPanelScrollPosition = new Vector2(0f, 0f);

        private Vector2 soundSelectorScrollPosition = new Vector2(0f, 0f);

        private SoundViewController soundViewController;

        private Boolean isEnableSound = true;

        private Boolean isSfxEnabled = true;

        private Boolean isMusicEnabled = true;

        private Int32 sfxSoundUIState;

        private Int32 CurrentSpecialEffectID;
    }
}
