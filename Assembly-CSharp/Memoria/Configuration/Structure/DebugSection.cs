using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class DebugSection : IniSection
        {
            public readonly IniValue<Boolean> SigningEventObjects;
            public readonly IniValue<Boolean> StartModelViewer;

            public DebugSection() : base(nameof(DebugSection), false)
            {
                SigningEventObjects = BindBoolean(nameof(SigningEventObjects), false);
                StartModelViewer = BindBoolean(nameof(StartModelViewer), false);
            }
        }
    }
}