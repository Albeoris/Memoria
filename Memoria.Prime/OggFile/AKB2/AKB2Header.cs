using System;
using System.IO;

namespace Memoria.Prime.AKB2
{
    public struct AKB2Header
    {
        public UInt32 Signature;    //0x0
        public UInt32 Constant01;   //0x4
        public UInt32 FileSize;     //0x8
        public UInt32 Constant02;   //0xc


        public UInt32 Constant03;   //0x10
        public UInt32 Constant04;   //0x14
        public UInt32 Zero05;       //0x18
        public UInt32 Zero06;       //0x1c

        public UInt32 Constant07;   //0x20
        public UInt32 Constant08;   //0x24
        public UInt32 Unknown09;    //0x28 // looks like a sound ID
        public UInt32 Constant10;   //0x2c

        public UInt32 Constant11;   //0x30
        public UInt32 Constant12;   //0x34
        public UInt32 Zero13;       //0x38
        public UInt32 Constant14;   //0x3c

        public double LengthMS;     //0x40
        public UInt32 Zero17;       //0x48
        public UInt32 Zero18;       //0x4c

        public UInt32 Constant19;   //0x50
        public UInt32 Constant20;   //0x54
        public UInt32 Zero21;       //0x58
        public UInt32 Zero22;       //0x5c

        public UInt32 Constant23;   //0x60
        public UInt32 Zero24;       //0x64
        public UInt32 Zero25;       //0x68
        public UInt32 Zero26;       //0x6c

        public UInt32 Zero27;        //0x70
        public UInt32 Zero28;        //0x74
        public UInt32 Zero29;        //0x78

        public AKB2UnknownSection Section1;   //0x7c
        public AKB2UnknownSection Section2;   //0x94
        public AKB2UnknownSection Section3;   //0xAC
        public AKB2UnknownSection Section4;   //0xC4

        public UInt32 Zero31;        //0xDC


        public UInt16 Constant32;    //0xE0
        public UInt16 Unknown33;     //0xE2
        public UInt16 Constant34;    //0xE4
        public UInt16 SampleRate;    //0xE6
        public UInt32 ContentSize;   //0xE8
        public UInt32 SampleCount;   //0xEC   // Ignored?

        public UInt32 LoopStart;     //0xF0   // Samples
        public UInt32 LoopEnd; // Samples
        public UInt16 Constant36;
        public UInt32 LoopStartAlternate; // Samples; used when there are several loop regions (Final Battle)
        public UInt32 LoopEndAlternate; // Samples; used when there are several loop regions (Final Battle)
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

        public UInt32 LengthInSeconds { get
            {
                return SampleCount / SampleRate;
            } 
        }

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

