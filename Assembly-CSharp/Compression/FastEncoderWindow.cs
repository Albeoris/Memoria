using System;

using System.Diagnostics;

namespace Compression;

internal class FastEncoderWindow
{
    private const int FastEncoderHashShift = 4;
    private const int FastEncoderHashtableSize = 2048;
    private const int FastEncoderHashMask = 2047;
    private const int FastEncoderWindowSize = 8192;
    private const int FastEncoderWindowMask = 8191;
    private const int FastEncoderMatch3DistThreshold = 16384;
    internal const int MaxMatch = 258;
    internal const int MinMatch = 3;
    private const int SearchDepth = 32;
    private const int GoodLength = 4;
    private const int NiceLength = 32;
    private const int LazyMatchThreshold = 6;

    private byte[] window;
    private int bufPos;
    private int bufEnd;
    private ushort[] prev;
    private ushort[] lookup;

    public int BytesAvailable => bufEnd - bufPos;
    public int FreeWindowSpace => FastEncoderWindow.FastEncoderMatch3DistThreshold - bufEnd;

    public FastEncoderWindow()
    {
        window = new byte[16646]; // FastEncoderMatch3DistThreshold + MaxMatch + GoodLength
        prev = new ushort[8450]; // FastEncoderWindowSize + MaxMatch
        lookup = new ushort[FastEncoderWindow.FastEncoderHashtableSize];
        bufPos = FastEncoderWindow.FastEncoderWindowSize;
        bufEnd = bufPos;
    }

    public void CopyBytes(byte[] inputBuffer, int startIndex, int count)
    {
        Array.Copy(inputBuffer, startIndex, window, bufEnd, count);
        bufEnd += count;
    }

    public void MoveWindows()
    {
        Array.Copy(window, bufPos - FastEncoderWindow.FastEncoderWindowSize, window, 0, FastEncoderWindow.FastEncoderWindowSize);
        for (int i = 0; i < FastEncoderWindow.FastEncoderHashtableSize; i++)
        {
            int num = lookup[i] - FastEncoderWindow.FastEncoderWindowSize;
            lookup[i] = (ushort)(num <= 0 ? 0 : num);
        }
        for (int i = 0; i < FastEncoderWindow.FastEncoderWindowSize; i++)
        {
            long num = prev[i] - FastEncoderWindow.FastEncoderWindowSize;
            prev[i] = (ushort)(num <= 0 ? 0 : num);
        }
        bufPos = FastEncoderWindow.FastEncoderWindowSize;
        bufEnd = bufPos;
    }

    private uint HashValue(uint hash, byte b)
    {
        return (hash << FastEncoderWindow.FastEncoderHashShift) ^ b;
    }

    private uint InsertString(ref uint hash)
    {
        hash = HashValue(hash, window[bufPos + 2]);
        uint num = lookup[hash & 0x7FF];
        lookup[hash & 0x7FF] = (ushort)bufPos;
        prev[bufPos & 0x1FFF] = (ushort)num;
        return num;
    }

    private void InsertStrings(ref uint hash, int matchLen)
    {
        if (bufEnd - bufPos <= matchLen)
        {
            bufPos += matchLen - 1;
            return;
        }
        while (--matchLen > 0)
        {
            InsertString(ref hash);
            bufPos++;
        }
    }

    internal bool GetNextSymbolOrMatch(Match match)
    {
        uint hash = HashValue(0u, window[bufPos]);
        hash = HashValue(hash, window[bufPos + 1]);
        int matchPos = 0;
        int num;
        if (bufEnd - bufPos <= FastEncoderWindow.MinMatch)
        {
            num = 0;
        }
        else
        {
            int num2 = (int)InsertString(ref hash);
            if (num2 != 0)
            {
                num = FindMatch(num2, out matchPos, FastEncoderWindow.SearchDepth, FastEncoderWindow.NiceLength);
                if (bufPos + num > bufEnd)
                    num = bufEnd - bufPos;
            }
            else
            {
                num = 0;
            }
        }
        if (num < FastEncoderWindow.MinMatch)
        {
            match.State = MatchState.HasSymbol;
            match.Symbol = window[bufPos];
            bufPos++;
        }
        else
        {
            bufPos++;
            if (num <= FastEncoderWindow.LazyMatchThreshold)
            {
                int matchPos2 = 0;
                int num3 = (int)InsertString(ref hash);
                int num4;
                if (num3 != 0)
                {
                    num4 = FindMatch(num3, out matchPos2, num < FastEncoderWindow.GoodLength ? FastEncoderWindow.SearchDepth : 8, FastEncoderWindow.NiceLength);
                    if (bufPos + num4 > bufEnd)
                        num4 = bufEnd - bufPos;
                }
                else
                {
                    num4 = 0;
                }
                if (num4 > num)
                {
                    match.State = MatchState.HasSymbolAndMatch;
                    match.Symbol = window[bufPos - 1];
                    match.Position = matchPos2;
                    match.Length = num4;
                    bufPos++;
                    num = num4;
                    InsertStrings(ref hash, num);
                }
                else
                {
                    match.State = MatchState.HasMatch;
                    match.Position = matchPos;
                    match.Length = num;
                    num--;
                    bufPos++;
                    InsertStrings(ref hash, num);
                }
            }
            else
            {
                match.State = MatchState.HasMatch;
                match.Position = matchPos;
                match.Length = num;
                InsertStrings(ref hash, num);
            }
        }
        if (bufPos == FastEncoderWindow.FastEncoderMatch3DistThreshold)
            MoveWindows();
        return true;
    }

    private int FindMatch(int search, out int matchPos, int searchDepth, int niceLength)
    {
        int num = 0;
        int num2 = 0;
        int num3 = bufPos - FastEncoderWindow.FastEncoderWindowSize;
        byte b = window[bufPos];
        while (search > num3)
        {
            if (window[search + num] == b)
            {
                int i = 0;
                while (i < FastEncoderWindow.MaxMatch && window[bufPos + i] == window[search + i])
                    i++;
                if (i > num)
                {
                    num = i;
                    num2 = search;
                    if (i > niceLength)
                        break;
                    b = window[bufPos + i];
                }
            }
            if (--searchDepth == 0)
                break;
            search = prev[search & 0x1FFF];
        }
        matchPos = bufPos - num2 - 1;
        if (num == FastEncoderWindow.MinMatch && matchPos >= FastEncoderWindow.FastEncoderMatch3DistThreshold)
            return 0;
        return num;
    }

    [Conditional("DEBUG")]
    private void VerifyHashes()
    {
        for (int i = 0; i < FastEncoderWindow.FastEncoderHashtableSize; i++)
        {
            ushort num = lookup[i];
            while (num != 0 && bufPos - num < FastEncoderWindow.FastEncoderWindowSize)
            {
                ushort num2 = prev[num & 0x1FFF];
                if (bufPos - num2 >= FastEncoderWindow.FastEncoderWindowSize)
                    break;
                num = num2;
            }
        }
    }

    private uint RecalculateHash(int position)
    {
        return (uint)((window[position] << 8) ^ (window[position + 1] << FastEncoderWindow.FastEncoderHashShift) ^ window[position + 2]) & 0x7FFu;
    }
}
