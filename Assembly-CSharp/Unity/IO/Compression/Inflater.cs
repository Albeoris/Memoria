using System;

namespace Unity.IO.Compression
{
    internal class Inflater
    {
        public Inflater()
        {
            this.output = new OutputWindow();
            this.input = new InputBuffer();
            this.codeList = new Byte[320];
            this.codeLengthTreeCodeLength = new Byte[19];
            this.Reset();
        }

        internal void SetFileFormatReader(IFileFormatReader reader)
        {
            this.formatReader = reader;
            this.hasFormatReader = true;
            this.Reset();
        }

        private void Reset()
        {
            if (this.hasFormatReader)
            {
                this.state = InflaterState.ReadingHeader;
            }
            else
            {
                this.state = InflaterState.ReadingBFinal;
            }
        }

        public void SetInput(Byte[] inputBytes, Int32 offset, Int32 length)
        {
            this.input.SetInput(inputBytes, offset, length);
        }

        public Boolean Finished()
        {
            return this.state == InflaterState.Done || this.state == InflaterState.VerifyingFooter;
        }

        public Int32 AvailableOutput
        {
            get
            {
                return this.output.AvailableBytes;
            }
        }

        public Boolean NeedsInput()
        {
            return this.input.NeedsInput();
        }

        public Int32 Inflate(Byte[] bytes, Int32 offset, Int32 length)
        {
            Int32 num = 0;
            do
            {
                Int32 num2 = this.output.CopyTo(bytes, offset, length);
                if (num2 > 0)
                {
                    if (this.hasFormatReader)
                    {
                        this.formatReader.UpdateWithBytesRead(bytes, offset, num2);
                    }
                    offset += num2;
                    num += num2;
                    length -= num2;
                }
                if (length == 0)
                {
                    break;
                }
            }
            while (!this.Finished() && this.Decode());
            if (this.state == InflaterState.VerifyingFooter && this.output.AvailableBytes == 0)
            {
                this.formatReader.Validate();
            }
            return num;
        }

        private Boolean Decode()
        {
            Boolean flag = false;
            if (this.Finished())
            {
                return true;
            }
            if (this.hasFormatReader)
            {
                if (this.state == InflaterState.ReadingHeader)
                {
                    if (!this.formatReader.ReadHeader(this.input))
                    {
                        return false;
                    }
                    this.state = InflaterState.ReadingBFinal;
                }
                else if (this.state == InflaterState.StartReadingFooter || this.state == InflaterState.ReadingFooter)
                {
                    if (!this.formatReader.ReadFooter(this.input))
                    {
                        return false;
                    }
                    this.state = InflaterState.VerifyingFooter;
                    return true;
                }
            }
            if (this.state == InflaterState.ReadingBFinal)
            {
                if (!this.input.EnsureBitsAvailable(1))
                {
                    return false;
                }
                this.bfinal = this.input.GetBits(1);
                this.state = InflaterState.ReadingBType;
            }
            if (this.state == InflaterState.ReadingBType)
            {
                if (!this.input.EnsureBitsAvailable(2))
                {
                    this.state = InflaterState.ReadingBType;
                    return false;
                }
                this.blockType = (BlockType)this.input.GetBits(2);
                if (this.blockType == BlockType.Dynamic)
                {
                    this.state = InflaterState.ReadingNumLitCodes;
                }
                else if (this.blockType == BlockType.Static)
                {
                    this.literalLengthTree = HuffmanTree.StaticLiteralLengthTree;
                    this.distanceTree = HuffmanTree.StaticDistanceTree;
                    this.state = InflaterState.DecodeTop;
                }
                else
                {
                    if (this.blockType != BlockType.Uncompressed)
                    {
                        throw new InvalidDataException(SR.GetString("Unknown block type"));
                    }
                    this.state = InflaterState.UncompressedAligning;
                }
            }
            Boolean result;
            if (this.blockType == BlockType.Dynamic)
            {
                if (this.state < InflaterState.DecodeTop)
                {
                    result = this.DecodeDynamicBlockHeader();
                }
                else
                {
                    result = this.DecodeBlock(out flag);
                }
            }
            else if (this.blockType == BlockType.Static)
            {
                result = this.DecodeBlock(out flag);
            }
            else
            {
                if (this.blockType != BlockType.Uncompressed)
                {
                    throw new InvalidDataException(SR.GetString("Unknown block type"));
                }
                result = this.DecodeUncompressedBlock(out flag);
            }
            if (flag && this.bfinal != 0)
            {
                if (this.hasFormatReader)
                {
                    this.state = InflaterState.StartReadingFooter;
                }
                else
                {
                    this.state = InflaterState.Done;
                }
            }
            return result;
        }

