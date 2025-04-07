using System;
using System.Linq;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Label")]
[ExecuteInEditMode]
public class UILabel : UIWidget
{
    public Dialog DialogWindow => this.transform?.parent?.parent?.GetComponent<Dialog>();
    public HelpDialog HelpDialogWindow => this.transform?.parent?.GetComponent<HelpDialog>();
    public UIInput InputField => this.transform?.parent?.GetComponent<UIInput>();
    public UIButton ButtonContainer => this.transform?.parent?.GetComponent<UIButton>();

    public void PrintIcon(DialogImage dialogImage, Single alpha = 1f)
    {
        if (!dialogImage.IsShown)
        {
            dialogImage.SpriteGo = Singleton<BitmapIconManager>.Instance.InsertBitmapIcon(dialogImage, base.gameObject);
            if (dialogImage.SpriteGo == null)
                return;
            NGUIText.SetIconDepth(base.gameObject, dialogImage.SpriteGo, DialogWindow != null);
            dialogImage.IsShown = true;
        }
        if (dialogImage.IsShown)
            Singleton<BitmapIconManager>.Instance.SetBitmapIconAlpha(dialogImage.SpriteGo, alpha);
    }

    public void HideIcon(DialogImage dialogImage)
    {
        if (dialogImage.IsShown)
        {
            dialogImage.SpriteGo.SetActive(false);
            dialogImage.IsShown = false;
        }
    }

    public void PreloadAllIcons()
    {
        foreach (FFIXTextTag tag in this.Parser.ParsedTagList)
        {
            DialogImage newImg = null;
            if (DialogBoxSymbols.ParseImageTag(tag, ref newImg) && newImg != null && !this.Parser.SpecialImages.Any(img => DialogImage.CompareImages(newImg, img)))
                this.Parser.SpecialImages.Add(newImg);
        }
        foreach (TextAnimatedTag animTag in this.Parser.AnimatedTags)
            foreach (DialogImage animImage in animTag.GetImages())
                if (!this.Parser.SpecialImages.Any(img => DialogImage.CompareImages(animImage, img)))
                    this.Parser.SpecialImages.Add(animImage);
        foreach (DialogImage dialogImage in this.Parser.SpecialImages)
        {
            if (!dialogImage.IsShown && dialogImage.SpriteGo == null)
            {
                this.PrintIcon(dialogImage);
                this.HideIcon(dialogImage);
            }
        }
    }

    public void ReleaseAllIcons()
    {
        Int32 childCount = base.transform.childCount;
        for (Int32 i = 0; i < childCount; i++)
            Singleton<BitmapIconManager>.Instance.RemoveBitmapIcon(base.transform.GetChild(0).gameObject);
        this.Parser.SpecialImages.Clear();
    }

    public Int32 finalFontSize => Mathf.RoundToInt(this.mScale * this.mFinalFontSize);

    private Int32 ReprocessCounter { get; set; }
    private Boolean shouldBeProcessed
    {
        get => this.mShouldBeProcessed;
        set
        {
            if (value)
            {
                this.mChanged = true;
                this.mShouldBeProcessed = true;
            }
            else
            {
                this.mShouldBeProcessed = false;
            }
        }
    }

    public override Boolean isAnchoredHorizontally => base.isAnchoredHorizontally || this.mOverflow == UILabel.Overflow.ResizeFreely;
    public override Boolean isAnchoredVertically => base.isAnchoredVertically || this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight;

    public override Material material
    {
        get
        {
            if (this.mMaterial != null)
                return this.mMaterial;
            if (this.mFont != null)
                return this.mFont.material;
            if (this.mTrueTypeFont != null)
                return this.mTrueTypeFont.material;
            return null;
        }
        set
        {
            if (this.mMaterial != value)
            {
                base.RemoveFromPanel();
                this.mMaterial = value;
                this.MarkAsChanged();
            }
        }
    }

    public UIFont bitmapFont
    {
        get => this.mFont;
        set
        {
            if (this.mFont != value)
            {
                base.RemoveFromPanel();
                this.mFont = value;
                this.mTrueTypeFont = null;
                this.MarkAsChanged();
            }
        }
    }

    public Font trueTypeFont
    {
        get
        {
            if (this.mTrueTypeFont != null)
                return this.mTrueTypeFont;
            if (this.mFont != null)
                return this.mFont.dynamicFont;
            return EncryptFontManager.SetDefaultFont();
        }
        set
        {
            if (this.mTrueTypeFont != value)
            {
                this.SetActiveFont(null);
                base.RemoveFromPanel();
                this.mTrueTypeFont = value;
                this.MarkAsChanged();
                this.mFont = null;
                this.SetActiveFont(value);
            }
        }
    }

