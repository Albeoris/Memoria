using System;
using UnityEngine;

namespace Memoria
{
    public static class EmbadedSentenseLoader
    {
        public static String[] LoadSentense(String path)
        {
            String text = AssetManager.Load<TextAsset>(path)?.text;
            return text == null ? null : FF9TextToolInterceptor.ExtractSentenseEnd(text);
        }

        public static String LoadText(String path)
        {
            return AssetManager.Load<TextAsset>(path)?.text;
        }
    }
}