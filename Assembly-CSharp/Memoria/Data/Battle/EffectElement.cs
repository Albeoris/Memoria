using System;

namespace Memoria.Data
{
    [Flags]
    public enum EffectElement : byte
    {
        None = 0,
        Fire = 1,
        Cold = 2,
        Thunder = 4,
        Earth = 8,
        Aqua = 16,
        Wind = 32,
        Holy = 64,
        Darkness = 128
    }
}