using System;
using System.IO;

namespace Memoria.Assets
{
    public interface ITxtFormatter
    {
        void Write(StreamWriter sw, TxtEntry entry);
        TxtEntry Read(StreamReader sr);
    }
}