using System;
using System.Collections.Generic;
using System.Text;

namespace Memoria
{
    public class ExportFieldCharacterNames
    {
        public static ExportFieldCharacterNames GetCurrent()
        {
            String localizationSymbol = Localization.GetSymbol();
            ExportFieldCharacterNames result = Create(localizationSymbol);
            result.Initialize();
            return result;
        }

        private static ExportFieldCharacterNames Create(String localizationSymbol)
        {
            if (localizationSymbol == "US")
                return new UsExportFieldCharacterNames();

            return new ExportFieldCharacterNames();
        }

        public TxtEntry[] General { get; set; }
        public Dictionary<String, TxtEntry[]> Fields { get; set; }
        private volatile KeyValuePair<String, TextReplacement>[] _general;
        private volatile Dictionary<String, KeyValuePair<String, TextReplacement>[]> _fields;

        protected ExportFieldCharacterNames()
        {
        }

        private void Initialize()
        {
            String[] general = InitializeGeneral();
            Dictionary<String, String[]> fields = InitializeFields();

            _general = Prepare(general);
            _fields = Prepare(fields);

            General = Reverse(general);
            Fields = Reverse(fields);
        }

        private static KeyValuePair<String, TextReplacement>[] Prepare(String[] source)
        {
            if (source == null)
                return null;

            KeyValuePair<String, TextReplacement>[] dic = new KeyValuePair<String, TextReplacement>[source.Length];
            for (Int32 i = 0; i < source.Length; i++)
                dic[i] = new KeyValuePair<String, TextReplacement>(source[i], (ReplaceTextDelegate)ReplaceText);
            return dic;
        }

        private Dictionary<String, KeyValuePair<String, TextReplacement>[]> Prepare(Dictionary<String, String[]> source)
        {
            if (source == null)
                return null;

            Dictionary<String, KeyValuePair<String, TextReplacement>[]> result = new Dictionary<String, KeyValuePair<String, TextReplacement>[]>(source.Count);
            foreach (KeyValuePair<String, String[]> pair in source)
                result[pair.Key] = Prepare(pair.Value);
            return result;
        }

        private TxtEntry[] Reverse(String[] general)
        {
            if (general == null)
                return null;

            TxtEntry[] array = new TxtEntry[general.Length];
            for (Int32 i = 0; i < general.Length; i++)
                array[i] = new TxtEntry {Index = i, Prefix = '{' + general[i] + '}', Value = general[i]};
            return array;
        }

        private Dictionary<String, TxtEntry[]> Reverse(Dictionary<String, String[]> general)
        {
            if (general == null)
                return null;

            Dictionary<String, TxtEntry[]> result = new Dictionary<String, TxtEntry[]>(general.Count);
            foreach (KeyValuePair<String, String[]> pair in general)
                result[pair.Key] = Reverse(pair.Value);
            return result;
        }

        public String Replace(String fieldName, String str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            KeyValuePair<String, TextReplacement>[] dic;
            if (_fields != null && _fields.TryGetValue(fieldName, out dic))
                str = str.ReplaceAll(dic);

            if (_general != null)
                str = str.ReplaceAll(_general);

            return str;
        }

        protected virtual String[] InitializeGeneral()
        {
            return null;
        }

        protected virtual Dictionary<String, String[]> InitializeFields()
        {
            return null;
        }

        //private static KeyValuePair<String, String>[] GetNameExpressions()
        //{
        //    var array = GetArray();
        //    try
        //    {
        //        KeyValuePair<Regex, String>[] result = new KeyValuePair<Regex, String>[array.Length];
        //        for (int i = 0; i < result.Length; i++)
        //            result[i] = new KeyValuePair<Regex, string>(new Regex(@"([^{])\b" + array[i] + @"\b"), "$1{" + array[i] + "}");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}

        private static String ReplaceText(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            if (length == word.Length)
            {
                if (index + 1 < str.Length)
                {
                    Char ch = str[index + 1];
                    if (Char.IsLetterOrDigit(ch))
                    {
                        length = 1;
                        return word[0].ToString();
                    }
                }
            }
            else
            {
                Char ch = word[length];
                if (Char.IsLetterOrDigit(ch))
                {
                    length = 1;
                    return word[0].ToString();
                }
            }

            return '{' + word.ToString(0, length) + '}';
        }
    }
}