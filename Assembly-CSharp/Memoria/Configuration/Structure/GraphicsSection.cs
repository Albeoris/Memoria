using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class GraphicsSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> BattleFPS = IniValue.Int32(nameof(BattleFPS));
            public readonly IniValue<Int32> MovieFPS = IniValue.Int32(nameof(MovieFPS));
            public readonly IniValue<Int32> BattleSwirlFrames = IniValue.Int32(nameof(BattleSwirlFrames));
            public readonly IniValue<Boolean> WidescreenSupport = IniValue.Boolean(nameof(WidescreenSupport));
            public readonly IniValue<Int32> SkipIntros = IniValue.Int32(nameof(SkipIntros));
            public readonly IniValue<Int32> GarnetHair = IniValue.Int32(nameof(GarnetHair));

            public GraphicsSection() : base("Graphics")
            {
                Enabled.Value = false;
                BattleFPS.Value = 30;
                MovieFPS.Value = 15;
                BattleSwirlFrames.Value = 25;
                WidescreenSupport.Value = true;
                SkipIntros.Value = 0;
                GarnetHair.Value = 0;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return BattleFPS;
                yield return MovieFPS;
                yield return BattleSwirlFrames;
                yield return WidescreenSupport;
                yield return SkipIntros;
                yield return GarnetHair;
            }
        }
    }
}