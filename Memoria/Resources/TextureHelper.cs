using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Memoria
{
    public static class TextureHelper
    {
        public static Texture2D GetFragment(Texture2D texture, int x, int y, int width, int height)
        {
            if (texture == null)
                return null;

            Texture2D result = new Texture2D(width, height, texture.format, false);
            Color[] colors = texture.GetPixels(x, y, width, height);
            result.SetPixels(colors);
            result.Apply();
            return result;
        }

        public static Texture2D CopyAsReadable(Texture texture)
        {
            if (texture == null)
                return null;

            RenderTexture oldTarget = Camera.main.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
            try
            {
                Camera.main.targetTexture = rt;
                //Camera.main.Render();
                Graphics.Blit(texture, rt);

                RenderTexture.active = rt;
                result.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            }
            finally
            {
                RenderTexture.active = oldActive;
                Camera.main.targetTexture = oldTarget;
                RenderTexture.ReleaseTemporary(rt);
            }
            return result;
        }

        public static void WriteTextureToFile(Texture2D texture, String outputPath)
        {
            if (texture == null)
            {
                Log.Warning("[TextureResources] Cannot export texure. Texture is null. Stacktrace: " + Environment.StackTrace);
                return;
            }

            Byte[] data = texture.EncodeToPNG();
            File.WriteAllBytes(outputPath, data);
        }
    }
}