using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class VoiceActing
        {
            public static Boolean Enabled => Instance._voiceActing.Enabled;

            public static Boolean LogVoiceActing => Instance._voiceActing.LogVoiceActing;
            public static Boolean StopVoiceWhenDialogDismissed = Instance._voiceActing.StopVoiceWhenDialogDismissed;
            public static Boolean AutoDismissDialogAfterCompletion = Instance._voiceActing.AutoDismissDialogAfterCompletion;
            public static Int32 Volume
            {
                get => Instance._voiceActing.Volume;
                set => Instance._voiceActing.Volume.Value = value;
            }

            public static void SaveVolume()
            {
                // We need to make sure VoiceActing is enabled otherwise the volume won't apply
                // This can happen if a mod enable the VoiceActing
                SaveValue(Instance._voiceActing.Name, Instance._voiceActing.Enabled);
                SaveValue(Instance._voiceActing.Name, Instance._voiceActing.Volume);
            }
        }
    }
}