using System;
using UnityEngine;

public class AndroidExpansionUI : MonoBehaviour
{
    public void SetSceneActive(Boolean isShow)
    {
        Single num = (!isShow) ? 0f : 1f;
        if (this.panel.alpha != num)
        {
            this.panel.alpha = num;
            if (num == 1f)
            {
                this.StartFadingCharacter();
            }
        }
    }

    public void SetProgress(Int32 progress, Boolean isShow)
    {
        Single num = (!isShow) ? 0f : 1f;
        if (this.PercentageSymbol.alpha != num || this.currentProgress != progress)
        {
            if (isShow)
            {
                this.currentProgress = progress;
                this.Percentage100Sprite.alpha = 1f;
                this.Percentage10Sprite.alpha = 1f;
                this.Percentage1Sprite.alpha = 1f;
                this.PercentageSymbol.alpha = 1f;
                this.ProgressBar.value = (Single)progress / 100f;
                this.Percentage100Sprite.alpha = (Single)((progress != 100) ? 0 : 1);
                this.Percentage10Sprite.alpha = (Single)((progress < 10) ? 0 : 1);
                if (progress < 100)
                {
                    this.Percentage10Sprite.spriteName = "text_" + progress / 10;
                }
                else
                {
                    this.Percentage10Sprite.spriteName = "text_" + 0;
                }
                this.Percentage1Sprite.spriteName = "text_" + progress % 10;
            }
            else
            {
                this.Percentage100Sprite.alpha = 0f;
                this.Percentage10Sprite.alpha = 0f;
                this.Percentage1Sprite.alpha = 0f;
                this.PercentageSymbol.alpha = 0f;
            }
        }
    }

    public void SetTimeRemaining(Int32 second, Boolean isShow)
    {
        Single num = (!isShow) ? 0f : 1f;
        if (this.TimeRemainTextSprite.alpha != num || this.currentTimeRemaining != second)
        {
            if (isShow)
            {
                this.currentTimeRemaining = second;
                Int32 num2 = this.currentTimeRemaining / 60;
                Int32 num3 = this.currentTimeRemaining % 60;
                this.TimeRemainTextSprite.alpha = 1f;
                this.TimeRemainSec1Sprite.alpha = 1f;
                this.TimeRemainSec10Sprite.alpha = 1f;
                this.TimeRemainMin1Sprite.alpha = 1f;
                this.TimeRemainMin10Sprite.alpha = 1f;
                this.TimeRemainTextSprite.spriteName = "text_time_" + this.GetSystemLanguageSymbol();
                this.TimeRemainMin10Sprite.alpha = (Single)((num2 < 10) ? 0 : 1);
                this.TimeRemainMin1Sprite.spriteName = "text_" + num2 % 10;
                this.TimeRemainMin10Sprite.spriteName = "text_" + num2 / 10;
                this.TimeRemainSec1Sprite.spriteName = "text_" + num3 % 10;
                this.TimeRemainSec10Sprite.spriteName = "text_" + num3 / 10;
            }
            else
            {
                this.TimeRemainTextSprite.alpha = 0f;
                this.TimeRemainSec1Sprite.alpha = 0f;
                this.TimeRemainSec10Sprite.alpha = 0f;
                this.TimeRemainMin1Sprite.alpha = 0f;
                this.TimeRemainMin10Sprite.alpha = 0f;
            }
        }
    }

