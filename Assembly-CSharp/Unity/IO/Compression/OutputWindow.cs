using System;

namespace Unity.IO.Compression
{
	internal class OutputWindow
	{
		public void Write(Byte b)
		{
			this.window[this.end++] = b;
			this.end &= 32767;
			this.bytesUsed++;
		}

		public void WriteLengthDistance(Int32 length, Int32 distance)
		{
			this.bytesUsed += length;
			Int32 num = this.end - distance & 32767;
			Int32 num2 = 32768 - length;
			if (num <= num2 && this.end < num2)
			{
				if (length <= distance)
				{
					Array.Copy(this.window, num, this.window, this.end, length);
					this.end += length;
				}
				else
				{
					while (length-- > 0)
					{
						this.window[this.end++] = this.window[num++];
					}
				}
			}
			else
			{
				while (length-- > 0)
				{
					this.window[this.end++] = this.window[num++];
					this.end &= 32767;
					num &= 32767;
				}
			}
		}

		public Int32 CopyFrom(InputBuffer input, Int32 length)
		{
			length = Math.Min(Math.Min(length, 32768 - this.bytesUsed), input.AvailableBytes);
			Int32 num = 32768 - this.end;
			Int32 num2;
			if (length > num)
			{
				num2 = input.CopyTo(this.window, this.end, num);
				if (num2 == num)
				{
					num2 += input.CopyTo(this.window, 0, length - num);
				}
			}
			else
			{
				num2 = input.CopyTo(this.window, this.end, length);
			}
			this.end = (this.end + num2 & 32767);
			this.bytesUsed += num2;
			return num2;
		}

		public Int32 FreeBytes
		{
			get
			{
				return 32768 - this.bytesUsed;
			}
		}

		public Int32 AvailableBytes
		{
			get
			{
				return this.bytesUsed;
			}
		}

		public Int32 CopyTo(Byte[] output, Int32 offset, Int32 length)
		{
			Int32 num;
			if (length > this.bytesUsed)
			{
				num = this.end;
				length = this.bytesUsed;
			}
			else
			{
				num = (this.end - this.bytesUsed + length & 32767);
			}
			Int32 num2 = length;
			Int32 num3 = length - num;
			if (num3 > 0)
			{
				Array.Copy(this.window, 32768 - num3, output, offset, num3);
				offset += num3;
				length = num;
			}
			Array.Copy(this.window, num - length, output, offset, length);
			this.bytesUsed -= num2;
			return num2;
		}

		private const Int32 WindowSize = 32768;

		private const Int32 WindowMask = 32767;

		private Byte[] window = new Byte[32768];

		private Int32 end;

		private Int32 bytesUsed;
	}
}
