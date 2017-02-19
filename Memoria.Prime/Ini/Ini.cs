using System.Collections.Generic;

namespace Memoria.Prime.Ini
{
    public abstract class Ini
    {
        public abstract IEnumerable<IniSection> GetSections();
    }
}