        private Boolean DecodeUncompressedBlock(out Boolean end_of_block)
        {
            end_of_block = false;
            for (; ; )
            {
                switch (this.state)
                {
                    case InflaterState.UncompressedAligning:
                        this.input.SkipToByteBoundary();
                        this.state = InflaterState.UncompressedByte1;
                        goto IL_48;
                    case InflaterState.UncompressedByte1:
                    case InflaterState.UncompressedByte2:
                    case InflaterState.UncompressedByte3:
                    case InflaterState.UncompressedByte4:
                        goto IL_48;
                    case InflaterState.DecodingUncompressed:
                        goto IL_E4;
                }
                break;
            IL_48:
                Int32 bits = this.input.GetBits(8);
                if (bits < 0)
                {
                    return false;
                }
                this.blockLengthBuffer[this.state - InflaterState.UncompressedByte1] = (Byte)bits;
                if (this.state == InflaterState.UncompressedByte4)
                {
                    this.blockLength = (Int32)this.blockLengthBuffer[0] + (Int32)this.blockLengthBuffer[1] * 256;
                    Int32 num = (Int32)this.blockLengthBuffer[2] + (Int32)this.blockLengthBuffer[3] * 256;
                    if ((UInt16)this.blockLength != (UInt16)(~(UInt16)num))
                    {
                        goto Block_4;
                    }
                }
                this.state++;
            }
            throw new InvalidDataException(SR.GetString("Unknown state"));
        Block_4:
            throw new InvalidDataException(SR.GetString("Invalid block length"));
        IL_E4:
            Int32 num2 = this.output.CopyFrom(this.input, this.blockLength);
            this.blockLength -= num2;
            if (this.blockLength == 0)
            {
                this.state = InflaterState.ReadingBFinal;
                end_of_block = true;
                return true;
            }
            return this.output.FreeBytes == 0;
        }

        private Boolean DecodeBlock(out Boolean end_of_block_code_seen)
        {
            end_of_block_code_seen = false;
            Int32 i = this.output.FreeBytes;
            while (i > 258)
            {
                switch (this.state)
                {
                    case InflaterState.DecodeTop:
                    {
                        Int32 num = this.literalLengthTree.GetNextSymbol(this.input);
                        if (num < 0)
                        {
                            return false;
                        }
                        if (num < 256)
                        {
                            this.output.Write((Byte)num);
                            i--;
                            continue;
                        }
                        if (num == 256)
                        {
                            end_of_block_code_seen = true;
                            this.state = InflaterState.ReadingBFinal;
                            return true;
                        }
                        num -= 257;
                        if (num < 8)
                        {
                            num += 3;
                            this.extraBits = 0;
                        }
                        else if (num == 28)
                        {
                            num = 258;
                            this.extraBits = 0;
                        }
                        else
                        {
                            if (num < 0 || num >= (Int32)Inflater.extraLengthBits.Length)
                            {
                                throw new InvalidDataException(SR.GetString("Invalid data"));
                            }
                            this.extraBits = (Int32)Inflater.extraLengthBits[num];
                        }
                        this.length = num;
                        goto IL_109;
                    }
                    case InflaterState.HaveInitialLength:
                        goto IL_109;
                    case InflaterState.HaveFullLength:
                        goto IL_187;
                    case InflaterState.HaveDistCode:
                        break;
                    default:
                        throw new InvalidDataException(SR.GetString("Unknown state"));
                }
            IL_1FA:
                Int32 distance;
                if (this.distanceCode > 3)
                {
                    this.extraBits = this.distanceCode - 2 >> 1;
                    Int32 bits = this.input.GetBits(this.extraBits);
                    if (bits < 0)
                    {
                        return false;
                    }
                    distance = Inflater.distanceBasePosition[this.distanceCode] + bits;
                }
                else
                {
                    distance = this.distanceCode + 1;
                }
                this.output.WriteLengthDistance(this.length, distance);
                i -= this.length;
                this.state = InflaterState.DecodeTop;
                continue;
            IL_187:
                if (this.blockType == BlockType.Dynamic)
                {
                    this.distanceCode = this.distanceTree.GetNextSymbol(this.input);
                }
                else
                {
                    this.distanceCode = this.input.GetBits(5);
                    if (this.distanceCode >= 0)
                    {
                        this.distanceCode = (Int32)Inflater.staticDistanceTreeTable[this.distanceCode];
                    }
                }
                if (this.distanceCode < 0)
                {
                    return false;
                }
                this.state = InflaterState.HaveDistCode;
                goto IL_1FA;
            IL_109:
                if (this.extraBits > 0)
                {
                    this.state = InflaterState.HaveInitialLength;
                    Int32 bits2 = this.input.GetBits(this.extraBits);
                    if (bits2 < 0)
                    {
                        return false;
                    }
                    if (this.length < 0 || this.length >= (Int32)Inflater.lengthBase.Length)
                    {
                        throw new InvalidDataException(SR.GetString("Invalid data"));
                    }
                    this.length = Inflater.lengthBase[this.length] + bits2;
                }
                this.state = InflaterState.HaveFullLength;
                goto IL_187;
            }
            return true;
        }

