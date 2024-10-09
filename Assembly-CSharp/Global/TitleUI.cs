using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Scripts;
using Memoria.Speedrun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static AssetManager;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable ArrangeThisQualifier
// ReSharper disable MemberCanBeProtected.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedField.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

#pragma warning disable 414
#pragma warning disable 169

public class TitleUI : UIScene
{
    private static String MenuGroupButton = "Title.Menu";

    private static String LanguageGroupButton = "Title.MenuLanguage";

    private static String StaffRollGroupButton = "Title.MenuStaffRoll";

    private static String LicenseGroupButton = "Title.MenuLicense";

    private static String MovieGalleryGroupButton = "MovieGallery.Thumbnail";

    public GameObject MenuPanelObject;

    public GameObject MenuGroupPanel;

    public GameObject MenuLanguageButton;

    public GameObject MenuStaffButton;

    public GameObject MenuStaffPCButton;

    public GameObject MenuMovieButton;

    public GameObject MenuBlackjackButton;

    public GameObject MenuBlackjackPCButton;

    public GameObject FaqsButton;

    public GameObject SquareEnixButton;

    public GameObject AchievementButton;

    public GameObject ScreenRotateButton;

    public GameObject SlideShowObject;

    public GameObject SlideShowHitArea;

    public GameObject TitleImageTextObject0;

    public GameObject TitleImageTextObject1;

    public GameObject TitleImageTextJpObject0;

    public GameObject TitleImageTextJpObject1;

    public GameObject SelectLanguagePanel;

    public GameObject LanguageGroupPanel;

    public GameObject BackButton;

    public GameObject StaffPanel;

    public GameObject StaffBackButton;

    public GameObject StaffLicenseButton;

    public UIProgressBar StaffScrollBar;

    public UILabel StaffLabel;

    public UIWidget StaffScrollContainer;

    public UIWidget StaffFFIXLogo;

    public UIWidget StaffForeground;

    public GameObject LicensePanel;

    public GameObject LicenseBackButton;

    public UILabel LicensePageIndicator;

    public UICenterOnChild LicenseCenterOnChild;

    public GameObject MoviePanel;

    public GameObject MovieBackButton;

    public UICenterOnChild MovieCenterOnChild;

    public UIGrid MoviePagerGrid;

    public UIGrid MoviePageGrid;

    public UIScrollView MovieScrollView;

    public UIWidget MovieForeGround;

    public GameObject MovieHitArea;

    public GameObject ScreenFadeGameObject;

    public GameObject LoadingIcon;

    private GameObject continueButton;

    private GameObject newGameButton;

    private GameObject loadGameButton;

    private GameObject cloudButton;

    private GameObject startGameObject;

    private GameObject englishUSButton;

    private GameObject japaneseButton;

    private GameObject germanButton;

    private GameObject spanishButton;

    private GameObject italianButton;

    private GameObject frenchButton;

    private GameObject englishUKButton;

    private UISprite rotateButtonSprite;

    private UISprite continueButtonSprite;

    private UISprite cloudButtonSprite;

    private Boolean canSync;

    private Boolean canContinue;

    private SlideShow idleScreen;

    public Single idleTime = 20f;

    private Timer timer;

    private SlideShow.Type idleScreenType = SlideShow.Type.Sequence1;

    public GameObject SlideShowFadingObject;

    public Boolean SplashScreenEnabled = true;

    public Boolean ForceCheckingAutoSave;

    public UILabel SpashText;

    public UISprite LogoSprite;

    private SlashScreen slashScreen;

    private Boolean IsJustLaunchApp = true;

    private Single staffScrollingTime = 180f;

    private Boolean isStaffScene;

    private Boolean isLicenseScene;

    private Boolean isPlayingMovie;

    private Int32 currentMoviePageIndex;

    private ButtonGroupState[] movieThumbnails;

    private List<GameObject> movieThumbnailList;

    private static readonly String[] MovieFiles =
    {
        "FMV001",
        "FMV002",
        "FMV003",
        "FMV004",
        "FMV005",
        "FMV006A",
        "FMV006B",
        "FMV008",
        "FMV011",
        "FMV012",
        "FMV013",
        "FMV015",
        "FMV016",
        "FMV017",
        "FMV019",
        "FMV021",
        "FMV023",
        "FMV024",
        "FMV027",
        "FMV029",
        "FMV031",
        "FMV032",
        "FMV033",
        "FMV034",
        "FMV035",
        "FMV036",
        "FMV038",
        "FMV039",
        "FMV040",
        "FMV042",
        "FMV045",
        "FMV046",
        "FMV052",
        "FMV053",
        "FMV055A",
        "FMV055C",
        "FMV055D",
        "FMV056",
        "FMV059",
        "FMV060"
    };

    private Action onResumeFromQuit;

    private GameObject selectedMovieThumbnail;

    private Boolean fromAchievementPage;

    private Boolean authenticatedBeforeClickingAchievementPage;

    private Int32 licensePageNum;

    private Boolean playSoundFMV000 = true;

    private Single doubleClickInterval = 0.35f;

    private Single clickedTimer = -1f;

    private Boolean isPaused;

    public string GClunguage;

    public bool IsSplashTextActive
    {
        get
        {
            return this.SpashText != (UnityEngine.Object)null && this.SpashText.isActiveAndEnabled;
        }
    }

    public class UiElement
    {
        protected static readonly Color DefaultColor = new Color(1f, 1f, 1f, 0.392f);

        public readonly GameObject GameObject;
        public readonly RectTransform RectTransform;

        public UiElement()
        {
            GameObject = new GameObject();
            RectTransform = GameObject.EnsureExactComponent<RectTransform>();
        }

        public String Name
        {
            get => GameObject.name;
            set => GameObject.name = value;
        }
    }

    public class UiPanel : UiElement
    {
        public readonly Image Image;

        public UiPanel()
        {
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.sizeDelta = Vector2.zero;

            Image = GameObject.EnsureExactComponent<Image>();
            Image.type = Image.Type.Sliced;
            Image.color = DefaultColor;
            Image.fillCenter = true;
        }

        public Sprite Sprite
        {
            get => Image.sprite;
            set => Image.sprite = value;
        }

        public Color Color
        {
            get => Image.color;
            set => Image.color = value;
        }
    }

    //class Aaaaaaaaa : MonoBehaviour
    //{
    //    private void Start()
    //    {
    //        var friendlyName = "PlayerAppDomain";
    //        var assembly = Assembly.GetExecutingAssembly();
    //        var codeBase = assembly.Location;
    //        var codeBaseDirectory = Path.GetDirectoryName(codeBase);
    //        var info = new AppDomainSetup()
    //        {
    //            ApplicationName = "123",
    //            ApplicationBase = codeBaseDirectory,
    //            DynamicBase = codeBaseDirectory,
    //        };
    //        var domain = AppDomain.CreateDomain(friendlyName, new System.Security.Policy.Evidence(), info);
    //        AppDomain.Unload(domain);
    //    }
    //}

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        //GameObject root = new GameObject();
        //Canvas uiRoot = root.EnsureExactComponent<Canvas>();
        //
        //
        //UiPanel panel = new UiPanel();
        ////panel.RectTransform.offsetMin = new Vector2(500, 300);
        ////panel.RectTransform.offsetMax = new Vector2(-420, 50);
        //
        //
        //panel.RectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        //panel.RectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        //panel.RectTransform.sizeDelta = new Vector2(920, 350);
        //panel.GameObject.transform.SetParent(root.transform);


