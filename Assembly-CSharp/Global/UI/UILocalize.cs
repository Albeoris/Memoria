using Memoria.Assets;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
	public delegate String OverwriteTextDelegate(String key, String text);

	public event OverwriteTextDelegate TextOverwriting;

	private void EnablePrintIcon(UILabel lbl)
	{
		lbl.PrintIconAfterProcessedText = true;
	}

	private string OverwriteText(string rawText)
	{
		string result = rawText;
		if (this.key == "Collector" && Localization.CurrentLanguage == "German")
		{
			result = rawText.Substring(0, rawText.Length - 1);
		}

		OverwriteTextDelegate h = TextOverwriting;
		if (h != null)
			result = h(this.key, result);

		return result;
	}

	public String value
	{
		set
		{
			if (!String.IsNullOrEmpty(value))
			{
				UIWidget component = base.GetComponent<UIWidget>();
				UILabel uilabel = component as UILabel;
				UISprite uisprite = component as UISprite;
				if (uilabel != null)
				{
					UIInput uiinput = NGUITools.FindInParents<UIInput>(uilabel.gameObject);
					if (uiinput != null && uiinput.label == uilabel)
					{
						uiinput.defaultText = value;
					}
					else
					{
						Single num = 0f;
						this.EnablePrintIcon(uilabel);
						string text = uilabel.PhrasePreOpcodeSymbol(value, ref num);
						text = this.OverwriteText(text);
						uilabel.text = text;
					}
				}
				else if (uisprite != null)
				{
					UIButton uibutton = NGUITools.FindInParents<UIButton>(uisprite.gameObject);
					if (uibutton != null && uibutton.tweenTarget == uisprite.gameObject)
					{
						uibutton.normalSprite = value;
					}
					uisprite.spriteName = value;
					uisprite.MakePixelPerfect();
				}
			}
		}
	}

	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnLocalize();
		}
	}

	private void Start()
	{
		this.mStarted = true;
		this.OnLocalize();
	}

	public void OnLocalize()
	{
		if (String.IsNullOrEmpty(this.key))
		{
			UILabel component = base.GetComponent<UILabel>();
			if (component != null)
			{
				this.key = component.text;
			}
		}
		if (!String.IsNullOrEmpty(this.key))
		{
			this.value = Localization.Get(this.key);
		}
	}

	public String key;

	private Boolean mStarted;
}
