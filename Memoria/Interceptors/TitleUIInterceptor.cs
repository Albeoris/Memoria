using System;
using System.Linq;

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
            //TextureResources.WriteTextureToFile(texture, "SavedScreen.png");
        }
    }
}