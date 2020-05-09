using System;
using Assets.Sources.Scripts.Common;
using UnityEngine;

namespace Memoria
{
    public class MemoriaConfigurationMenu : MonoBehaviour
    {
        private Single _timer;
        private Boolean _soundVolumeChanged;
        private Boolean _musicVolumeChanged;
		
        private void OnEnable()
        {
            _soundVolumeChanged = false;
            _musicVolumeChanged = false;
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
        }

        private void OnGUI()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            DebugGuiSkin.ApplySkin();
            GUILayout.BeginArea(fullscreenRect);
            {
                GUILayout.BeginVertical("Box");
                {
                    BuildSoundSlider();
                    BuildMusicSlider();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
		
        private void BuildSoundSlider()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sound Volume");

            Int32 oldValue = Configuration.Audio.SoundVolume;
            Int32 newValue = (Int32)GUILayout.HorizontalSlider(oldValue, 0, 100);
            if (oldValue != newValue)
            {
                _soundVolumeChanged = true;
                Configuration.Audio.SoundVolume = newValue;
                SoundLib.TryUpdateSoundVolume();
            }
            GUILayout.EndHorizontal();
        }
		
        private void BuildMusicSlider()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Music Volume");

            Int32 oldValue = Configuration.Audio.MusicVolume;
            Int32 newValue = (Int32)GUILayout.HorizontalSlider(oldValue, 0, 100);
            if (oldValue != newValue)
            {
                _musicVolumeChanged = true;
                Configuration.Audio.MusicVolume = newValue;
                SoundLib.TryUpdateMusicVolume();
            }
            GUILayout.EndHorizontal();
        }
    }

}