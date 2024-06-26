using Assets.Scripts.Common;
using Memoria.Assets;
using System;
using System.Collections;
using UnityEngine;

public class GameOverUI : UIScene
{
    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        SceneDirector.FF9Wipe_FadeInEx(30);
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            // Replace gameover screen with modded version if it exists
            UI2DSprite sprite2D = this.GameOverSprite.GetComponent<UI2DSprite>();
            Sprite sprite = sprite2D.sprite2D;
            String externalPath = AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/gameover", true, false);
            if (!String.IsNullOrEmpty(externalPath))
            {
                Texture2D texture = StreamingResources.LoadTexture2D(externalPath);
                sprite2D.sprite2D = Sprite.Create(texture, sprite.rect, sprite.pivot);
                sprite2D.sprite2D.name = sprite.name;
            }
            externalPath = AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/gameover.png", true, false); // also check .png for simplicity
            if (!String.IsNullOrEmpty(externalPath))
            {
                Texture2D texture = StreamingResources.LoadTexture2D(externalPath);
                sprite2D.sprite2D = Sprite.Create(texture, sprite.rect, sprite.pivot);
                sprite2D.sprite2D.name = sprite.name;
            }

            SceneDirector.FF9Wipe_FadeInEx(24);
            this.AnimatePanel(1);
        };
        TimerUI.SetEnable(false);
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Show(sceneVoidDelegate);
        PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(false);
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
        {
            if (PersistenSingleton<EventEngine>.Instance.gMode == 2)
            {
                battle.ff9ShutdownStateBattleResult();
            }
            else if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
            {
                HonoluluFieldMain.FadeOutMusic();
            }
            TimerUI.SetEnable(false);
            SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
            SceneDirector.Replace("Title", SceneTransition.FadeOutToBlack_FadeIn, true);
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Hide(sceneVoidDelegate);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            this.GoToTitleScene();
        }
        return true;
    }

    private void AnimatePanel(Int32 alpha)
    {
        TweenAlpha.Begin(this.GameOverSprite, this.duration, (Single)alpha);
    }

    private void GoToTitleScene()
    {
        this.AnimatePanel(0);
        base.StartCoroutine(this.WaitAndHide());
    }

    private IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(this.duration);
        this.Hide((UIScene.SceneVoidDelegate)null);
        yield break;
    }

    private void Start()
    {
        this.AnimatePanel(1);
    }

    private void Update()
    {
        SceneDirector.ServiceFade();
    }

    public GameObject GameOverSprite;

    private Single duration = 1f;
}
