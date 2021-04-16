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

        public static Dictionary<String, String[]> _characterNames = new Dictionary<String, String[]>()
        {
            { "US", new String[]{ "Zidane", "Vivi", "Dagger", "Steiner", "Freya", "Quina", "Eiko", "Amarant", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Zidane", "Cinna", "Marcus", "Blank" } },
            { "UK", new String[]{ "Zidane", "Vivi", "Dagger", "Steiner", "Freya", "Quina", "Eiko", "Amarant", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Zidane", "Cinna", "Marcus", "Blank" } },
            { "JP", new String[]{ "ジタン", "ビビ", "ダガー", "スタイナー", "フライヤ", "クイナ", "エーコ", "サラマンダー", "シナ", "シナ", "マーカス", "マーカス", "ブランク", "ブランク", "ベアトリクス", "ベアトリクス", "ジタン", "シナ", "マーカス", "ブランク" } },
            { "GR", new String[]{ "Zidane", "Vivi", "Lili", "Steiner", "Freya", "Quina", "Eiko", "Mahagon", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Zidane", "Cinna", "Marcus", "Blank" } },
            { "FR", new String[]{ "Djidane", "Bibi", "Dagga", "Steiner", "Freyja", "Kweena", "Eiko", "Tarask", "Cina", "Cina", "Markus", "Markus", "Frank", "Frank", "Beate", "Beate", "Djidane", "Cina", "Markus", "Frank" } },
            { "IT", new String[]{ "Gidan", "Vivi", "Daga", "Steiner", "Freija", "Quina", "Eiko", "Amarant", "Er Cina", "Er Cina", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Gidan", "Er Cina", "Marcus", "Blank" } },
            { "ES", new String[]{ "Yitán", "Vivi", "Daga", "Steiner", "Freija", "Quina", "Eiko", "Amarant", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Yitán", "Cinna", "Marcus", "Blank" } }
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