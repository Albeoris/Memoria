using System;

namespace Memoria.Prime
{
    internal sealed class DisposableAction : IDisposable
    {
        private readonly Action _action;
        private readonly Boolean _isSafe;
        private Boolean _isCanceled;

        public DisposableAction(Action action, Boolean isSafe = false)
        {
            _action = Exceptions.Exceptions.CheckArgumentNull(action, "action");
            _isSafe = isSafe;
        }

        public void Dispose()
        {
            try
            {
                if (!_isCanceled)
                    _action();
            }
            catch (Exception ex)
            {
                if (_isSafe)
                    Log.Error(ex);
                else
                    throw;
            }
        }

        public void Cancel()
        {
            _isCanceled = true;
        }
    }
}