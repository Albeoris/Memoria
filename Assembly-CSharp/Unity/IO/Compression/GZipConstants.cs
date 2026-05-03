using System;

namespace Unity.IO.Compression
{
    internal static class GZipConstants
    {
        internal const Int32 CompressionLevel_3 = 3;

        internal const Int32 CompressionLevel_10 = 10;

        internal const Int64 FileLengthModulo = 4294967296L;

        internal const Byte ID1 = 31;

        internal const Byte ID2 = 139;

        internal const Byte Deflate = 8;

        internal const Int32 Xfl_HeaderPos = 8;

        internal const Byte Xfl_FastestAlgorithm = 4;

        internal const Byte Xfl_MaxCompressionSlowestAlgorithm = 2;
    }
}
