using System;
using UnityEngine;

public class QuadMistGameResourceMapper : MonoBehaviour
{
	private void Start()
	{
		MapSprite(cardTrayBg, "card_tray_bg.png");
		MapSprite(cardTrayBottom, "card_tray_bottom.png");
		MapSprite(cardTrayBox, "card_tray_box.png");
		MapSprite(cardTrayLeft, "card_tray_left.png");
		MapSprite(cardTrayRight, "card_tray_right.png");
		MapSprite(cardTryTop, "card_tray_top.png");
		MapSprite(comboText, "text_combo.png");
		MapSprite(cardToggleButton, "text_name.png");
		MapSprite(cardToggleButtonHilight, "text_name_hilight.png");
	}

	private void MapSprite(SpriteRenderer spr, String name)
	{
		if (spr != (UnityEngine.Object)null)
		{
			spr.sprite = QuadMistResourceManager.Instance.GetSprite(name);
		}
	}

	public SpriteRenderer cardTrayBg;

	public SpriteRenderer cardTrayBottom;

	public SpriteRenderer cardTrayBox;

	public SpriteRenderer cardTrayLeft;

	public SpriteRenderer cardTrayRight;

	public SpriteRenderer cardTryTop;

	public SpriteRenderer comboText;

	public SpriteRenderer cardSelect;

	public SpriteRenderer cardToggleButton;

	public SpriteRenderer cardToggleButtonHilight;
}
