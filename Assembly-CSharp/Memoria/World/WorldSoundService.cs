using System;
using System.Collections.Generic;

namespace Memoria
{
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
