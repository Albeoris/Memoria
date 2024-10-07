using System;
using UnityEngine;
using Memoria.Assets;

public class SlideshowUI : MonoBehaviour
{
    public void SetupEndingText()
    {
        String languageSymbol = Localization.GetSymbol().ToLower();
        if (languageSymbol == "uk")
            languageSymbol = "us";
        GameObject endingText = AssetManager.LoadInArchive<GameObject>(languageSymbol == "us" || languageSymbol == "jp" || languageSymbol == "gr" ? ATLAS_PATH_US_JP_GR : ATLAS_PATH_FR_IT_ES, false);
        UIAtlas endingTextAtlas = endingText.GetComponent<UIAtlas>();
        this.text1Sprite.atlas = endingTextAtlas;
        this.text2Sprite.atlas = endingTextAtlas;
        this.text3Sprite.atlas = endingTextAtlas;
        this.text4Sprite.atlas = endingTextAtlas;
        this.text5Sprite.atlas = endingTextAtlas;
        this.text6Sprite.atlas = endingTextAtlas;
        this.text7Sprite.atlas = endingTextAtlas;
        this.text8Sprite.atlas = endingTextAtlas;
        this.text1Sprite.spriteName = "ending_text_01_" + languageSymbol;
        this.text2Sprite.spriteName = "ending_text_02_" + languageSymbol;
        this.text3Sprite.spriteName = "ending_text_03_" + languageSymbol;
        this.text4Sprite.spriteName = "ending_text_04_" + languageSymbol;
        this.text5Sprite.spriteName = "ending_text_05_" + languageSymbol;
        this.text6Sprite.spriteName = "ending_text_06_" + languageSymbol;
        this.text7Sprite.spriteName = "ending_text_07_" + languageSymbol;
        this.text8Sprite.spriteName = "ending_text_07_bg_" + languageSymbol;
        this.text1Sprite.alpha = 0f;
        this.text2Sprite.alpha = 0f;
        this.text3Sprite.alpha = 0f;
        this.text4Sprite.alpha = 0f;
        this.text5Sprite.alpha = 0f;
        this.text6Sprite.alpha = 0f;
        this.text7Sprite.alpha = 0f;
        this.text8Sprite.alpha = 0f;
        this.text1Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text2Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text3Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text4Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text5Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text6Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text7Sprite.GetComponent<HonoFading>().widescreenRescale = false;
        this.text8Sprite.GetComponent<HonoFading>().widescreenRescale = false;
    }

    public void PlayEndingText(UIScene.SceneVoidDelegate action = null)
    {
        this.text1Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
        this.text2Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
        this.text3Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
        this.text4Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
        this.text5Sprite.GetComponent<HonoFading>().FadePingPong(null, action);
        this.text6Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
        this.text7Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
        this.text8Sprite.GetComponent<HonoFading>().FadePingPong(null, null);
    }

    public void SetupLastEndingText()
    {
        this.lastEndingTextContainer.alpha = 0f;
    }

    public void PlayLastEndingText(UIScene.SceneVoidDelegate callback = null)
    {
        this.lastEndingTextContainer.GetComponent<HonoFading>().FadeIn(callback);
    }

    private const String ATLAS_PATH_US_JP_GR = "EmbeddedAsset/UI/Atlas/Ending_Text_US_JP_GR_Atlas";
    private const String ATLAS_PATH_FR_IT_ES = "EmbeddedAsset/UI/Atlas/Ending_Text_FR_IT_ES_Atlas";

    public UISprite text1Sprite; // "How did you survive...?"
    public UISprite text2Sprite; // "I didn't have a choice."
    public UISprite text3Sprite; // "I had to live."
    public UISprite text4Sprite; // "I wanted to come home to you."
    public UISprite text5Sprite; // "So..."
    public UISprite text6Sprite; // "I sang your song."
    public UISprite text7Sprite; // "Our song"
    public UISprite text8Sprite; // "Our song" (fading to cinematic's)
    public UIWidget lastEndingTextContainer;
}
