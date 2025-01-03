using System;

namespace Compression;

internal class FastEncoder
{
    internal class Output
    {
        private byte[] outputBuf;
        private int outputPos;
        private uint bitBuf;
        private int bitCount;

        private static byte[] distLookup;

        internal int BytesWritten => outputPos;
        internal int FreeBytes => outputBuf.Length - outputPos;

        static Output()
        {
            distLookup = new byte[512];
            GenerateSlotTables();
        }

        internal static void GenerateSlotTables()
        {
            int num = 0;
            int i;
            for (i = 0; i < 16; i++)
                for (int j = 0; j < 1 << (int)FastEncoderStatics.ExtraDistanceBits[i]; j++)
                    distLookup[num++] = (byte)i;
            num >>= 7;
            for (; i < 30; i++)
                for (int j = 0; j < 1 << FastEncoderStatics.ExtraDistanceBits[i] - 7; j++)
                    distLookup[256 + num++] = (byte)i;
        }

        internal void UpdateBuffer(byte[] output)
        {
            outputBuf = output;
            outputPos = 0;
        }

        internal bool SafeToWriteTo()
        {
            return outputBuf.Length - outputPos > 16;
        }

        internal void WritePreamble()
        {
            Array.Copy(FastEncoderStatics.FastEncoderTreeStructureData, 0, outputBuf, outputPos, FastEncoderStatics.FastEncoderTreeStructureData.Length);
            outputPos += FastEncoderStatics.FastEncoderTreeStructureData.Length;
            bitCount = 9;
            bitBuf = 34u;
        }

        internal void WriteMatch(int matchLen, int matchPos)
        {
            uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[254 + matchLen];
            int num2 = (int)(num & 0x1F);
            if (num2 <= 16)
            {
                WriteBits(num2, num >> 5);
            }
            else
            {
                WriteBits(16, (num >> 5) & 0xFFFFu);
                WriteBits(num2 - 16, num >> 21);
            }

            num = FastEncoderStatics.FastEncoderDistanceCodeInfo[GetSlot(matchPos)];
            WriteBits((int)(num & 0xF), num >> 8);
            int num3 = (int)((num >> 4) & 0xF);
            if (num3 != 0)
                WriteBits(num3, (uint)matchPos & FastEncoderStatics.BitMask[num3]);
        }

        internal void WriteGzipFooter(uint gzipCrc32, uint inputStreamSize)
        {
            outputBuf[outputPos++] = (byte)(gzipCrc32 & 0xFFu);
            outputBuf[outputPos++] = (byte)((gzipCrc32 >> 8) & 0xFFu);
            outputBuf[outputPos++] = (byte)((gzipCrc32 >> 16) & 0xFFu);
            outputBuf[outputPos++] = (byte)((gzipCrc32 >> 24) & 0xFFu);
            outputBuf[outputPos++] = (byte)(inputStreamSize & 0xFFu);
            outputBuf[outputPos++] = (byte)((inputStreamSize >> 8) & 0xFFu);
            outputBuf[outputPos++] = (byte)((inputStreamSize >> 16) & 0xFFu);
            outputBuf[outputPos++] = (byte)((inputStreamSize >> 24) & 0xFFu);
        }

        internal void WriteGzipHeader(int compression_level)
        {
            outputBuf[outputPos++] = 31;
            outputBuf[outputPos++] = 139;
            outputBuf[outputPos++] = 8;
            outputBuf[outputPos++] = 0;
            outputBuf[outputPos++] = 0;
            outputBuf[outputPos++] = 0;
            outputBuf[outputPos++] = 0;
            outputBuf[outputPos++] = 0;
            outputBuf[outputPos++] = (byte)(compression_level == 10 ? 2 : 4);
            outputBuf[outputPos++] = 0;
        }

        internal void WriteChar(byte b)
        {
            uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[b];
            WriteBits((int)(num & 0x1F), num >> 5);
        }

