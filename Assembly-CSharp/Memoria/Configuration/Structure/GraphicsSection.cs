using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class GraphicsSection : IniSection
        {
            public readonly IniValue<Int32> BattleFPS;
            public readonly IniValue<Int32> MovieFPS;
            public readonly IniValue<Int32> BattleSwirlFrames;
            public readonly IniValue<Boolean> WidescreenSupport;
            public readonly IniValue<Int32> SkipIntros;
            public readonly IniValue<Int32> GarnetHair;

            public GraphicsSection() : base(nameof(GraphicsSection), false)
            {
                BattleFPS = BindInt32(nameof(BattleFPS), 15);
                MovieFPS = BindInt32(nameof(MovieFPS), 15);
                BattleSwirlFrames = BindInt32(nameof(BattleSwirlFrames), 115);
                WidescreenSupport = BindBoolean(nameof(WidescreenSupport), true);
                SkipIntros = BindInt32(nameof(SkipIntros), 0);
                GarnetHair = BindInt32(nameof(GarnetHair), 0);
            }
        }
    }
}