using System.Collections.Generic;

namespace Memoria
{
    public abstract class Ini
    {
        internal abstract IEnumerable<IniSection> GetSections();
    }
}