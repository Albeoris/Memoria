using System;

namespace Memoria.Prime.AKB2
{
    public struct AKB2Header
    {
        public UInt32 Signature;
        public UInt32 Constant01;
        public UInt32 FileSize;
        public UInt32 Constant02;

        public UInt32 Constant03;
        public UInt32 Constant04;
        public UInt32 Zero05;
        public UInt32 Zero06;

        public UInt32 Constant07;
        public UInt32 Constant08;
        public UInt32 Unknown09;
        public UInt32 Constant10;

        public UInt32 Constant11;
        public UInt32 Constant12;
        public UInt32 Zero13;
        public UInt32 Constant14;

        public UInt32 Zero15;
        public UInt32 Zero16;
        public UInt32 Zero17;
        public UInt32 Zero18;

        public UInt32 Constant19;
        public UInt32 Constant20;
        public UInt32 Zero21;
        public UInt32 Zero22;

        public UInt32 Constant23;
        public UInt32 Zero24;
        public UInt32 Zero25;
        public UInt32 Zero26;

        public UInt32 Zero27;
        public UInt32 Zero28;
        public UInt32 Zero29;

        public AKB2UnknownSection Section1;
        public AKB2UnknownSection Section2;
        public AKB2UnknownSection Section3;
        public AKB2UnknownSection Section4;

        public UInt32 Zero31;
        public UInt16 Constant32;
        public UInt16 Unknown33;
        public UInt16 Constant34;
        public UInt16 SampleRate;
        public UInt32 ContentSize;
        public UInt32 SampleCount; // Ignored?
        public UInt32 LoopStart; // Samples
        public UInt32 LoopEnd; // Samples
        public UInt16 Constant36;
        public UInt32 Optional36; // Can be zero
        public UInt32 Optional37; // Can be zero
        public UInt32 Zero38;
        public UInt32 Constant39;
        public UInt32 Constant40;
        public UInt32 Constant41;
        public UInt32 Constant42;
        public UInt32 Zero43;
        public UInt32 Zero44;
        public UInt32 Zero45;
        public UInt32 Optional46; // Loop?
        public UInt32 Optional47; // Can be zero
        public UInt32 Zero48;

        public static unsafe void Initialize(AKB2Header* header)
        {
            header->Signature = 0x32424b41;
            header->Constant01 = 0x00100000;
            header->Constant02 = 0x00000001;

            header->Constant03 = 0x00100000;
            header->Constant04 = 0x00000020;

            header->Constant07 = 0x00300101;
            header->Constant08 = 0x3F800000;
            header->Constant10 = 0x0180007C;

            header->Constant11 = 0x000000FF;
            header->Constant12 = 0x00000001;
            header->Constant14 = 0x41F00000;

            header->Constant19 = 0x00100000;
            header->Constant20 = 0x000000C0;

            header->Constant23 = 0x00000002;

            AKB2UnknownSection.Initialize(&header->Section1);
            AKB2UnknownSection.Initialize(&header->Section2);
            AKB2UnknownSection.Initialize(&header->Section3);
            AKB2UnknownSection.Initialize(&header->Section4);

            header->Constant32 = 0x0500;
            header->Unknown33 = 0x0002;
            header->Constant34 = 0x0040;
            header->SampleRate = 44100;
            header->Constant36 = 0x00000010;
            header->Constant39 = 0x3F800000;
            header->Constant40 = 0x3F800000;
            header->Constant41 = 0x3F800000;
            header->Constant42 = 0x3F800000;
        }
    }
}