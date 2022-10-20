using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Audio
        {
            public static Int32 SoundVolume
            {
                get => Instance._audio.SoundVolume;
                set => Instance._audio.SoundVolume.Value = value;
            }

            public static Int32 MusicVolume
            {
                get => Instance._audio.MusicVolume;
                set => Instance._audio.MusicVolume.Value = value;
            }

            public static Boolean LogVoiceActing => Instance._audio.LogVoiceActing;
            public static Boolean PriorityToOGG => Instance._audio.PriorityToOGG;

            public static void SaveSoundVolume()
            {
                SaveValue(Instance._audio.Name, Instance._audio.SoundVolume);
            }

            public static void SaveMusicVolume()
            {
                SaveValue(Instance._audio.Name, Instance._audio.MusicVolume);
            }
        }
    }
}