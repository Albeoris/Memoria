using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Memoria
{
    // Don't call from Patcher!
    public static class TitleUIInterceptor
    {
        public static void Start(ref UISprite originalSource)
        {
            //Log.Message("Start" + Environment.StackTrace);
            //Log.Message("{0} {1}", originalSource.GetType(), originalSource);
            //Log.Message(originalSource.name);
            //Log.Message("Local: {0}", String.Join("|", originalSource.localCorners.Select(c=>c.ToString()).ToArray()));
            //Log.Message("World: {0}", String.Join("|", originalSource.worldCorners.Select(c=>c.ToString()).ToArray()));

            //Texture2D texture = (Texture2D)originalSource.mainTexture;
            //Log.Message(texture.name);
            //SetTextureImporterFormat(texture);
        }

        public static void SetTextureImporterFormat(Texture2D texture)
        {
            if (null == texture)
                return;

            RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            renderTexture.Create();

            Log.Message("Blit");
            Graphics.Blit(texture, renderTexture);

            Log.Message("Transfer");
            RenderTexture.active = renderTexture;
            Texture2D virtualPhoto = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            virtualPhoto.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            RenderTexture.active = null;

            Log.Message("Encdoing");
            byte[] data = virtualPhoto.EncodeToPNG();
            Log.Message("Encoded");
            File.WriteAllBytes("SavedScreen.png", data);
            Log.Message("Saved");
        }
    }
}