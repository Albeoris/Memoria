using System;
using Memoria.Prime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Memoria.Assets
{
    public class EncryptFontManager
    {
        private static readonly Font DefaultFont = InitializeFont();

        static EncryptFontManager()
        {
            loadInEditMode = false;
        }

        public static Font LoadFont(Font originalFont)
        {
            Log.Message("[FontInterceptor] Loading font [Original: {0}]", originalFont);
            try
            {
                return DefaultFont ?? originalFont;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FontInterceptor] Failed to intercept font.");
                return originalFont;
            }
        }

        private static void LoadFont()
        {
            String path = "EmbeddedAsset/FA/" + AssetManagerUtil.GetPlatformPrefix(Application.platform) + "_fa.mpc";
            if (fontBundle == null)
            {
				String[] fontInfo;
                Byte[] binAsset = AssetManager.LoadBytes(path, out fontInfo, false);
                Byte[] binary = ByteEncryption.Decryption(binAsset);
                fontBundle = AssetBundle.CreateFromMemoryImmediate(binary);
            }
            if (Configuration.Font.Names.Length > 0 && EncryptFontManager.fontBundle.Contains(Configuration.Font.Names[0]))
                EncryptFontManager.defaultFont = EncryptFontManager.LoadFont(EncryptFontManager.fontBundle.LoadAsset<Font>(Configuration.Font.Names[0]));
            else
                EncryptFontManager.defaultFont = EncryptFontManager.LoadFont(EncryptFontManager.fontBundle.LoadAsset<Font>("TBUDGoStd-Bold"));
            fontBundle.Unload(false);
            Object.DestroyImmediate(fontBundle);
            fontBundle = null;
        }

        private static void StateChange()
        {
            if (!Application.isPlaying)
            {
                if (fontBundle != null)
                {
                    fontBundle.Unload(true);
                    Object.DestroyImmediate(fontBundle);
                    fontBundle = null;
                }
                defaultFont = null;
            }
        }

        public static Font SetDefaultFont()
        {
            if (defaultFont == null)
            {
                LoadFont();
            }
            return defaultFont;
        }

        public static Font defaultFont;

        private static AssetBundle fontBundle;

        private static Boolean loadInEditMode = true;

        private static Font InitializeFont()
        {
            Log.Message("[FontInterceptor] Dynamic font initialization.");
            try
            {
                if (!Configuration.Font.Enabled)
                {
                    Log.Message("[FontInterceptor] Pass through {Configuration.Font.Enabled = 0}.");
                    return null;
                }

                if (Configuration.Font.Names.IsNullOrEmpty())
                {
                    Log.Error("[FontInterceptor] An invalid font configuration [Configuration.Font.Names is empty].");
                    return null;
                }

                Log.Message("[FontInterceptor] Initialize new font: [Names: {{{0}}}, Size: {1}]", String.Join(", ", Configuration.Font.Names), Configuration.Font.Size);
                if (Configuration.Font.Names.Length > 0 && Array.Exists(Font.GetOSInstalledFontNames(), font => font.CompareTo(Configuration.Font.Names[0]) == 0))
                    return Font.CreateDynamicFontFromOSFont(Configuration.Font.Names, Configuration.Font.Size);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FontInterceptor] Failed to create a dynamic font.");
                return null;
            }
        }
    }
}
