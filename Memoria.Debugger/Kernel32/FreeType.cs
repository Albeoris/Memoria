using System;

namespace Memoria.Debugger
{
    [Flags]
    public enum FreeType
    {
        Decommit = 0x4000,
        Release = 0x8000,
    }
}