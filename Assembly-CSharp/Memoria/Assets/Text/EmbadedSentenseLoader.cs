using System;
using UnityEngine;

namespace Memoria.Assets
{
    public static class EmbadedSentenseLoader
    {
        public static String[] LoadSentense(String path)
        {
            String text = AssetManager.LoadString(path, out _, false);
            return text == null ? null : ExtractSentenseEnd(text);
        }

        public static String LoadText(String path)
        {
            return AssetManager.LoadString(path, out _, false);
        }

        private static String[] ExtractSentenseEnd(String text)
        {
            return text.Split(new[] { "[ENDN]" }, StringSplitOptions.None);
        }
    }
}