using System;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Patcher
{
	public sealed class ConsoleProgressHandler : IDisposable
	{
		private readonly Int64 _totalSize;
		private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
		private readonly DateTime _begin;

		private Int64 _processedSize = 0;

		public ConsoleProgressHandler(Int64 totalSize)
		{
			_totalSize = totalSize;
			_begin = DateTime.UtcNow;
			Task.Run(() => BackgroundProcess());
		}

		public void Dispose()
		{
			_stopEvent.Set();
			_stopEvent.Dispose();
			Refresh();
		}

		public void IncrementProcessedSize(Int64 size)
		{
			Interlocked.Add(ref _processedSize, size);
		}

		private void BackgroundProcess()
		{
			try
			{
				while (!_stopEvent.WaitOne(200))
				{
					Refresh();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private void Refresh()
		{
			Double percents = (_totalSize == 0) ? 0.0 : 100 * _processedSize / (double)_totalSize;
			TimeSpan elapsed = DateTime.UtcNow - _begin;
			Double speed = _processedSize / Math.Max(elapsed.TotalSeconds, 1);
			if (speed < 1) speed = 1;
			TimeSpan left = TimeSpan.FromSeconds((_totalSize - _processedSize) / speed);

			Console.Title = String.Format("Patching... {0:F2}%  {1}: {2:mm\\:ss}  {3} / {4}  {5}: {6:mm\\:ss}",
				percents,
				Lang.Measurement.Elapsed, elapsed,
				FormatBytes(_processedSize), FormatBytes(_totalSize),
				Lang.Measurement.Remaining, left);
		}

		public String FormatBytes(Int64 value)
		{
			Int32 i = 0;
			Decimal dec = value;
			while ((dec > 1024))
			{
				dec /= 1024;
				i++;
			}

			switch (i)
			{
				case 0:
					return String.Format("{0:F2} " + Lang.Measurement.ByteAbbr, dec);
				case 1:
					return String.Format("{0:F2} " + Lang.Measurement.KByteAbbr, dec);
				case 2:
					return String.Format("{0:F2} " + Lang.Measurement.MByteAbbr, dec);
				case 3:
					return String.Format("{0:F2} " + Lang.Measurement.GByteAbbr, dec);
				case 4:
					return String.Format("{0:F2} " + Lang.Measurement.TByteAbbr, dec);
				case 5:
					return String.Format("{0:F2} " + Lang.Measurement.PByteAbbr, dec);
				case 6:
					return String.Format("{0:F2} " + Lang.Measurement.EByteAbbr, dec);
				default:
					throw new ArgumentOutOfRangeException(nameof(value));
			}
		}
	}
}
