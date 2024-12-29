﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class PillarBoxOverrideManager : MonoBehaviour
{
    private void Start()
    {
        this.topRect = base.gameObject.FindChild("Top").GetComponent<RectTransform>();
        this.bottomRect = base.gameObject.FindChild("Bottom").GetComponent<RectTransform>();
        this.leftRect = base.gameObject.FindChild("Left").GetComponent<RectTransform>();
        this.rightRect = base.gameObject.FindChild("Right").GetComponent<RectTransform>();
        this.leftImage = base.gameObject.FindChild("Left").GetComponent<Image>();
        this.rightImage = base.gameObject.FindChild("Right").GetChild(0).GetComponent<Image>();
        if (AssetManager.HasAssetOnDisc("embeddedasset/ui/sprites/overlay canvas/letterbox_verticle", true, false))
        {
            // TODO: for now, the existence of a custom texture replaces the pillars by black bars, without loading the texture itself
            this.leftImage.gameObject.active = false;
            this.rightImage.gameObject.active = false;
        }
        this.UpdatePillarSize();
    }

    public void Restart()
    {
        this.UpdatePillarSize();
    }

    private void LateUpdate()
    {
    }

    private void UpdatePillarSize()
    {
        this.topRect.sizeDelta = new Vector2(0f, UIManager.UIPillarBoxSize.y);
        this.bottomRect.sizeDelta = new Vector2(0f, UIManager.UIPillarBoxSize.y);
        this.leftRect.sizeDelta = new Vector2(UIManager.UIPillarBoxSize.x, 0f);
        this.rightRect.sizeDelta = new Vector2(UIManager.UIPillarBoxSize.x, 0f);
    }

    private RectTransform topRect;
    private RectTransform bottomRect;
    private RectTransform leftRect;
    private RectTransform rightRect;

    private Image leftImage;
    private Image rightImage;
}
