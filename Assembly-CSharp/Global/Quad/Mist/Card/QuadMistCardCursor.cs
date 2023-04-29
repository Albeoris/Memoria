using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMistCardCursor : MonoBehaviour
{
	private void Start()
	{
		LoadSprites("EmbeddedAsset/QuadMist/Atlas/quadmist_image0");
		gameObjects[0].GetComponent<Renderer>().enabled = true;
		for (Int32 i = 1; i < CURSOR_COUNT; i++)
			gameObjects[i].GetComponent<Renderer>().enabled = false;
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
				timeBeforeNextFrame = ANIMATION_TIME - timeBeforeNextFrame;
				Int32 num = animationIndex;
				if (animationIndex < 6)
					animationIndex++;
				else
					animationIndex = 0;
				gameObjects[num].GetComponent<Renderer>().enabled = false;
				gameObjects[animationIndex].GetComponent<Renderer>().enabled = true;
			}
		}
	}

	public void Hide()
	{
		if (!isShow)
			return;
		for (Int32 i = 0; i < CURSOR_COUNT; i++)
			gameObjects[i].GetComponent<Renderer>().enabled = false;
		cursorEffect.GetComponent<Renderer>().enabled = false;
		isShow = false;
	}

	public void Show()
	{
		if (isShow)
			return;
		for (Int32 i = 0; i < CURSOR_COUNT; i++)
			gameObjects[i].GetComponent<Renderer>().enabled = true;
		if (state == QuadMistCardCursor.State.Normal)
			cursorEffect.GetComponent<Renderer>().enabled = false;
		else if (state == QuadMistCardCursor.State.Black)
			cursorEffect.GetComponent<Renderer>().enabled = true;
		isShow = true;
	}

	public void SetBlackState()
	{
		if (cursorEffect == null)
			return;
		cursorEffect.GetComponent<Renderer>().enabled = true;
		state = QuadMistCardCursor.State.Black;
	}

	public void SetNormalState()
	{
		if (cursorEffect == null)
			return;
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
		Dictionary<String, Sprite> nameToSprite = new Dictionary<String, Sprite>();
		List<String> cursorSpriteNames = new List<String>();
		Texture2D moddedAtlas = null;
		String atlasOnDisc = AssetManager.SearchAssetOnDisc(atlasName, true, false);
		if (!String.IsNullOrEmpty(atlasOnDisc))
			moddedAtlas = AssetManager.LoadFromDisc<Texture2D>(atlasOnDisc, "");
		for (Int32 i = 0; i < CURSOR_COUNT; i++)
			cursorSpriteNames.Add($"card_cursor_{i}.png");
		for (Int32 i = 0; i < spriteArray.Length; i++)
		{
			Sprite sprite = spriteArray[i];
			if (cursorSpriteNames.Contains(sprite.name))
			{
				Sprite value = Sprite.Create(moddedAtlas != null ? moddedAtlas : sprite.texture, sprite.rect, new Vector2(0f, 0f), 482f);
				nameToSprite.Add(sprite.name, value);
			}
		}
		for (Int32 i = 0; i < spriteArray.Length; i++)
		{
			Sprite sprite = spriteArray[i];
			if (sprite.name == "card_cursor_0.png")
			{
				Sprite value = Sprite.Create(moddedAtlas != null ? moddedAtlas : sprite.texture, sprite.rect, new Vector2(0f, 0f), 482f);
				nameToSprite.Add("card_effect", value);
				break;
			}
		}
		gameObjects = new GameObject[CURSOR_COUNT];
		for (Int32 i = 0; i < CURSOR_COUNT; i++)
		{
			GameObject spriteGo = new GameObject($"CardCursor{i}");
			SpriteRenderer spriteRenderer = spriteGo.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
			spriteRenderer.sprite = nameToSprite[$"card_cursor_{i}.png"];
			spriteGo.transform.parent = base.transform;
			spriteGo.transform.localPosition = default(Vector3);
			gameObjects[i] = spriteGo;
		}
		cursorEffect = new GameObject("CursorEffect");
		SpriteRenderer effectRenderer = cursorEffect.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		effectRenderer.sprite = nameToSprite["card_effect"];
		cursorEffect.transform.parent = base.transform;
		cursorEffect.transform.localPosition = new Vector3(0f, 0f, -0.1f);
		effectRenderer.color = new Color(0f, 0f, 0f, 0.5f);
	}

	private const Int32 CURSOR_COUNT = 7;
	private const Single ANIMATION_TIME = 0.1f;

	private GameObject[] gameObjects;

	private Int32 animationIndex;
	private Single timeBeforeNextFrame = ANIMATION_TIME;

	private GameObject cursorEffect;

	private Boolean isShow = true;
	private QuadMistCardCursor.State state;

	private enum State
	{
		Normal,
		Black
	}
}
