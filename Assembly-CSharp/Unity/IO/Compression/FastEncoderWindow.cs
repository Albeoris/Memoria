using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class FastEncoderWindow
	{
		public FastEncoderWindow()
		{
			this.ResetWindow();
		}

		public Int32 BytesAvailable
		{
			get
			{
				return this.bufEnd - this.bufPos;
			}
		}

		public DeflateInput UnprocessedInput
		{
			get
			{
				return new DeflateInput
				{
					Buffer = this.window,
					StartIndex = this.bufPos,
					Count = this.bufEnd - this.bufPos
				};
			}
		}

		public void FlushWindow()
		{
			this.ResetWindow();
		}

		private void ResetWindow()
		{
			this.window = new Byte[16646];
			this.prev = new UInt16[8450];
			this.lookup = new UInt16[2048];
			this.bufPos = 8192;
			this.bufEnd = this.bufPos;
		}

		public Int32 FreeWindowSpace
		{
			get
			{
				return 16384 - this.bufEnd;
			}
		}

		public void CopyBytes(Byte[] inputBuffer, Int32 startIndex, Int32 count)
		{
			Array.Copy(inputBuffer, startIndex, this.window, this.bufEnd, count);
			this.bufEnd += count;
		}

		public void MoveWindows()
		{
			Array.Copy(this.window, this.bufPos - 8192, this.window, 0, 8192);
			for (Int32 i = 0; i < 2048; i++)
			{
				Int32 num = (Int32)(this.lookup[i] - 8192);
				if (num <= 0)
				{
					this.lookup[i] = 0;
				}
				else
				{
					this.lookup[i] = (UInt16)num;
				}
			}
			for (Int32 i = 0; i < 8192; i++)
			{
				Int64 num2 = (Int64)this.prev[i] - 8192L;
				if (num2 <= 0L)
				{
					this.prev[i] = 0;
				}
				else
				{
					this.prev[i] = (UInt16)num2;
				}
			}
			this.bufPos = 8192;
			this.bufEnd = this.bufPos;
		}

		private UInt32 HashValue(UInt32 hash, Byte b)
		{
			return hash << 4 ^ (UInt32)b;
		}

		private UInt32 InsertString(ref UInt32 hash)
		{
			hash = this.HashValue(hash, this.window[this.bufPos + 2]);
			UInt32 num = (UInt32)this.lookup[(Int32)((UIntPtr)(hash & 2047u))];
			this.lookup[(Int32)((UIntPtr)(hash & 2047u))] = (UInt16)this.bufPos;
			this.prev[this.bufPos & 8191] = (UInt16)num;
			return num;
		}

		private void InsertStrings(ref UInt32 hash, Int32 matchLen)
		{
			if (this.bufEnd - this.bufPos <= matchLen)
			{
				this.bufPos += matchLen - 1;
			}
			else
			{
				while (--matchLen > 0)
				{
					this.InsertString(ref hash);
					this.bufPos++;
				}
			}
		}

		internal Boolean GetNextSymbolOrMatch(Match match)
		{
			UInt32 hash = this.HashValue(0u, this.window[this.bufPos]);
			hash = this.HashValue(hash, this.window[this.bufPos + 1]);
			Int32 position = 0;
			Int32 num;
			if (this.bufEnd - this.bufPos <= 3)
			{
				num = 0;
			}
			else
			{
				Int32 num2 = (Int32)this.InsertString(ref hash);
				if (num2 != 0)
				{
					num = this.FindMatch(num2, out position, 32, 32);
					if (this.bufPos + num > this.bufEnd)
					{
						num = this.bufEnd - this.bufPos;
					}
				}
				else
				{
					num = 0;
				}
			}
			if (num < 3)
			{
				match.State = MatchState.HasSymbol;
				match.Symbol = this.window[this.bufPos];
				this.bufPos++;
			}
			else
			{
				this.bufPos++;
				if (num <= 6)
				{
					Int32 position2 = 0;
					Int32 num3 = (Int32)this.InsertString(ref hash);
					Int32 num4;
					if (num3 != 0)
					{
						num4 = this.FindMatch(num3, out position2, (Int32)((num >= 4) ? 8 : 32), 32);
						if (this.bufPos + num4 > this.bufEnd)
						{
							num4 = this.bufEnd - this.bufPos;
						}
					}
					else
					{
						num4 = 0;
					}
					if (num4 > num)
					{
						match.State = MatchState.HasSymbolAndMatch;
						match.Symbol = this.window[this.bufPos - 1];
						match.Position = position2;
						match.Length = num4;
						this.bufPos++;
						num = num4;
						this.InsertStrings(ref hash, num);
					}
					else
					{
						match.State = MatchState.HasMatch;
						match.Position = position;
						match.Length = num;
						num--;
						this.bufPos++;
						this.InsertStrings(ref hash, num);
					}
				}
				else
				{
					match.State = MatchState.HasMatch;
					match.Position = position;
					match.Length = num;
					this.InsertStrings(ref hash, num);
				}
			}
			if (this.bufPos == 16384)
			{
				this.MoveWindows();
			}
			return true;
		}

		private Int32 FindMatch(Int32 search, out Int32 matchPos, Int32 searchDepth, Int32 niceLength)
		{
			Int32 num = 0;
			Int32 num2 = 0;
			Int32 num3 = this.bufPos - 8192;
			Byte b = this.window[this.bufPos];
			while (search > num3)
			{
				if (this.window[search + num] == b)
				{
					Int32 i;
					for (i = 0; i < 258; i++)
					{
						if (this.window[this.bufPos + i] != this.window[search + i])
						{
							break;
						}
					}
					if (i > num)
					{
						num = i;
						num2 = search;
						if (i > 32)
						{
							break;
						}
						b = this.window[this.bufPos + i];
					}
				}
				if (--searchDepth == 0)
				{
					break;
				}
				search = (Int32)this.prev[search & 8191];
			}
			matchPos = this.bufPos - num2 - 1;
			if (num == 3 && matchPos >= 16384)
			{
				return 0;
			}
			return num;
		}

		[Conditional("DEBUG")]
		private void VerifyHashes()
		{
			for (Int32 i = 0; i < 2048; i++)
			{
				UInt16 num = this.lookup[i];
				while (num != 0 && this.bufPos - (Int32)num < 8192)
				{
					UInt16 num2 = this.prev[(Int32)(num & 8191)];
					if (this.bufPos - (Int32)num2 >= 8192)
					{
						break;
					}
					num = num2;
				}
			}
		}

		private UInt32 RecalculateHash(Int32 position)
		{
			return (UInt32)(((Int32)this.window[position] << 8 ^ (Int32)this.window[position + 1] << 4 ^ (Int32)this.window[position + 2]) & 2047);
		}

		private const Int32 FastEncoderHashShift = 4;

		private const Int32 FastEncoderHashtableSize = 2048;

		private const Int32 FastEncoderHashMask = 2047;

		private const Int32 FastEncoderWindowSize = 8192;

		private const Int32 FastEncoderWindowMask = 8191;

		private const Int32 FastEncoderMatch3DistThreshold = 16384;

		internal const Int32 MaxMatch = 258;

		internal const Int32 MinMatch = 3;

		private const Int32 SearchDepth = 32;

		private const Int32 GoodLength = 4;

		private const Int32 NiceLength = 32;

		private const Int32 LazyMatchThreshold = 6;

		private Byte[] window;

		private Int32 bufPos;

		private Int32 bufEnd;

		private UInt16[] prev;

		private UInt16[] lookup;
	}
}
