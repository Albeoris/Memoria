using System;
using System.Collections;
using UnityEngine;

public class HonoTweenClipping : MonoBehaviour
{
    private void Awake()
    {
        this.scaleWidget = this.ScaleGameObject.GetComponent<UIWidget>();
        this.defaultClipScale = new Vector2(this.ClipPanel.width, this.ClipPanel.height);
        this.defaultClipCenter = new Vector2(this.ClipPanel.baseClipRegion.x, this.ClipPanel.baseClipRegion.y);
        this.defaultSpriteScale = new Vector2(this.scaleWidget.width, this.scaleWidget.height);
        this.defaultSpriteCenter = new Vector2(this.scaleWidget.transform.localPosition.x, this.scaleWidget.transform.localPosition.y);
        if (this.ShiftContentGameObject != null)
            this.defaultShiftPos = new Vector2(this.ShiftContentGameObject.transform.localPosition.x, this.ShiftContentGameObject.transform.localPosition.y);
    }

    public void TweenIn(Action callBack = null)
    {
        this.progress = 0f;
        this.ResetToMinimum();
        this.ClipGameObject.SetActive(true);
        base.StartCoroutine(this.StartTweenIn(callBack));
    }

    public void TweenOut(Action callBack = null)
    {
        this.progress = 0f;
        base.StartCoroutine(this.StartTweenOut(callBack));
    }

    public void ResetDefaultSpriteValue()
    {
        this.defaultSpriteScale = new Vector2(this.scaleWidget.width, this.scaleWidget.height);
        this.defaultSpriteCenter = new Vector2(this.scaleWidget.transform.localPosition.x, this.scaleWidget.transform.localPosition.y);
    }

    public void ResetToDefault()
    {
        this.ClipPanel.baseClipRegion = new Vector4(this.defaultClipCenter.x, this.defaultClipCenter.y, this.defaultClipScale.x, this.defaultClipCenter.y);
        this.scaleWidget.width = (Int32)this.defaultSpriteScale.x;
        this.scaleWidget.height = (Int32)this.defaultSpriteScale.y;
        this.scaleWidget.transform.localPosition = new Vector3(this.defaultSpriteCenter.x, this.defaultSpriteCenter.y, 0f);
    }

