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
            public readonly IniValue<Boolean> StopVoiceWhenDialogDismissed;

            public readonly IniArray<String> PreventMultiPlay;

            public readonly IniValue<Int32> CharAttackAudioChance;
            public readonly IniValue<Int32> CharHitAudioChance;
            public readonly IniValue<Int32> StartBattleChance;
            public readonly IniValue<Int32> EndBattleChance;
            public readonly IniValue<Int32> CharDeathChance;
            public readonly IniValue<Int32> CharAutoLifeChance;
            public readonly IniValue<Int32> CharStatusRemovedChance;
            public readonly IniValue<Int32> CharStatusAfflictedChance;

            public AudioSection() : base(nameof(AudioSection), true)
            {
                SoundVolume = BindInt32(nameof(SoundVolume), 100);
                MusicVolume = BindInt32(nameof(MusicVolume), 100);
                LogVoiceActing = BindBoolean(nameof(LogVoiceActing), false);
                PriorityToOGG = BindBoolean(nameof(PriorityToOGG), false);
                PreventMultiPlay = BindStringArray(nameof(PreventMultiPlay), new String[0]);

                CharAttackAudioChance = BindInt32(nameof(CharAttackAudioChance), 1);
                CharHitAudioChance = BindInt32(nameof(CharHitAudioChance), 1);
                StartBattleChance = BindInt32(nameof(StartBattleChance), 1);
                CharDeathChance = BindInt32(nameof(CharDeathChance), 1);
                CharAutoLifeChance = BindInt32(nameof(CharAutoLifeChance), 1);
                CharStatusRemovedChance = BindInt32(nameof(CharStatusRemovedChance), 1);
                CharStatusAfflictedChance = BindInt32(nameof(CharStatusAfflictedChance), 1);

                StopVoiceWhenDialogDismissed = BindBoolean(nameof(StopVoiceWhenDialogDismissed), false);
            }
        }
    }
}