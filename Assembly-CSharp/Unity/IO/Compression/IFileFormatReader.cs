using System;

namespace Unity.IO.Compression
{
    internal interface IFileFormatReader
    {
        Boolean ReadHeader(InputBuffer input);

        Boolean ReadFooter(InputBuffer input);

        void UpdateWithBytesRead(Byte[] buffer, Int32 offset, Int32 bytesToCopy);

        void Validate();
    }
}
