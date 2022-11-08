using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class VoiceActingSection : IniSection
        {
            public readonly IniValue<Boolean> LogVoiceActing;
            public readonly IniValue<Boolean> StopVoiceWhenDialogDismissed;
            public readonly IniValue<Boolean> AutoDismissDialogAfterCompletion;

            public VoiceActingSection() : base(nameof(VoiceActingSection), false)
            {
                LogVoiceActing = BindBoolean(nameof(LogVoiceActing), false);
                StopVoiceWhenDialogDismissed = BindBoolean(nameof(StopVoiceWhenDialogDismissed), false);
                AutoDismissDialogAfterCompletion = BindBoolean(nameof(AutoDismissDialogAfterCompletion), false);
            }
        }
    }
}