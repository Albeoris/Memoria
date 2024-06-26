using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/UI/NGUI Label")]
[ExecuteInEditMode]
public class UILabel : UIWidget
{
    public UILabel()
    {
        this.imageList = new BetterList<Dialog.DialogImage>();
        this.vertsLineOffsets = new BetterList<Int32>();
    }

    public BetterList<Dialog.DialogImage> ImageList
    {
        get
        {
            return this.imageList;
        }
    }

    public Boolean PrintIconAfterProcessedText
    {
        get
        {
            return this.printIconAfterProcessedText;
        }
        set
        {
            this.printIconAfterProcessedText = value;
        }
    }

    public BetterList<Int32> VertsLineOffsets
    {
        get
        {
            return this.vertsLineOffsets;
        }
    }

    public void ApplyHighShadow(BetterList<Vector3> verts, BetterList<Int32> shadowVertIndexes, Single x, Single y)
    {
        foreach (Int32 num in shadowVertIndexes)
        {
            Vector3 vector = verts.buffer[num];
            vector.x += x;
            vector.y += y;
            verts.buffer[num] = vector;
        }
    }

    public void PrintIcon(BetterList<Dialog.DialogImage> imageList)
    {
        foreach (Dialog.DialogImage dialogImage in imageList)
        {
            if (!dialogImage.IsShown)
            {
                GameObject iconObject = Singleton<BitmapIconManager>.Instance.InsertBitmapIcon(dialogImage, base.gameObject);
                NGUIText.SetIconDepth(base.gameObject, iconObject, false);
                dialogImage.IsShown = true;
            }
        }
    }

    public void ReleaseIcon()
    {
        Int32 childCount = base.transform.childCount;
        for (Int32 i = 0; i < childCount; i++)
        {
            Singleton<BitmapIconManager>.Instance.RemoveBitmapIcon(base.transform.GetChild(0).gameObject);
        }
    }

    public String PhrasePreOpcodeSymbol(String text, ref Single additionalWidth)
    {
        return DialogLabelFilter.PhrasePreOpcodeSymbol(this, text.ToCharArray(), ref additionalWidth);
    }

    public void ApplyIconOffset(BetterList<Dialog.DialogImage> imageList, Vector2 offset)
    {
        foreach (Dialog.DialogImage dialogImage in imageList)
        {
            Dialog.DialogImage dialogImage2 = dialogImage;
            dialogImage2.LocalPosition.x = dialogImage2.LocalPosition.x + offset.x;
            Dialog.DialogImage dialogImage3 = dialogImage;
            dialogImage3.LocalPosition.y = dialogImage3.LocalPosition.y + offset.y;
        }
    }

    public Int32 finalFontSize
    {
        get
        {
            if (this.trueTypeFont)
            {
                return Mathf.RoundToInt(this.mScale * (Single)this.mFinalFontSize);
            }
            return Mathf.RoundToInt((Single)this.mFinalFontSize * this.mScale);
        }
    }