    public void SetStatusText(ExpansionVerifier.State state, ExpansionVerifier.ErrorCode errorCode)
    {
        if (this.currentState == state && this.currentErrorCode == errorCode)
        {
            return;
        }
        this.currentErrorCode = errorCode;
        this.currentState = state;
        switch (state)
        {
            case ExpansionVerifier.State.ValidateDownloadOBB:
            case ExpansionVerifier.State.ValidateDecompressOBB:
            case ExpansionVerifier.State.DetermineAvailableSpace:
            case ExpansionVerifier.State.DecompressOBB:
            case ExpansionVerifier.State.ValidateDecompressedOBBSize:
            case ExpansionVerifier.State.ReplaceCompressedOBBWithUncompressedOBB:
                this.StatusTextSprite.spriteName = "text_decompress_" + this.GetSystemLanguageSymbol();
                break;
            case ExpansionVerifier.State.DecompressSuccess:
                this.StatusTextSprite.spriteName = "text_success_" + this.GetSystemLanguageSymbol();
                break;
            case ExpansionVerifier.State.DecompressFailure:
                switch (errorCode)
                {
                    case ExpansionVerifier.ErrorCode.NotEnoughStorage:
                        this.StatusTextSprite.spriteName = "text_nospace_" + this.GetSystemLanguageSymbol();
                        goto IL_135;
                    case ExpansionVerifier.ErrorCode.FileTypeNotSupport:
                        global::Debug.LogError("File type not support, need to upload again");
                        goto IL_135;
                    case ExpansionVerifier.ErrorCode.DecompressionFailure:
                    case ExpansionVerifier.ErrorCode.MoveTempFileFailure:
                    case ExpansionVerifier.ErrorCode.ValidateFileSizeFailure:
                        this.StatusTextSprite.spriteName = "text_corrupt_" + this.GetSystemLanguageSymbol();
                        goto IL_135;
                    case ExpansionVerifier.ErrorCode.PatchOBBNotFound:
                        global::Debug.LogError("Patch not found, should not occurred");
                        goto IL_135;
                }
                global::Debug.LogError("Error code default, undefined error");
            IL_135:
                break;
        }
    }

    private void StartFadingCharacter()
    {
        this.fading.FadePingPong(new UIScene.SceneVoidDelegate(this.ChangeCharSprite), new UIScene.SceneVoidDelegate(this.StartFadingCharacter));
    }

    public void ChangeCharSprite()
    {
        this.currentCharSprite++;
        if (this.currentCharSprite > 7)
        {
            this.currentCharSprite = 0;
        }
        this.CharacterSprite.spriteName = "data_scene_0" + this.currentCharSprite;
    }

    private String GetSystemLanguageSymbol()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        switch (systemLanguage)
        {
            case SystemLanguage.English:
                return "us";
            case SystemLanguage.Estonian:
            case SystemLanguage.Faroese:
            case SystemLanguage.Finnish:
            IL_27:
                if (systemLanguage == SystemLanguage.Italian)
                {
                    return "it";
                }
                if (systemLanguage == SystemLanguage.Japanese)
                {
                    return "jp";
                }
                if (systemLanguage != SystemLanguage.Spanish)
                {
                    return "us";
                }
                return "es";
            case SystemLanguage.French:
                return "fr";
            case SystemLanguage.German:
                return "gr";
            default:
                goto IL_27;
        }
    }

    private void Awake()
    {
        this.panel = base.gameObject.GetComponent<UIPanel>();
        this.fading = this.CharacterSprite.GetComponent<HonoFading>();
    }

    public UISprite CharacterSprite;

    public UISprite StatusTextSprite;

    public UISlider ProgressBar;

    public UISprite PercentageSymbol;

    public UISprite Percentage100Sprite;

    public UISprite Percentage10Sprite;

    public UISprite Percentage1Sprite;

    public UISprite TimeRemainTextSprite;

    public UISprite TimeRemainSec1Sprite;

    public UISprite TimeRemainSec10Sprite;

    public UISprite TimeRemainMin1Sprite;

    public UISprite TimeRemainMin10Sprite;

    private UIPanel panel;

    private Int32 currentCharSprite;

    private ExpansionVerifier.State currentState;

    private ExpansionVerifier.ErrorCode currentErrorCode;

    private Int32 currentProgress;

    private Int32 currentTimeRemaining;

    private HonoFading fading;
}
