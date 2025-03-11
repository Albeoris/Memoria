using System;
using System.Collections;
using UnityEngine;

public class DialogAnimator : MonoBehaviour
{
    public static Single ShowAnimationTime
    {
        get
        {
            if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
                return DialogAnimator.DEFAULT_ANIMATION_TIME / HonoBehaviorSystem.Instance.GetFastForwardFactor();
            return DialogAnimator.DEFAULT_ANIMATION_TIME;
        }
    }

    public static Single HideAnimationTime => DialogAnimator.ShowAnimationTime;

    /// <summary>This is about the dialog frame growing in 0.15 seconds</summary>
    public Boolean ShowWithoutAnimation
    {
        get => this.showWithoutAnimation;
        set => this.showWithoutAnimation = value;
    }

    public Boolean Pause
    {
        set => this.pauseAnimation = value;
    }

    private void Awake()
    {
        this.dialog = base.gameObject.GetComponent<Dialog>();
        this.bodySprite = this.dialog.BodyGameObject.GetComponent<UISprite>();
        this.borderSprite = this.dialog.BorderGameObject.GetComponent<UISprite>();
        this.clipPanel = base.gameObject.GetComponent<UIPanel>();
        this.phraseLabel = this.dialog.PhraseGameObject.GetComponent<UILabel>();
        this.phraseTextEffect = this.dialog.PhraseGameObject.GetComponent<TypewriterEffect>();
        this.tailSprite = this.dialog.TailGameObject.GetComponent<UISprite>();
    }

    public void ShowDialog()
    {
        this.progress = 0.3f;
        base.StartCoroutine("StartShowDialog");
    }

    public void HideDialog()
    {
        this.progress = 0f;
        if (base.gameObject.activeInHierarchy)
            base.StartCoroutine("StartHideDialog");
    }

    public void ShowNewPage()
    {
        base.StartCoroutine("StartShowNewPage");
    }

