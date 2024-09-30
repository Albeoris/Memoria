﻿using Memoria.Prime;
using System;
using System.IO;
using UnityEngine;

namespace Memoria.Assets
{
    public static class TextureHelper
    {
        public static Texture2D GetFragment(Texture2D texture, Int32 x, Int32 y, Int32 width, Int32 height)
        {
            if (texture == null)
                return null;

            Texture2D result = new Texture2D(width, height, texture.format, false);
            Color[] colors = texture.GetPixels(x, y, width, height);
            result.SetPixels(colors);
            result.Apply();
            return result;
        }

        public static Texture2D CopyAsReadable(Texture texture, Boolean destroyOld = false)
        {
            if (texture == null)
                return null;

            if (texture is Texture2D texture2d)
            {
                try
                {
                    Color firstPixel = texture2d.GetPixel(0, 0);
                    return texture2d;
                }
                catch (Exception) {}
            }

            Camera camera = Camera.main ?? GameObject.Find("FieldMap Camera")?.GetComponent<Camera>() ?? GameObject.Find("UI Camera")?.GetComponent<Camera>() ?? UICamera.mainCamera;
            RenderTexture oldTarget = camera.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
            try
            {
                camera.targetTexture = rt;
                //camera.Render();
                Graphics.Blit(texture, rt);

                RenderTexture.active = rt;
                result.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                if (destroyOld)
                    UnityEngine.Object.DestroyImmediate(texture);
            }
            finally
            {
                RenderTexture.active = oldActive;
                camera.targetTexture = oldTarget;
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
