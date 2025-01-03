using System;
using System.IO;

/*
                  Copyright (C) 2015  hamish-milne

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

namespace Memoria.Assets
{
    // Base API written by hamish-milne (https://github.com/hamish-milne/FbxWriter), adjusted/completed a bit
    // Documentation: https://code.blender.org/2013/08/fbx-binary-file-format-specification/
    // and: https://archive.blender.org/wiki/2015/index.php/User:Mont29/Foundation/FBX_File_Structure/

    /// <summary>Static read and write methods</summary>
    // IO is an acronym
    public static class FbxIO
    {
        /// <summary>Reads a binary FBX file</summary>
        /// <param name="path"></param>
        /// <returns>The top level document node</returns>
        public static FbxDocument ReadBinary(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var reader = new FbxBinaryReader(stream);
                return reader.Read();
            }
        }

        /// <summary>Reads an ASCII FBX file</summary>
        /// <param name="path"></param>
        /// <returns>The top level document node</returns>
        public static FbxDocument ReadAscii(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var reader = new FbxAsciiReader(stream);
                return reader.Read();
            }
        }

        /// <summary>Reads a FBX file in ASCII or Binary format</summary>
        /// <param name="path"></param>
        /// <returns>The top level document node</returns>
        public static FbxDocument ReadFlexible(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if (FbxBinary.ReadHeader(stream))
                {
                    stream.Position = 0;
                    var reader = new FbxBinaryReader(stream);
                    return reader.Read();
                }
                else
                {
                    stream.Position = 0;
                    var reader = new FbxAsciiReader(stream);
                    return reader.Read();
                }
            }
        }

        /// <summary>Writes an FBX document in Binary format</summary>
        /// <param name="document">The top level document node</param>
        /// <param name="path"></param>
        public static void WriteBinary(FbxDocument document, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var writer = new FbxBinaryWriter(stream);
                writer.Write(document);
            }
        }

        /// <summary>Writes an FBX document in ASCII format</summary>
        /// <param name="document">The top level document node</param>
        /// <param name="path"></param>
        public static void WriteAscii(FbxDocument document, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var writer = new FbxAsciiWriter(stream);
                writer.Write(document);
            }
        }
    }
}
