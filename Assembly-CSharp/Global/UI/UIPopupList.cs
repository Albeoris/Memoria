using Memoria.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Popup List")]
public class UIPopupList : UIWidgetContainer
{
    public UnityEngine.Object ambigiousFont
    {
        get
        {
            if (this.trueTypeFont != (UnityEngine.Object)null)
            {
                return this.trueTypeFont;
            }
            if (this.bitmapFont != (UnityEngine.Object)null)
            {
                return this.bitmapFont;
            }
            return this.font;
        }
        set
        {
            if (value is Font)
            {
                this.trueTypeFont = (value as Font);
                this.bitmapFont = (UIFont)null;
                this.font = (UIFont)null;
            }
            else if (value is UIFont)
            {
                this.bitmapFont = (value as UIFont);
                this.trueTypeFont = (Font)null;
                this.font = (UIFont)null;
            }
        }
    }

    [Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
    public UIPopupList.LegacyEvent onSelectionChange
    {
        get
        {
            return this.mLegacyEvent;
        }
        set
        {
            this.mLegacyEvent = value;
        }
    }

    public static Boolean isOpen
    {
        get
        {
            return UIPopupList.current != (UnityEngine.Object)null && (UIPopupList.mChild != (UnityEngine.Object)null || UIPopupList.mFadeOutComplete > Time.unscaledTime);
        }
    }

    public String value
    {
        get
        {
            return this.mSelectedItem;
        }
        set
        {
            this.mSelectedItem = value;
            if (this.mSelectedItem == null)
            {
                return;
            }
            if (this.mSelectedItem != null)
            {
                this.TriggerCallbacks();
            }
        }
    }

    public Object data
    {
        get
        {
            Int32 num = this.items.IndexOf(this.mSelectedItem);
            return (num <= -1 || num >= this.itemData.Count) ? null : this.itemData[num];
        }
    }

    public Boolean isColliderEnabled
    {
        get
        {
            Collider component = base.GetComponent<Collider>();
            if (component != (UnityEngine.Object)null)
            {
                return component.enabled;
            }
            Collider2D component2 = base.GetComponent<Collider2D>();
            return component2 != (UnityEngine.Object)null && component2.enabled;
        }
    }

    [Obsolete("Use 'value' instead")]
    public String selection
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = value;
        }
    }

    private Boolean isValid
    {
        get
        {
            return this.bitmapFont != (UnityEngine.Object)null || this.trueTypeFont != (UnityEngine.Object)null;
        }
    }

    private Int32 activeFontSize
    {
        get
        {
            return (Int32)((!(this.trueTypeFont != (UnityEngine.Object)null) && !(this.bitmapFont == (UnityEngine.Object)null)) ? this.bitmapFont.defaultSize : this.fontSize);
        }
    }

    private Single activeFontScale
    {
        get
        {
            return (!(this.trueTypeFont != (UnityEngine.Object)null) && !(this.bitmapFont == (UnityEngine.Object)null)) ? ((Single)this.fontSize / (Single)this.bitmapFont.defaultSize) : 1f;
        }
    }

    public void Clear()
    {
        this.items.Clear();
        this.itemData.Clear();
    }

    public void AddItem(String text)
    {
        this.items.Add(text);
        this.itemData.Add(null);
    }

    public void AddItem(String text, Object data)
    {
        this.items.Add(text);
        this.itemData.Add(data);
    }

    public void RemoveItem(String text)
    {
        Int32 num = this.items.IndexOf(text);
        if (num != -1)
        {
            this.items.RemoveAt(num);
            this.itemData.RemoveAt(num);
        }
    }

    public void RemoveItemByData(Object data)
    {
        Int32 num = this.itemData.IndexOf(data);
        if (num != -1)
        {
            this.items.RemoveAt(num);
            this.itemData.RemoveAt(num);
        }
    }

    protected void TriggerCallbacks()
    {
        if (!this.mExecuting)
        {
            this.mExecuting = true;
            UIPopupList uipopupList = UIPopupList.current;
            UIPopupList.current = this;
            if (this.mLegacyEvent != null)
            {
                this.mLegacyEvent(this.mSelectedItem);
            }
            if (EventDelegate.IsValid(this.onChange))
            {
                EventDelegate.Execute(this.onChange);
            }
            else if (this.eventReceiver != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.functionName))
            {
                this.eventReceiver.SendMessage(this.functionName, this.mSelectedItem, SendMessageOptions.DontRequireReceiver);
            }
            UIPopupList.current = uipopupList;
            this.mExecuting = false;
        }
    }

    private void OnEnable()
    {
        if (EventDelegate.IsValid(this.onChange))
        {
            this.eventReceiver = (GameObject)null;
            this.functionName = (String)null;
        }
        if (this.font != (UnityEngine.Object)null)
        {
            if (this.font.isDynamic)
            {
                this.trueTypeFont = this.font.dynamicFont;
                this.fontStyle = this.font.dynamicFontStyle;
                this.mUseDynamicFont = true;
            }
            else if (this.bitmapFont == (UnityEngine.Object)null)
            {
                this.bitmapFont = this.font;
                this.mUseDynamicFont = false;
            }
            this.font = (UIFont)null;
        }
        if (this.textScale != 0f)
        {
            this.fontSize = (Int32)((!(this.bitmapFont != (UnityEngine.Object)null)) ? 16 : Mathf.RoundToInt((Single)this.bitmapFont.defaultSize * this.textScale));
            this.textScale = 0f;
        }
        if (this.trueTypeFont == (UnityEngine.Object)null && this.bitmapFont != (UnityEngine.Object)null && this.bitmapFont.isDynamic)
        {
            this.trueTypeFont = this.bitmapFont.dynamicFont;
            this.bitmapFont = (UIFont)null;
        }
    }

    private void OnValidate()
    {
        Font x = this.trueTypeFont;
        UIFont uifont = this.bitmapFont;
        this.bitmapFont = (UIFont)null;
        this.trueTypeFont = (Font)null;
        if (x != (UnityEngine.Object)null && (uifont == (UnityEngine.Object)null || !this.mUseDynamicFont))
        {
            this.bitmapFont = (UIFont)null;
            this.trueTypeFont = x;
            this.mUseDynamicFont = true;
        }
        else if (uifont != (UnityEngine.Object)null)
        {
            if (uifont.isDynamic)
            {
                this.trueTypeFont = uifont.dynamicFont;
                this.fontStyle = uifont.dynamicFontStyle;
                this.fontSize = uifont.defaultSize;
                this.mUseDynamicFont = true;
            }
            else
            {
                this.bitmapFont = uifont;
                this.mUseDynamicFont = false;
            }
        }
        else
        {
            this.trueTypeFont = x;
            this.mUseDynamicFont = true;
        }
    }

    private void Start()
    {
        if (this.textLabel != (UnityEngine.Object)null)
        {
            EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
            this.textLabel = (UILabel)null;
        }
        if (Application.isPlaying && String.IsNullOrEmpty(this.mSelectedItem) && this.items.Count > 0)
        {
            this.value = this.items[0];
        }
    }

    private void OnLocalize()
    {
        if (this.isLocalized)
        {
            this.TriggerCallbacks();
        }
    }

    private void Highlight(UILabel lbl, Boolean instant)
    {
        if (this.mHighlight != (UnityEngine.Object)null)
        {
            this.mHighlightedLabel = lbl;
            if (this.mHighlight.GetAtlasSprite() == null)
            {
                return;
            }
            Vector3 highlightPosition = this.GetHighlightPosition();
            if (!instant && this.isAnimated)
            {
                TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
                if (!this.mTweening)
                {
                    this.mTweening = true;
                    base.StartCoroutine("UpdateTweenPosition");
                }
            }
            else
            {
                this.mHighlight.cachedTransform.localPosition = highlightPosition;
            }
        }
    }

    private Vector3 GetHighlightPosition()
    {
        if (this.mHighlightedLabel == (UnityEngine.Object)null || this.mHighlight == (UnityEngine.Object)null)
        {
            return Vector3.zero;
        }
        UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
        if (atlasSprite == null)
        {
            return Vector3.zero;
        }
        Single pixelSize = this.atlas.pixelSize;
        Single num = (Single)atlasSprite.borderLeft * pixelSize;
        Single y = (Single)atlasSprite.borderTop * pixelSize;
        return this.mHighlightedLabel.cachedTransform.localPosition + new Vector3(-num, y, 1f);
    }

    private IEnumerator UpdateTweenPosition()
    {
        if (this.mHighlight != (UnityEngine.Object)null && this.mHighlightedLabel != (UnityEngine.Object)null)
        {
            TweenPosition tp = this.mHighlight.GetComponent<TweenPosition>();
            while (tp != (UnityEngine.Object)null && tp.enabled)
            {
                tp.to = this.GetHighlightPosition();
                yield return null;
            }
        }
        this.mTweening = false;
        yield break;
    }

    private void OnItemHover(GameObject go, Boolean isOver)
    {
        if (isOver)
        {
            UILabel component = go.GetComponent<UILabel>();
            this.Highlight(component, false);
        }
    }

    private void OnItemPress(GameObject go, Boolean isPressed)
    {
        if (isPressed)
        {
            this.Select(go.GetComponent<UILabel>(), true);
            UIEventListener component = go.GetComponent<UIEventListener>();
            this.value = (component.parameter as String);
            UIPlaySound[] components = base.GetComponents<UIPlaySound>();
            Int32 i = 0;
            Int32 num = (Int32)components.Length;
            while (i < num)
            {
                UIPlaySound uiplaySound = components[i];
                if (uiplaySound.trigger == UIPlaySound.Trigger.OnClick)
                {
                    NGUITools.PlaySound(uiplaySound.audioClip, uiplaySound.volume, 1f);
                }
                i++;
            }
            this.CloseSelf();
        }
    }

    private void Select(UILabel lbl, Boolean instant)
    {
        this.Highlight(lbl, instant);
    }

    private void OnNavigate(KeyCode key)
    {
        if (base.enabled && UIPopupList.current == this)
        {
            Int32 num = this.mLabelList.IndexOf(this.mHighlightedLabel);
            if (num == -1)
            {
                num = 0;
            }
            if (key == KeyCode.UpArrow)
            {
                if (num > 0)
                {
                    this.Select(this.mLabelList[num - 1], false);
                }
            }
            else if (key == KeyCode.DownArrow && num + 1 < this.mLabelList.Count)
            {
                this.Select(this.mLabelList[num + 1], false);
            }
        }
    }

    private void OnKey(KeyCode key)
    {
        if (base.enabled && UIPopupList.current == this && (key == UICamera.current.cancelKey0 || key == UICamera.current.cancelKey1))
        {
            this.OnSelect(false);
        }
    }

    private void OnDisable()
    {
        this.CloseSelf();
    }

    private void OnSelect(Boolean isSelected)
    {
        if (!isSelected)
        {
            this.CloseSelf();
        }
    }

    public static void Close()
    {
        if (UIPopupList.current != (UnityEngine.Object)null)
        {
            UIPopupList.current.CloseSelf();
            UIPopupList.current = (UIPopupList)null;
        }
    }

    public void CloseSelf()
    {
        if (UIPopupList.mChild != (UnityEngine.Object)null && UIPopupList.current == this)
        {
            base.StopCoroutine("CloseIfUnselected");
            this.mSelection = (GameObject)null;
            this.mLabelList.Clear();
            if (this.isAnimated)
            {
                UIWidget[] componentsInChildren = UIPopupList.mChild.GetComponentsInChildren<UIWidget>();
                Int32 i = 0;
                Int32 num = (Int32)componentsInChildren.Length;
                while (i < num)
                {
                    UIWidget uiwidget = componentsInChildren[i];
                    Color color = uiwidget.color;
                    color.a = 0f;
                    TweenColor.Begin(uiwidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
                    i++;
                }
                Collider[] componentsInChildren2 = UIPopupList.mChild.GetComponentsInChildren<Collider>();
                Int32 j = 0;
                Int32 num2 = (Int32)componentsInChildren2.Length;
                while (j < num2)
                {
                    componentsInChildren2[j].enabled = false;
                    j++;
                }
                UnityEngine.Object.Destroy(UIPopupList.mChild, 0.15f);
                UIPopupList.mFadeOutComplete = Time.unscaledTime + Mathf.Max(0.1f, 0.15f);
            }
            else
            {
                UnityEngine.Object.Destroy(UIPopupList.mChild);
                UIPopupList.mFadeOutComplete = Time.unscaledTime + 0.1f;
            }
            this.mBackground = (UISprite)null;
            this.mHighlight = (UISprite)null;
            UIPopupList.mChild = (GameObject)null;
            UIPopupList.current = (UIPopupList)null;
        }
    }

    private void AnimateColor(UIWidget widget)
    {
        Color color = widget.color;
        widget.color = new Color(color.r, color.g, color.b, 0f);
        TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
    }

    private void AnimatePosition(UIWidget widget, Boolean placeAbove, Single bottom)
    {
        Vector3 localPosition = widget.cachedTransform.localPosition;
        Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
        widget.cachedTransform.localPosition = localPosition2;
        GameObject gameObject = widget.gameObject;
        TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
    }

    private void AnimateScale(UIWidget widget, Boolean placeAbove, Single bottom)
    {
        GameObject gameObject = widget.gameObject;
        Transform cachedTransform = widget.cachedTransform;
        Single num = (Single)this.activeFontSize * this.activeFontScale + this.mBgBorder * 2f;
        cachedTransform.localScale = new Vector3(1f, num / (Single)widget.height, 1f);
        TweenScale.Begin(gameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
        if (placeAbove)
        {
            Vector3 localPosition = cachedTransform.localPosition;
            cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - (Single)widget.height + num, localPosition.z);
            TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
        }
    }

    private void Animate(UIWidget widget, Boolean placeAbove, Single bottom)
    {
        this.AnimateColor(widget);
        this.AnimatePosition(widget, placeAbove, bottom);
    }

    private void OnClick()
    {
        if (this.mOpenFrame == Time.frameCount)
        {
            return;
        }
        if (UIPopupList.mChild == (UnityEngine.Object)null)
        {
            if (this.openOn == UIPopupList.OpenOn.DoubleClick || this.openOn == UIPopupList.OpenOn.Manual)
            {
                return;
            }
            if (this.openOn == UIPopupList.OpenOn.RightClick && UICamera.currentTouchID != -2)
            {
                return;
            }
            this.Show();
        }
        else if (this.mHighlightedLabel != (UnityEngine.Object)null)
        {
            this.OnItemPress(this.mHighlightedLabel.gameObject, true);
        }
    }

    private void OnDoubleClick()
    {
        if (this.openOn == UIPopupList.OpenOn.DoubleClick)
        {
            this.Show();
        }
    }

    private IEnumerator CloseIfUnselected()
    {
        do
        {
            yield return null;
        }
        while (!(UICamera.selectedObject != this.mSelection));
        this.CloseSelf();
        yield break;
    }

    public void Show()
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && UIPopupList.mChild == (UnityEngine.Object)null && this.atlas != (UnityEngine.Object)null && this.isValid && this.items.Count > 0)
        {
            this.mLabelList.Clear();
            base.StopCoroutine("CloseIfUnselected");
            UICamera.selectedObject = (UICamera.hoveredObject ?? base.gameObject);
            this.mSelection = UICamera.selectedObject;
            this.source = UICamera.selectedObject;
            if (this.source == (UnityEngine.Object)null)
            {
                global::Debug.LogError("Popup list needs a source object...");
                return;
            }
            this.mOpenFrame = Time.frameCount;
            if (this.mPanel == (UnityEngine.Object)null)
            {
                this.mPanel = UIPanel.Find(base.transform);
                if (this.mPanel == (UnityEngine.Object)null)
                {
                    return;
                }
            }
            UIPopupList.mChild = new GameObject("Drop-down List");
            UIPopupList.mChild.layer = base.gameObject.layer;
            UIPopupList.current = this;
            Transform transform = UIPopupList.mChild.transform;
            transform.parent = this.mPanel.cachedTransform;
            Vector3 localPosition;
            Vector3 vector;
            Vector3 v;
            if (this.openOn == UIPopupList.OpenOn.Manual && this.mSelection != base.gameObject)
            {
                localPosition = UICamera.lastEventPosition;
                vector = this.mPanel.cachedTransform.InverseTransformPoint(this.mPanel.anchorCamera.ScreenToWorldPoint(localPosition));
                v = vector;
                transform.localPosition = vector;
                localPosition = transform.position;
            }
            else
            {
                Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, base.transform, false, false);
                vector = bounds.min;
                v = bounds.max;
                transform.localPosition = vector;
                localPosition = transform.position;
            }
            base.StartCoroutine("CloseIfUnselected");
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            this.mBackground = NGUITools.AddSprite(UIPopupList.mChild, this.atlas, this.backgroundSprite);
            this.mBackground.pivot = UIWidget.Pivot.TopLeft;
            this.mBackground.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
            this.mBackground.color = this.backgroundColor;
            Vector4 border = this.mBackground.border;
            this.mBgBorder = border.y;
            this.mBackground.cachedTransform.localPosition = new Vector3(0f, border.y, 0f);
            this.mHighlight = NGUITools.AddSprite(UIPopupList.mChild, this.atlas, this.highlightSprite);
            this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
            this.mHighlight.color = this.highlightColor;
            UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
            if (atlasSprite == null)
            {
                return;
            }
            Single num = (Single)atlasSprite.borderTop;
            Single num2 = (Single)this.activeFontSize;
            Single activeFontScale = this.activeFontScale;
            Single num3 = num2 * activeFontScale;
            Single num4 = 0f;
            Single num5 = -this.padding.y;
            List<UILabel> list = new List<UILabel>();
            if (!this.items.Contains(this.mSelectedItem))
            {
                this.mSelectedItem = (String)null;
            }
            Int32 i = 0;
            Int32 count = this.items.Count;
            while (i < count)
            {
                String text = this.items[i];
                UILabel uilabel = NGUITools.AddWidget<UILabel>(UIPopupList.mChild);
                uilabel.name = i.ToString();
                uilabel.pivot = UIWidget.Pivot.TopLeft;
                uilabel.bitmapFont = this.bitmapFont;
                uilabel.trueTypeFont = this.trueTypeFont;
                uilabel.fontSize = this.fontSize;
                uilabel.fontStyle = this.fontStyle;
                uilabel.rawText = ((!this.isLocalized) ? text : Localization.Get(text));
                uilabel.color = this.textColor;
                uilabel.cachedTransform.localPosition = new Vector3(border.x + this.padding.x - uilabel.pivotOffset.x, num5, -1f);
                uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
                uilabel.alignment = this.alignment;
                list.Add(uilabel);
                num5 -= num3;
                num5 -= this.padding.y;
                num4 = Mathf.Max(num4, uilabel.printedSize.x);
                UIEventListener uieventListener = UIEventListener.Get(uilabel.gameObject);
                uieventListener.onHover = new UIEventListener.BoolDelegate(this.OnItemHover);
                uieventListener.onPress = new UIEventListener.BoolDelegate(this.OnItemPress);
                uieventListener.parameter = text;
                if (this.mSelectedItem == text || (i == 0 && String.IsNullOrEmpty(this.mSelectedItem)))
                {
                    this.Highlight(uilabel, true);
                }
                this.mLabelList.Add(uilabel);
                i++;
            }
            num4 = Mathf.Max(num4, v.x - vector.x - (border.x + this.padding.x) * 2f);
            Single num6 = num4;
            Vector3 vector2 = new Vector3(num6 * 0.5f, -num3 * 0.5f, 0f);
            Vector3 vector3 = new Vector3(num6, num3 + this.padding.y, 1f);
            Int32 j = 0;
            Int32 count2 = list.Count;
            while (j < count2)
            {
                UILabel uilabel2 = list[j];
                NGUITools.AddWidgetCollider(uilabel2.gameObject);
                uilabel2.autoResizeBoxCollider = false;
                BoxCollider component = uilabel2.GetComponent<BoxCollider>();
                if (component != (UnityEngine.Object)null)
                {
                    vector2.z = component.center.z;
                    component.center = vector2;
                    component.size = vector3;
                }
                else
                {
                    BoxCollider2D component2 = uilabel2.GetComponent<BoxCollider2D>();
                    component2.offset = vector2;
                    component2.size = vector3;
                }
                j++;
            }
            Int32 width = Mathf.RoundToInt(num4);
            num4 += (border.x + this.padding.x) * 2f;
            num5 -= border.y;
            this.mBackground.width = Mathf.RoundToInt(num4);
            this.mBackground.height = Mathf.RoundToInt(-num5 + border.y);
            Int32 k = 0;
            Int32 count3 = list.Count;
            while (k < count3)
            {
                UILabel uilabel3 = list[k];
                uilabel3.overflowMethod = UILabel.Overflow.ShrinkContent;
                uilabel3.width = width;
                k++;
            }
            Single num7 = 2f * this.atlas.pixelSize;
            Single f = num4 - (border.x + this.padding.x) * 2f + (Single)atlasSprite.borderLeft * num7;
            Single f2 = num3 + num * num7;
            this.mHighlight.width = Mathf.RoundToInt(f);
            this.mHighlight.height = Mathf.RoundToInt(f2);
            Boolean flag = this.position == UIPopupList.Position.Above;
            if (this.position == UIPopupList.Position.Auto)
            {
                UICamera uicamera = UICamera.FindCameraForLayer(this.mSelection.layer);
                if (uicamera != (UnityEngine.Object)null)
                {
                    flag = (uicamera.cachedCamera.WorldToViewportPoint(localPosition).y < 0.5f);
                }
            }
            if (this.isAnimated)
            {
                this.AnimateColor(this.mBackground);
                if (Time.timeScale == 0f || Time.timeScale >= 0.1f)
                {
                    Single bottom = num5 + num3;
                    this.Animate(this.mHighlight, flag, bottom);
                    Int32 l = 0;
                    Int32 count4 = list.Count;
                    while (l < count4)
                    {
                        this.Animate(list[l], flag, bottom);
                        l++;
                    }
                    this.AnimateScale(this.mBackground, flag, bottom);
                }
            }
            if (flag)
            {
                vector.y = v.y - border.y;
                v.y = vector.y + (Single)this.mBackground.height;
                v.x = vector.x + (Single)this.mBackground.width;
                transform.localPosition = new Vector3(vector.x, v.y - border.y, vector.z);
            }
            else
            {
                v.y = vector.y + border.y;
                vector.y = v.y - (Single)this.mBackground.height;
                v.x = vector.x + (Single)this.mBackground.width;
            }
            Transform parent = this.mPanel.cachedTransform.parent;
            if (parent != (UnityEngine.Object)null)
            {
                vector = this.mPanel.cachedTransform.TransformPoint(vector);
                v = this.mPanel.cachedTransform.TransformPoint(v);
                vector = parent.InverseTransformPoint(vector);
                v = parent.InverseTransformPoint(v);
            }
            Vector3 b = this.mPanel.CalculateConstrainOffset(vector, v);
            localPosition = transform.localPosition + b;
            localPosition.x = Mathf.Round(localPosition.x);
            localPosition.y = Mathf.Round(localPosition.y);
            transform.localPosition = localPosition;
        }
        else
        {
            this.OnSelect(false);
        }
    }

    private const Single animSpeed = 0.15f;

    public static UIPopupList current;

    private static GameObject mChild;

    private static Single mFadeOutComplete;

    public UIAtlas atlas;

    public UIFont bitmapFont;

    public Font trueTypeFont;

    public Int32 fontSize = 16;

    public FontStyle fontStyle;

    public String backgroundSprite;

    public String highlightSprite;

    public UIPopupList.Position position;

    public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

    public List<String> items = new List<String>();

    public List<Object> itemData = new List<Object>();

    public Vector2 padding = new Vector3(4f, 4f);

    public Color textColor = Color.white;

    public Color backgroundColor = Color.white;

    public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

    public Boolean isAnimated = true;

    public Boolean isLocalized;

    public UIPopupList.OpenOn openOn;

    public List<EventDelegate> onChange = new List<EventDelegate>();

    [SerializeField]
    [HideInInspector]
    private String mSelectedItem;

    [HideInInspector]
    [SerializeField]
    private UIPanel mPanel;

    [HideInInspector]
    [SerializeField]
    private UISprite mBackground;

    [HideInInspector]
    [SerializeField]
    private UISprite mHighlight;

    [HideInInspector]
    [SerializeField]
    private UILabel mHighlightedLabel;

    [SerializeField]
    [HideInInspector]
    private List<UILabel> mLabelList = new List<UILabel>();

    [SerializeField]
    [HideInInspector]
    private Single mBgBorder;

    [NonSerialized]
    private GameObject mSelection;

    [NonSerialized]
    private Int32 mOpenFrame;

    [SerializeField]
    [HideInInspector]
    private GameObject eventReceiver;

    [HideInInspector]
    [SerializeField]
    private String functionName = "OnSelectionChange";

    [HideInInspector]
    [SerializeField]
    private Single textScale;

    [HideInInspector]
    [SerializeField]
    private UIFont font;

    [SerializeField]
    [HideInInspector]
    private UILabel textLabel;

    private UIPopupList.LegacyEvent mLegacyEvent;

    [NonSerialized]
    private Boolean mExecuting;

    private Boolean mUseDynamicFont;

    private Boolean mTweening;

    public GameObject source;

    public enum Position
    {
        Auto,
        Above,
        Below
    }

    public enum OpenOn
    {
        ClickOrTap,
        RightClick,
        DoubleClick,
        Manual
    }

    public delegate void LegacyEvent(String val);
}
