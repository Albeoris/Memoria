using System;

namespace Memoria.Prime.CSV
{
    public interface ICsvEntry
    {
        void ParseEntry(String[] raw);
        void WriteEntry(CsvWriter sw);
    }
}