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
        GameObject imgGo;
        if (!String.IsNullOrEmpty(dialogImg.SpriteName))
            imgGo = FF9UIDataTool.SpriteGameObject(dialogImg.AtlasName, dialogImg.SpriteName);
        else if (dialogImg.IsButton)
            imgGo = FF9UIDataTool.ButtonGameObject((Control)dialogImg.Id, dialogImg.checkFromConfig, dialogImg.tag);
        else
            imgGo = FF9UIDataTool.IconGameObject(dialogImg.Id);
        if (imgGo == null)
        {
            Log.Warning("[BitmapIconManager] Could not create image: " + (!String.IsNullOrEmpty(dialogImg.SpriteName) ? dialogImg.AtlasName + ":" + dialogImg.SpriteName : (dialogImg.IsButton ? "Button " + dialogImg.Id : "Icon " + dialogImg.Id)));
            return null;
        }
        imgGo.transform.parent = label.transform;
        dialogImg.SpriteGo = imgGo;
        return imgGo;
    }

    public void PlaceBitmapIcon(DialogImage dialogImg)
    {
        GameObject imgGo = dialogImg.SpriteGo;
        if (imgGo == null)
            return;
        if (dialogImg.Rescale)
        {
            Vector2 defaultSize = FF9UIDataTool.GetSpriteSize(dialogImg.AtlasName, dialogImg.SpriteName);
            imgGo.transform.localScale = new Vector3(dialogImg.Size.x / defaultSize.x, dialogImg.Size.y / defaultSize.y, 1f);
        }
        else
        {
            imgGo.transform.localScale = Vector3.one;
        }
        imgGo.transform.localPosition = new Vector3(dialogImg.LocalPosition.x, dialogImg.LocalPosition.y, 0f);
        UISprite sprite = imgGo.GetComponent<UISprite>();
        if (sprite != null)
        {
            if (dialogImg.Mirror)
                sprite.flip = UIBasicSprite.Flip.Horizontally;
            else
                sprite.flip = UIBasicSprite.Flip.Nothing;
        }
        imgGo.SetActive(true);
    }

    public void RemoveBitmapIcon(GameObject bitmap)
    {
        FF9UIDataTool.ReleaseBitmapIconToPool(bitmap);
    }

    public void RemoveAllBitmapIcon()
    {
        FF9UIDataTool.ReleaseAllTypeBitmapIconsToPool();
    }
}
