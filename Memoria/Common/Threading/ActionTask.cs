using System;

namespace Memoria
{
    internal sealed class ActionTask : Task
    {
        private readonly Action _action;

        public ActionTask(Action action)
        {
            _action = action;
        }

        protected override void Invoke()
        {
            _action();
        }
    }

    internal sealed class ActionTask<TState> : Task
    {
        private readonly Action<TState> _action;
        private readonly TState _state;

        public ActionTask(Action<TState> action, TState state)
        {
            _action = action;
            _state = state;
        }

        protected override void Invoke()
        {
            _action(_state);
        }
    }
}