using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class DebugSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Boolean> SigningEventObjects = IniValue.Boolean(nameof(SigningEventObjects));

            public DebugSection() : base("Debug")
            {
                Enabled.Value = false;
                SigningEventObjects.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return SigningEventObjects;
            }
        }
    }
}