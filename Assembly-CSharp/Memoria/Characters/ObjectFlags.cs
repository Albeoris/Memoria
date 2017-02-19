using System;

namespace Memoria
{
    [Flags]
    public enum ObjectFlags : byte
    {
        Visible = 1,
        ExpectVisible = 254
    }
}