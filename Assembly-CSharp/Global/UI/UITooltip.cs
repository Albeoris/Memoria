using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
    public static Boolean isVisible
    {
        get
        {
            return UITooltip.mInstance != (UnityEngine.Object)null && UITooltip.mInstance.mTarget == 1f;
        }
    }

    private void Awake()
    {
        UITooltip.mInstance = this;
    }

    private void OnDestroy()
    {
        UITooltip.mInstance = (UITooltip)null;
    }

    protected virtual void Start()
    {
        this.mTrans = base.transform;
        this.mWidgets = base.GetComponentsInChildren<UIWidget>();
        this.mPos = this.mTrans.localPosition;
        if (this.uiCamera == (UnityEngine.Object)null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.SetAlpha(0f);
    }

    protected virtual void Update()
    {
        if (this.mTooltip != UICamera.tooltipObject)
        {
            this.mTooltip = (GameObject)null;
            this.mTarget = 0f;
        }
        if (this.mCurrent != this.mTarget)
        {
            this.mCurrent = Mathf.Lerp(this.mCurrent, this.mTarget, RealTime.deltaTime * this.appearSpeed);
            if (Mathf.Abs(this.mCurrent - this.mTarget) < 0.001f)
            {
                this.mCurrent = this.mTarget;
            }
            this.SetAlpha(this.mCurrent * this.mCurrent);
            if (this.scalingTransitions)
            {
                Vector3 b = this.mSize * 0.25f;
                b.y = -b.y;
                Vector3 localScale = Vector3.one * (1.5f - this.mCurrent * 0.5f);
                Vector3 localPosition = Vector3.Lerp(this.mPos - b, this.mPos, this.mCurrent);
                this.mTrans.localPosition = localPosition;
                this.mTrans.localScale = localScale;
            }
        }
    }

    protected virtual void SetAlpha(Single val)
    {
        Int32 i = 0;
        Int32 num = (Int32)this.mWidgets.Length;
        while (i < num)
        {
            UIWidget uiwidget = this.mWidgets[i];
            Color color = uiwidget.color;
            color.a = val;
            uiwidget.color = color;
            i++;
        }
    }

    protected virtual void SetText(String tooltipText)
    {
        if (this.text != (UnityEngine.Object)null && !String.IsNullOrEmpty(tooltipText))
        {
            this.mTarget = 1f;
            this.mTooltip = UICamera.tooltipObject;
            this.text.rawText = tooltipText;
            this.mPos = UICamera.lastEventPosition;
            Transform transform = this.text.transform;
            Vector3 localPosition = transform.localPosition;
            Vector3 localScale = transform.localScale;
            this.mSize = this.text.printedSize;
            this.mSize.x = this.mSize.x * localScale.x;
            this.mSize.y = this.mSize.y * localScale.y;
            if (this.background != (UnityEngine.Object)null)
            {
                Vector4 border = this.background.border;
                this.mSize.x = this.mSize.x + (border.x + border.z + (localPosition.x - border.x) * 2f);
                this.mSize.y = this.mSize.y + (border.y + border.w + (-localPosition.y - border.y) * 2f);
                this.background.width = Mathf.RoundToInt(this.mSize.x);
                this.background.height = Mathf.RoundToInt(this.mSize.y);
            }
            if (this.uiCamera != (UnityEngine.Object)null)
            {
                this.mPos.x = Mathf.Clamp01(this.mPos.x / (Single)Screen.width);
                this.mPos.y = Mathf.Clamp01(this.mPos.y / (Single)Screen.height);
                Single num = this.uiCamera.orthographicSize / this.mTrans.parent.lossyScale.y;
                Single num2 = (Single)Screen.height * 0.5f / num;
                Vector2 vector = new Vector2(num2 * this.mSize.x / (Single)Screen.width, num2 * this.mSize.y / (Single)Screen.height);
                this.mPos.x = Mathf.Min(this.mPos.x, 1f - vector.x);
                this.mPos.y = Mathf.Max(this.mPos.y, vector.y);
                this.mTrans.position = this.uiCamera.ViewportToWorldPoint(this.mPos);
                this.mPos = this.mTrans.localPosition;
                this.mPos.x = Mathf.Round(this.mPos.x);
                this.mPos.y = Mathf.Round(this.mPos.y);
                this.mTrans.localPosition = this.mPos;
            }
            else
            {
                if (this.mPos.x + this.mSize.x > (Single)Screen.width)
                {
                    this.mPos.x = (Single)Screen.width - this.mSize.x;
                }
                if (this.mPos.y - this.mSize.y < 0f)
                {
                    this.mPos.y = this.mSize.y;
                }
                this.mPos.x = this.mPos.x - (Single)Screen.width * 0.5f;
                this.mPos.y = this.mPos.y - (Single)Screen.height * 0.5f;
            }
        }
        else
        {
            this.mTooltip = (GameObject)null;
            this.mTarget = 0f;
        }
    }

    [Obsolete("Use UITooltip.Show instead")]
    public static void ShowText(String text)
    {
        if (UITooltip.mInstance != (UnityEngine.Object)null)
        {
            UITooltip.mInstance.SetText(text);
        }
    }

    public static void Show(String text)
    {
        if (UITooltip.mInstance != (UnityEngine.Object)null)
        {
            UITooltip.mInstance.SetText(text);
        }
    }

    public static void Hide()
    {
        if (UITooltip.mInstance != (UnityEngine.Object)null)
        {
            UITooltip.mInstance.mTooltip = (GameObject)null;
            UITooltip.mInstance.mTarget = 0f;
        }
    }

    protected static UITooltip mInstance;

    public Camera uiCamera;

    public UILabel text;

    public UISprite background;

    public Single appearSpeed = 10f;

    public Boolean scalingTransitions = true;

    protected GameObject mTooltip;

    protected Transform mTrans;

    protected Single mTarget;

    protected Single mCurrent;

    protected Vector3 mPos;

    protected Vector3 mSize = Vector3.zero;

    protected UIWidget[] mWidgets;
}
