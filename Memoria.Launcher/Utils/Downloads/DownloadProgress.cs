using System;

namespace Memoria.Launcher.Utils.Downloads
{
    public readonly struct DownloadProgress
    {
        public DownloadProgress(Int64 bytesReceived, Int64 totalBytes)
        {
            if (bytesReceived < 0)
                throw new ArgumentOutOfRangeException(nameof(bytesReceived));
            if (totalBytes < -1)
                throw new ArgumentOutOfRangeException(nameof(totalBytes));

            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        public Int64 BytesReceived { get; }
        public Int64 TotalBytes { get; }

        public Int32 Percentage
        {
            get
            {
                if (TotalBytes <= 0)
                    return 0;

                Double value = BytesReceived * 100d / TotalBytes;
                return (Int32)Math.Max(0, Math.Min(100, Math.Round(value)));
            }
        }
    }
}
