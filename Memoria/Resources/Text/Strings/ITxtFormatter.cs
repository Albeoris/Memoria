using System;
using System.IO;

namespace Memoria
{
    public interface ITxtFormatter
    {
        void Write(StreamWriter sw, TxtEntry entry);
        TxtEntry Read(StreamReader sr);
    }
}