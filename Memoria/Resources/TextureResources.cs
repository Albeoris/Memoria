using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Memoria
{
    public static class TextureResources
    {
        public static void WriteTextureToFile(Texture2D texture, String outputPath)
        {
            if (texture == null)
            {
                Log.Warning("[TextureResources] Cannot export texure. Texture is null. Stacktrace: " + Environment.StackTrace);
                return;
            }

            Texture2D tmp = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture rt = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
            if (!rt.IsCreated())
                rt.Create();

            Camera.main.targetTexture = rt;
            Camera.main.Render();
            Graphics.Blit(texture, rt);
            RenderTexture.active = rt;
            tmp.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            Byte[] data = tmp.EncodeToPNG();

            String directoryPath = Path.GetDirectoryName(outputPath);
            if (directoryPath != null)
                Directory.CreateDirectory(directoryPath);

            File.WriteAllBytes(outputPath, data);
        }

        public static void WriteTextureToFile(Material material, String outputPath)
        {
            if (material == null)
            {
                Log.Warning("[TextureResources] Cannot export texure. Texture is null. Stacktrace: " + Environment.StackTrace);
                return;
            }

            var texture = material.mainTexture;

            Texture2D tmp = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture rt = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
            if (!rt.IsCreated())
                rt.Create();

            Camera.main.targetTexture = rt;
            Camera.main.Render();
            Graphics.Blit(texture, rt, material);
            RenderTexture.active = rt;
            tmp.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            Byte[] data = tmp.EncodeToPNG();

            String directoryPath = Path.GetDirectoryName(outputPath);
            if (directoryPath != null)
                Directory.CreateDirectory(directoryPath);

            File.WriteAllBytes(outputPath, data);
        }
    }
}