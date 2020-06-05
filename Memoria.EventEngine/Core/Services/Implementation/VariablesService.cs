using System;
using System.Collections.Generic;

namespace FF8.Core
{
    public sealed class VariablesService : IVariablesService
    {
        public Boolean IsSupported => true;

        public Int32 Get(GlobalVariableId id) => throw new NotImplementedException();
        public void Set(GlobalVariableId id, Int32 value) => throw new NotImplementedException();
        public Int32 Get(MapVariableId id) => throw new NotImplementedException();
        public void Set(MapVariableId id, Int32 value) => throw new NotImplementedException();
        public Int32 Get(SystemVariableId id) => throw new NotImplementedException();
        public void Set(SystemVariableId id, Int32 value) => throw new NotImplementedException();
    }
}