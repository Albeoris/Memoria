using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public String defaultText
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mDefaultText;
		}
		set
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			this.mDefaultText = value;
			this.UpdateLabel();
		}
	}

	public Boolean inputShouldBeHidden
	{
		get
		{
			return this.hideInput && this.label != (UnityEngine.Object)null && !this.label.multiLine && this.inputType != UIInput.InputType.Password;
		}
	}

	[Obsolete("Use UIInput.value instead")]
	public String text
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

	public String value
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mValue;
		}
		set
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			UIInput.mDrawStart = 0;
			if (Application.platform == RuntimePlatform.BlackBerryPlayer)
			{
				value = value.Replace("\\b", "\b");
			}
			value = this.Validate(value);
			if (this.mValue != value)
			{
				this.mValue = value;
				this.mLoadSavedValue = false;
				if (this.isSelected)
				{
					if (String.IsNullOrEmpty(value))
					{
						this.mSelectionStart = 0;
						this.mSelectionEnd = 0;
					}
					else
					{
						this.mSelectionStart = value.Length;
						this.mSelectionEnd = this.mSelectionStart;
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

	[Obsolete("Use UIInput.isSelected instead")]
	public Boolean selected
	{
		get
		{
			return this.isSelected;
		}
		set
		{
			this.isSelected = value;
		}
	}

	public Boolean isSelected
	{
		get
		{
			return UIInput.selection == this;
		}
		set
		{
			if (!value)
			{
				if (this.isSelected)
				{
					UICamera.selectedObject = (GameObject)null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public Int32 cursorPosition
	{
		get
		{
			return (Int32)((!this.isSelected) ? this.value.Length : this.mSelectionEnd);
		}
		set
		{
			if (this.isSelected)
			{
				this.mSelectionEnd = value;
				this.UpdateLabel();
			}
		}
	}

	public Int32 selectionStart
	{
		get
		{
			return (Int32)((!this.isSelected) ? this.value.Length : this.mSelectionStart);
		}
		set
		{
			if (this.isSelected)
			{
				this.mSelectionStart = value;
				this.UpdateLabel();
			}
		}
	}

	public Int32 selectionEnd
	{
		get
		{
			return (Int32)((!this.isSelected) ? this.value.Length : this.mSelectionEnd);
		}
		set
		{
			if (this.isSelected)
			{
				this.mSelectionEnd = value;
				this.UpdateLabel();
			}
		}
	}

	public UITexture caret
	{
		get
		{
			return this.mCaret;
		}
	}

	public String Validate(String val)
	{
		if (String.IsNullOrEmpty(val))
		{
			return String.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(val.Length);
		foreach (Char c in val)
		{
            Char ch = c;
			if (this.onValidate != null)
			{
                ch = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, ch);
			}
			else if (this.validation != UIInput.Validation.None)
			{
                ch = this.Validate(stringBuilder.ToString(), stringBuilder.Length, ch);
			}
			if (ch != '\0')
			{
				stringBuilder.Append(ch);
			}
		}
		if (this.characterLimit > 0 && stringBuilder.Length > this.characterLimit)
		{
			return stringBuilder.ToString(0, this.characterLimit);
		}
		return stringBuilder.ToString();
	}

	private void Start()
	{
		if (this.selectOnTab != (UnityEngine.Object)null)
		{
			UIKeyNavigation uikeyNavigation = base.GetComponent<UIKeyNavigation>();
			if (uikeyNavigation == (UnityEngine.Object)null)
			{
				uikeyNavigation = base.gameObject.AddComponent<UIKeyNavigation>();
				uikeyNavigation.onDown = this.selectOnTab;
			}
			this.selectOnTab = (GameObject)null;
			NGUITools.SetDirty(this);
		}
		if (this.mLoadSavedValue && !String.IsNullOrEmpty(this.savedAs))
		{
			this.LoadValue();
		}
		else
		{
			this.value = this.mValue.Replace("\\n", "\n");
		}
		this.isAndroidTV = FF9StateSystem.AndroidTVPlatform;
	}

	protected void Init()
	{
		if (this.mDoInit && this.label != (UnityEngine.Object)null)
		{
			this.mDoInit = false;
			this.mDefaultText = this.label.text;
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
			{
				PlayerPrefs.DeleteKey(this.savedAs);
			}
			else
			{
				PlayerPrefs.SetString(this.savedAs, val);
			}
		}
	}

	protected virtual void OnSelect(Boolean isSelected)
	{
		if (isSelected)
		{
			if (this.mOnGUI == (UnityEngine.Object)null)
			{
				this.mOnGUI = base.gameObject.AddComponent<UIInputOnGUI>();
			}
			this.OnSelectEvent();
		}
		else
		{
			if (this.mOnGUI != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy(this.mOnGUI);
				this.mOnGUI = (UIInputOnGUI)null;
			}
			this.OnDeselectEvent();
		}
	}

	protected void OnSelectEvent()
	{
		this.mSelectTime = Time.frameCount;
		UIInput.selection = this;
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.label != (UnityEngine.Object)null && NGUITools.GetActive(this))
		{
			this.mSelectMe = Time.frameCount;
		}
	}

	protected void OnDeselectEvent()
	{
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.label != (UnityEngine.Object)null && NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (String.IsNullOrEmpty(this.mValue))
			{
				this.label.text = this.mDefaultText;
				this.label.color = this.mDefaultColor;
			}
			else
			{
				this.label.text = this.mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			this.RestoreLabelPivot();
		}
		UIInput.selection = (UIInput)null;
		this.UpdateLabel();
	}

	protected virtual void Update()
	{
		if (!this.isSelected || this.mSelectTime == Time.frameCount)
		{
			return;
		}
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.mSelectMe != -1 && this.mSelectMe != Time.frameCount)
		{
			this.ignoreChange = true;
			this.mSelectMe = -1;
			this.mSelectionEnd = (Int32)((!String.IsNullOrEmpty(this.mValue)) ? this.mValue.Length : 0);
			UIInput.mDrawStart = 0;
			this.mSelectionStart = (Int32)((!this.selectAllTextOnFocus) ? this.mSelectionEnd : 0);
			this.label.color = this.activeTextColor;
			Vector2 compositionCursorPos = (!(UICamera.current != (UnityEngine.Object)null) || !(UICamera.current.cachedCamera != (UnityEngine.Object)null)) ? this.label.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(this.label.worldCorners[0]);
			compositionCursorPos.y = (Single)Screen.height - compositionCursorPos.y;
			Input.imeCompositionMode = IMECompositionMode.On;
			Input.compositionCursorPos = compositionCursorPos;
			this.UpdateLabel();
			if (String.IsNullOrEmpty(Input.inputString))
			{
				return;
			}
		}
		String compositionString = Input.compositionString;
		if (String.IsNullOrEmpty(compositionString) && !String.IsNullOrEmpty(Input.inputString))
		{
			foreach (Char c in Input.inputString)
			{
				if (c >= ' ')
				{
					if (c != '')
					{
						if (c != '')
						{
							if (c != '')
							{
								if (c != '')
								{
									this.Insert(c.ToString());
								}
							}
						}
					}
				}
			}
		}
		if (UIInput.mLastIME != compositionString)
		{
			this.mSelectionEnd = (Int32)((!String.IsNullOrEmpty(compositionString)) ? (this.mValue.Length + compositionString.Length) : this.mSelectionStart);
			UIInput.mLastIME = compositionString;
			this.UpdateLabel();
			this.ExecuteOnChange();
		}
		if (this.mCaret != (UnityEngine.Object)null && this.mNextBlink < RealTime.time)
		{
			this.mNextBlink = RealTime.time + 0.5f;
			this.mCaret.enabled = !this.mCaret.enabled;
		}
		if (this.isSelected && this.mLastAlpha != this.label.finalAlpha)
		{
			this.UpdateLabel();
		}
		if (this.mCam == (UnityEngine.Object)null)
		{
			this.mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		}
		if (this.mCam != (UnityEngine.Object)null)
		{
			if (UICamera.GetKeyDown(this.mCam.submitKey0))
			{
				Boolean flag = this.onReturnKey == UIInput.OnReturnKey.NewLine || (this.onReturnKey == UIInput.OnReturnKey.Default && this.label.multiLine && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && this.label.overflowMethod != UILabel.Overflow.ClampContent && this.validation == UIInput.Validation.None);
				if (flag)
				{
					this.Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != (UnityEngine.Object)null)
					{
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					}
					UICamera.currentKey = this.mCam.submitKey0;
					this.Submit();
				}
			}
			if (UICamera.GetKeyDown(this.mCam.submitKey1))
			{
				Boolean flag2 = this.onReturnKey == UIInput.OnReturnKey.NewLine || (this.onReturnKey == UIInput.OnReturnKey.Default && this.label.multiLine && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && this.label.overflowMethod != UILabel.Overflow.ClampContent && this.validation == UIInput.Validation.None);
				if (flag2)
				{
					this.Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != (UnityEngine.Object)null)
					{
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					}
					UICamera.currentKey = this.mCam.submitKey1;
					this.Submit();
				}
			}
			if (!this.mCam.useKeyboard && UICamera.GetKeyUp(KeyCode.Tab))
			{
				this.OnKey(KeyCode.Tab);
			}
		}
	}

	private void OnKey(KeyCode key)
	{
		Int32 frameCount = Time.frameCount;
		if (UIInput.mIgnoreKey == frameCount)
		{
			return;
		}
		if (key == this.mCam.cancelKey0 || key == this.mCam.cancelKey1)
		{
			UIInput.mIgnoreKey = frameCount;
			this.isSelected = false;
		}
		else if (key == KeyCode.Tab)
		{
			UIInput.mIgnoreKey = frameCount;
			this.isSelected = false;
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			if (component != (UnityEngine.Object)null)
			{
				component.OnKey(KeyCode.Tab);
			}
		}
	}

	protected void DoBackspace()
	{
		if (!String.IsNullOrEmpty(this.mValue))
		{
			if (this.mSelectionStart == this.mSelectionEnd)
			{
				if (this.mSelectionStart < 1)
				{
					return;
				}
				this.mSelectionEnd--;
			}
			this.Insert(String.Empty);
		}
	}

	public virtual Boolean ProcessEvent(Event ev)
	{
		if (this.label == (UnityEngine.Object)null)
		{
			return false;
		}
		RuntimePlatform platform = Application.platform;
		Boolean flag = platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.OSXWebPlayer;
		Boolean flag2 = (!flag) ? ((ev.modifiers & EventModifiers.Control) != EventModifiers.None) : ((ev.modifiers & EventModifiers.Command) != EventModifiers.None);
		if ((ev.modifiers & EventModifiers.Alt) != EventModifiers.None)
		{
			flag2 = false;
		}
		Boolean flag3 = (ev.modifiers & EventModifiers.Shift) != EventModifiers.None;
		KeyCode keyCode = ev.keyCode;
		switch (keyCode)
		{
		case KeyCode.UpArrow:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				this.mSelectionEnd = this.label.GetCharacterIndex(this.mSelectionEnd, KeyCode.UpArrow);
				if (this.mSelectionEnd != 0)
				{
					this.mSelectionEnd += UIInput.mDrawStart;
				}
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.DownArrow:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				this.mSelectionEnd = this.label.GetCharacterIndex(this.mSelectionEnd, KeyCode.DownArrow);
				if (this.mSelectionEnd != this.label.processedText.Length)
				{
					this.mSelectionEnd += UIInput.mDrawStart;
				}
				else
				{
					this.mSelectionEnd = this.mValue.Length;
				}
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.RightArrow:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				this.mSelectionEnd = Mathf.Min(this.mSelectionEnd + 1, this.mValue.Length);
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.LeftArrow:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				this.mSelectionEnd = Mathf.Max(this.mSelectionEnd - 1, 0);
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.Insert:
			IL_AD:
			switch (keyCode)
			{
			case KeyCode.A:
				if (flag2)
				{
					ev.Use();
					this.mSelectionStart = 0;
					this.mSelectionEnd = this.mValue.Length;
					this.UpdateLabel();
				}
				return true;
			case KeyCode.B:
				IL_C3:
				switch (keyCode)
				{
				case KeyCode.V:
					if (flag2)
					{
						ev.Use();
						this.Insert(NGUITools.clipboard);
					}
					return true;
				case KeyCode.W:
					IL_D9:
					if (keyCode == KeyCode.Backspace)
					{
						ev.Use();
						this.DoBackspace();
						return true;
					}
					if (keyCode != KeyCode.Delete)
					{
						return false;
					}
					ev.Use();
					if (!String.IsNullOrEmpty(this.mValue))
					{
						if (this.mSelectionStart == this.mSelectionEnd)
						{
							if (this.mSelectionStart >= this.mValue.Length)
							{
								return true;
							}
							this.mSelectionEnd++;
						}
						this.Insert(String.Empty);
					}
					return true;
				case KeyCode.X:
					if (flag2)
					{
						ev.Use();
						NGUITools.clipboard = this.GetSelection();
						this.Insert(String.Empty);
					}
					return true;
				default:
					goto IL_D9;
				}
			case KeyCode.C:
				if (flag2)
				{
					ev.Use();
					NGUITools.clipboard = this.GetSelection();
				}
				return true;
			default:
				goto IL_C3;
			}
		case KeyCode.Home:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				if (this.label.multiLine)
				{
					this.mSelectionEnd = this.label.GetCharacterIndex(this.mSelectionEnd, KeyCode.Home);
				}
				else
				{
					this.mSelectionEnd = 0;
				}
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.End:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				if (this.label.multiLine)
				{
					this.mSelectionEnd = this.label.GetCharacterIndex(this.mSelectionEnd, KeyCode.End);
				}
				else
				{
					this.mSelectionEnd = this.mValue.Length;
				}
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.PageUp:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				this.mSelectionEnd = 0;
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		case KeyCode.PageDown:
			ev.Use();
			if (!String.IsNullOrEmpty(this.mValue))
			{
				this.mSelectionEnd = this.mValue.Length;
				if (!flag3)
				{
					this.mSelectionStart = this.mSelectionEnd;
				}
				this.UpdateLabel();
			}
			return true;
		default:
			goto IL_AD;
		}
	}

	protected virtual void Insert(String text)
	{
		String leftText = this.GetLeftText();
		String rightText = this.GetRightText();
		Int32 length = rightText.Length;
		StringBuilder stringBuilder = new StringBuilder(leftText.Length + rightText.Length + text.Length);
		stringBuilder.Append(leftText);
		Int32 i = 0;
		Int32 length2 = text.Length;
		while (i < length2)
		{
			Char c = text[i];
			if (c == '\b')
			{
				this.DoBackspace();
			}
			else
			{
				if (this.characterLimit > 0 && stringBuilder.Length + length >= this.characterLimit)
				{
					break;
				}
				if (this.onValidate != null)
				{
					c = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
				}
				else if (this.validation != UIInput.Validation.None)
				{
					c = this.Validate(stringBuilder.ToString(), stringBuilder.Length, c);
				}
				if (c != '\0')
				{
					stringBuilder.Append(c);
				}
			}
			i++;
		}
		this.mSelectionStart = stringBuilder.Length;
		this.mSelectionEnd = this.mSelectionStart;
		Int32 j = 0;
		Int32 length3 = rightText.Length;
		while (j < length3)
		{
			Char c2 = rightText[j];
			if (this.onValidate != null)
			{
				c2 = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			else if (this.validation != UIInput.Validation.None)
			{
				c2 = this.Validate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			if (c2 != '\0')
			{
				stringBuilder.Append(c2);
			}
			j++;
		}
		this.mValue = stringBuilder.ToString();
		this.UpdateLabel();
		this.ExecuteOnChange();
	}

	protected String GetLeftText()
	{
		Int32 num = Mathf.Min(this.mSelectionStart, this.mSelectionEnd);
		return (!String.IsNullOrEmpty(this.mValue) && num >= 0) ? this.mValue.Substring(0, num) : String.Empty;
	}

	protected String GetRightText()
	{
		Int32 num = Mathf.Max(this.mSelectionStart, this.mSelectionEnd);
		return (!String.IsNullOrEmpty(this.mValue) && num < this.mValue.Length) ? this.mValue.Substring(num) : String.Empty;
	}

	protected String GetSelection()
	{
		if (String.IsNullOrEmpty(this.mValue) || this.mSelectionStart == this.mSelectionEnd)
		{
			return String.Empty;
		}
		Int32 num = Mathf.Min(this.mSelectionStart, this.mSelectionEnd);
		Int32 num2 = Mathf.Max(this.mSelectionStart, this.mSelectionEnd);
		return this.mValue.Substring(num, num2 - num);
	}

	protected Int32 GetCharUnderMouse()
	{
		Vector3[] worldCorners = this.label.worldCorners;
		Ray currentRay = UICamera.currentRay;
		Plane plane = new Plane(worldCorners[0], worldCorners[1], worldCorners[2]);
		Single distance;
		return (Int32)((!plane.Raycast(currentRay, out distance)) ? 0 : (UIInput.mDrawStart + this.label.GetCharacterIndexAtPosition(currentRay.GetPoint(distance), false)));
	}

	protected virtual void OnPress(Boolean isPressed)
	{
		if (isPressed && this.isSelected && this.label != (UnityEngine.Object)null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			this.selectionEnd = this.GetCharUnderMouse();
			if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				this.selectionStart = this.mSelectionEnd;
			}
		}
	}

	protected virtual void OnDrag(Vector2 delta)
	{
		if (this.label != (UnityEngine.Object)null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			this.selectionEnd = this.GetCharUnderMouse();
		}
	}

	private void OnDisable()
	{
		this.Cleanup();
	}

	protected virtual void Cleanup()
	{
		if (this.mHighlight)
		{
			this.mHighlight.enabled = false;
		}
		if (this.mCaret)
		{
			this.mCaret.enabled = false;
		}
		if (this.mBlankTex)
		{
			NGUITools.Destroy(this.mBlankTex);
			this.mBlankTex = (Texture2D)null;
		}
	}

	public void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (UIInput.current == (UnityEngine.Object)null)
			{
				UIInput.current = this;
				EventDelegate.Execute(this.onSubmit);
				UIInput.current = (UIInput)null;
			}
			this.SaveToPlayerPrefs(this.mValue);
		}
	}

	public void UpdateLabel()
	{
		if (this.label != (UnityEngine.Object)null)
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			Boolean isSelected = this.isSelected;
			String value = this.value;
			Boolean flag = String.IsNullOrEmpty(value) && String.IsNullOrEmpty(Input.compositionString);
			this.label.color = ((!flag || isSelected) ? this.activeTextColor : this.mDefaultColor);
			String text;
			if (flag)
			{
				text = ((!isSelected) ? this.mDefaultText : String.Empty);
				this.RestoreLabelPivot();
			}
			else
			{
				if (this.inputType == UIInput.InputType.Password)
				{
					text = String.Empty;
					String str = "*";
					if (this.label.bitmapFont != (UnityEngine.Object)null && this.label.bitmapFont.bmFont != null && this.label.bitmapFont.bmFont.GetGlyph(42) == null)
					{
						str = "x";
					}
					Int32 i = 0;
					Int32 length = value.Length;
					while (i < length)
					{
						text += str;
						i++;
					}
				}
				else
				{
					text = value;
				}
				Int32 num = (Int32)((!isSelected) ? 0 : Mathf.Min(text.Length, this.cursorPosition));
				String str2 = text.Substring(0, num);
				if (isSelected)
				{
					str2 += Input.compositionString;
				}
				text = str2 + text.Substring(num, text.Length - num);
				if (isSelected && this.label.overflowMethod == UILabel.Overflow.ClampContent && this.label.maxLineCount == 1)
				{
					Int32 num2 = this.label.CalculateOffsetToFit(text);
					if (num2 == 0)
					{
						UIInput.mDrawStart = 0;
						this.RestoreLabelPivot();
					}
					else if (num < UIInput.mDrawStart)
					{
						UIInput.mDrawStart = num;
						this.SetPivotToLeft();
					}
					else if (num2 < UIInput.mDrawStart)
					{
						UIInput.mDrawStart = num2;
						this.SetPivotToLeft();
					}
					else
					{
						num2 = this.label.CalculateOffsetToFit(text.Substring(0, num));
						if (num2 > UIInput.mDrawStart)
						{
							UIInput.mDrawStart = num2;
							this.SetPivotToRight();
						}
					}
					if (UIInput.mDrawStart != 0)
					{
						text = text.Substring(UIInput.mDrawStart, text.Length - UIInput.mDrawStart);
					}
				}
				else
				{
					UIInput.mDrawStart = 0;
					this.RestoreLabelPivot();
				}
			}
			this.label.text = text;
			if (isSelected)
			{
				Int32 num3 = this.mSelectionStart - UIInput.mDrawStart;
				Int32 num4 = this.mSelectionEnd - UIInput.mDrawStart;
				if (this.mBlankTex == (UnityEngine.Object)null)
				{
					this.mBlankTex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
					for (Int32 j = 0; j < 2; j++)
					{
						for (Int32 k = 0; k < 2; k++)
						{
							this.mBlankTex.SetPixel(k, j, Color.white);
						}
					}
					this.mBlankTex.Apply();
				}
				if (num3 != num4)
				{
					if (this.mHighlight == (UnityEngine.Object)null)
					{
						this.mHighlight = NGUITools.AddWidget<UITexture>(this.label.cachedGameObject);
						this.mHighlight.name = "Input Highlight";
						this.mHighlight.mainTexture = this.mBlankTex;
						this.mHighlight.fillGeometry = false;
						this.mHighlight.pivot = this.label.pivot;
						this.mHighlight.SetAnchor(this.label.cachedTransform);
					}
					else
					{
						this.mHighlight.pivot = this.label.pivot;
						this.mHighlight.mainTexture = this.mBlankTex;
						this.mHighlight.MarkAsChanged();
						this.mHighlight.enabled = true;
					}
				}
				if (this.mCaret == (UnityEngine.Object)null)
				{
					this.mCaret = NGUITools.AddWidget<UITexture>(this.label.cachedGameObject);
					this.mCaret.name = "Input Caret";
					this.mCaret.mainTexture = this.mBlankTex;
					this.mCaret.fillGeometry = false;
					this.mCaret.pivot = this.label.pivot;
					this.mCaret.SetAnchor(this.label.cachedTransform);
				}
				else
				{
					this.mCaret.pivot = this.label.pivot;
					this.mCaret.mainTexture = this.mBlankTex;
					this.mCaret.MarkAsChanged();
					this.mCaret.enabled = true;
				}
				if (num3 != num4)
				{
					this.label.PrintOverlay(num3, num4, this.mCaret.geometry, this.mHighlight.geometry, this.caretColor, this.selectionColor);
					this.mHighlight.enabled = this.mHighlight.geometry.hasVertices;
				}
				else
				{
					this.label.PrintOverlay(num3, num4, this.mCaret.geometry, (UIGeometry)null, this.caretColor, this.selectionColor);
					if (this.mHighlight != (UnityEngine.Object)null)
					{
						this.mHighlight.enabled = false;
					}
				}
				this.mNextBlink = RealTime.time + 0.5f;
				this.mLastAlpha = this.label.finalAlpha;
			}
			else
			{
				this.Cleanup();
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
		if (this.label != (UnityEngine.Object)null && this.label.pivot != this.mPivot)
		{
			this.label.pivot = this.mPivot;
		}
	}

	protected Char Validate(String text, Int32 pos, Char ch)
	{
		if (this.validation == UIInput.Validation.None || !base.enabled)
		{
			return ch;
		}
		if (this.validation == UIInput.Validation.Integer)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Float)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
			if (ch == '.' && !text.Contains("."))
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Username)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return (Char)(ch - 'A' + 'a');
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Filename)
		{
			if (ch == ':')
			{
				return '\0';
			}
			if (ch == '/')
			{
				return '\0';
			}
			if (ch == '\\')
			{
				return '\0';
			}
			if (ch == '<')
			{
				return '\0';
			}
			if (ch == '>')
			{
				return '\0';
			}
			if (ch == '|')
			{
				return '\0';
			}
			if (ch == '^')
			{
				return '\0';
			}
			if (ch == '*')
			{
				return '\0';
			}
			if (ch == ';')
			{
				return '\0';
			}
			if (ch == '"')
			{
				return '\0';
			}
			if (ch == '`')
			{
				return '\0';
			}
			if (ch == '\t')
			{
				return '\0';
			}
			if (ch == '\n')
			{
				return '\0';
			}
			return ch;
		}
		else if (this.validation == UIInput.Validation.Name)
		{
			Char c = (Char)((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
			Char c2 = (Char)((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
			if (ch >= 'a' && ch <= 'z')
			{
				if (c == ' ')
				{
					return (Char)(ch - 'a' + 'A');
				}
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				if (c != ' ' && c != '\'')
				{
					return (Char)(ch - 'A' + 'a');
				}
				return ch;
			}
			else if (ch == '\'')
			{
				if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
				{
					return ch;
				}
			}
			else if (ch == ' ' && c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
			{
				return ch;
			}
		}
		return '\0';
	}

	protected void ExecuteOnChange()
	{
		if (UIInput.current == (UnityEngine.Object)null && EventDelegate.IsValid(this.onChange))
		{
			UIInput.current = this;
			EventDelegate.Execute(this.onChange);
			UIInput.current = (UIInput)null;
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
			String text = this.mValue.Replace("\\n", "\n");
			this.mValue = String.Empty;
			this.value = ((!PlayerPrefs.HasKey(this.savedAs)) ? text : PlayerPrefs.GetString(this.savedAs));
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
	protected Int32 mSelectionStart;

	[NonSerialized]
	protected Int32 mSelectionEnd;

	[NonSerialized]
	protected UITexture mHighlight;

	[NonSerialized]
	protected UITexture mCaret;

	[NonSerialized]
	protected Texture2D mBlankTex;

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
