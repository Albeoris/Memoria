using System;
using Memoria.Assets;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Lang
        {
            /// <summary>Modes: (0) no dual language / (1) switch by pressing a key / (2) both texts displayed</summary>
            public static Int32 DualLanguageMode => String.IsNullOrEmpty(DualLanguage) || DualLanguage == Localization.CurrentSymbol ? 0 : Instance._lang.DualLanguageMode;
            public static String DualLanguage => Instance._lang.DualLanguage.Value.Trim().Trim('"');
        }
    }
}
