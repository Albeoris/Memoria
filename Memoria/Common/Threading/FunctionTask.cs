using System;

namespace Memoria
{
    internal sealed class FunctionTask<T> : Task<T>
    {
        private readonly Func<T> _function;

        public FunctionTask(Func<T> function)
        {
            _function = function;
        }

        protected override void Invoke()
        {
            _result = _function();
        }
    }

    internal sealed class FunctionTask<TState, T> : Task<T>
    {
        private readonly Func<TState, T> _function;
        private readonly TState _state;

        public FunctionTask(Func<TState, T> function, TState state)
        {
            _function = function;
            _state = state;
        }

        protected override void Invoke()
        {
            _result = _function(_state);
        }
    }
}