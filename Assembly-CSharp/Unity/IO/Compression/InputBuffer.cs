using System;

namespace Unity.IO.Compression
{
	internal class InputBuffer
	{
		public Int32 AvailableBits
		{
			get
			{
				return this.bitsInBuffer;
			}
		}

		public Int32 AvailableBytes
		{
			get
			{
				return this.end - this.start + this.bitsInBuffer / 8;
			}
		}

		public Boolean EnsureBitsAvailable(Int32 count)
		{
			if (this.bitsInBuffer < count)
			{
				if (this.NeedsInput())
				{
					return false;
				}
				this.bitBuffer |= (UInt32)((UInt32)this.buffer[this.start++] << (this.bitsInBuffer & 31 & 31));
				this.bitsInBuffer += 8;
				if (this.bitsInBuffer < count)
				{
					if (this.NeedsInput())
					{
						return false;
					}
					this.bitBuffer |= (UInt32)((UInt32)this.buffer[this.start++] << (this.bitsInBuffer & 31 & 31));
					this.bitsInBuffer += 8;
				}
			}
			return true;
		}

		public UInt32 TryLoad16Bits()
		{
			if (this.bitsInBuffer < 8)
			{
				if (this.start < this.end)
				{
					this.bitBuffer |= (UInt32)((UInt32)this.buffer[this.start++] << (this.bitsInBuffer & 31 & 31));
					this.bitsInBuffer += 8;
				}
				if (this.start < this.end)
				{
					this.bitBuffer |= (UInt32)((UInt32)this.buffer[this.start++] << (this.bitsInBuffer & 31 & 31));
					this.bitsInBuffer += 8;
				}
			}
			else if (this.bitsInBuffer < 16 && this.start < this.end)
			{
				this.bitBuffer |= (UInt32)((UInt32)this.buffer[this.start++] << (this.bitsInBuffer & 31 & 31));
				this.bitsInBuffer += 8;
			}
			return this.bitBuffer;
		}

		private UInt32 GetBitMask(Int32 count)
		{
			return (1u << count) - 1u;
		}

		public Int32 GetBits(Int32 count)
		{
			if (!this.EnsureBitsAvailable(count))
			{
				return -1;
			}
			Int32 result = (Int32)(this.bitBuffer & this.GetBitMask(count));
			this.bitBuffer >>= count;
			this.bitsInBuffer -= count;
			return result;
		}

		public Int32 CopyTo(Byte[] output, Int32 offset, Int32 length)
		{
			Int32 num = 0;
			while (this.bitsInBuffer > 0 && length > 0)
			{
				output[offset++] = (Byte)this.bitBuffer;
				this.bitBuffer >>= 8;
				this.bitsInBuffer -= 8;
				length--;
				num++;
			}
			if (length == 0)
			{
				return num;
			}
			Int32 num2 = this.end - this.start;
			if (length > num2)
			{
				length = num2;
			}
			Array.Copy(this.buffer, this.start, output, offset, length);
			this.start += length;
			return num + length;
		}

		public Boolean NeedsInput()
		{
			return this.start == this.end;
		}

		public void SetInput(Byte[] buffer, Int32 offset, Int32 length)
		{
			this.buffer = buffer;
			this.start = offset;
			this.end = offset + length;
		}

		public void SkipBits(Int32 n)
		{
			this.bitBuffer >>= n;
			this.bitsInBuffer -= n;
		}

		public void SkipToByteBoundary()
		{
			this.bitBuffer >>= this.bitsInBuffer % 8;
			this.bitsInBuffer -= this.bitsInBuffer % 8;
		}

		private Byte[] buffer;

		private Int32 start;

		private Int32 end;

		private UInt32 bitBuffer;

		private Int32 bitsInBuffer;
	}
}
