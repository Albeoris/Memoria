using System;

namespace Assets.Sources.Graphics.Movie
{
    internal class AssetStream
    {
        public static Boolean GetZipFileOffsetLength(String zipFilePath, String fileName, out Int64 offset, out Int64 length)
        {
            offset = 0L;
            length = 0L;
            return true;
        }
    }
}
