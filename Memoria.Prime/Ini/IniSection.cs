using System;
using System.Collections.Generic;

namespace Memoria.Prime.Ini
{
    public abstract class IniSection
    {
        public readonly String Name;

        protected IniSection(String name)
        {
            Name = name;
        }

        protected abstract IEnumerable<IniValue> GetValues();

        internal IEnumerable<IniValue> GetValuesInternal()
        {
            return GetValues();
        }
    }
}