using Memoria.Prime.Ini;
using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class LangSection : IniSection
        {
            public readonly IniValue<Int32> DualLanguageMode;
            public readonly IniValue<String> DualLanguage;
            public readonly IniValue<String> KeyDualLanguage;

            public LangSection() : base(nameof(LangSection), false)
            {
                DualLanguageMode = BindInt32(nameof(DualLanguageMode), 0);
                DualLanguage = BindString(nameof(DualLanguage), String.Empty);
                KeyDualLanguage = BindString(nameof(KeyDualLanguage), "CapsLock");
            }
        }
    }
}
