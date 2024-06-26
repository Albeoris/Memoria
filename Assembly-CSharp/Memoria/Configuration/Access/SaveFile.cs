using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class SaveFile
        {
            public static Boolean DisableAutoSave => Instance._saves.DisableAutoSave;
            public static Boolean AutoSaveOnlyAtMoogle => Instance._saves.AutoSaveOnlyAtMoogle;
            public static Boolean SaveOnCloud => Instance._saves.SaveOnCloud;
        }
    }
}