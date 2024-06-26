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
        this.defaultSpriteScale = new Vector2((Single)this.scaleWidget.width, (Single)this.scaleWidget.height);
        this.defaultSpriteCenter = new Vector2(this.scaleWidget.transform.localPosition.x, this.scaleWidget.transform.localPosition.y);
        if (this.ShiftContentGameObject != (UnityEngine.Object)null)
        {
            this.defaultShiftPos = new Vector2(this.ShiftContentGameObject.transform.localPosition.x, this.ShiftContentGameObject.transform.localPosition.y);
        }
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
        this.defaultSpriteScale = new Vector2((Single)this.scaleWidget.width, (Single)this.scaleWidget.height);
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
        Single num = 1f - this.CalcProgressFromTween(this.TweenClipTo);
        Single num2 = 1f - this.CalcProgressFromTween(this.TweenSpriteTo);
        Vector2 centerValue = this.getCenterValue(this.defaultSpriteCenter, this.defaultSpriteScale, this.ClipPosition, 1f - num2);
        Vector2 centerValue2 = this.getCenterValue(this.defaultClipCenter, this.defaultClipScale, this.ClipPosition, 1f - num);
        this.scaleWidget.width = (Int32)((!this.ClipX) ? ((Int32)this.defaultSpriteScale.x) : ((Int32)this.TweenSpriteTo.x));
        this.scaleWidget.height = (Int32)((!this.ClipY) ? ((Int32)this.defaultSpriteScale.y) : ((Int32)this.TweenSpriteTo.y));
        this.scaleWidget.transform.localPosition = new Vector3(centerValue.x, centerValue.y, 0f);
        this.ClipPanel.baseClipRegion = new Vector4(centerValue2.x, centerValue2.y, (!this.ClipX) ? this.defaultClipScale.x : this.TweenClipTo.x, (!this.ClipY) ? this.defaultClipScale.y : this.TweenClipTo.y);
        if (this.ShiftContentGameObject != (UnityEngine.Object)null)
        {
            this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x + this.ShiftContentClip.x, this.defaultShiftPos.y + this.ShiftContentClip.y, 0f);
        }
    }

    private IEnumerator StartTweenIn(Action callBack)
    {
        Single processClip = this.CalcProgressFromTween(this.TweenClipTo);
        Single processSprite = this.CalcProgressFromTween(this.TweenSpriteTo);
        Single progress = Math.Min(processClip, processSprite);
        while (progress < 1f)
        {
            Single scaleValue = progress;
            Single scaleXValue = (!this.ClipX) ? 1f : scaleValue;
            Single scaleYValue = (!this.ClipY) ? 1f : scaleValue;
            Vector2 bodyCenterValue = this.getCenterValue(this.defaultSpriteCenter, this.defaultSpriteScale, this.ClipPosition, scaleValue);
            Vector2 clipCenterValue = this.getCenterValue(this.defaultClipCenter, this.defaultClipScale, this.ClipPosition, scaleValue);
            Vector2 bodySizeValue = new Vector2(scaleXValue * this.defaultSpriteScale.x, scaleYValue * this.defaultSpriteScale.y);
            Vector2 clipSizeValue = new Vector2(scaleXValue * this.defaultClipScale.x, scaleYValue * this.defaultClipScale.y);
            bodySizeValue.x = Mathf.Max(bodySizeValue.x, this.TweenSpriteTo.x);
            bodySizeValue.y = Mathf.Max(bodySizeValue.y, this.TweenSpriteTo.y);
            clipSizeValue.x = Mathf.Max(clipSizeValue.x, this.TweenClipTo.x);
            clipSizeValue.y = Mathf.Max(clipSizeValue.y, this.TweenClipTo.y);
            if (progress > processClip)
            {
                this.ClipPanel.baseClipRegion = new Vector4(clipCenterValue.x, clipCenterValue.y, clipSizeValue.x, clipSizeValue.y - 20f);
                if (this.ShiftContentGameObject != (UnityEngine.Object)null)
                {
                    this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x + this.ShiftContentClip.x * (1f - scaleXValue), this.defaultShiftPos.y + this.ShiftContentClip.y * (1f - scaleYValue), 0f);
                }
            }
            if (progress > processSprite)
            {
                this.scaleWidget.width = (Int32)bodySizeValue.x;
                this.scaleWidget.height = (Int32)bodySizeValue.y;
                this.scaleWidget.transform.localPosition = new Vector3(bodyCenterValue.x, bodyCenterValue.y, 0f);
                this.BGSprite.birthPosition = this.getBirthPosition(this.ClipPosition, scaleValue);
            }
            progress += Time.deltaTime / this.AnimationTime;
            yield return new WaitForEndOfFrame();
        }
        this.scaleWidget.width = (Int32)this.defaultSpriteScale.x;
        this.scaleWidget.height = (Int32)this.defaultSpriteScale.y;
        this.scaleWidget.transform.localPosition = new Vector3(this.defaultSpriteCenter.x, this.defaultSpriteCenter.y, 0f);
        this.ClipPanel.baseClipRegion = new Vector4(this.defaultClipCenter.x, this.defaultClipCenter.y, this.defaultClipScale.x, this.defaultClipScale.y);
        if (this.ShiftContentGameObject != (UnityEngine.Object)null)
        {
            this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x, this.defaultShiftPos.y, 0f);
        }
        if (callBack != null)
        {
            callBack();
        }
        yield return new WaitForEndOfFrame();
        yield break;
    }

    private IEnumerator StartTweenOut(Action callBack)
    {
        Single endProcessClip = 1f - this.CalcProgressFromTween(this.TweenClipTo);
        Single endProcessSprite = 1f - this.CalcProgressFromTween(this.TweenSpriteTo);
        Single endProcess = Math.Max(endProcessClip, endProcessSprite);
        while (this.progress < endProcess)
        {
            Single scaleValue = 1f - this.progress;
            Single scaleXValue = (!this.ClipX) ? 1f : scaleValue;
            Single scaleYValue = (!this.ClipY) ? 1f : scaleValue;
            Vector2 bodyCenterValue = this.getCenterValue(this.defaultSpriteCenter, this.defaultSpriteScale, this.ClipPosition, scaleValue);
            Vector2 clipCenterValue = this.getCenterValue(this.defaultClipCenter, this.defaultClipScale, this.ClipPosition, scaleValue);
            Vector2 bodySizeValue = new Vector2(scaleXValue * this.defaultSpriteScale.x, scaleYValue * this.defaultSpriteScale.y);
            Vector2 clipSizeValue = new Vector2(scaleXValue * this.defaultClipScale.x, scaleYValue * this.defaultClipScale.y);
            bodySizeValue.x = Mathf.Max(bodySizeValue.x, this.TweenSpriteTo.x);
            bodySizeValue.y = Mathf.Max(bodySizeValue.y, this.TweenSpriteTo.y);
            clipSizeValue.x = Mathf.Max(clipSizeValue.x, this.TweenClipTo.x);
            clipSizeValue.y = Mathf.Max(clipSizeValue.y, this.TweenClipTo.y);
            if (this.progress < endProcessClip)
            {
                this.ClipPanel.baseClipRegion = new Vector4(clipCenterValue.x, clipCenterValue.y, clipSizeValue.x, clipSizeValue.y - 20f);
                if (this.ShiftContentGameObject != (UnityEngine.Object)null)
                {
                    this.ShiftContentGameObject.transform.localPosition = new Vector3(this.defaultShiftPos.x + this.ShiftContentClip.x * (1f - scaleXValue), this.defaultShiftPos.y + this.ShiftContentClip.y * (1f - scaleYValue), 0f);
                }
            }
            if (this.progress < endProcessSprite)
            {
                this.scaleWidget.width = (Int32)bodySizeValue.x;
                this.scaleWidget.height = (Int32)bodySizeValue.y;
                this.scaleWidget.transform.localPosition = new Vector3(bodyCenterValue.x, bodyCenterValue.y, 0f);
                this.BGSprite.birthPosition = this.getBirthPosition(this.ClipPosition, scaleValue);
            }
            this.progress += Time.deltaTime / this.AnimationTime;
            yield return new WaitForEndOfFrame();
        }
        this.ResetToMinimum();
        if (this.HideAfterClip)
        {
            this.ClipGameObject.SetActive(false);
        }
        if (callBack != null)
        {
            callBack();
        }
        yield return new WaitForEndOfFrame();
        yield break;
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
        {
            return tweenTo.y / this.defaultSpriteScale.y;
        }
        if (!this.ClipY)
        {
            return tweenTo.x / this.defaultSpriteScale.x;
        }
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
