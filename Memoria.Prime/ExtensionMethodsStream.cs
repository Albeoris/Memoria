using System;
using System.IO;

namespace Memoria.Prime
{
    public static class ExtensionMethodsStream
    {
        public static void CopyTo(this Stream self, Stream destination, Int64 bufferSize)
        {
            Int32 count;
            Byte[] buffer = new Byte[bufferSize];
            while ((count = self.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, count);
        }

        public static void EnsureRead(this Stream self, Byte[] buff, Int32 offset, Int32 size)
        {
            Int32 readed;
            while (size > 0 && (readed = self.Read(buff, offset, size)) != 0)
            {
                size -= readed;
                offset += readed;
            }

            if (size != 0)
                throw new EndOfStreamException("Unexpected end of stream.");
        }
    }
}