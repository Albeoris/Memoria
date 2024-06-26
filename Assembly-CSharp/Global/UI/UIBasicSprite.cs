using System;
using UnityEngine;

public abstract class UIBasicSprite : UIWidget
{
    public virtual UIBasicSprite.Type type
    {
        get
        {
            return this.mType;
        }
        set
        {
            if (this.mType != value)
            {
                this.mType = value;
                this.MarkAsChanged();
            }
        }
    }

    public UIBasicSprite.Flip flip
    {
        get
        {
            return this.mFlip;
        }
        set
        {
            if (this.mFlip != value)
            {
                this.mFlip = value;
                this.MarkAsChanged();
            }
        }
    }

    public UIBasicSprite.FillDirection fillDirection
    {
        get
        {
            return this.mFillDirection;
        }
        set
        {
            if (this.mFillDirection != value)
            {
                this.mFillDirection = value;
                this.mChanged = true;
            }
        }
    }

    public Single fillAmount
    {
        get
        {
            return this.mFillAmount;
        }
        set
        {
            Single num = Mathf.Clamp01(value);
            if (this.mFillAmount != num)
            {
                this.mFillAmount = num;
                this.mChanged = true;
            }
        }
    }

    public Vector2 birthPosition
    {
        get
        {
            return this.mBirthPos;
        }
        set
        {
            this.mBirthPos = value;
        }
    }

    public override Int32 minWidth
    {
        get
        {
            if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
            {
                Vector4 vector = this.border * this.pixelSize;
                Int32 num = Mathf.RoundToInt(vector.x + vector.z);
                return Mathf.Max(base.minWidth, (Int32)(((num & 1) != 1) ? num : (num + 1)));
            }
            return base.minWidth;
        }
    }

    public override Int32 minHeight
    {
        get
        {
            if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
            {
                Vector4 vector = this.border * this.pixelSize;
                Int32 num = Mathf.RoundToInt(vector.y + vector.w);
                return Mathf.Max(base.minHeight, (Int32)(((num & 1) != 1) ? num : (num + 1)));
            }
            return base.minHeight;
        }
    }

    public Boolean invert
    {
        get
        {
            return this.mInvert;
        }
        set
        {
            if (this.mInvert != value)
            {
                this.mInvert = value;
                this.mChanged = true;
            }
        }
    }

    public Boolean hasBorder
    {
        get
        {
            Vector4 border = this.border;
            return border.x != 0f || border.y != 0f || border.z != 0f || border.w != 0f;
        }
    }

    public virtual Boolean premultipliedAlpha
    {
        get
        {
            return false;
        }
    }

    public virtual Single pixelSize
    {
        get
        {
            return 1f;
        }
    }

    private Vector4 drawingUVs
    {
        get
        {
            switch (this.mFlip)
            {
                case UIBasicSprite.Flip.Horizontally:
                    return new Vector4(this.mOuterUV.xMax, this.mOuterUV.yMin, this.mOuterUV.xMin, this.mOuterUV.yMax);
                case UIBasicSprite.Flip.Vertically:
                    return new Vector4(this.mOuterUV.xMin, this.mOuterUV.yMax, this.mOuterUV.xMax, this.mOuterUV.yMin);
                case UIBasicSprite.Flip.Both:
                    return new Vector4(this.mOuterUV.xMax, this.mOuterUV.yMax, this.mOuterUV.xMin, this.mOuterUV.yMin);
                default:
                    return new Vector4(this.mOuterUV.xMin, this.mOuterUV.yMin, this.mOuterUV.xMax, this.mOuterUV.yMax);
            }
        }
    }

