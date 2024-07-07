using System;

namespace FF9
{
    public class ccommon
    {
        public static UInt32 max(UInt32 a, UInt32 b)
        {
            return (UInt32)((a <= b) ? b : a);
        }

        public static UInt32 min(UInt32 a, UInt32 b)
        {
            return (UInt32)((a >= b) ? b : a);
        }
    }
}
