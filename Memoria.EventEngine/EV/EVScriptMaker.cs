using System;
using System.IO;
using FF8.JSM;
using Memoria.Prime;

namespace Memoria.EventEngine.EV
{
    public sealed class EVScriptMaker
    {
        private readonly MemoryStream _ms;

        public Int32 Offset => checked((Int32) _ms.Position);
        public UInt16 Size => checked((UInt16) _ms.Length);

        private readonly Byte[] _buffer = new Byte[4];

        public EVScriptMaker(ArraySegment<Byte> segment)
        {
            _ms = new MemoryStream(segment.Array, segment.Offset, segment.Count);
        }

        public Boolean TryReadOpcode(out Jsm.Opcode opcode)
        {
            Int32 value = _ms.ReadByte();
            if (value < 0)
            {
                opcode = default;
                return false;
            }

            opcode = (Jsm.Opcode) value;
            return true;
        }
        
        public SByte ReadSByte() => (SByte) ReadByte();
        public Int16 ReadInt16() => Read<Int16>();
        public UInt16 ReadUInt16() => Read<UInt16>();
        public Int32 ReadInt24() => ReadByte() | ReadByte() << 8 | ReadByte() << 16;
        public Int32 ReadInt26() => ReadInt32() & 0x03FFFFFF;
        public Int32 ReadInt32() => Read<Int32>();

        public Byte ReadByte()
        {
            Int32 result = _ms.ReadByte();
            if (result < 0)
                throw new EndOfStreamException($"Cannot read {nameof(Byte)}. Unexpected end of stream.");

            return (Byte) result;
        }

        private unsafe T Read<T>() where T : unmanaged
        {
            _ms.EnsureRead(_buffer, 0, sizeof(T));
            fixed (Byte* b = _buffer)
                return *((T*) b);
        }
    }
}