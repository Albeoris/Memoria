using System;
using UnityEngine;

public class QuadMistDialogResourceMapper : MonoBehaviour
{
    private void Start()
    {
    }

    private void MapSprite(SpriteRenderer spr, String name)
    {
        if (spr != (UnityEngine.Object)null)
        {
            spr.sprite = QuadMistResourceManager.Instance.GetSprite(name);
        }
    }

    public SpriteRenderer quadMistConfirmDialog;

    public SpriteRenderer quadMistStockDialog;

    public SpriteRenderer quadMistGetCardDialog;

    public SpriteRenderer quadMistCardNameDialog;
}
