using System;

namespace Unity.IO.Compression
{
    internal class GZipFormatter : IFileFormatWriter
    {
        internal GZipFormatter() : this(3)
        {
        }

        internal GZipFormatter(Int32 compressionLevel)
        {
            if (compressionLevel == 10)
            {
                this.headerBytes[8] = 2;
            }
        }

        public Byte[] GetHeader()
        {
            return this.headerBytes;
        }

        public void UpdateWithBytesRead(Byte[] buffer, Int32 offset, Int32 bytesToCopy)
        {
            this._crc32 = Crc32Helper.UpdateCrc32(this._crc32, buffer, offset, bytesToCopy);
            Int64 num = this._inputStreamSizeModulo + (Int64)((UInt64)bytesToCopy);
            if (num >= 4294967296L)
            {
                num %= 4294967296L;
            }
            this._inputStreamSizeModulo = num;
        }

        public Byte[] GetFooter()
        {
            Byte[] array = new Byte[8];
            this.WriteUInt32(array, this._crc32, 0);
            this.WriteUInt32(array, (UInt32)this._inputStreamSizeModulo, 4);
            return array;
        }

        internal void WriteUInt32(Byte[] b, UInt32 value, Int32 startIndex)
        {
            b[startIndex] = (Byte)value;
            b[startIndex + 1] = (Byte)(value >> 8);
            b[startIndex + 2] = (Byte)(value >> 16);
            b[startIndex + 3] = (Byte)(value >> 24);
        }

        private Byte[] headerBytes = new Byte[]
        {
            31,
            139,
            8,
            0,
            0,
            0,
            0,
            0,
            4,
            0
        };

        private UInt32 _crc32;

        private Int64 _inputStreamSizeModulo;
    }
}
