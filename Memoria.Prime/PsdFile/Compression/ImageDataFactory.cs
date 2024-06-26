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
    public static class ImageDataFactory
    {
        /// <summary>
        /// Creates an ImageData object to compress or decompress image data.
        /// </summary>
        /// <param name="channel">The Channel associated with the image data.</param>
        /// <param name="data">The image data to be decompressed, or null if
        ///   image data is to be compressed.</param>
        public static ImageData Create(Channel channel, Byte[] data)
        {
            var bitDepth = channel.Layer.PsdFile.BitDepth;
            ImageData imageData;
            switch (channel.ImageCompression)
            {
                case ImageCompression.Raw:
                    imageData = new RawImage(data, channel.Rect.Size, bitDepth);
                    break;

                case ImageCompression.Rle:
                    imageData = new RleImage(data, channel.RleRowLengths,
                      channel.Rect.Size, bitDepth);
                    break;

                case ImageCompression.Zip:
                    // Photoshop treats 32-bit Zip as 32-bit ZipPrediction
                    imageData = (bitDepth == 32)
                      ? CreateZipPredict(data, channel.Rect.Size, bitDepth)
                      : new ZipImage(data, channel.Rect.Size, bitDepth);
                    break;

                case ImageCompression.ZipPrediction:
                    imageData = CreateZipPredict(data, channel.Rect.Size, bitDepth);
                    break;

                default:
                    throw new PsdInvalidException("Unknown image compression method.");
            }

            // Reverse endianness of multi-byte image data
            imageData = WrapEndianness(imageData);

            return imageData;
        }

        private static ImageData CreateZipPredict(Byte[] data, Size size,
          Int32 bitDepth)
        {
            switch (bitDepth)
            {
                case 16:
                    return new ZipPredict16Image(data, size);
                case 32:
                    return new ZipPredict32Image(data, size);
                default:
                    throw new PsdInvalidException(
                      "ZIP with prediction is only available for 16 and 32 bit depths.");
            }
        }

        private static ImageData WrapEndianness(ImageData imageData)
        {
            // Single-byte image does not require endianness reversal
            if (imageData.BitDepth <= 8)
            {
                return imageData;
            }

            // Bytes will be reordered by the compressor, so no wrapper is needed
            if ((imageData is ZipPredict16Image) || (imageData is ZipPredict32Image))
            {
                return imageData;
            }

            return new EndianReverser(imageData);
        }
    }
}
