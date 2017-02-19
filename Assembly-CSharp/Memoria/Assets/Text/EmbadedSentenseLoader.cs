using System;
using UnityEngine;

namespace Memoria.Assets
{
    public static class EmbadedSentenseLoader
    {
        public static String[] LoadSentense(String path)
        {
            String text = AssetManager.Load<TextAsset>(path)?.text;
            return text == null ? null : ExtractSentenseEnd(text);
        }

        public static String LoadText(String path)
        {
            return AssetManager.Load<TextAsset>(path)?.text;
        }

        private static String[] ExtractSentenseEnd(String text)
        {
            return text.Split(new[] { "[ENDN]" }, StringSplitOptions.None);
        }
    }
}