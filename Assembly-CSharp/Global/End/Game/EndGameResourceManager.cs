using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Assets;

public class EndGameResourceManager : MonoBehaviour
{
    private void Start()
    {
        String quadMistTextAtlasPath = GetQuadMistTextAtlasPath();
        Dictionary<String, Sprite> atlasSprites = new Dictionary<String, Sprite>();
        Sprite[] spriteArray = Resources.LoadAll<Sprite>(quadMistTextAtlasPath);
        foreach (Sprite sprite in spriteArray)
            atlasSprites.Add(sprite.name, sprite);
        foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
        {
            if (String.IsNullOrEmpty(folder.FolderPath))
                continue;
            if (folder.TryFindAssetInModOnDisc(quadMistTextAtlasPath, out String fullPath, AssetManagerUtil.GetResourcesAssetsPath(true) + "/"))
                UIAtlas.ReadRawSpritesFromDisc(fullPath, atlasSprites);
        }
        if (atlasSprites.TryGetValue("card_win.png", out this.winSprite))
            this.WinSpriteRenderer.sprite = RecreateSprite(this.winSprite);
        if (atlasSprites.TryGetValue("card_win_shadow.png", out this.winShadowSprite))
            this.WinShadowSpriteRenderer.sprite = RecreateSprite(this.winShadowSprite);
        if (atlasSprites.TryGetValue("card_lose.png", out this.loseSprite))
            this.loseSpriteRenderer.sprite = RecreateSprite(this.loseSprite);
        if (atlasSprites.TryGetValue("card_lose_shadow.png", out this.loseShadowSprite))
            this.loseShadowSpriteRenderer.sprite = RecreateSprite(this.loseShadowSprite);
        if (atlasSprites.TryGetValue("card_draw.png", out this.drawSprite))
            this.drawSpriteRenderer.sprite = RecreateSprite(this.drawSprite);
        if (atlasSprites.TryGetValue("card_draw_shadow.png", out this.drawShadowSprite))
            this.drawShadowSpriteRenderer.sprite = RecreateSprite(this.drawShadowSprite);
    }

    private static Sprite RecreateSprite(Sprite originalSprite, Texture2D moddedAtlas = null)
    {
        return Sprite.Create(moddedAtlas != null ? moddedAtlas : originalSprite.texture, originalSprite.rect, new Vector2(0f, 1f), 10f);
    }

    private static String GetQuadMistTextAtlasPath()
    {
        return "EmbeddedAsset/QuadMist/Atlas/quadmist_text_" + Localization.GetSymbol().ToLower();
    }

    private Sprite winSprite;
    private Sprite winShadowSprite;
    private Sprite loseSprite;
    private Sprite loseShadowSprite;
    private Sprite drawSprite;
    private Sprite drawShadowSprite;

    public SpriteRenderer WinSpriteRenderer;
    public SpriteRenderer WinShadowSpriteRenderer;
    public SpriteRenderer loseSpriteRenderer;
    public SpriteRenderer loseShadowSpriteRenderer;
    public SpriteRenderer drawSpriteRenderer;
    public SpriteRenderer drawShadowSpriteRenderer;
}
