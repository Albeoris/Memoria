/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Memoria.Prime.PsdFile
{
    /// <summary>
    /// Writes the actual length in front of the data block upon disposal.
    /// </summary>
    class PsdBlockLengthWriter : IDisposable
    {
        private Boolean _disposed = false;

        Int64 _lengthPosition;
        Int64 _startPosition;
        Boolean _hasLongLength;
        PsdBinaryWriter _writer;

        public PsdBlockLengthWriter(PsdBinaryWriter writer)
          : this(writer, false)
        {
        }

        public PsdBlockLengthWriter(PsdBinaryWriter writer, Boolean hasLongLength)
        {
            this._writer = writer;
            this._hasLongLength = hasLongLength;

            // Store position so that we can return to it when the length is known.
            _lengthPosition = writer.BaseStream.Position;

            // Write a sentinel value as a placeholder for the length.
            writer.Write((UInt32)0xFEEDFEED);
            if (hasLongLength)
            {
                writer.Write((UInt32)0xFEEDFEED);
            }

            // Store the start position of the data block so that we can calculate
            // its length when we're done writing.
            _startPosition = writer.BaseStream.Position;
        }

        public void Write()
        {
            var endPosition = _writer.BaseStream.Position;

            _writer.BaseStream.Position = _lengthPosition;
            Int64 length = endPosition - _startPosition;
            if (_hasLongLength)
            {
                _writer.Write(length);
            }
            else
            {
                _writer.Write((UInt32)length);
            }

            _writer.BaseStream.Position = endPosition;
        }

        public void Dispose()
        {
            if (!this._disposed)
            {
                Write();
                this._disposed = true;
            }
        }
    }

}
