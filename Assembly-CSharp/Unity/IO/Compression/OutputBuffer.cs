using System;

namespace Unity.IO.Compression
{
    internal class OutputBuffer
    {
        internal void UpdateBuffer(Byte[] output)
        {
            this.byteBuffer = output;
            this.pos = 0;
        }

        internal Int32 BytesWritten
        {
            get
            {
                return this.pos;
            }
        }

        internal Int32 FreeBytes
        {
            get
            {
                return (Int32)this.byteBuffer.Length - this.pos;
            }
        }

        internal void WriteUInt16(UInt16 value)
        {
            this.byteBuffer[this.pos++] = (Byte)value;
            this.byteBuffer[this.pos++] = (Byte)(value >> 8);
        }

        internal void WriteBits(Int32 n, UInt32 bits)
        {
            this.bitBuf |= bits << this.bitCount;
            this.bitCount += n;
            if (this.bitCount >= 16)
            {
                this.byteBuffer[this.pos++] = (Byte)this.bitBuf;
                this.byteBuffer[this.pos++] = (Byte)(this.bitBuf >> 8);
                this.bitCount -= 16;
                this.bitBuf >>= 16;
            }
        }

        internal void FlushBits()
        {
            while (this.bitCount >= 8)
            {
                this.byteBuffer[this.pos++] = (Byte)this.bitBuf;
                this.bitCount -= 8;
                this.bitBuf >>= 8;
            }
            if (this.bitCount > 0)
            {
                this.byteBuffer[this.pos++] = (Byte)this.bitBuf;
                this.bitBuf = 0u;
                this.bitCount = 0;
            }
        }

        internal void WriteBytes(Byte[] byteArray, Int32 offset, Int32 count)
        {
            if (this.bitCount == 0)
            {
                Array.Copy(byteArray, offset, this.byteBuffer, this.pos, count);
                this.pos += count;
            }
            else
            {
                this.WriteBytesUnaligned(byteArray, offset, count);
            }
        }

        private void WriteBytesUnaligned(Byte[] byteArray, Int32 offset, Int32 count)
        {
            for (Int32 i = 0; i < count; i++)
            {
                Byte b = byteArray[offset + i];
                this.WriteByteUnaligned(b);
            }
        }

        private void WriteByteUnaligned(Byte b)
        {
            this.WriteBits(8, (UInt32)b);
        }

        internal Int32 BitsInBuffer
        {
            get
            {
                return this.bitCount / 8 + 1;
            }
        }

        internal OutputBuffer.BufferState DumpState()
        {
            OutputBuffer.BufferState result;
            result.pos = this.pos;
            result.bitBuf = this.bitBuf;
            result.bitCount = this.bitCount;
            return result;
        }

        internal void RestoreState(OutputBuffer.BufferState state)
        {
            this.pos = state.pos;
            this.bitBuf = state.bitBuf;
            this.bitCount = state.bitCount;
        }

        private Byte[] byteBuffer;

        private Int32 pos;

        private UInt32 bitBuf;

        private Int32 bitCount;

        internal struct BufferState
        {
            internal Int32 pos;

            internal UInt32 bitBuf;

            internal Int32 bitCount;
        }
    }
}
