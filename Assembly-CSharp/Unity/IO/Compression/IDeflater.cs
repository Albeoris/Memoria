using System;

namespace Unity.IO.Compression
{
	internal interface IDeflater : IDisposable
	{
		Boolean NeedsInput();

		void SetInput(Byte[] inputBuffer, Int32 startIndex, Int32 count);

		Int32 GetDeflateOutput(Byte[] outputBuffer);

		Boolean Finish(Byte[] outputBuffer, out Int32 bytesRead);
	}
}
