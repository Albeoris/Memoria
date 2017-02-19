using System;

namespace Memoria.Data
{
    [Flags]
    public enum PlayerCategory : byte
    {
        Male = 1,
        Female = 2,
        Gaia = 4,
        Terra = 8,
        Subpc = 16,
        Reserve1 = 32,
        Reserve2 = 64,
        Reserve3 = 128
    }
}