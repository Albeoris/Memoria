using System;
using System.IO;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    private const String CUSTOM_SPLASH_PATH = "SplashTitle.png";

    private void Awake()
    {
        global::Debug.Log("20 SplashScreen.Awake");
        splashSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        UnityEngine.Object.DontDestroyOnLoad(this);
        Boolean forceUsingIOS = false;
        if (Application.platform == RuntimePlatform.IPhonePlayer || forceUsingIOS)
            Application.LoadLevel("Bundle-iOS");
        else
            Application.LoadLevel("Bundle");

        // Use a custom texture for the splash screen
        // TODO: currently, this is done before AssetManager (and mod setups) are initialised, so only the file in the game's base folder is taken into account
        if (File.Exists(CUSTOM_SPLASH_PATH))
        {
            Texture2D pngTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            if (pngTexture.LoadImage(File.ReadAllBytes(CUSTOM_SPLASH_PATH)))
                splashSprite.sprite = Sprite.Create(pngTexture, new Rect(0f, 0f, pngTexture.width, pngTexture.height), new Vector2(0.5f, 0.5f));
        }
    }

    [NonSerialized]
    private SpriteRenderer splashSprite;
}
