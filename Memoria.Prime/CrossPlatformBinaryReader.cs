//using System;
//using System.IO;
//using System.Text;
//using JetBrains.Annotations;

//namespace Memoria.Prime
//{
//    public sealed class CrossPlatformBinaryReader : BinaryReader
//    {
//        private readonly Encoding _encoding;
//        private Byte[] _buff;

//        public CrossPlatformBinaryReader([NotNull] Stream input)
//            : this(input, Encoding.UTF8)
//        {
//        }

//        public CrossPlatformBinaryReader([NotNull] Stream input, [NotNull] Encoding encoding)
//            : base(input, encoding)
//        {
//            _encoding = encoding;
//        }

//        public override String ReadString()
//        {
//            UInt16 binarySize = this.ReadUInt16();
//            if (binarySize == 0)
//                return String.Empty;

//            if (_buff == null || _buff.Length < binarySize)
//                _buff = new Byte[binarySize];

//            BaseStream.EnsureRead(_buff, 0, binarySize);
//            return _encoding.GetString(_buff, 0, binarySize);
//        }
//    }
//}