        public void ReadFromBytes(Byte[] raw)
        {
            if (raw.Length < 304)
                return;
            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(raw)))
            {
                this.Signature = binaryReader.ReadUInt32();
                this.Constant01 = binaryReader.ReadUInt32();
                this.FileSize = binaryReader.ReadUInt32();
                this.Constant02 = binaryReader.ReadUInt32();
                this.Constant03 = binaryReader.ReadUInt32();
                this.Constant04 = binaryReader.ReadUInt32();
                this.Zero05 = binaryReader.ReadUInt32();
                this.Zero06 = binaryReader.ReadUInt32();
                this.Constant07 = binaryReader.ReadUInt32();
                this.Constant08 = binaryReader.ReadUInt32();
                this.Unknown09 = binaryReader.ReadUInt32();
                this.Constant10 = binaryReader.ReadUInt32();
                this.Constant11 = binaryReader.ReadUInt32();
                this.Constant12 = binaryReader.ReadUInt32();
                this.Zero13 = binaryReader.ReadUInt32();
                this.Constant14 = binaryReader.ReadUInt32();
                this.Zero15 = binaryReader.ReadUInt32();
                this.Zero16 = binaryReader.ReadUInt32();
                this.Zero17 = binaryReader.ReadUInt32();
                this.Zero18 = binaryReader.ReadUInt32();
                this.Constant19 = binaryReader.ReadUInt32();
                this.Constant20 = binaryReader.ReadUInt32();
                this.Zero21 = binaryReader.ReadUInt32();
                this.Zero22 = binaryReader.ReadUInt32();
                this.Constant23 = binaryReader.ReadUInt32();
                this.Zero24 = binaryReader.ReadUInt32();
                this.Zero25 = binaryReader.ReadUInt32();
                this.Zero26 = binaryReader.ReadUInt32();
                this.Zero27 = binaryReader.ReadUInt32();
                this.Zero28 = binaryReader.ReadUInt32();
                this.Zero29 = binaryReader.ReadUInt32();
                this.Section1.Constant01 = binaryReader.ReadUInt32();
                this.Section1.Zero02 = binaryReader.ReadUInt32();
                this.Section1.Zero03 = binaryReader.ReadUInt32();
                this.Section1.Zero04 = binaryReader.ReadUInt32();
                this.Section1.Zero05 = binaryReader.ReadUInt32();
                this.Section1.Zero06 = binaryReader.ReadUInt32();
                this.Section2.Constant01 = binaryReader.ReadUInt32();
                this.Section2.Zero02 = binaryReader.ReadUInt32();
                this.Section2.Zero03 = binaryReader.ReadUInt32();
                this.Section2.Zero04 = binaryReader.ReadUInt32();
                this.Section2.Zero05 = binaryReader.ReadUInt32();
                this.Section2.Zero06 = binaryReader.ReadUInt32();
                this.Section3.Constant01 = binaryReader.ReadUInt32();
                this.Section3.Zero02 = binaryReader.ReadUInt32();
                this.Section3.Zero03 = binaryReader.ReadUInt32();
                this.Section3.Zero04 = binaryReader.ReadUInt32();
                this.Section3.Zero05 = binaryReader.ReadUInt32();
                this.Section3.Zero06 = binaryReader.ReadUInt32();
                this.Section4.Constant01 = binaryReader.ReadUInt32();
                this.Section4.Zero02 = binaryReader.ReadUInt32();
                this.Section4.Zero03 = binaryReader.ReadUInt32();
                this.Section4.Zero04 = binaryReader.ReadUInt32();
                this.Section4.Zero05 = binaryReader.ReadUInt32();
                this.Section4.Zero06 = binaryReader.ReadUInt32();
                this.Zero31 = binaryReader.ReadUInt32();
                this.Constant32 = binaryReader.ReadUInt16();
                this.Unknown33 = binaryReader.ReadUInt16();
                this.Constant34 = binaryReader.ReadUInt16();
                this.SampleRate = binaryReader.ReadUInt16();
                this.ContentSize = binaryReader.ReadUInt32();
                this.SampleCount = binaryReader.ReadUInt32();
                this.LoopStart = binaryReader.ReadUInt32();
                this.LoopEnd = binaryReader.ReadUInt32();
                this.Constant36 = binaryReader.ReadUInt16();
                this.LoopStartAlternate = binaryReader.ReadUInt32();
                this.LoopEndAlternate = binaryReader.ReadUInt32();
                this.Zero38 = binaryReader.ReadUInt32();
                this.Constant39 = binaryReader.ReadUInt32();
                this.Constant40 = binaryReader.ReadUInt32();
                this.Constant41 = binaryReader.ReadUInt32();
                this.Constant42 = binaryReader.ReadUInt32();
                this.Zero43 = binaryReader.ReadUInt32();
                this.Zero44 = binaryReader.ReadUInt32();
                this.Zero45 = binaryReader.ReadUInt32();
                this.Optional46 = binaryReader.ReadUInt32();
                this.Optional47 = binaryReader.ReadUInt32();
                this.Zero48 = binaryReader.ReadUInt32();
            }
        }

        public Byte[] WriteToBytes()
        {
            Byte[] result = new Byte[304];
            using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(result)))
            {
                binaryWriter.Write(this.Signature);
                binaryWriter.Write(this.Constant01);
                binaryWriter.Write(this.FileSize);
                binaryWriter.Write(this.Constant02);
                binaryWriter.Write(this.Constant03);
                binaryWriter.Write(this.Constant04);
                binaryWriter.Write(this.Zero05);
                binaryWriter.Write(this.Zero06);
                binaryWriter.Write(this.Constant07);
                binaryWriter.Write(this.Constant08);
                binaryWriter.Write(this.Unknown09);
                binaryWriter.Write(this.Constant10);
                binaryWriter.Write(this.Constant11);
                binaryWriter.Write(this.Constant12);
                binaryWriter.Write(this.Zero13);
                binaryWriter.Write(this.Constant14);
                binaryWriter.Write(this.Zero15);
                binaryWriter.Write(this.Zero16);
                binaryWriter.Write(this.Zero17);
                binaryWriter.Write(this.Zero18);
                binaryWriter.Write(this.Constant19);
                binaryWriter.Write(this.Constant20);
                binaryWriter.Write(this.Zero21);
                binaryWriter.Write(this.Zero22);
                binaryWriter.Write(this.Constant23);
                binaryWriter.Write(this.Zero24);
                binaryWriter.Write(this.Zero25);
                binaryWriter.Write(this.Zero26);
                binaryWriter.Write(this.Zero27);
                binaryWriter.Write(this.Zero28);
                binaryWriter.Write(this.Zero29);
                binaryWriter.Write(this.Section1.Constant01);
                binaryWriter.Write(this.Section1.Zero02);
                binaryWriter.Write(this.Section1.Zero03);
                binaryWriter.Write(this.Section1.Zero04);
                binaryWriter.Write(this.Section1.Zero05);
                binaryWriter.Write(this.Section1.Zero06);
                binaryWriter.Write(this.Section2.Constant01);
                binaryWriter.Write(this.Section2.Zero02);
                binaryWriter.Write(this.Section2.Zero03);
                binaryWriter.Write(this.Section2.Zero04);
                binaryWriter.Write(this.Section2.Zero05);
                binaryWriter.Write(this.Section2.Zero06);
                binaryWriter.Write(this.Section3.Constant01);
                binaryWriter.Write(this.Section3.Zero02);
                binaryWriter.Write(this.Section3.Zero03);
                binaryWriter.Write(this.Section3.Zero04);
                binaryWriter.Write(this.Section3.Zero05);
                binaryWriter.Write(this.Section3.Zero06);
                binaryWriter.Write(this.Section4.Constant01);
                binaryWriter.Write(this.Section4.Zero02);
                binaryWriter.Write(this.Section4.Zero03);
                binaryWriter.Write(this.Section4.Zero04);
                binaryWriter.Write(this.Section4.Zero05);
                binaryWriter.Write(this.Section4.Zero06);
                binaryWriter.Write(this.Zero31);
                binaryWriter.Write(this.Constant32);
                binaryWriter.Write(this.Unknown33);
                binaryWriter.Write(this.Constant34);
                binaryWriter.Write(this.SampleRate);
                binaryWriter.Write(this.ContentSize);
                binaryWriter.Write(this.SampleCount);
                binaryWriter.Write(this.LoopStart);
                binaryWriter.Write(this.LoopEnd);
                binaryWriter.Write(this.Constant36);
                binaryWriter.Write(this.LoopStartAlternate);
                binaryWriter.Write(this.LoopEndAlternate);
                binaryWriter.Write(this.Zero38);
                binaryWriter.Write(this.Constant39);
                binaryWriter.Write(this.Constant40);
                binaryWriter.Write(this.Constant41);
                binaryWriter.Write(this.Constant42);
                binaryWriter.Write(this.Zero43);
                binaryWriter.Write(this.Zero44);
                binaryWriter.Write(this.Zero45);
                binaryWriter.Write(this.Optional46);
                binaryWriter.Write(this.Optional47);
                binaryWriter.Write(this.Zero48);
            }
            return result;
        }

    }
}