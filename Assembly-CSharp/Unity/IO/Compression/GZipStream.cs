using System;
using System.IO;

namespace Unity.IO.Compression
{
    public class GZipStream : Stream
    {
        public GZipStream(Stream stream, CompressionMode mode) : this(stream, mode, false)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, Boolean leaveOpen)
        {
            this.deflateStream = new DeflateStream(stream, mode, leaveOpen);
            this.SetDeflateStreamFileFormatter(mode);
        }

        private void SetDeflateStreamFileFormatter(CompressionMode mode)
        {
            if (mode == CompressionMode.Compress)
            {
                IFileFormatWriter fileFormatWriter = new GZipFormatter();
                this.deflateStream.SetFileFormatWriter(fileFormatWriter);
            }
            else
            {
                IFileFormatReader fileFormatReader = new GZipDecoder();
                this.deflateStream.SetFileFormatReader(fileFormatReader);
            }
        }

        public override Boolean CanRead
        {
            get
            {
                return this.deflateStream != null && this.deflateStream.CanRead;
            }
        }

        public override Boolean CanWrite
        {
            get
            {
                return this.deflateStream != null && this.deflateStream.CanWrite;
            }
        }

        public override Boolean CanSeek
        {
            get
            {
                return this.deflateStream != null && this.deflateStream.CanSeek;
            }
        }

        public override Int64 Length
        {
            get
            {
                throw new NotSupportedException(SR.GetString("Not supported"));
            }
        }

        public override Int64 Position
        {
            get
            {
                throw new NotSupportedException(SR.GetString("Not supported"));
            }
            set
            {
                throw new NotSupportedException(SR.GetString("Not supported"));
            }
        }

        public override void Flush()
        {
            if (this.deflateStream == null)
            {
                throw new ObjectDisposedException((String)null, SR.GetString("Object disposed"));
            }
            this.deflateStream.Flush();
        }

        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.GetString("Not supported"));
        }

        public override void SetLength(Int64 value)
        {
            throw new NotSupportedException(SR.GetString("Not supported"));
        }

        public override IAsyncResult BeginRead(Byte[] array, Int32 offset, Int32 count, AsyncCallback asyncCallback, Object asyncState)
        {
            if (this.deflateStream == null)
            {
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            }
            return this.deflateStream.BeginRead(array, offset, count, asyncCallback, asyncState);
        }

        public override Int32 EndRead(IAsyncResult asyncResult)
        {
            if (this.deflateStream == null)
            {
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            }
            return this.deflateStream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(Byte[] array, Int32 offset, Int32 count, AsyncCallback asyncCallback, Object asyncState)
        {
            if (this.deflateStream == null)
            {
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            }
            return this.deflateStream.BeginWrite(array, offset, count, asyncCallback, asyncState);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (this.deflateStream == null)
            {
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            }
            this.deflateStream.EndWrite(asyncResult);
        }

        public override Int32 Read(Byte[] array, Int32 offset, Int32 count)
        {
            if (this.deflateStream == null)
            {
                throw new ObjectDisposedException((String)null, SR.GetString("Object disposed"));
            }
            return this.deflateStream.Read(array, offset, count);
        }

        public override void Write(Byte[] array, Int32 offset, Int32 count)
        {
            if (this.deflateStream == null)
            {
                throw new ObjectDisposedException((String)null, SR.GetString("Object disposed"));
            }
            this.deflateStream.Write(array, offset, count);
        }

        protected override void Dispose(Boolean disposing)
        {
            try
            {
                if (disposing && this.deflateStream != null)
                {
                    this.deflateStream.Dispose();
                }
                this.deflateStream = (DeflateStream)null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public Stream BaseStream
        {
            get
            {
                if (this.deflateStream != null)
                {
                    return this.deflateStream.BaseStream;
                }
                return (Stream)null;
            }
        }

        private DeflateStream deflateStream;
    }
}
