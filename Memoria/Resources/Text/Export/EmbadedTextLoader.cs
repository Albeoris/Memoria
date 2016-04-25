using System;
using System.IO;
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

    public sealed class EmbadedTextLoader
    {
        public String FileName { get; }
        public String Text { get; }

        public static String[] ReadSentense(String relativePath, out String directory)
        {
            EmbadedTextLoader loader = new EmbadedTextLoader(relativePath);
            directory = Path.GetDirectoryName(loader.FileName)?.Replace("EmbeddedAsset", "StreamingAssets");
            if (loader.Text == null)
                return null;

            return FF9TextToolInterceptor.ExtractSentenseEnd(loader.Text);
        }

        public EmbadedTextLoader(String relativePath)
        {
            Log.Message("TextLoader: {0}", relativePath);
            FileName = Localization.GetPath() + relativePath;
            Text = AssetManager.Load<TextAsset>(FileName)?.text;
        }
    }
}