    private IEnumerator StartShowDialog()
    {
        Boolean isTransparentDialog = this.dialog.Style == Dialog.WindowStyle.WindowStyleTransparent;
        if (this.showWithoutAnimation)
        {
            this.progress = 0.99f;
            if (!isTransparentDialog)
            {
                this.borderSprite.alpha = 0f;
                this.bodySprite.alpha = 0f;
            }
        }
        while (this.progress < 1f)
        {
            while (this.pauseAnimation)
                yield return new WaitForEndOfFrame();
            while ((FF9StateSystem.Field.FF9Field.attr & 1u) != 0u && this.dialog.Id != 9 && (MBG.IsNull || !MBG.Instance.isFMV055A))
            {
                this.borderSprite.alpha = 0f;
                this.tailSprite.alpha = 0f;
                this.bodySprite.alpha = 0f;
                this.progress = 0f;
                yield return new WaitForEndOfFrame();
            }
            Single scaleValue = this.progress;
            Single centerYValue = this.getCenterValue(this.dialog.Tail, scaleValue);
            Vector2 bodySizeValue = new Vector2(scaleValue * this.dialog.Size.x, scaleValue * this.dialog.Size.y);
            Vector2 clipSizeValue = new Vector2(scaleValue * this.dialog.Size.x + Dialog.DialogXPadding * 2f, scaleValue * this.dialog.Size.y + Dialog.DialogYPadding);
            bodySizeValue.x = Mathf.Max(bodySizeValue.x, 32f);
            bodySizeValue.y = Mathf.Max(bodySizeValue.y, 32f);
            clipSizeValue.x = Mathf.Max(clipSizeValue.x, 32f);
            clipSizeValue.y = Mathf.Max(clipSizeValue.y, 32f);
            this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, centerYValue, clipSizeValue.x, clipSizeValue.y);
            this.bodySprite.width = (Int32)bodySizeValue.x;
            this.bodySprite.height = (Int32)bodySizeValue.y;
            this.bodySprite.birthPosition = this.getBirthPosition(this.dialog.Tail, scaleValue);
            this.borderSprite.UpdateAnchors();
            this.progress += Time.deltaTime / DialogAnimator.ShowAnimationTime;
            yield return new WaitForEndOfFrame();
        }
        while (this.pauseAnimation)
            yield return new WaitForEndOfFrame();
        if (this.showWithoutAnimation)
        {
            this.progress = 0.99f;
            if (!isTransparentDialog)
            {
                this.borderSprite.alpha = 1f;
                this.bodySprite.alpha = 1f;
                if (this.dialog.Style == Dialog.WindowStyle.WindowStyleAuto)
                    this.tailSprite.alpha = 1f;
            }
        }
        this.bodySprite.width = (Int32)this.dialog.Size.x;
        this.bodySprite.height = (Int32)this.dialog.Size.y;
        this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, this.getCenterValue(this.dialog.Tail, 1f), this.dialog.ClipSize.x, this.dialog.ClipSize.y);
        if (!this.dialog.IsETbDialog)
            this.dialog.CurrentParser.ResetBeforeVariableTags(); // UI dialogs sometimes need to wrap their text after the frame grew to their full size
        this.dialog.CurrentState = Dialog.State.TextAnimation;
        if (this.dialog.TypeEffect)
            this.phraseTextEffect.SetActive(true, true);
        else
            this.dialog.CurrentParser.AdvanceProgress(this.dialog.CurrentParser.AppearProgressMax);
        yield return new WaitForEndOfFrame();
        this.pauseAnimation = false;
        this.dialog.AfterShown();
        yield break;
    }

    private IEnumerator StartShowNewPage()
    {
        yield return new WaitForEndOfFrame();
        if (this.dialog.TypeEffect)
        {
            this.dialog.CurrentState = Dialog.State.TextAnimation;
            this.phraseTextEffect.SetActive(true, true);
        }
        else
        {
            this.phraseLabel.Parser.AdvanceProgress(this.phraseLabel.Parser.AppearProgressMax);
            this.dialog.CurrentState = Dialog.State.CompleteAnimation;
        }
        yield break;
    }

    private IEnumerator StartHideDialog()
    {
        yield return new WaitForEndOfFrame();
        while (this.progress < 0.6f)
        {
            while (this.pauseAnimation)
                yield return new WaitForEndOfFrame();
            Single scaleValue = 1f - this.progress;
            Single centerYValue = this.getCenterValue(this.dialog.Tail, scaleValue);
            Vector2 bodySizeValue = new Vector2(scaleValue * this.dialog.Size.x, scaleValue * this.dialog.Size.y);
            Vector2 clipSizeValue = new Vector2(scaleValue * this.dialog.Size.x + Dialog.DialogXPadding * 2f, scaleValue * this.dialog.Size.y + Dialog.DialogYPadding);
            bodySizeValue.x = Mathf.Max(bodySizeValue.x, 32f);
            bodySizeValue.y = Mathf.Max(bodySizeValue.y, 32f);
            clipSizeValue.x = Mathf.Max(clipSizeValue.x, 32f);
            clipSizeValue.y = Mathf.Max(clipSizeValue.y, 32f);
            this.clipPanel.baseClipRegion = new Vector4(this.clipPanel.baseClipRegion.x, centerYValue, clipSizeValue.x, clipSizeValue.y);
            this.bodySprite.width = (Int32)bodySizeValue.x;
            this.bodySprite.height = (Int32)bodySizeValue.y;
            this.bodySprite.birthPosition = this.getBirthPosition(this.dialog.Tail, scaleValue);
            this.borderSprite.UpdateAnchors();
            this.progress += Time.deltaTime / DialogAnimator.HideAnimationTime;
            yield return new WaitForEndOfFrame();
        }
        while (this.pauseAnimation)
            yield return new WaitForEndOfFrame();
        this.phraseLabel.rawText = String.Empty;
        this.pauseAnimation = false;
        this.dialog.AfterHidden();
        yield break;
    }

    private Single getCenterValue(Dialog.TailPosition tailPosition, Single ratio = 1f)
    {
        switch (tailPosition)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerCenter:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                return (this.dialog.ClipSize.y / 2f - Dialog.DialogYPadding / 2f) * (1f - ratio);
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperCenter:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                return -(this.dialog.ClipSize.y / 2f - Dialog.DialogYPadding / 2f) * (1f - ratio);
        }
        return 0f;
    }

    private Vector2 getBirthPosition(Dialog.TailPosition tailPosition, Single ratio = 1f)
    {
        switch (tailPosition)
        {
            case Dialog.TailPosition.LowerRight:
            case Dialog.TailPosition.LowerLeft:
            case Dialog.TailPosition.LowerRightForce:
            case Dialog.TailPosition.LowerLeftForce:
                return new Vector2(this.dialog.Size.x / 2f * ratio, this.dialog.Size.y * ratio);
            case Dialog.TailPosition.UpperRight:
            case Dialog.TailPosition.UpperLeft:
            case Dialog.TailPosition.UpperRightForce:
            case Dialog.TailPosition.UpperLeftForce:
                return new Vector2(this.dialog.Size.x / 2f * ratio, 0f);
        }
        return new Vector2(this.dialog.Size.x / 2f * ratio, this.dialog.Size.y / 2f * ratio);
    }

    public const Single DEFAULT_ANIMATION_TIME = 0.15f;

    [SerializeField]
    private Single progress;

    private Boolean showWithoutAnimation;

    private Dialog dialog;
    private UIPanel clipPanel;

    private UISprite bodySprite;
    private UISprite borderSprite;
    private UILabel phraseLabel;
    private UISprite tailSprite;

    private TypewriterEffect phraseTextEffect;

    private Boolean pauseAnimation;
}