    public UnityEngine.Object ambigiousFont
    {
        get => (UnityEngine.Object)this.mFont ?? this.mTrueTypeFont;
        set
        {
            if (value is UIFont uifont)
                this.bitmapFont = uifont;
            else
                this.trueTypeFont = value as Font;
        }
    }

    public String rawText
    {
        get => this.mText;
        set
        {
            if (this.mText == value && value != null)
                return;
            if (String.IsNullOrEmpty(value))
                value = String.Empty;
            this.mParser = new TextParser(this, value);
            this.mText = this.mParser.InitialText;
            this.ReprocessCounter = 1;
            this.ReleaseAllIcons();
            this.MarkAsChanged();
            if (this.autoResizeBoxCollider)
                base.ResizeCollider();
        }
    }

    public TextParser Parser
    {
        get
        {
            if (this.mParser == null)
            {
                if (this.mText == null)
                    this.mText = String.Empty;
                this.mParser = new TextParser(this, this.mText);
                this.ReprocessCounter = 1;
                this.ReleaseAllIcons();
                this.MarkAsChanged();
                if (this.autoResizeBoxCollider)
                    base.ResizeCollider();
            }
            return this.mParser;
        }
        set
        {
            if (this.mParser == value && value != null)
                return;
            if (value == null)
                value = new TextParser(this, String.Empty);
            this.mParser = value;
            this.mText = value.InitialText;
            this.ReprocessCounter = 1;
            this.ReleaseAllIcons();
            this.MarkAsChanged();
            if (this.autoResizeBoxCollider)
                base.ResizeCollider();
        }
    }

    public Int32 defaultFontSize => this.trueTypeFont == null ? (this.mFont == null ? 16 : this.mFont.defaultSize) : this.mFontSize;

    public Int32 fontSize
    {
        get => this.mFontSize;
        set
        {
            value = Mathf.Clamp(value, 0, 256);
            if (this.mFontSize != value)
            {
                this.mFontSize = value;
                this.MarkAsChanged();
            }
        }
    }

    public FontStyle fontStyle
    {
        get => this.mFontStyle;
        set
        {
            if (this.mFontStyle != value)
            {
                this.mFontStyle = value;
                this.MarkAsChanged();
            }
        }
    }

    public NGUIText.Alignment alignment
    {
        get => this.mAlignment;
        set
        {
            if (this.mAlignment != value)
            {
                this.mAlignment = value;
                this.MarkAsChanged();
            }
        }
    }

    /// <summary>Prevent using right alignment for right-to-left languages</summary>
    public Boolean fixedAlignment { get; set; } = false;
    /// <summary>Prevent word wrap</summary>
    public Boolean preventWrapping { get; set; } = false;

    public Boolean applyGradient
    {
        get => this.mApplyGradient;
        set
        {
            if (this.mApplyGradient != value)
            {
                this.mApplyGradient = value;
                this.MarkAsChanged();
            }
        }
    }

    /// <summary>Doesn't apply a global color filter like base.color but rather setup the initial color</summary>
    public Color DefaultTextColor { get; set; } = Color.white;

    public Color gradientTop
    {
        get => this.mGradientTop;
        set
        {
            if (this.mGradientTop != value)
            {
                this.mGradientTop = value;
                if (this.mApplyGradient)
                    this.MarkAsChanged();
            }
        }
    }

    public Color gradientBottom
    {
        get => this.mGradientBottom;
        set
        {
            if (this.mGradientBottom != value)
            {
                this.mGradientBottom = value;
                if (this.mApplyGradient)
                    this.MarkAsChanged();
            }
        }
    }

    public Int32 spacingX
    {
        get => this.mSpacingX;
        set
        {
            if (this.mSpacingX != value)
            {
                this.mSpacingX = value;
                this.MarkAsChanged();
            }
        }
    }

    public Int32 spacingY
    {
        get => this.mSpacingY;
        set
        {
            if (this.mSpacingY != value)
            {
                this.mSpacingY = value;
                this.MarkAsChanged();
            }
        }
    }

    public Boolean useFloatSpacing
    {
        get => this.mUseFloatSpacing;
        set
        {
            if (this.mUseFloatSpacing != value)
            {
                this.mUseFloatSpacing = value;
                this.MarkAsChanged();
            }
        }
    }

    public Single floatSpacingX
    {
        get => this.mFloatSpacingX;
        set
        {
            if (!Mathf.Approximately(this.mFloatSpacingX, value))
            {
                this.mFloatSpacingX = value;
                this.MarkAsChanged();
            }
        }
    }

