using System;

namespace Memoria.Assets
{
    public static class CharacterNamesFormatter
    {
        public static String[] CharacterDefaultNames()
        {
            switch (EmbadedTextResources.CurrentSymbol ?? Localization.GetSymbol())
            {
                case "US":
                    return new[]
                    {
                        "Zidane", "Vivi", "Dagger", "Steiner", "Freya", "Quina", "Eiko", "Amarant", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Zidane", "Cinna", "Marcus", "Blank"
                    };
                case "UK":
                    return new[]
                    {
                        "Zidane", "Vivi", "Dagger", "Steiner", "Freya", "Quina", "Eiko", "Amarant", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Zidane", "Cinna", "Marcus", "Blank"
                    };
                case "JP":
                    return new[]
                    {
                        "ジタン", "ビビ", "ダガー", "スタイナー", "フライヤ", "クイナ", "エーコ", "サラマンダー", "シナ", "シナ", "マーカス", "マーカス", "ブランク", "ブランク", "ベアトリクス", "ベアトリクス", "ジタン", "シナ", "マーカス", "ブランク"
                    };
                case "GR":
                    return new[]
                    {
                        "Zidane", "Vivi", "Lili", "Steiner", "Freya", "Quina", "Eiko", "Mahagon", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Zidane", "Cinna", "Marcus", "Blank"
                    };
                case "FR":
                    return new[]
                    {
                        "Djidane", "Bibi", "Dagga", "Steiner", "Freyja", "Kweena", "Eiko", "Tarask", "Cina", "Cina", "Markus", "Markus", "Frank", "Frank", "Beate", "Beate", "Djidane", "Cina", "Markus", "Frank"
                    };
                case "IT":
                    return new[]
                    {
                        "Gidan", "Vivi", "Daga", "Steiner", "Freija", "Quina", "Eiko", "Amarant", "Er Cina", "Er Cina", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Gidan", "Er Cina", "Marcus", "Blank"
                    };
                case "ES":
                    return new[]
                    {
                        "Yitán", "Vivi", "Daga", "Steiner", "Freija", "Quina", "Eiko", "Amarant", "Cinna", "Cinna", "Marcus", "Marcus", "Blank", "Blank", "Beatrix", "Beatrix", "Yitán", "Cinna", "Marcus", "Blank"
                    };
                default:
                    return new String[0];
            }
        }

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