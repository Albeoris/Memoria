using System;
using System.Collections.Generic;
using System.Linq;

namespace Memoria
{
    public static class LocationNameFormatter
    {
        private const String Prefix = "$location";
        private const String RootPrefix = "$locationHeader";
        private const String OtherGroup = "Other";

        public static Dictionary<String, LinkedList<TxtEntry>> Build(Dictionary<Int32, String> locationNames)
        {
            Dictionary<String, LinkedList<TxtEntry>> dic = new Dictionary<String, LinkedList<TxtEntry>>();
            foreach (KeyValuePair<Int32, String> pair in locationNames)
                AddEntry(pair, dic);

            RegroupSingleEntries(dic);
            return dic;
        }

        private static void AddEntry(KeyValuePair<Int32, String> pair, Dictionary<String, LinkedList<TxtEntry>> dic)
        {
            String fullName = pair.Value;
            String[] parts = fullName.Split('/');
            String left = parts[0];
            Int32 index = pair.Key;

            LinkedList<TxtEntry> list = GetList(dic, left);
            AddEntry(parts, index, list);
        }

        private static void AddEntry(String[] parts, Int32 index, LinkedList<TxtEntry> list)
        {
            if (parts.Length > 1)
            {
                TxtEntry entry = new TxtEntry {Prefix = Prefix, Index = index, Value = parts[1]};
                list.AddLast(entry);
            }
            else
            {
                list.First.Value.Prefix = Prefix;
                list.First.Value.Index = index;
            }
        }

        private static LinkedList<TxtEntry> GetList(Dictionary<String, LinkedList<TxtEntry>> dic, String fileName)
        {
            LinkedList<TxtEntry> list;
            if (dic.TryGetValue(fileName, out list))
                return list;

            list = new LinkedList<TxtEntry>();
            dic.Add(fileName, list);

            TxtEntry entry = new TxtEntry {Prefix = RootPrefix, Index = dic.Count, Value = fileName};
            list.AddLast((TxtEntry)entry);
            return list;
        }

        private static void RegroupSingleEntries(Dictionary<String, LinkedList<TxtEntry>> dic)
        {
            TxtEntry[] single = dic.Where(p => p.Value.Count == 1).Select(p => p.Value.First.Value).ToArray();
            if (single.Length <= 0)
                return;

            LinkedList<TxtEntry> list = new LinkedList<TxtEntry>();
            dic.Add(OtherGroup, list);
            foreach (TxtEntry entry in single)
            {
                entry.Prefix = Prefix;
                list.AddLast(entry);
                dic.Remove(entry.Value);
            }
        }
    }
}