    public Single floatSpacingY
    {
        get => this.mFloatSpacingY;
        set
        {
            if (!Mathf.Approximately(this.mFloatSpacingY, value))
            {
                this.mFloatSpacingY = value;
                this.MarkAsChanged();
            }
        }
    }

    public Single effectiveSpacingY => this.mUseFloatSpacing ? this.mFloatSpacingY : this.mSpacingY;
    public Single effectiveSpacingX => this.mUseFloatSpacing ? this.mFloatSpacingX : this.mSpacingX;

    /// <summary>Use different font sizes (font texture resolution) instead of different font scales (mesh vertices spacing)</summary>
    private Boolean keepCrisp => this.trueTypeFont != null && this.keepCrispWhenShrunk != UILabel.Crispness.Never;

    public Boolean supportEncoding
    {
        get => this.mEncoding;
        set
        {
            if (this.mEncoding != value)
            {
                this.mEncoding = value;
                this.MarkAsChanged();
                this.Parser.ResetCompletly();
            }
        }
    }

    public NGUIText.SymbolStyle symbolStyle
    {
        get => this.mSymbols;
        set
        {
            if (this.mSymbols != value)
            {
                this.mSymbols = value;
                this.MarkAsChanged();
            }
        }
    }

    public UILabel.Overflow overflowMethod
    {
        get => this.mOverflow;
        set
        {
            if (this.mOverflow != value)
            {
                this.mOverflow = value;
                this.MarkAsChanged();
            }
        }
    }

    public Boolean multiLine
    {
        get => this.mMaxLineCount != 1;
        set
        {
            if (this.mMaxLineCount != 1 != value)
            {
                this.mMaxLineCount = value ? 0 : 1;
                this.MarkAsChanged();
            }
        }
    }

    public override Vector3[] localCorners
    {
        get
        {
            if (this.shouldBeProcessed)
                this.ProcessText();
            return base.localCorners;
        }
    }

    public override Vector3[] worldCorners
    {
        get
        {
            if (this.shouldBeProcessed)
                this.ProcessText();
            return base.worldCorners;
        }
    }

    public override Vector4 drawingDimensions
    {
        get
        {
            if (this.shouldBeProcessed)
                this.ProcessText();
            return base.drawingDimensions;
        }
    }

    public Int32 maxLineCount
    {
        get => this.mMaxLineCount;
        set
        {
            if (this.mMaxLineCount != value)
            {
                this.mMaxLineCount = Mathf.Max(value, 0);
                this.MarkAsChanged();
                if (this.overflowMethod == UILabel.Overflow.ShrinkContent)
                    this.MakePixelPerfect();
            }
        }
    }

    public UILabel.Effect effectStyle
    {
        get => this.mEffectStyle;
        set
        {
            if (this.mEffectStyle != value)
            {
                this.mEffectStyle = value;
                this.MarkAsChanged();
            }
        }
    }

    public Color effectColor
    {
        get => this.mEffectColor;
        set
        {
            if (this.mEffectColor != value)
            {
                this.mEffectColor = value;
                if (this.mEffectStyle != UILabel.Effect.None)
                    this.MarkAsChanged();
            }
        }
    }

    public Vector2 effectDistance
    {
        get => this.mEffectDistance;
        set
        {
            if (this.mEffectDistance != value)
            {
                this.mEffectDistance = value;
                this.MarkAsChanged();
            }
        }
    }

    public Vector2 printedSize
    {
        get
        {
            if (this.shouldBeProcessed)
                this.ProcessText();
            return this.Parser.FullSize;
        }
    }

    public override Vector2 localSize
    {
        get
        {
            if (this.shouldBeProcessed)
                this.ProcessText();
            return base.localSize;
        }
    }

    private Boolean isValid => this.mFont != null || this.mTrueTypeFont != null;

    protected override void OnInit()
    {
        base.OnInit();
        UILabel.mList.Add(this);
        this.SetActiveFont(this.trueTypeFont);
    }

    protected override void OnDisable()
    {
        this.SetActiveFont(null);
        UILabel.mList.Remove(this);
        base.OnDisable();
    }

    protected void SetActiveFont(Font fnt)
    {
        if (this.mActiveTTF != fnt)
        {
            if (this.mActiveTTF != null && UILabel.mFontUsage.TryGetValue(this.mActiveTTF, out Int32 fontUsageCount))
            {
                fontUsageCount = Mathf.Max(0, fontUsageCount - 1);
                if (fontUsageCount == 0)
                    UILabel.mFontUsage.Remove(this.mActiveTTF);
                else
                    UILabel.mFontUsage[this.mActiveTTF] = fontUsageCount;
            }
            this.mActiveTTF = fnt;
            if (this.mActiveTTF != null)
                UILabel.mFontUsage[this.mActiveTTF] = 1;
        }
    }

