using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class AudioSection : IniSection
        {
            public readonly IniValue<Int32> SoundVolume;
            public readonly IniValue<Int32> MusicVolume;
            public readonly IniValue<Boolean> LogVoiceActing;
            public readonly IniValue<Boolean> PriorityToOGG;

            public readonly IniArray<String> PreventMultiPlay;
            public readonly IniValue<Int32> CharAttackAudioChance;
            public readonly IniValue<Int32> CharHitAudioChance;
            public readonly IniValue<Int32> StartBattleChance;
            public readonly IniValue<Int32> EndBattleChance;

            public AudioSection() : base(nameof(AudioSection), true)
            {
                SoundVolume = BindInt32(nameof(SoundVolume), 100);
                MusicVolume = BindInt32(nameof(MusicVolume), 100);
                LogVoiceActing = BindBoolean(nameof(LogVoiceActing), false);
                PriorityToOGG = BindBoolean(nameof(PriorityToOGG), false);
                PreventMultiPlay = BindStringArray(nameof(PreventMultiPlay), new String[0]);
                CharAttackAudioChance = BindInt32(nameof(CharAttackAudioChance), 1);
                CharHitAudioChance = BindInt32(nameof(CharHitAudioChance), 1);
                StartBattleChance = BindInt32(nameof(CharHitAudioChance), 1);
                EndBattleChance = BindInt32(nameof(CharHitAudioChance), 1);
            }
        }
    }
}