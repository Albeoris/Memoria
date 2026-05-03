/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Memoria.Prime.PsdFile
{
    public abstract class ImageData
    {
        public Int32 BitDepth { get; private set; }

        public Int32 BytesPerRow { get; private set; }

        public Size Size { get; private set; }

        protected abstract Boolean AltersWrittenData { get; }

        protected ImageData(Size size, Int32 bitDepth)
        {
            Size = size;
            BitDepth = bitDepth;
            BytesPerRow = Util.BytesPerRow(size, bitDepth);
        }

        /// <summary>
        /// Reads decompressed image data.
        /// </summary>
        public virtual Byte[] Read()
        {
            var imageLongLength = (Int64)BytesPerRow * Size.Height;
            Util.CheckByteArrayLength(imageLongLength);

            var buffer = new Byte[imageLongLength];
            Read(buffer);
            return buffer;
        }

        internal abstract void Read(Byte[] buffer);

        /// <summary>
        /// Reads compressed image data.
        /// </summary>
        public abstract Byte[] ReadCompressed();

        /// <summary>
        /// Writes rows of image data into compressed format.
        /// </summary>
        /// <param name="array">An array containing the data to be compressed.</param>
        public void Write(Byte[] array)
        {
            var imageLength = (Int64)BytesPerRow * Size.Height;
            if (array.Length != imageLength)
            {
                throw new ArgumentException(
                  "Array length is not equal to image length.",
                  nameof(array));
            }

            if (AltersWrittenData)
            {
                array = (Byte[])array.Clone();
            }

            WriteInternal(array);
        }

        internal abstract void WriteInternal(Byte[] array);
    }
}
