//using System;
//using System.IO;
//using System.Text;
//using JetBrains.Annotations;

//namespace Memoria.Prime
//{
//    public sealed class CrossPlatformBinaryWriter : BinaryWriter
//    {
//        private readonly Encoding _encoding;
//        private Byte[] _buff;

//        public CrossPlatformBinaryWriter([NotNull] Stream input)
//            : this(input, Encoding.UTF8)
//        {
//        }

//        public CrossPlatformBinaryWriter([NotNull] Stream input, [NotNull] Encoding encoding)
//            : base(input, encoding)
//        {
//            _encoding = encoding;
//        }

//        public override void Write(String value)
//        {
//            if (ReferenceEquals(value, null))
//                throw new ArgumentNullException(nameof(value));

//            if (value.Length == 0)
//            {
//                Write((UInt16)0);
//                return;
//            }

//            unsafe
//            {
//                fixed (Char* strPtr = value)
//                {
//                    Int32 binarySize = _encoding.GetByteCount(strPtr, value.Length);
//                    if (_buff == null || _buff.Length < binarySize)
//                        _buff = new Byte[binarySize];

//                    fixed (Byte* buffPtr = _buff)
//                    {
//                        if (_encoding.GetBytes(strPtr, value.Length, buffPtr, binarySize) != binarySize)
//                            throw new InvalidOperationException();
//                    }

//                    Write(checked((UInt16)binarySize));
//                    Write(_buff, 0, binarySize);
//                }
//            }
//        }
//    }
//}