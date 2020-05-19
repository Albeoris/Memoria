using System;
using System.Runtime.CompilerServices;

namespace FF8.Core
{
    public interface IAwaiter
    {
        Boolean IsCompleted { get; }
        void GetResult();
    }
}