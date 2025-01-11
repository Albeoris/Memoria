namespace Compression;

internal class GZipDecoder
{
    private const int FileText = 1;
    private const int CRCFlag = 2;
    private const int ExtraFieldsFlag = 4;
    private const int FileNameFlag = 8;
    private const int CommentFlag = 16;

    private InputBuffer input;
    private GZIPHeaderState gzipHeaderSubstate;
    private GZIPHeaderState gzipFooterSubstate;
    private int gzip_header_flag;
    private int gzip_header_xlen;
    private uint gzipCrc32;
    private uint gzipOutputStreamSize;
    private int loopCounter;

    public uint Crc32 => gzipCrc32;
    public uint StreamSize => gzipOutputStreamSize;

    public GZipDecoder(InputBuffer input)
    {
        this.input = input;
        Reset();
    }

    public void Reset()
    {
        gzipHeaderSubstate = GZIPHeaderState.ReadingID1;
        gzipFooterSubstate = GZIPHeaderState.ReadingCRC;
        gzipCrc32 = 0u;
        gzipOutputStreamSize = 0u;
    }

    public bool ReadGzipHeader()
    {
        switch (gzipHeaderSubstate)
        {
            case GZIPHeaderState.ReadingID1:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                if (bits != 31)
                    throw new InvalidDataException("CorruptedGZipHeader");
                gzipHeaderSubstate = GZIPHeaderState.ReadingID2;
                goto case GZIPHeaderState.ReadingID2;
            }
            case GZIPHeaderState.ReadingID2:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                if (bits != 139)
                    throw new InvalidDataException("CorruptedGZipHeader");
                gzipHeaderSubstate = GZIPHeaderState.ReadingCM;
                goto case GZIPHeaderState.ReadingCM;
            }
            case GZIPHeaderState.ReadingCM:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                if (bits != 8)
                    throw new InvalidDataException("UnknownCompressionMode");
                gzipHeaderSubstate = GZIPHeaderState.ReadingFLG;
                goto case GZIPHeaderState.ReadingFLG;
            }
            case GZIPHeaderState.ReadingFLG:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzip_header_flag = bits;
                gzipHeaderSubstate = GZIPHeaderState.ReadingMMTime;
                loopCounter = 0;
                goto case GZIPHeaderState.ReadingMMTime;
            }
            case GZIPHeaderState.ReadingMMTime:
            {
                int bits = 0;
                while (loopCounter < 4)
                {
                    bits = input.GetBits(8);
                    if (bits < 0)
                        return false;
                    loopCounter++;
                }
                gzipHeaderSubstate = GZIPHeaderState.ReadingXFL;
                loopCounter = 0;
                goto case GZIPHeaderState.ReadingXFL;
            }
            case GZIPHeaderState.ReadingXFL:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzipHeaderSubstate = GZIPHeaderState.ReadingOS;
                goto case GZIPHeaderState.ReadingOS;
            }
            case GZIPHeaderState.ReadingOS:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzipHeaderSubstate = GZIPHeaderState.ReadingXLen1;
                goto case GZIPHeaderState.ReadingXLen1;
            }
            case GZIPHeaderState.ReadingXLen1:
                if (((uint)gzip_header_flag & GZipDecoder.ExtraFieldsFlag) != 0)
                {
                    int bits = input.GetBits(8);
                    if (bits < 0)
                        return false;
                    gzip_header_xlen = bits;
                    gzipHeaderSubstate = GZIPHeaderState.ReadingXLen2;
                    goto case GZIPHeaderState.ReadingXLen2;
                }
                goto case GZIPHeaderState.ReadingFileName;
            case GZIPHeaderState.ReadingXLen2:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzip_header_xlen |= bits << 8;
                gzipHeaderSubstate = GZIPHeaderState.ReadingXLenData;
                loopCounter = 0;
                goto case GZIPHeaderState.ReadingXLenData;
            }
            case GZIPHeaderState.ReadingXLenData:
            {
                int bits = 0;
                while (loopCounter < gzip_header_xlen)
                {
                    bits = input.GetBits(8);
                    if (bits < 0)
                        return false;
                    loopCounter++;
                }
                gzipHeaderSubstate = GZIPHeaderState.ReadingFileName;
                loopCounter = 0;
                goto case GZIPHeaderState.ReadingFileName;
            }
            case GZIPHeaderState.ReadingFileName:
                if ((gzip_header_flag & GZipDecoder.FileNameFlag) == 0)
                {
                    gzipHeaderSubstate = GZIPHeaderState.ReadingComment;
                }
                else
                {
                    int bits;
                    do
                    {
                        bits = input.GetBits(8);
                        if (bits < 0)
                            return false;
                    }
                    while (bits != 0);
                    gzipHeaderSubstate = GZIPHeaderState.ReadingComment;
                }
                goto case GZIPHeaderState.ReadingComment;
            case GZIPHeaderState.ReadingComment:
                if ((gzip_header_flag & GZipDecoder.CommentFlag) == 0)
                {
                    gzipHeaderSubstate = GZIPHeaderState.ReadingCRC16Part1;
                }
                else
                {
                    int bits;
                    do
                    {
                        bits = input.GetBits(8);
                        if (bits < 0)
                            return false;
                    }
                    while (bits != 0);
                    gzipHeaderSubstate = GZIPHeaderState.ReadingCRC16Part1;
                }
                goto case GZIPHeaderState.ReadingCRC16Part1;
            case GZIPHeaderState.ReadingCRC16Part1:
            {
                if ((gzip_header_flag & GZipDecoder.CRCFlag) == 0)
                {
                    gzipHeaderSubstate = GZIPHeaderState.Done;
                    goto case GZIPHeaderState.Done;
                }
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzipHeaderSubstate = GZIPHeaderState.ReadingCRC16Part2;
                goto case GZIPHeaderState.ReadingCRC16Part2;
            }
            case GZIPHeaderState.ReadingCRC16Part2:
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzipHeaderSubstate = GZIPHeaderState.Done;
                goto case GZIPHeaderState.Done;
            }
            case GZIPHeaderState.Done:
                return true;
            default:
                throw new InvalidDataException("UnknownState");
        }
    }

    public bool ReadGzipFooter()
    {
        input.SkipToByteBoundary();
        if (gzipFooterSubstate == GZIPHeaderState.ReadingCRC)
        {
            while (loopCounter < 4)
            {
                int bits = input.GetBits(8);
                if (bits < 0)
                    return false;
                gzipCrc32 |= (uint)(bits << 8 * loopCounter);
                loopCounter++;
            }
            gzipFooterSubstate = GZIPHeaderState.ReadingFileSize;
            loopCounter = 0;
        }
        if (gzipFooterSubstate == GZIPHeaderState.ReadingFileSize)
        {
            if (loopCounter == 0)
                gzipOutputStreamSize = 0u;
            while (loopCounter < 4)
            {
                int bits2 = input.GetBits(8);
                if (bits2 < 0)
                    return false;
                gzipOutputStreamSize |= (uint)(bits2 << 8 * loopCounter);
                loopCounter++;
            }
        }
        return true;
    }
}
