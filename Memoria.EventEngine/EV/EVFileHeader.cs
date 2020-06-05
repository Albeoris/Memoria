using System;

#pragma warning disable 169
#pragma warning disable 649

namespace Memoria.EventEngine.EV
{
    public struct EVFileHeader
    {
        private UInt16 _magicNumber; // EV
        private Byte _unknown;
        public Byte ObjectCount;
        private unsafe fixed Byte _padding[124];

        public void Check()
        {
            const Int16 expected = 0x5645; // EV

            if (_magicNumber != expected)
                throw new FormatException($"Unexpected magic number: {_magicNumber}. Expected: {expected}");
        }
    }
}