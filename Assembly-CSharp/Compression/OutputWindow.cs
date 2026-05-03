using System;

namespace Compression;

internal class OutputWindow
{
    private const int WindowSize = 32768;
    private const int WindowMask = 32767;

    private byte[] window = new byte[OutputWindow.WindowSize];
    private int end;
    private int bytesUsed;

    public int FreeBytes => OutputWindow.WindowSize - bytesUsed;
    public int AvailableBytes => bytesUsed;

    public void Write(byte b)
    {
        window[end++] = b;
        end &= OutputWindow.WindowMask;
        bytesUsed++;
    }

    public void WriteLengthDistance(int length, int distance)
    {
        bytesUsed += length;
        int num = (end - distance) & 0x7FFF;
        int num2 = OutputWindow.WindowSize - length;
        if (num <= num2 && end < num2)
        {
            if (length <= distance)
            {
                Array.Copy(window, num, window, end, length);
                end += length;
            }
            else
            {
                while (length-- > 0)
                    window[end++] = window[num++];
            }
        }
        else
        {
            while (length-- > 0)
            {
                window[end++] = window[num++];
                end &= OutputWindow.WindowMask;
                num &= 0x7FFF;
            }
        }
    }

    public int CopyFrom(InputBuffer input, int length)
    {
        length = Math.Min(Math.Min(length, OutputWindow.WindowSize - bytesUsed), input.AvailableBytes);
        int num = OutputWindow.WindowSize - end;
        int num2;
        if (length > num)
        {
            num2 = input.CopyTo(window, end, num);
            if (num2 == num)
                num2 += input.CopyTo(window, 0, length - num);
        }
        else
        {
            num2 = input.CopyTo(window, end, length);
        }
        end = (end + num2) & 0x7FFF;
        bytesUsed += num2;
        return num2;
    }

    public int CopyTo(byte[] output, int offset, int length)
    {
        int num;
        if (length > bytesUsed)
        {
            num = end;
            length = bytesUsed;
        }
        else
        {
            num = (end - bytesUsed + length) & 0x7FFF;
        }
        int num2 = length;
        int num3 = length - num;
        if (num3 > 0)
        {
            Array.Copy(window, OutputWindow.WindowSize - num3, output, offset, num3);
            offset += num3;
            length = num;
        }
        Array.Copy(window, num - length, output, offset, length);
        bytesUsed -= num2;
        return num2;
    }
}
