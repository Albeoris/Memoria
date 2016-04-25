using System.IO;

namespace Memoria
{
    internal static class StreamExm
    {
        public static void CopyTo(this Stream self, Stream destination, int bufferSize)
        {
            int count;
            byte[] buffer = new byte[bufferSize];
            while ((count = self.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, count);
        }
    }
}