    private Color32 drawingColor
    {
        get
        {
            Color c = base.color;
            c.a = this.finalAlpha;
            if (this.premultipliedAlpha)
            {
                c = NGUITools.ApplyPMA(c);
            }
            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                c.r = Mathf.GammaToLinearSpace(c.r);
                c.g = Mathf.GammaToLinearSpace(c.g);
                c.b = Mathf.GammaToLinearSpace(c.b);
            }
            return c;
        }
    }

    protected void Fill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Rect outer, Rect inner)
    {
        this.mOuterUV = outer;
        this.mInnerUV = inner;
        switch (this.type)
        {
            case UIBasicSprite.Type.Simple:
                this.SimpleFill(verts, uvs, cols);
                break;
            case UIBasicSprite.Type.Sliced:
                this.SlicedFill(verts, uvs, cols);
                break;
            case UIBasicSprite.Type.Tiled:
                this.TiledFill(verts, uvs, cols);
                break;
            case UIBasicSprite.Type.Filled:
                this.FilledFill(verts, uvs, cols);
                break;
            case UIBasicSprite.Type.Advanced:
                this.AdvancedFill(verts, uvs, cols);
                break;
        }
    }

    private void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Vector4 drawingDimensions = this.drawingDimensions;
        Vector4 drawingUVs = this.drawingUVs;
        Color32 drawingColor = this.drawingColor;
        verts.Add(new Vector3(drawingDimensions.x, drawingDimensions.y));
        verts.Add(new Vector3(drawingDimensions.x, drawingDimensions.w));
        verts.Add(new Vector3(drawingDimensions.z, drawingDimensions.w));
        verts.Add(new Vector3(drawingDimensions.z, drawingDimensions.y));
        uvs.Add(new Vector2(drawingUVs.x, drawingUVs.y));
        uvs.Add(new Vector2(drawingUVs.x, drawingUVs.w));
        uvs.Add(new Vector2(drawingUVs.z, drawingUVs.w));
        uvs.Add(new Vector2(drawingUVs.z, drawingUVs.y));
        cols.Add(drawingColor);
        cols.Add(drawingColor);
        cols.Add(drawingColor);
        cols.Add(drawingColor);
    }

    private void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Vector4 vector = this.border * this.pixelSize;
        if (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f)
        {
            this.SimpleFill(verts, uvs, cols);
            return;
        }
        Color32 drawingColor = this.drawingColor;
        Vector4 drawingDimensions = this.drawingDimensions;
        UIBasicSprite.mTempPos[0].x = drawingDimensions.x;
        UIBasicSprite.mTempPos[0].y = drawingDimensions.y;
        UIBasicSprite.mTempPos[3].x = drawingDimensions.z;
        UIBasicSprite.mTempPos[3].y = drawingDimensions.w;
        if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
        {
            UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.z;
            UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.x;
            UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMin;
            UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMin;
            UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMax;
            UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMax;
        }
        else
        {
            UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.x;
            UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.z;
            UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMin;
            UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMin;
            UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMax;
            UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMax;
        }
        if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
        {
            UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.w;
            UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.y;
            UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMin;
            UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMin;
            UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMax;
            UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMax;
        }
        else
        {
            UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.y;
            UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.w;
            UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMin;
            UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMin;
            UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMax;
            UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMax;
        }
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 num = i + 1;
            for (Int32 j = 0; j < 3; j++)
            {
                if (this.centerType != UIBasicSprite.AdvancedType.Invisible || i != 1 || j != 1)
                {
                    Int32 num2 = j + 1;
                    verts.Add(new Vector3(UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[j].y));
                    verts.Add(new Vector3(UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num2].y));
                    verts.Add(new Vector3(UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[num2].y));
                    verts.Add(new Vector3(UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y));
                    uvs.Add(new Vector2(UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[j].y));
                    uvs.Add(new Vector2(UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num2].y));
                    uvs.Add(new Vector2(UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[num2].y));
                    uvs.Add(new Vector2(UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y));
                    cols.Add(drawingColor);
                    cols.Add(drawingColor);
                    cols.Add(drawingColor);
                    cols.Add(drawingColor);
                }
            }
        }
    }

    private void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Texture mainTexture = this.mainTexture;
        if (mainTexture == (UnityEngine.Object)null)
        {
            return;
        }
        Vector2 a = new Vector2(this.mInnerUV.width * (Single)mainTexture.width, this.mInnerUV.height * (Single)mainTexture.height);
        a *= this.pixelSize;
        if (mainTexture == (UnityEngine.Object)null || a.x < 2f || a.y < 2f)
        {
            return;
        }
        Color32 drawingColor = this.drawingColor;
        Vector4 drawingDimensions = this.drawingDimensions;
        Vector4 vector;
        if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
        {
            vector.x = this.mInnerUV.xMax;
            vector.z = this.mInnerUV.xMin;
        }
        else
        {
            vector.x = this.mInnerUV.xMin;
            vector.z = this.mInnerUV.xMax;
        }
        if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
        {
            vector.y = this.mInnerUV.yMax;
            vector.w = this.mInnerUV.yMin;
        }
        else
        {
            vector.y = this.mInnerUV.yMin;
            vector.w = this.mInnerUV.yMax;
        }
        Single num = Mathf.Clamp(drawingDimensions.x + this.mBirthPos.x, drawingDimensions.x, drawingDimensions.z);
        Single num2 = Mathf.Clamp(drawingDimensions.y + this.mBirthPos.y, drawingDimensions.y, drawingDimensions.w);
        Single num3 = num2;
        Single x = vector.x;
        Single y = vector.y;
        Single num4 = num3 + a.y;
        while (num3 < drawingDimensions.w)
        {
            Single num5 = num;
            x = vector.x;
            num4 = num3 + a.y;
            Single num6 = num5 + a.x;
            Single y2 = vector.w;
            if (num4 > drawingDimensions.w)
            {
                y2 = Mathf.Lerp(vector.y, vector.w, (drawingDimensions.w - num3) / a.y);
                num4 = drawingDimensions.w;
            }
            while (num5 < drawingDimensions.z)
            {
                num6 = num5 + a.x;
                Single x2 = vector.z;
                if (num6 > drawingDimensions.z)
                {
                    x2 = Mathf.Lerp(vector.x, vector.z, (drawingDimensions.z - num5) / a.x);
                    num6 = drawingDimensions.z;
                }
                verts.Add(new Vector3(num5, num3));
                verts.Add(new Vector3(num5, num4));
                verts.Add(new Vector3(num6, num4));
                verts.Add(new Vector3(num6, num3));
                uvs.Add(new Vector2(x, y));
                uvs.Add(new Vector2(x, y2));
                uvs.Add(new Vector2(x2, y2));
                uvs.Add(new Vector2(x2, y));
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                num5 += a.x;
            }
            for (num6 = num; num6 > drawingDimensions.x; num6 -= a.x)
            {
                Single z = vector.z;
                num5 = num6 - a.x;
                x = vector.x;
                if (num5 < drawingDimensions.x)
                {
                    x = Mathf.Lerp(vector.z, vector.x, (num6 - drawingDimensions.x) / a.x);
                    num5 = drawingDimensions.x;
                }
                verts.Add(new Vector3(num5, num3));
                verts.Add(new Vector3(num5, num4));
                verts.Add(new Vector3(num6, num4));
                verts.Add(new Vector3(num6, num3));
                uvs.Add(new Vector2(x, y));
                uvs.Add(new Vector2(x, y2));
                uvs.Add(new Vector2(z, y2));
                uvs.Add(new Vector2(z, y));
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
            }
            num3 += a.y;
        }
        for (num4 = num2; num4 > drawingDimensions.y; num4 -= a.y)
        {
            Single num5 = num;
            x = vector.x;
            num3 = num4 - a.y;
            Single num7 = num5 + a.x;
            Single w = vector.w;
            x = Mathf.Lerp(vector.z, vector.x, (num7 - drawingDimensions.x) / a.x);
            if (num3 < drawingDimensions.y)
            {
                y = Mathf.Lerp(vector.w, vector.y, (num4 - drawingDimensions.y) / a.y);
                num3 = drawingDimensions.y;
            }
            while (num5 < drawingDimensions.z)
            {
                num7 = num5 + a.x;
                Single x3 = vector.z;
                if (num7 > drawingDimensions.z)
                {
                    x3 = Mathf.Lerp(vector.x, vector.z, (drawingDimensions.z - num5) / a.x);
                    num7 = drawingDimensions.z;
                }
                verts.Add(new Vector3(num5, num3));
                verts.Add(new Vector3(num5, num4));
                verts.Add(new Vector3(num7, num4));
                verts.Add(new Vector3(num7, num3));
                uvs.Add(new Vector2(x, y));
                uvs.Add(new Vector2(x, w));
                uvs.Add(new Vector2(x3, w));
                uvs.Add(new Vector2(x3, y));
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                num5 += a.x;
            }
            for (num7 = num; num7 > drawingDimensions.x; num7 -= a.x)
            {
                Single z2 = vector.z;
                num5 = num7 - a.x;
                x = vector.x;
                if (num5 < drawingDimensions.x)
                {
                    x = Mathf.Lerp(vector.z, vector.x, (num7 - drawingDimensions.x) / a.x);
                    num5 = drawingDimensions.x;
                }
                verts.Add(new Vector3(num5, num3));
                verts.Add(new Vector3(num5, num4));
                verts.Add(new Vector3(num7, num4));
                verts.Add(new Vector3(num7, num3));
                uvs.Add(new Vector2(x, y));
                uvs.Add(new Vector2(x, w));
                uvs.Add(new Vector2(z2, w));
                uvs.Add(new Vector2(z2, y));
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
                cols.Add(drawingColor);
            }
        }
    }

    private void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        if (this.mFillAmount < 0.001f)
        {
            return;
        }
        Vector4 drawingDimensions = this.drawingDimensions;
        Vector4 drawingUVs = this.drawingUVs;
        Color32 drawingColor = this.drawingColor;
        if (this.mFillDirection == UIBasicSprite.FillDirection.Horizontal || this.mFillDirection == UIBasicSprite.FillDirection.Vertical)
        {
            if (this.mFillDirection == UIBasicSprite.FillDirection.Horizontal)
            {
                Single num = (drawingUVs.z - drawingUVs.x) * this.mFillAmount;
                if (this.mInvert)
                {
                    drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * this.mFillAmount;
                    drawingUVs.x = drawingUVs.z - num;
                }
                else
                {
                    drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * this.mFillAmount;
                    drawingUVs.z = drawingUVs.x + num;
                }
            }
            else if (this.mFillDirection == UIBasicSprite.FillDirection.Vertical)
            {
                Single num2 = (drawingUVs.w - drawingUVs.y) * this.mFillAmount;
                if (this.mInvert)
                {
                    drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * this.mFillAmount;
                    drawingUVs.y = drawingUVs.w - num2;
                }
                else
                {
                    drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * this.mFillAmount;
                    drawingUVs.w = drawingUVs.y + num2;
                }
            }
        }
        UIBasicSprite.mTempPos[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
        UIBasicSprite.mTempPos[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
        UIBasicSprite.mTempPos[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
        UIBasicSprite.mTempPos[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
        UIBasicSprite.mTempUVs[0] = new Vector2(drawingUVs.x, drawingUVs.y);
        UIBasicSprite.mTempUVs[1] = new Vector2(drawingUVs.x, drawingUVs.w);
        UIBasicSprite.mTempUVs[2] = new Vector2(drawingUVs.z, drawingUVs.w);
        UIBasicSprite.mTempUVs[3] = new Vector2(drawingUVs.z, drawingUVs.y);
        if (this.mFillAmount < 1f)
        {
            if (this.mFillDirection == UIBasicSprite.FillDirection.Radial90)
            {
                if (UIBasicSprite.RadialCut(UIBasicSprite.mTempPos, UIBasicSprite.mTempUVs, this.mFillAmount, this.mInvert, 0))
                {
                    for (Int32 i = 0; i < 4; i++)
                    {
                        verts.Add(UIBasicSprite.mTempPos[i]);
                        uvs.Add(UIBasicSprite.mTempUVs[i]);
                        cols.Add(drawingColor);
                    }
                }
                return;
            }
            if (this.mFillDirection == UIBasicSprite.FillDirection.Radial180)
            {
                for (Int32 j = 0; j < 2; j++)
                {
                    Single t = 0f;
                    Single t2 = 1f;
                    Single t3;
                    Single t4;
                    if (j == 0)
                    {
                        t3 = 0f;
                        t4 = 0.5f;
                    }
                    else
                    {
                        t3 = 0.5f;
                        t4 = 1f;
                    }
                    UIBasicSprite.mTempPos[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t3);
                    UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x;
                    UIBasicSprite.mTempPos[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t4);
                    UIBasicSprite.mTempPos[3].x = UIBasicSprite.mTempPos[2].x;
                    UIBasicSprite.mTempPos[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t);
                    UIBasicSprite.mTempPos[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t2);
                    UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[1].y;
                    UIBasicSprite.mTempPos[3].y = UIBasicSprite.mTempPos[0].y;
                    UIBasicSprite.mTempUVs[0].x = Mathf.Lerp(drawingUVs.x, drawingUVs.z, t3);
                    UIBasicSprite.mTempUVs[1].x = UIBasicSprite.mTempUVs[0].x;
                    UIBasicSprite.mTempUVs[2].x = Mathf.Lerp(drawingUVs.x, drawingUVs.z, t4);
                    UIBasicSprite.mTempUVs[3].x = UIBasicSprite.mTempUVs[2].x;
                    UIBasicSprite.mTempUVs[0].y = Mathf.Lerp(drawingUVs.y, drawingUVs.w, t);
                    UIBasicSprite.mTempUVs[1].y = Mathf.Lerp(drawingUVs.y, drawingUVs.w, t2);
                    UIBasicSprite.mTempUVs[2].y = UIBasicSprite.mTempUVs[1].y;
                    UIBasicSprite.mTempUVs[3].y = UIBasicSprite.mTempUVs[0].y;
                    Single value = this.mInvert ? (this.mFillAmount * 2f - (Single)(1 - j)) : (this.fillAmount * 2f - (Single)j);
                    if (UIBasicSprite.RadialCut(UIBasicSprite.mTempPos, UIBasicSprite.mTempUVs, Mathf.Clamp01(value), !this.mInvert, NGUIMath.RepeatIndex(j + 3, 4)))
                    {
                        for (Int32 k = 0; k < 4; k++)
                        {
                            verts.Add(UIBasicSprite.mTempPos[k]);
                            uvs.Add(UIBasicSprite.mTempUVs[k]);
                            cols.Add(drawingColor);
                        }
                    }
                }
                return;
            }
            if (this.mFillDirection == UIBasicSprite.FillDirection.Radial360)
            {
                for (Int32 l = 0; l < 4; l++)
                {
                    Single t5;
                    Single t6;
                    if (l < 2)
                    {
                        t5 = 0f;
                        t6 = 0.5f;
                    }
                    else
                    {
                        t5 = 0.5f;
                        t6 = 1f;
                    }
                    Single t7;
                    Single t8;
                    if (l == 0 || l == 3)
                    {
                        t7 = 0f;
                        t8 = 0.5f;
                    }
                    else
                    {
                        t7 = 0.5f;
                        t8 = 1f;
                    }
                    UIBasicSprite.mTempPos[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t5);
                    UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x;
                    UIBasicSprite.mTempPos[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t6);
                    UIBasicSprite.mTempPos[3].x = UIBasicSprite.mTempPos[2].x;
                    UIBasicSprite.mTempPos[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t7);
                    UIBasicSprite.mTempPos[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t8);
                    UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[1].y;
                    UIBasicSprite.mTempPos[3].y = UIBasicSprite.mTempPos[0].y;
                    UIBasicSprite.mTempUVs[0].x = Mathf.Lerp(drawingUVs.x, drawingUVs.z, t5);
                    UIBasicSprite.mTempUVs[1].x = UIBasicSprite.mTempUVs[0].x;
                    UIBasicSprite.mTempUVs[2].x = Mathf.Lerp(drawingUVs.x, drawingUVs.z, t6);
                    UIBasicSprite.mTempUVs[3].x = UIBasicSprite.mTempUVs[2].x;
                    UIBasicSprite.mTempUVs[0].y = Mathf.Lerp(drawingUVs.y, drawingUVs.w, t7);
                    UIBasicSprite.mTempUVs[1].y = Mathf.Lerp(drawingUVs.y, drawingUVs.w, t8);
                    UIBasicSprite.mTempUVs[2].y = UIBasicSprite.mTempUVs[1].y;
                    UIBasicSprite.mTempUVs[3].y = UIBasicSprite.mTempUVs[0].y;
                    Single value2 = (!this.mInvert) ? (this.mFillAmount * 4f - (Single)(3 - NGUIMath.RepeatIndex(l + 2, 4))) : (this.mFillAmount * 4f - (Single)NGUIMath.RepeatIndex(l + 2, 4));
                    if (UIBasicSprite.RadialCut(UIBasicSprite.mTempPos, UIBasicSprite.mTempUVs, Mathf.Clamp01(value2), this.mInvert, NGUIMath.RepeatIndex(l + 2, 4)))
                    {
                        for (Int32 m = 0; m < 4; m++)
                        {
                            verts.Add(UIBasicSprite.mTempPos[m]);
                            uvs.Add(UIBasicSprite.mTempUVs[m]);
                            cols.Add(drawingColor);
                        }
                    }
                }
                return;
            }
        }
        for (Int32 n = 0; n < 4; n++)
        {
            verts.Add(UIBasicSprite.mTempPos[n]);
            uvs.Add(UIBasicSprite.mTempUVs[n]);
            cols.Add(drawingColor);
        }
    }

    private void AdvancedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        Texture mainTexture = this.mainTexture;
        if (mainTexture == (UnityEngine.Object)null)
        {
            return;
        }
        Vector4 vector = this.border * this.pixelSize;
        if (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f)
        {
            this.SimpleFill(verts, uvs, cols);
            return;
        }
        Color32 drawingColor = this.drawingColor;
        Vector4 drawingDimensions = this.drawingDimensions;
        Vector2 a = new Vector2(this.mInnerUV.width * (Single)mainTexture.width, this.mInnerUV.height * (Single)mainTexture.height);
        a *= this.pixelSize;
        if (a.x < 1f)
        {
            a.x = 1f;
        }
        if (a.y < 1f)
        {
            a.y = 1f;
        }
        UIBasicSprite.mTempPos[0].x = drawingDimensions.x;
        UIBasicSprite.mTempPos[0].y = drawingDimensions.y;
        UIBasicSprite.mTempPos[3].x = drawingDimensions.z;
        UIBasicSprite.mTempPos[3].y = drawingDimensions.w;
        if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
        {
            UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.z;
            UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.x;
            UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMin;
            UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMin;
            UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMax;
            UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMax;
        }
        else
        {
            UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.x;
            UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.z;
            UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMin;
            UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMin;
            UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMax;
            UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMax;
        }
        if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
        {
            UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.w;
            UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.y;
            UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMin;
            UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMin;
            UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMax;
            UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMax;
        }
        else
        {
            UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.y;
            UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.w;
            UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMin;
            UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMin;
            UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMax;
            UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMax;
        }
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 num = i + 1;
            for (Int32 j = 0; j < 3; j++)
            {
                if (this.centerType != UIBasicSprite.AdvancedType.Invisible || i != 1 || j != 1)
                {
                    Int32 num2 = j + 1;
                    if (i == 1 && j == 1)
                    {
                        if (this.centerType == UIBasicSprite.AdvancedType.Tiled)
                        {
                            Single x = UIBasicSprite.mTempPos[i].x;
                            Single x2 = UIBasicSprite.mTempPos[num].x;
                            Single y = UIBasicSprite.mTempPos[j].y;
                            Single y2 = UIBasicSprite.mTempPos[num2].y;
                            Single x3 = UIBasicSprite.mTempUVs[i].x;
                            Single y3 = UIBasicSprite.mTempUVs[j].y;
                            for (Single num3 = y; num3 < y2; num3 += a.y)
                            {
                                Single num4 = x;
                                Single num5 = UIBasicSprite.mTempUVs[num2].y;
                                Single num6 = num3 + a.y;
                                if (num6 > y2)
                                {
                                    num5 = Mathf.Lerp(y3, num5, (y2 - num3) / a.y);
                                    num6 = y2;
                                }
                                while (num4 < x2)
                                {
                                    Single num7 = num4 + a.x;
                                    Single num8 = UIBasicSprite.mTempUVs[num].x;
                                    if (num7 > x2)
                                    {
                                        num8 = Mathf.Lerp(x3, num8, (x2 - num4) / a.x);
                                        num7 = x2;
                                    }
                                    UIBasicSprite.Fill(verts, uvs, cols, num4, num7, num3, num6, x3, num8, y3, num5, drawingColor);
                                    num4 += a.x;
                                }
                            }
                        }
                        else if (this.centerType == UIBasicSprite.AdvancedType.Sliced)
                        {
                            UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, drawingColor);
                        }
                    }
                    else if (i == 1)
                    {
                        if ((j == 0 && this.bottomType == UIBasicSprite.AdvancedType.Tiled) || (j == 2 && this.topType == UIBasicSprite.AdvancedType.Tiled))
                        {
                            Single x4 = UIBasicSprite.mTempPos[i].x;
                            Single x5 = UIBasicSprite.mTempPos[num].x;
                            Single y4 = UIBasicSprite.mTempPos[j].y;
                            Single y5 = UIBasicSprite.mTempPos[num2].y;
                            Single x6 = UIBasicSprite.mTempUVs[i].x;
                            Single y6 = UIBasicSprite.mTempUVs[j].y;
                            Single y7 = UIBasicSprite.mTempUVs[num2].y;
                            for (Single num9 = x4; num9 < x5; num9 += a.x)
                            {
                                Single num10 = num9 + a.x;
                                Single num11 = UIBasicSprite.mTempUVs[num].x;
                                if (num10 > x5)
                                {
                                    num11 = Mathf.Lerp(x6, num11, (x5 - num9) / a.x);
                                    num10 = x5;
                                }
                                UIBasicSprite.Fill(verts, uvs, cols, num9, num10, y4, y5, x6, num11, y6, y7, drawingColor);
                            }
                        }
                        else if ((j == 0 && this.bottomType != UIBasicSprite.AdvancedType.Invisible) || (j == 2 && this.topType != UIBasicSprite.AdvancedType.Invisible))
                        {
                            UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, drawingColor);
                        }
                    }
                    else if (j == 1)
                    {
                        if ((i == 0 && this.leftType == UIBasicSprite.AdvancedType.Tiled) || (i == 2 && this.rightType == UIBasicSprite.AdvancedType.Tiled))
                        {
                            Single x7 = UIBasicSprite.mTempPos[i].x;
                            Single x8 = UIBasicSprite.mTempPos[num].x;
                            Single y8 = UIBasicSprite.mTempPos[j].y;
                            Single y9 = UIBasicSprite.mTempPos[num2].y;
                            Single x9 = UIBasicSprite.mTempUVs[i].x;
                            Single x10 = UIBasicSprite.mTempUVs[num].x;
                            Single y10 = UIBasicSprite.mTempUVs[j].y;
                            for (Single num12 = y8; num12 < y9; num12 += a.y)
                            {
                                Single num13 = UIBasicSprite.mTempUVs[num2].y;
                                Single num14 = num12 + a.y;
                                if (num14 > y9)
                                {
                                    num13 = Mathf.Lerp(y10, num13, (y9 - num12) / a.y);
                                    num14 = y9;
                                }
                                UIBasicSprite.Fill(verts, uvs, cols, x7, x8, num12, num14, x9, x10, y10, num13, drawingColor);
                            }
                        }
                        else if ((i == 0 && this.leftType != UIBasicSprite.AdvancedType.Invisible) || (i == 2 && this.rightType != UIBasicSprite.AdvancedType.Invisible))
                        {
                            UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, drawingColor);
                        }
                    }
                    else if ((j == 0 && this.bottomType != UIBasicSprite.AdvancedType.Invisible) || (j == 2 && this.topType != UIBasicSprite.AdvancedType.Invisible) || (i == 0 && this.leftType != UIBasicSprite.AdvancedType.Invisible) || (i == 2 && this.rightType != UIBasicSprite.AdvancedType.Invisible))
                    {
                        UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, drawingColor);
                    }
                }
            }
        }
    }

    private static Boolean RadialCut(Vector2[] xy, Vector2[] uv, Single fill, Boolean invert, Int32 corner)
    {
        if (fill < 0.001f)
        {
            return false;
        }
        if ((corner & 1) == 1)
        {
            invert = !invert;
        }
        if (!invert && fill > 0.999f)
        {
            return true;
        }
        Single num = Mathf.Clamp01(fill);
        if (invert)
        {
            num = 1f - num;
        }
        num *= 1.57079637f;
        Single cos = Mathf.Cos(num);
        Single sin = Mathf.Sin(num);
        UIBasicSprite.RadialCut(xy, cos, sin, invert, corner);
        UIBasicSprite.RadialCut(uv, cos, sin, invert, corner);
        return true;
    }

    private static void RadialCut(Vector2[] xy, Single cos, Single sin, Boolean invert, Int32 corner)
    {
        Int32 num = NGUIMath.RepeatIndex(corner + 1, 4);
        Int32 num2 = NGUIMath.RepeatIndex(corner + 2, 4);
        Int32 num3 = NGUIMath.RepeatIndex(corner + 3, 4);
        if ((corner & 1) == 1)
        {
            if (sin > cos)
            {
                cos /= sin;
                sin = 1f;
                if (invert)
                {
                    xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
                    xy[num2].x = xy[num].x;
                }
            }
            else if (cos > sin)
            {
                sin /= cos;
                cos = 1f;
                if (!invert)
                {
                    xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
                    xy[num3].y = xy[num2].y;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }
            if (!invert)
            {
                xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
            }
            else
            {
                xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
            }
        }
        else
        {
            if (cos > sin)
            {
                sin /= cos;
                cos = 1f;
                if (!invert)
                {
                    xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
                    xy[num2].y = xy[num].y;
                }
            }
            else if (sin > cos)
            {
                cos /= sin;
                sin = 1f;
                if (invert)
                {
                    xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
                    xy[num3].x = xy[num2].x;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }
            if (invert)
            {
                xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
            }
            else
            {
                xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
            }
        }
    }

    private static void Fill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Single v0x, Single v1x, Single v0y, Single v1y, Single u0x, Single u1x, Single u0y, Single u1y, Color col)
    {
        verts.Add(new Vector3(v0x, v0y));
        verts.Add(new Vector3(v0x, v1y));
        verts.Add(new Vector3(v1x, v1y));
        verts.Add(new Vector3(v1x, v0y));
        uvs.Add(new Vector2(u0x, u0y));
        uvs.Add(new Vector2(u0x, u1y));
        uvs.Add(new Vector2(u1x, u1y));
        uvs.Add(new Vector2(u1x, u0y));
        cols.Add(col);
        cols.Add(col);
        cols.Add(col);
        cols.Add(col);
    }

    [SerializeField]
    [HideInInspector]
    protected UIBasicSprite.Type mType;

    [SerializeField]
    [HideInInspector]
    protected UIBasicSprite.FillDirection mFillDirection = UIBasicSprite.FillDirection.Radial360;

    [Range(0f, 1f)]
    [SerializeField]
    [HideInInspector]
    protected Single mFillAmount = 1f;

    [SerializeField]
    [HideInInspector]
    protected Boolean mInvert;

    [SerializeField]
    [HideInInspector]
    protected UIBasicSprite.Flip mFlip;

    [HideInInspector]
    [SerializeField]
    protected Vector2 mBirthPos = new Vector2(0f, 0f);

    [NonSerialized]
    private Rect mInnerUV = default(Rect);

    [NonSerialized]
    private Rect mOuterUV = default(Rect);

    public UIBasicSprite.AdvancedType centerType = UIBasicSprite.AdvancedType.Sliced;

    public UIBasicSprite.AdvancedType leftType = UIBasicSprite.AdvancedType.Sliced;

    public UIBasicSprite.AdvancedType rightType = UIBasicSprite.AdvancedType.Sliced;

    public UIBasicSprite.AdvancedType bottomType = UIBasicSprite.AdvancedType.Sliced;

    public UIBasicSprite.AdvancedType topType = UIBasicSprite.AdvancedType.Sliced;

    protected static Vector2[] mTempPos = new Vector2[4];

    protected static Vector2[] mTempUVs = new Vector2[4];

    public enum Type
    {
        Simple,
        Sliced,
        Tiled,
        Filled,
        Advanced
    }

    public enum FillDirection
    {
        Horizontal,
        Vertical,
        Radial90,
        Radial180,
        Radial360
    }

    public enum AdvancedType
    {
        Invisible,
        Sliced,
        Tiled
    }

    public enum Flip
    {
        Nothing,
        Horizontally,
        Vertically,
        Both
    }
}
