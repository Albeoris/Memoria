using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public interface ITextWriter
    {
        void WriteAll(String outputPath, IList<TxtEntry> entries);
    }
}
