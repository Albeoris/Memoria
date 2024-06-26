using System;

namespace Unity.IO.Compression
{
    internal class DeflateInput
    {
        internal Byte[] Buffer
        {
            get
            {
                return this.buffer;
            }
            set
            {
                this.buffer = value;
            }
        }

        internal Int32 Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
            }
        }

        internal Int32 StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                this.startIndex = value;
            }
        }

        internal void ConsumeBytes(Int32 n)
        {
            this.startIndex += n;
            this.count -= n;
        }

        internal DeflateInput.InputState DumpState()
        {
            DeflateInput.InputState result;
            result.count = this.count;
            result.startIndex = this.startIndex;
            return result;
        }

        internal void RestoreState(DeflateInput.InputState state)
        {
            this.count = state.count;
            this.startIndex = state.startIndex;
        }

        private Byte[] buffer;

        private Int32 count;

        private Int32 startIndex;

        internal struct InputState
        {
            internal Int32 count;

            internal Int32 startIndex;
        }
    }
}
