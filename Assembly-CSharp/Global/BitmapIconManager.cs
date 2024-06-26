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
        GameObject gameObject;
        if (dialogImg.IsButton)
        {
            gameObject = FF9UIDataTool.ButtonGameObject((Control)dialogImg.Id, dialogImg.checkFromConfig, dialogImg.tag);
        }
        else
        {
            gameObject = FF9UIDataTool.IconGameObject(dialogImg.Id);
        }
        if (gameObject != (UnityEngine.Object)null)
        {
            gameObject.transform.parent = label.transform;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = new Vector3(dialogImg.LocalPosition.x, dialogImg.LocalPosition.y, 0f);
            gameObject.SetActive(true);
        }
        return gameObject;
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
