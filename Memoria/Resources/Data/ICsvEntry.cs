using System;
using System.IO;
using System.Text;

namespace Memoria
{
    public interface ICsvEntry
    {
        void ParseEntry(String[] raw);
        void WriteEntry(CsvWriter sw);
    }
}