    private Boolean shouldBeProcessed
    {
        get
        {
            return this.mShouldBeProcessed;
        }
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

    public override Boolean isAnchoredHorizontally
    {
        get
        {
            return base.isAnchoredHorizontally || this.mOverflow == UILabel.Overflow.ResizeFreely;
        }
    }

    public override Boolean isAnchoredVertically
    {
        get
        {
            return base.isAnchoredVertically || this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight;
        }
    }

    public override Material material
    {
        get
        {
            if (this.mMaterial != (UnityEngine.Object)null)
            {
                return this.mMaterial;
            }
            if (this.mFont != (UnityEngine.Object)null)
            {
                return this.mFont.material;
            }
            if (this.mTrueTypeFont != (UnityEngine.Object)null)
            {
                return this.mTrueTypeFont.material;
            }
            return (Material)null;
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

    [Obsolete("Use UILabel.bitmapFont instead")]
    public UIFont font
    {
        get
        {
            return this.bitmapFont;
        }
        set
        {
            this.bitmapFont = value;
        }
    }

    public UIFont bitmapFont
    {
        get
        {
            return this.mFont;
        }
        set
        {
            if (this.mFont != value)
            {
                base.RemoveFromPanel();
                this.mFont = value;
                this.mTrueTypeFont = (Font)null;
                this.MarkAsChanged();
            }
        }
    }

    public Font trueTypeFont
    {
        get
        {
            if (this.mTrueTypeFont != (UnityEngine.Object)null)
            {
                return this.mTrueTypeFont;
            }
            if (this.mFont != (UnityEngine.Object)null)
            {
                return this.mFont.dynamicFont;
            }
            return EncryptFontManager.SetDefaultFont();
        }
        set
        {
            if (this.mTrueTypeFont != value)
            {
                this.SetActiveFont((Font)null);
                base.RemoveFromPanel();
                this.mTrueTypeFont = value;
                this.shouldBeProcessed = true;
                this.mFont = (UIFont)null;
                this.SetActiveFont(value);
                this.ProcessAndRequest();
                if (this.mActiveTTF != (UnityEngine.Object)null)
                {
                    base.MarkAsChanged();
                }
            }
        }
    }

    public UnityEngine.Object ambigiousFont
    {
        get
        {
            return (UnityEngine.Object)this.mFont ?? (UnityEngine.Object)this.mTrueTypeFont;
        }
        set
        {
            UIFont uifont = value as UIFont;
            if (uifont != (UnityEngine.Object)null)
            {
                this.bitmapFont = uifont;
            }
            else
            {
                this.trueTypeFont = (value as Font);
            }
        }
    }

    public String text
    {
        get
        {
            return this.mText;
        }
        set
        {
            if (this.mText == value)
            {
                return;
            }
            if (String.IsNullOrEmpty(value))
            {
                if (!String.IsNullOrEmpty(this.mText))
                {
                    this.mText = String.Empty;
                    this.MarkAsChanged();
                    this.ProcessAndRequest();
                }
            }
            else if (this.mText != value)
            {
                this.mText = value;
                this.MarkAsChanged();
                this.ProcessAndRequest();
            }
            if (this.autoResizeBoxCollider)
            {
                base.ResizeCollider();
            }
        }
    }

    public Int32 defaultFontSize
    {
        get
        {
            return (Int32)((!(this.trueTypeFont != (UnityEngine.Object)null)) ? ((Int32)((!(this.mFont != (UnityEngine.Object)null)) ? 16 : this.mFont.defaultSize)) : this.mFontSize);
        }
    }

    public Int32 fontSize
    {
        get
        {
            return this.mFontSize;
        }
        set
        {
            value = Mathf.Clamp(value, 0, 256);
            if (this.mFontSize != value)
            {
                this.mFontSize = value;
                this.shouldBeProcessed = true;
                this.ProcessAndRequest();
            }
        }
    }

    public FontStyle fontStyle
    {
        get
        {
            return this.mFontStyle;
        }
        set
        {
            if (this.mFontStyle != value)
            {
                this.mFontStyle = value;
                this.shouldBeProcessed = true;
                this.ProcessAndRequest();
            }
        }
    }

    public NGUIText.Alignment alignment
    {
        get
        {
            return this.mAlignment;
        }
        set
        {
            if (this.mAlignment != value)
            {
                this.mAlignment = value;
                this.shouldBeProcessed = true;
                this.ProcessAndRequest();
            }
        }
    }

    public Boolean applyGradient
    {
        get
        {
            return this.mApplyGradient;
        }
        set
        {
            if (this.mApplyGradient != value)
            {
                this.mApplyGradient = value;
                this.MarkAsChanged();
            }
        }
    }

    public Color gradientTop
    {
        get
        {
            return this.mGradientTop;
        }
        set
        {
            if (this.mGradientTop != value)
            {
                this.mGradientTop = value;
                if (this.mApplyGradient)
                {
                    this.MarkAsChanged();
                }
            }
        }
    }

    public Color gradientBottom
    {
        get
        {
            return this.mGradientBottom;
        }
        set
        {
            if (this.mGradientBottom != value)
            {
                this.mGradientBottom = value;
                if (this.mApplyGradient)
                {
                    this.MarkAsChanged();
                }
            }
        }
    }

    public Int32 spacingX
    {
        get
        {
            return this.mSpacingX;
        }
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
        get
        {
            return this.mSpacingY;
        }
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
        get
        {
            return this.mUseFloatSpacing;
        }
        set
        {
            if (this.mUseFloatSpacing != value)
            {
                this.mUseFloatSpacing = value;
                this.shouldBeProcessed = true;
            }
        }
    }

    public Single floatSpacingX
    {
        get
        {
            return this.mFloatSpacingX;
        }
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
        get
        {
            return this.mFloatSpacingY;
        }
        set
        {
            if (!Mathf.Approximately(this.mFloatSpacingY, value))
            {
                this.mFloatSpacingY = value;
                this.MarkAsChanged();
            }
        }
    }

    public Single effectiveSpacingY
    {
        get
        {
            return (!this.mUseFloatSpacing) ? ((Single)this.mSpacingY) : this.mFloatSpacingY;
        }
    }

    public Single effectiveSpacingX
    {
        get
        {
            return (!this.mUseFloatSpacing) ? ((Single)this.mSpacingX) : this.mFloatSpacingX;
        }
    }

    private Boolean keepCrisp
    {
        get
        {
            return this.trueTypeFont != (UnityEngine.Object)null && this.keepCrispWhenShrunk != UILabel.Crispness.Never;
        }
    }

    public Boolean supportEncoding
    {
        get
        {
            return this.mEncoding;
        }
        set
        {
            if (this.mEncoding != value)
            {
                this.mEncoding = value;
                this.shouldBeProcessed = true;
            }
        }
    }

    public NGUIText.SymbolStyle symbolStyle
    {
        get
        {
            return this.mSymbols;
        }
        set
        {
            if (this.mSymbols != value)
            {
                this.mSymbols = value;
                this.shouldBeProcessed = true;
            }
        }
    }

    public UILabel.Overflow overflowMethod
    {
        get
        {
            return this.mOverflow;
        }
        set
        {
            if (this.mOverflow != value)
            {
                this.mOverflow = value;
                this.shouldBeProcessed = true;
            }
        }
    }

    [Obsolete("Use 'width' instead")]
    public Int32 lineWidth
    {
        get
        {
            return base.width;
        }
        set
        {
            base.width = value;
        }
    }

    [Obsolete("Use 'height' instead")]
    public Int32 lineHeight
    {
        get
        {
            return base.height;
        }
        set
        {
            base.height = value;
        }
    }

    public Boolean multiLine
    {
        get
        {
            return this.mMaxLineCount != 1;
        }
        set
        {
            if (this.mMaxLineCount != 1 != value)
            {
                this.mMaxLineCount = (Int32)((!value) ? 1 : 0);
                this.shouldBeProcessed = true;
            }
        }
    }

    public override Vector3[] localCorners
    {
        get
        {
            if (this.shouldBeProcessed)
            {
                this.ProcessText();
            }
            return base.localCorners;
        }
    }

    public override Vector3[] worldCorners
    {
        get
        {
            if (this.shouldBeProcessed)
            {
                this.ProcessText();
            }
            return base.worldCorners;
        }
    }

    public override Vector4 drawingDimensions
    {
        get
        {
            if (this.shouldBeProcessed)
            {
                this.ProcessText();
            }
            return base.drawingDimensions;
        }
    }

    public Int32 maxLineCount
    {
        get
        {
            return this.mMaxLineCount;
        }
        set
        {
            if (this.mMaxLineCount != value)
            {
                this.mMaxLineCount = Mathf.Max(value, 0);
                this.shouldBeProcessed = true;
                if (this.overflowMethod == UILabel.Overflow.ShrinkContent)
                {
                    this.MakePixelPerfect();
                }
            }
        }
    }

    public UILabel.Effect effectStyle
    {
        get
        {
            return this.mEffectStyle;
        }
        set
        {
            if (this.mEffectStyle != value)
            {
                this.mEffectStyle = value;
                this.shouldBeProcessed = true;
            }
        }
    }

    public Color effectColor
    {
        get
        {
            return this.mEffectColor;
        }
        set
        {
            if (this.mEffectColor != value)
            {
                this.mEffectColor = value;
                if (this.mEffectStyle != UILabel.Effect.None)
                {
                    this.shouldBeProcessed = true;
                }
            }
        }
    }

    public Vector2 effectDistance
    {
        get
        {
            return this.mEffectDistance;
        }
        set
        {
            if (this.mEffectDistance != value)
            {
                this.mEffectDistance = value;
                this.shouldBeProcessed = true;
            }
        }
    }

    [Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
    public Boolean shrinkToFit
    {
        get
        {
            return this.mOverflow == UILabel.Overflow.ShrinkContent;
        }
        set
        {
            if (value)
            {
                this.overflowMethod = UILabel.Overflow.ShrinkContent;
            }
        }
    }

    public String processedText
    {
        get
        {
            if (this.mLastWidth != this.mWidth || this.mLastHeight != this.mHeight)
            {
                this.mLastWidth = this.mWidth;
                this.mLastHeight = this.mHeight;
                this.mShouldBeProcessed = true;
            }
            if (this.shouldBeProcessed)
            {
                this.ProcessText();
            }
            return this.mProcessedText;
        }
    }

    public Vector2 printedSize
    {
        get
        {
            if (this.shouldBeProcessed)
            {
                this.ProcessText();
            }
            return this.mCalculatedSize;
        }
    }

    public override Vector2 localSize
    {
        get
        {
            if (this.shouldBeProcessed)
            {
                this.ProcessText();
            }
            return base.localSize;
        }
    }

    private Boolean isValid
    {
        get
        {
            return this.mFont != (UnityEngine.Object)null || this.mTrueTypeFont != (UnityEngine.Object)null;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        UILabel.mList.Add(this);
        this.SetActiveFont(this.trueTypeFont);
    }

    protected override void OnDisable()
    {
        this.SetActiveFont((Font)null);
        UILabel.mList.Remove(this);
        base.OnDisable();
    }

    protected void SetActiveFont(Font fnt)
    {
        if (this.mActiveTTF != fnt)
        {
            Int32 num;
            if (this.mActiveTTF != (UnityEngine.Object)null && UILabel.mFontUsage.TryGetValue(this.mActiveTTF, out num))
            {
                num = Mathf.Max(0, --num);
                if (num == 0)
                {
                    UILabel.mFontUsage.Remove(this.mActiveTTF);
                }
                else
                {
                    UILabel.mFontUsage[this.mActiveTTF] = num;
                }
            }
            this.mActiveTTF = fnt;
            if (this.mActiveTTF != (UnityEngine.Object)null)
            {
                Int32 num2 = 0;
                UILabel.mFontUsage[this.mActiveTTF] = num2 + 1;
            }
        }
    }

    private static void OnFontChanged(Font font)
    {
        for (Int32 i = 0; i < UILabel.mList.size; i++)
        {
            UILabel uilabel = UILabel.mList[i];
            if (uilabel != (UnityEngine.Object)null)
            {
                Font trueTypeFont = uilabel.trueTypeFont;
                if (trueTypeFont == font)
                {
                    trueTypeFont.RequestCharactersInTexture(uilabel.mText, uilabel.mFinalFontSize, uilabel.mFontStyle);
                }
            }
        }
        for (Int32 j = 0; j < UILabel.mList.size; j++)
        {
            UILabel uilabel2 = UILabel.mList[j];
            if (uilabel2 != (UnityEngine.Object)null)
            {
                Font trueTypeFont2 = uilabel2.trueTypeFont;
                if (trueTypeFont2 == font)
                {
                    uilabel2.RemoveFromPanel();
                    uilabel2.CreatePanel();
                }
            }
        }
    }

    public override Vector3[] GetSides(Transform relativeTo)
    {
        if (this.shouldBeProcessed)
        {
            this.ProcessText();
        }
        return base.GetSides(relativeTo);
    }

    protected override void UpgradeFrom265()
    {
        this.ProcessText(true, true);
        if (this.mShrinkToFit)
        {
            this.overflowMethod = UILabel.Overflow.ShrinkContent;
            this.mMaxLineCount = 0;
        }
        if (this.mMaxLineWidth != 0)
        {
            base.width = this.mMaxLineWidth;
            this.overflowMethod = (UILabel.Overflow)((this.mMaxLineCount <= 0) ? UILabel.Overflow.ShrinkContent : UILabel.Overflow.ResizeHeight);
        }
        else
        {
            this.overflowMethod = UILabel.Overflow.ResizeFreely;
        }
        if (this.mMaxLineHeight != 0)
        {
            base.height = this.mMaxLineHeight;
        }
        if (this.mFont != (UnityEngine.Object)null)
        {
            Int32 defaultSize = this.mFont.defaultSize;
            if (base.height < defaultSize)
            {
                base.height = defaultSize;
            }
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
            {
                this.mOverflow = UILabel.Overflow.ShrinkContent;
            }
        }
        else if (this.mOverflow == UILabel.Overflow.ResizeHeight && this.topAnchor.target != (UnityEngine.Object)null && this.bottomAnchor.target != (UnityEngine.Object)null)
        {
            this.mOverflow = UILabel.Overflow.ShrinkContent;
        }
        base.OnAnchor();
    }

    private void ProcessAndRequest()
    {
        if (this.ambigiousFont != (UnityEngine.Object)null)
        {
            this.ProcessText();
        }
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
        this.mPremultiply = (this.material != (UnityEngine.Object)null && this.material.shader != (UnityEngine.Object)null && this.material.shader.name.Contains("Premultiplied"));
        this.ProcessAndRequest();
    }

    public override void MarkAsChanged()
    {
        this.shouldBeProcessed = true;
        base.MarkAsChanged();
    }

    public void ProcessText()
    {
        this.ProcessText(false, true);
    }

    private void ProcessText(Boolean legacyMode, Boolean full)
    {
        if (!this.isValid)
        {
            return;
        }
        this.mChanged = true;
        this.shouldBeProcessed = false;
        Single num = this.mDrawRegion.z - this.mDrawRegion.x;
        Single num2 = this.mDrawRegion.w - this.mDrawRegion.y;
        NGUIText.rectWidth = (Int32)((!legacyMode) ? base.width : ((Int32)((this.mMaxLineWidth == 0) ? 1000000 : this.mMaxLineWidth)));
        NGUIText.rectHeight = (Int32)((!legacyMode) ? base.height : ((Int32)((this.mMaxLineHeight == 0) ? 1000000 : this.mMaxLineHeight)));
        NGUIText.regionWidth = (Int32)((num == 1f) ? NGUIText.rectWidth : Mathf.RoundToInt((Single)NGUIText.rectWidth * num));
        NGUIText.regionHeight = (Int32)((num2 == 1f) ? NGUIText.rectHeight : Mathf.RoundToInt((Single)NGUIText.rectHeight * num2));
        this.mFinalFontSize = Mathf.Abs((Int32)((!legacyMode) ? this.defaultFontSize : Mathf.RoundToInt(base.cachedTransform.localScale.x)));
        this.mScale = 1f;
        if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 0)
        {
            this.mProcessedText = String.Empty;
            return;
        }
        Boolean flag = this.trueTypeFont != (UnityEngine.Object)null;
        if (flag && this.keepCrisp)
        {
            UIRoot root = base.root;
            if (root != (UnityEngine.Object)null)
            {
                this.mDensity = ((!(root != (UnityEngine.Object)null)) ? 1f : root.pixelSizeAdjustment);
            }
        }
        else
        {
            this.mDensity = 1f;
        }
        if (full)
        {
            this.UpdateNGUIText();
        }
        if (this.mOverflow == UILabel.Overflow.ResizeFreely)
        {
            NGUIText.rectWidth = 1000000;
            NGUIText.regionWidth = 1000000;
        }
        if (this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight)
        {
            NGUIText.rectHeight = 1000000;
            NGUIText.regionHeight = 1000000;
        }
        if (this.mFinalFontSize > 0)
        {
            Boolean keepCrisp = this.keepCrisp;
            for (Int32 i = this.mFinalFontSize; i > 0; i--)
            {
                if (keepCrisp)
                {
                    this.mFinalFontSize = i;
                    NGUIText.fontSize = this.mFinalFontSize;
                }
                else
                {
                    this.mScale = (Single)i / (Single)this.mFinalFontSize;
                    NGUIText.fontScale = ((!flag) ? ((Single)this.mFontSize / (Single)this.mFont.defaultSize * this.mScale) : this.mScale);
                }
                NGUIText.Update(false);
                Boolean flag2 = NGUIText.WrapText(this.mText, out this.mProcessedText, true, false);
                if (this.mOverflow != UILabel.Overflow.ShrinkContent || flag2)
                {
                    if (this.mOverflow == UILabel.Overflow.ResizeFreely)
                    {
                        this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
                        this.mWidth = Mathf.Max(this.minWidth, Mathf.RoundToInt(this.mCalculatedSize.x));
                        if (num != 1f)
                        {
                            this.mWidth = Mathf.RoundToInt((Single)this.mWidth / num);
                        }
                        this.mHeight = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
                        if (num2 != 1f)
                        {
                            this.mHeight = Mathf.RoundToInt((Single)this.mHeight / num2);
                        }
                        if ((this.mWidth & 1) == 1)
                        {
                            this.mWidth++;
                        }
                        if ((this.mHeight & 1) == 1)
                        {
                            this.mHeight++;
                        }
                    }
                    else if (this.mOverflow == UILabel.Overflow.ResizeHeight)
                    {
                        this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
                        this.mHeight = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
                        if (num2 != 1f)
                        {
                            this.mHeight = Mathf.RoundToInt((Single)this.mHeight / num2);
                        }
                        if ((this.mHeight & 1) == 1)
                        {
                            this.mHeight++;
                        }
                    }
                    else
                    {
                        this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
                    }
                    if (legacyMode)
                    {
                        base.width = Mathf.RoundToInt(this.mCalculatedSize.x);
                        base.height = Mathf.RoundToInt(this.mCalculatedSize.y);
                        base.cachedTransform.localScale = Vector3.one;
                    }
                    break;
                }
                if (--i <= 1)
                {
                    break;
                }
            }
        }
        else
        {
            base.cachedTransform.localScale = Vector3.one;
            this.mProcessedText = String.Empty;
            this.mScale = 1f;
        }
        if (full)
        {
            NGUIText.bitmapFont = (UIFont)null;
            NGUIText.dynamicFont = (Font)null;
        }
    }

    public override void MakePixelPerfect()
    {
        if (this.ambigiousFont != (UnityEngine.Object)null)
        {
            Vector3 localPosition = base.cachedTransform.localPosition;
            localPosition.x = (Single)Mathf.RoundToInt(localPosition.x);
            localPosition.y = (Single)Mathf.RoundToInt(localPosition.y);
            localPosition.z = (Single)Mathf.RoundToInt(localPosition.z);
            base.cachedTransform.localPosition = localPosition;
            base.cachedTransform.localScale = Vector3.one;
            if (this.mOverflow == UILabel.Overflow.ResizeFreely)
            {
                this.AssumeNaturalSize();
            }
            else
            {
                Int32 width = base.width;
                Int32 height = base.height;
                UILabel.Overflow overflow = this.mOverflow;
                if (overflow != UILabel.Overflow.ResizeHeight)
                {
                    this.mWidth = 100000;
                }
                this.mHeight = 100000;
                this.mOverflow = UILabel.Overflow.ShrinkContent;
                this.ProcessText(false, true);
                this.mOverflow = overflow;
                Int32 num = Mathf.RoundToInt(this.mCalculatedSize.x);
                Int32 num2 = Mathf.RoundToInt(this.mCalculatedSize.y);
                num = Mathf.Max(num, base.minWidth);
                num2 = Mathf.Max(num2, base.minHeight);
                if ((num & 1) == 1)
                {
                    num++;
                }
                if ((num2 & 1) == 1)
                {
                    num2++;
                }
                this.mWidth = Mathf.Max(width, num);
                this.mHeight = Mathf.Max(height, num2);
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
        if (this.ambigiousFont != (UnityEngine.Object)null)
        {
            this.mWidth = 100000;
            this.mHeight = 100000;
            this.ProcessText(false, true);
            this.mWidth = Mathf.RoundToInt(this.mCalculatedSize.x);
            this.mHeight = Mathf.RoundToInt(this.mCalculatedSize.y);
            if ((this.mWidth & 1) == 1)
            {
                this.mWidth++;
            }
            if ((this.mHeight & 1) == 1)
            {
                this.mHeight++;
            }
            this.MarkAsChanged();
        }
    }

    [Obsolete("Use UILabel.GetCharacterAtPosition instead")]
    public Int32 GetCharacterIndex(Vector3 worldPos)
    {
        return this.GetCharacterIndexAtPosition(worldPos, false);
    }

    [Obsolete("Use UILabel.GetCharacterAtPosition instead")]
    public Int32 GetCharacterIndex(Vector2 localPos)
    {
        return this.GetCharacterIndexAtPosition(localPos, false);
    }

    public Int32 GetCharacterIndexAtPosition(Vector3 worldPos, Boolean precise)
    {
        Vector2 localPos = base.cachedTransform.InverseTransformPoint(worldPos);
        return this.GetCharacterIndexAtPosition(localPos, precise);
    }

    public Int32 GetCharacterIndexAtPosition(Vector2 localPos, Boolean precise)
    {
        if (this.isValid)
        {
            String processedText = this.processedText;
            if (String.IsNullOrEmpty(processedText))
            {
                return 0;
            }
            this.UpdateNGUIText();
            if (precise)
            {
                NGUIText.PrintExactCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
            }
            else
            {
                NGUIText.PrintApproximateCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
            }
            if (UILabel.mTempVerts.size > 0)
            {
                this.ApplyOffset(UILabel.mTempVerts, 0);
                Int32 result = (Int32)((!precise) ? NGUIText.GetApproximateCharacterIndex(UILabel.mTempVerts, UILabel.mTempIndices, localPos) : NGUIText.GetExactCharacterIndex(UILabel.mTempVerts, UILabel.mTempIndices, localPos));
                UILabel.mTempVerts.Clear();
                UILabel.mTempIndices.Clear();
                NGUIText.bitmapFont = (UIFont)null;
                NGUIText.dynamicFont = (Font)null;
                return result;
            }
            NGUIText.bitmapFont = (UIFont)null;
            NGUIText.dynamicFont = (Font)null;
        }
        return 0;
    }

    public String GetWordAtPosition(Vector3 worldPos)
    {
        Int32 characterIndexAtPosition = this.GetCharacterIndexAtPosition(worldPos, true);
        return this.GetWordAtCharacterIndex(characterIndexAtPosition);
    }

    public String GetWordAtPosition(Vector2 localPos)
    {
        Int32 characterIndexAtPosition = this.GetCharacterIndexAtPosition(localPos, true);
        return this.GetWordAtCharacterIndex(characterIndexAtPosition);
    }

    public String GetWordAtCharacterIndex(Int32 characterIndex)
    {
        if (characterIndex != -1 && characterIndex < this.mText.Length)
        {
            Int32 num = this.mText.LastIndexOfAny(new Char[]
            {
                ' ',
                '\n'
            }, characterIndex) + 1;
            Int32 num2 = this.mText.IndexOfAny(new Char[]
            {
                ' ',
                '\n',
                ',',
                '.'
            }, characterIndex);
            if (num2 == -1)
            {
                num2 = this.mText.Length;
            }
            if (num != num2)
            {
                Int32 num3 = num2 - num;
                if (num3 > 0)
                {
                    String text = this.mText.Substring(num, num3);
                    return NGUIText.StripSymbols(text);
                }
            }
        }
        return (String)null;
    }

    public String GetUrlAtPosition(Vector3 worldPos)
    {
        return this.GetUrlAtCharacterIndex(this.GetCharacterIndexAtPosition(worldPos, true));
    }

    public String GetUrlAtPosition(Vector2 localPos)
    {
        return this.GetUrlAtCharacterIndex(this.GetCharacterIndexAtPosition(localPos, true));
    }

    public String GetUrlAtCharacterIndex(Int32 characterIndex)
    {
        if (characterIndex != -1 && characterIndex < this.mText.Length - 6)
        {
            Int32 num;
            if (this.mText[characterIndex] == '[' && this.mText[characterIndex + 1] == 'u' && this.mText[characterIndex + 2] == 'r' && this.mText[characterIndex + 3] == 'l' && this.mText[characterIndex + 4] == '=')
            {
                num = characterIndex;
            }
            else
            {
                num = this.mText.LastIndexOf("[url=", characterIndex);
            }
            if (num == -1)
            {
                return (String)null;
            }
            num += 5;
            Int32 num2 = this.mText.IndexOf("]", num);
            if (num2 == -1)
            {
                return (String)null;
            }
            Int32 num3 = this.mText.IndexOf("[/url]", num2);
            if (num3 == -1 || characterIndex <= num3)
            {
                return this.mText.Substring(num, num2 - num);
            }
        }
        return (String)null;
    }

    public Int32 GetCharacterIndex(Int32 currentIndex, KeyCode key)
    {
        if (this.isValid)
        {
            String processedText = this.processedText;
            if (String.IsNullOrEmpty(processedText))
            {
                return 0;
            }
            Int32 defaultFontSize = this.defaultFontSize;
            this.UpdateNGUIText();
            NGUIText.PrintApproximateCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
            if (UILabel.mTempVerts.size > 0)
            {
                this.ApplyOffset(UILabel.mTempVerts, 0);
                Int32 i = 0;
                while (i < UILabel.mTempIndices.size)
                {
                    if (UILabel.mTempIndices[i] == currentIndex)
                    {
                        Vector2 pos = UILabel.mTempVerts[i];
                        if (key == KeyCode.UpArrow)
                        {
                            pos.y += (Single)defaultFontSize + this.effectiveSpacingY;
                        }
                        else if (key == KeyCode.DownArrow)
                        {
                            pos.y -= (Single)defaultFontSize + this.effectiveSpacingY;
                        }
                        else if (key == KeyCode.Home)
                        {
                            pos.x -= 1000f;
                        }
                        else if (key == KeyCode.End)
                        {
                            pos.x += 1000f;
                        }
                        Int32 approximateCharacterIndex = NGUIText.GetApproximateCharacterIndex(UILabel.mTempVerts, UILabel.mTempIndices, pos);
                        if (approximateCharacterIndex == currentIndex)
                        {
                            break;
                        }
                        UILabel.mTempVerts.Clear();
                        UILabel.mTempIndices.Clear();
                        return approximateCharacterIndex;
                    }
                    else
                    {
                        i++;
                    }
                }
                UILabel.mTempVerts.Clear();
                UILabel.mTempIndices.Clear();
            }
            NGUIText.bitmapFont = (UIFont)null;
            NGUIText.dynamicFont = (Font)null;
            if (key == KeyCode.UpArrow || key == KeyCode.Home)
            {
                return 0;
            }
            if (key == KeyCode.DownArrow || key == KeyCode.End)
            {
                return processedText.Length;
            }
        }
        return currentIndex;
    }

    public void PrintOverlay(Int32 start, Int32 end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
    {
        if (caret != null)
        {
            caret.Clear();
        }
        if (highlight != null)
        {
            highlight.Clear();
        }
        if (!this.isValid)
        {
            return;
        }
        String processedText = this.processedText;
        this.UpdateNGUIText();
        Int32 size = caret.verts.size;
        Vector2 item = new Vector2(0.5f, 0.5f);
        Single finalAlpha = this.finalAlpha;
        if (highlight != null && start != end)
        {
            Int32 size2 = highlight.verts.size;
            NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, highlight.verts);
            if (highlight.verts.size > size2)
            {
                this.ApplyOffset(highlight.verts, size2);
                Color32 item2 = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * finalAlpha);
                for (Int32 i = size2; i < highlight.verts.size; i++)
                {
                    highlight.uvs.Add(item);
                    highlight.cols.Add(item2);
                }
            }
        }
        else
        {
            NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, null);
        }
        this.ApplyOffset(caret.verts, size);
        Color32 item3 = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * finalAlpha);
        for (Int32 j = size; j < caret.verts.size; j++)
        {
            caret.uvs.Add(item);
            caret.cols.Add(item3);
        }
        NGUIText.bitmapFont = (UIFont)null;
        NGUIText.dynamicFont = (Font)null;
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        if (!this.isValid)
        {
            return;
        }
        Int32 num = verts.size;
        Color color = base.color;
        color.a = this.finalAlpha;
        if (this.mFont != (UnityEngine.Object)null && this.mFont.premultipliedAlphaShader)
        {
            color = NGUITools.ApplyPMA(color);
        }
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            color.r = Mathf.GammaToLinearSpace(color.r);
            color.g = Mathf.GammaToLinearSpace(color.g);
            color.b = Mathf.GammaToLinearSpace(color.b);
        }
        String processedText = this.processedText;
        Int32 size = verts.size;
        this.UpdateNGUIText();
        if (this.printIconAfterProcessedText)
        {
            this.ReleaseIcon();
        }
        NGUIText.tint = color;
        BetterList<Int32> shadowVertIndexes;
        NGUIText.Print(processedText, verts, uvs, cols, out shadowVertIndexes, out this.imageList, out this.vertsLineOffsets);
        NGUIText.bitmapFont = (UIFont)null;
        NGUIText.dynamicFont = (Font)null;
        Vector2 offset = this.ApplyOffset(verts, size);
        this.ApplyIconOffset(this.imageList, offset);
        if (this.mFont != (UnityEngine.Object)null && this.mFont.packedFontShader)
        {
            return;
        }
        if (this.effectStyle != UILabel.Effect.None)
        {
            Int32 size2 = verts.size;
            offset.x = this.mEffectDistance.x;
            offset.y = this.mEffectDistance.y;
            this.ApplyShadow(verts, uvs, cols, num, size2, offset.x, -offset.y);
            this.ApplyHighShadow(verts, shadowVertIndexes, offset.x / 3f, -offset.y / 3f);
            if (this.effectStyle == UILabel.Effect.Outline || this.effectStyle == UILabel.Effect.Outline8)
            {
                num = size2;
                size2 = verts.size;
                this.ApplyShadow(verts, uvs, cols, num, size2, -offset.x, offset.y);
                num = size2;
                size2 = verts.size;
                this.ApplyShadow(verts, uvs, cols, num, size2, offset.x, offset.y);
                num = size2;
                size2 = verts.size;
                this.ApplyShadow(verts, uvs, cols, num, size2, -offset.x, -offset.y);
                if (this.effectStyle == UILabel.Effect.Outline8)
                {
                    num = size2;
                    size2 = verts.size;
                    this.ApplyShadow(verts, uvs, cols, num, size2, -offset.x, 0f);
                    num = size2;
                    size2 = verts.size;
                    this.ApplyShadow(verts, uvs, cols, num, size2, offset.x, 0f);
                    num = size2;
                    size2 = verts.size;
                    this.ApplyShadow(verts, uvs, cols, num, size2, 0f, offset.y);
                    num = size2;
                    size2 = verts.size;
                    this.ApplyShadow(verts, uvs, cols, num, size2, 0f, -offset.y);
                }
            }
        }
        if (this.onPostFill != null)
        {
            this.onPostFill(this, num, verts, uvs, cols);
        }
        if (this.printIconAfterProcessedText)
        {
            this.PrintIcon(this.imageList);
        }
    }

    public Vector2 ApplyOffset(BetterList<Vector3> verts, Int32 start)
    {
        Vector2 pivotOffset = base.pivotOffset;
        Single num = Mathf.Lerp(0f, (Single)(-(Single)this.mWidth), pivotOffset.x);
        Single num2 = Mathf.Lerp((Single)this.mHeight, 0f, pivotOffset.y) + Mathf.Lerp(this.mCalculatedSize.y - (Single)this.mHeight, 0f, pivotOffset.y);
        num = Mathf.Round(num);
        num2 = Mathf.Round(num2);
        for (Int32 i = start; i < verts.size; i++)
        {
            Vector3[] buffer = verts.buffer;
            Int32 num3 = i;
            buffer[num3].x = buffer[num3].x + num;
            Vector3[] buffer2 = verts.buffer;
            Int32 num4 = i;
            buffer2[num4].y = buffer2[num4].y + num2;
        }
        return new Vector2(num, num2);
    }

    public void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Int32 start, Int32 end, Single x, Single y)
    {
        Color color = this.mEffectColor;
        color.a *= this.finalAlpha;
        Color32 color2 = (!(this.bitmapFont != (UnityEngine.Object)null) || !this.bitmapFont.premultipliedAlphaShader) ? color : NGUITools.ApplyPMA(color);
        for (Int32 i = start; i < end; i++)
        {
            verts.Add(verts.buffer[i]);
            uvs.Add(uvs.buffer[i]);
            cols.Add(cols.buffer[i]);
            Vector3 vector = verts.buffer[i];
            vector.x += x;
            vector.y += y;
            verts.buffer[i] = vector;
            Color32 color3 = cols.buffer[i];
            if (color3.a == 255)
            {
                cols.buffer[i] = color2;
            }
            else
            {
                Color color4 = color;
                color4.a = (Single)color3.a / 255f * color.a;
                cols.buffer[i] = ((!(this.bitmapFont != (UnityEngine.Object)null) || !this.bitmapFont.premultipliedAlphaShader) ? color4 : NGUITools.ApplyPMA(color4));
            }
        }
    }

    public Int32 CalculateOffsetToFit(String text)
    {
        this.UpdateNGUIText();
        NGUIText.encoding = false;
        NGUIText.symbolStyle = NGUIText.SymbolStyle.None;
        Int32 result = NGUIText.CalculateOffsetToFit(text);
        NGUIText.bitmapFont = (UIFont)null;
        NGUIText.dynamicFont = (Font)null;
        return result;
    }

    public void SetCurrentProgress()
    {
        if (UIProgressBar.current != (UnityEngine.Object)null)
        {
            this.text = UIProgressBar.current.value.ToString("F");
        }
    }

    public void SetCurrentPercent()
    {
        if (UIProgressBar.current != (UnityEngine.Object)null)
        {
            this.text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
        }
    }

    public void SetCurrentSelection()
    {
        if (UIPopupList.current != (UnityEngine.Object)null)
        {
            this.text = ((!UIPopupList.current.isLocalized) ? UIPopupList.current.value : Localization.Get(UIPopupList.current.value));
        }
    }

    public Boolean Wrap(String text, out String final)
    {
        return this.Wrap(text, out final, 1000000);
    }

    public Boolean Wrap(String text, out String final, Int32 height)
    {
        this.UpdateNGUIText();
        NGUIText.rectHeight = height;
        NGUIText.regionHeight = height;
        Boolean result = NGUIText.WrapText(text, out final, false);
        NGUIText.bitmapFont = (UIFont)null;
        NGUIText.dynamicFont = (Font)null;
        return result;
    }

    public void UpdateNGUIText()
    {
        Font trueTypeFont = this.trueTypeFont;
        Boolean flag = trueTypeFont != (UnityEngine.Object)null;
        NGUIText.fontSize = this.mFinalFontSize;
        NGUIText.fontStyle = this.mFontStyle;
        NGUIText.rectWidth = this.mWidth;
        NGUIText.rectHeight = this.mHeight;
        NGUIText.regionWidth = Mathf.RoundToInt((Single)this.mWidth * (this.mDrawRegion.z - this.mDrawRegion.x));
        NGUIText.regionHeight = Mathf.RoundToInt((Single)this.mHeight * (this.mDrawRegion.w - this.mDrawRegion.y));
        NGUIText.gradient = (this.mApplyGradient && (this.mFont == (UnityEngine.Object)null || !this.mFont.packedFontShader));
        NGUIText.gradientTop = this.mGradientTop;
        NGUIText.gradientBottom = this.mGradientBottom;
        NGUIText.encoding = this.mEncoding;
        NGUIText.premultiply = this.mPremultiply;
        NGUIText.symbolStyle = this.mSymbols;
        NGUIText.maxLines = this.mMaxLineCount;
        NGUIText.spacingX = this.effectiveSpacingX;
        NGUIText.spacingY = this.effectiveSpacingY;
        NGUIText.fontScale = ((!flag) ? ((Single)this.mFontSize / (Single)this.mFont.defaultSize * this.mScale) : this.mScale);
        if (this.mFont != (UnityEngine.Object)null)
        {
            NGUIText.bitmapFont = this.mFont;
            for (; ; )
            {
                UIFont replacement = NGUIText.bitmapFont.replacement;
                if (replacement == (UnityEngine.Object)null)
                {
                    break;
                }
                NGUIText.bitmapFont = replacement;
            }
            if (NGUIText.bitmapFont.isDynamic)
            {
                NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
                NGUIText.bitmapFont = (UIFont)null;
            }
            else
            {
                NGUIText.dynamicFont = (Font)null;
            }
        }
        else
        {
            NGUIText.dynamicFont = trueTypeFont;
            NGUIText.bitmapFont = (UIFont)null;
        }
        if (flag && this.keepCrisp)
        {
            UIRoot root = base.root;
            if (root != (UnityEngine.Object)null)
            {
                NGUIText.pixelDensity = ((!(root != (UnityEngine.Object)null)) ? 1f : root.pixelSizeAdjustment);
            }
        }
        else
        {
            NGUIText.pixelDensity = 1f;
        }
        if (this.mDensity != NGUIText.pixelDensity)
        {
            this.ProcessText(false, false);
            NGUIText.rectWidth = this.mWidth;
            NGUIText.rectHeight = this.mHeight;
            NGUIText.regionWidth = Mathf.RoundToInt((Single)this.mWidth * (this.mDrawRegion.z - this.mDrawRegion.x));
            NGUIText.regionHeight = Mathf.RoundToInt((Single)this.mHeight * (this.mDrawRegion.w - this.mDrawRegion.y));
        }
        if (this.alignment == NGUIText.Alignment.Automatic)
        {
            UIWidget.Pivot pivot = base.pivot;
            if (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.TopLeft || pivot == UIWidget.Pivot.BottomLeft)
            {
                NGUIText.alignment = NGUIText.Alignment.Left;
            }
            else if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.TopRight || pivot == UIWidget.Pivot.BottomRight)
            {
                NGUIText.alignment = NGUIText.Alignment.Right;
            }
            else
            {
                NGUIText.alignment = NGUIText.Alignment.Center;
            }
        }
        else
        {
            NGUIText.alignment = this.alignment;
        }
        NGUIText.Update();
    }

    private void OnApplicationPause(Boolean paused)
    {
        if (!paused && this.mTrueTypeFont != (UnityEngine.Object)null)
        {
            this.Invalidate(false);
        }
    }

    private BetterList<Dialog.DialogImage> imageList;

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
    private Single mDensity = 1f;

    [NonSerialized]
    private Boolean mShouldBeProcessed = true;

    [NonSerialized]
    private String mProcessedText;

    [NonSerialized]
    private Boolean mPremultiply;

    [NonSerialized]
    private Vector2 mCalculatedSize = Vector2.zero;

    [NonSerialized]
    private Single mScale = 1f;

    [NonSerialized]
    private Int32 mFinalFontSize;

    [NonSerialized]
    private Int32 mLastWidth;

    [NonSerialized]
    private Int32 mLastHeight;

    private static BetterList<UILabel> mList = new BetterList<UILabel>();

    private static Dictionary<Font, Int32> mFontUsage = new Dictionary<Font, Int32>();

    private static Boolean mTexRebuildAdded = false;

    private static BetterList<Vector3> mTempVerts = new BetterList<Vector3>();

    private static BetterList<Int32> mTempIndices = new BetterList<Int32>();

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
