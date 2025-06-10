using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using Global.Sound.SaXAudio;
using Memoria.Prime;
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
            SceneDirector.Replace(PersistenSingleton<SceneDirector>.Instance.LastScene, SceneTransition.FadeOutToBlack_FadeIn, false);
        }

        private void OnGUI()
        {
            try
            {
                Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();

                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.skin.font = font;
                GUI.skin.horizontalSlider.margin = new RectOffset(font.lineHeight / 2, font.lineHeight / 2, font.lineHeight - (Int32)GUI.skin.horizontalSlider.fixedHeight / 2, 0);

                GUILayout.BeginArea(fullscreenRect);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);

                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                this.BuildSoundName();
                this.BuildPlayer();
                this.BuildAdjustment(fullscreenRect.width / 4f);
                GUILayout.EndVertical();

                BuildAudioEffects();

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(fullscreenRect.width / 2f) });
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(fullscreenRect.width / 4f) });
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
                if (soundViewController.ModVoiceDictionary.Count > 0 && GUILayout.Button("Mod Voices", new GUILayoutOption[0]))
                {
                    this.soundViewController.SetActiveSoundType(SoundProfileType.Voice);
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
                GUILayout.BeginHorizontal("box", new GUILayoutOption[] { GUILayout.Width(fullscreenRect.width / 4f) });
                if (this.soundViewController.GetActiveSoundType() == SoundProfileType.Sfx)
                {
                    this.BuildSfxSoundSelector();
                }
                else if (this.soundViewController.GetActiveSoundType() == SoundProfileType.Voice)
                {
                    BuildVoiceSoundSelectot();
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
            catch (Exception e)
            {
                Log.Error(e);
            }
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
            if (GUILayout.Button(soundViewController.IsPlay ? "Pause" : "Play", new GUILayoutOption[0]))
            {
                if (this.soundViewController.IsPlay)
                    this.soundViewController.PauseActiveSound();
                else
                    this.soundViewController.PlayActiveSound();
            }
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
        }

        private void BuildAdjustment(Single width)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Label("Volume", new GUILayoutOption[0]);
            Single soundVolume = this.SoundVolume;
            this.SoundVolume = GUILayout.HorizontalSlider(this.SoundVolume, 0f, 1f, new GUILayoutOption[] { GUILayout.Width(width * 0.75f) });
            if (soundVolume != this.SoundVolume)
            {
                this.soundViewController.SetMusicVolume(this.SoundVolume);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Panning", new GUILayoutOption[0]);
            Single panningPosition = this.PanningPosition;
            this.PanningPosition = GUILayout.HorizontalSlider(this.PanningPosition, -1f, 1f, new GUILayoutOption[] { GUILayout.Width(width * 0.75f) });
            if (panningPosition != this.PanningPosition)
            {
                this.soundViewController.SetMusicPanning(this.PanningPosition);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Pitch", new GUILayoutOption[0]);
            Single pitchPosition = this.PitchPosition;
            this.PitchPosition = GUILayout.HorizontalSlider(this.PitchPosition, 0f, 2f, new GUILayoutOption[] { GUILayout.Width(width * 0.75f) });
            if (pitchPosition != this.PitchPosition)
            {
                this.soundViewController.SetMusicPitch(this.PitchPosition);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Label("", new GUILayoutOption[0]);
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

            GUILayout.EndHorizontal();
        }

        private void BuildAudioEffects()
        {
            if (soundViewController.IsSaXAudio)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
                fullscreenRect.width -= 40;
                Single columnWidth = fullscreenRect.width / 4f;
                Single sliderWidth = fullscreenRect.width / 7.5f;
                Single buttonWidth = fullscreenRect.width / 10f;

                BuildRevebParameters(columnWidth, sliderWidth, buttonWidth);
                BuildEqParameters(columnWidth, sliderWidth, buttonWidth);
                BuildEchoParameters(columnWidth, sliderWidth, buttonWidth);

                GUILayout.EndHorizontal();
            }
        }

        private void BuildRevebParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(width * 2) });

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Space(5);
            GUILayout.Label("Reverb", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(soundViewController.IsReverbEnabled ? "Disable" : "Enable", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
            {
                soundViewController.IsReverbEnabled = !soundViewController.IsReverbEnabled;
                if (soundViewController.IsReverbEnabled)
                    this.soundViewController.SetReverb(ref soundViewController.Reverb);
                else
                    this.soundViewController.RemoveReverb();
            }
            if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
            {
                soundViewController.Reverb = new SaXAudio.ReverbParameters();
                update = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
            Single result = BuildEffectParam("WetDryMix", soundViewController.Reverb.WetDryMix, 0, 100, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.WetDryMix != result)
            {
                soundViewController.Reverb.WetDryMix = result;
                update = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Label("Indexed parameters (no units)", new GUILayoutOption[0]);

            result = BuildEffectParam("PositionLeft", soundViewController.Reverb.PositionLeft, 0, 30, 1, sliderWidth);
            if (soundViewController.Reverb.PositionLeft != result)
            {
                soundViewController.Reverb.PositionLeft = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("PositionRight", soundViewController.Reverb.PositionRight, 0, 30, 1, sliderWidth);
            if (soundViewController.Reverb.PositionRight != result)
            {
                soundViewController.Reverb.PositionRight = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("PositionMatrixLeft", soundViewController.Reverb.PositionMatrixLeft, 0, 30, 1, sliderWidth);
            if (soundViewController.Reverb.PositionMatrixLeft != result)
            {
                soundViewController.Reverb.PositionMatrixLeft = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("PositionMatrixRight", soundViewController.Reverb.PositionMatrixRight, 0, 30, 1, sliderWidth);
            if (soundViewController.Reverb.PositionMatrixRight != result)
            {
                soundViewController.Reverb.PositionMatrixRight = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("EarlyDiffusion", soundViewController.Reverb.EarlyDiffusion, 0, 15, 1, sliderWidth);
            if (soundViewController.Reverb.EarlyDiffusion != result)
            {
                soundViewController.Reverb.EarlyDiffusion = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("LateDiffusion", soundViewController.Reverb.EarlyDiffusion, 0, 15, 1, sliderWidth);
            if (soundViewController.Reverb.LateDiffusion != result)
            {
                soundViewController.Reverb.LateDiffusion = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("LowEQGain", soundViewController.Reverb.LowEQGain, 0, 12, 1, sliderWidth);
            if (soundViewController.Reverb.LowEQGain != result)
            {
                soundViewController.Reverb.LowEQGain = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("LowEQCutoff", soundViewController.Reverb.LowEQCutoff, 0, 9, 1, sliderWidth);
            if (soundViewController.Reverb.LowEQCutoff != result)
            {
                soundViewController.Reverb.LowEQCutoff = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("HighEQGain", soundViewController.Reverb.HighEQGain, 0, 8, 1, sliderWidth);
            if (soundViewController.Reverb.HighEQGain != result)
            {
                soundViewController.Reverb.HighEQGain = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("HighEQCutoff", soundViewController.Reverb.HighEQCutoff, 0, 14, 1, sliderWidth);
            if (soundViewController.Reverb.HighEQCutoff != result)
            {
                soundViewController.Reverb.HighEQCutoff = (Byte)result;
                update = true;
            }

            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Label("Delay times (ms)", new GUILayoutOption[0]);

            result = BuildEffectParam("ReflectionsDelay", soundViewController.Reverb.ReflectionsDelay, 0, 300, 1, sliderWidth);
            if (soundViewController.Reverb.ReflectionsDelay != result)
            {
                soundViewController.Reverb.ReflectionsDelay = (UInt32)result;
                update = true;
            }

            result = BuildEffectParam("ReverbDelay", soundViewController.Reverb.ReverbDelay, 0, 85, 1, sliderWidth);
            if (soundViewController.Reverb.ReverbDelay != result)
            {
                soundViewController.Reverb.ReverbDelay = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("RearDelay", soundViewController.Reverb.RearDelay, 0, 5, 1, sliderWidth);
            if (soundViewController.Reverb.RearDelay != result)
            {
                soundViewController.Reverb.RearDelay = (Byte)result;
                update = true;
            }

            result = BuildEffectParam("SideDelay", soundViewController.Reverb.SideDelay, 0, 5, 1, sliderWidth);
            if (soundViewController.Reverb.SideDelay != result)
            {
                soundViewController.Reverb.SideDelay = (Byte)result;
                update = true;
            }

            GUILayout.Label("Direct parameters", new GUILayoutOption[0]);

            result = BuildEffectParam("RoomFilterFreq", soundViewController.Reverb.RoomFilterFreq, 20, 20000, 10, sliderWidth);
            if (soundViewController.Reverb.RoomFilterFreq != result)
            {
                soundViewController.Reverb.RoomFilterFreq = (UInt32)result;
                update = true;
            }

            result = BuildEffectParam("RoomFilterMain", soundViewController.Reverb.RoomFilterMain, -100f, 0, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.RoomFilterMain != result)
            {
                soundViewController.Reverb.RoomFilterMain = result;
                update = true;
            }

            result = BuildEffectParam("RoomFilterHF", soundViewController.Reverb.RoomFilterHF, -100f, 0, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.RoomFilterHF != result)
            {
                soundViewController.Reverb.RoomFilterHF = result;
                update = true;
            }

            result = BuildEffectParam("ReflectionsGain", soundViewController.Reverb.ReflectionsGain, -100f, 20, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.ReflectionsGain != result)
            {
                soundViewController.Reverb.ReflectionsGain = result;
                update = true;
            }

            result = BuildEffectParam("ReverbGain", soundViewController.Reverb.ReverbGain, -100f, 20, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.ReverbGain != result)
            {
                soundViewController.Reverb.ReverbGain = result;
                update = true;
            }

            result = BuildEffectParam("DecayTime", soundViewController.Reverb.DecayTime * 1000f, 100f, 5000f, 10f, sliderWidth, "F1") / 1000f;
            if (soundViewController.Reverb.DecayTime != result)
            {
                soundViewController.Reverb.DecayTime = result;
                update = true;
            }

            result = BuildEffectParam("Density", soundViewController.Reverb.Density, 0, 100, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.Density != result)
            {
                soundViewController.Reverb.Density = result;
                update = true;
            }
            result = BuildEffectParam("RoomSize", soundViewController.Reverb.RoomSize, 0, 100, 0.5f, sliderWidth, "F1");
            if (soundViewController.Reverb.RoomSize != result)
            {
                soundViewController.Reverb.RoomSize = result;
                update = true;
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            if (soundViewController.IsReverbEnabled && update)
            {
                this.soundViewController.SetReverb(ref soundViewController.Reverb);
            }
        }

        private void BuildEqParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(width) });

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Space(5);
            GUILayout.Label("Eq", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(soundViewController.IsEqEnabled ? "Disable" : "Enable", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
            {
                soundViewController.IsEqEnabled = !soundViewController.IsEqEnabled;
                if (soundViewController.IsEqEnabled)
                    this.soundViewController.SetEq(ref soundViewController.Eq);
                else
                    this.soundViewController.RemoveEq();
            }
            if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
            {
                soundViewController.Eq = new SaXAudio.EqParameters();
                update = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Band 0", new GUILayoutOption[0]);

            Single result = BuildEffectParam("FrequencyCenter", soundViewController.Eq.FrequencyCenter0, 20, 20000, 10f, sliderWidth);
            if (soundViewController.Eq.FrequencyCenter0 != result)
            {
                soundViewController.Eq.FrequencyCenter0 = result;
                update = true;
            }

            result = BuildEffectParam("Gain", soundViewController.Eq.Gain0, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Gain0 != result)
            {
                soundViewController.Eq.Gain0 = result;
                update = true;
            }

            result = BuildEffectParam("Bandwidth", soundViewController.Eq.Bandwidth0, 0.1f, 2f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Bandwidth0 != result)
            {
                soundViewController.Eq.Bandwidth0 = result;
                update = true;
            }

            GUILayout.Label("Band 1", new GUILayoutOption[0]);

            result = BuildEffectParam("FrequencyCenter", soundViewController.Eq.FrequencyCenter1, 20, 20000, 10f, sliderWidth);
            if (soundViewController.Eq.FrequencyCenter1 != result)
            {
                soundViewController.Eq.FrequencyCenter1 = result;
                update = true;
            }

            result = BuildEffectParam("Gain", soundViewController.Eq.Gain1, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Gain1 != result)
            {
                soundViewController.Eq.Gain1 = result;
                update = true;
            }

            result = BuildEffectParam("Bandwidth", soundViewController.Eq.Bandwidth1, 0.1f, 2f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Bandwidth1 != result)
            {
                soundViewController.Eq.Bandwidth1 = result;
                update = true;
            }

            GUILayout.Label("Band 2", new GUILayoutOption[0]);

            result = BuildEffectParam("FrequencyCenter", soundViewController.Eq.FrequencyCenter2, 20, 20000, 10f, sliderWidth);
            if (soundViewController.Eq.FrequencyCenter2 != result)
            {
                soundViewController.Eq.FrequencyCenter2 = result;
                update = true;
            }

            result = BuildEffectParam("Gain", soundViewController.Eq.Gain2, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Gain2 != result)
            {
                soundViewController.Eq.Gain2 = result;
                update = true;
            }

            result = BuildEffectParam("Bandwidth", soundViewController.Eq.Bandwidth2, 0.1f, 2f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Bandwidth2 != result)
            {
                soundViewController.Eq.Bandwidth2 = result;
                update = true;
            }

            GUILayout.Label("Band 3", new GUILayoutOption[0]);

            result = BuildEffectParam("FrequencyCenter", soundViewController.Eq.FrequencyCenter3, 20, 20000, 10f, sliderWidth);
            if (soundViewController.Eq.FrequencyCenter3 != result)
            {
                soundViewController.Eq.FrequencyCenter3 = result;
                update = true;
            }

            result = BuildEffectParam("Gain", soundViewController.Eq.Gain3, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Gain3 != result)
            {
                soundViewController.Eq.Gain3 = result;
                update = true;
            }

            result = BuildEffectParam("Bandwidth", soundViewController.Eq.Bandwidth3, 0.1f, 2f, 0.01f, sliderWidth, "F2");
            if (soundViewController.Eq.Bandwidth3 != result)
            {
                soundViewController.Eq.Bandwidth3 = result;
                update = true;
            }

            GUILayout.EndVertical();

            if (soundViewController.IsEqEnabled && update)
            {
                this.soundViewController.SetEq(ref soundViewController.Eq);
            }
        }

        private void BuildEchoParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(width) });

            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
            GUILayout.Space(5);
            GUILayout.Label("Echo", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(soundViewController.IsEchoEnabled ? "Disable" : "Enable", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
            {
                soundViewController.IsEchoEnabled = !soundViewController.IsEchoEnabled;
                if (soundViewController.IsEchoEnabled)
                    this.soundViewController.SetEcho(ref soundViewController.Echo);
                else
                    this.soundViewController.RemoveEcho();
            }
            if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
            {
                soundViewController.Echo = new SaXAudio.EchoParameters();
                update = true;
            }
            GUILayout.EndHorizontal();

            Single result = BuildEffectParam("WetDryMix", soundViewController.Echo.WetDryMix * 100f, 0, 100f, 0.5f, sliderWidth, "F1") / 100f;
            if (soundViewController.Echo.WetDryMix != result)
            {
                soundViewController.Echo.WetDryMix = result;
                update = true;
            }

            result = BuildEffectParam("Feedback", soundViewController.Echo.Feedback * 100f, 0, 100f, 0.5f, sliderWidth, "F1") / 100f;
            if (soundViewController.Echo.Feedback != result)
            {
                soundViewController.Echo.Feedback = result;
                update = true;
            }

            result = BuildEffectParam("Delay", soundViewController.Echo.Delay, 1f, 2000f, 1, sliderWidth);
            if (soundViewController.Echo.Delay != result)
            {
                soundViewController.Echo.Delay = result;
                update = true;
            }

            GUILayout.EndVertical();

            if (soundViewController.IsEchoEnabled && update)
            {
                this.soundViewController.SetEcho(ref soundViewController.Echo);
            }
        }

        private Single BuildEffectParam(String name, Single value, Single min, Single max, Single step, Single width, String format = "")
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5);
            GUILayout.Label(name, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label(value.ToString(format), new GUILayoutOption[0]);
            Single result = GUILayout.HorizontalSlider(value, min, max, new GUILayoutOption[] { GUILayout.Width(width) });
            result = Mathf.Round(result / step) * step;
            GUILayout.EndHorizontal();
            return result;
        }

        private void BuildSfxSoundSelector()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.soundSelectorScrollPosition = GUILayout.BeginScrollView(this.soundSelectorScrollPosition, new GUILayoutOption[0]);
            if (this.sfxSoundUIState == 0)
            {
                foreach (Int32 num in this.soundViewController.AllSfxGroupSongIndex)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    if (GUILayout.Button("Load EFX ID: " + num, new GUILayoutOption[0]))
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
                    if (GUILayout.Button(text, new GUILayoutOption[0]))
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

        private void BuildVoiceSoundSelectot()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            List<SoundProfile> voices = soundViewController.GetPlaylist();
            if (voices == null || voices.Count == 0)
            {
                soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition, new GUILayoutOption[0]);
                foreach (String mod in soundViewController.ModVoiceDictionary.Keys)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    if (GUILayout.Button(mod, new GUILayoutOption[0]))
                    {
                        soundViewController.GenerateVoiceList(mod);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                String search = GUILayout.TextField(searchString);
                if (search != searchString && search.Length > 0)
                {
                    soundViewController.FilterVoiceList(search);
                }
                searchString = search;

                if (GUILayout.Button("Back", new GUILayoutOption[0]))
                {
                    searchString = "";
                    voices.Clear();
                }
                soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition, new GUILayoutOption[0]);
                List<SoundProfile> playlist = soundViewController.GetPlaylist();
                if (playlist != null)
                {
                    foreach (SoundProfile soundProfile in playlist)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        if (GUILayout.Button(soundProfile.Name, new GUILayoutOption[0]))
                        {
                            soundViewController.SelectSound(soundProfile);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
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
                GUILayout.Width(fullscreenRect.width / 4f)
            });
            List<SoundProfile> playlist = this.soundViewController.GetPlaylist();
            if (playlist != null)
            {
                foreach (SoundProfile soundProfile in playlist)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    if (GUILayout.Button(soundProfile.Name, new GUILayoutOption[0]))
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

        private static readonly Font font = Font.CreateDynamicFontFromOSFont(["Segoe UI", "Helvetica", " Geneva", "Verdana", "Arial"], 14);

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

        private String searchString = "";
    }
}
