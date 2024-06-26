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
            public readonly IniValue<Boolean> StartFieldCreator;
            public readonly IniValue<Boolean> RenderWalkmeshes;

            public DebugSection() : base(nameof(DebugSection), false)
            {
                SigningEventObjects = BindBoolean(nameof(SigningEventObjects), false);
                StartModelViewer = BindBoolean(nameof(StartModelViewer), false);
                StartFieldCreator = BindBoolean(nameof(StartFieldCreator), false);
                RenderWalkmeshes = BindBoolean(nameof(RenderWalkmeshes), false);
            }
        }
    }
}