        internal void WriteBits(int n, uint bits)
        {
            bitBuf |= bits << bitCount;
            bitCount += n;
            if (bitCount >= 16)
            {
                outputBuf[outputPos++] = (byte)bitBuf;
                outputBuf[outputPos++] = (byte)(bitBuf >> 8);
                bitCount -= 16;
                bitBuf >>= 16;
            }
        }

        internal int GetSlot(int pos)
        {
            return distLookup[(pos < 256) ? pos : (256 + (pos >> 7))];
        }

        internal void FlushBits()
        {
            while (bitCount >= 8)
            {
                outputBuf[outputPos++] = (byte)bitBuf;
                bitCount -= 8;
                bitBuf >>= 8;
            }
            if (bitCount > 0)
            {
                outputBuf[outputPos++] = (byte)bitBuf;
                bitCount = 0;
            }
        }
    }

    private bool hasBlockHeader;
    private bool hasGzipHeader;
    private bool usingGzip;
    private uint gzipCrc32;
    private uint inputStreamSize;
    private FastEncoderWindow inputWindow;
    private DeflateInput inputBuffer;
    private Output output;
    private Match currentMatch;
    private bool needsEOB;

    public FastEncoder(bool doGZip)
    {
        usingGzip = doGZip;
        inputWindow = new FastEncoderWindow();
        inputBuffer = new DeflateInput();
        output = new Output();
        currentMatch = new Match();
    }

    public void SetInput(byte[] input, int startIndex, int count)
    {
        inputBuffer.Buffer = input;
        inputBuffer.Count = count;
        inputBuffer.StartIndex = startIndex;
    }

    public bool NeedsInput()
    {
        if (inputBuffer.Count == 0)
            return inputWindow.BytesAvailable == 0;
        return false;
    }

    public int GetCompressedOutput(byte[] outputBuffer)
    {
        output.UpdateBuffer(outputBuffer);
        if (usingGzip && !hasGzipHeader)
        {
            output.WriteGzipHeader(3);
            hasGzipHeader = true;
        }
        if (!hasBlockHeader)
        {
            hasBlockHeader = true;
            output.WritePreamble();
        }
        do
        {
            int num = (inputBuffer.Count < inputWindow.FreeWindowSpace) ? inputBuffer.Count : inputWindow.FreeWindowSpace;
            if (num > 0)
            {
                inputWindow.CopyBytes(inputBuffer.Buffer, inputBuffer.StartIndex, num);
                if (usingGzip)
                {
                    gzipCrc32 = DecodeHelper.UpdateCrc32(gzipCrc32, inputBuffer.Buffer, inputBuffer.StartIndex, num);
                    uint num2 = inputStreamSize + (uint)num;
                    if (num2 < inputStreamSize)
                        throw new InvalidDataException("StreamSizeOverflow");
                    inputStreamSize = num2;
                }
                inputBuffer.ConsumeBytes(num);
            }
            while (inputWindow.BytesAvailable > 0 && output.SafeToWriteTo())
            {
                inputWindow.GetNextSymbolOrMatch(currentMatch);
                if (currentMatch.State == MatchState.HasSymbol)
                {
                    output.WriteChar(currentMatch.Symbol);
                    continue;
                }
                if (currentMatch.State == MatchState.HasMatch)
                {
                    output.WriteMatch(currentMatch.Length, currentMatch.Position);
                    continue;
                }
                output.WriteChar(currentMatch.Symbol);
                output.WriteMatch(currentMatch.Length, currentMatch.Position);
            }
        }
        while (output.SafeToWriteTo() && !NeedsInput());
        needsEOB = true;
        return output.BytesWritten;
    }

    public int Finish(byte[] outputBuffer)
    {
        output.UpdateBuffer(outputBuffer);
        if (needsEOB)
        {
            uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[256];
            int n = (int)(num & 0x1F);
            output.WriteBits(n, num >> 5);
            output.FlushBits();
            if (usingGzip)
                output.WriteGzipFooter(gzipCrc32, inputStreamSize);
        }
        return output.BytesWritten;
    }
}
