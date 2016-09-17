using System;

namespace Memoria
{
    [Flags]
    public enum CalcFlag : byte
    {
        HpAlteration = 1,
        HpRecovery = 2,
        Critical = 4,
        MpAlteration = 8,
        MpRecovery = 16
    }
}