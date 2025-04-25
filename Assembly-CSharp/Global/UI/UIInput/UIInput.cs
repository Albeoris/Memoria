using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Memoria.Assets;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
    public String defaultText
    {
        get
        {
            if (this.mDoInit)
                this.Init();
            return this.mDefaultText;
        }
        set
        {
            if (this.mDoInit)
                this.Init();
            this.mDefaultText = value;
            this.UpdateLabel();
        }
    }

    public Boolean inputShouldBeHidden => this.hideInput && this.label != null && !this.label.multiLine && this.inputType != UIInput.InputType.Password;

    public String value
    {
        get
        {
            if (this.mDoInit)
                this.Init();
            return this.mValue;
        }
        set
        {
            if (this.mDoInit)
                this.Init();
            UIInput.mDrawStart = 0;
            if (Application.platform == RuntimePlatform.BlackBerryPlayer)
                value = value.Replace("\\b", "\b");
            value = this.Validate(value);
            if (this.mValue != value)
            {
                this.mValue = value;
                this.mLoadSavedValue = false;
                if (this.isSelected)
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        this.mSelectionRangeStart = 0;
                        this.mSelectionCaret = 0;
                    }
                    else
                    {
                        this.mSelectionRangeStart = value.Length;
                        this.mSelectionCaret = this.mSelectionRangeStart;
                    }
                }
                else
                {
                    this.SaveToPlayerPrefs(value);
                }
                this.UpdateLabel();
                this.ExecuteOnChange();
            }
        }
    }

    public Boolean isSelected
    {
        get => UIInput.selection == this;
        set
        {
            if (value)
                UICamera.selectedObject = base.gameObject;
            else if (this.isSelected)
                UICamera.selectedObject = null;
        }
    }

    public Int32 cursorPosition
    {
        get => this.isSelected ? this.mSelectionCaret : this.value.Length;
        set
        {
            if (this.isSelected)
            {
                this.mSelectionCaret = value;
                this.UpdateLabel();
            }
        }
    }

    public Int32 selectionStart
    {
        get => this.isSelected ? this.mSelectionRangeStart : this.value.Length;
        set
        {
            if (this.isSelected)
            {
                this.mSelectionRangeStart = value;
                this.UpdateLabel();
            }
        }
    }

    public Int32 selectionEnd
    {
        get => this.isSelected ? this.mSelectionCaret : this.value.Length;
        set
        {
            if (this.isSelected)
            {
                this.mSelectionCaret = value;
                this.UpdateLabel();
            }
        }
    }

    public String Validate(String text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;
        StringBuilder updatedText = new StringBuilder(text.Length);
        foreach (Char c in text)
        {
            Char ch = c;
            if (this.onValidate != null)
                ch = this.onValidate(updatedText.ToString(), updatedText.Length, ch);
            else if (this.validation != UIInput.Validation.None)
                ch = this.Validate(updatedText.ToString(), updatedText.Length, ch);
            if (ch != '\0')
                updatedText.Append(ch);
        }
        if (this.characterLimit > 0 && updatedText.Length > this.characterLimit)
            return updatedText.ToString(0, this.characterLimit);
        return updatedText.ToString();
    }

    private void Start()
    {
        if (this.selectOnTab != null)
        {
            UIKeyNavigation navig = base.GetComponent<UIKeyNavigation>();
            if (navig == null)
            {
                navig = base.gameObject.AddComponent<UIKeyNavigation>();
                navig.onDown = this.selectOnTab;
            }
            this.selectOnTab = null;
            NGUITools.SetDirty(this);
        }
        if (this.mLoadSavedValue && !String.IsNullOrEmpty(this.savedAs))
            this.LoadValue();
        else
            this.value = this.mValue.Replace("\\n", "\n");
        this.isAndroidTV = FF9StateSystem.AndroidTVPlatform;
    }

    protected void Init()
    {
        if (this.mDoInit && this.label != null)
        {
            this.mDoInit = false;
            this.mDefaultText = this.label.rawText;
            this.mDefaultColor = this.label.color;
            this.label.supportEncoding = false;
            if (this.label.alignment == NGUIText.Alignment.Justified)
            {
                this.label.alignment = NGUIText.Alignment.Left;
                global::Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
            }
            this.mPivot = this.label.pivot;
            this.mPosition = this.label.cachedTransform.localPosition.x;
            this.UpdateLabel();
        }
    }

    protected void SaveToPlayerPrefs(String val)
    {
        if (!String.IsNullOrEmpty(this.savedAs))
        {
            if (String.IsNullOrEmpty(val))
                PlayerPrefs.DeleteKey(this.savedAs);
            else
                PlayerPrefs.SetString(this.savedAs, val);
        }
    }

    protected virtual void OnSelect(Boolean isSelected)
    {
        if (isSelected)
        {
            if (this.mOnGUI == null)
                this.mOnGUI = base.gameObject.AddComponent<UIInputOnGUI>();
            this.OnSelectEvent();
        }
        else
        {
            if (this.mOnGUI != null)
            {
                UnityEngine.Object.Destroy(this.mOnGUI);
                this.mOnGUI = null;
            }
            this.OnDeselectEvent();
        }
    }

    protected void OnSelectEvent()
    {
        this.mSelectTime = Time.frameCount;
        UIInput.selection = this;
        if (this.mDoInit)
            this.Init();
        if (this.label != null && NGUITools.GetActive(this))
            this.mSelectMe = Time.frameCount;
    }

    protected void OnDeselectEvent()
    {
        if (this.mDoInit)
            this.Init();
        if (this.label != null && NGUITools.GetActive(this))
        {
            this.mValue = this.value;
            if (String.IsNullOrEmpty(this.mValue))
            {
                this.label.rawText = this.mDefaultText;
                this.label.color = this.mDefaultColor;
            }
            else
            {
                this.label.rawText = this.mValue;
            }
            Input.imeCompositionMode = IMECompositionMode.Auto;
            this.RestoreLabelPivot();
        }
        UIInput.selection = null;
        this.UpdateLabel();
    }

    protected virtual void Update()
    {
        if (!this.isSelected || this.mSelectTime == Time.frameCount)
            return;
        if (this.mDoInit)
            this.Init();
        if (this.mSelectMe != -1 && this.mSelectMe != Time.frameCount)
        {
            this.ignoreChange = true;
            this.mSelectMe = -1;
            this.mSelectionCaret = String.IsNullOrEmpty(this.mValue) ? 0 : this.mValue.Length;
            this.mSelectionRangeStart = this.selectAllTextOnFocus ? 0 : this.mSelectionCaret;
            this.label.color = this.activeTextColor;
            UIInput.mDrawStart = 0;
            Vector2 cursorPos = UICamera.current == null || UICamera.current.cachedCamera == null ? this.label.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(this.label.worldCorners[0]);
            cursorPos.y = Screen.height - cursorPos.y;
            Input.imeCompositionMode = IMECompositionMode.On;
            Input.compositionCursorPos = cursorPos;
            this.UpdateLabel();
            if (String.IsNullOrEmpty(Input.inputString))
                return;
        }
        String compositionString = Input.compositionString;
        if (String.IsNullOrEmpty(compositionString) && !String.IsNullOrEmpty(Input.inputString))
            foreach (Char c in Input.inputString)
                if (c >= ' ' && c != 0xF700 && c != 0xF701 && c != 0xF702 && c != 0xF703) // Exceptions seem to be F1-F4
                    this.Insert(c.ToString());
        if (UIInput.mLastIME != compositionString)
        {
            this.mSelectionCaret = String.IsNullOrEmpty(compositionString) ? this.mSelectionRangeStart : this.mValue.Length + compositionString.Length;
            UIInput.mLastIME = compositionString;
            this.UpdateLabel();
            this.ExecuteOnChange();
        }
        if (this.mSelectionStartTag != null && this.mSelectionEndTag != null && !this.label.Parser.ParsedTagList.Contains(this.mSelectionStartTag))
        {
            this.label.Parser.InsertTag(this.mSelectionStartTag, this.mSelectionStartTag.TextOffset);
            this.label.Parser.InsertTag(this.mSelectionEndTag, this.mSelectionEndTag.TextOffset);
            this.label.MarkAsChanged();
        }
        if (this.mCaretStartTag != null && this.mCaretEndTag != null)
        {
            if (this.mNextBlink < RealTime.time)
            {
                this.mNextBlink = RealTime.time + 0.5f;
                this.mBlinkSwitch = !this.mBlinkSwitch;
            }
            if (this.mBlinkSwitch && !this.label.Parser.ParsedTagList.Contains(this.mCaretStartTag))
            {
                this.label.Parser.InsertTag(this.mCaretStartTag, this.mCaretStartTag.TextOffset);
                this.label.Parser.InsertTag(this.mCaretEndTag, this.mCaretEndTag.TextOffset);
                this.label.MarkAsChanged();
            }
            else if (!this.mBlinkSwitch && this.label.Parser.ParsedTagList.Contains(this.mCaretStartTag))
            {
                this.label.Parser.ParsedTagList.Remove(this.mCaretStartTag);
                this.label.Parser.ParsedTagList.Remove(this.mCaretEndTag);
                this.label.MarkAsChanged();
            }
        }
        if (this.isSelected && this.mLastAlpha != this.label.finalAlpha)
            this.UpdateLabel();
        if (this.mCam == null)
            this.mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
        if (this.mCam != null)
        {
            if (UICamera.GetKeyDown(this.mCam.submitKey0))
            {
                Boolean insertNewLine = this.onReturnKey == UIInput.OnReturnKey.NewLine || (this.onReturnKey == UIInput.OnReturnKey.Default && this.label.multiLine && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && this.label.overflowMethod != UILabel.Overflow.ClampContent && this.validation == UIInput.Validation.None);
                if (insertNewLine)
                {
                    this.Insert("\n");
                }
                else
                {
                    if (UICamera.controller.current != null)
                        UICamera.controller.clickNotification = UICamera.ClickNotification.None;
                    UICamera.currentKey = this.mCam.submitKey0;
                    this.Submit();
                }
            }
            if (UICamera.GetKeyDown(this.mCam.submitKey1))
            {
                Boolean insertNewLine = this.onReturnKey == UIInput.OnReturnKey.NewLine || (this.onReturnKey == UIInput.OnReturnKey.Default && this.label.multiLine && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && this.label.overflowMethod != UILabel.Overflow.ClampContent && this.validation == UIInput.Validation.None);
                if (insertNewLine)
                {
                    this.Insert("\n");
                }
                else
                {
                    if (UICamera.controller.current != null)
                        UICamera.controller.clickNotification = UICamera.ClickNotification.None;
                    UICamera.currentKey = this.mCam.submitKey1;
                    this.Submit();
                }
            }
            if (!this.mCam.useKeyboard && UICamera.GetKeyUp(KeyCode.Tab))
                this.OnKey(KeyCode.Tab);
        }
    }

    private void OnKey(KeyCode key)
    {
        Int32 frameCountNow = Time.frameCount;
        if (UIInput.mIgnoreKey == frameCountNow)
            return;
        if (key == this.mCam.cancelKey0 || key == this.mCam.cancelKey1)
        {
            UIInput.mIgnoreKey = frameCountNow;
            this.isSelected = false;
        }
        else if (key == KeyCode.Tab)
        {
            UIInput.mIgnoreKey = frameCountNow;
            this.isSelected = false;
            UIKeyNavigation navig = base.GetComponent<UIKeyNavigation>();
            if (navig != null)
                navig.OnKey(KeyCode.Tab);
        }
    }

    protected void DoBackspace()
    {
        if (!String.IsNullOrEmpty(this.mValue))
        {
            if (this.mSelectionRangeStart == this.mSelectionCaret)
            {
                if (this.mSelectionRangeStart < 1)
                    return;
                this.mSelectionCaret--;
            }
            this.Insert(String.Empty);
        }
    }

    public virtual Boolean ProcessEvent(Event ev)
    {
        if (this.label == null)
            return false;
        RuntimePlatform platform = Application.platform;
        Boolean isMacPlatform = platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.OSXWebPlayer;
        Boolean ctrlPressed = isMacPlatform ? (ev.modifiers & EventModifiers.Command) != EventModifiers.None : (ev.modifiers & EventModifiers.Control) != EventModifiers.None;
        if ((ev.modifiers & EventModifiers.Alt) != EventModifiers.None)
            ctrlPressed = false;
        Boolean shiftPressed = (ev.modifiers & EventModifiers.Shift) != EventModifiers.None;
        switch (ev.keyCode)
        {
            case KeyCode.UpArrow:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    this.mSelectionCaret = this.label.GetCharacterIndexAtPosition(this.label.transform.TransformPoint(this.label.Parser.GetCharacterRenderRect(this.mSelectionCaret).center) + new Vector3(0f, this.label.defaultFontSize + this.label.effectiveSpacingY));
                    if (this.mSelectionCaret != 0)
                        this.mSelectionCaret += UIInput.mDrawStart;
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.DownArrow:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    this.mSelectionCaret = this.label.GetCharacterIndexAtPosition(this.label.transform.TransformPoint(this.label.Parser.GetCharacterRenderRect(this.mSelectionCaret).center) - new Vector3(0f, this.label.defaultFontSize + this.label.effectiveSpacingY));
                    if (this.mSelectionCaret != this.label.Parser.ParsedText.Length)
                        this.mSelectionCaret += UIInput.mDrawStart;
                    else
                        this.mSelectionCaret = this.mValue.Length;
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.RightArrow:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    this.mSelectionCaret = Mathf.Min(this.mSelectionCaret + 1, this.mValue.Length);
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.LeftArrow:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    this.mSelectionCaret = Mathf.Max(this.mSelectionCaret - 1, 0);
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.A:
                if (ctrlPressed)
                {
                    ev.Use();
                    this.mSelectionRangeStart = 0;
                    this.mSelectionCaret = this.mValue.Length;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.V:
                if (ctrlPressed)
                {
                    ev.Use();
                    this.Insert(NGUITools.clipboard);
                }
                return true;
            case KeyCode.X:
                if (ctrlPressed)
                {
                    ev.Use();
                    NGUITools.clipboard = this.GetSelection();
                    this.Insert(String.Empty);
                }
                return true;
            case KeyCode.C:
                if (ctrlPressed)
                {
                    ev.Use();
                    NGUITools.clipboard = this.GetSelection();
                }
                return true;
            case KeyCode.Backspace:
                ev.Use();
                this.DoBackspace();
                return true;
            case KeyCode.Delete:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    if (this.mSelectionRangeStart == this.mSelectionCaret)
                    {
                        if (this.mSelectionRangeStart >= this.mValue.Length)
                            return true;
                        this.mSelectionCaret++;
                    }
                    this.Insert(String.Empty);
                }
                return true;
            case KeyCode.Home:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    Int32 newLineIndex = this.mValue.LastIndexOf('\n', 0, this.mSelectionCaret);
                    this.mSelectionCaret = newLineIndex >= 0 ? newLineIndex + 1 : 0;
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.End:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    Int32 newLineIndex = this.mValue.IndexOf('\n', this.mSelectionCaret);
                    this.mSelectionCaret = newLineIndex >= 0 ? newLineIndex : this.mValue.Length;
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.PageUp:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    this.mSelectionCaret = 0;
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            case KeyCode.PageDown:
                ev.Use();
                if (!String.IsNullOrEmpty(this.mValue))
                {
                    this.mSelectionCaret = this.mValue.Length;
                    if (!shiftPressed)
                        this.mSelectionRangeStart = this.mSelectionCaret;
                    this.UpdateLabel();
                }
                return true;
            default:
                return false;
        }
    }

    protected virtual void Insert(String text)
    {
        String leftText = this.GetLeftText();
        String rightText = this.GetRightText();
        StringBuilder updatedText = new StringBuilder(leftText.Length + rightText.Length + text.Length);
        updatedText.Append(leftText);
        for (Int32 i = 0; i < text.Length; i++)
        {
            Char ch = text[i];
            if (ch == '\b')
            {
                this.DoBackspace();
            }
            else
            {
                if (this.characterLimit > 0 && updatedText.Length + rightText.Length >= this.characterLimit)
                    break;
                if (this.onValidate != null)
                    ch = this.onValidate(updatedText.ToString(), updatedText.Length, ch);
                else if (this.validation != UIInput.Validation.None)
                    ch = this.Validate(updatedText.ToString(), updatedText.Length, ch);
                if (ch != '\0')
                    updatedText.Append(ch);
            }
        }
        this.mSelectionRangeStart = updatedText.Length;
        this.mSelectionCaret = this.mSelectionRangeStart;
        for (Int32 i = 0; i < rightText.Length; i++)
        {
            Char ch = rightText[i];
            if (this.onValidate != null)
                ch = this.onValidate(updatedText.ToString(), updatedText.Length, ch);
            else if (this.validation != UIInput.Validation.None)
                ch = this.Validate(updatedText.ToString(), updatedText.Length, ch);
            if (ch != '\0')
                updatedText.Append(ch);
        }
        this.mValue = updatedText.ToString();
        this.UpdateLabel();
        this.ExecuteOnChange();
    }

    protected String GetLeftText()
    {
        Int32 start = Mathf.Min(this.mSelectionRangeStart, this.mSelectionCaret);
        return !String.IsNullOrEmpty(this.mValue) && start >= 0 ? this.mValue.Substring(0, start) : String.Empty;
    }

    protected String GetRightText()
    {
        Int32 end = Mathf.Max(this.mSelectionRangeStart, this.mSelectionCaret);
        return !String.IsNullOrEmpty(this.mValue) && end < this.mValue.Length ? this.mValue.Substring(end) : String.Empty;
    }

    protected String GetSelection()
    {
        if (String.IsNullOrEmpty(this.mValue) || this.mSelectionRangeStart == this.mSelectionCaret)
            return String.Empty;
        Int32 start = Mathf.Min(this.mSelectionRangeStart, this.mSelectionCaret);
        Int32 end = Mathf.Max(this.mSelectionRangeStart, this.mSelectionCaret);
        return this.mValue.Substring(start, end - start);
    }

    protected Int32 GetCharUnderMouse()
    {
        Vector3[] worldCorners = this.label.worldCorners;
        Ray currentRay = UICamera.currentRay;
        Plane plane = new Plane(worldCorners[0], worldCorners[1], worldCorners[2]);
        return plane.Raycast(currentRay, out Single distance) ? UIInput.mDrawStart + this.label.GetCharacterIndexAtPosition(currentRay.GetPoint(distance)) : 0;
    }

    protected virtual void OnPress(Boolean isPressed)
    {
        if (isPressed && this.isSelected && this.label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
        {
            this.selectionEnd = this.GetCharUnderMouse();
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                this.selectionStart = this.mSelectionCaret;
        }
    }

    protected virtual void OnDrag(Vector2 delta)
    {
        if (this.label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
            this.selectionEnd = this.GetCharUnderMouse();
    }

    private void OnDisable()
    {
        if (this.mCaretStartTag != null && this.mCaretEndTag != null)
            if (this.label.Parser.ParsedTagList.Remove(this.mCaretStartTag) | this.label.Parser.ParsedTagList.Remove(this.mCaretEndTag))
                this.label.MarkAsChanged();
    }

    public void Submit()
    {
        if (NGUITools.GetActive(this))
        {
            this.mValue = this.value;
            if (UIInput.current == null)
            {
                UIInput.current = this;
                EventDelegate.Execute(this.onSubmit);
                UIInput.current = null;
            }
            this.SaveToPlayerPrefs(this.mValue);
        }
    }

    public void UpdateLabel()
    {
        if (this.label != null)
        {
            if (this.mDoInit)
                this.Init();
            Boolean isSelected = this.isSelected;
            String currentValue = this.value;
            Boolean isEmpty = String.IsNullOrEmpty(currentValue) && String.IsNullOrEmpty(Input.compositionString);
            this.label.color = (!isEmpty || isSelected) ? this.activeTextColor : this.mDefaultColor;
            String labelText;
            if (isEmpty)
            {
                labelText = isSelected ? String.Empty : this.mDefaultText;
                this.RestoreLabelPivot();
            }
            else
            {
                if (this.inputType == UIInput.InputType.Password)
                {
                    labelText = String.Empty;
                    String passwordChar = "*";
                    if (this.label.bitmapFont != null && this.label.bitmapFont.bmFont != null && this.label.bitmapFont.bmFont.GetGlyph('*') == null)
                        passwordChar = "x";
                    for (Int32 i = 0; i < currentValue.Length; i++)
                        labelText += passwordChar;
                }
                else
                {
                    labelText = currentValue;
                }
            }
            Int32 cursorPos = isSelected ? Mathf.Min(labelText.Length, this.cursorPosition) : 0;
            String leftPart = labelText.Substring(0, cursorPos);
            if (isSelected)
                leftPart += Input.compositionString;
            labelText = leftPart + labelText.Substring(cursorPos, labelText.Length - cursorPos);
            if (isSelected && this.label.overflowMethod == UILabel.Overflow.ClampContent && this.label.maxLineCount == 1)
            {
                Int32 overlimitCharCount = this.label.CalculateOffsetToFit(labelText);
                if (overlimitCharCount == 0)
                {
                    UIInput.mDrawStart = 0;
                    this.RestoreLabelPivot();
                }
                else if (cursorPos < UIInput.mDrawStart)
                {
                    UIInput.mDrawStart = cursorPos;
                    this.SetPivotToLeft();
                }
                else if (overlimitCharCount < UIInput.mDrawStart)
                {
                    UIInput.mDrawStart = overlimitCharCount;
                    this.SetPivotToLeft();
                }
                else
                {
                    overlimitCharCount = this.label.CalculateOffsetToFit(labelText.Substring(0, cursorPos));
                    if (overlimitCharCount > UIInput.mDrawStart)
                    {
                        UIInput.mDrawStart = overlimitCharCount;
                        this.SetPivotToRight();
                    }
                }
                if (UIInput.mDrawStart != 0)
                    labelText = labelText.Substring(UIInput.mDrawStart, labelText.Length - UIInput.mDrawStart);
            }
            else
            {
                UIInput.mDrawStart = 0;
                this.RestoreLabelPivot();
            }
            if (String.IsNullOrEmpty(labelText))
                labelText = " ";
            TextParser textParser = new TextParser(this.label, labelText);
            this.label.Parser = textParser;
            this.label.ReprocessCounter = 0;
            textParser.Parse(TextParser.ParseStep.Wrapped);
            this.mSelectionStartTag = null;
            this.mSelectionEndTag = null;
            this.mCaretStartTag = null;
            this.mCaretEndTag = null;
            this.mBlinkSwitch = true;
            if (isSelected)
            {
                Int32 shiftedSelStart = Mathf.Clamp(this.mSelectionRangeStart - UIInput.mDrawStart, 0, labelText.Length);
                Int32 shiftedSelEnd = Mathf.Clamp(this.mSelectionCaret - UIInput.mDrawStart, 0, labelText.Length);
                Int32 caretPos = shiftedSelEnd;
                if (shiftedSelStart > shiftedSelEnd)
                {
                    shiftedSelEnd = shiftedSelStart;
                    shiftedSelStart = caretPos;
                }
                if (shiftedSelStart != shiftedSelEnd)
                {
                    this.mSelectionStartTag = new FFIXTextTag(FFIXTextTagCode.BackgroundRGBA, [this.selectionColor.r.ToString(), this.selectionColor.g.ToString(), this.selectionColor.b.ToString(), this.selectionColor.a.ToString(), "0", "0", "1", "1", "0", "0", "2", "0"]);
                    this.mSelectionEndTag = new FFIXTextTag(FFIXTextTagCode.BackgroundRGBA, ["Off"]);
                    textParser.InsertTag(mSelectionStartTag, shiftedSelStart);
                    textParser.InsertTag(mSelectionEndTag, shiftedSelEnd);
                }
                if (caretPos < labelText.Length)
                {
                    this.mCaretStartTag = new FFIXTextTag(FFIXTextTagCode.BackgroundRGBA, [this.caretColor.r.ToString(), this.caretColor.g.ToString(), this.caretColor.b.ToString(), this.caretColor.a.ToString(), "0", "0", "0", "1", "-1", "-6", "1", "6"]);
                    this.mCaretEndTag = new FFIXTextTag(FFIXTextTagCode.BackgroundRGBA, ["Off"]);
                    textParser.InsertTag(mCaretStartTag, caretPos);
                    textParser.InsertTag(mCaretEndTag, caretPos + 1);
                }
                else if (caretPos > 0)
                {
                    this.mCaretStartTag = new FFIXTextTag(FFIXTextTagCode.BackgroundRGBA, [this.caretColor.r.ToString(), this.caretColor.g.ToString(), this.caretColor.b.ToString(), this.caretColor.a.ToString(), "1", "0", "1", "1", "-1", "-6", "1", "6"]);
                    this.mCaretEndTag = new FFIXTextTag(FFIXTextTagCode.BackgroundRGBA, ["Off"]);
                    textParser.InsertTag(mCaretStartTag, caretPos - 1);
                    textParser.InsertTag(mCaretEndTag, caretPos);
                }
                this.mNextBlink = RealTime.time + 0.5f;
                this.mLastAlpha = this.label.finalAlpha;
            }
        }
    }

    protected void SetPivotToLeft()
    {
        Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.mPivot);
        pivotOffset.x = 0f;
        this.label.pivot = NGUIMath.GetPivot(pivotOffset);
    }

    protected void SetPivotToRight()
    {
        Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.mPivot);
        pivotOffset.x = 1f;
        this.label.pivot = NGUIMath.GetPivot(pivotOffset);
    }

    protected void RestoreLabelPivot()
    {
        if (this.label != null && this.label.pivot != this.mPivot)
            this.label.pivot = this.mPivot;
    }

    protected Char Validate(String text, Int32 pos, Char ch)
    {
        if (this.validation == UIInput.Validation.None || !base.enabled)
            return ch;
        if (this.validation == UIInput.Validation.Integer)
        {
            if (ch >= '0' && ch <= '9')
                return ch;
            if (ch == '-' && pos == 0 && !text.Contains("-"))
                return ch;
        }
        else if (this.validation == UIInput.Validation.Float)
        {
            if (ch >= '0' && ch <= '9')
                return ch;
            if (ch == '-' && pos == 0 && !text.Contains("-"))
                return ch;
            if (ch == '.' && !text.Contains("."))
                return ch;
        }
        else if (this.validation == UIInput.Validation.Alphanumeric)
        {
            if (ch >= 'A' && ch <= 'Z')
                return ch;
            if (ch >= 'a' && ch <= 'z')
                return ch;
            if (ch >= '0' && ch <= '9')
                return ch;
        }
        else if (this.validation == UIInput.Validation.Username)
        {
            if (ch >= 'A' && ch <= 'Z')
                return (Char)(ch - 'A' + 'a');
            if (ch >= 'a' && ch <= 'z')
                return ch;
            if (ch >= '0' && ch <= '9')
                return ch;
        }
        else if (this.validation == UIInput.Validation.Filename)
        {
            if (ch == ':' || ch == '/' || ch == '\\' || ch == '<' || ch == '>' || ch == '|' || ch == '^' || ch == '*' || ch == ';' || ch == '"' || ch == '`' || ch == '\t' || ch == '\n')
                return '\0';
            return ch;
        }
        else if (this.validation == UIInput.Validation.Name)
        {
            Char curCh = text.Length <= 0 ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
            Char nextCh = text.Length <= 0 ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
            if (ch >= 'a' && ch <= 'z')
            {
                if (curCh == ' ')
                    return (Char)(ch - 'a' + 'A');
                return ch;
            }
            else if (ch >= 'A' && ch <= 'Z')
            {
                if (curCh != ' ' && curCh != '\'')
                    return (Char)(ch - 'A' + 'a');
                return ch;
            }
            else if (ch == '\'')
            {
                if (curCh != ' ' && curCh != '\'' && nextCh != '\'' && !text.Contains("'"))
                    return ch;
            }
            else if (ch == ' ' && curCh != ' ' && curCh != '\'' && nextCh != ' ' && nextCh != '\'')
            {
                return ch;
            }
        }
        return '\0';
    }

    protected void ExecuteOnChange()
    {
        if (UIInput.current == null && EventDelegate.IsValid(this.onChange))
        {
            UIInput.current = this;
            EventDelegate.Execute(this.onChange);
            UIInput.current = null;
        }
    }

    public void RemoveFocus()
    {
        this.isSelected = false;
    }

    public void SaveValue()
    {
        this.SaveToPlayerPrefs(this.mValue);
    }

    public void LoadValue()
    {
        if (!String.IsNullOrEmpty(this.savedAs))
        {
            String multilineValue = this.mValue.Replace("\\n", "\n");
            this.mValue = String.Empty;
            this.value = PlayerPrefs.HasKey(this.savedAs) ? PlayerPrefs.GetString(this.savedAs) : multilineValue;
        }
    }

    public static UIInput current;
    public static UIInput selection;

    public UILabel label;

    public UIInput.InputType inputType;
    public UIInput.OnReturnKey onReturnKey;
    public UIInput.KeyboardType keyboardType;

    public Boolean hideInput;

    [NonSerialized]
    public Boolean selectAllTextOnFocus = true;

    public UIInput.Validation validation;
    public Int32 characterLimit;

    public String savedAs;

    [SerializeField]
    [HideInInspector]
    private GameObject selectOnTab;

    public Color activeTextColor = Color.white;
    public Color caretColor = new Color(1f, 1f, 1f, 0.8f);
    public Color selectionColor = new Color(1f, 0.8745098f, 0.5529412f, 0.5f);

    public List<EventDelegate> onSubmit = new List<EventDelegate>();
    public List<EventDelegate> onChange = new List<EventDelegate>();
    public UIInput.OnValidate onValidate;

    [HideInInspector]
    [SerializeField]
    protected String mValue;

    [NonSerialized]
    protected String mDefaultText = String.Empty;

    [NonSerialized]
    protected Color mDefaultColor = Color.white;

    [NonSerialized]
    protected Single mPosition;

    [NonSerialized]
    protected Boolean mDoInit = true;

    [NonSerialized]
    protected UIWidget.Pivot mPivot;

    [NonSerialized]
    protected Boolean mLoadSavedValue = true;

    protected static Int32 mDrawStart;
    protected static String mLastIME = String.Empty;

    [NonSerialized]
    protected Int32 mSelectionRangeStart;

    [NonSerialized]
    protected Int32 mSelectionCaret;

    [NonSerialized]
    protected Boolean mBlinkSwitch;
    [NonSerialized]
    protected FFIXTextTag mCaretStartTag;
    [NonSerialized]
    protected FFIXTextTag mCaretEndTag;
    [NonSerialized]
    protected FFIXTextTag mSelectionStartTag;
    [NonSerialized]
    protected FFIXTextTag mSelectionEndTag;

    [NonSerialized]
    protected Single mNextBlink;

    [NonSerialized]
    protected Single mLastAlpha;

    [NonSerialized]
    protected String mCached = String.Empty;

    [NonSerialized]
    protected Int32 mSelectMe = -1;

    [NonSerialized]
    protected Int32 mSelectTime = -1;

    private Boolean isAndroidTV;
    private Boolean ignoreChange;

    [NonSerialized]
    private UIInputOnGUI mOnGUI;

    [NonSerialized]
    private UICamera mCam;

    private static Int32 mIgnoreKey;

    public enum InputType
    {
        Standard,
        AutoCorrect,
        Password
    }

    public enum Validation
    {
        None,
        Integer,
        Float,
        Alphanumeric,
        Username,
        Name,
        Filename
    }

    public enum KeyboardType
    {
        Default,
        ASCIICapable,
        NumbersAndPunctuation,
        URL,
        NumberPad,
        PhonePad,
        NamePhonePad,
        EmailAddress
    }

    public enum OnReturnKey
    {
        Default,
        Submit,
        NewLine
    }

    public delegate Char OnValidate(String text, Int32 charIndex, Char addedChar);
}
