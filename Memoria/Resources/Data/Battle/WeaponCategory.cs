using System;

namespace Memoria
{
    [Flags]
    public enum WeaponCategory : byte
    {
        ShortRange = 1,
        LongRange = 2,
        Throw = 4,
        OfsDim = 8,
        Open2 = 16,
        Open3 = 32,
        Open4 = 64,
        Default = 128
    }
}