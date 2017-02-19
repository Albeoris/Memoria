using System;
using UnityEngine;

public class QuadMistGameResourceMapper : MonoBehaviour
{
	private void Start()
	{
		this.MapSprite(this.cardTrayBg, "card_tray_bg.png");
		this.MapSprite(this.cardTrayBottom, "card_tray_bottom.png");
		this.MapSprite(this.cardTrayBox, "card_tray_box.png");
		this.MapSprite(this.cardTrayLeft, "card_tray_left.png");
		this.MapSprite(this.cardTrayRight, "card_tray_right.png");
		this.MapSprite(this.cardTryTop, "card_tray_top.png");
		this.MapSprite(this.comboText, "text_combo.png");
		this.MapSprite(this.cardToggleButton, "text_name.png");
		this.MapSprite(this.cardToggleButtonHilight, "text_name_hilight.png");
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
