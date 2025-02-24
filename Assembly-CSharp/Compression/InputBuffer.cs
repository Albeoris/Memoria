using System;

namespace Compression;

internal class InputBuffer
{
    private byte[] buffer;
    private int start;
    private int end;
    private uint bitBuffer;
    private int bitsInBuffer;

    public int AvailableBits => bitsInBuffer;
    public int AvailableBytes => end - start + bitsInBuffer / 8;

    public bool EnsureBitsAvailable(int count)
    {
        if (bitsInBuffer < count)
        {
            if (NeedsInput())
                return false;
            bitBuffer |= (uint)(buffer[start++] << bitsInBuffer);
            bitsInBuffer += 8;
            if (bitsInBuffer < count)
            {
                if (NeedsInput())
                    return false;
                bitBuffer |= (uint)(buffer[start++] << bitsInBuffer);
                bitsInBuffer += 8;
            }
        }
        return true;
    }

    public uint TryLoad16Bits()
    {
        if (bitsInBuffer < 8)
        {
            if (start < end)
            {
                bitBuffer |= (uint)(buffer[start++] << bitsInBuffer);
                bitsInBuffer += 8;
            }
            if (start < end)
            {
                bitBuffer |= (uint)(buffer[start++] << bitsInBuffer);
                bitsInBuffer += 8;
            }
        }
        else if (bitsInBuffer < 16 && start < end)
        {
            bitBuffer |= (uint)(buffer[start++] << bitsInBuffer);
            bitsInBuffer += 8;
        }
        return bitBuffer;
    }

    private uint GetBitMask(int count)
    {
        return (uint)((1 << count) - 1);
    }

    public int GetBits(int count)
    {
        if (!EnsureBitsAvailable(count))
            return -1;
        int result = (int)(bitBuffer & GetBitMask(count));
        bitBuffer >>= count;
        bitsInBuffer -= count;
        return result;
    }

    public int CopyTo(byte[] output, int offset, int length)
    {
        int num = 0;
        while (bitsInBuffer > 0 && length > 0)
        {
            output[offset++] = (byte)bitBuffer;
            bitBuffer >>= 8;
            bitsInBuffer -= 8;
            length--;
            num++;
        }
        if (length == 0)
            return num;
        int num2 = end - start;
        if (length > num2)
            length = num2;
        Array.Copy(buffer, start, output, offset, length);
        start += length;
        return num + length;
    }

    public bool NeedsInput()
    {
        return start == end;
    }

    public void SetInput(byte[] buffer, int offset, int length)
    {
        this.buffer = buffer;
        start = offset;
        end = offset + length;
    }

    public void SkipBits(int n)
    {
        bitBuffer >>= n;
        bitsInBuffer -= n;
    }

    public void SkipToByteBoundary()
    {
        bitBuffer >>= bitsInBuffer % 8;
        bitsInBuffer -= bitsInBuffer % 8;
    }
}