    private static void OnFontChanged(Font font)
    {
        foreach (UILabel uilabel in UILabel.mList)
        {
            if (uilabel != null)
            {
                Font trueTypeFont = uilabel.trueTypeFont;
                if (trueTypeFont == font)
                    trueTypeFont.RequestCharactersInTexture(uilabel.mText + NGUIText.CHARACTER_CONSTANT_REQUESTS, uilabel.mFinalFontSize, uilabel.mFontStyle);
            }
        }
        foreach (UILabel uilabel in UILabel.mList)
        {
            if (uilabel != null)
            {
                Font trueTypeFont = uilabel.trueTypeFont;
                if (trueTypeFont == font)
                {
                    uilabel.RemoveFromPanel();
                    uilabel.CreatePanel();
                }
            }
        }
    }

    public override Vector3[] GetSides(Transform relativeTo)
    {
        if (this.shouldBeProcessed)
            this.ProcessText();
        return base.GetSides(relativeTo);
    }

    protected override void UpgradeFrom265()
    {
        System.IO.File.AppendAllText("aaaaadebuglabel.txt", $"[DBG] UpgradeFrom265 of {this.gameObject}: {this.mText.Replace('\n', '+')}");
        this.ProcessText();
        if (this.mShrinkToFit)
        {
            this.overflowMethod = UILabel.Overflow.ShrinkContent;
            this.mMaxLineCount = 0;
        }
        if (this.mMaxLineWidth != 0)
        {
            base.width = this.mMaxLineWidth;
            this.overflowMethod = this.mMaxLineCount <= 0 ? UILabel.Overflow.ShrinkContent : UILabel.Overflow.ResizeHeight;
        }
        else
        {
            this.overflowMethod = UILabel.Overflow.ResizeFreely;
        }
        if (this.mMaxLineHeight != 0)
            base.height = this.mMaxLineHeight;
        if (this.mFont != null)
        {
            Int32 defaultSize = this.mFont.defaultSize;
            if (base.height < defaultSize)
                base.height = defaultSize;
            this.fontSize = defaultSize;
        }
        this.mMaxLineWidth = 0;
        this.mMaxLineHeight = 0;
        this.mShrinkToFit = false;
        NGUITools.UpdateWidgetCollider(base.gameObject, true);
    }

