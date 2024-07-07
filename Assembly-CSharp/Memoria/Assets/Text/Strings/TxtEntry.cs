using System;

namespace Memoria.Assets
{
    public sealed class TxtEntry
    {
        public Int32 Index = -1;
        public String Prefix = String.Empty;
        public String Value = String.Empty;

        public Int32 TryUpdateIndex(Int32 index)
        {
            if (Index < 0)
                Index = index;
            return Index;
        }
    }
}
