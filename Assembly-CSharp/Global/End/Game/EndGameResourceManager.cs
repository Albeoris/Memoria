using System;
using Memoria.Assets;
using UnityEngine;

public class EndGameResourceManager : MonoBehaviour
{
	private void Start()
	{
		String quadMistTextAtlasPath = this.GetQuadMistTextAtlasPath();
		Sprite[] array = Resources.LoadAll<Sprite>(quadMistTextAtlasPath);
		Sprite[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			Sprite sprite = array2[i];
			if (String.Compare(sprite.name, "card_win.png") == 0)
			{
				this.winSprite = sprite;
			}
			else if (String.Compare(sprite.name, "card_win_shadow.png") == 0)
			{
				this.winShadowSprite = sprite;
			}
			else if (String.Compare(sprite.name, "card_lose.png") == 0)
			{
				this.loseSprite = sprite;
			}
			else if (String.Compare(sprite.name, "card_lose_shadow.png") == 0)
			{
				this.loseShadowSprite = sprite;
			}
			else if (String.Compare(sprite.name, "card_draw.png") == 0)
			{
				this.drawSprite = sprite;
			}
			else if (String.Compare(sprite.name, "card_draw_shadow.png") == 0)
			{
				this.drawShadowSprite = sprite;
			}
		}
		this.WinSpriteRenderer.sprite = this.ReCreateSpriteWithUpdatePixelPerUnity(this.winSprite);
		this.WinShadowSpriteRenderer.sprite = this.ReCreateSpriteWithUpdatePixelPerUnity(this.winShadowSprite);
		this.loseSpriteRenderer.sprite = this.ReCreateSpriteWithUpdatePixelPerUnity(this.loseSprite);
		this.loseShadowSpriteRenderer.sprite = this.ReCreateSpriteWithUpdatePixelPerUnity(this.loseShadowSprite);
		this.drawSpriteRenderer.sprite = this.ReCreateSpriteWithUpdatePixelPerUnity(this.drawSprite);
		this.drawShadowSpriteRenderer.sprite = this.ReCreateSpriteWithUpdatePixelPerUnity(this.drawShadowSprite);
	}

	private Sprite ReCreateSpriteWithUpdatePixelPerUnity(Sprite originalSprite)
	{
		return Sprite.Create(originalSprite.texture, originalSprite.rect, new Vector2(0f, 1f), 10f);
	}

	private String GetQuadMistTextAtlasPath()
	{
		String str = String.Empty;
		String language = Localization.language;
		switch (language)
		{
		case "English(US)":
			str = "quadmist_text_us";
			break;
		case "Japanese":
			str = "quadmist_text_jp";
			break;
		case "German":
			str = "quadmist_text_gr";
			break;
		case "Spanish":
			str = "quadmist_text_es";
			break;
		case "Italian":
			str = "quadmist_text_it";
			break;
		case "French":
			str = "quadmist_text_fr";
			break;
		case "English(UK)":
			str = "quadmist_text_uk";
			break;
		}
		return "EmbeddedAsset/QuadMist/Atlas/" + str;
	}

	private const String winSpriteName = "card_win.png";

	private const String winShadowSpriteName = "card_win_shadow.png";

	private const String loseSpriteName = "card_lose.png";

	private const String loseShadowSpriteName = "card_lose_shadow.png";

	private const String drawSpriteName = "card_draw.png";

	private const String drawShadowSpriteName = "card_draw_shadow.png";

	private Sprite winSprite;

	private Sprite winShadowSprite;

	private Sprite loseSprite;

	private Sprite loseShadowSprite;

	private Sprite drawSprite;

	private Sprite drawShadowSprite;

	public SpriteRenderer WinSpriteRenderer;

	public SpriteRenderer WinShadowSpriteRenderer;

	public SpriteRenderer loseSpriteRenderer;

	public SpriteRenderer loseShadowSpriteRenderer;

	public SpriteRenderer drawSpriteRenderer;

	public SpriteRenderer drawShadowSpriteRenderer;
}
