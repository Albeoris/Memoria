using System;
using System.IO;
using FF8.JSM;

namespace Memoria.EventEngine.EV
{
    public sealed class EVScriptMaker
    {
        private readonly MemoryStream _ms;

        public Int32 Offset => checked((Int32) _ms.Position);
        public UInt16 Size => checked((UInt16) _ms.Length);

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
    }
}