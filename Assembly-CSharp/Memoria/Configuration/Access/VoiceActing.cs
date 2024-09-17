﻿using System;
using UnityEngine;

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
            public static Int32 ForceMessageSpeed = Mathf.Clamp(Instance._voiceActing.ForceMessageSpeed.Value, -1, 6);
            public static Int32 ForceLanguage = (Instance._voiceActing.ForceLanguage.Value > 6 || Instance._voiceActing.ForceLanguage.Value < 0) ? -1 : Instance._voiceActing.ForceLanguage.Value;
            public static Int32 Volume = Instance._voiceActing.Volume.Value;

            public static void SaveVolume()
            {
                Instance._voiceActing.Volume.Value = Volume;
                SaveValue(Instance._voiceActing.Name, Instance._voiceActing.Volume);
            }

            public static void SaveAutoText()
            {
                Instance._voiceActing.AutoDismissDialogAfterCompletion.Value = AutoDismissDialogAfterCompletion;
                SaveValue(Instance._voiceActing.Name, Instance._voiceActing.AutoDismissDialogAfterCompletion);
            }
        }
    }
}
