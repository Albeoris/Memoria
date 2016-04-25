using System;
using System.Collections.Generic;

namespace Memoria
{
    public abstract class IniSection
    {
        public readonly String Name;

        protected IniSection(String name)
        {
            Name = name;
        }

        internal abstract IEnumerable<IniValue> GetValues();
    }
}