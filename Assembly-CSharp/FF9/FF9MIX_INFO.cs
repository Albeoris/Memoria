using System;

namespace FF9
{
    public class FF9MIX_INFO
    {
        public const UInt16 FF9MIX_ITEM_MAX = 32;

        public Byte input;

        public Byte item_ct;

        public Byte mix_ct;

        public Byte mix_max;

        public Byte is_party;

        public Byte[] pad = new Byte[3];

        public FF9MIX_DATA[] item = new FF9MIX_DATA[32];
    }
}
