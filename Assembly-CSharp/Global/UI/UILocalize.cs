using System;
using Memoria.Assets;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
	private void EnablePrintIcon(UILabel lbl)
	{
		lbl.PrintIconAfterProcessedText = true;
	}

    private string OverwriteText(string rawText)
    {
        string result = rawText;
        if (this.key == "Collector" && Localization.language == "German")
        {
            result = rawText.Substring(0, rawText.Length - 1);
        }
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
				if (uilabel != (UnityEngine.Object)null)
				{
					UIInput uiinput = NGUITools.FindInParents<UIInput>(uilabel.gameObject);
					if (uiinput != (UnityEngine.Object)null && uiinput.label == uilabel)
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
				else if (uisprite != (UnityEngine.Object)null)
				{
					UIButton uibutton = NGUITools.FindInParents<UIButton>(uisprite.gameObject);
					if (uibutton != (UnityEngine.Object)null && uibutton.tweenTarget == uisprite.gameObject)
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

	private void OnLocalize()
	{
		if (String.IsNullOrEmpty(this.key))
		{
			UILabel component = base.GetComponent<UILabel>();
			if (component != (UnityEngine.Object)null)
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
