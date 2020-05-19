using System;
using System.Collections.Generic;

namespace FF8.Core
{
    public sealed class GlobalVariableService : IGlobalVariableService
    {
        public Boolean IsSupported => true;

        private readonly Int64[] _values = new Int64[8192];

        public T Get<T>(GlobalVariableId<T> id) where T : unmanaged
        {
            unsafe
            {
                fixed (Int64* ptr = &_values[id.VariableId])
                    return *(T*)ptr;
            }
        }

        public void Set<T>(GlobalVariableId<T> id, T value) where T : unmanaged
        {
            unsafe
            {
                fixed (Int64* ptr = &_values[id.VariableId])
                    *(T*)ptr = value;
            }
        }
    }
}