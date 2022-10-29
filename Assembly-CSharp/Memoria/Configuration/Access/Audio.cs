using System;
using System.Collections.Generic;

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

            public static Int32 CharAttackAudioChance => Instance._audio.CharAttackAudioChance;
            public static Int32 CharHitAudioChance => Instance._audio.CharHitAudioChance;
            public static Int32 StartBattleChance => Instance._audio.StartBattleChance;
            public static Int32 EndBattleChance => Instance._audio.EndBattleChance;

            public static string[] preventmultiplay = Instance._audio.PreventMultiPlay;
            private static Dictionary<string, UInt16> tmp;
            public static Dictionary<string, UInt16> preventMultiPlay
            {
                get
                {
                    if (tmp == null) {
                        tmp = new Dictionary<string, UInt16>();
                        foreach (string filePath in preventmultiplay)
                        {
                            tmp.Add(filePath, 0);
                        }
                    }
                    return tmp;
                }
            }
        }
    }
}