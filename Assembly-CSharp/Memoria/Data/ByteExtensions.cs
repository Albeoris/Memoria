using Memoria.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Memoria
{
    public static class ByteExtensions
    {
        public static bool HasFlag(this byte value, byte flag)
        {
            return (value & flag) != 0;
        }

        public static bool HasFlag(this byte value, ItemType flag)
        {
            return HasFlag(value, (byte)flag);
        }
    }
}
