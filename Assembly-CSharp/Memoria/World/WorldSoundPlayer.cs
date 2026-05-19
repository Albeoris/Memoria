using Memoria.Prime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria
{
    /// <summary>
    /// Handles loading and playing custom sound effects for world map features (rain, effects, etc.).
    /// Sounds are loaded through the standard sound import pipeline and loop automatically.
    /// Modders place .ogg files in their Sounds/ folder and reference them via the [Sound=path]
    /// parameter in Environment.txt (e.g. [Sound=SE/rain_loop]).
    /// The .ogg file can include LoopStart/LoopEnd vorbis comments for seamless looping.
    /// </summary>
    public class WorldSoundPlayer
    {
        private SoundProfile _soundProfile;
        private Boolean _isPlaying;
        private Boolean _isLoaded;
        private Boolean _isLoading;
        private String _currentPath;

        public Boolean IsPlaying => _isPlaying;
        public String CurrentPath => _currentPath;

        public void Load(String resourcePath)
        {
            if (_isLoading || (_isLoaded && String.Equals(_currentPath, resourcePath)))
                return;

            // If we had a different sound loaded, unload it first
            if (_isLoaded)
                Unload();

            _currentPath = resourcePath;
            _isLoading = true;

            Int32 soundIndex = resourcePath.GetHashCode();
            _soundProfile = new SoundProfile
            {
                Code = soundIndex.ToString(),
                Name = resourcePath,
                SoundIndex = soundIndex,
                ResourceID = resourcePath,
                SoundProfileType = SoundProfileType.SoundEffect,
                SoundVolume = 1f,
                Panning = 0f,
                Pitch = 1f
            };

            SoundLoaderProxy.Instance.Load(_soundProfile, OnLoadComplete, null);
        }

        private void OnLoadComplete(SoundProfile profile, SoundDatabase db)
        {
            _isLoading = false;
            if (profile == null || profile.BankID == 0)
            {
                Log.Error($"[WorldSoundPlayer] Failed to load sound: {_currentPath}");
                _soundProfile = null;
                return;
            }
            _isLoaded = true;
        }

        public void Play(Single volume)
        {
            if (!_isLoaded || _soundProfile == null)
                return;

            if (!_isPlaying)
            {
                SoundPlayer.StaticCreateSound(_soundProfile);
                if (_soundProfile.SoundID <= 0)
                    return;

                ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(_soundProfile.SoundID, volume * SoundLib.SoundEffectPlayer.Volume, 0);
                ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(_soundProfile.SoundID, 0);
                _isPlaying = true;
            }
            else
            {
                // Update volume
                if (_soundProfile.SoundID > 0 && ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(_soundProfile.SoundID) != 0)
                {
                    ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(_soundProfile.SoundID, volume * SoundLib.SoundEffectPlayer.Volume, 0);
                }
                else
                {
                    // Sound ended or was lost, restart it
                    _isPlaying = false;
                    _soundProfile.SoundID = 0;
                    Play(volume);
                }
            }
        }

        public void Stop()
        {
            if (!_isPlaying || _soundProfile == null)
                return;

            if (_soundProfile.SoundID > 0)
            {
                ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(_soundProfile.SoundID, 0);
                _soundProfile.SoundID = 0;
            }
            _isPlaying = false;
        }

        public void Unload()
        {
            Stop();
            if (_soundProfile != null && _isLoaded)
            {
                SoundPlayer.StaticUnregisterBank(_soundProfile);
                _soundProfile = null;
            }
            _isLoaded = false;
            _currentPath = null;
        }
    }

    /// <summary>
    /// Manages multiple WorldSoundPlayers keyed by a string identifier (effect name, rain entry, etc.).
    /// Handles proximity-based volume and automatic start/stop.
    /// </summary>
    public class WorldSoundService
    {
        private Dictionary<String, WorldSoundPlayer> _players = new Dictionary<String, WorldSoundPlayer>();

        /// <summary>
        /// Updates a sound: loads if needed, plays at the given volume, or stops if volume is 0.
        /// </summary>
        /// <param name="key">Unique key for this sound instance (e.g. effect name or "rain")</param>
        /// <param name="soundPath">Resource path for the sound (e.g. "SE/rain_loop")</param>
        /// <param name="volume">Volume from 0 to 1. If 0, the sound is stopped.</param>
        public void Update(String key, String soundPath, Single volume)
        {
            if (String.IsNullOrEmpty(soundPath))
                return;

            WorldSoundPlayer player;
            if (!_players.TryGetValue(key, out player))
            {
                player = new WorldSoundPlayer();
                _players[key] = player;
            }

            // If the path changed, unload the old one
            if (player.CurrentPath != null && !String.Equals(player.CurrentPath, soundPath))
                player.Unload();

            player.Load(soundPath);

            if (volume > 0f)
                player.Play(volume);
            else
                player.Stop();
        }

        /// <summary>
        /// Stops a sound by key.
        /// </summary>
        public void Stop(String key)
        {
            WorldSoundPlayer player;
            if (_players.TryGetValue(key, out player))
                player.Stop();
        }

        /// <summary>
        /// Stops all managed sounds.
        /// </summary>
        public void StopAll()
        {
            foreach (var kvp in _players)
                kvp.Value.Stop();
        }

        /// <summary>
        /// Unloads all managed sounds and clears the cache.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var kvp in _players)
                kvp.Value.Unload();
            _players.Clear();
        }
    }
}
