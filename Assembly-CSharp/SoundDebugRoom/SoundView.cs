using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using Global.Sound.SaXAudio;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static Global.Sound.SaXAudio.AudioEffectManager;

namespace SoundDebugRoom
{
    public class SoundView : MonoBehaviour
    {
        SoundView()
        {
            enabled = false;
            soundViewController = new SoundViewController(this);
        }

        private void OnEnable()
        {
            var state = PersistenSingleton<UIManager>.Instance.State;
            if (state != UIManager.UIState.Pause)
                PersistenSingleton<UIManager>.Instance.GetSceneFromState(state).OnKeyPause(null);

            previousState = PersistenSingleton<UIManager>.Instance.State;
            PersistenSingleton<UIManager>.Instance.State = UIManager.UIState.Quit;

            if (SceneDirector.Instance.CurrentScene == "Title")
                PersistenSingleton<UIManager>.Instance.TitleScene?.gameObject?.SetActive(false);

            if (presetFilterCaption == "FieldID")
                presetFilterIndex = presetFilterCurrent = FF9StateSystem.Common.FF9.fldMapNo;
            else if (presetFilterCaption == "BattleID")
                presetFilterIndex = presetFilterCurrent = FF9StateSystem.Battle.battleMapIndex;
            else if (presetFilterCaption == "BattleBgID")
                presetFilterIndex = presetFilterCurrent = battlebg.nf_BbgNumber;
            ResetEffects();
        }

        private void OnDisable()
        {
            soundViewController.StopActiveSound();
            soundViewController.SetActiveSoundType(SoundProfileType.Default);

            PersistenSingleton<UIManager>.Instance.State = previousState;
            if (previousState == UIManager.UIState.Pause)
                PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).OnKeyPause(null);

            if (SceneDirector.Instance.CurrentScene == "Title")
                PersistenSingleton<UIManager>.Instance.TitleScene?.gameObject?.SetActive(true);

            if (SceneDirector.IsFieldScene())
            {
                ApplyFieldEffects(FF9StateSystem.Common.FF9.fldMapNo);
            }
            else if (SceneDirector.IsBattleScene())
            {
                ApplyBattleEffects(FF9StateSystem.Battle.battleMapIndex, battlebg.nf_BbgNumber);
            }
        }

        private void OnApplicationQuit()
        {
            // Disabling so we don't hide the confirm window
            enabled = false;
        }