        private Boolean DecodeDynamicBlockHeader()
        {
            switch (this.state)
            {
                case InflaterState.ReadingNumLitCodes:
                    this.literalLengthCodeCount = this.input.GetBits(5);
                    if (this.literalLengthCodeCount < 0)
                    {
                        return false;
                    }
                    this.literalLengthCodeCount += 257;
                    this.state = InflaterState.ReadingNumDistCodes;
                    break;
                case InflaterState.ReadingNumDistCodes:
                    break;
                case InflaterState.ReadingNumCodeLengthCodes:
                    goto IL_A6;
                case InflaterState.ReadingCodeLengthCodes:
                    goto IL_E7;
                case InflaterState.ReadingTreeCodesBefore:
                case InflaterState.ReadingTreeCodesAfter:
                    goto IL_199;
                default:
                    throw new InvalidDataException(SR.GetString("Unknown state"));
            }
            this.distanceCodeCount = this.input.GetBits(5);
            if (this.distanceCodeCount < 0)
            {
                return false;
            }
            this.distanceCodeCount++;
            this.state = InflaterState.ReadingNumCodeLengthCodes;
        IL_A6:
            this.codeLengthCodeCount = this.input.GetBits(4);
            if (this.codeLengthCodeCount < 0)
            {
                return false;
            }
            this.codeLengthCodeCount += 4;
            this.loopCounter = 0;
            this.state = InflaterState.ReadingCodeLengthCodes;
        IL_E7:
            while (this.loopCounter < this.codeLengthCodeCount)
            {
                Int32 bits = this.input.GetBits(3);
                if (bits < 0)
                {
                    return false;
                }
                this.codeLengthTreeCodeLength[(Int32)Inflater.codeOrder[this.loopCounter]] = (Byte)bits;
                this.loopCounter++;
            }
            for (Int32 i = this.codeLengthCodeCount; i < (Int32)Inflater.codeOrder.Length; i++)
            {
                this.codeLengthTreeCodeLength[(Int32)Inflater.codeOrder[i]] = 0;
            }
            this.codeLengthTree = new HuffmanTree(this.codeLengthTreeCodeLength);
            this.codeArraySize = this.literalLengthCodeCount + this.distanceCodeCount;
            this.loopCounter = 0;
            this.state = InflaterState.ReadingTreeCodesBefore;
        IL_199:
            while (this.loopCounter < this.codeArraySize)
            {
                if (this.state == InflaterState.ReadingTreeCodesBefore && (this.lengthCode = this.codeLengthTree.GetNextSymbol(this.input)) < 0)
                {
                    return false;
                }
                if (this.lengthCode <= 15)
                {
                    this.codeList[this.loopCounter++] = (Byte)this.lengthCode;
                }
                else
                {
                    if (!this.input.EnsureBitsAvailable(7))
                    {
                        this.state = InflaterState.ReadingTreeCodesAfter;
                        return false;
                    }
                    if (this.lengthCode == 16)
                    {
                        if (this.loopCounter == 0)
                        {
                            throw new InvalidDataException();
                        }
                        Byte b = this.codeList[this.loopCounter - 1];
                        Int32 num = this.input.GetBits(2) + 3;
                        if (this.loopCounter + num > this.codeArraySize)
                        {
                            throw new InvalidDataException();
                        }
                        for (Int32 j = 0; j < num; j++)
                        {
                            this.codeList[this.loopCounter++] = b;
                        }
                    }
                    else if (this.lengthCode == 17)
                    {
                        Int32 num = this.input.GetBits(3) + 3;
                        if (this.loopCounter + num > this.codeArraySize)
                        {
                            throw new InvalidDataException();
                        }
                        for (Int32 k = 0; k < num; k++)
                        {
                            this.codeList[this.loopCounter++] = 0;
                        }
                    }
                    else
                    {
                        Int32 num = this.input.GetBits(7) + 11;
                        if (this.loopCounter + num > this.codeArraySize)
                        {
                            throw new InvalidDataException();
                        }
                        for (Int32 l = 0; l < num; l++)
                        {
                            this.codeList[this.loopCounter++] = 0;
                        }
                    }
                }
                this.state = InflaterState.ReadingTreeCodesBefore;
            }
            Byte[] array = new Byte[288];
            Byte[] array2 = new Byte[32];
            Array.Copy(this.codeList, array, this.literalLengthCodeCount);
            Array.Copy(this.codeList, this.literalLengthCodeCount, array2, 0, this.distanceCodeCount);
            if (array[256] == 0)
            {
                throw new InvalidDataException();
            }
            this.literalLengthTree = new HuffmanTree(array);
            this.distanceTree = new HuffmanTree(array2);
            this.state = InflaterState.DecodeTop;
            return true;
        }

