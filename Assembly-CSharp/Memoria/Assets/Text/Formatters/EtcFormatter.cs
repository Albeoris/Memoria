using System;

namespace Memoria.Assets
{
    public static class EtcFormatter
    {
        public static TxtEntry[] Build(String prefix, String[] value)
        {
            TxtEntry[] abilities = new TxtEntry[value.Length];
            for (Int32 i = 0; i < abilities.Length; i++)
                abilities[i] = new TxtEntry { Prefix = prefix, Value = value[i] };
            return abilities;
        }

        public static void Parse(TxtEntry[] entreis, out String[] values)
        {
            values = new String[entreis.Length];

            for (Int32 i = 0; i < entreis.Length; i++)
                values[i] = entreis[i].Value;
        }
    }
}
