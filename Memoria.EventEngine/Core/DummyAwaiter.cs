using System;

namespace FF8.Core
{
    public sealed class DummyAwaiter : IAwaiter
    {
        public static IAwaiter Instance { get; } = new DummyAwaiter();

        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        public Boolean IsCompleted => true;

        public void GetResult()
        {
        }
    }
}