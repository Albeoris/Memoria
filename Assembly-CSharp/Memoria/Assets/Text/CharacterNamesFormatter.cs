using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public static class CharacterNamesFormatter
    {
        public static String[] CharacterDefaultNames()
        {
            String[] nameArray;
            if (_characterNames.TryGetValue(EmbadedTextResources.CurrentSymbol ?? Localization.GetSymbol(), out nameArray))
                return nameArray;
            return new String[0];
        }

        public static String[] CharacterScriptNames()
        {
            String[] nameArray;
            if (_characterNames.TryGetValue("US", out nameArray))
                return nameArray;
            return new String[0];
        }

        public static Dictionary<String, String[]> _characterNames = new Dictionary<String, String[]>()
        {
            { "US", new String[]{ "Zidane", "Vivi", "Dagger", "Steiner", "Freya", "Quina", "Eiko", "Amarant", "Cinna", "Marcus", "Blank", "Beatrix" } },
            { "UK", new String[]{ "Zidane", "Vivi", "Dagger", "Steiner", "Freya", "Quina", "Eiko", "Amarant", "Cinna", "Marcus", "Blank", "Beatrix" } },
            { "JP", new String[]{ "ジタン", "ビビ", "ダガー", "スタイナー", "フライヤ", "クイナ", "エーコ", "サラマンダー", "シナ", "マーカス", "ブランク", "ベアトリクス" } },
            { "GR", new String[]{ "Zidane", "Vivi", "Lili", "Steiner", "Freya", "Quina", "Eiko", "Mahagon", "Cinna", "Marcus", "Blank", "Beatrix" } },
            { "FR", new String[]{ "Djidane", "Bibi", "Dagga", "Steiner", "Freyja", "Kweena", "Eiko", "Tarask", "Cina", "Markus", "Frank", "Beate" } },
            { "IT", new String[]{ "Gidan", "Vivi", "Daga", "Steiner", "Freija", "Quina", "Eiko", "Amarant", "Er Cina", "Marcus", "Blank", "Beatrix" } },
            { "ES", new String[]{ "Yitán", "Vivi", "Daga", "Steiner", "Freija", "Quina", "Eiko", "Amarant", "Cinna", "Marcus", "Blank", "Beatrix" } }
        };

        public static TxtEntry[] Build(String prefix, String[] characterNames)
        {
            TxtEntry[] names = new TxtEntry[characterNames.Length];
            for (Int32 i = 0; i < names.Length; i++)
                names[i] = new TxtEntry {Prefix = prefix, Value = characterNames[i]};
            return names;
        }

        public static String[] Parse(TxtEntry[] entreis)
        {
            String[] result = new String[entreis.Length];
            for (Int32 i = 0; i < result.Length; i++)
                result[i] = entreis[i].Value;
            return result;
        }
    }
}