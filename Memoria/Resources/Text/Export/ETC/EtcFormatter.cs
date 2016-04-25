using System;

namespace Memoria
{
    public static class EtcFormatter
    {
        public static TxtEntry[] Build(String prefix, String[] value)
        {
            TxtEntry[] abilities = new TxtEntry[value.Length];
            for (int i = 0; i < abilities.Length; i++)
                abilities[i] = Build(prefix, value[i]);
            return abilities;
        }

        private static TxtEntry Build(String prefix, String value)
        {
            return new TxtEntry
            {
                Prefix = prefix,
                Value = value
            };
        }
    }
}