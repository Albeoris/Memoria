using Assets.Sources.Scripts.UI.Common;
using System;
using UnityEngine;
using Memoria.Prime;

internal class BitmapIconManager : Singleton<BitmapIconManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public GameObject InsertBitmapIcon(DialogImage dialogImg, GameObject label)
    {
        if (dialogImg.SpriteGo != null)
        {
            PlaceBitmapIcon(dialogImg);
            return dialogImg.SpriteGo;
        }
        if (!String.IsNullOrEmpty(dialogImg.SpriteName))
            dialogImg.SpriteGo = FF9UIDataTool.SpriteGameObject(dialogImg.AtlasName, dialogImg.SpriteName);
        else if (dialogImg.IsButton)
            dialogImg.SpriteGo = FF9UIDataTool.ButtonGameObject((Control)dialogImg.Id, dialogImg.checkFromConfig, dialogImg.tag);
        else
            dialogImg.SpriteGo = FF9UIDataTool.IconGameObject(dialogImg.Id);
        if (dialogImg.SpriteGo == null)
        {
            Log.Warning("[BitmapIconManager] Could not create image: " + (!String.IsNullOrEmpty(dialogImg.SpriteName) ? dialogImg.AtlasName + ":" + dialogImg.SpriteName : (dialogImg.IsButton ? "Button " + dialogImg.Id : "Icon " + dialogImg.Id)));
            return null;
        }
        dialogImg.SpriteGo.transform.parent = label.transform;
        PlaceBitmapIcon(dialogImg);
        return dialogImg.SpriteGo;
    }

    public void PlaceBitmapIcon(DialogImage dialogImg)
    {
        GameObject imgGo = dialogImg.SpriteGo;
        if (imgGo == null)
            return;
        imgGo.SetActive(true);
        if (dialogImg.Rescale)
        {
            Vector2 defaultSize = FF9UIDataTool.GetSpriteSize(dialogImg.AtlasName, dialogImg.SpriteName);
            imgGo.transform.localScale = new Vector3(dialogImg.Size.x / defaultSize.x, dialogImg.Size.y / defaultSize.y, 1f);
        }
        else
        {
            imgGo.transform.localScale = Vector3.one;
        }
        imgGo.transform.localPosition = new Vector3(dialogImg.LocalPosition.x, -dialogImg.LocalPosition.y, 0f);
        UISprite sprite = imgGo.GetComponent<UISprite>();
        if (sprite != null)
        {
            if (dialogImg.Mirror)
                sprite.flip = UIBasicSprite.Flip.Horizontally;
            else
                sprite.flip = UIBasicSprite.Flip.Nothing;
        }
        UILabel keyLabel = imgGo.transform.childCount > 0 ? imgGo.GetChild(0).GetComponent<UILabel>() : null;
        if (keyLabel != null && dialogImg.Mirror && keyLabel.rawText.Length >= 1 && keyLabel.rawText[0] != '[')
            keyLabel.rawText = "[MIRR]" + keyLabel.rawText;
    }

    public void SetBitmapIconAlpha(GameObject imgGo, Single alpha)
    {
        if (imgGo == null)
            return;
        UISprite sprite = imgGo.GetComponent<UISprite>();
        if (sprite != null && sprite.alpha != alpha)
            sprite.alpha = alpha;
        UILabel keyLabel = imgGo.transform.childCount > 0 ? imgGo.GetChild(0).GetComponent<UILabel>() : null;
        if (keyLabel != null && keyLabel.alpha != alpha)
            keyLabel.alpha = alpha;
    }

    public void RemoveBitmapIcon(GameObject bitmap)
    {
        SetBitmapIconAlpha(bitmap, 1f);
        FF9UIDataTool.ReleaseBitmapIconToPool(bitmap);
    }

    public void RemoveAllBitmapIcon()
    {
        FF9UIDataTool.ReleaseAllTypeBitmapIconsToPool();
    }
}
