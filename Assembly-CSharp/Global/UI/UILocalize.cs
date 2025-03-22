using Memoria.Assets;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
    public delegate void OverwriteTextDelegate(String key, ref String text);
    public event OverwriteTextDelegate TextOverwriting;

    private String OverwriteText(String text)
    {
        if (this.key == "Collector" && Localization.CurrentLanguage == "German")
            text = text.Substring(0, text.Length - 1);
        if (TextOverwriting != null)
            TextOverwriting(this.key, ref text);
        return text;
    }

    public String value
    {
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                UIWidget widget = base.GetComponent<UIWidget>();
                UILabel uilabel = widget as UILabel;
                UISprite uisprite = widget as UISprite;
                if (uilabel != null)
                {
                    UIInput uiinput = NGUITools.FindInParents<UIInput>(uilabel.gameObject);
                    if (uiinput != null && uiinput.label == uilabel)
                        uiinput.defaultText = value;
                    else
                        uilabel.rawText = this.OverwriteText(value);
                }
                else if (uisprite != null)
                {
                    UIButton uibutton = NGUITools.FindInParents<UIButton>(uisprite.gameObject);
                    if (uibutton != null && uibutton.tweenTarget == uisprite.gameObject)
                        uibutton.normalSprite = value;
                    uisprite.spriteName = value;
                    uisprite.MakePixelPerfect();
                }
            }
        }
    }

    private void OnEnable()
    {
        if (this.mStarted)
            this.OnLocalize();
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
            UILabel label = base.GetComponent<UILabel>();
            if (label != null)
                this.key = label.rawText;
        }
        if (!String.IsNullOrEmpty(this.key))
            this.value = Localization.GetWithDefault(this.key);
    }

    public String key;

    private Boolean mStarted;
}
