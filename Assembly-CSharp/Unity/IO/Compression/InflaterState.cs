using System;

namespace Unity.IO.Compression
{
    internal enum InflaterState
    {
        ReadingHeader,
        ReadingBFinal = 2,
        ReadingBType,
        ReadingNumLitCodes,
        ReadingNumDistCodes,
        ReadingNumCodeLengthCodes,
        ReadingCodeLengthCodes,
        ReadingTreeCodesBefore,
        ReadingTreeCodesAfter,
        DecodeTop,
        HaveInitialLength,
        HaveFullLength,
        HaveDistCode,
        UncompressedAligning = 15,
        UncompressedByte1,
        UncompressedByte2,
        UncompressedByte3,
        UncompressedByte4,
        DecodingUncompressed,
        StartReadingFooter,
        ReadingFooter,
        VerifyingFooter,
        Done
    }
}
