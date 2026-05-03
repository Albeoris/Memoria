using System;
using System.Collections.Generic;

namespace Memoria.Prime
{
    public sealed class DisposableStack : IDisposable
    {
        private readonly Stack<IDisposable> _stack;

        public DisposableStack(Int32 capacity = 4)
        {
            _stack = new Stack<IDisposable>(capacity);
        }

        public void Dispose()
        {
            while (_stack.Count > 0)
                _stack.Pop().SafeDispose();
        }

        public T Add<T>(T item) where T : IDisposable
        {
            _stack.Push(item);
            return item;
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}
