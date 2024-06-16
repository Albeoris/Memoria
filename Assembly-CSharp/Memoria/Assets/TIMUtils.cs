using System;

namespace Memoria.Assets
{
    public static class TIMUtils
    {
        public struct TPage
        {
            public Int32 FlagTP => this.value >> 7 & 3;
            public Int32 FlagABR => this.value >> 5 & 3;
            public Int32 FlagTY => this.value >> 4 & 1;
            public Int32 FlagTX => this.value & 0xF;

            public UInt16 value;
        }

        public struct Clut
        {
            public Int32 FlagClutY => this.value >> 6 & 0x1FF;
            public Int32 FlagClutX => this.value & 0x3F;

            public UInt16 value;
        }
    }
}
