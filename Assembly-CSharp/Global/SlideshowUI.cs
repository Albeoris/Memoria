using System;
using UnityEngine;

public class SlideshowUI : MonoBehaviour
{
	public void SetupEndingText()
	{
		String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
		GameObject gameObject;
		if (currentLanguage == "English(UK)" || currentLanguage == "English(US)" || currentLanguage == "Japanese" || currentLanguage == "German")
		{
			gameObject = AssetManager.Load<GameObject>("EmbeddedAsset/UI/Atlas/Ending_Text_US_JP_GR_Atlas", out _, false);
		}
		else
		{
			gameObject = AssetManager.Load<GameObject>("EmbeddedAsset/UI/Atlas/Ending_Text_FR_IT_ES_Atlas", out _, false);
		}
		UIAtlas component = gameObject.GetComponent<UIAtlas>();
		this.text1Sprite.atlas = component;
		this.text2Sprite.atlas = component;
		this.text3Sprite.atlas = component;
		this.text4Sprite.atlas = component;
		this.text5Sprite.atlas = component;
		this.text6Sprite.atlas = component;
		this.text7Sprite.atlas = component;
		this.text8Sprite.atlas = component;
		this.text1Sprite.spriteName = "ending_text_01" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text2Sprite.spriteName = "ending_text_02" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text3Sprite.spriteName = "ending_text_03" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text4Sprite.spriteName = "ending_text_04" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text5Sprite.spriteName = "ending_text_05" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text6Sprite.spriteName = "ending_text_06" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text7Sprite.spriteName = "ending_text_07" + this.GetLocalizeNameSubfix(currentLanguage);
		this.text8Sprite.spriteName = "ending_text_07_bg" + this.GetLocalizeNameSubfix(currentLanguage);
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

	private String GetLocalizeNameSubfix(String language)
	{
		String empty = String.Empty;
		switch (language)
		{
		case "English(UK)":
		case "English(US)":
			return "_us";
		case "German":
			return "_gr";
		case "Spanish":
			return "_es";
		case "French":
			return "_fr";
		case "Italian":
			return "_it";
		case "Japanese":
			return "_jp";
		}
		return "_us";
	}

	public void PlayEndingText(UIScene.SceneVoidDelegate action = null)
	{
		this.text1Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
		this.text2Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
		this.text3Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
		this.text4Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
		this.text5Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, action);
		this.text6Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
		this.text7Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
		this.text8Sprite.GetComponent<HonoFading>().FadePingPong((UIScene.SceneVoidDelegate)null, (UIScene.SceneVoidDelegate)null);
	}

	public void SetupLastEndingText()
	{
		this.lastEndingTextContainer.alpha = 0f;
	}

	public void PlayLastEndingText(UIScene.SceneVoidDelegate callback = null)
	{
		this.lastEndingTextContainer.GetComponent<HonoFading>().FadeIn(callback);
	}

	private const String endingTextAtlasName_us_jp_gr = "Ending_Text_US_JP_GR_Atlas";

	private const String endingTextAtlasName_fr_it_es = "Ending_Text_FR_IT_ES_Atlas";

	public UISprite text1Sprite;

	public UISprite text2Sprite;

	public UISprite text3Sprite;

	public UISprite text4Sprite;

	public UISprite text5Sprite;

	public UISprite text6Sprite;

	public UISprite text7Sprite;

	public UISprite text8Sprite;

	public UIWidget lastEndingTextContainer;
}
