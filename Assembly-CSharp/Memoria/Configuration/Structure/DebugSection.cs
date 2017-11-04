using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class DebugSection : IniSection
        {
            public readonly IniValue<Boolean> SigningEventObjects;

            public DebugSection() : base(nameof(DebugSection), false)
            {
                SigningEventObjects = BindBoolean(nameof(SigningEventObjects), false);
            }
        }
    }
}