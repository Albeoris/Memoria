using System;

namespace Unity.IO.Compression
{
	internal class FastEncoder
	{
		public FastEncoder()
		{
			this.inputWindow = new FastEncoderWindow();
			this.currentMatch = new Match();
		}

		internal Int32 BytesInHistory
		{
			get
			{
				return this.inputWindow.BytesAvailable;
			}
		}

		internal DeflateInput UnprocessedInput
		{
			get
			{
				return this.inputWindow.UnprocessedInput;
			}
		}

		internal void FlushInput()
		{
			this.inputWindow.FlushWindow();
		}

		internal Double LastCompressionRatio
		{
			get
			{
				return this.lastCompressionRatio;
			}
		}

		internal void GetBlock(DeflateInput input, OutputBuffer output, Int32 maxBytesToCopy)
		{
			FastEncoder.WriteDeflatePreamble(output);
			this.GetCompressedOutput(input, output, maxBytesToCopy);
			this.WriteEndOfBlock(output);
		}

		internal void GetCompressedData(DeflateInput input, OutputBuffer output)
		{
			this.GetCompressedOutput(input, output, -1);
		}

		internal void GetBlockHeader(OutputBuffer output)
		{
			FastEncoder.WriteDeflatePreamble(output);
		}

		internal void GetBlockFooter(OutputBuffer output)
		{
			this.WriteEndOfBlock(output);
		}

		private void GetCompressedOutput(DeflateInput input, OutputBuffer output, Int32 maxBytesToCopy)
		{
			Int32 bytesWritten = output.BytesWritten;
			Int32 num = 0;
			Int32 num2 = this.BytesInHistory + input.Count;
			do
			{
				Int32 num3 = (Int32)((input.Count >= this.inputWindow.FreeWindowSpace) ? this.inputWindow.FreeWindowSpace : input.Count);
				if (maxBytesToCopy >= 1)
				{
					num3 = Math.Min(num3, maxBytesToCopy - num);
				}
				if (num3 > 0)
				{
					this.inputWindow.CopyBytes(input.Buffer, input.StartIndex, num3);
					input.ConsumeBytes(num3);
					num += num3;
				}
				this.GetCompressedOutput(output);
			}
			while (this.SafeToWriteTo(output) && this.InputAvailable(input) && (maxBytesToCopy < 1 || num < maxBytesToCopy));
			Int32 bytesWritten2 = output.BytesWritten;
			Int32 num4 = bytesWritten2 - bytesWritten;
			Int32 num5 = this.BytesInHistory + input.Count;
			Int32 num6 = num2 - num5;
			if (num4 != 0)
			{
				this.lastCompressionRatio = (Double)num4 / (Double)num6;
			}
		}

		private void GetCompressedOutput(OutputBuffer output)
		{
			while (this.inputWindow.BytesAvailable > 0 && this.SafeToWriteTo(output))
			{
				this.inputWindow.GetNextSymbolOrMatch(this.currentMatch);
				if (this.currentMatch.State == MatchState.HasSymbol)
				{
					FastEncoder.WriteChar(this.currentMatch.Symbol, output);
				}
				else if (this.currentMatch.State == MatchState.HasMatch)
				{
					FastEncoder.WriteMatch(this.currentMatch.Length, this.currentMatch.Position, output);
				}
				else
				{
					FastEncoder.WriteChar(this.currentMatch.Symbol, output);
					FastEncoder.WriteMatch(this.currentMatch.Length, this.currentMatch.Position, output);
				}
			}
		}

		private Boolean InputAvailable(DeflateInput input)
		{
			return input.Count > 0 || this.BytesInHistory > 0;
		}

		private Boolean SafeToWriteTo(OutputBuffer output)
		{
			return output.FreeBytes > 16;
		}

		private void WriteEndOfBlock(OutputBuffer output)
		{
			UInt32 num = FastEncoderStatics.FastEncoderLiteralCodeInfo[256];
			Int32 n = (Int32)(num & 31u);
			output.WriteBits(n, num >> 5);
		}

		internal static void WriteMatch(Int32 matchLen, Int32 matchPos, OutputBuffer output)
		{
			UInt32 num = FastEncoderStatics.FastEncoderLiteralCodeInfo[254 + matchLen];
			Int32 num2 = (Int32)(num & 31u);
			if (num2 <= 16)
			{
				output.WriteBits(num2, num >> 5);
			}
			else
			{
				output.WriteBits(16, num >> 5 & 65535u);
				output.WriteBits(num2 - 16, num >> 21);
			}
			num = FastEncoderStatics.FastEncoderDistanceCodeInfo[FastEncoderStatics.GetSlot(matchPos)];
			output.WriteBits((Int32)(num & 15u), num >> 8);
			Int32 num3 = (Int32)(num >> 4 & 15u);
			if (num3 != 0)
			{
				output.WriteBits(num3, (UInt32)(matchPos & (Int32)FastEncoderStatics.BitMask[num3]));
			}
		}

		internal static void WriteChar(Byte b, OutputBuffer output)
		{
			UInt32 num = FastEncoderStatics.FastEncoderLiteralCodeInfo[(Int32)b];
			output.WriteBits((Int32)(num & 31u), num >> 5);
		}

		internal static void WriteDeflatePreamble(OutputBuffer output)
		{
			output.WriteBytes(FastEncoderStatics.FastEncoderTreeStructureData, 0, (Int32)FastEncoderStatics.FastEncoderTreeStructureData.Length);
			output.WriteBits(9, 34u);
		}

		private FastEncoderWindow inputWindow;

		private Match currentMatch;

		private Double lastCompressionRatio;
	}
}
