using System;

namespace Unity.IO.Compression
{
	internal class DeflaterManaged : IDisposable, IDeflater
	{
		internal DeflaterManaged()
		{
			this.deflateEncoder = new FastEncoder();
			this.copyEncoder = new CopyEncoder();
			this.input = new DeflateInput();
			this.output = new OutputBuffer();
			this.processingState = DeflaterManaged.DeflaterState.NotStarted;
		}

		Boolean IDeflater.NeedsInput()
		{
			return this.input.Count == 0 && this.deflateEncoder.BytesInHistory == 0;
		}

		void IDeflater.SetInput(Byte[] inputBuffer, Int32 startIndex, Int32 count)
		{
			Debug.Assert(this.input.Count == 0, "We have something left in previous input!");
			this.input.Buffer = inputBuffer;
			this.input.Count = count;
			this.input.StartIndex = startIndex;
			if (count > 0 && count < 256)
			{
				switch (this.processingState)
				{
				case DeflaterManaged.DeflaterState.NotStarted:
				case DeflaterManaged.DeflaterState.CheckingForIncompressible:
					this.processingState = DeflaterManaged.DeflaterState.StartingSmallData;
					break;
				case DeflaterManaged.DeflaterState.CompressThenCheck:
					this.processingState = DeflaterManaged.DeflaterState.HandlingSmallData;
					break;
				}
			}
		}

		Int32 IDeflater.GetDeflateOutput(Byte[] outputBuffer)
		{
			Debug.Assert(outputBuffer != null, "Can't pass in a null output buffer!");
			Debug.Assert(!this.NeedsInput(), "GetDeflateOutput should only be called after providing input");
			this.output.UpdateBuffer(outputBuffer);
			switch (this.processingState)
			{
			case DeflaterManaged.DeflaterState.NotStarted:
			{
				Debug.Assert(this.deflateEncoder.BytesInHistory == 0, "have leftover bytes in window");
				DeflateInput.InputState state = this.input.DumpState();
				OutputBuffer.BufferState state2 = this.output.DumpState();
				this.deflateEncoder.GetBlockHeader(this.output);
				this.deflateEncoder.GetCompressedData(this.input, this.output);
				if (!this.UseCompressed(this.deflateEncoder.LastCompressionRatio))
				{
					this.input.RestoreState(state);
					this.output.RestoreState(state2);
					this.copyEncoder.GetBlock(this.input, this.output, false);
					this.FlushInputWindows();
					this.processingState = DeflaterManaged.DeflaterState.CheckingForIncompressible;
				}
				else
				{
					this.processingState = DeflaterManaged.DeflaterState.CompressThenCheck;
				}
				goto IL_2A9;
			}
			case DeflaterManaged.DeflaterState.SlowDownForIncompressible1:
				this.deflateEncoder.GetBlockFooter(this.output);
				this.processingState = DeflaterManaged.DeflaterState.SlowDownForIncompressible2;
				break;
			case DeflaterManaged.DeflaterState.SlowDownForIncompressible2:
				break;
			case DeflaterManaged.DeflaterState.StartingSmallData:
				this.deflateEncoder.GetBlockHeader(this.output);
				this.processingState = DeflaterManaged.DeflaterState.HandlingSmallData;
				goto IL_28D;
			case DeflaterManaged.DeflaterState.CompressThenCheck:
				this.deflateEncoder.GetCompressedData(this.input, this.output);
				if (!this.UseCompressed(this.deflateEncoder.LastCompressionRatio))
				{
					this.processingState = DeflaterManaged.DeflaterState.SlowDownForIncompressible1;
					this.inputFromHistory = this.deflateEncoder.UnprocessedInput;
				}
				goto IL_2A9;
			case DeflaterManaged.DeflaterState.CheckingForIncompressible:
			{
				Debug.Assert(this.deflateEncoder.BytesInHistory == 0, "have leftover bytes in window");
				DeflateInput.InputState state3 = this.input.DumpState();
				OutputBuffer.BufferState state4 = this.output.DumpState();
				this.deflateEncoder.GetBlock(this.input, this.output, 8072);
				if (!this.UseCompressed(this.deflateEncoder.LastCompressionRatio))
				{
					this.input.RestoreState(state3);
					this.output.RestoreState(state4);
					this.copyEncoder.GetBlock(this.input, this.output, false);
					this.FlushInputWindows();
				}
				goto IL_2A9;
			}
			case DeflaterManaged.DeflaterState.HandlingSmallData:
				goto IL_28D;
			default:
				goto IL_2A9;
			}
			if (this.inputFromHistory.Count > 0)
			{
				this.copyEncoder.GetBlock(this.inputFromHistory, this.output, false);
			}
			if (this.inputFromHistory.Count == 0)
			{
				this.deflateEncoder.FlushInput();
				this.processingState = DeflaterManaged.DeflaterState.CheckingForIncompressible;
			}
			goto IL_2A9;
			IL_28D:
			this.deflateEncoder.GetCompressedData(this.input, this.output);
			IL_2A9:
			return this.output.BytesWritten;
		}

		Boolean IDeflater.Finish(Byte[] outputBuffer, out Int32 bytesRead)
		{
			Debug.Assert(outputBuffer != null, "Can't pass in a null output buffer!");
			Debug.Assert(this.processingState == DeflaterManaged.DeflaterState.NotStarted || this.processingState == DeflaterManaged.DeflaterState.CheckingForIncompressible || this.processingState == DeflaterManaged.DeflaterState.HandlingSmallData || this.processingState == DeflaterManaged.DeflaterState.CompressThenCheck || this.processingState == DeflaterManaged.DeflaterState.SlowDownForIncompressible1, "got unexpected processing state = " + this.processingState);
			Debug.Assert(this.NeedsInput());
			if (this.processingState == DeflaterManaged.DeflaterState.NotStarted)
			{
				bytesRead = 0;
				return true;
			}
			this.output.UpdateBuffer(outputBuffer);
			if (this.processingState == DeflaterManaged.DeflaterState.CompressThenCheck || this.processingState == DeflaterManaged.DeflaterState.HandlingSmallData || this.processingState == DeflaterManaged.DeflaterState.SlowDownForIncompressible1)
			{
				this.deflateEncoder.GetBlockFooter(this.output);
			}
			this.WriteFinal();
			bytesRead = this.output.BytesWritten;
			return true;
		}

		void IDisposable.Dispose()
		{
		}

		private Boolean NeedsInput()
		{
			return ((IDeflater)this).NeedsInput();
		}

		protected void Dispose(Boolean disposing)
		{
		}

		private Boolean UseCompressed(Double ratio)
		{
			return ratio <= 1.0;
		}

		private void FlushInputWindows()
		{
			this.deflateEncoder.FlushInput();
		}

		private void WriteFinal()
		{
			this.copyEncoder.GetBlock((DeflateInput)null, this.output, true);
		}

		private const Int32 MinBlockSize = 256;

		private const Int32 MaxHeaderFooterGoo = 120;

		private const Int32 CleanCopySize = 8072;

		private const Double BadCompressionThreshold = 1.0;

		private FastEncoder deflateEncoder;

		private CopyEncoder copyEncoder;

		private DeflateInput input;

		private OutputBuffer output;

		private DeflaterManaged.DeflaterState processingState;

		private DeflateInput inputFromHistory;

		private enum DeflaterState
		{
			NotStarted,
			SlowDownForIncompressible1,
			SlowDownForIncompressible2,
			StartingSmallData,
			CompressThenCheck,
			CheckingForIncompressible,
			HandlingSmallData
		}
	}
}
