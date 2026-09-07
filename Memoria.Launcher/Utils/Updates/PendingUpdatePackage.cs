using NLog;
using System;
using System.IO;
using System.Threading;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class PendingUpdatePackage : IDisposable
    {
        private readonly Logger _log = AppLogger.GetLogger(nameof(PendingUpdatePackage));
        private readonly String _pendingPath;
        private readonly String _targetPath;
        private Boolean _committed;

        public PendingUpdatePackage(String pendingPath, String targetPath)
        {
            _pendingPath = Path.GetFullPath(pendingPath ?? throw new ArgumentNullException(nameof(pendingPath)));
            _targetPath = Path.GetFullPath(targetPath ?? throw new ArgumentNullException(nameof(targetPath)));
        }

        public String Commit(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_committed)
                throw new InvalidOperationException("The update package has already been committed.");

            if (File.Exists(_targetPath))
                File.Replace(_pendingPath, _targetPath, destinationBackupFileName: null);
            else
                File.Move(_pendingPath, _targetPath);

            _committed = true;
            return _targetPath;
        }

        public void Dispose()
        {
            if (_committed || !File.Exists(_pendingPath))
                return;

            try
            {
                File.Delete(_pendingPath);
            }
            catch (Exception exception)
            {
                _log.Warn(exception, "Unable to delete a pending launcher update. Path: {Path}", _pendingPath);
            }
        }
    }
}
