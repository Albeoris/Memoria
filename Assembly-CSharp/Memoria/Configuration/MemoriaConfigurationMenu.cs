using Assets.Sources.Scripts.Common;
using System;
using UnityEngine;

namespace Memoria
{
    public class MemoriaConfigurationMenu : MonoBehaviour
    {
        private Single _timer;
        private Boolean _soundVolumeChanged;
        private Boolean _musicVolumeChanged;
        private Boolean _movieVolumeChanged;
        private Boolean _voiceVolumeChanged;

        private void OnEnable()
        {
            _soundVolumeChanged = false;
            _musicVolumeChanged = false;
            _movieVolumeChanged = false;
            _voiceVolumeChanged = false;
        }

        private void OnDisable()
        {
            // Save changes on hide menu
            SaveChanges();
        }

        private void Update()
        {
            // Save changes every 10 seconds
            _timer += Time.deltaTime;
            if (_timer > 10)
            {
                SaveChanges();
                _timer = 0;
            }
        }

        private void SaveChanges()
        {
            if (_soundVolumeChanged)
            {
                Configuration.Audio.SaveSoundVolume();
                _soundVolumeChanged = false;
            }

            if (_musicVolumeChanged)
            {
                Configuration.Audio.SaveMusicVolume();
                _musicVolumeChanged = false;
            }

            if (_movieVolumeChanged)
            {
                Configuration.Audio.SaveMovieVolume();
                _movieVolumeChanged = false;
            }

            if (_voiceVolumeChanged)
            {
                Configuration.VoiceActing.SaveVolume();
                _voiceVolumeChanged = false;
            }
        }

        private void OnGUI()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            Int32 lineHeight = DebugGuiSkin.font.lineHeight;
            GUI.skin.horizontalSlider.margin = new RectOffset(lineHeight / 2, lineHeight / 2, lineHeight - (Int32)GUI.skin.horizontalSlider.fixedHeight / 2, 0);
            GUI.skin.font = DebugGuiSkin.font;
            GUISkin guiSkin = GUI.skin;
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            DebugGuiSkin.ApplySkin();
            GUI.skin.box.padding = new RectOffset(10, 10, 0, 10);

            GUILayout.BeginArea(fullscreenRect);
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical("Box", GUILayout.Width(Mathf.Round(fullscreenRect.width / 4f)));
                {
                    GUI.skin = guiSkin;
                    GUI.color = Color.white;
                    Int32 width = Mathf.RoundToInt(fullscreenRect.width / 5f);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Volume");
                    if (GUILayout.Button("X", GUILayout.Width(lineHeight * 1.5f)))
                    {
                        enabled = false;
                    }
                    GUILayout.EndHorizontal();


                    BuildSoundSlider(width);
                    BuildMusicSlider(width);
                    BuildMovieSlider(width);
                    if (Configuration.VoiceActing.Enabled)
                        BuildVoiceSlider(width);
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void BuildSoundSlider(Int32 width)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sound");
            GUILayout.FlexibleSpace();
            Int32 oldValue = Configuration.Audio.SoundVolume;
            GUILayout.Label(oldValue.ToString());
            Int32 newValue = Mathf.RoundToInt(GUILayout.HorizontalSlider(oldValue, 0, 100, GUILayout.Width(width)) / 5f) * 5;
            if (oldValue != newValue)
            {
                _soundVolumeChanged = true;
                Configuration.Audio.SoundVolume = newValue;
                SoundLib.TryUpdateSoundVolume();
            }
            GUILayout.EndHorizontal();
        }

        private void BuildMusicSlider(Int32 width)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Music");
            GUILayout.FlexibleSpace();
            Int32 oldValue = Configuration.Audio.MusicVolume;
            GUILayout.Label(oldValue.ToString());
            Int32 newValue = Mathf.RoundToInt(GUILayout.HorizontalSlider(oldValue, 0, 100, GUILayout.Width(width)) / 5f) * 5;
            if (oldValue != newValue)
            {
                _musicVolumeChanged = true;
                Configuration.Audio.MusicVolume = newValue;
                SoundLib.TryUpdateMusicVolume();
            }
            GUILayout.EndHorizontal();
        }

        private void BuildMovieSlider(Int32 width)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Movie");
            GUILayout.FlexibleSpace();
            Int32 oldValue = Configuration.Audio.MovieVolume;
            GUILayout.Label(oldValue.ToString());
            Int32 newValue = Mathf.RoundToInt(GUILayout.HorizontalSlider(oldValue, 0, 100, GUILayout.Width(width)) / 5f) * 5;
            if (oldValue != newValue)
            {
                _movieVolumeChanged = true;
                Configuration.Audio.MovieVolume = newValue;
                SoundLib.TryUpdateMusicVolume();
            }
            GUILayout.EndHorizontal();
        }

        private void BuildVoiceSlider(Int32 width)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Voice");
            GUILayout.FlexibleSpace();
            Int32 oldValue = Configuration.VoiceActing.Volume;
            GUILayout.Label(oldValue.ToString());
            Int32 newValue = Mathf.RoundToInt(GUILayout.HorizontalSlider(oldValue, 0, 100, GUILayout.Width(width)) / 5f) * 5;
            if (oldValue != newValue)
            {
                _voiceVolumeChanged = true;
                Configuration.VoiceActing.Volume = newValue;
            }
            GUILayout.EndHorizontal();
        }
    }

}
