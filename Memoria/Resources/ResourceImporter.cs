using System;
using UnityEngine;

namespace Memoria
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

                FF9TextToolInterceptor.BattleImporter.InitializeAsync();
                FF9TextToolInterceptor.FieldImporter.InitializeAsync();
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

        public static string GetURLForImport()
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

        public static string FormatUrl(String path)
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
            WWW videoStreamer = new WWW(FormatUrl(path));
            return videoStreamer.texture;
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
}