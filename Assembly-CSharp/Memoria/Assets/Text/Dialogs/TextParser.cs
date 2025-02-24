using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    public class TextParser
    {
        // Properly initialised when Step >= Page
        public readonly UILabel LabelContainer;
        public readonly String InitialText;
        public ParseStep Step = ParseStep.Page;
        public String ParsedText = String.Empty;

        // Properly initialised when Step >= ConstantReplaceTags
        public List<FFIXTextTag> ParsedTagList = new List<FFIXTextTag>();

        // Properly initialised when Step >= ChoiceSetup
        public List<FFIXTextTag> VariableTagList = new List<FFIXTextTag>();
        public List<Int32> VariableTagListOffset = new List<Int32>();
        public String VariableText = String.Empty;

        // Properly initialised when Step >= VariableReplaceTags
        public Dictionary<Int32, Int32> VariableMessageValues = new Dictionary<Int32, Int32>();

        // Properly initialised when Step >= Bidi
        public UnicodeBIDI Bidi = null;

        // Properly initialised when Step >= Wrapped
        public List<Line> LineInfo = new List<Line>();

        // Properly initialised when Step >= Render
        public BetterList<Vector3> Vertices = new BetterList<Vector3>();
        public BetterList<Vector2> UVs = new BetterList<Vector2>();
        public BetterList<Color32> Colors = new BetterList<Color32>();
        public BetterList<Single> VertexAppearStep = new BetterList<Single>();
        public BetterList<Byte> FinalAlpha = new BetterList<Byte>();
        public BetterList<DialogImage> SpecialImages = new BetterList<DialogImage>();
        public Single AppearProgress = 0f;
        public Single AppearProgressMax = 0f;

        public Boolean VariableMessageNeedUpdate => VariableMessageValues.Count > 0;
        public Single MaxWidth => LineInfo.Count > 0 ? LineInfo.Max(line => line.Width) : 0f;
        public Single RawHeight => LineInfo.Count > 0 ? LineInfo.Max(line => line.BaseY + line.BaseHeight) : 0f;
        public Single DialogLineCount => Mathf.CeilToInt(RawHeight / Dialog.DialogLineHeight);
        public Vector2 FullSize => new Vector2(MaxWidth, RawHeight);
        public Rect RenderRect => NGUIMath.GetBoundingBox(Vertices);

        /// <summary>Create an instance from pages returned by DialogBoxSymbols.ParseTextSplitTags, or from text with no page tag</summary>
        public TextParser(UILabel container, String text)
        {
            LabelContainer = container;
            InitialText = text;
            ParsedText = text;
        }

        public void Parse(ParseStep target)
        {
            try
            {
                if (Step == ParseStep.Page && target > ParseStep.Page)
                {
                    DialogBoxSymbols.ParseInitialAndConstantTextTags(this);
                    Step++;
                }
                if (Step == ParseStep.ConstantReplaceTags && target > ParseStep.ConstantReplaceTags)
                {
                    DialogBoxSymbols.ParseChoiceTags(this);
                    Step++;
                    VariableTagList = new List<FFIXTextTag>(ParsedTagList);
                    VariableText = ParsedText;
                }
                if (Step == ParseStep.ChoiceSetup && target > ParseStep.ChoiceSetup)
                {
                    DialogBoxSymbols.ParseVariableTextReplaceTags(this);
                    Step++;
                }
                if (Step == ParseStep.VariableReplaceTags && target > ParseStep.VariableReplaceTags)
                {
                    if (ShouldUseBIDI)
                    {
                        Bidi = new UnicodeBIDI(ParsedText.ToCharArray(), NGUIText.readingDirection);
                        ParsedText = new String(Bidi.FullText.ToArray());
                    }
                    Step++;
                }
                if (Step == ParseStep.BIDI && target > ParseStep.BIDI)
                {
                    LabelContainer.GenerateWrapping();
                    Step++;
                }
                if (Step == ParseStep.Wrapped && target > ParseStep.Wrapped)
                {
                    LabelContainer.GenerateTextRender();
                    Step++;
                }
            }
            catch (Exception err)
            {
                Memoria.Prime.Log.Error($"[DBG] parse to {target}, fail at {Step}: {err}");
            }
        }

        public void ResetRender()
        {
            if (Step <= ParseStep.Wrapped)
                return;
            Vertices.Clear();
            UVs.Clear();
            Colors.Clear();
            VertexAppearStep.Clear();
            FinalAlpha.Clear();
            foreach (Line line in LineInfo)
            {
                line.FirstVertexIndex = 0;
                line.EndVertexIndex = 0;
            }
            LabelContainer.HideAllIcons(SpecialImages);
            Step = ParseStep.Wrapped;
        }

        public void ResetBeforeVariableTags()
        {
            if (Step <= ParseStep.ChoiceSetup)
                return;
            ParsedText = VariableText;
            ParsedTagList = new List<FFIXTextTag>(VariableTagList);
            VariableMessageValues.Clear();
            Bidi = null;
            LineInfo.Clear();
            Vertices.Clear();
            UVs.Clear();
            Colors.Clear();
            VertexAppearStep.Clear();
            FinalAlpha.Clear();
            LabelContainer.HideAllIcons(SpecialImages);
            Step = ParseStep.ChoiceSetup;
        }

        public void ResetCompletly()
        {
            if (Step == ParseStep.Page)
                return;
            ResetBeforeVariableTags();
            VariableTagList.Clear();
            VariableTagListOffset.Clear();
            VariableText = String.Empty;
            ParsedText = InitialText;
            ParsedTagList.Clear();
            LabelContainer.ReleaseAllIcons();
            Step = ParseStep.Page;
        }

        public void ComputeAppearProgressMax()
        {
            Single vertProgressMax = VertexAppearStep.size > 0 ? VertexAppearStep.Max() : 0f;
            Single imgProgressMax = SpecialImages.size > 0 ? SpecialImages.Max(img => img.AppearStep) : 0f;
            Single tagProgressMax = ParsedTagList.Count > 0 ? ParsedTagList.Max(tag => tag.AppearStep) : 0f;
            AppearProgressMax = Math.Max(vertProgressMax, imgProgressMax);
            AppearProgressMax = Math.Max(AppearProgressMax, tagProgressMax);
        }

        public void RemovePart(Int32 pos, Int32 length)
        {
            if (pos < 0 || pos >= ParsedText.Length || length <= 0)
                return;
            ParsedText = ParsedText.Remove(pos, length);
            for (Int32 i = 0; i < ParsedTagList.Count; i++)
            {
                if (ParsedTagList[i].TextOffset >= pos)
                {
                    if (ParsedTagList[i].TextOffset < pos + length)
                    {
                        ParsedTagList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        ParsedTagList[i].TextOffset -= length;
                    }
                }
            }
            if (Bidi != null)
                Bidi.RemovePart(pos, length);
        }

        public void InsertTag(String tagString, Int32 pos = -1)
        {
            Int32 offset = 0;
            FFIXTextTag tag = FFIXTextTag.TryRead(tagString, ref offset);
            if (tag != null)
                InsertTag(tag, pos);
        }

        public void InsertTag(FFIXTextTag tag, Int32 pos = -1)
        {
            if (pos >= ParsedText.Length)
            {
                tag.TextOffset = ParsedText.Length;
                ParsedTagList.Add(tag);
                return;
            }
            for (Int32 i = 0; i < ParsedTagList.Count; i++)
            {
                if (ParsedTagList[i].TextOffset > pos)
                {
                    tag.TextOffset = Math.Max(pos, 0);
                    ParsedTagList.Insert(i, tag);
                    return;
                }
            }
            tag.TextOffset = Math.Max(pos, 0);
            ParsedTagList.Add(tag);
        }

        public void ReplaceTag(Int32 tagIndex, String replaceStr)
        {
            if (tagIndex < 0 || tagIndex >= ParsedTagList.Count)
                return;
            Int32 insertPos = ParsedTagList[tagIndex].TextOffset;
            ParsedText = ParsedText.Insert(insertPos, replaceStr);
            ParsedTagList.RemoveAt(tagIndex);
            for (Int32 i = tagIndex; i < ParsedTagList.Count; i++)
                ParsedTagList[i].TextOffset += replaceStr.Length;
        }

        public Boolean AdvanceProgress(Single progress)
        {
            if (AppearProgress >= AppearProgressMax)
                return true;
            Single nextStep = AppearProgress + progress;
            for (Int32 i = 0; i < ParsedTagList.Count; i++)
                if (ParsedTagList[i].AppearStep >= AppearProgress && ParsedTagList[i].AppearStep <= nextStep)
                    DialogBoxSymbols.OnAppearTag(ParsedTagList[i], LabelContainer.DialogWindow, LabelContainer);
            AppearProgress = nextStep;
            return AppearProgress >= AppearProgressMax;
        }

        public void ResetProgress()
        {
            AppearProgress = 0f;
            for (Int32 i = 0; i < Colors.size; i++)
                Colors.buffer[i].a = 0;
            for (Int32 i = 0; i < SpecialImages.size; i++)
                LabelContainer.HideIcon(SpecialImages[i]);
        }

        public void ApplyRenderProgress()
        {
            for (Int32 i = 0; i < VertexAppearStep.size; i++)
            {
                if (VertexAppearStep[i] <= AppearProgress)
                    Colors.buffer[i].a = FinalAlpha[i];
                else if (VertexAppearStep[i] > AppearProgress && VertexAppearStep[i] < AppearProgress + 1f)
                    Colors.buffer[i].a = (Byte)Math.Round((AppearProgress + 1f - VertexAppearStep[i]) * FinalAlpha[i]);
            }
            for (Int32 i = 0; i < SpecialImages.size; i++)
            {
                if (SpecialImages[i].AppearStep <= AppearProgress)
                    LabelContainer.PrintIcon(SpecialImages[i]);
                else
                    LabelContainer.HideIcon(SpecialImages[i]);
            }
        }

        public void AddVertex(Vector3 pos, Vector2 uv, Color32 col, Single appearStep)
        {
            Vertices.Add(pos);
            UVs.Add(uv);
            Colors.Add(col);
            VertexAppearStep.Add(appearStep);
            FinalAlpha.Add(col.a);
        }

        public void AddSegment(Vector3 pos1, Vector3 pos2, Color32 col1, Color32 col2, Single appearStep)
        {
            const Single LINE_WIDTH = 1f;
            NGUIText.GlyphInfo lineGlyph = NGUIText.GetGlyph('-', 0);
            if (NGUIText.bitmapFont != null)
            {
                Rect bmUvRect = NGUIText.bitmapFont.uvRect;
                Single bmTextureFactorX = bmUvRect.width / NGUIText.bitmapFont.texWidth;
                Single bmTextureFactorY = bmUvRect.height / NGUIText.bitmapFont.texHeight;
                lineGlyph.u0.x = bmUvRect.xMin + bmTextureFactorX * lineGlyph.u0.x;
                lineGlyph.u2.x = bmUvRect.xMin + bmTextureFactorX * lineGlyph.u2.x;
                lineGlyph.u0.y = bmUvRect.yMax - bmTextureFactorY * lineGlyph.u0.y;
                lineGlyph.u2.y = bmUvRect.yMax - bmTextureFactorY * lineGlyph.u2.y;
            }
            Vector2 uvPoint = Vector2.Lerp(lineGlyph.u0, lineGlyph.u2, 0.5f);
            Vector3 lineOff = LINE_WIDTH * new Vector3(pos1.y - pos2.y, pos2.x - pos1.x, 0f).normalized;
            AddVertex(pos1 + lineOff, uvPoint, col1, appearStep);
            AddVertex(pos1 - lineOff, uvPoint, col1, appearStep);
            AddVertex(pos2 - lineOff, uvPoint, col2, appearStep);
            AddVertex(pos2 + lineOff, uvPoint, col2, appearStep);
        }

        public void ApplyOffset(Vector2 offset)
        {
            for (Int32 i = 0; i < Vertices.size; i++)
            {
                Vertices.buffer[i].x += offset.x;
                Vertices.buffer[i].y += offset.y;
            }
            foreach (DialogImage dialogImage in SpecialImages)
            {
                dialogImage.LocalPosition.x += offset.x;
                dialogImage.LocalPosition.y += offset.y;
            }
        }

        public Rect GetLineRenderRect(Int32 line)
        {
            if (line < 0 || line >= LineInfo.Count || Step < ParseStep.Render)
                return new Rect();
            Int32 firstIndex = LineInfo[line].FirstVertexIndex;
            Int32 endIndex = LineInfo[line].EndVertexIndex;
            return NGUIMath.GetBoundingBox(Vertices.Where((v, i) => i >= firstIndex && i < endIndex));
        }

        // Currently, BIDI is used only if the base language is not left-to-right for speed performance
        private Boolean ShouldUseBIDI => NGUIText.readingDirection != UnicodeBIDI.LanguageReadingDirection.LeftToRight;

        public class Line
        {
            /// <summary>Approximate line width</summary>
            public Single Width = 0f;
            /// <summary>The top position of characters, with positive pointing downward</summary>
            public Single BaseY = 0f;
            /// <summary>Approximate line height</summary>
            public Single BaseHeight = 0f;

            /// <summary>First character index of the line</summary>
            public Int32 FirstVertexIndex = 0;
            /// <summary>Last character index of the line + 1</summary>
            public Int32 EndVertexIndex = 0;

            /// <summary>Line alignments are computed only at Render step</summary>
            public NGUIText.Alignment Alignment = NGUIText.Alignment.Left;
        }

        public enum ParseStep
        {
            Page,
            ConstantReplaceTags,
            ChoiceSetup,
            VariableReplaceTags,
            BIDI,
            Wrapped,
            Render
        }
    }
}
