using System;
using System.IO;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Prime;
using UnityEngine;

namespace Memoria.Assets
{
    public static class ResourceImporter
    {
        public static void Initialize()
        {
            try
            {
                if (!Configuration.Import.Enabled)
                {
                    Log.Message("[ResourceImporter] Pass through {Configuration.Import.Enabled = 0}.");
                    return;
                }

                FF9TextTool.BattleImporter.InitializeAsync();
                FF9TextTool.FieldImporter.InitializeAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ResourceImporter] Failed to import resources.");
            }
        }

        public static Texture2D ImportTexture(String relativePath)
        {
            WWW videoStreamer = new WWW(GetURLForImport() + relativePath);
            return videoStreamer.texture;
        }

        public static String GetURLForImport()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.PSP2:
                    return "file://" + Configuration.Import.Path + '/';
                case RuntimePlatform.Android:
                    return "jar:file://" + Configuration.Import.Path + '/';
                default:
                    return String.Empty;
            }
        }

        public static String FormatUrl(String path)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.PSP2:
                    return "file://" + path;
                case RuntimePlatform.Android:
                    return "jar:file://" + path;
                default:
                    return String.Empty;
            }
        }
    }

    public static class StreamingResources
    {
        public static Texture2D LoadTexture2D(String path)
        {
            Byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.LoadImage(data);
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        public static String FormatUrl(String path)
        {
            path = Path.GetFullPath(path);
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.PSP2:
                    return "file://" + path;
                case RuntimePlatform.Android:
                    return "jar:file://" + path;
                default:
                    return String.Empty;
            }
        }
    }
}