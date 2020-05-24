using System;

namespace FF8.Core
{
    public interface IVariablesService
    {
        Boolean IsSupported { get; }

        Int32 Get(GlobalVariableId id);
        void Set(GlobalVariableId id, Int32 value);
        
        Int32 Get(MapVariableId id);
        void Set(MapVariableId id, Int32 value);
        
        Int32 Get(SystemVariableId id);
        void Set(SystemVariableId id, Int32 value);
    }
}