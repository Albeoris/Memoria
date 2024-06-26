using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class FixesSection : IniSection
        {
            public readonly IniValue<Boolean> KeepRestTimeInBattle;

            public FixesSection() : base(nameof(FixesSection), true)
            {
                KeepRestTimeInBattle = BindBoolean(nameof(KeepRestTimeInBattle), true);
            }
        }
    }
}