    protected override void OnAnchor()
    {
        if (this.mOverflow == UILabel.Overflow.ResizeFreely)
        {
            if (base.isFullyAnchored)
                this.mOverflow = UILabel.Overflow.ShrinkContent;
        }
        else if (this.mOverflow == UILabel.Overflow.ResizeHeight && this.topAnchor.target != null && this.bottomAnchor.target != null)
        {
            this.mOverflow = UILabel.Overflow.ShrinkContent;
        }
        base.OnAnchor();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.mTrueTypeFont = EncryptFontManager.SetDefaultFont();
        if (!UILabel.mTexRebuildAdded)
        {
            UILabel.mTexRebuildAdded = true;
            Font.textureRebuilt += UILabel.OnFontChanged;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (this.mLineWidth > 0f)
        {
            this.mMaxLineWidth = Mathf.RoundToInt(this.mLineWidth);
            this.mLineWidth = 0f;
        }
        if (!this.mMultiline)
        {
            this.mMaxLineCount = 1;
            this.mMultiline = true;
        }
        this.mPremultiply = this.material != null && this.material.shader != null && this.material.shader.name.Contains("Premultiplied");
    }

    protected override void OnUpdate()
    {
        Boolean resetRender = false;
        if (this.mParser != null)
            foreach (TextAnimatedTag animTag in this.mParser.AnimatedTags)
                resetRender = animTag.ApplyTime(RealTime.time) || resetRender;
        if (resetRender)
            this.MarkAsChanged();
        base.OnUpdate();
    }

    /// <summary>Same as "label.rawText = text" but re-process the text even if it is the same as before</summary>
    public void SetText(String text)
    {
        this.mText = null;
        this.rawText = text;
    }

    /// <summary>Same as "label.Parser = parser" but re-process the text even if it is the same as before</summary>
    public void SetParser(TextParser parser)
    {
        this.mParser = null;
        this.Parser = parser;
    }

    public override void MarkAsChanged()
    {
        if (this.mParser != null)
            this.mParser.ResetRender();
        this.shouldBeProcessed = true;
        base.MarkAsChanged();
    }

    public void ProcessText()
    {
        this.Parser.Parse(TextParser.ParseStep.Wrapped);
    }

    public void GenerateWrapping()
    {
        if (!this.isValid)
            return;
        this.mChanged = true;
        this.shouldBeProcessed = false;
        Single drawWidth = this.mDrawRegion.z - this.mDrawRegion.x;
        Single drawHeight = this.mDrawRegion.w - this.mDrawRegion.y;
        this.mFinalFontSize = Mathf.Abs(this.defaultFontSize);
        this.mScale = 1f;
        this.UpdateNGUIText(false);
        if (this.mFinalFontSize > 0)
        {
            Boolean keepCrisp = this.keepCrisp;
            for (Int32 shrinkedFontSize = this.mFinalFontSize; shrinkedFontSize > 2; shrinkedFontSize -= 2)
            {
                Boolean canShrink = shrinkedFontSize > 4;
                if (keepCrisp)
                {
                    this.mFinalFontSize = shrinkedFontSize;
                    NGUIText.fontSize = this.mFinalFontSize;
                }
                else
                {
                    this.mScale = (Single)shrinkedFontSize / this.mFinalFontSize;
                    NGUIText.fontScale = this.trueTypeFont != null ? this.mScale : this.mScale * this.mFontSize / this.mFont.defaultSize;
                }
                NGUIText.UpdateFontSizes(false);
                Boolean properWrapping = NGUIText.WrapText(this.mParser, this.mOverflow == UILabel.Overflow.ShrinkContent && canShrink);
                if (this.mOverflow != UILabel.Overflow.ShrinkContent || properWrapping || !canShrink)
                {
                    if (this.DialogWindow != null)
                        this.mCalculatedSize = this.DialogWindow.Size;
                    else if (this.HelpDialogWindow != null)
                        this.mCalculatedSize = new Vector2(this.mWidth, this.mHeight);
                    else
                        this.mCalculatedSize = this.mParser.FullSize;
                    if (this.mOverflow == UILabel.Overflow.ResizeFreely)
                    {
                        this.mWidth = Mathf.Max(this.minWidth, Mathf.RoundToInt(this.mCalculatedSize.x));
                        if (drawWidth != 1f)
                            this.mWidth = Mathf.RoundToInt(this.mWidth / drawWidth);
                        this.mHeight = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
                        if (drawHeight != 1f)
                            this.mHeight = Mathf.RoundToInt(this.mHeight / drawHeight);
                        this.mWidth = NGUIText.EnsureEvenSize(this.mWidth);
                        this.mHeight = NGUIText.EnsureEvenSize(this.mHeight);
                    }
                    else if (this.mOverflow == UILabel.Overflow.ResizeHeight)
                    {
                        this.mHeight = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
                        if (drawHeight != 1f)
                            this.mHeight = Mathf.RoundToInt(this.mHeight / drawHeight);
                        this.mHeight = NGUIText.EnsureEvenSize(this.mHeight);
                    }
                    break;
                }
            }
        }
        else
        {
            base.cachedTransform.localScale = Vector3.one;
            this.mScale = 1f;
        }
        NGUIText.bitmapFont = null;
        NGUIText.dynamicFont = null;
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        try
        {
            if (!this.isValid)
                return;
            if (this.ReprocessCounter > 0)
            {
                // Computing wrapping at least twice fixes some (relatively rare) issues with texts not being properly rescaled to fit within their frame
                this.mParser.ResetBeforeVariableTags();
                this.ReprocessCounter--;
            }
            else
            {
                this.mParser.ResetRender(); // In theory, it would be enough there to only update mParser.UVs, but that optimisation would require storing logical UV datas in an extra list
            }
            this.mParser.Parse(TextParser.ParseStep.Render);
            Dialog dialog = this.DialogWindow;
            if (dialog != null && (dialog.StartChoiceRow >= 0 || dialog.IsOverlayDialog))
                UIKeyTrigger.preventTurboKey = true;
            if (this.DialogWindow == null)
                this.mParser.AdvanceProgressToMax(false);
            this.mParser.ApplyRenderProgress();
            this.geometry.verts = new BetterList<Vector3>(this.mParser.Vertices);
            this.geometry.uvs = new BetterList<Vector2>(this.mParser.UVs);
            this.geometry.cols = new BetterList<Color32>(this.mParser.Colors);
            NGUIText.bitmapFont = null;
            NGUIText.dynamicFont = null;
            if (this.onPostFill != null)
                this.onPostFill(this, 0, this.geometry.verts, this.geometry.uvs, this.geometry.cols);
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    public void GenerateTextRender()
    {
        Color textColor = base.color;
        textColor.a = this.finalAlpha;
        if (this.mFont != null && this.mFont.premultipliedAlphaShader)
            textColor = NGUITools.ApplyPMA(textColor);
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            textColor.r = Mathf.GammaToLinearSpace(textColor.r);
            textColor.g = Mathf.GammaToLinearSpace(textColor.g);
            textColor.b = Mathf.GammaToLinearSpace(textColor.b);
        }
        this.UpdateNGUIText(true);
        NGUIText.tint = textColor;
        NGUIText.GenerateTextRender(this.mParser);
    }

    public Vector2 GetApplyOffset()
    {
        Vector2 pivotOffset = base.pivotOffset;
        Single offsetX = Mathf.Round(Mathf.Lerp(0f, -this.mWidth, pivotOffset.x));
        Single offsetY = Mathf.Round(Mathf.Lerp(this.mCalculatedSize.y, 0f, pivotOffset.y));
        return new Vector2(offsetX, offsetY);
    }

    public override void MakePixelPerfect()
    {
        if (this.ambigiousFont != null)
        {
            Vector3 localPosition = base.cachedTransform.localPosition;
            localPosition.x = Mathf.RoundToInt(localPosition.x);
            localPosition.y = Mathf.RoundToInt(localPosition.y);
            localPosition.z = Mathf.RoundToInt(localPosition.z);
            base.cachedTransform.localPosition = localPosition;
            base.cachedTransform.localScale = Vector3.one;
            if (this.mOverflow == UILabel.Overflow.ResizeFreely)
            {
                this.AssumeNaturalSize();
            }
            else
            {
                Int32 width = this.mWidth;
                Int32 height = this.mHeight;
                UILabel.Overflow overflow = this.mOverflow;
                if (overflow != UILabel.Overflow.ResizeHeight)
                    this.mWidth = 100000;
                this.mHeight = 100000;
                this.mOverflow = UILabel.Overflow.ShrinkContent;
                this.ProcessText();
                this.mOverflow = overflow;
                this.mWidth = NGUIText.EnsureEvenSize(Mathf.Max(Mathf.RoundToInt(this.mCalculatedSize.x), base.minWidth));
                this.mHeight = NGUIText.EnsureEvenSize(Mathf.Max(Mathf.RoundToInt(this.mCalculatedSize.y), base.minHeight));
                if (this.mWidth < width)
                    this.mWidth = width;
                if (this.mHeight < height)
                    this.mHeight = height;
                this.MarkAsChanged();
            }
        }
        else
        {
            base.MakePixelPerfect();
        }
    }

    public void AssumeNaturalSize()
    {
        if (this.ambigiousFont != null)
        {
            this.mWidth = 100000;
            this.mHeight = 100000;
            this.ProcessText();
            this.mWidth = NGUIText.EnsureEvenSize(Mathf.RoundToInt(this.mCalculatedSize.x));
            this.mHeight = NGUIText.EnsureEvenSize(Mathf.RoundToInt(this.mCalculatedSize.y));
            this.MarkAsChanged();
        }
    }

    public Int32 GetCharacterIndexAtPosition(Vector3 worldPos, Boolean forSelectionIndex = true)
    {
        Vector2 localPos = base.cachedTransform.InverseTransformPoint(worldPos);
        return this.GetCharacterIndexAtPosition(localPos, forSelectionIndex);
    }

    public Int32 GetCharacterIndexAtPosition(Vector2 localPos, Boolean forSelectionIndex = true)
    {
        if (!this.isValid || this.mParser == null || this.mParser.Step < TextParser.ParseStep.Render)
            return forSelectionIndex ? 0 : -1;
        Int32 textLength = this.mParser.ParsedText.Length;
        Vector2[] screenPoints = new Vector2[4];
        Single closestDist = Single.MaxValue;
        Int32 closestIndex = -1;
        Boolean isAboveAll = true;
        Boolean isBelowAll = true;
        for (Int32 i = 0; i < textLength; i++)
        {
            Rect charRect = this.mParser.GetCharacterRenderRect(i);
            if (charRect.width == 0f && charRect.height == 0f)
                continue;
            screenPoints[0] = new Vector2(charRect.xMin, charRect.yMin);
            screenPoints[1] = new Vector2(charRect.xMin, charRect.yMax);
            screenPoints[2] = new Vector2(charRect.xMax, charRect.yMax);
            screenPoints[3] = new Vector2(charRect.xMax, charRect.yMin);
            Single dist = NGUIMath.DistanceToRectangle(screenPoints, localPos);
            if (dist <= 0f)
            {
                if (forSelectionIndex && localPos.x > charRect.center.x)
                    return i + 1;
                return i;
            }
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
            if (isAboveAll && localPos.y <= charRect.yMax)
                isAboveAll = false;
            if (isBelowAll && localPos.y >= charRect.yMin)
                isBelowAll = false;
        }
        if (!forSelectionIndex)
            return -1;
        if (isAboveAll || closestIndex < 0)
            return 0;
        if (isBelowAll || closestIndex == textLength - 1)
            return textLength;
        return closestIndex;
    }

    private static readonly Char[] WordStartCheck = [' ', '\n'];
    private static readonly Char[] WordEndCheck = [' ', '\n', ',', '.'];
    public String GetWordAtCharacterIndex(Int32 characterIndex)
    {
        if (characterIndex != -1 && characterIndex < this.mText.Length)
        {
            Int32 wordStart = Math.Min(this.mParser.ParsedText.Length, this.mParser.ParsedText.LastIndexOfAny(UILabel.WordStartCheck, 0, characterIndex) + 1);
            Int32 wordEnd = Math.Min(this.mParser.ParsedText.Length, this.mParser.ParsedText.IndexOfAny(UILabel.WordEndCheck, characterIndex));
            if (wordEnd < 0)
                wordEnd = this.mParser.ParsedText.Length;
            if (wordStart < wordEnd)
                return this.mParser.ParsedText.Substring(wordStart, wordEnd - wordStart);
        }
        return null;
    }

    public Int32 CalculateOffsetToFit(String text)
    {
        this.UpdateNGUIText();
        Int32 result = NGUIText.CalculateOffsetToFit(text);
        NGUIText.bitmapFont = null;
        NGUIText.dynamicFont = null;
        return result;
    }

    public void SetCurrentSelection()
    {
        if (UIPopupList.current != null)
            this.rawText = UIPopupList.current.isLocalized ? Localization.Get(UIPopupList.current.value) : UIPopupList.current.value;
    }

    public void UpdateNGUIText(Boolean renderStep = false)
    {
        Font trueTypeFont = this.trueTypeFont;
        Boolean hasTrueTypeFont = trueTypeFont != null;
        NGUIText.fontSize = this.mFinalFontSize;
        NGUIText.fontStyle = this.mFontStyle;
        if (!renderStep && this.mOverflow == UILabel.Overflow.ResizeFreely)
        {
            NGUIText.rectWidth = 1000000;
            NGUIText.regionWidth = 1000000;
        }
        else
        {
            NGUIText.rectWidth = this.mWidth;
            NGUIText.regionWidth = Mathf.RoundToInt(this.mWidth * (this.mDrawRegion.z - this.mDrawRegion.x));
        }
        if (!renderStep && (this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight))
        {
            NGUIText.rectHeight = 1000000;
            NGUIText.regionHeight = 1000000;
        }
        else
        {
            NGUIText.rectHeight = this.mHeight;
            NGUIText.regionHeight = Mathf.RoundToInt(this.mHeight * (this.mDrawRegion.w - this.mDrawRegion.y));
        }
        NGUIText.gradient = this.mApplyGradient && (this.mFont == null || !this.mFont.packedFontShader);
        NGUIText.gradientTop = this.mGradientTop;
        NGUIText.gradientBottom = this.mGradientBottom;
        NGUIText.encoding = this.mEncoding;
        NGUIText.premultiply = this.mPremultiply;
        NGUIText.symbolStyle = this.mSymbols;
        NGUIText.maxLines = this.mMaxLineCount;
        NGUIText.spacingX = this.effectiveSpacingX;
        NGUIText.spacingY = this.effectiveSpacingY;
        NGUIText.fontScale = hasTrueTypeFont ? this.mScale : (this.mScale * this.mFontSize / this.mFont.defaultSize);
        if (this.mFont != null)
        {
            NGUIText.bitmapFont = this.mFont;
            UIFont replacement;
            while ((replacement = NGUIText.bitmapFont.replacement) != null)
                NGUIText.bitmapFont = replacement;
            if (NGUIText.bitmapFont.isDynamic)
            {
                NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
                NGUIText.bitmapFont = null;
            }
            else
            {
                NGUIText.dynamicFont = null;
            }
        }
        else
        {
            NGUIText.dynamicFont = trueTypeFont;
            NGUIText.bitmapFont = null;
        }
        if (hasTrueTypeFont && this.keepCrisp)
        {
            UIRoot root = base.root;
            NGUIText.pixelDensity = root == null ? 1f : root.pixelSizeAdjustment;
        }
        else
        {
            NGUIText.pixelDensity = 1f;
        }
        if (this.alignment == NGUIText.Alignment.Automatic)
        {
            UIWidget.Pivot pivot = base.pivot;
            if (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.TopLeft || pivot == UIWidget.Pivot.BottomLeft)
                NGUIText.alignment = NGUIText.Alignment.Left;
            else if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.TopRight || pivot == UIWidget.Pivot.BottomRight)
                NGUIText.alignment = NGUIText.Alignment.Right;
            else
                NGUIText.alignment = NGUIText.Alignment.Center;
        }
        else
        {
            NGUIText.alignment = this.alignment;
        }
        NGUIText.fixedAlignment = this.fixedAlignment;
        NGUIText.preventWrapping = this.preventWrapping;
        NGUIText.UpdateFontSizes(true);
    }

    private void OnApplicationPause(Boolean paused)
    {
        if (!paused && this.mTrueTypeFont != null)
            this.Invalidate(false);
    }

    // Dummied
    private BetterList<DialogImage> imageList;
    private Boolean printIconAfterProcessedText;
    private BetterList<Int32> vertsLineOffsets;

    public UILabel.Crispness keepCrispWhenShrunk = UILabel.Crispness.OnDesktop;

    [SerializeField]
    [HideInInspector]
    private Font mTrueTypeFont;

    [SerializeField]
    [HideInInspector]
    private UIFont mFont;

    [Multiline(6)]
    [HideInInspector]
    [SerializeField]
    private String mText = String.Empty;

    [SerializeField]
    [HideInInspector]
    private Int32 mFontSize = 16;

    [SerializeField]
    [HideInInspector]
    private FontStyle mFontStyle;

    [HideInInspector]
    [SerializeField]
    private NGUIText.Alignment mAlignment;

    [HideInInspector]
    [SerializeField]
    private Boolean mEncoding = true;

    [SerializeField]
    [HideInInspector]
    private Int32 mMaxLineCount;

    [SerializeField]
    [HideInInspector]
    private UILabel.Effect mEffectStyle;

    [HideInInspector]
    [SerializeField]
    private Color mEffectColor = Color.black;

    [SerializeField]
    [HideInInspector]
    private NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Normal;

    [SerializeField]
    [HideInInspector]
    private Vector2 mEffectDistance = Vector2.one;

    [HideInInspector]
    [SerializeField]
    private UILabel.Overflow mOverflow;

    [HideInInspector]
    [SerializeField]
    private Material mMaterial;

    [HideInInspector]
    [SerializeField]
    private Boolean mApplyGradient;

    [HideInInspector]
    [SerializeField]
    private Color mGradientTop = Color.white;

    [SerializeField]
    [HideInInspector]
    private Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

    [HideInInspector]
    [SerializeField]
    private Int32 mSpacingX;

    [HideInInspector]
    [SerializeField]
    private Int32 mSpacingY;

    [HideInInspector]
    [SerializeField]
    private Boolean mUseFloatSpacing;

    [HideInInspector]
    [SerializeField]
    private Single mFloatSpacingX;

    [HideInInspector]
    [SerializeField]
    private Single mFloatSpacingY;

    [SerializeField]
    [HideInInspector]
    private Boolean mShrinkToFit;

    [HideInInspector]
    [SerializeField]
    private Int32 mMaxLineWidth;

    [SerializeField]
    [HideInInspector]
    private Int32 mMaxLineHeight;

    [SerializeField]
    [HideInInspector]
    private Single mLineWidth;

    [HideInInspector]
    [SerializeField]
    private Boolean mMultiline = true;

    [NonSerialized]
    private Font mActiveTTF;

    [NonSerialized]
    private Boolean mShouldBeProcessed = true;

    [NonSerialized]
    private Boolean mPremultiply;

    [NonSerialized]
    private Vector2 mCalculatedSize = Vector2.zero;

    [NonSerialized]
    private Single mScale = 1f;

    [NonSerialized]
    private Int32 mFinalFontSize;

    [NonSerialized]
    private TextParser mParser;

    private static BetterList<UILabel> mList = new BetterList<UILabel>();
    private static Dictionary<Font, Int32> mFontUsage = new Dictionary<Font, Int32>();

    private static Boolean mTexRebuildAdded = false;

    private static BetterList<Vector3> mTempVerts;
    private static BetterList<Int32> mTempIndices;

    public enum Effect
    {
        None,
        Shadow,
        Outline,
        Outline8
    }

    public enum Overflow
    {
        ShrinkContent,
        ClampContent,
        ResizeFreely,
        ResizeHeight
    }

    public enum Crispness
    {
        Never,
        OnDesktop,
        Always
    }
}
