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
                TextAsset textAsset = Resources.Load<TextAsset>(path);
                Byte[] binary = ByteEncryption.Decryption(textAsset.bytes);
                fontBundle = AssetBundle.CreateFromMemoryImmediate(binary);
            }
            defaultFont = LoadFont(fontBundle.LoadAsset<Font>("TBUDGoStd-Bold"));
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
                return Font.CreateDynamicFontFromOSFont(Configuration.Font.Names, Configuration.Font.Size);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FontInterceptor] Failed to create a dynamic font.");
                return null;
            }
        }
    }
}
