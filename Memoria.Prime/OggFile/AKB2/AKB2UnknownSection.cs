using System;

namespace Memoria.Prime.AKB2
{
    public struct AKB2UnknownSection
    {
        public UInt32 Constant01; //0x0
        public UInt32 Zero02;     //0x4
        public UInt32 Zero03;     //0x8
        public UInt32 Zero04;     //0xc
        public UInt32 Zero05;     //0x10
        public UInt32 Zero06;     //0x14
                                  //0x18  

        public static unsafe void Initialize(AKB2UnknownSection* section)
        {
            section->Constant01 = 0x00004040;
        }
    }
}