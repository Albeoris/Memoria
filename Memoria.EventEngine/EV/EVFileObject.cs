using System;
// ReSharper disable UnassignedField.Global
#pragma warning disable 169

namespace Memoria.EventEngine.EV
{
    public struct EVFileObject
    {
        public UInt16 Offset;
        public UInt16 Size;
        public Byte VariableCount;
        public Byte Flags;
        private Int16 _padding;
    }
}