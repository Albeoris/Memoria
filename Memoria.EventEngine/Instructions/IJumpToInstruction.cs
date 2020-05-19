using System;

namespace FF8.JSM.Instructions
{
    public interface IJumpToInstruction : IJsmInstruction
    {
        Int32 Index { get; set; }
    }
}