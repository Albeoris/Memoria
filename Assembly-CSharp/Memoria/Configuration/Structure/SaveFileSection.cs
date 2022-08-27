using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class SaveFileSection : IniSection
        {
            public readonly IniValue<Boolean> DisableAutoSave;
            public readonly IniValue<Boolean> AutoSaveOnlyAtMoogle;
            public readonly IniValue<Boolean> SaveOnCloud;

            public SaveFileSection() : base(nameof(SaveFileSection), true)
            {
                DisableAutoSave = BindBoolean(nameof(DisableAutoSave), false);
                AutoSaveOnlyAtMoogle = BindBoolean(nameof(AutoSaveOnlyAtMoogle), false);
                SaveOnCloud = BindBoolean(nameof(SaveOnCloud), false);
            }
        }
    }
}