using System;

namespace Memoria.Assets
{
    public static class EmbadedSentenseLoader
    {
        public static String[] LoadSentense(String path)
        {
            String text = AssetManager.LoadString(path);
            return text == null ? null : ExtractSentenseEnd(text);
        }

        public static String LoadText(String path)
        {
            return AssetManager.LoadString(path);
        }

        public static String[] ExtractSentenseEnd(String text)
        {
            String[] split = text.Split(DELIM, StringSplitOptions.None);
            if (split.Length > 0 && text.EndsWith(DELIM[0]))
                Array.Resize(ref split, split.Length - 1);
            return split;
        }

        private static readonly String[] DELIM = new[] { "[ENDN]" };
}
}