    public void ResetToMinimum()
    {
        Single clipFactor = 1f - this.CalcProgressFromTween(this.TweenClipTo);
        Single sprtFactor = 1f - this.CalcProgressFromTween(this.TweenSpriteTo);
        Vector2 centerValueSprt = this.getCenterValue(this.defaultSpriteCenter, this.defaultSpriteScale, this.ClipPosition, 1f - sprtFactor);
        Vector2 centerValueClip = this.getCenterValue(this.defaultClipCenter, this.defaultClipScale, this.ClipPosition, 1f - clipFactor);
        this.scaleWidget.width = this.ClipX ? (Int32)this.TweenSpriteTo.x : (Int32)this.defaultSpriteScale.x;
        this.scaleWidget.height = this.ClipY ? (Int32)this.TweenSpriteTo.y : (Int32)this.defaultSpriteScale.y;
        this.scaleWidget.transform.localPosition = new Vector3(centerValueSprt.x, centerValueSprt.y, 0f);
        this.ClipPanel.baseClipRegion = new Vector4(centerValueClip.x, centerValueClip.y, this.ClipX ? this.TweenClipTo.x : this.defaultClipScale.x, this.ClipY ? this.TweenClipTo.y : this.defaultClipScale.y);
        if (this.ShiftContentGameObject != null)
            this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x + this.ShiftContentClip.x, this.defaultShiftPos.y + this.ShiftContentClip.y, 0f);
    }

    private IEnumerator StartTweenIn(Action callBack)
    {
        Single processClip = this.CalcProgressFromTween(this.TweenClipTo);
        Single processSprite = this.CalcProgressFromTween(this.TweenSpriteTo);
        Single progress = Math.Min(processClip, processSprite);
        while (progress < 1f)
        {
            ApplyTweenFrame(progress, processClip, processSprite);
            progress += Time.deltaTime / this.AnimationTime;
            yield return new WaitForEndOfFrame();
        }
        this.scaleWidget.width = (Int32)this.defaultSpriteScale.x;
        this.scaleWidget.height = (Int32)this.defaultSpriteScale.y;
        this.scaleWidget.transform.localPosition = new Vector3(this.defaultSpriteCenter.x, this.defaultSpriteCenter.y, 0f);
        this.ClipPanel.baseClipRegion = new Vector4(this.defaultClipCenter.x, this.defaultClipCenter.y, this.defaultClipScale.x, this.defaultClipScale.y);
        if (this.ShiftContentGameObject != null)
            this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x, this.defaultShiftPos.y, 0f);
        if (callBack != null)
            callBack();
        yield return new WaitForEndOfFrame();
        yield break;
    }

    private IEnumerator StartTweenOut(Action callBack)
    {
        Single processClip = this.CalcProgressFromTween(this.TweenClipTo);
        Single processSprite = this.CalcProgressFromTween(this.TweenSpriteTo);
        Single endProcess = Math.Max(1f - processClip, 1f - processSprite);
        while (this.progress < endProcess)
        {
            ApplyTweenFrame(1f - this.progress, processClip, processSprite);
            this.progress += Time.deltaTime / this.AnimationTime;
            yield return new WaitForEndOfFrame();
        }
        this.ResetToMinimum();
        if (this.HideAfterClip)
            this.ClipGameObject.SetActive(false);
        if (callBack != null)
            callBack();
        yield return new WaitForEndOfFrame();
        yield break;
    }

    private void ApplyTweenFrame(Single scaleValue, Single processClip, Single processSprite)
    {
        Single scaleXValue = this.ClipX ? scaleValue : 1f;
        Single scaleYValue = this.ClipY ? scaleValue : 1f;
        Vector2 bodyCenterValue = this.getCenterValue(this.defaultSpriteCenter, this.defaultSpriteScale, this.ClipPosition, scaleValue);
        Vector2 clipCenterValue = this.getCenterValue(this.defaultClipCenter, this.defaultClipScale, this.ClipPosition, scaleValue);
        Vector2 bodySizeValue = new Vector2(scaleXValue * this.defaultSpriteScale.x, scaleYValue * this.defaultSpriteScale.y);
        Vector2 clipSizeValue = new Vector2(scaleXValue * this.defaultClipScale.x, scaleYValue * this.defaultClipScale.y);
        bodySizeValue.x = Mathf.Max(bodySizeValue.x, this.TweenSpriteTo.x);
        bodySizeValue.y = Mathf.Max(bodySizeValue.y, this.TweenSpriteTo.y);
        clipSizeValue.x = Mathf.Max(clipSizeValue.x, this.TweenClipTo.x);
        clipSizeValue.y = Mathf.Max(clipSizeValue.y, this.TweenClipTo.y);
        if (scaleValue > processClip)
        {
            this.ClipPanel.baseClipRegion = new Vector4(clipCenterValue.x, clipCenterValue.y, clipSizeValue.x, clipSizeValue.y - 20f);
            if (this.ShiftContentGameObject != null)
                this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x + this.ShiftContentClip.x * (1f - scaleXValue), this.defaultShiftPos.y + this.ShiftContentClip.y * (1f - scaleYValue), 0f);
        }
        if (scaleValue > processSprite)
        {
            this.scaleWidget.width = (Int32)bodySizeValue.x;
            this.scaleWidget.height = (Int32)bodySizeValue.y;
            this.scaleWidget.transform.localPosition = new Vector3(bodyCenterValue.x, bodyCenterValue.y, 0f);
            this.BGSprite.birthPosition = this.getBirthPosition(this.ClipPosition, scaleValue);
        }
    }

    private Vector2 getCenterValue(Vector2 defaultCenter, Vector2 defaultSize, HonoTweenClipping.ClipPos clipPosition, Single ratio = 1f)
    {
        switch (clipPosition)
        {
            case HonoTweenClipping.ClipPos.TopLeft:
                return new Vector2(0f, 0f);
            case HonoTweenClipping.ClipPos.Top:
                return new Vector2(defaultCenter.x, defaultCenter.y + defaultSize.y / 2f * (1f - ratio));
            case HonoTweenClipping.ClipPos.TopRight:
                return new Vector2(defaultSize.x * (1f - ratio), 0f);
            case HonoTweenClipping.ClipPos.Left:
                return new Vector2(0f, defaultCenter.y);
            case HonoTweenClipping.ClipPos.Center:
                return new Vector2(defaultCenter.x, defaultCenter.y);
            case HonoTweenClipping.ClipPos.Right:
                return new Vector2(defaultSize.x * (1f - ratio), defaultCenter.y);
            case HonoTweenClipping.ClipPos.BottomLeft:
                return new Vector2(0f, defaultSize.y * (1f - ratio));
            case HonoTweenClipping.ClipPos.Bottom:
                return new Vector2(defaultCenter.x, defaultSize.y * (1f - ratio));
            case HonoTweenClipping.ClipPos.BottomRight:
                return new Vector2(defaultSize.x / 2f * (1f - ratio) + defaultCenter.x, -(defaultSize.y / 2f) * (1f - ratio) + defaultCenter.y);
            default:
                return new Vector2(0f, 0f);
        }
    }

    private Vector2 getBirthPosition(HonoTweenClipping.ClipPos clipPosition, Single ratio = 1f)
    {
        switch (clipPosition)
        {
            case HonoTweenClipping.ClipPos.TopLeft:
                return new Vector2(0f, 0f);
            case HonoTweenClipping.ClipPos.Top:
                return new Vector2(0f, this.defaultSpriteScale.y * ratio);
            case HonoTweenClipping.ClipPos.TopRight:
                return new Vector2(0f, 0f);
            case HonoTweenClipping.ClipPos.Left:
                return new Vector2(0f, this.defaultSpriteScale.y / 2f * ratio);
            case HonoTweenClipping.ClipPos.Center:
                return new Vector2(this.defaultSpriteScale.x / 2f * ratio, this.defaultSpriteScale.y / 2f * ratio);
            case HonoTweenClipping.ClipPos.Right:
                return new Vector2(this.defaultSpriteScale.x * ratio, this.defaultSpriteScale.y / 2f * ratio);
            case HonoTweenClipping.ClipPos.BottomLeft:
                return new Vector2(0f, this.defaultSpriteScale.y * ratio);
            case HonoTweenClipping.ClipPos.Bottom:
                return new Vector2(this.defaultSpriteScale.x / 2f * ratio, this.defaultSpriteScale.y * ratio);
            case HonoTweenClipping.ClipPos.BottomRight:
                return new Vector2(this.defaultSpriteScale.x * ratio, this.defaultSpriteScale.y * ratio);
            default:
                return new Vector2(0f, 0f);
        }
    }

    private Single CalcProgressFromTween(Vector2 tweenTo)
    {
        if (!this.ClipX)
            return tweenTo.y / this.defaultSpriteScale.y;
        if (!this.ClipY)
            return tweenTo.x / this.defaultSpriteScale.x;
        return Mathf.Min(tweenTo.x / this.defaultSpriteScale.x, tweenTo.y / this.defaultSpriteScale.y);
    }

    public Single AnimationTime = 0.16f;
    public Vector2 TweenClipTo;
    public Vector2 TweenSpriteTo;
    public Vector2 ShiftContentClip;

    public GameObject ClipGameObject;
    public UIPanel ClipPanel;
    public GameObject ScaleGameObject;
    public UISprite BGSprite;
    public GameObject ShiftContentGameObject;

    public HonoTweenClipping.ClipPos ClipPosition;
    public Boolean ClipX;
    public Boolean ClipY;

    public Boolean HideAfterClip = true;

    private Single progress;

    private UIWidget scaleWidget;

    private Vector2 defaultClipScale;
    private Vector2 defaultClipCenter;
    private Vector2 defaultSpriteScale;
    private Vector2 defaultSpriteCenter;
    private Vector2 defaultShiftPos = default(Vector2);

    public enum ClipPos
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }
}