        private void OnGUI()
        {
            try
            {
                Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();

                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.skin.font = DebugGuiSkin.font;
                Int32 lineHeight = DebugGuiSkin.font.lineHeight;
                GUI.skin.horizontalSlider.margin = new RectOffset(lineHeight / 2, lineHeight / 2, lineHeight - (Int32)GUI.skin.horizontalSlider.fixedHeight / 2, 0);
                GUI.skin.toggle.alignment = TextAnchor.MiddleLeft;
                GUI.skin.toggle.padding.left = 20;
                GUI.skin.toggle.contentOffset = new Vector2(0, -3f);

                GUISkin guiSkin = GUI.skin;
                DebugGuiSkin.ApplySkin();
                GUI.color = new Color(0.2314f, 0.2588f, 0.2314f, 0.9f);

                GUILayout.BeginArea(fullscreenRect);
                GUILayout.BeginVertical("box");
                {
                    GUI.skin = guiSkin;
                    GUI.color = Color.white;
                    Single width = (fullscreenRect.width - 20) / 4f;
                    GUILayout.Space(4);
                    GUILayout.BeginVertical();
                    {
                        BuildHeader();
                        BuildPlayer(width);
                    }
                    GUILayout.EndVertical();

                    BuildAudioEffects(width);

                    GUILayout.BeginHorizontal();
                    {
                        BuildSelectorManager(width);
                        BuildSelector(width);
                        BuildPresetManager(width);
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

        private void BuildHeader()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DebugGuiSkin.font.lineHeight * 1.5f);
                GUILayout.Label("Sound View");
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(DebugGuiSkin.font.lineHeight * 1.5f)))
                {
                    enabled = false;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();
        }

        private void BuildPlayer(Single width)
        {
            width -= 4;
            GUILayout.BeginHorizontal("Box");
            {
                GUILayout.BeginVertical(GUILayout.Width(width));
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(width));
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Volume", GUILayout.ExpandWidth(false));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"{Mathf.Round(SoundVolume * 100)}", GUILayout.ExpandWidth(false));
                        Single soundVolume = SoundVolume;
                        SoundVolume = GUILayout.HorizontalSlider(SoundVolume, 0f, 1f, GUILayout.Width(width * 0.75f));

                        Rect sliderRect = GUILayoutUtility.GetLastRect();
                        sliderRect.y -= 8;
                        sliderRect.height += 16;
                        Event e = Event.current;
                        if (sliderRect.Contains(e.mousePosition) && e.type == EventType.ScrollWheel)
                        {
                            SoundVolume -= Mathf.Sign(e.delta.y) * 0.01f;
                            SoundVolume = Mathf.Clamp(SoundVolume, 0, 1f);
                            e.Use();
                        }

                        if (soundVolume != SoundVolume)
                        {
                            SoundVolume = Mathf.Round(SoundVolume * 100) / 100f;
                            soundViewController.SetVolume(SoundVolume);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Panning", GUILayout.ExpandWidth(false));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"{Mathf.Round(PanningPosition * 100)}", GUILayout.ExpandWidth(false));
                        Single panningPosition = PanningPosition;
                        PanningPosition = GUILayout.HorizontalSlider(PanningPosition, -1f, 1f, GUILayout.Width(width * 0.75f));

                        Rect sliderRect = GUILayoutUtility.GetLastRect();
                        sliderRect.y -= 8;
                        sliderRect.height += 16;
                        Event e = Event.current;
                        if (sliderRect.Contains(e.mousePosition) && e.type == EventType.ScrollWheel)
                        {
                            PanningPosition -= Mathf.Sign(e.delta.y) * 0.01f;
                            PanningPosition = Mathf.Clamp(PanningPosition, -1f, 1f);
                            e.Use();
                        }

                        if (panningPosition != PanningPosition)
                        {
                            PanningPosition = Mathf.Round(PanningPosition * 100) / 100f;
                            soundViewController.SetPanning(PanningPosition);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Pitch", GUILayout.ExpandWidth(false));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"{Mathf.Round(PitchPosition * 100)}", GUILayout.ExpandWidth(false));
                        Single pitchPosition = PitchPosition;
                        PitchPosition = GUILayout.HorizontalSlider(PitchPosition, 0f, 2f, GUILayout.Width(width * 0.75f));

                        Rect sliderRect = GUILayoutUtility.GetLastRect();
                        sliderRect.y -= 8;
                        sliderRect.height += 16;
                        Event e = Event.current;
                        if (sliderRect.Contains(e.mousePosition) && e.type == EventType.ScrollWheel)
                        {
                            PitchPosition -= Mathf.Sign(e.delta.y) * 0.01f;
                            PitchPosition = Mathf.Clamp(PitchPosition, 0, 2f);
                            e.Use();
                        }

                        if (pitchPosition != PitchPosition)
                        {
                            PitchPosition = Mathf.Round(PitchPosition * 100) / 100f;
                            soundViewController.SetPitch(PitchPosition);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(width));
                {
                    if (IsSaXAudio)
                    {
                        SoundProfile active = soundViewController?.GetActiveSound();
                        GUILayout.Space(4);
                        GUILayout.Label(active != null ? active.Name : "---");
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("", GUILayout.Width(0));

                            Single currentPosition = soundViewController.GetActivePosition();
                            Single length = soundViewController.GetActiveLength();
                            Rect sliderRect = GUILayoutUtility.GetLastRect();

                            // Draw loop markers
                            if (length > 1 && SaXAudio.GetLooping(active.SoundID))
                            {
                                UInt32 loopStart = SaXAudio.GetLoopStart(active.SoundID);
                                UInt32 loopEnd = SaXAudio.GetLoopEnd(active.SoundID);

                                float startMarker = (sliderRect.x + 10) + (width - 30) * loopStart / length;
                                GUI.Label(new Rect(startMarker, sliderRect.y + 8, 10, 11), "┃");

                                float endMarker = (sliderRect.x + 10) + (width - 30) * loopEnd / length;
                                GUI.Label(new Rect(endMarker, sliderRect.y + 8, 10, 11), "┃");
                            }

                            Single newPosition = GUI.HorizontalSlider(new Rect(sliderRect.x + 9, sliderRect.y + 8, width - 18, GUI.skin.horizontalSlider.fixedHeight), currentPosition, 0, length);
                            if (Math.Abs(newPosition - currentPosition) > 100f)
                            {
                                soundViewController.SeekActive(newPosition);
                            }

                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Reset"))
                        {
                            SoundVolume = 1f;
                            PitchPosition = 1f;
                            PanningPosition = 0f;
                            soundViewController.SetVolume(SoundVolume);
                            soundViewController.SetPitch(PitchPosition);
                            soundViewController.SetPanning(PanningPosition);
                        }
                        if (soundViewController.GetActiveSound()?.ResourceID == "Sounds01/BGM_/music101"
                            && GUILayout.Button("Next Loop Region", GUILayout.Width(width * 0.5f)))
                        {
                            SoundLib.SetNextLoopRegion(71);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(width));
                {
                    GUILayout.Space(4);
                    GUILayoutOption btnWidth = GUILayout.Width(width / 2f);

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Previous", btnWidth))
                        {
                            soundViewController.PreviousSound();
                        }
                        if (GUILayout.Button("Next", btnWidth))
                        {
                            soundViewController.NextSound();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(soundViewController.IsPlay ? "Pause" : "Play", btnWidth))
                        {
                            if (soundViewController.IsPlay)
                                soundViewController.PauseActiveSound();
                            else
                                soundViewController.PlayActiveSound();
                        }
                        if (GUILayout.Button("Stop", btnWidth))
                        {
                            soundViewController.StopActiveSound();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        if (IsSaXAudio)
                        {
                            if (soundViewController.IsLooping)
                            {
                                if (GUILayout.Button("Stop Looping", btnWidth))
                                {
                                    soundViewController.IsLooping = false;
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Start Looping", btnWidth))
                                {
                                    soundViewController.IsLooping = true;
                                }
                            }
                        }
                        if (GUILayout.Button("Reload", btnWidth))
                        {
                            soundViewController.ReloadSoundMetaData();
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
            GUILayout.BeginVertical("Box", GUILayout.Width(width));
            {
                if (GUILayout.Button("Sound Effect"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.SoundEffect);
                    soundSelectorScrollPosition = Vector2.zero;
                }
                if (GUILayout.Button("BGM"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Music);
                    soundViewController.SetPanning(PanningPosition);
                    soundViewController.SetPitch(PitchPosition);
                    soundViewController.SetVolume(SoundVolume);
                    soundSelectorScrollPosition = Vector2.zero;
                }
                if (GUILayout.Button("Song"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Song);
                    soundSelectorScrollPosition = Vector2.zero;
                }
                if (GUILayout.Button("Sfx Sound"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.Sfx);
                    soundSelectorScrollPosition = Vector2.zero;
                }
                if (GUILayout.Button("Movie Audio"))
                {
                    soundViewController.SetActiveSoundType(SoundProfileType.MovieAudio);
                    soundSelectorScrollPosition = Vector2.zero;
                }
                if (soundViewController.ModVoiceDictionary.Count > 0)
                {
                    if (GUILayout.Button("Mod Voices"))
                    {
                        soundViewController.SetActiveSoundType(SoundProfileType.Voice);
                        soundSelectorScrollPosition = Vector2.zero;
                    }
                }
                else
                {
                    GUILayout.Button("Loading voices...");
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("<"))
                    {
                        soundViewController.PreviousPlayList();
                        soundSelectorScrollPosition = Vector2.zero;
                    }
                    GUILayout.Label(soundViewController.PlaylistInfo, GUILayout.Width(width / 3f));
                    if (GUILayout.Button(">"))
                    {
                        soundViewController.NextPlayList();
                        soundSelectorScrollPosition = Vector2.zero;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Label(soundViewController.PlaylistDetail);
            }
            GUILayout.EndVertical();
        }

        private void BuildSelector(Single width)
        {
            GUILayout.BeginVertical("Box", GUILayout.Width(width));
            {
                if (soundViewController.GetActiveSoundType() == SoundProfileType.Sfx)
                {
                    BuildSfxSoundSelector();
                }
                else if (soundViewController.GetActiveSoundType() == SoundProfileType.Voice)
                {
                    BuildVoiceSoundSelector();
                }
                else
                {
                    BuildSoundSelector();
                }
            }
            GUILayout.EndVertical();
        }

        private void BuildAudioEffects(Single width)
        {
            if (!IsSaXAudio) return;
            GUILayout.BeginHorizontal(GUILayout.Height(1));
            {
                Single sliderWidth = width / 2f;
                Single buttonWidth = width / 2.5f;

                GUILayout.Space(4);
                BuildReverbParameters(width, sliderWidth, buttonWidth);
                GUILayout.BeginVertical();
                {
                    BuildEqParameters(width, sliderWidth, buttonWidth);

                    GUILayout.BeginHorizontal();
                    {
                        BuildEchoParameters(width, sliderWidth, buttonWidth);
                        BuildVolumeParameter(width, sliderWidth, buttonWidth);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void BuildReverbParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            width -= 4;
            Boolean update = false;
            GUILayout.BeginVertical("box", GUILayout.Width(1));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical(GUILayout.Width(width));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(5);
                            GUILayout.Label("Reverb");
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button(soundViewController.IsReverbEnabled ? "Disable" : "Enable", GUILayout.Width(buttonWidth)))
                            {
                                presetChanged = true;
                                soundViewController.IsReverbEnabled = !soundViewController.IsReverbEnabled;
                                if (soundViewController.IsReverbEnabled)
                                    soundViewController.SetReverb();
                                else
                                    soundViewController.RemoveReverb();
                            }
                            if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
                            {
                                soundViewController.CurrentEffect.Reverb = new SaXAudio.ReverbParameters();
                                update = true;
                            }
                        }
                        GUILayout.EndHorizontal();
                        Single result = BuildEffectParam("WetDryMix", soundViewController.CurrentEffect.Reverb.WetDryMix, 0, 100, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.WetDryMix != result)
                        {
                            soundViewController.CurrentEffect.Reverb.WetDryMix = result;
                            update = true;
                        }

                        GUILayout.Label("Indexed parameters (no units)");

                        result = BuildEffectParam("PositionLeft", soundViewController.CurrentEffect.Reverb.PositionLeft, 0, 30, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.PositionLeft != result)
                        {
                            soundViewController.CurrentEffect.Reverb.PositionLeft = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("PositionRight", soundViewController.CurrentEffect.Reverb.PositionRight, 0, 30, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.PositionRight != result)
                        {
                            soundViewController.CurrentEffect.Reverb.PositionRight = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("PositionMatrixLeft", soundViewController.CurrentEffect.Reverb.PositionMatrixLeft, 0, 30, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.PositionMatrixLeft != result)
                        {
                            soundViewController.CurrentEffect.Reverb.PositionMatrixLeft = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("PositionMatrixRight", soundViewController.CurrentEffect.Reverb.PositionMatrixRight, 0, 30, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.PositionMatrixRight != result)
                        {
                            soundViewController.CurrentEffect.Reverb.PositionMatrixRight = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("EarlyDiffusion", soundViewController.CurrentEffect.Reverb.EarlyDiffusion, 0, 15, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.EarlyDiffusion != result)
                        {
                            soundViewController.CurrentEffect.Reverb.EarlyDiffusion = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("LateDiffusion", soundViewController.CurrentEffect.Reverb.LateDiffusion, 0, 15, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.LateDiffusion != result)
                        {
                            soundViewController.CurrentEffect.Reverb.LateDiffusion = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("LowEQGain", soundViewController.CurrentEffect.Reverb.LowEQGain, 0, 12, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.LowEQGain != result)
                        {
                            soundViewController.CurrentEffect.Reverb.LowEQGain = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("LowEQCutoff", soundViewController.CurrentEffect.Reverb.LowEQCutoff, 0, 9, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.LowEQCutoff != result)
                        {
                            soundViewController.CurrentEffect.Reverb.LowEQCutoff = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("HighEQGain", soundViewController.CurrentEffect.Reverb.HighEQGain, 0, 8, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.HighEQGain != result)
                        {
                            soundViewController.CurrentEffect.Reverb.HighEQGain = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("HighEQCutoff", soundViewController.CurrentEffect.Reverb.HighEQCutoff, 0, 14, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.HighEQCutoff != result)
                        {
                            soundViewController.CurrentEffect.Reverb.HighEQCutoff = (Byte)result;
                            update = true;
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Width(width));
                    {
                        GUILayout.Label("Delay times (ms)");

                        Single result = BuildEffectParam("ReflectionsDelay", soundViewController.CurrentEffect.Reverb.ReflectionsDelay, 0, 300, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.ReflectionsDelay != result)
                        {
                            soundViewController.CurrentEffect.Reverb.ReflectionsDelay = (UInt32)result;
                            update = true;
                        }

                        result = BuildEffectParam("ReverbDelay", soundViewController.CurrentEffect.Reverb.ReverbDelay, 0, 85, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.ReverbDelay != result)
                        {
                            soundViewController.CurrentEffect.Reverb.ReverbDelay = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("RearDelay", soundViewController.CurrentEffect.Reverb.RearDelay, 0, 5, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.RearDelay != result)
                        {
                            soundViewController.CurrentEffect.Reverb.RearDelay = (Byte)result;
                            update = true;
                        }

                        result = BuildEffectParam("SideDelay", soundViewController.CurrentEffect.Reverb.SideDelay, 0, 5, 1, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.SideDelay != result)
                        {
                            soundViewController.CurrentEffect.Reverb.SideDelay = (Byte)result;
                            update = true;
                        }

                        GUILayout.Label("Direct parameters");

                        result = BuildEffectParam("RoomFilterFreq", soundViewController.CurrentEffect.Reverb.RoomFilterFreq, 20, 20000, 10, sliderWidth);
                        if (soundViewController.CurrentEffect.Reverb.RoomFilterFreq != result)
                        {
                            soundViewController.CurrentEffect.Reverb.RoomFilterFreq = (UInt32)result;
                            update = true;
                        }

                        result = BuildEffectParam("RoomFilterMain", soundViewController.CurrentEffect.Reverb.RoomFilterMain, -100f, 0, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.RoomFilterMain != result)
                        {
                            soundViewController.CurrentEffect.Reverb.RoomFilterMain = result;
                            update = true;
                        }

                        result = BuildEffectParam("RoomFilterHF", soundViewController.CurrentEffect.Reverb.RoomFilterHF, -100f, 0, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.RoomFilterHF != result)
                        {
                            soundViewController.CurrentEffect.Reverb.RoomFilterHF = result;
                            update = true;
                        }

                        result = BuildEffectParam("ReflectionsGain", soundViewController.CurrentEffect.Reverb.ReflectionsGain, -100f, 20, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.ReflectionsGain != result)
                        {
                            soundViewController.CurrentEffect.Reverb.ReflectionsGain = result;
                            update = true;
                        }

                        result = BuildEffectParam("ReverbGain", soundViewController.CurrentEffect.Reverb.ReverbGain, -100f, 20, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.ReverbGain != result)
                        {
                            soundViewController.CurrentEffect.Reverb.ReverbGain = result;
                            update = true;
                        }

                        result = BuildEffectParam("DecayTime", soundViewController.CurrentEffect.Reverb.DecayTime * 1000f, 100f, 5000f, 10f, sliderWidth, "F1") / 1000f;
                        if (soundViewController.CurrentEffect.Reverb.DecayTime != result)
                        {
                            soundViewController.CurrentEffect.Reverb.DecayTime = result;
                            update = true;
                        }

                        result = BuildEffectParam("Density", soundViewController.CurrentEffect.Reverb.Density, 0, 100, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.Density != result)
                        {
                            soundViewController.CurrentEffect.Reverb.Density = result;
                            update = true;
                        }
                        result = BuildEffectParam("RoomSize", soundViewController.CurrentEffect.Reverb.RoomSize, 0, 100, 0.1f, sliderWidth, "F1");
                        if (soundViewController.CurrentEffect.Reverb.RoomSize != result)
                        {
                            soundViewController.CurrentEffect.Reverb.RoomSize = result;
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
                presetChanged = true;
                soundViewController.SetReverb();
            }
        }

        private void BuildEqParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            width -= 4;
            Boolean update = false;
            GUILayout.BeginVertical("box", GUILayout.Width(1));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical(GUILayout.Width(width));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(5);
                            GUILayout.Label("Eq");
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button(soundViewController.IsEqEnabled ? "Disable" : "Enable", GUILayout.Width(buttonWidth)))
                            {
                                presetChanged = true;
                                soundViewController.IsEqEnabled = !soundViewController.IsEqEnabled;
                                if (soundViewController.IsEqEnabled)
                                    soundViewController.SetEq();
                                else
                                    soundViewController.RemoveEq();
                            }
                            if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
                            {
                                soundViewController.CurrentEffect.Eq = new SaXAudio.EqParameters();
                                update = true;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Band 1");
                        Single result = BuildEffectParam("FrequencyCenter", soundViewController.CurrentEffect.Eq.FrequencyCenter0, 20, 20000, 10f, sliderWidth);
                        if (soundViewController.CurrentEffect.Eq.FrequencyCenter0 != result)
                        {
                            soundViewController.CurrentEffect.Eq.FrequencyCenter0 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Gain", soundViewController.CurrentEffect.Eq.Gain0, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Gain0 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Gain0 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Bandwidth", soundViewController.CurrentEffect.Eq.Bandwidth0, 0.1f, 2f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Bandwidth0 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Bandwidth0 = result;
                            update = true;
                        }

                        GUILayout.Label("Band 3");
                        result = BuildEffectParam("FrequencyCenter", soundViewController.CurrentEffect.Eq.FrequencyCenter2, 20, 20000, 10f, sliderWidth);
                        if (soundViewController.CurrentEffect.Eq.FrequencyCenter2 != result)
                        {
                            soundViewController.CurrentEffect.Eq.FrequencyCenter2 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Gain", soundViewController.CurrentEffect.Eq.Gain2, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Gain2 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Gain2 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Bandwidth", soundViewController.CurrentEffect.Eq.Bandwidth2, 0.1f, 2f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Bandwidth2 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Bandwidth2 = result;
                            update = true;
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUILayout.Width(width));
                    {
                        GUILayout.Label("");
                        GUILayout.Label("Band 2");
                        Single result = BuildEffectParam("FrequencyCenter", soundViewController.CurrentEffect.Eq.FrequencyCenter1, 20, 20000, 10f, sliderWidth);
                        if (soundViewController.CurrentEffect.Eq.FrequencyCenter1 != result)
                        {
                            soundViewController.CurrentEffect.Eq.FrequencyCenter1 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Gain", soundViewController.CurrentEffect.Eq.Gain1, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Gain1 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Gain1 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Bandwidth", soundViewController.CurrentEffect.Eq.Bandwidth1, 0.1f, 2f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Bandwidth1 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Bandwidth1 = result;
                            update = true;
                        }

                        GUILayout.Label("Band 4");
                        result = BuildEffectParam("FrequencyCenter", soundViewController.CurrentEffect.Eq.FrequencyCenter3, 20, 20000, 10f, sliderWidth);
                        if (soundViewController.CurrentEffect.Eq.FrequencyCenter3 != result)
                        {
                            soundViewController.CurrentEffect.Eq.FrequencyCenter3 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Gain", soundViewController.CurrentEffect.Eq.Gain3, 0.126f, 7.94f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Gain3 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Gain3 = result;
                            update = true;
                        }

                        result = BuildEffectParam("Bandwidth", soundViewController.CurrentEffect.Eq.Bandwidth3, 0.1f, 2f, 0.01f, sliderWidth, "F2");
                        if (soundViewController.CurrentEffect.Eq.Bandwidth3 != result)
                        {
                            soundViewController.CurrentEffect.Eq.Bandwidth3 = result;
                            update = true;
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            if (soundViewController.IsEqEnabled && update)
            {
                presetChanged = true;
                soundViewController.SetEq();
            }
        }

        private void BuildEchoParameters(Single width, Single sliderWidth, Single buttonWidth)
        {
            Boolean update = false;

            GUILayout.BeginVertical("box", GUILayout.Width(width));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Echo");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(soundViewController.IsEchoEnabled ? "Disable" : "Enable", GUILayout.Width(buttonWidth)))
                    {
                        presetChanged = true;
                        soundViewController.IsEchoEnabled = !soundViewController.IsEchoEnabled;
                        if (soundViewController.IsEchoEnabled)
                            soundViewController.SetEcho();
                        else
                            soundViewController.RemoveEcho();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
                    {
                        soundViewController.CurrentEffect.Echo = new SaXAudio.EchoParameters();
                        update = true;
                    }
                }
                GUILayout.EndHorizontal();

                Single result = BuildEffectParam("WetDryMix", soundViewController.CurrentEffect.Echo.WetDryMix * 100f, 0, 100f, 0.1f, sliderWidth, "F1") / 100f;
                if (soundViewController.CurrentEffect.Echo.WetDryMix != result)
                {
                    soundViewController.CurrentEffect.Echo.WetDryMix = result;
                    update = true;
                }

                result = BuildEffectParam("Feedback", soundViewController.CurrentEffect.Echo.Feedback * 100f, 0, 100f, 0.1f, sliderWidth, "F1") / 100f;
                if (soundViewController.CurrentEffect.Echo.Feedback != result)
                {
                    soundViewController.CurrentEffect.Echo.Feedback = result;
                    update = true;
                }

                result = BuildEffectParam("Delay", soundViewController.CurrentEffect.Echo.Delay, 1f, 2000f, 1, sliderWidth);
                if (soundViewController.CurrentEffect.Echo.Delay != result)
                {
                    soundViewController.CurrentEffect.Echo.Delay = result;
                    update = true;
                }

                if (soundViewController.IsEchoEnabled && update)
                {
                    presetChanged = true;
                    soundViewController.SetEcho();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }

        private void BuildVolumeParameter(Single width, Single sliderWidth, Single buttonWidth)
        {
            GUILayout.BeginVertical("box", GUILayout.Width(width));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Volume");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(soundViewController.IsEffectVolumeEnabled ? "Disable" : "Enable", GUILayout.Width(buttonWidth)))
                    {
                        presetChanged = true;
                        soundViewController.IsEffectVolumeEnabled = !soundViewController.IsEffectVolumeEnabled;
                        soundViewController.SetEffectVolume();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
                    {
                        presetChanged = true;
                        soundViewController.CurrentEffect.Volume = 1f;
                        soundViewController.SetEffectVolume();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    Single volume = soundViewController.CurrentEffect.Volume * 100f;
                    GUILayout.Space(5);
                    GUILayout.Label(volume.ToString());
                    GUILayout.FlexibleSpace();
                    Single result = GUILayout.HorizontalSlider(volume, 0, 500f, GUILayout.Width(width - 60));
                    result = Mathf.Round(result) / 100f;

                    Rect sliderRect = GUILayoutUtility.GetLastRect();
                    sliderRect.y -= 8;
                    sliderRect.height += 16;
                    Event e = Event.current;
                    if (sliderRect.Contains(e.mousePosition) && e.type == EventType.ScrollWheel)
                    {
                        result -= Mathf.Sign(e.delta.y) * 0.01f;
                        result = Mathf.Clamp(result, 0, 5f);
                        e.Use();
                    }
                    if (soundViewController.CurrentEffect.Volume != result)
                    {
                        presetChanged = true;
                        soundViewController.CurrentEffect.Volume = result;
                        soundViewController.SetEffectVolume();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
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
                result = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(width));
                result = Mathf.Round(result / step) * step;

                Rect sliderRect = GUILayoutUtility.GetLastRect();
                sliderRect.y -= 8;
                sliderRect.height += 16;
                Event e = Event.current;
                if (sliderRect.Contains(e.mousePosition) && e.type == EventType.ScrollWheel)
                {
                    result -= Mathf.Sign(e.delta.y) * step;
                    result = Mathf.Clamp(result, min, max);
                    e.Use();
                }
            }
            GUILayout.EndHorizontal();
            return result;
        }

        private void BuildSfxSoundSelector()
        {
            soundSelectorScrollPosition = GUILayout.BeginScrollView(soundSelectorScrollPosition, GUILayout.ExpandHeight(true));
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

        private void BuildVoiceSoundSelector()
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
                    soundViewController.GenerateVoiceList("");
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
                        if (GUILayout.Button($"{soundProfile.Name} ({soundProfile.SoundIndex})"))
                        {
                            soundViewController.SelectSound(soundProfile);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        private void BuildPresetManager(Single width)
        {
            if (!IsSaXAudio) return;
            GUILayout.BeginVertical("box", GUILayout.Width(width));
            {
                GUILayout.Label("Effect Manager");
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Enable All"))
                    {
                        presetChanged = true;
                        soundViewController.CurrentEffect.Effects = AudioEffectManager.EffectPreset.Effect.All;

                        soundViewController.SetReverb();
                        soundViewController.SetEq();
                        soundViewController.SetEcho();
                        soundViewController.SetEffectVolume();
                    }
                    if (GUILayout.Button("Disable All"))
                    {
                        presetChanged = true;
                        soundViewController.CurrentEffect.Effects = 0;

                        soundViewController.RemoveReverb();
                        soundViewController.RemoveEq();
                        soundViewController.RemoveEcho();
                        soundViewController.RemoveEffectVolume();
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button(presetManagerMod == null ? "  Select Mod ➔" : "Select Mod"))
                {
                    presetManagerMod = null;
                }

                if (presetManagerMod != null)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Backup");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Create", GUILayout.Width(7f * width / 20f)))
                        {
                            soundViewController.SaveEffectPresets(presetManagerMod, true);
                        }
                        if (GUILayout.Button("Restore", GUILayout.Width(7f * width / 20f)))
                        {
                            soundViewController.LoadEffectPresets(presetManagerMod, true);
                            soundViewController.SaveEffectPresets(presetManagerMod, false);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.Label($"Editing '{presetName}'");

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Preset Name");
                        GUILayout.FlexibleSpace();
                        String name = presetName;
                        presetName = GUILayout.TextField(name, GUILayout.Width(4 + width / 2f + width / 5f));
                        if (name != presetName)
                        {
                            presetChanged = true;
                            presetName = presetName.Replace(";", "");
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    GUILayout.Label("Applies to");
                    ref EffectPreset preset = ref soundViewController.CurrentEffect;

                    GUILayout.BeginHorizontal();
                    {
                        EffectPreset.Layer layers = preset.Layers;

                        Boolean music = (preset.Layers & EffectPreset.Layer.Music) != 0;
                        music = GUILayout.Toggle(music, "Music", GUILayout.Width(width / 5f));
                        preset.Layers = music ? preset.Layers | EffectPreset.Layer.Music : preset.Layers & ~EffectPreset.Layer.Music;
                        GUILayout.FlexibleSpace();
                        Boolean ambient = (preset.Layers & EffectPreset.Layer.Ambient) != 0;
                        ambient = GUILayout.Toggle(ambient, "Ambient", GUILayout.Width(width / 5f));
                        preset.Layers = ambient ? preset.Layers | EffectPreset.Layer.Ambient : preset.Layers & ~EffectPreset.Layer.Ambient;
                        GUILayout.FlexibleSpace();
                        Boolean sounds = (preset.Layers & EffectPreset.Layer.SoundEffect) != 0;
                        sounds = GUILayout.Toggle(sounds, "Sounds", GUILayout.Width(width / 5f));
                        preset.Layers = sounds ? preset.Layers | EffectPreset.Layer.SoundEffect : preset.Layers & ~EffectPreset.Layer.SoundEffect;
                        GUILayout.FlexibleSpace();
                        Boolean voice = (preset.Layers & EffectPreset.Layer.Voice) != 0;
                        voice = GUILayout.Toggle(voice, "Voice", GUILayout.Width(width / 5f));
                        preset.Layers = voice ? preset.Layers | EffectPreset.Layer.Voice : preset.Layers & ~EffectPreset.Layer.Voice;

                        if (layers != preset.Layers) presetChanged = true;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    GUILayout.Label("Filters");

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button($"FieldID ({preset.FieldIDs.Count})", GUILayout.Width((width - 20) / 2f)))
                        {
                            presetFilterCaption = "FieldID";
                            presetFilterIndex = presetFilterCurrent = FF9StateSystem.Common.FF9.fldMapNo;
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button($"ResourceID ({preset.ResourceIDs.Count})", GUILayout.Width((width - 20) / 2f)))
                        {
                            presetFilterCaption = "ResourceID";
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button($"BattleBgID ({preset.BattleBgIDs.Count})", GUILayout.Width((width - 20) / 2f)))
                        {
                            presetFilterCaption = "BattleBgID";
                            presetFilterIndex = presetFilterCurrent = battlebg.nf_BbgNumber;
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button($"BattleID ({preset.BattleIDs.Count})", GUILayout.Width((width - 20) / 2f)))
                        {
                            presetFilterCaption = "BattleID";
                            presetFilterIndex = presetFilterCurrent = FF9StateSystem.Battle.battleMapIndex;
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    GUILayout.Label("NCalc Condition");
                    String condition = preset.Condition;
                    preset.Condition = GUILayout.TextField(preset.Condition);
                    if (condition != preset.Condition) presetChanged = true;

                    GUILayout.Space(5);

                    GUI.color = presetChanged ? new Color(1f, 0.75f, 0.5f) : Color.white;
                    if (GUILayout.Button("Save Preset"))
                    {
                        soundViewController.SaveEffectPreset(presetName, presetManagerMod);
                        presetChanged = false;
                    }
                    GUI.color = Color.white;
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box", GUILayout.Width(width));
            {
                if (presetFilterCaption != null)
                {
                    BuildPresetFilterList(width);
                }
                else if (presetManagerMod == null)
                    BuildPresetModList();
                else
                    BuildPresetList();
            }
            GUILayout.EndVertical();
        }

        private void BuildPresetModList()
        {
            GUILayout.Label("Select Mod");
            soundPresetManagerScrollPosition = GUILayout.BeginScrollView(soundPresetManagerScrollPosition);
            foreach (AssetManager.AssetFolder folder in AssetManager.FolderHighToLow)
            {
                if (String.IsNullOrEmpty(folder.FolderPath))
                    continue;
                if (GUILayout.Button(folder.FolderPath.TrimEnd('/')))
                {
                    presetManagerMod = folder.FolderPath;
                    soundViewController.LoadEffectPresets(folder.FolderPath);
                    presetChanged = false;
                }
            }
            GUILayout.EndScrollView();
        }

        private void BuildPresetList()
        {
            GUILayout.Label($"{presetManagerMod.TrimEnd('/')} Presets");
            soundPresetManagerScrollPosition = GUILayout.BeginScrollView(soundPresetManagerScrollPosition);
            String toDelete = null;
            foreach (String preset in soundViewController.EffectPresetDictionary.Keys)
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(preset))
                    {
                        presetChanged = false;
                        presetName = preset;
                        soundViewController.ApplyEffectPreset(preset);
                    }

                    if (GUILayout.Button(" Delete ", GUILayout.ExpandWidth(false)))
                    {
                        toDelete = preset;
                    }
                }
                GUILayout.EndHorizontal();
            }
            if (toDelete != null)
                soundViewController.DeleteEffectPreset(toDelete, presetManagerMod);
            GUILayout.EndScrollView();
        }

        private void BuildPresetFilterList(Single width)
        {
            GUILayout.Label($"'{presetName}' {presetFilterCaption} Filters");

            if (GUILayout.Button("Back"))
            {
                presetFilterCaption = null;
                return;
            }

            if (presetFilterCaption == "ResourceID")
            {
                BuildPresetFilterResourceIDList(width);
                return;
            }

            HashSet<Int32> presetFilterIds;
            if (presetFilterCaption == "FieldID")
                presetFilterIds = soundViewController.CurrentEffect.FieldIDs;
            else if (presetFilterCaption == "BattleID")
                presetFilterIds = soundViewController.CurrentEffect.BattleIDs;
            else if (presetFilterCaption == "BattleBgID")
                presetFilterIds = soundViewController.CurrentEffect.BattleBgIDs;
            else
                return;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(presetFilterCaption);
                GUILayout.FlexibleSpace();

                String value = presetFilterIndex == Int32.MinValue ? "" : $"{presetFilterIndex}";
                String result = GUILayout.TextField(value, GUILayout.Width(width / 6f));
                if (result != value)
                {
                    result = Regex.Replace(result, @"[^\d]", "");
                    if (result.Length == 0)
                        presetFilterIndex = Int32.MinValue;
                    else
                        Int32.TryParse(result, out presetFilterIndex);
                }
                if (GUILayout.Button("Add", GUILayout.Width(width / 3.5f)) && presetFilterIndex != Int32.MinValue)
                {
                    presetChanged = true;
                    presetFilterIds.Add(presetFilterIndex);
                }
                if (GUILayout.Button($"Add current ({presetFilterCurrent})", GUILayout.Width(width / 3.5f)))
                {
                    presetChanged = true;
                    presetFilterIds.Add(presetFilterCurrent);
                }
            }
            GUILayout.EndHorizontal();

            soundPresetManagerScrollPosition = GUILayout.BeginScrollView(soundPresetManagerScrollPosition);
            Int32 toDelete = Int32.MinValue;
            foreach (Int32 id in presetFilterIds.OrderBy(x => x))
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Button($"{id}");

                    if (GUILayout.Button(" Delete ", GUILayout.ExpandWidth(false)))
                    {
                        toDelete = id;
                    }
                }
                GUILayout.EndHorizontal();
            }
            if (toDelete != Int32.MinValue)
            {
                presetChanged = true;
                presetFilterIds.Remove(toDelete);
            }
            GUILayout.EndScrollView();
        }

        private void BuildPresetFilterResourceIDList(Single width)
        {
            HashSet<String> presetFilterIds = soundViewController.CurrentEffect.ResourceIDs;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(presetFilterCaption);
                GUILayout.FlexibleSpace();

                String value = presetFilterResourceIndex;
                String result = GUILayout.TextField(value, GUILayout.Width(width / 2f));
                if (result != value)
                {
                    presetFilterResourceIndex = Regex.Replace(result, @"[;|]", "");
                }
                if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)) && !String.IsNullOrEmpty(presetFilterResourceIndex))
                {
                    presetChanged = true;
                    presetFilterIds.Add(presetFilterResourceIndex);
                }
            }
            GUILayout.EndHorizontal();

            soundPresetManagerScrollPosition = GUILayout.BeginScrollView(soundPresetManagerScrollPosition);
            String toDelete = null;
            foreach (String id in presetFilterIds.OrderBy(x => x))
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Button($"{id}");

                    if (GUILayout.Button(" Delete ", GUILayout.ExpandWidth(false)))
                    {
                        toDelete = id;
                    }
                }
                GUILayout.EndHorizontal();
            }
            if (toDelete != null)
            {
                presetChanged = true;
                presetFilterIds.Remove(toDelete);
            }
            GUILayout.EndScrollView();
        }

        public Single SoundVolume = 1f;

        public Single SeekerPosition;

        public Single PitchPosition = 1f;

        public Single PanningPosition;

        private Vector2 soundSelectorScrollPosition = new Vector2(0f, 0f);

        private Vector2 soundPresetManagerScrollPosition = new Vector2(0f, 0f);

        private SoundViewController soundViewController;

        private Int32 sfxSoundUIState;

        private Int32 CurrentSpecialEffectID;

        private String searchString = "";

        private String presetManagerMod = null;

        private Boolean presetChanged = false;

        private String presetName = "My preset";

        private String presetFilterCaption = null;

        private Int32 presetFilterIndex = Int32.MinValue;

        public String presetFilterResourceIndex = "";

        private Int32 presetFilterCurrent = Int32.MinValue;

        private UIManager.UIState previousState;
    }
}
