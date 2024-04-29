using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public interface ITextReader
    {
        TxtEntry[] ReadAll(String inputPath);
    }
}
