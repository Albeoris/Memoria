using System;

namespace Memoria.Patcher
{
    public interface IPatch
    {
        String SourceName { get; }
        String TargetName { get; }
    }
}