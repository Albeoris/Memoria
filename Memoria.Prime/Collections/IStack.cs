using System;

namespace Memoria.Prime.Collections
{
    public interface IStack<T>
    {
        Int32 Count { get; }
        void Push(T item);
        T Pop();
    }
}