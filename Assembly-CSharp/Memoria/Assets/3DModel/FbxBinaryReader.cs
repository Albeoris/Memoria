using System;
using System.Text;
using System.IO;
using InvalidDataException = Compression.InvalidDataException;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace Memoria.Assets
{
    /// <summary>Reads FBX nodes from a binary stream</summary>
    public class FbxBinaryReader : FbxBinary
    {
        private readonly BinaryReader stream;
        private readonly FbxErrorLevel errorLevel;

        private delegate object ReadPrimitive(BinaryReader reader);

        /// <summary>Creates a new reader</summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="errorLevel">When to throw an <see cref="FbxException"/></param>
        /// <exception cref="ArgumentException"><paramref name="stream"/> does not support seeking</exception>
        public FbxBinaryReader(Stream stream, FbxErrorLevel errorLevel = FbxErrorLevel.Checked)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek)
                throw new ArgumentException(
                    "The stream must support seeking. Try reading the data into a buffer first");
            this.stream = new BinaryReader(stream, Encoding.ASCII);
            this.errorLevel = errorLevel;
        }

        // Reads a single property
        object ReadProperty()
        {
            var dataType = (char)stream.ReadByte();
            switch (dataType)
            {
                case 'Y':
                    return stream.ReadInt16();
                case 'C':
                    return (char)stream.ReadByte();
                case 'I':
                    return stream.ReadInt32();
                case 'F':
                    return stream.ReadSingle();
                case 'D':
                    return stream.ReadDouble();
                case 'L':
                    return stream.ReadInt64();
                case 'f':
                    return ReadArray(br => br.ReadSingle(), typeof(float));
                case 'd':
                    return ReadArray(br => br.ReadDouble(), typeof(double));
                case 'l':
                    return ReadArray(br => br.ReadInt64(), typeof(long));
                case 'i':
                    return ReadArray(br => br.ReadInt32(), typeof(int));
                case 'b':
                    return ReadArray(br => br.ReadBoolean(), typeof(bool));
                case 'S':
                    var len = stream.ReadInt32();
                    var str = len == 0 ? "" : Encoding.ASCII.GetString(stream.ReadBytes(len));
                    // Convert \0\1 to '::' and reverse the tokens
                    if (str.Contains(binarySeparator))
                    {
                        var tokens = str.Split(new[] { binarySeparator }, StringSplitOptions.None);
                        var sb = new StringBuilder();
                        bool first = true;
                        for (int i = tokens.Length - 1; i >= 0; i--)
                        {
                            if (!first)
                                sb.Append(asciiSeparator);
                            sb.Append(tokens[i]);
                            first = false;
                        }
                        str = sb.ToString();
                    }
                    return str;
                case 'R':
                    return stream.ReadBytes(stream.ReadInt32());
                default:
                    throw new FbxException(stream.BaseStream.Position - 1,
                        "Invalid property data type `" + dataType + "'");
            }
        }

        // Reads an array, decompressing it if required
        Array ReadArray(ReadPrimitive readPrimitive, Type arrayType)
        {
            var len = stream.ReadInt32();
            var encoding = stream.ReadInt32();
            var compressedLen = stream.ReadInt32();
            var ret = Array.CreateInstance(arrayType, len);
            var s = stream;
            var endPos = stream.BaseStream.Position + compressedLen;
            FbxDeflateWithChecksum codec = null;
            if (encoding != 0)
            {
                if (errorLevel >= FbxErrorLevel.Checked)
                {
                    if (encoding != 1)
                        throw new FbxException(stream.BaseStream.Position - 1,
                            "Invalid compression encoding (must be 0 or 1)");
                    var cmf = stream.ReadByte();
                    if ((cmf & 0xF) != 8 || (cmf >> 4) > 7)
                        throw new FbxException(stream.BaseStream.Position - 1,
                            "Invalid compression format " + cmf);
                    var flg = stream.ReadByte();
                    if (errorLevel >= FbxErrorLevel.Strict && ((cmf << 8) + flg) % 31 != 0)
                        throw new FbxException(stream.BaseStream.Position - 1,
                            "Invalid compression FCHECK");
                    if ((flg & (1 << 5)) != 0)
                        throw new FbxException(stream.BaseStream.Position - 1,
                            "Invalid compression flags; dictionary not supported");
                }
                else
                {
                    stream.BaseStream.Position += 2;
                }
                codec = new FbxDeflateWithChecksum(stream.BaseStream, CompressionMode.Decompress);
                s = new BinaryReader(codec);
            }
            try
            {
                for (int i = 0; i < len; i++)
                    ret.SetValue(readPrimitive(s), i);
            }
            catch (InvalidDataException)
            {
                throw new FbxException(stream.BaseStream.Position - 1, "Compressed data was malformed");
            }
            if (encoding != 0)
            {
                if (errorLevel >= FbxErrorLevel.Checked)
                {
                    stream.BaseStream.Position = endPos - sizeof(int);
                    var checksumBytes = new byte[sizeof(int)];
                    stream.BaseStream.Read(checksumBytes, 0, checksumBytes.Length);
                    int checksum = 0;
                    for (int i = 0; i < checksumBytes.Length; i++)
                        checksum = (checksum << 8) + checksumBytes[i];
                    if (checksum != codec.Checksum)
                        throw new FbxException(stream.BaseStream.Position,
                            "Compressed data has invalid checksum");
                }
                else
                {
                    stream.BaseStream.Position = endPos;
                }
            }
            return ret;
        }

        /// <summary>Reads a single node.</summary>
        /// <remarks>This won't read the file header or footer, and as such will fail if the stream is a full FBX file</remarks>
        /// <returns>The node</returns>
        /// <exception cref="FbxException">The FBX data was malformed for the reader's error level</exception>
        public FbxNode ReadNode(FbxDocument document)
        {
            var endOffset = document.Version >= FbxVersion.v7_5 ? stream.ReadInt64() : stream.ReadInt32();
            var numProperties = document.Version >= FbxVersion.v7_5 ? stream.ReadInt64() : stream.ReadInt32();
            var propertyListLen = document.Version >= FbxVersion.v7_5 ? stream.ReadInt64() : stream.ReadInt32();
            var nameLen = stream.ReadByte();
            var name = nameLen == 0 ? "" : Encoding.ASCII.GetString(stream.ReadBytes(nameLen));

            if (endOffset == 0)
            {
                // The end offset should only be 0 in a null node
                if (errorLevel >= FbxErrorLevel.Checked && (numProperties != 0 || propertyListLen != 0 || !string.IsNullOrEmpty(name)))
                    throw new FbxException(stream.BaseStream.Position,
                        "Invalid node; expected NULL record");
                return null;
            }

            var node = new FbxNode { Name = name };

            var propertyEnd = stream.BaseStream.Position + propertyListLen;
            // Read properties
            for (int i = 0; i < numProperties; i++)
                node.Properties.Add(ReadProperty());

            if (errorLevel >= FbxErrorLevel.Checked && stream.BaseStream.Position != propertyEnd)
                throw new FbxException(stream.BaseStream.Position,
                    "Too many bytes in property list, end point is " + propertyEnd);

            // Read nested nodes
            var listLen = endOffset - stream.BaseStream.Position;
            if (errorLevel >= FbxErrorLevel.Checked && listLen < 0)
                throw new FbxException(stream.BaseStream.Position,
                    "Node has invalid end point");
            if (listLen > 0)
            {
                FbxNode nested;
                do
                {
                    nested = ReadNode(document);
                    node.Nodes.Add(nested);
                } while (nested != null);
                if (errorLevel >= FbxErrorLevel.Checked && stream.BaseStream.Position != endOffset)
                    throw new FbxException(stream.BaseStream.Position,
                        "Too many bytes in node, end point is " + endOffset);
            }
            return node;
        }

        /// <summary>Reads an FBX document from the stream</summary>
        /// <returns>The top-level node</returns>
        /// <exception cref="FbxException">The FBX data was malformed for the reader's error level</exception>
        public FbxDocument Read()
        {
            // Read header
            bool validHeader = ReadHeader(stream.BaseStream);
            if (errorLevel >= FbxErrorLevel.Strict && !validHeader)
                throw new FbxException(stream.BaseStream.Position,
                    "Invalid header string");
            var document = new FbxDocument { Version = (FbxVersion)stream.ReadInt32() };

            // Read nodes
            var dataPos = stream.BaseStream.Position;
            FbxNode nested;
            do
            {
                nested = ReadNode(document);
                if (nested != null)
                    document.Nodes.Add(nested);
            } while (nested != null);

            // Read footer code
            var footerCode = new byte[footerCodeSize];
            stream.BaseStream.Read(footerCode, 0, footerCode.Length);
            if (errorLevel >= FbxErrorLevel.Strict)
            {
                var validCode = GenerateFooterCode(document);
                if (!CheckEqual(footerCode, validCode))
                    throw new FbxException(stream.BaseStream.Position - footerCodeSize,
                        "Incorrect footer code");
            }

            // Read footer extension
            dataPos = stream.BaseStream.Position;
            var validFooterExtension = CheckFooter(stream, document.Version);
            if (errorLevel >= FbxErrorLevel.Strict && !validFooterExtension)
                throw new FbxException(dataPos, "Invalid footer");
            return document;
        }
    }
}
