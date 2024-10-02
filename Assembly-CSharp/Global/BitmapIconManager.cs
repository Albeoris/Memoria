using Assets.Sources.Scripts.UI.Common;
using System;
using UnityEngine;

internal class BitmapIconManager : Singleton<BitmapIconManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public GameObject InsertBitmapIcon(Dialog.DialogImage dialogImg, Dialog dialog)
    {
        return this.InsertBitmapIcon(dialogImg, dialog.PhraseLabel.gameObject);
    }

    public GameObject InsertBitmapIcon(Dialog.DialogImage dialogImg, GameObject label)
    {
        GameObject imgGo;
        if (!String.IsNullOrEmpty(dialogImg.SpriteName))
            imgGo = FF9UIDataTool.SpriteGameObject(dialogImg.AtlasName, dialogImg.SpriteName);
        else if (dialogImg.IsButton)
            imgGo = FF9UIDataTool.ButtonGameObject((Control)dialogImg.Id, dialogImg.checkFromConfig, dialogImg.tag);
        else
            imgGo = FF9UIDataTool.IconGameObject(dialogImg.Id);
        if (imgGo != null)
        {
            imgGo.transform.parent = label.transform;
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
            imgGo.SetActive(true);
        }
        return imgGo;
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
