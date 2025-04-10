using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    public class FFIXTextModifier
    {
        public BetterList<Color> colors = new BetterList<Color>();
        public List<Background> backgroundColors = new List<Background>();
        public Int32 sub;
        public Boolean bold;
        public Boolean italic;
        public Boolean underline;
        public Boolean strike;
        public Boolean ignoreColor;
        public Boolean highShadow;
        public Boolean center;
        public Boolean justified;
        public Boolean mirror;
        public Boolean choice;
        public Vector2 extraOffset;
        public Vector2 frameOffset;
        public Single? tabX;
        public DialogImage insertImage;
        public Single appearanceSpeed;

        public void Reset()
        {
            colors.Clear();
            backgroundColors.Clear();
            sub = 0;
            bold = false;
            italic = false;
            underline = false;
            strike = false;
            ignoreColor = false;
            highShadow = false;
            center = false;
            justified = false;
            mirror = false;
            choice = false;
            extraOffset = Vector2.zero;
            frameOffset = Vector2.zero;
            tabX = null;
            insertImage = null;
            SetAppearanceSpeed(-1f);
        }

        public void ResetLine()
        {
            center = false;
            extraOffset = Vector2.zero;
        }

        public void SetAppearanceSpeed(Single speed)
        {
            Single baseRatio = speed > 0f ? speed : (Configuration.VoiceActing.ForceMessageSpeed < 0 ? Dialog.DialogTextAnimationTick[FF9StateSystem.Settings.cfg.fld_msg] : Dialog.DialogTextAnimationTick[Configuration.VoiceActing.ForceMessageSpeed]);
            appearanceSpeed = 30f / (Configuration.Graphics.FieldTPS * Dialog.FF9TextSpeedRatio * baseRatio);
        }

        public void UpdateSettingsAfterTag(ref Single currentX, ref Single currentY, ref Boolean afterImage, ref BetterList<DialogImage> specialImages, ref Color32 textColor, ref Color gradientColorBottom, ref Color gradientColorTop, NGUIText.Alignment defaultAlignment, Int32 printedLine, Single typicalCharacterHeight, LinkedListNode<TextDirectionBlock> bidiBlock)
        {
            Boolean invertXOffset = bidiBlock != null ? !bidiBlock.Value.ltr : false;
            if (extraOffset != Vector2.zero)
            {
                currentX += invertXOffset ? -extraOffset.x : extraOffset.x;
                currentY += extraOffset.y;
                extraOffset = Vector2.zero;
                afterImage = false;
            }
            else if (insertImage != null)
            {
                Single frameOffsetX = NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft ? -frameOffset.x : frameOffset.x;
                Int32 recycleImgIndex = -1;
                for (Int32 i = 0; i < specialImages.size; i++)
                {
                    if (!specialImages[i].IsRegistered && DialogImage.CompareImages(specialImages[i], insertImage))
                    {
                        // This dialog image was potentially already generated (typically from a previous NGUIText.GenerateTextRender call): recycle its gameobject
                        insertImage.SpriteGo = specialImages[i].SpriteGo;
                        recycleImgIndex = i;
                        break;
                    }
                }
                if (invertXOffset)
                    currentX -= insertImage.Size.x;
                insertImage.LocalPosition = new Vector3(frameOffsetX + currentX, currentY);
                insertImage.PrintedLine = printedLine;
                insertImage.AppearStep = NGUIText.progressStep;
                insertImage.Mirror = mirror;
                if (!invertXOffset)
                    currentX += insertImage.Size.x;
                if (NGUIText.ShouldAlignImageVertically(insertImage))
                    insertImage.LocalPosition.y += typicalCharacterHeight + insertImage.Offset.y - insertImage.Size.y;
                insertImage.IsRegistered = true;
                if (recycleImgIndex < 0)
                    specialImages.Add(insertImage);
                else
                    specialImages[recycleImgIndex] = insertImage;
                if (bidiBlock != null)
                    bidiBlock.Value.images.Add(insertImage);
                insertImage = null;
                afterImage = true;
            }
            else if (tabX.HasValue)
            {
                currentX = invertXOffset ? -tabX.Value : tabX.Value;
                tabX = null;
                afterImage = false;
            }
            Color colorFromOpcode;
            if (ignoreColor)
            {
                colorFromOpcode = colors[colors.size - 1];
                colorFromOpcode.a *= NGUIText.mAlpha * NGUIText.tint.a;
            }
            else
            {
                colorFromOpcode = NGUIText.tint * colors[colors.size - 1];
                colorFromOpcode.a *= NGUIText.mAlpha;
            }
            textColor = colorFromOpcode;
            for (Int32 i = 0; i < colors.size - 2; i++)
                colorFromOpcode.a *= colors[i].a;
            if (NGUIText.gradient)
            {
                gradientColorBottom = NGUIText.gradientBottom * colorFromOpcode;
                gradientColorTop = NGUIText.gradientTop * colorFromOpcode;
            }
            NGUIText.alignment = justified ? NGUIText.Alignment.Justified : (center ? NGUIText.Alignment.Center : defaultAlignment);
        }

        public struct Background
        {
            public Color color;
            public Rect relativeOffset;
            public Rect pixelOffset;

            public Background(Color col)
            {
                color = col;
                relativeOffset = new Rect(0f, 0f, 1f, 1f);
                pixelOffset = new Rect(0f, 0f, 0f, 0f);
            }

            public Background(Color col, Rect relRect, Rect absRect)
            {
                color = col;
                relativeOffset = relRect;
                pixelOffset = absRect;
            }

            public Rect ApplyRect(Single x, Single y, Single w, Single h)
            {
                Rect rect = new Rect();
                rect.xMin = x + relativeOffset.xMin * w + pixelOffset.xMin;
                rect.yMin = -y - relativeOffset.yMin * h - pixelOffset.yMin;
                rect.xMax = x + relativeOffset.xMax * w + pixelOffset.xMax;
                rect.yMax = -y - relativeOffset.yMax * h - pixelOffset.yMax;
                return rect;
            }
        }
    }
}
