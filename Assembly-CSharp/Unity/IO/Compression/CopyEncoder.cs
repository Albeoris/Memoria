using System;

namespace Unity.IO.Compression
{
    internal class CopyEncoder
    {
        public void GetBlock(DeflateInput input, OutputBuffer output, Boolean isFinal)
        {
            Int32 num = 0;
            if (input != null)
            {
                num = Math.Min(input.Count, output.FreeBytes - 5 - output.BitsInBuffer);
                if (num > 65531)
                {
                    num = 65531;
                }
            }
            if (isFinal)
            {
                output.WriteBits(3, 1u);
            }
            else
            {
                output.WriteBits(3, 0u);
            }
            output.FlushBits();
            this.WriteLenNLen((UInt16)num, output);
            if (input != null && num > 0)
            {
                output.WriteBytes(input.Buffer, input.StartIndex, num);
                input.ConsumeBytes(num);
            }
        }

        private void WriteLenNLen(UInt16 len, OutputBuffer output)
        {
            output.WriteUInt16(len);
            UInt16 value = (UInt16)(~len);
            output.WriteUInt16(value);
        }

        private const Int32 PaddingSize = 5;

        private const Int32 MaxUncompressedBlockSize = 65536;
    }
}
