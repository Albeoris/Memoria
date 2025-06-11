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
            soundViewController = new SoundViewController(this);
        }

        private void Exit()
        {
            soundViewController.StopActiveSound();
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
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(4);
                    GUILayout.BeginVertical("box");
                    {
                        BuildSoundName();
                        GUILayout.Space(4);
                        BuildPlayer();
                        BuildAdjustment((fullscreenRect.width - 42) / 4f);
                    }
                    GUILayout.EndVertical();

                    BuildAudioEffects();

                    GUILayout.BeginHorizontal();
                    {
                        BuildSelectorManager((fullscreenRect.width - 16) / 4f);
                        BuildSelector((fullscreenRect.width - 16) / 4f);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(4);
                }
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
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Back"))
                {
                    Exit();
                }
                GUILayout.BeginHorizontal();
                {
                    if (soundViewController != null && soundViewController.GetActiveSound() != null)
                    {
                        GUILayout.Label(soundViewController.GetActiveSound().Name);
                    }
                }
                GUILayout.EndHorizontal();
                if (isEnableSound)
                {
                    if (GUILayout.Button("Suspend SEAD"))
                    {
                        if (ISdLibAPIProxy.Instance.SdSoundSystem_Suspend() == 0)
                        {
                            isEnableSound = false;
                        }
                        else
                        {
                            SoundLib.LogWarning("SdSoundSystem_Suspend failure");
                        }
                    }
                }
                else if (GUILayout.Button("Resume SEAD"))
                {
                    if (ISdLibAPIProxy.Instance.SdSoundSystem_Resume() == 0)
                    {
                        isEnableSound = true;
                    }
                    else
                    {
                        SoundLib.LogWarning("SdSoundSystem_Resume failure");
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void BuildPlayer()
        {
            GUILayoutOption[] btnOption = new GUILayoutOption[] { GUILayout.Width((DebugGuiSkin.GetFullscreenRect().width - 36) / (soundViewController.IsSaXAudio ? 6 : 5)) };

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Previous", btnOption))
                {
                    soundViewController.PreviousSound();
                }
                if (GUILayout.Button(soundViewController.IsPlay ? "Pause" : "Play", btnOption))
                {
                    if (soundViewController.IsPlay)
                        soundViewController.PauseActiveSound();
                    else
                        soundViewController.PlayActiveSound();
                }
                if (soundViewController.IsSaXAudio)
                {
                    if (GUILayout.Button("Loop", btnOption))
                    {
                        if (!soundViewController.IsPlay)
                        {
                            soundViewController.PlayActiveSound();
                            soundViewController.Loop(false);
                        }
                        else
                        {
                            soundViewController.Loop(true);
                        }
                    }
                }
                if (GUILayout.Button("Stop", btnOption))
                {
                    soundViewController.StopActiveSound();
                }
                if (GUILayout.Button("Next", btnOption))
                {
                    soundViewController.NextSound();
                }
                if (GUILayout.Button("Reload", btnOption))
                {
                    soundViewController.ReloadSoundMetaData();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void BuildAdjustment(Single width)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                    {
                        GUILayout.Label("Volume");
                        Single soundVolume = SoundVolume;
                        SoundVolume = GUILayout.HorizontalSlider(SoundVolume, 0f, 1f, new GUILayoutOption[] { GUILayout.Width(width * 0.75f) });
                        if (soundVolume != SoundVolume)
                        {
                            soundViewController.SetVolume(SoundVolume);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Panning");
                        Single panningPosition = PanningPosition;
                        PanningPosition = GUILayout.HorizontalSlider(PanningPosition, -1f, 1f, new GUILayoutOption[] { GUILayout.Width(width * 0.75f) });
                        if (panningPosition != PanningPosition)
                        {
                            soundViewController.SetPanning(PanningPosition);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Pitch");
                        Single pitchPosition = PitchPosition;
                        PitchPosition = GUILayout.HorizontalSlider(PitchPosition, 0f, 2f, new GUILayoutOption[] { GUILayout.Width(width * 0.75f) });
                        if (pitchPosition != PitchPosition)
                        {
                            soundViewController.SetPitch(PitchPosition);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
                {
                    if (soundViewController.IsSaXAudio)
                    {

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Position", new GUILayoutOption[] { GUILayout.Width(0) });
                        Single currentPosition = soundViewController.GetActivePosition();
                        Single newPostision = GUILayout.HorizontalSlider(currentPosition, 0, soundViewController.GetActiveLength());
                        if(Math.Abs(newPostision - currentPosition) > 0.1f)
                        {
                            soundViewController.SeekActive(newPostision);
                        }
                        GUILayout.EndVertical();
                    }
                    if (GUILayout.Button("Reset"))
                    {
                        SoundVolume = 1f;
                        PitchPosition = 1f;
                        PanningPosition = 0f;
                        soundViewController.SetVolume(SoundVolume);
                        soundViewController.SetPitch(PitchPosition);
                        soundViewController.SetPanning(PanningPosition);
                    }
                    GUILayout.BeginHorizontal();
                    {
                        if (isSfxEnabled)
                        {
                            if (GUILayout.Button("Disable SFX"))
                            {
                                SoundLib.DisableSoundEffect();
                                isSfxEnabled = false;
                            }
                        }
                        else if (GUILayout.Button("Enable SFX"))
                        {
                            SoundLib.EnableSoundEffect();
                            isSfxEnabled = true;
                        }
                        if (isMusicEnabled)
                        {
                            if (GUILayout.Button("Disable Music"))
                            {
                                SoundLib.DisableMusic();
                                isMusicEnabled = false;
                            }
                        }
                        else if (GUILayout.Button("Enable Music"))
                        {
                            SoundLib.EnableMusic();
                            isMusicEnabled = true;
                        }
                        if (GUILayout.Button("Next Loop Region"))
                        {
                            SoundLib.SetNextLoopRegion(71);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void BuildSelectorManager(Single width)
        {
            GUILayout.BeginVertical("Box", new GUILayoutOption[] { GUILayout.Width(width) });
            {
                if (GUILayout.Button("Sound Effect"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.SoundEffect);
                }
                if (GUILayout.Button("BGM"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Music);
                    soundViewController.SetPanning(PanningPosition);
                    soundViewController.SetPitch(PitchPosition);
                    soundViewController.SetVolume(SoundVolume);
                }
                if (GUILayout.Button("Song"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Song);
                }
                if (GUILayout.Button("Sfx Sound"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Sfx);
                }
                if (GUILayout.Button("Movie Audio"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.MovieAudio);
                }
                if (soundViewController.ModVoiceDictionary.Count > 0 && GUILayout.Button("Mod Voices"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Voice);
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("<"))
                    {
                        soundViewController.PreviousPlayList();
                    }
                    GUILayout.Label(soundViewController.PlaylistInfo);
                    if (GUILayout.Button(">"))
                    {
                        soundViewController.NextPlayList();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Label(soundViewController.PlaylistDetail);
            }
            GUILayout.EndVertical();
        }

        private void BuildSelector(Single width)
        {
            GUILayout.BeginVertical("Box", new GUILayoutOption[] { GUILayout.Width(width) });
            {
                if (soundViewController.GetActiveSoundType() == SoundProfileType.Sfx)
                {
                    BuildSfxSoundSelector();
                }
                else if (soundViewController.GetActiveSoundType() == SoundProfileType.Voice)
                {
                    BuildVoiceSoundSelectot();
                }
                else
                {
                    BuildSoundSelector();
                }
            }
            GUILayout.EndVertical();
        }

        private void BuildAudioEffects()
        {
            if (soundViewController.IsSaXAudio)
            {
                Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
                fullscreenRect.width -= 42;
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(fullscreenRect.height / 4f) });
                {
                    Single columnWidth = fullscreenRect.width / 4f;
                    Single sliderWidth = fullscreenRect.width / 7.5f;
                    Single buttonWidth = fullscreenRect.width / 10f;

                    GUILayout.Space(4);
                    BuildRevebParameters(columnWidth, sliderWidth, buttonWidth);
                    BuildEqParameters(columnWidth, sliderWidth, buttonWidth);
                    BuildEchoParameters(columnWidth, sliderWidth, buttonWidth);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void BuildRevebParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(width * 2) });
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Reverb");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(soundViewController.IsReverbEnabled ? "Disable" : "Enable", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
                        {
                            soundViewController.IsReverbEnabled = !soundViewController.IsReverbEnabled;
                            if (soundViewController.IsReverbEnabled)
                                soundViewController.SetReverb(ref soundViewController.Reverb);
                            else
                                soundViewController.RemoveReverb();
                        }
                        if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
                        {
                            soundViewController.Reverb = new SaXAudio.ReverbParameters();
                            update = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                    {
                        Single result = BuildEffectParam("WetDryMix", soundViewController.Reverb.WetDryMix, 0, 100, 0.5f, sliderWidth, "F1");
                        if (soundViewController.Reverb.WetDryMix != result)
                        {
                            soundViewController.Reverb.WetDryMix = result;
                            update = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {

                    GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
                    {
                        GUILayout.Label("Indexed parameters (no units)");

                        Single result = BuildEffectParam("PositionLeft", soundViewController.Reverb.PositionLeft, 0, 30, 1, sliderWidth);
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
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
                    {
                        GUILayout.Label("Delay times (ms)");

                        Single result = BuildEffectParam("ReflectionsDelay", soundViewController.Reverb.ReflectionsDelay, 0, 300, 1, sliderWidth);
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

                        GUILayout.Label("Direct parameters");

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
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();

            if (soundViewController.IsReverbEnabled && update)
            {
                soundViewController.SetReverb(ref soundViewController.Reverb);
            }
        }

        private void BuildEqParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(width) });
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Eq");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(soundViewController.IsEqEnabled ? "Disable" : "Enable", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
                    {
                        soundViewController.IsEqEnabled = !soundViewController.IsEqEnabled;
                        if (soundViewController.IsEqEnabled)
                            soundViewController.SetEq(ref soundViewController.Eq);
                        else
                            soundViewController.RemoveEq();
                    }
                    if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
                    {
                        soundViewController.Eq = new SaXAudio.EqParameters();
                        update = true;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Label("Band 0");

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

                GUILayout.Label("Band 1");

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

                GUILayout.Label("Band 2");

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

                GUILayout.Label("Band 3");

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
            }
            GUILayout.EndVertical();

            if (soundViewController.IsEqEnabled && update)
            {
                soundViewController.SetEq(ref soundViewController.Eq);
            }
        }

        private void BuildEchoParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Width(width) });
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Echo");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(soundViewController.IsEchoEnabled ? "Disable" : "Enable", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
                    {
                        soundViewController.IsEchoEnabled = !soundViewController.IsEchoEnabled;
                        if (soundViewController.IsEchoEnabled)
                            soundViewController.SetEcho(ref soundViewController.Echo);
                        else
                            soundViewController.RemoveEcho();
                    }
                    if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(buttonWidth) }))
                    {
                        soundViewController.Echo = new SaXAudio.EchoParameters();
                        update = true;
                    }
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

                if (soundViewController.IsEchoEnabled && update)
                {
                    soundViewController.SetEcho(ref soundViewController.Echo);
                }

                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable all effects"))
                        {
                            soundViewController.IsEchoEnabled = soundViewController.IsEqEnabled = soundViewController.IsReverbEnabled = true;

                            soundViewController.SetReverb(ref soundViewController.Reverb);
                            soundViewController.SetEq(ref soundViewController.Eq);
                            soundViewController.SetEcho(ref soundViewController.Echo);
                        }
                        if (GUILayout.Button("Disable all effects"))
                        {
                            soundViewController.IsEchoEnabled = soundViewController.IsEqEnabled = soundViewController.IsReverbEnabled = false;

                            soundViewController.RemoveReverb();
                            soundViewController.RemoveEq();
                            soundViewController.RemoveEcho();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private Single BuildEffectParam(String name, Single value, Single min, Single max, Single step, Single width, String format = "")
        {
            Single result;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);
                GUILayout.Label(name);
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToString(format));
                result = GUILayout.HorizontalSlider(value, min, max, new GUILayoutOption[] { GUILayout.Width(width) });
                result = Mathf.Round(result / step) * step;
            }
            GUILayout.EndHorizontal();
            return result;
        }

        private void BuildSfxSoundSelector()
        {
            soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition);
            if (sfxSoundUIState == 0)
            {
                foreach (Int32 num in soundViewController.AllSfxGroupSongIndex)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Load EFX ID: " + num))
                        {
                            CurrentSpecialEffectID = num;
                            sfxSoundUIState = 1;
                            soundViewController.LoadSfxSoundGroup(num);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else if (sfxSoundUIState == 1)
            {
                List<String> sfxSoundPlaylist = soundViewController.GetSfxSoundPlaylist(CurrentSpecialEffectID);
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Back"))
                    {
                        sfxSoundUIState = 0;
                    }
                    for (Int32 i = 0; i < sfxSoundPlaylist.Count; i++)
                    {
                        if (i == SoundLib.GetResidentSfxSoundCount())
                        {
                            GUILayout.Label("---- ---- ---- ----");
                        }
                        String text = sfxSoundPlaylist[i];
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(text))
                            {
                                soundViewController.SelectSound(i);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
            else
            {
                SoundLib.LogError("sfxSoundUIState is invalid: " + sfxSoundUIState);
            }
            GUILayout.EndScrollView();
        }

        private void BuildVoiceSoundSelectot()
        {
            List<SoundProfile> voices = soundViewController.GetPlaylist();
            if (voices == null || voices.Count == 0)
            {
                soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition);
                foreach (String mod in soundViewController.ModVoiceDictionary.Keys)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(mod))
                        {
                            soundViewController.GenerateVoiceList(mod);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                String search = GUILayout.TextField(searchString);
                if (search != searchString)
                {
                    soundViewController.FilterVoiceList(search);
                }
                searchString = search;

                if (GUILayout.Button("Back"))
                {
                    searchString = "";
                    voices.Clear();
                }
                soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition);
                List<SoundProfile> playlist = soundViewController.GetPlaylist();
                if (playlist != null)
                {
                    foreach (SoundProfile soundProfile in playlist)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(soundProfile.Name))
                        {
                            soundViewController.SelectSound(soundProfile);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        private void BuildSoundSelector()
        {
            soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition);
            List<SoundProfile> playlist = soundViewController.GetPlaylist();
            if (playlist != null)
            {
                foreach (SoundProfile soundProfile in playlist)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(soundProfile.Name))
                        {
                            soundViewController.SelectSound(soundProfile);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
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
