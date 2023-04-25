using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMistCardCursor : MonoBehaviour
{
	private void Start()
	{
		LoadSprites("EmbeddedAsset/QuadMist/Atlas/quadmist_image0");
		gameObjects[0].GetComponent<Renderer>().enabled = true;
		for (Int32 i = 1; i < 7; i++)
		{
			gameObjects[i].GetComponent<Renderer>().enabled = false;
		}
		SetNormalState();
		Hide();
	}

	private void Update()
	{
		if (isShow)
		{
			timeBeforeNextFrame -= Time.deltaTime;
			if (timeBeforeNextFrame <= 0f)
			{
				timeBeforeNextFrame = 0.1f - timeBeforeNextFrame;
				Int32 num = animationIndex;
				if (animationIndex < 6)
				{
					animationIndex++;
				}
				else
				{
					animationIndex = 0;
				}
				gameObjects[num].GetComponent<Renderer>().enabled = false;
				gameObjects[animationIndex].GetComponent<Renderer>().enabled = true;
			}
		}
	}

	public void Hide()
	{
		if (!isShow)
		{
			return;
		}
		for (Int32 i = 0; i < 7; i++)
		{
			gameObjects[i].GetComponent<Renderer>().enabled = false;
		}
		cursorEffect.GetComponent<Renderer>().enabled = false;
		isShow = false;
	}

	public void Show()
	{
		if (isShow)
		{
			return;
		}
		for (Int32 i = 0; i < 7; i++)
		{
			gameObjects[i].GetComponent<Renderer>().enabled = true;
		}
		if (state == QuadMistCardCursor.State.Normal)
		{
			cursorEffect.GetComponent<Renderer>().enabled = false;
		}
		else if (state == QuadMistCardCursor.State.Black)
		{
			cursorEffect.GetComponent<Renderer>().enabled = true;
		}
		isShow = true;
	}

	public void SetBlackState()
	{
		if (cursorEffect == (UnityEngine.Object)null)
		{
			return;
		}
		cursorEffect.GetComponent<Renderer>().enabled = true;
		state = QuadMistCardCursor.State.Black;
	}

	public void SetNormalState()
	{
		if (cursorEffect == (UnityEngine.Object)null)
		{
			return;
		}
		cursorEffect.GetComponent<Renderer>().enabled = false;
		state = QuadMistCardCursor.State.Normal;
	}

	public void ForceHide()
	{
		isShow = true;
		Hide();
	}

	private void LoadSprites(String atlasName)
	{
        Sprite[] spriteArray = Resources.LoadAll<Sprite>(atlasName);
        Dictionary<String, Sprite> dictionary = new Dictionary<String, Sprite>();
        List<String> list = new List<String>();
        Texture2D moddedAtlas = null;
        String atlasOnDisc = AssetManager.SearchAssetOnDisc(atlasName, true, false);
        if (!String.IsNullOrEmpty(atlasOnDisc))
            moddedAtlas = AssetManager.LoadFromDisc<Texture2D>(atlasOnDisc, "");
        for (Int32 i = 0; i <= 6; i++)
            list.Add("card_cursor_" + i + ".png");
		for (Int32 j = 0; j < spriteArray.Length; j++)
		{
			Sprite sprite = spriteArray[j];
			if (list.Contains(sprite.name))
			{
				Sprite value = Sprite.Create(moddedAtlas != null ? moddedAtlas : sprite.texture, sprite.rect, new Vector2(0f, 0f), 482f);
				dictionary.Add(sprite.name, value);
			}
		}
		for (Int32 k = 0; k < spriteArray.Length; k++)
		{
			Sprite sprite = spriteArray[k];
			if (sprite.name == "card_cursor_0.png")
			{
				Sprite value = Sprite.Create(moddedAtlas != null ? moddedAtlas : sprite.texture, sprite.rect, new Vector2(0f, 0f), 482f);
				dictionary.Add("card_effect", value);
				break;
			}
		}
		gameObjects = new GameObject[7];
		for (Int32 l = 0; l < 7; l++)
		{
			GameObject gameObject = new GameObject("CardCursor" + l);
			SpriteRenderer spriteRenderer = gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
			spriteRenderer.sprite = dictionary["card_cursor_" + l + ".png"];
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = default(Vector3);
			gameObjects[l] = gameObject;
		}
		cursorEffect = new GameObject("CursorEffect");
		SpriteRenderer spriteRenderer2 = cursorEffect.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		spriteRenderer2.sprite = dictionary["card_effect"];
		cursorEffect.transform.parent = base.transform;
		cursorEffect.transform.localPosition = new Vector3(0f, 0f, -0.1f);
		spriteRenderer2.color = new Color(0f, 0f, 0f, 0.5f);
	}

	private const Int32 CURSOR_COUNT = 7;

	private const Single ANIMATION_TIME = 0.1f;

	private GameObject[] gameObjects;

	private Int32 animationIndex;

	private Single timeBeforeNextFrame = 0.1f;

	private GameObject cursorEffect;

	private Boolean isShow = true;

	private QuadMistCardCursor.State state;

	private enum State
	{
		Normal,
		Black
	}
}