        private static readonly Byte[] extraLengthBits = new Byte[]
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            2,
            2,
            2,
            2,
            3,
            3,
            3,
            3,
            4,
            4,
            4,
            4,
            5,
            5,
            5,
            5,
            0
        };

        private static readonly Int32[] lengthBase = new Int32[]
        {
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            13,
            15,
            17,
            19,
            23,
            27,
            31,
            35,
            43,
            51,
            59,
            67,
            83,
            99,
            115,
            131,
            163,
            195,
            227,
            258
        };

        private static readonly Int32[] distanceBasePosition = new Int32[]
        {
            1,
            2,
            3,
            4,
            5,
            7,
            9,
            13,
            17,
            25,
            33,
            49,
            65,
            97,
            129,
            193,
            257,
            385,
            513,
            769,
            1025,
            1537,
            2049,
            3073,
            4097,
            6145,
            8193,
            12289,
            16385,
            24577,
            0,
            0
        };

        private static readonly Byte[] codeOrder = new Byte[]
        {
            16,
            17,
            18,
            0,
            8,
            7,
            9,
            6,
            10,
            5,
            11,
            4,
            12,
            3,
            13,
            2,
            14,
            1,
            15
        };

        private static readonly Byte[] staticDistanceTreeTable = new Byte[]
        {
            0,
            16,
            8,
            24,
            4,
            20,
            12,
            28,
            2,
            18,
            10,
            26,
            6,
            22,
            14,
            30,
            1,
            17,
            9,
            25,
            5,
            21,
            13,
            29,
            3,
            19,
            11,
            27,
            7,
            23,
            15,
            31
        };

        private OutputWindow output;

        private InputBuffer input;

        private HuffmanTree literalLengthTree;

        private HuffmanTree distanceTree;

        private InflaterState state;

        private Boolean hasFormatReader;

        private Int32 bfinal;

        private BlockType blockType;

        private Byte[] blockLengthBuffer = new Byte[4];

        private Int32 blockLength;

        private Int32 length;

        private Int32 distanceCode;

        private Int32 extraBits;

        private Int32 loopCounter;

        private Int32 literalLengthCodeCount;

        private Int32 distanceCodeCount;

        private Int32 codeLengthCodeCount;

        private Int32 codeArraySize;

        private Int32 lengthCode;

        private Byte[] codeList;

        private Byte[] codeLengthTreeCodeLength;

        private HuffmanTree codeLengthTree;

        private IFileFormatReader formatReader;
    }
}
