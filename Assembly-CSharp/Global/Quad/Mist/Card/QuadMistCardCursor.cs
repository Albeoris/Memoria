using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMistCardCursor : MonoBehaviour
{
	private void Start()
	{
		this.LoadSprites("EmbeddedAsset/QuadMist/Atlas/quadmist_image0");
		this.gameObjects[0].GetComponent<Renderer>().enabled = true;
		for (Int32 i = 1; i < 7; i++)
		{
			this.gameObjects[i].GetComponent<Renderer>().enabled = false;
		}
		this.SetNormalState();
		this.Hide();
	}

	private void Update()
	{
		if (this.isShow)
		{
			this.timeBeforeNextFrame -= Time.deltaTime;
			if (this.timeBeforeNextFrame <= 0f)
			{
				this.timeBeforeNextFrame = 0.1f - this.timeBeforeNextFrame;
				Int32 num = this.animationIndex;
				if (this.animationIndex < 6)
				{
					this.animationIndex++;
				}
				else
				{
					this.animationIndex = 0;
				}
				this.gameObjects[num].GetComponent<Renderer>().enabled = false;
				this.gameObjects[this.animationIndex].GetComponent<Renderer>().enabled = true;
			}
		}
	}

	public void Hide()
	{
		if (!this.isShow)
		{
			return;
		}
		for (Int32 i = 0; i < 7; i++)
		{
			this.gameObjects[i].GetComponent<Renderer>().enabled = false;
		}
		this.cursorEffect.GetComponent<Renderer>().enabled = false;
		this.isShow = false;
	}

	public void Show()
	{
		if (this.isShow)
		{
			return;
		}
		for (Int32 i = 0; i < 7; i++)
		{
			this.gameObjects[i].GetComponent<Renderer>().enabled = true;
		}
		if (this.state == QuadMistCardCursor.State.Normal)
		{
			this.cursorEffect.GetComponent<Renderer>().enabled = false;
		}
		else if (this.state == QuadMistCardCursor.State.Black)
		{
			this.cursorEffect.GetComponent<Renderer>().enabled = true;
		}
		this.isShow = true;
	}

	public void SetBlackState()
	{
		if (this.cursorEffect == (UnityEngine.Object)null)
		{
			return;
		}
		this.cursorEffect.GetComponent<Renderer>().enabled = true;
		this.state = QuadMistCardCursor.State.Black;
	}

	public void SetNormalState()
	{
		if (this.cursorEffect == (UnityEngine.Object)null)
		{
			return;
		}
		this.cursorEffect.GetComponent<Renderer>().enabled = false;
		this.state = QuadMistCardCursor.State.Normal;
	}

	public void ForceHide()
	{
		this.isShow = true;
		this.Hide();
	}

	private void LoadSprites(String atlasName)
	{
		Sprite[] spriteArray = Resources.LoadAll<Sprite>(atlasName);
		Dictionary<String, Sprite> dictionary = new Dictionary<String, Sprite>();
		List<String> list = new List<String>();
		Texture2D moddedAtlas = null;
		String atlasOnDisc = AssetManager.SearchAssetOnDisc(atlasName, true, false);
		if (!String.IsNullOrEmpty(atlasOnDisc))
		{
			String[] atlasInfo = new String[0];
			moddedAtlas = AssetManager.LoadFromDisc<Texture2D>(atlasOnDisc, ref atlasInfo, "");
		}
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
		this.gameObjects = new GameObject[7];
		for (Int32 l = 0; l < 7; l++)
		{
			GameObject gameObject = new GameObject("CardCursor" + l);
			SpriteRenderer spriteRenderer = gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
			spriteRenderer.sprite = dictionary["card_cursor_" + l + ".png"];
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = default(Vector3);
			this.gameObjects[l] = gameObject;
		}
		this.cursorEffect = new GameObject("CursorEffect");
		SpriteRenderer spriteRenderer2 = this.cursorEffect.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		spriteRenderer2.sprite = dictionary["card_effect"];
		this.cursorEffect.transform.parent = base.transform;
		this.cursorEffect.transform.localPosition = new Vector3(0f, 0f, -0.1f);
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