        // ====================================
        // TEMP
        try
        {
            UI2DSprite sprite2D = this.MenuPanelObject.GetChild(0).GetComponent<UI2DSprite>();
            Sprite sprite = sprite2D.sprite2D;
            String externalPath = "StreamingAssets/UI/Sprites/US/" + sprite.name + ".png";
            if (File.Exists(externalPath))
            {
                Texture2D texture = StreamingResources.LoadTexture2D(externalPath);
                sprite2D.sprite2D = Sprite.Create(texture, sprite.rect, sprite.pivot);
                sprite2D.sprite2D.name = sprite.name;
            }

            // Replace title screen with modded version if it exists
            externalPath = AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/title_bg.png", true, false);
            externalPath = !String.IsNullOrEmpty(externalPath) ? externalPath : AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/title_bg", true, false);
            if (!String.IsNullOrEmpty(externalPath))
            {
                Texture2D texture = StreamingResources.LoadTexture2D(externalPath);
                sprite2D.sprite2D = Sprite.Create(texture, sprite.rect, sprite.pivot);
                sprite2D.sprite2D.name = sprite.name;
            }

            // Replace title logo with modded version if it exists (advice: replace with transparent and include logo in title_bg.png)
            UITexture texture2D = this.MenuPanelObject.GetChild(1).GetComponent<UITexture>();
            externalPath = AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Materials/title_logo.png", true, false);
            externalPath = !String.IsNullOrEmpty(externalPath) ? externalPath : AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Materials/title_logo", true, false);
            externalPath = !String.IsNullOrEmpty(externalPath) ? externalPath : AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/title_logo.png", true, false);
            externalPath = !String.IsNullOrEmpty(externalPath) ? externalPath : AssetManager.SearchAssetOnDisc("EmbeddedAsset/UI/Sprites/title_logo", true, false);
            if (!String.IsNullOrEmpty(externalPath))
            {
                Texture2D texture = StreamingResources.LoadTexture2D(externalPath);
                texture2D.mainTexture = texture;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        ////===================

        ExpansionVerifier.printLog("TitleUI: Show");
        SceneVoidDelegate sceneVoidDelegate = delegate
        {
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(-60f, 0f), LanguageGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(-60f, 0f), MenuGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-400f, -1000f, 400f, 1000f), MenuGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-100f, -100f, 100f, 100f), StaffRollGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-400f, -1000f, 400f, 1000f), LanguageGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-100f, -100f, 100f, 100f), LicenseGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-700f, -200f, 700f, 200f), MovieGalleryGroupButton);
            ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, MenuGroupButton);
            ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, StaffRollGroupButton);
            ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, LanguageGroupButton);
            ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, LicenseGroupButton);
            ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, MovieGalleryGroupButton);
            if (!this.SplashScreenEnabled && !this.ForceCheckingAutoSave)
            {
                ButtonGroupState.ActiveGroup = MenuGroupButton;
            }
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        this.SetupIdleScreen();
        this.timer.Start();
        base.Show(sceneVoidDelegate);
        if (this.SplashScreenEnabled)
        {
            SoundLib.StopAllSounds(true);
            SoundLib.ClearSuspendedSounds();
            FF9StateSystem.ReInitStateSystem();
            this.CheckAutoSaveSlot(DataSerializerErrorCode.Success, false);
            FF9StateSystem.Serializer.HasAutoload(this.CheckAutoSaveSlot);
            FF9StateSystem.Serializer.GetGameFinishFlag(this.CheckGameFinishFlag);
            ExpansionVerifier.printLog("TitleUI: PlaySplashScreen()");
            {
                this.PlaySplashScreen();
            }
        }
        else if (!this.ForceCheckingAutoSave && !this.SplashScreenEnabled && PersistenSingleton<UIManager>.Instance.PreviousState != UIManager.UIState.EndGame)
        {
            ButtonGroupState.SetCursorMemorize(this.loadGameButton, MenuGroupButton);
        }
        else if (this.ForceCheckingAutoSave)
        {
            this.ForceCheckingAutoSave = false;
            ButtonGroupState.SetCursorMemorize(this.cloudButton, MenuGroupButton);
            FF9StateSystem.Serializer.HasAutoload(this.CheckAutoSaveSlot);
            FF9StateSystem.Serializer.GetGameFinishFlag(this.CheckGameFinishFlag);
            FF9StateSystem.Settings.ReadSystemData(this.SetRotateScreen);
        }
        else
        {
            this.ShowMenuPanel();
        }
        this.SetRotateScreen();
        this.fromAchievementPage = false;
        this.authenticatedBeforeClickingAchievementPage = false;
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate sceneVoidDelegate = delegate
        {
            FF9StateSystem.Common.FF9.attr &= 4294967293u;
            SceneDirector.FF9Wipe_FadeInEx(24);
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Hide(sceneVoidDelegate);
        this.timer.Stop();
        ButtonGroupState.RemoveCursorMemorize(MenuGroupButton);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            if (ButtonGroupState.ActiveGroup == MenuGroupButton)
            {
                if (go == this.continueButton)
                {
                    this.OnContinueButtonClick();
                }
                else if (go == this.newGameButton)
                {
                    this.OnNewGameButtonClick();
                }
                else if (go == this.loadGameButton)
                {
                    this.OnLoadGameButtonClick();
                }
                else if (go == this.cloudButton)
                {
                    this.OnCloudGameButtonClick();
                }
                else if (go == this.MenuLanguageButton)
                {
                    if (Configuration.VoiceActing.ForceLanguage >= 0)
                        FF9Sfx.FF9SFX_Play(102); // Language has been forced
                    else
                        this.OnMenuLanguageButtonClick();
                }
                else if (go == this.MenuStaffButton || go == this.MenuStaffPCButton)
                {
                    this.OnMenuStaffButtonClick();
                }
                else if (go == this.MenuMovieButton)
                {
                    this.OnMenuMovieButtonClick();
                }
                else if (go == this.MenuBlackjackButton || go == this.MenuBlackjackPCButton)
                {
                    this.OnMenuBlackjackButtonClick();
                }
                else if (go == this.AchievementButton)
                {
                    this.OnAchievementButtonClick();
                }
                else if (go == this.ScreenRotateButton)
                {
                    this.SwapScreenRotation();
                    FF9StateSystem.Settings.SetScreenRotation(FF9StateSystem.Settings.ScreenRotation, () => { });
                }
                else if (go == this.FaqsButton)
                {
                    this.OnFaqsButtonClick();
                }
                else if (go == this.SquareEnixButton)
                {
                    this.OnSquareEnixButtonClick();
                }
            }
            else if (ButtonGroupState.ActiveGroup == LanguageGroupButton)
            {
                if (go == this.BackButton)
                {
                    this.OnKeyCancel(go);
                    return true;
                }
                MenuLanguage menuLanguageFromGameObject = this.GetMenuLanguageFromGameObject(go);
                if (menuLanguageFromGameObject != MenuLanguage.None)
                {
                    String language;
                    switch (menuLanguageFromGameObject)
                    {
                        case MenuLanguage.EnglishUS:
                            language = "English(US)";
                            break;
                        case MenuLanguage.Japanese:
                            language = "Japanese";
                            break;
                        case MenuLanguage.German:
                            language = "German";
                            break;
                        case MenuLanguage.Spanish:
                            language = "Spanish";
                            break;
                        case MenuLanguage.Italian:
                            language = "Italian";
                            break;
                        case MenuLanguage.French:
                            language = "French";
                            break;
                        case MenuLanguage.EnglishUK:
                            language = "English(UK)";
                            break;
                        default:
                            language = String.Empty;
                            break;
                    }
                    EventInput.ChangeInputLayout(language);
                    this.SetLanguage(language);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MovieGalleryGroupButton)
            {
                if (!this.isPlayingMovie && !Loading)
                {
                    if (go == this.MovieBackButton)
                    {
                        this.OnKeyCancel(go);
                        return true;
                    }
                    if (this.movieThumbnailList.Contains(go))
                    {
                        Int32 siblingIndex = go.transform.parent.parent.GetSiblingIndex();
                        Int32 siblingIndex2 = go.transform.GetSiblingIndex();
                        Int32 num = siblingIndex * 6 + siblingIndex2;
                        String movieName = MovieFiles[num];
                        Loading = true;
                        this.isPlayingMovie = true;
                        FF9Sfx.FF9SFX_Play(103);
                        this.MovieForeGround.GetComponent<HonoFading>().FadeIn(delegate
                        {
                            this.selectedMovieThumbnail = go;
                            this.Loading = false;
                            this.PlayGalleryMovie(movieName);
                        });
                    }
                }
                else if (!this.OnDoubleClickGalleryMovie())
                {
                    StartCoroutine(this.StopGalleryMovieCoroutine());
                }
            }
            else if (ButtonGroupState.ActiveGroup == StaffRollGroupButton)
            {
                if (go == this.StaffBackButton)
                {
                    this.OnKeyCancel(go);
                }
                else
                {
                    this.OnKeySpecial(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == LicenseGroupButton)
            {
                this.OnKeyCancel(go);
                return true;
            }
        }
        if (this.idleScreen.isActive)
        {
            this.idleScreen.Stop();
        }
        return true;
    }

    [DebuggerHidden]
    public IEnumerator StopGalleryMovieCoroutine()
    {
        yield return new WaitForSeconds(this.doubleClickInterval);

        if (this.clickedTimer >= 0.0)
            this.StopGalleryMovie();

        this.clickedTimer = -1f;
    }

    private Boolean OnDoubleClickGalleryMovie()
    {
        if (this.clickedTimer < 0f)
        {
            this.clickedTimer = Time.realtimeSinceStartup;
        }
        else
        {
            Single num = Time.realtimeSinceStartup - this.clickedTimer;
            this.clickedTimer = -1f;
            if (num <= this.doubleClickInterval && this.isPlayingMovie && !base.Loading)
            {
                this.PauseAndResumeGalleryMovie();
                return true;
            }
        }
        return false;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
            if (ButtonGroupState.ActiveGroup == MenuGroupButton)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    UIManager.Input.OnQuitCommandDetected();
                }
            }
            else if (ButtonGroupState.ActiveGroup == LanguageGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.CloseSelectLanguagePanel();
            }
            else if (ButtonGroupState.ActiveGroup == MovieGalleryGroupButton)
            {
                if (this.isPlayingMovie)
                {
                    this.StopGalleryMovie();
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(101);
                    this.CloseMoviePanel();
                }
            }
            else if (this.isStaffScene)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.CloseStaffPanel();
            }
            else if (this.isLicenseScene)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.CloseLicensePanel();
                this.OpenStaffPanel();
            }
        }
        return true;
    }

    public override Boolean OnKeySpecial(GameObject go)
    {
        if (base.OnKeySpecial(go) && this.isStaffScene)
        {
            FF9Sfx.FF9SFX_Play(103);
            this.isStaffScene = false;
            this.StaffPanel.SetActive(false);
            StopCoroutine("ScrollingStaffCredits");
            this.OpenLicensePanel();
        }
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == MenuGroupButton)
            {
                ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, MenuGroupButton);
            }
            else if (ButtonGroupState.ActiveGroup == TitleUI.MovieGalleryGroupButton && !go.Equals(this.MovieBackButton))
            {
                Transform parent = go.transform.parent.parent;
                if (this.currentMoviePageIndex != parent.GetSiblingIndex())
                {
                    this.currentMoviePageIndex = parent.GetSiblingIndex();
                    this.MovieCenterOnChild.CenterOn(parent);
                }
            }
            this.timer.Reset();
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == MovieGalleryGroupButton && this.currentMoviePageIndex < this.MoviePageGrid.GetChildList().Count - 1 && !this.isPlayingMovie)
        {
            this.currentMoviePageIndex++;
            this.CenterOnCurrentMoviePageIndex();
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == MovieGalleryGroupButton && this.currentMoviePageIndex > 0 && !this.isPlayingMovie)
        {
            this.currentMoviePageIndex--;
            this.CenterOnCurrentMoviePageIndex();
        }
        return true;
    }

    public override Boolean OnKeySelect(GameObject go)
    {
        return false;
    }

    public override void OnKeyQuit()
    {
        if (this.isPlayingMovie)
        {
            this.MenuPanelObject.SetActive(false);
            this.MoviePanel.SetActive(false);
            MBG.Instance.SetDepthForTitle();
            ShowQuitUI(this.onResumeFromQuit);
            return;
        }
        base.OnKeyQuit();
    }

    public void OnKeyNavigate(GameObject go, KeyCode key)
    {
        if (this.isLicenseScene)
        {
            Int32 siblingIndex = this.LicenseCenterOnChild.centeredObject.transform.GetSiblingIndex();
            GameObject parent = this.LicenseCenterOnChild.centeredObject.GetParent();
            if (key != KeyCode.RightArrow)
            {
                if (key == KeyCode.LeftArrow)
                {
                    if (siblingIndex > 0)
                    {
                        this.LicenseCenterOnChild.CenterOn(parent.GetChild(siblingIndex - 1).transform);
                    }
                }
            }
            else if (siblingIndex < this.licensePageNum - 1)
            {
                this.LicenseCenterOnChild.CenterOn(parent.GetChild(siblingIndex + 1).transform);
            }
        }
    }

    private void OpenStaffPanel()
    {
        this.isStaffScene = true;
        this.StaffPanel.SetActive(true);
        ButtonGroupState.ActiveGroup = StaffRollGroupButton;
        this.StaffScrollContainer.alpha = 1f;
        this.StaffFFIXLogo.alpha = 0f;
        this.StaffForeground.alpha = 0f;
        this.StartScrollStaffCredits();
    }

    public void CloseStaffPanel()
    {
        this.isStaffScene = false;
        this.StaffPanel.SetActive(false);
        ButtonGroupState.ActiveGroup = MenuGroupButton;
        StopCoroutine("ScrollingStaffCredits");
        this.timer.Start();
    }

    private void OnMenuStaffButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        this.OpenStaffPanel();
        this.timer.Stop();
    }

    private void StartScrollStaffCredits()
    {
        this.StaffScrollBar.value = 0f;
        StartCoroutine("ScrollingStaffCredits");
    }

    [DebuggerHidden]
    private IEnumerator ScrollingStaffCredits()
    {
        while (this.StaffScrollBar.value < 1.0)
        {
            yield return null;
            this.StaffScrollBar.value += Time.deltaTime * (1f / this.staffScrollingTime);
        }

        TweenAlpha.Begin(this.StaffScrollContainer.gameObject, 0.5f, 0.0f);
        yield return new WaitForSeconds(0.5f);

        TweenAlpha.Begin(this.StaffForeground.gameObject, 0.5f, 1f);
        TweenAlpha.Begin(this.StaffFFIXLogo.gameObject, 0.5f, 1f);
        yield return new WaitForSeconds(2f);

        TweenAlpha.Begin(this.StaffFFIXLogo.gameObject, 0.5f, 0.0f);
        yield return new WaitForSeconds(1f);

        this.CloseStaffPanel();
    }

    private void CloseMoviePanel()
    {
        Int32 movieSoundIndex = SoundLib.GetMovieSoundIndex("FMV000");
        SoundProfile activeMovieAudioSoundProfile = SoundLib.GetActiveMovieAudioSoundProfile();
        Int32 num = -1;
        if (activeMovieAudioSoundProfile != null)
        {
            num = SoundLib.GetMovieSoundIndex(activeMovieAudioSoundProfile.Name);
        }
        if (this.playSoundFMV000 && movieSoundIndex != num)
        {
            SoundLib.PlayMovieMusic("FMV000", 0);
        }
        this.MoviePanel.SetActive(false);
        MBG.Instance.gameObject.SetActive(false);
        ButtonGroupState.ActiveGroup = MenuGroupButton;
        this.timer.Start();
    }

    private void CenterOnCurrentMoviePageIndex()
    {
        Transform child = this.MoviePageGrid.GetChild(this.currentMoviePageIndex);
        this.MovieCenterOnChild.CenterOn(child);
        ButtonGroupState componentInChildren = child.GetComponentInChildren<ButtonGroupState>();
        ButtonGroupState.ActiveButton = componentInChildren.gameObject;
    }

    private void OnCenterMoviePage(GameObject centeredObject)
    {
        Transform child = this.MoviePagerGrid.GetChild(centeredObject.transform.GetSiblingIndex());
        UIToggle component = child.GetComponent<UIToggle>();
        component.value = true;
    }

    public void PlayGalleryMovie(String movieName)
    {
        this.isPaused = false;
        this.MovieHitArea.SetActive(true);
        ButtonGroupState.SetActiveGroupEnable(false);
        MBG.Instance.SetFinishCallback(this.StopGalleryMovie);
        MBG.Instance.gameObject.SetActive(true);
        MBG.Instance.LoadMovie(movieName);
        MBG.Instance.SetDepthForMovieGallery();
        MBG.Instance.Play();
        UnityEngine.Debug.Log(movieName);
        if (movieName == "FMV019")
        {
            SoundLib.PlayMusic(89);
        }
        else if (movieName == "FMV040")
        {
            SoundLib.PlayMusic(118);
        }
        else if (movieName == "FMV055A" || movieName == "FMV055C" || movieName == "FMV055D")
        {
            SoundLib.PlayMusic(147);
        }
        else if (movieName == "FMV021")
        {
        }
    }

    public void PauseAndResumeGalleryMovie()
    {
        if (this.isPlayingMovie)
        {
            this.isPaused = !this.isPaused;
            MBG.Instance.Pause(this.isPaused);
            if (this.isPaused)
            {
                SoundLib.PauseMusic();
            }
            else
            {
                SoundLib.ResumeMusic();
            }
        }
    }

    public void StopGalleryMovie()
    {
        if (this.isPlayingMovie)
        {
            this.isPaused = false;
            this.clickedTimer = -1f;
            this.MovieHitArea.SetActive(false);
            ButtonGroupState.SetActiveGroupEnable(true);
            Loading = true;
            MBG.Instance.Stop();
            SoundLib.StopAllSounds(true);
            this.MovieForeGround.GetComponent<HonoFading>().FadeOut(delegate
            {
                Loading = false;
                this.isPlayingMovie = false;
                Int32 movieSoundIndex = SoundLib.GetMovieSoundIndex("FMV000");
                SoundProfile activeMovieAudioSoundProfile = SoundLib.GetActiveMovieAudioSoundProfile();
                Int32 num = -1;
                if (activeMovieAudioSoundProfile != null)
                {
                    num = SoundLib.GetMovieSoundIndex(activeMovieAudioSoundProfile.Name);
                }
                if (this.playSoundFMV000 && movieSoundIndex != num)
                {
                    SoundLib.PlayMovieMusic("FMV000", 0);
                }
            });
            SceneDirector.ClearFadeColor();
        }
    }

    private void OnMenuBlackjackButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        SoundLib.StopMovieMusic("FMV000", true);
        this.Hide(delegate
        {
            SceneDirector.Replace("EndGame", SceneTransition.FadeOutToBlack_FadeIn, true);
            SceneDirector.ToggleFadeAll(false);
        });
    }

    private void OpenLicensePanel()
    {
        this.isLicenseScene = true;
        this.LicensePanel.SetActive(true);
        ButtonGroupState.ActiveGroup = LicenseGroupButton;
    }

    private void CloseLicensePanel()
    {
        this.isLicenseScene = false;
        this.LicensePanel.SetActive(false);
    }

    private void UpdateLicenseText(String text)
    {
        String[] array = text.Split('♀');
        GameObject obj = this.LicenseCenterOnChild.gameObject;
        this.licensePageNum = array.Length;
        for (Int32 i = 0; i < this.licensePageNum; i++)
        {
            GameObject child = obj.GetChild(i);
            UILabel component = child.GetChild(0).GetComponent<UILabel>();
            child.SetActive(true);
            component.text = array[i];
        }
    }

    private void OnCenterLicensePage(GameObject centeredObject)
    {
        Int32 siblingIndex = centeredObject.transform.GetSiblingIndex();
        this.LicensePageIndicator.text = (siblingIndex + 1).ToString() + "/" + this.licensePageNum;
        GameObject parent = centeredObject.GetParent();
        for (Int32 i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.GetChild(i).GetChild(0);
            child.SetActive(false);
        }
        parent.GetChild(siblingIndex).GetChild(0).SetActive(true);
        if (siblingIndex > 0)
        {
            parent.GetChild(siblingIndex - 1).GetChild(0).SetActive(true);
        }
        if (siblingIndex < parent.transform.childCount - 1)
        {
            parent.GetChild(siblingIndex + 1).GetChild(0).SetActive(true);
        }
    }

    public void ShowMenuPanel()
    {
        AutoSplitterPipe.SignalLoadEnd();
        if (Configuration.Debug.StartModelViewer || Configuration.Debug.StartFieldCreator)
        {
            OnNewGameButtonClick();
            return;
        }
        this.SlideShowHitArea.SetActive(false);
        this.MenuPanelObject.SetActive(true);
        this.CheckCloudAvalability();
        Int32 movieSoundIndex = SoundLib.GetMovieSoundIndex("FMV000");
        SoundProfile activeMovieAudioSoundProfile = SoundLib.GetActiveMovieAudioSoundProfile();
        Int32 num = -1;
        if (activeMovieAudioSoundProfile != null)
        {
            num = SoundLib.GetMovieSoundIndex(activeMovieAudioSoundProfile.Name);
        }
        if (this.playSoundFMV000 && movieSoundIndex != num)
        {
            SoundLib.PlayMovieMusic("FMV000", 0);
        }
        SceneDirector.ClearFadeColor();
    }

    public void HideMenuPanel()
    {
        this.SlideShowHitArea.SetActive(true);
        this.MenuPanelObject.SetActive(false);
        ButtonGroupState.DisableAllGroup(true);
    }

    private void OnContinueButtonClick()
    {
        if (this.canContinue)
        {
            FF9Sfx.FF9SFX_Play(1261);
            FF9StateSystem.Serializer.Autoload(null, delegate (DataSerializerErrorCode errNo, Boolean success)
            {
                if (!success)
                    return;

                FadingComponent.fadeInDuration = 2f;
                this.Hide(delegate
                {
                    FadingComponent.fadeInDuration = 0.3f;
                    EventEngine instance = PersistenSingleton<EventEngine>.Instance;
                    instance.ReplaceLoadMap();
                    this.SplashScreenEnabled = true;
                    SoundLib.StopMovieMusic("FMV000", true);
                    AchievementManager.ResyncNormalAchievements();
                    FF9StateSystem.Settings.UpdateSetting();
                    ff9.w_frameNewSession();
                });
            });
        }
        else
        {
            FF9Sfx.FF9SFX_Play(102);
        }
    }

    private void OnNewGameButtonClick()
    {
        AutoSplitterPipe.SignalGameStart();
        FF9Sfx.FF9SFX_Play(3096);
        FadingComponent.fadeInDuration = 2f;
        this.Hide(delegate
        {
            FadingComponent.fadeInDuration = 0.3f;
            FF9StateSystem.ReInitStateSystem();
            SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
            this.SplashScreenEnabled = true;
            SoundLib.StopMovieMusic("FMV000", true);
        });
    }

    private void OnLoadGameButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        this.Hide(delegate
        {
            PersistenSingleton<UIManager>.Instance.SaveLoadScene.Type = SaveLoadUI.SerializeType.Load;
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Serialize);
        });
    }

    private void OnCloudGameButtonClick()
    {
        if (this.canSync)
        {
            FF9Sfx.FF9SFX_Play(103);
            this.Hide(delegate
            {
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Cloud);
            });
        }
        else
        {
            FF9Sfx.FF9SFX_Play(102);
        }
    }

    private void OnMenuMovieButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        this.MoviePanel.SetActive(true);
        if (this.movieThumbnailList == null)
        {
            this.movieThumbnailList = new List<GameObject>();
            this.movieThumbnails = this.MoviePageGrid.GetComponentsInChildren<ButtonGroupState>();
            ButtonGroupState[] array = this.movieThumbnails;
            for (Int32 i = 0; i < array.Length; i++)
            {
                ButtonGroupState buttonGroupState = array[i];
                this.movieThumbnailList.Add(buttonGroupState.gameObject);
                UIEventListener expr_68 = UIEventListener.Get(buttonGroupState.gameObject);
                expr_68.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_68.onClick, new UIEventListener.VoidDelegate(this.onClick));
            }
        }
        ButtonGroupState.ActiveGroup = MovieGalleryGroupButton;
        this.timer.Stop();
    }

    private void OnMenuLanguageButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        this.SelectLanguagePanel.SetActive(true);
        GameObject gameObjectFromCurrentLanguage = this.GetGameObjectFromCurrentLanguage();
        ButtonGroupState.SetCursorMemorize(gameObjectFromCurrentLanguage, LanguageGroupButton);
        ButtonGroupState.ActiveGroup = LanguageGroupButton;
        this.timer.Stop();
    }

    private void OnAchievementButtonClick()
    {
        this.fromAchievementPage = true;
        this.authenticatedBeforeClickingAchievementPage = Social.localUser.authenticated;
        FF9Sfx.FF9SFX_Play(103);
        SiliconStudio.Social.ShowAchievement();
    }

    private void OnFaqsButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        switch (Localization.CurrentLanguage)
        {
            case "English(US)": Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_us");  break;
            case "English(UK)": Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_uk");  break;
            case "Japanese":    Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_jp");  break;
            case "Spanish":     Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_spa"); break;
            case "French":      Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_fra"); break;
            case "German":      Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_ger"); break;
            case "Italian":     Application.OpenURL("http://an.sqexm.net/sp/site/Page/sqmk/ff9/help/help_ita"); break;
        }
    }

    private void OnSquareEnixButtonClick()
    {
        if (FF9StateSystem.PCEStorePlatform)
            return;

        FF9Sfx.FF9SFX_Play(103);
        switch (Localization.CurrentLanguage)
        {
            case "English(US)": Application.OpenURL("http://sqex-bridge.jp/");  break;
            case "English(UK)": Application.OpenURL("http://sqex-bridge.jp/");  break;
            case "Japanese":    Application.OpenURL("http://sqex-bridge.jp/");  break;
            case "Spanish":     Application.OpenURL("http://sqex-bridge.jp/");  break;
            case "French":      Application.OpenURL("http://sqex-bridge.jp/");  break;
            case "German":      Application.OpenURL("http://sqex-bridge.jp/");  break;
            case "Italian":     Application.OpenURL("http://sqex-bridge.jp/");  break;
        }
    }

    public void SwapScreenRotation()
    {
        if (FF9StateSystem.Settings.ScreenRotation == 3)
            FF9StateSystem.Settings.ScreenRotation = 4;
        else if (FF9StateSystem.Settings.ScreenRotation == 4)
            FF9StateSystem.Settings.ScreenRotation = 3;
        this.SetRotateScreen();
    }

    public void SetRotateScreen()
    {
        if (FF9StateSystem.Settings.ScreenRotation == 3)
        {
            if (this.rotateButtonSprite != null)
                this.rotateButtonSprite.spriteName = "title_menu_rotate_right";
        }
        else if (FF9StateSystem.Settings.ScreenRotation == 4 && this.rotateButtonSprite != null)
        {
            this.rotateButtonSprite.spriteName = "title_menu_rotate_left";
        }

        Screen.orientation = (ScreenOrientation)FF9StateSystem.Settings.ScreenRotation;
    }

    private void CheckAutoSaveSlot(DataSerializerErrorCode errNo, Boolean isSuccess)
    {
        this.canContinue = isSuccess;
        this.continueButtonSprite.color = isSuccess ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
        if (isSuccess && ButtonGroupState.ActiveGroup != MenuGroupButton && !ButtonGroupState.HaveCursorMemorize(MenuGroupButton))
            ButtonGroupState.SetCursorStartSelect(this.continueButton, MenuGroupButton);
    }

    private void CheckCloudAvalability()
    {
        this.canSync = SiliconStudio.Social.IsCloudAvailable();
        this.cloudButtonSprite.color = this.canSync ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
    }

    private void CheckGameFinishFlag(DataSerializerErrorCode errNo, Boolean isLoadSuccess, Boolean isFinished)
    {
        if (errNo == DataSerializerErrorCode.Success)
        {
            if (isLoadSuccess && isFinished)
            {
                this.MenuMovieButton.SetActive(true);
                if (FF9StateSystem.PCPlatform)
                    this.MenuBlackjackPCButton.SetActive(true);
                else
                    this.MenuBlackjackButton.SetActive(true);
            }
            else
            {
                this.MenuMovieButton.gameObject.SetActive(false);
                if (FF9StateSystem.PCPlatform)
                    this.MenuBlackjackPCButton.SetActive(false);
                else
                    this.MenuBlackjackButton.SetActive(false);
            }
        }
        else
        {
            this.MenuMovieButton.gameObject.SetActive(false);
            if (FF9StateSystem.PCPlatform)
                this.MenuBlackjackPCButton.SetActive(false);
            else
                this.MenuBlackjackButton.SetActive(false);
        }
    }

    private MenuLanguage GetMenuLanguageFromGameObject(GameObject go)
    {
        if (go == this.englishUSButton)
            return MenuLanguage.EnglishUS;
        if (go == this.japaneseButton)
            return MenuLanguage.Japanese;
        if (go == this.germanButton)
            return MenuLanguage.German;
        if (go == this.spanishButton)
            return MenuLanguage.Spanish;
        if (go == this.italianButton)
            return MenuLanguage.Italian;
        if (go == this.frenchButton)
            return MenuLanguage.French;
        if (go == this.englishUKButton)
            return MenuLanguage.EnglishUK;
        return MenuLanguage.None;
    }

    private GameObject GetGameObjectFromCurrentLanguage()
    {
        switch (Localization.CurrentLanguage)
        {
            case "English(US)": return this.englishUSButton;
            case "Japanese":    return this.japaneseButton;
            case "German":      return this.germanButton;
            case "Spanish":     return this.spanishButton;
            case "Italian":     return this.italianButton;
            case "French":      return this.frenchButton;
            case "English(UK)": return this.englishUKButton;
        }
        return null;
    }

    private void SetLanguage(String selectedLanguage)
    {
        FF9Sfx.FF9SFX_Play(103);
        this.BlockInput();
        FF9StateSystem.Settings.SetMenuLanguage(selectedLanguage, this.SetMenuLanguageCallback);
    }

    private void BlockInput()
    {
        Loading = true;
        ButtonGroupState.DisableAllGroup(true);
        this.BackButton.GetComponent<BoxCollider>().enabled = false;
        this.LoadingIcon.SetActive(true);
    }

    private void SetMenuLanguageCallback()
    {
        FF9Sfx.FF9SFX_Play(103);
        Loading = false;
        this.LoadingIcon.SetActive(false);
        ButtonGroupState.ActiveGroup = LanguageGroupButton;
        this.BackButton.GetComponent<BoxCollider>().enabled = true;
    }

    private void CloseSelectLanguagePanel()
    {
        this.SelectLanguagePanel.SetActive(false);
        ButtonGroupState.ActiveGroup = MenuGroupButton;
        this.timer.Start();
    }

    private void SetupIdleScreen()
    {
        this.idleScreen = new SlideShow(this.LogoSprite, this.SlideShowObject, this.SlideShowFadingObject, this.TitleImageTextObject0, this.TitleImageTextObject1, this.TitleImageTextJpObject0, this.TitleImageTextJpObject1);
        SceneVoidDelegate postMenuFadeout = this.HideMenuPanel;
        SceneVoidDelegate postIdleScreenFadeOut = this.ShowMenuPanel;
        SceneVoidDelegate postMenuFadeIn = delegate
        {
            switch (this.idleScreenType)
            {
                case SlideShow.Type.Sequence1:
                    this.idleScreenType = SlideShow.Type.Movie1;
                    break;
                case SlideShow.Type.Sequence2:
                    this.idleScreenType = SlideShow.Type.Movie2;
                    break;
                case SlideShow.Type.Movie1:
                    this.ShowMenuPanel();
                    this.idleScreenType = SlideShow.Type.Sequence2;
                    break;
                case SlideShow.Type.Movie2:
                    this.ShowMenuPanel();
                    this.idleScreenType = SlideShow.Type.Sequence1;
                    break;
                case SlideShow.Type.Logo:
                    this.idleScreenType = SlideShow.Type.Logo;
                    break;
            }
            ButtonGroupState.ActiveGroup = MenuGroupButton;
            this.timer.Start();
        };
        this.timer = new Timer(this.idleTime, delegate
        {
            ButtonGroupState.DisableAllGroup(true);
            this.idleScreen.Play(this.idleScreenType, postMenuFadeout, postIdleScreenFadeOut, postMenuFadeIn);
        });
    }

    private void PlaySplashScreen()
    {
        this.timer.Pause();
        this.HideMenuPanel();
        SceneVoidDelegate postSplashScreenFadeOut = delegate
        {
            this.SplashScreenEnabled = false;
        };
        SceneVoidDelegate postMenuFadeIn_ = delegate
        {
            this.ShowMenuPanel();
            this.idleScreenType = SlideShow.Type.Sequence1;
            ButtonGroupState.ActiveGroup = MenuGroupButton;

            if (Configuration.Graphics.SkipIntros > 2)
                this.timer.Stop();
            else
                this.timer.Start();

            if (this.IsJustLaunchApp)
            {
                SiliconStudio.Social.Authenticate(true);
                this.IsJustLaunchApp = false;
            }
            else
            {
                SteamSdkWrapper.RequestStats(delegate
                {
                    AchievementManager.ResyncSystemAchievements();
                });
            }
        };
        SceneVoidDelegate postMenuFadeIn = delegate
        {
            SceneVoidDelegate postMenuFadeOut = this.HideMenuPanel;
            SceneVoidDelegate postIdleScreenFadeOut = delegate
            {
                this.ShowMenuPanel();
                this.WaitForSocialLogin();
            };
            this.idleScreen.Play(SlideShow.Type.Logo, postMenuFadeOut, postIdleScreenFadeOut, postMenuFadeIn_);
        };
        this.slashScreen.Play(postSplashScreenFadeOut, postMenuFadeIn);
    }

    private void SetNonSocialUI()
    {
        if (!FF9StateSystem.AndroidTVPlatform && FF9StateSystem.MobilePlatform)
        {
            Vector3 localPosition = this.ScreenRotateButton.transform.localPosition;
            localPosition.y = -200f;
            this.ScreenRotateButton.transform.localPosition = localPosition;
            this.ScreenRotateButton.SetActive(true);
            UIEventListener.Get(this.ScreenRotateButton).onClick += this.onClick;
        }

        this.cloudButton.SetActive(false);
        this.MenuGroupPanel.transform.localPosition = new Vector3(0f, -300f, 0f);
    }

    private void WaitForSocialLogin()
    {
        StopCoroutine("WaitForSocialLoginProcess");
        StartCoroutine("WaitForSocialLoginProcess");
    }

    [DebuggerHidden]
    private IEnumerator WaitForSocialLoginProcess()
    {
        while (!SiliconStudio.Social.IsSocialPlatformAuthenticated())
            yield return new WaitForEndOfFrame();

        this.CheckCloudAvalability();
    }

    private void OnApplicationPause(Boolean pauseStatus)
    {
        if (!pauseStatus)
            this.CheckCloudAvalability();
    }

    private void Update()
    {
        this.timer.Update();
        if (ButtonGroupState.ActiveGroup == TitleUI.MovieGalleryGroupButton && (UIManager.Input.GetKeyTrigger(Control.Confirm) || Input.GetMouseButtonDown(0)) && (this.isPlayingMovie || base.Loading))
        {
            OSDLogger.AddStaticMessage(Time.frameCount + " : TAP!");
            if (!this.OnDoubleClickGalleryMovie())
                base.StartCoroutine(this.StopGalleryMovieCoroutine());
        }
    }

    private void Start()
    {
        try
        {
            File.WriteAllLines("FontList", Font.GetOSInstalledFontNames());
        }
        catch(Exception e)
        {
            Log.Error("Couldn't write the font list");
            Log.Error(e);
        }

        SiliconStudio.Social.InitializeSocialPlatform();
        PersistenSingleton<UIManager>.Instance.WorldHUDScene.EnableContinentTitle(false);
    }

    private void Awake()
    {
        ExpansionVerifier.printLog("TitleUI: awake lang = " + FF9StateSystem.Settings.CurrentLanguage);
        FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        this.continueButton = this.MenuGroupPanel.GetChild(0);
        this.newGameButton = this.MenuGroupPanel.GetChild(1);
        this.loadGameButton = this.MenuGroupPanel.GetChild(2);
        this.cloudButton = this.MenuGroupPanel.GetChild(3);
        this.rotateButtonSprite = this.ScreenRotateButton.GetComponent<UISprite>();
        this.continueButtonSprite = this.continueButton.GetComponent<UISprite>();
        this.cloudButtonSprite = this.cloudButton.GetComponent<UISprite>();
        this.MenuLanguageButton.GetChild(0).GetComponent<UILocalize>().key = "NameShort";
        UIEventListener.Get(this.continueButton).onClick += this.onClick;
        UIEventListener.Get(this.newGameButton).onClick += this.onClick;
        UIEventListener.Get(this.loadGameButton).onClick += this.onClick;
        UIEventListener.Get(this.MenuLanguageButton).onClick += this.onClick;
        UIEventListener.Get(this.MenuMovieButton).onClick += this.onClick;
        UIEventListener.Get(this.StaffLicenseButton).onClick += this.onClick;
        UIEventListener.Get(this.StaffBackButton).onClick += this.onClick;
        UIEventListener.Get(this.LicenseBackButton).onClick += this.onClick;
        UIEventListener.Get(this.MovieBackButton).onClick += this.onClick;
        UIEventListener.Get(this.SquareEnixButton).onClick += this.onClick;
        UIEventListener.Get(this.MenuStaffPCButton).onClick += this.onClick;
        UIEventListener.Get(this.MenuBlackjackPCButton).onClick += this.onClick;
        UIEventListener.Get(this.FaqsButton).onClick += this.onClick;
        UIEventListener.Get(this.MenuStaffButton).onClick += this.onClick;
        UIEventListener.Get(this.MenuBlackjackButton).onClick += this.onClick;
        UIEventListener.Get(this.AchievementButton).onClick += this.onClick;
        UIEventListener.Get(this.ScreenRotateButton).onClick += this.onClick;
        UIEventListener.Get(this.cloudButton).onClick += this.onClick;
        if (FF9StateSystem.PCPlatform)
        {
            this.FaqsButton.SetActive(false);
            this.MenuStaffButton.SetActive(false);
            this.MenuBlackjackButton.SetActive(false);
            this.MenuStaffPCButton.SetActive(true);
            this.MenuBlackjackPCButton.SetActive(true);
            this.SquareEnixButton.SetActive(true);
        }
        else
        {
            this.FaqsButton.SetActive(true);
            this.MenuStaffButton.SetActive(true);
            this.MenuBlackjackButton.SetActive(true);
            this.MenuStaffPCButton.SetActive(false);
            this.MenuBlackjackPCButton.SetActive(false);
            if (FF9StateSystem.AndroidAmazonPlatform)
            {
                this.ScreenRotateButton.transform.position = this.AchievementButton.transform.position;
                this.AchievementButton.transform.position = this.FaqsButton.transform.position;
                this.FaqsButton.transform.position = this.SquareEnixButton.transform.position;
                this.SquareEnixButton.SetActive(false);
            }
        }
        if (FF9StateSystem.MobilePlatform)
        {
            this.AchievementButton.SetActive(true);
            if (!FF9StateSystem.AndroidTVPlatform)
                this.ScreenRotateButton.SetActive(true);
        }
        this.japaneseButton = this.LanguageGroupPanel.GetChild(0);
        this.englishUSButton = this.LanguageGroupPanel.GetChild(1);
        this.frenchButton = this.LanguageGroupPanel.GetChild(2);
        this.spanishButton = this.LanguageGroupPanel.GetChild(3);
        this.germanButton = this.LanguageGroupPanel.GetChild(4);
        this.italianButton = this.LanguageGroupPanel.GetChild(5);
        this.englishUKButton = this.LanguageGroupPanel.GetChild(6);

        // Return a japanese version
        //this.japaneseButton.SetActive(false);
        //this.LanguageGroupPanel.transform.localPosition = new Vector3(this.LanguageGroupPanel.transform.localPosition.x, this.LanguageGroupPanel.transform.localPosition.y + 100f, this.LanguageGroupPanel.transform.localPosition.z);

        UIEventListener.Get(this.japaneseButton).onClick += this.onClick;
        UIEventListener.Get(this.englishUSButton).onClick += this.onClick;
        UIEventListener.Get(this.frenchButton).onClick += this.onClick;
        UIEventListener.Get(this.spanishButton).onClick += this.onClick;
        UIEventListener.Get(this.germanButton).onClick += this.onClick;
        UIEventListener.Get(this.italianButton).onClick += this.onClick;
        UIEventListener.Get(this.englishUKButton).onClick += this.onClick;
        UIEventListener.Get(this.BackButton).onClick += this.onClick;

        String credits;
        if (FF9StateSystem.AndroidAmazonPlatform)
            credits = CreditsImporter.TryLoadCredits("EmbeddedAsset/Manifest/Text/StaffCredits_Amazon.txt", ModTextResources.Import.CreditsAmazon);
        else if (FF9StateSystem.AndroidSQEXMarket)
            credits = CreditsImporter.TryLoadCredits("EmbeddedAsset/Manifest/Text/StaffCredits_AndroidSQEXMarket.txt", ModTextResources.Import.CreditsAndroid);
        else if (FF9StateSystem.MobilePlatform)
            credits = CreditsImporter.TryLoadCredits("EmbeddedAsset/Manifest/Text/StaffCredits_Mobile.txt", ModTextResources.Import.CreditsMobile);
        else if (FF9StateSystem.PCEStorePlatform)
            credits = CreditsImporter.TryLoadCredits("EmbeddedAsset/Manifest/Text/StaffCredits_EStore.txt", ModTextResources.Import.CreditsEStore);
        else
            credits = CreditsImporter.TryLoadCredits("EmbeddedAsset/Manifest/Text/StaffCredits_Steam.txt", ModTextResources.Import.CreditsSteam);

        if (!String.IsNullOrEmpty(credits))
            this.StaffLabel.text = credits;

        String licensePlatform;
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
                licensePlatform = "iOS";
                break;
            case RuntimePlatform.WindowsPlayer:
                if (FF9StateSystem.PCEStorePlatform)
                    licensePlatform = "EStore";
                else
                    licensePlatform = "Steam";
                break;
            case RuntimePlatform.Android:
                if (FF9StateSystem.AndroidAmazonPlatform)
                    licensePlatform = "Amazon";
                else if (FF9StateSystem.AndroidSQEXMarket)
                    licensePlatform = "AndroidSQEXMarket";
                else
                    licensePlatform = "Android";
                break;
            case RuntimePlatform.PS3:
            case RuntimePlatform.XBOX360:
            default:
                licensePlatform = "Steam";
                break;
        }

        String licenseText = AssetManager.LoadString("EmbeddedAsset/Manifest/Text/License_" + licensePlatform + ".txt");
        if (licenseText != null)
            this.UpdateLicenseText(licenseText);

        this.MovieCenterOnChild.onCenter += this.OnCenterMoviePage;
        this.LicenseCenterOnChild.onCenter += this.OnCenterLicensePage;
        this.onResumeFromQuit = delegate
        {
            if (this.isPlayingMovie)
            {
                MBG.Instance.SetDepthForMovieGallery();
                this.MenuPanelObject.SetActive(true);
                this.MoviePanel.SetActive(true);
                ButtonGroupState.ActiveButton = this.selectedMovieThumbnail;
                ButtonGroupState.SetActiveGroupEnable(false);
            }
        };
        if (Application.isMobilePlatform)
            this.MovieCenterOnChild.nextPageThreshold = 10f;
        this.slashScreen = new SlashScreen(null, this.SpashText, this.SlideShowObject, this.SlideShowFadingObject, this.TitleImageTextObject0, this.TitleImageTextObject1, this.TitleImageTextJpObject0, this.TitleImageTextJpObject1);
        UICamera.onNavigate += this.OnKeyNavigate;

        DataPatchers.Initialize();
        ScriptsLoader.InitializeAsync();
        QualitySettings.antiAliasing = Configuration.Graphics.AntiAliasing2;
    }

    private class SlideShow
    {
        public enum Type : byte
        {
            SplashScreen,
            Sequence1,
            Sequence2,
            Movie1,
            Movie2,
            Logo
        }

        private UI2DSprite ui2dSprite;

        private UI2DSprite titleImageText0;

        private UI2DSprite titleImageText1;

        private GameObject screenFadeGameObject;

        private GameObject slideShowObject;

        private GameObject titleImageTextObject0;

        private GameObject titleImageTextObject1;

        private GameObject titleImageTextJpObject0;

        private GameObject titleImageTextJpObject1;

        protected HonoFading honoFading;

        private Int32 listIndex;

        private Int32 index;

        private Boolean stopEnable;

        private Type type;

        private GameObject logoContainer;

        private UISprite logoSprite;

        private Int32 logoIndex;

        protected Single blackTime = 2f;

        protected Single whiteTime = 8f;

        protected Single fadeInterval = 2f;

        protected Single menuFadeInTime = 0.5f;

        protected Single skipFadeInTime = 0.2f;

        private SceneVoidDelegate beforeFirstCharacterFadeIn;

        private SceneVoidDelegate preCharacterFadeIn;

        private SceneVoidDelegate preCharacterFadeOut;

        private SceneVoidDelegate preMenuFadeIn;

        private SceneVoidDelegate onFadeOutFromTitle;

        private Action onMovieFinish;

        private SceneVoidDelegate beforeLogoFadeIn;

        private SceneVoidDelegate preLogoFadeIn;

        private SceneVoidDelegate preLogoFadeOut;

        private SceneVoidDelegate onLogoFinish;

        public Boolean isActive => this.slideShowObject.activeInHierarchy || MBG.Instance.gameObject.activeInHierarchy || this.logoContainer.activeInHierarchy;

        public SlideShow(Sprite[] pictureList1, Sprite[] pictureList2, GameObject slideShowObject, GameObject screenFadeGameObject, GameObject titleImageTextObject0, GameObject titleImageTextObject1, GameObject titleImageTextJpObject0, GameObject titleImageTextJpObject1)
        {
            this.screenFadeGameObject = screenFadeGameObject;
            this.slideShowObject = slideShowObject;
            this.honoFading = screenFadeGameObject.GetComponent<HonoFading>();
            this.titleImageTextObject0 = titleImageTextObject0;
            this.titleImageTextObject1 = titleImageTextObject1;
            this.titleImageTextJpObject0 = titleImageTextJpObject0;
            this.titleImageTextJpObject1 = titleImageTextJpObject1;
        }

        public SlideShow(UISprite logoSprite, GameObject slideShowObject, GameObject screenFadeGameObject, GameObject titleImageTextObject0, GameObject titleImageTextObject1, GameObject titleImageTextJpObject0, GameObject titleImageTextJpObject1)
        {
            this.logoSprite = logoSprite;
            this.logoContainer = logoSprite.transform.parent.gameObject;
            this.screenFadeGameObject = screenFadeGameObject;
            this.slideShowObject = slideShowObject;
            this.honoFading = screenFadeGameObject.GetComponent<HonoFading>();
            this.titleImageTextObject0 = titleImageTextObject0;
            this.titleImageTextObject1 = titleImageTextObject1;
            this.titleImageTextJpObject0 = titleImageTextJpObject0;
            this.titleImageTextJpObject1 = titleImageTextJpObject1;
        }

        public Sprite GetPicture(Type kind, Int32 idx)
        {
            if (kind == Type.Sequence1)
            {
                switch (idx)
                {
                    case 0:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_00", false);
                    case 1:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_01", false);
                    case 2:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_02", false);
                    case 3:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_03", false);
                }
            }
            else if (kind == Type.Sequence2)
            {
                switch (idx)
                {
                    case 0:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_04", false);
                    case 1:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_05", false);
                    case 2:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_06", false);
                    case 3:
                        return AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/title_image_07", false);
                }
            }
            else if (kind == Type.SplashScreen)
            {
                return null;
            }
            return null;
        }

        public Sprite GetTitleText(Int32 seqIndex, Int32 idx, Int32 textId)
        {
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_00_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_00_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_00_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_00_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_01_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_01_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_01_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_01_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_02_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_02_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_02_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_02_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_03_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_03_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_03_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_03_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_04_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_04_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_04_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_04_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_05_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_05_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_05_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_05_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_06_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_06_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_06_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_06_text1.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_07_text0").texture), "StreamingAssets/UI/Sprites/US/title_image_07_text0.png");
            //TextureHelper.WriteTextureToFile(TextureHelper.CopyAsReadable(Resources.Load<Sprite>("EmbeddedAsset/UI/Sprites/US/title_image_07_text1").texture), "StreamingAssets/UI/Sprites/US/title_image_07_text1.png");

            String text = Localization.GetSymbol();
            //if (text.Equals("JP"))
            //{
            //    text = "US";
            //}
            if (seqIndex != 0)
            {
                idx += 4;
            }

            return LoadSprite("UI/Sprites/" + text + "/title_image_0" + idx.ToString(CultureInfo.InvariantCulture) + "_text" + textId.ToString(CultureInfo.InvariantCulture));
        }

        private Sprite LoadSprite(String relativePath)
        {
            Sprite result = AssetManager.Load<Sprite>("EmbeddedAsset/" + relativePath, false);

            try
            {
                String externalPath = "StreamingAssets/" + relativePath + ".png";
                if (File.Exists(externalPath))
                    return Sprite.Create(StreamingResources.LoadTexture2D(externalPath), result.rect, result.pivot, result.pixelsPerUnit, 0, SpriteMeshType.Tight, result.border);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[TitleUI] Failed to load sprite: " + relativePath);
            }
            return result;
        }

        public void Play(Type kind, SceneVoidDelegate postMenuFadeOut, SceneVoidDelegate postIdleScreenFadeOut, SceneVoidDelegate postMenuFadeIn)
        {
            this.ui2dSprite = this.slideShowObject.GetComponent<UI2DSprite>();
            if (kind == Type.Sequence1 || kind == Type.Sequence2)
            {
                if (Localization.GetSymbol() == "JP")
                {
                    this.titleImageText0 = this.titleImageTextJpObject0.GetComponent<UI2DSprite>();
                    this.titleImageText1 = this.titleImageTextJpObject1.GetComponent<UI2DSprite>();
                    this.titleImageTextJpObject0.SetActive(true);
                    this.titleImageTextJpObject1.SetActive(true);
                    this.titleImageTextObject0.SetActive(false);
                    this.titleImageTextObject1.SetActive(false);
                    this.titleImageText0.MakePixelPerfect();
                    this.titleImageText1.MakePixelPerfect();
                }
                else
                {
                    this.titleImageText0 = this.titleImageTextObject0.GetComponent<UI2DSprite>();
                    this.titleImageText1 = this.titleImageTextObject1.GetComponent<UI2DSprite>();
                    this.titleImageTextObject0.SetActive(true);
                    this.titleImageTextObject1.SetActive(true);
                    this.titleImageTextJpObject0.SetActive(false);
                    this.titleImageTextJpObject1.SetActive(false);
                    this.titleImageText0.MakePixelPerfect();
                    this.titleImageText1.MakePixelPerfect();
                }
            }
            else
            {
                SoundLib.StopMovieMusic("FMV000", true);
                this.titleImageTextObject0.SetActive(false);
                this.titleImageTextObject1.SetActive(false);
                this.titleImageTextJpObject0.SetActive(false);
                this.titleImageTextJpObject1.SetActive(false);
            }
            this.index = 0;
            this.logoIndex = 0;
            this.type = kind;
            this.preMenuFadeIn = delegate
            {
                if (this.type == Type.SplashScreen)
                {
                    postIdleScreenFadeOut();
                    this.slideShowObject.SetActive(false);
                    this.stopEnable = false;
                    this.honoFading.Fade(1f, 0f, this.skipFadeInTime, 0f, this.honoFading.fadeInCurve, delegate
                    {
                    });
                    postMenuFadeIn();
                }
                else if (this.type == Type.Logo)
                {
                    if (this.logoIndex == 1)
                    {
                        this.stopEnable = false;
                        this.ChangeLogo(true);
                    }
                    else if (this.logoIndex == 2)
                    {
                        postIdleScreenFadeOut();
                        this.logoContainer.gameObject.SetActive(false);
                        this.stopEnable = false;
                        this.honoFading.Fade(1f, 0f, this.skipFadeInTime, 0f, this.honoFading.fadeInCurve, postMenuFadeIn);
                    }
                }
                else if (this.type == Type.Sequence1 || this.type == Type.Sequence2)
                {
                    postIdleScreenFadeOut();
                    this.slideShowObject.SetActive(false);
                    this.stopEnable = false;
                    this.honoFading.Fade(1f, 0f, this.skipFadeInTime, 0f, this.honoFading.fadeInCurve, postMenuFadeIn);
                }
                else if (this.logoIndex == 1)
                {
                    this.stopEnable = false;
                    this.ChangeLogo(true);
                }
                else if (this.logoIndex == 2)
                {
                    this.logoContainer.gameObject.SetActive(false);
                    MBG.Instance.Stop();
                    MBG.Instance.gameObject.SetActive(false);
                    postIdleScreenFadeOut();
                    this.slideShowObject.SetActive(false);
                    this.stopEnable = false;
                    this.honoFading.Fade(1f, 0f, this.skipFadeInTime, 0f, this.honoFading.fadeInCurve, postMenuFadeIn);
                }
            };
            this.preCharacterFadeIn = delegate
            {
                this.ui2dSprite.sprite2D = this.GetPicture(kind, this.index);
                if (kind == Type.Sequence1 || kind == Type.Sequence2)
                {
                    this.titleImageText0.sprite2D = this.GetTitleText(this.listIndex, this.index, 0);
                    this.titleImageText1.sprite2D = this.GetTitleText(this.listIndex, this.index, 1);
                    this.titleImageText0.MakePixelPerfect();
                    this.titleImageText1.MakePixelPerfect();
                }
                this.honoFading.Fade(1f, 0f, this.fadeInterval, this.blackTime, this.honoFading.fadeInCurve, this.preCharacterFadeOut);
            };
            this.preCharacterFadeOut = delegate
            {
                this.index++;
                Int32 num = (kind != Type.SplashScreen) ? 4 : 1;
                if (this.index < num)
                {
                    this.honoFading.Fade(0f, 1f, this.fadeInterval, this.whiteTime, this.honoFading.fadeInCurve, this.preCharacterFadeIn);
                }
                else
                {
                    this.honoFading.Fade(0f, 1f, this.fadeInterval, this.whiteTime, this.honoFading.fadeOutCurve, this.preMenuFadeIn);
                }
            };
            this.beforeFirstCharacterFadeIn = delegate
            {
                postMenuFadeOut();
                this.slideShowObject.SetActive(true);
                this.ui2dSprite.sprite2D = this.GetPicture(kind, this.index);
                if (kind == Type.Sequence1 || kind == Type.Sequence2)
                {
                    this.titleImageText0.sprite2D = this.GetTitleText(this.listIndex, this.index, 0);
                    this.titleImageText1.sprite2D = this.GetTitleText(this.listIndex, this.index, 1);
                    this.titleImageText0.MakePixelPerfect();
                    this.titleImageText1.MakePixelPerfect();
                }
                this.stopEnable = true;
                this.honoFading.Fade(1f, 0f, this.fadeInterval, this.blackTime, this.honoFading.fadeInCurve, this.preCharacterFadeOut);
            };
            this.preLogoFadeIn = delegate
            {
                this.ChangeLogo(false);
            };
            this.preLogoFadeOut = delegate
            {
                this.logoIndex++;
                this.stopEnable = true;
                if (this.logoIndex == 1)
                {
                    this.honoFading.Fade(0f, 1f, 1f, 1f, this.honoFading.fadeInCurve, this.preLogoFadeIn);
                }
                else if (this.logoIndex == 2)
                {
                    this.honoFading.Fade(0f, 1f, 1f, 1f, this.honoFading.fadeOutCurve, this.onLogoFinish);
                }
            };
            this.beforeLogoFadeIn = delegate
            {
                postMenuFadeOut();
                this.logoContainer.gameObject.SetActive(true);
                this.logoSprite.spriteName = this.logoIndex == 0 ? "logo_sqex" : "logo_sst";
                this.honoFading.Fade(1f, 0f, 0.7f, 1.3f, this.honoFading.fadeInCurve, this.preLogoFadeOut);
            };
            this.onLogoFinish = delegate
            {
                this.type = Type.Movie1;
                postMenuFadeOut();
                this.logoContainer.gameObject.SetActive(false);
                this.honoFading.Fade(1f, 0f, 1f, 0f, this.honoFading.fadeInCurve, delegate
                {
                });

                MBG.Instance.SetFinishCallback(this.onMovieFinish);
                MBG.Instance.gameObject.SetActive(true);
                MBG.Instance.LoadMovie("FMV000");
                MBG.Instance.SetDepthForTitle();
                MBG.Instance.Play();
                this.stopEnable = true;
            };
            this.onMovieFinish = delegate
            {
                this.honoFading.Fade(0f, 1f, 1f, 0f, this.honoFading.fadeInCurve, delegate
                {
                    this.preMenuFadeIn();
                });
            };
            if (Configuration.Graphics.SkipIntros == 1 && kind == Type.Logo)
            {
                postMenuFadeOut();
                this.logoIndex = 2;
                this.honoFading.Fade(0f, 1f, this.skipFadeInTime, 0f, this.honoFading.fadeOutCurve, this.onLogoFinish);
                return;
            }
            else if (Configuration.Graphics.SkipIntros > 0 && (kind == Type.SplashScreen || kind == Type.Logo))
            {
                postMenuFadeOut();
                postIdleScreenFadeOut();
                this.stopEnable = false;
                this.honoFading.Fade(1f, 0f, this.skipFadeInTime, 0f, this.honoFading.fadeInCurve, delegate
                {
                    postMenuFadeIn();
                });
                return;
            }
            switch (kind)
            {
                case Type.SplashScreen:
                    this.listIndex = 0;
                    this.FadeToBlack(this.beforeFirstCharacterFadeIn);
                    break;
                case Type.Sequence1:
                    this.listIndex = 0;
                    this.FadeToBlack(this.beforeFirstCharacterFadeIn);
                    break;
                case Type.Sequence2:
                    this.listIndex = 1;
                    this.FadeToBlack(this.beforeFirstCharacterFadeIn);
                    break;
                case Type.Movie1:
                    MBG.Instance.SetFinishCallback(this.onMovieFinish);
                    this.FadeToBlack(this.beforeLogoFadeIn);
                    break;
                case Type.Movie2:
                    MBG.Instance.SetFinishCallback(this.onMovieFinish);
                    this.FadeToBlack(this.beforeLogoFadeIn);
                    break;
                case Type.Logo:
                    this.FadeToBlack(this.beforeLogoFadeIn);
                    break;
                default:
                    throw new NotImplementedException("Not Implemented!");
            }
        }

        public void FadeToBlack(SceneVoidDelegate onFinish)
        {
            this.honoFading.Stop();
            Single value = this.honoFading.tweenAlpha.value;
            Single num = (1f - value) * this.fadeInterval;
            this.honoFading.Fade(value, 1f, num / 4f, 0f, this.honoFading.fadeOutCurve, onFinish);
        }

        public void Stop()
        {
            if (this.stopEnable)
            {
                this.FadeToBlack(this.preMenuFadeIn);
            }
        }

        private void ChangeLogo(Boolean isSkip)
        {
            this.logoSprite.spriteName = this.logoIndex == 0 ? "logo_sqex" : "logo_sst";
            this.stopEnable = false;
            ExpansionVerifier.printLog("TitleUI.SlideShow : logoSprite = " + this.logoSprite.spriteName);
            this.honoFading.Fade(1f, 0f, 0.7f, (!isSkip) ? 1.3f : 0f, this.honoFading.fadeInCurve, this.preLogoFadeOut);
        }
    }

    private class SlashScreen : SlideShow
    {
        private UILabel spashText;

        public SlashScreen(Sprite slashScreenPicture, UILabel spashText, GameObject slideShowGameObject, GameObject screenFadeGameObject, GameObject titleImageTextObject0, GameObject titleImageTextObject1, GameObject titleImageTextJpObject0, GameObject titleImageTextJpObject1) : base(new[]
        {
            slashScreenPicture
        }, null, slideShowGameObject, screenFadeGameObject, titleImageTextObject0, titleImageTextObject1, titleImageTextJpObject0, titleImageTextJpObject1)
        {
            this.blackTime = 0f;
            this.whiteTime = 1.5f;
            this.fadeInterval = 0.5f;
            this.spashText = spashText;
        }

        public void Play(SceneVoidDelegate postSplashScreenFadeOut, SceneVoidDelegate postMenuFadeIn)
        {
            this.honoFading.tweenAlpha.value = 1f;
            String text = Localization.GetSymbol();
            //if (text.Equals("JP"))
            //{
            //    text = "US";
            //}
            this.spashText.text = AssetManager.LoadString("EmbeddedAsset/Text/" + text + "/Title/warning");
            this.spashText.alignment = NGUIText.Alignment.Left;
            this.spashText.gameObject.SetActive(true);
            SceneVoidDelegate postIdleScreenFadeOut = delegate
            {
                postSplashScreenFadeOut();
                this.spashText.gameObject.SetActive(false);
            };
            base.Play(Type.SplashScreen, delegate
            {
            }, postIdleScreenFadeOut, postMenuFadeIn);
        }
    }

    private class Timer
    {
        public Single time;

        public Boolean pauseTimer;

        private Single timeout;

        private Action timeoutAction;

        public Timer(Single timeout, Action timeoutAction)
        {
            this.timeout = timeout;
            this.timeoutAction = timeoutAction;
            this.pauseTimer = true;
            this.time = 0f;
        }

        public void Start()
        {
            this.pauseTimer = false;
            this.time = 0f;
        }

        public void Stop()
        {
            this.pauseTimer = true;
            this.time = 0f;
        }

        public void Pause()
        {
            this.pauseTimer = true;
        }

        public void Continue()
        {
            this.pauseTimer = false;
        }

        public void Reset()
        {
            this.time = 0f;
        }

        public void Update()
        {
            if (!this.pauseTimer)
            {
                if (this.time > this.timeout)
                {
                    this.Stop();
                    this.timeoutAction();
                }
                else
                {
                    this.time += Time.deltaTime;
                }
            }
        }
    }

    private enum MenuLanguage
    {
        EnglishUS,
        Japanese,
        German,
        Spanish,
        Italian,
        French,
        EnglishUK,
        None,
    }
}
