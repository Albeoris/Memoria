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
    }
}