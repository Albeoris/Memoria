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
            return text.Split(DELIM, StringSplitOptions.None);
        }

        private static readonly String[] DELIM = new[] { "[ENDN]" };
}
}