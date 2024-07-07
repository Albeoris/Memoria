using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using System;
using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class ActiveTimeEvent : MonoBehaviour
{
    public void EnableATE(Boolean isEnable, ATEType ateType)
    {
        if (isEnable)
        {
            if (!base.gameObject.activeSelf)
            {
                base.gameObject.SetActive(true);
                base.StopCoroutine("DisplayBlueATEText");
                base.StopCoroutine("DisplayGrayATEText");
                if (this.mStart)
                {
                    this.SetIcon();
                }
                this.SetCurrentATESprite(ateType);
                this.SetSpriteVisibility(false);
                this.PressSelectSprite.SetActive(false);
                this.isReady = true;
            }
        }
        else if (base.gameObject.activeSelf)
        {
            base.gameObject.SetActive(false);
        }
    }

    private void SetCurrentATESprite(ATEType ateType)
    {
        this.currentType = ateType;
        if (ateType != ATEType.Blue)
        {
            if (ateType != ATEType.Gray)
            {
                this.currentFuntion = String.Empty;
            }
            else
            {
                this.currentFuntion = "DisplayGrayATEText";
            }
        }
        else
        {
            this.currentFuntion = "DisplayBlueATEText";
        }
    }

    public void InitializeATE()
    {
        Int32 num = 20;
        Single num2 = 50f;
        if (this.generalAtlas == (UnityEngine.Object)null)
        {
            this.generalAtlas = this.Text2Sprite.atlas;
        }
        if (this.buttonLabel == (UnityEngine.Object)null)
        {
            this.buttonLabel = base.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<UILabel>();
        }
        this.ATESpriteBlue.GetComponent<UISprite>().transform.localPosition = new Vector2(num2, 0f);
        this.ATESpriteGray.GetComponent<UISprite>().transform.localPosition = new Vector2(num2, 0f);
        String text = this.GetText1SpriteName();
        if (text == String.Empty)
        {
            this.Text1Sprite.atlas = (UIAtlas)null;
        }
        else
        {
            this.Text1Sprite.atlas = this.generalAtlas;
            this.Text1Sprite.spriteName = text;
            this.Text1Sprite.MakePixelPerfect();
            this.Text1Sprite.transform.localPosition = new Vector2(num2, 0f);
            num2 = num2 + (Single)this.Text1Sprite.width + (Single)num;
        }
        this.SetIcon();
        this.ButtonSprite.transform.localPosition = new Vector2(num2, 0f);
        num2 = num2 + (Single)this.ButtonSprite.width + (Single)num;
        text = this.GetText2SpriteName();
        if (text == String.Empty)
        {
            this.Text2Sprite.atlas = (UIAtlas)null;
        }
        else
        {
            this.Text2Sprite.atlas = this.generalAtlas;
            this.Text2Sprite.spriteName = text;
            this.Text2Sprite.MakePixelPerfect();
            this.Text2Sprite.transform.localPosition = new Vector2(num2, 0f);
        }
        this.AteIconSprite.spriteName = Localization.Get("AteHudIcon");
    }

    private void SetIcon()
    {
        if (FF9StateSystem.MobilePlatform)
        {
            this.ButtonSprite.spriteName = Localization.Get("AtePressButton");
            this.ButtonSprite.MakePixelPerfect();
            this.KeyboardText.SetActive(false);
        }
        else if (FF9StateSystem.PCPlatform)
        {
            if (global::GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                this.ButtonSprite.spriteName = "joystick_select";
                if (this.buttonLabel != (UnityEngine.Object)null)
                {
                    this.buttonLabel.text = String.Empty;
                }
            }
            else
            {
                this.ButtonSprite.spriteName = "keyboard_button";
                if (this.buttonLabel != (UnityEngine.Object)null)
                {
                    this.buttonLabel.text = FF9UIDataTool.KeyboardIconLabel[KeyCode.Alpha1];
                }
            }
            this.ButtonSprite.MakePixelPerfect();
            this.KeyboardText.SetActive(true);
        }
        else if (FF9StateSystem.aaaaPlatform)
        {
            this.ButtonSprite.spriteName = "ps_select";
            this.ButtonSprite.MakePixelPerfect();
            this.KeyboardText.SetActive(false);
        }
    }

    private IEnumerator DisplayBlueATEText()
    {
        Single timeDelta = 0f;
        this.isReady = false;
        this.SetSpriteVisibility(true);
        while (timeDelta < 2f)
        {
            if (!PersistenSingleton<UIManager>.Instance.IsPause)
            {
                timeDelta += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        this.SetSpriteVisibility(false);
        timeDelta = 0f;
        while (timeDelta < 2f)
        {
            if (!PersistenSingleton<UIManager>.Instance.IsPause)
            {
                timeDelta += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        this.isReady = true;
        yield break;
    }

    private void SetSpriteVisibility(Boolean isVisible)
    {
        if (this.currentType == ATEType.Blue)
        {
            this.ATESpriteBlue.SetActive(isVisible);
            this.ATESpriteGray.SetActive(false);
        }
        else
        {
            this.ATESpriteBlue.SetActive(false);
            this.ATESpriteGray.SetActive(isVisible);
        }
        this.PressSelectSprite.SetActive(!isVisible);
    }

    private IEnumerator DisplayGrayATEText()
    {
        Single timeDelta = 0f;
        this.isReady = false;
        this.ATESpriteGray.SetActive(true);
        while (timeDelta < 1f)
        {
            if (!PersistenSingleton<UIManager>.Instance.IsPause)
            {
                timeDelta += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        this.ATESpriteGray.SetActive(false);
        timeDelta = 0f;
        while (timeDelta < 1f)
        {
            if (!PersistenSingleton<UIManager>.Instance.IsPause)
            {
                timeDelta += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        this.isReady = true;
        yield break;
    }

    private String GetText1SpriteName()
    {
        String language = Localization.CurrentLanguage;
        String text = language;
        switch (text)
        {
            case "English(US)":
                return "ate_selective_text00_01_us_uk";
            case "Japanese":
                return String.Empty;
            case "German":
                return "ate_selective_text00_01_gr";
            case "Spanish":
                return "ate_selective_text00_01_es";
            case "Italian":
                return "ate_selective_text00_01_it";
            case "French":
                return "ate_selective_text00_01_fr";
            case "English(UK)":
                return "ate_selective_text00_01_us_uk";
        }
        return String.Empty;
    }

    private String GetText2SpriteName()
    {
        String language = Localization.CurrentLanguage;
        String text = language;
        switch (text)
        {
            case "English(US)":
                return "ate_selective_text00_02_us_uk";
            case "Japanese":
                return "ate_selective_text00_02_jp";
            case "German":
                return "ate_selective_text00_02_gr";
            case "Spanish":
                return "ate_selective_text00_02_es";
            case "Italian":
                return String.Empty;
            case "French":
                return String.Empty;
            case "English(UK)":
                return "ate_selective_text00_02_us_uk";
        }
        return String.Empty;
    }

    private void Start()
    {
        this.InitializeATE();
        this.mStart = true;
    }

    private void Update()
    {
        if (this.isReady && !PersistenSingleton<UIManager>.Instance.IsPause)
        {
            base.StartCoroutine(this.currentFuntion);
        }
    }

    private void OnDisable()
    {
        if (base.gameObject.activeSelf)
        {
            base.gameObject.SetActive(false);
        }
    }

    public GameObject ATESpriteGray;

    public GameObject ATESpriteBlue;

    public GameObject PressSelectSprite;

    public GameObject KeyboardText;

    public UISprite Text1Sprite;

    public UISprite ButtonSprite;

    public UISprite Text2Sprite;

    public UISprite AteIconSprite;

    private Boolean isReady = true;

    private Boolean mStart;

    private ATEType currentType;

    private String currentFuntion = "DisplayBlueATEText";

    private UIAtlas generalAtlas;

    private UILabel buttonLabel;
}
