/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;

namespace Memoria.Prime.PsdFile
{
    /// <summary>
    /// Writes PSD data types in big-endian byte order.
    /// </summary>
    public class PsdBinaryWriter : IDisposable
    {
        private BinaryWriter _writer;
        private Encoding _encoding;

        internal Stream BaseStream
        {
            get
            {
                // Flush the writer so that the Stream.Position is correct.
                Flush();
                return _writer.BaseStream;
            }
        }

        public PsdBinaryWriter(Stream stream, Encoding encoding)
        {
            this._encoding = encoding;

            // Specifying ASCII encoding will help catch any accidental calls to
            // BinaryWriter.Write(String).  Since we do not own the Stream, the
            // constructor is called with leaveOpen = true.
            _writer = new BinaryWriter(stream, Encoding.ASCII);
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Write(Rectangle rect)
        {
            Write(rect.Top);
            Write(rect.Left);
            Write(rect.Bottom);
            Write(rect.Right);
        }

        /// <summary>
        /// Pad the length of a block to a multiple.
        /// </summary>
        /// <param name="startPosition">Starting position of the padded block.</param>
        /// <param name="padMultiple">Byte multiple to pad to.</param>
        public void WritePadding(Int64 startPosition, Int32 padMultiple)
        {
            var length = _writer.BaseStream.Position - startPosition;
            var padBytes = Util.GetPadding((Int32)length, padMultiple);
            for (Int64 i = 0; i < padBytes; i++)
            {
                _writer.Write((Byte)0);
            }
        }

        /// <summary>
        /// Write string as ASCII characters, without a length prefix.
        /// </summary>
        public void WriteAsciiChars(String s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            _writer.Write(bytes);
        }


        /// <summary>
        /// Writes a Pascal string using the specified encoding.
        /// </summary>
        /// <param name="s">Unicode string to convert to the encoding.</param>
        /// <param name="padMultiple">Byte multiple that the Pascal string is padded to.</param>
        /// <param name="maxBytes">Maximum number of bytes to write.</param>
        public void WritePascalString(String s, Int32 padMultiple, Byte maxBytes = 255)
        {
            var startPosition = _writer.BaseStream.Position;

            Byte[] bytesArray = _encoding.GetBytes(s);
            if (bytesArray.Length > maxBytes)
            {
                var tempArray = new Byte[maxBytes];
                Array.Copy(bytesArray, tempArray, maxBytes);
                bytesArray = tempArray;
            }

            _writer.Write((Byte)bytesArray.Length);
            _writer.Write(bytesArray);
            WritePadding(startPosition, padMultiple);
        }

        /// <summary>
        /// Write a Unicode string to the stream.
        /// </summary>
        public void WriteUnicodeString(String s)
        {
            Write(s.Length);
            var data = Encoding.BigEndianUnicode.GetBytes(s);
            Write(data);
        }

        public void Write(Boolean value)
        {
            _writer.Write(value);
        }

        public void Write(Byte[] value)
        {
            _writer.Write(value);
        }

        public void Write(Byte value)
        {
            _writer.Write(value);
        }

        public void Write(Int16 value)
        {
            unsafe
            {
                Util.SwapBytes2((Byte*)&value);
            }
            _writer.Write(value);
        }

        public void Write(Int32 value)
        {
            unsafe
            {
                Util.SwapBytes4((Byte*)&value);
            }
            _writer.Write(value);
        }

        public void Write(Int64 value)
        {
            unsafe
            {
                Util.SwapBytes((Byte*)&value, 8);
            }
            _writer.Write(value);
        }

        public void Write(UInt16 value)
        {
            unsafe
            {
                Util.SwapBytes2((Byte*)&value);
            }
            _writer.Write(value);
        }

        public void Write(UInt32 value)
        {
            unsafe
            {
                Util.SwapBytes4((Byte*)&value);
            }
            _writer.Write(value);
        }

        public void Write(UInt64 value)
        {
            unsafe
            {
                Util.SwapBytes((Byte*)&value, 8);
            }
            _writer.Write(value);
        }


        //////////////////////////////////////////////////////////////////

        #region IDisposable

        private Boolean _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            // Check to see if Dispose has already been called. 
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_writer != null)
                {
                    // BinaryWriter.Dispose() is protected, so we have to call Close.
                    // The BinaryWriter will be automatically flushed on close.
                    _writer.Flush();
                    _writer = null;
                }
            }

            _disposed = true;
        }

        #endregion

    }
}
