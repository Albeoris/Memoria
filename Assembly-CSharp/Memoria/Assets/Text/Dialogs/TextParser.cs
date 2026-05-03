using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    public class TextParser
    {
        public Single FadingPreviewTime = Configuration.Interface.TextFadeDuration;

        // Properly initialised when Step >= Page
        public readonly UILabel LabelContainer;
        public readonly String InitialText;
        public ParseStep Step = ParseStep.Page;
        public String ParsedText = String.Empty;

        // Properly initialised when Step >= ConstantReplaceTags
        public List<FFIXTextTag> ParsedTagList = new List<FFIXTextTag>();
        public List<TextAnimatedTag> AnimatedTags = new List<TextAnimatedTag>();

        // Properly initialised when Step >= ChoiceSetup
        public List<FFIXTextTag> VariableTagList = new List<FFIXTextTag>();
        public String VariableText = String.Empty;

        // Properly initialised when Step >= VariableReplaceTags
        public Dictionary<Int32, Int32> VariableMessageValues = new Dictionary<Int32, Int32>();

        // Properly initialised when Step >= Bidi
        public UnicodeBIDI Bidi = null;

        // Properly initialised when Step >= Wrapped
        public List<Line> LineInfo = new List<Line>();

        // Properly initialised when Step >= Render
        public List<Int32> CharToVertexIndex = new List<Int32>();
        public BetterList<Vector3> Vertices = new BetterList<Vector3>();
        public BetterList<Vector2> UVs = new BetterList<Vector2>();
        public BetterList<Color32> Colors = new BetterList<Color32>();
        public BetterList<Single> VertexAppearStep = new BetterList<Single>();
        public BetterList<Byte> FinalAlpha = new BetterList<Byte>();
        public BetterList<DialogImage> SpecialImages = new BetterList<DialogImage>();
        public Single AppearProgress = 0f;
        public Single AppearProgressMax = 0f;

        public Single MaxWidth => LineInfo.Count > 0 ? LineInfo.Max(line => line.Width) : 0f;
        public Single RawHeight => LineInfo.Count > 0 ? LineInfo.Max(line => line.BaseY + line.HeightBelow) : 0f;
        public Single DialogLineCount => Mathf.CeilToInt(RawHeight / Dialog.DialogLineHeight);
        public Vector2 FullSize => new Vector2(MaxWidth, RawHeight);
        public Rect RenderRect => NGUIMath.GetBoundingBox(Vertices);

        /// <summary>Create an instance from pages returned by DialogBoxSymbols.ParseTextSplitTags, or from text with no page tag</summary>
        public TextParser(UILabel container, String text)
        {
            LabelContainer = container;
            if (!String.IsNullOrEmpty(text) && container.DialogWindow == null && container.InputField == null)
                text = TextPatcher.PatchInterfaceString(text, container);
            InitialText = text;
            ParsedText = text;
        }

        public void Parse(ParseStep target)
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
                VariableTagList = FFIXTextTag.DeepListCopy(ParsedTagList);
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
                    Bidi = new UnicodeBIDI(ParsedText.ToCharArray(), NGUIText.readingDirection, NGUIText.digitShapes);
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

        public void ResetRender()
        {
            if (Step <= ParseStep.Wrapped)
                return;
            CharToVertexIndex.Clear();
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
            foreach (DialogImage dialogImage in SpecialImages)
                dialogImage.IsRegistered = false;
            Step = ParseStep.Wrapped;
        }

        public void ResetBeforeVariableTags()
        {
            if (Step <= ParseStep.ChoiceSetup)
                return;
            ParsedText = VariableText;
            ParsedTagList = FFIXTextTag.DeepListCopy(VariableTagList);
            foreach (TextAnimatedTag animTag in AnimatedTags)
                animTag.UpdateTagAfterCopy(ParsedTagList);
            VariableMessageValues.Clear();
            Bidi = null;
            LineInfo.Clear();
            CharToVertexIndex.Clear();
            Vertices.Clear();
            UVs.Clear();
            Colors.Clear();
            VertexAppearStep.Clear();
            FinalAlpha.Clear();
            foreach (DialogImage dialogImage in SpecialImages)
                dialogImage.IsRegistered = false;
            Step = ParseStep.ChoiceSetup;
            LabelContainer.MarkAsChanged();
        }

        public void ResetCompletly()
        {
            if (Step == ParseStep.Page)
                return;
            ResetBeforeVariableTags();
            foreach (DialogImage dialogImage in SpecialImages)
                LabelContainer.HideIcon(dialogImage);
            VariableTagList.Clear();
            VariableText = String.Empty;
            ParsedText = InitialText;
            ParsedTagList.Clear();
            AnimatedTags.Clear();
            LabelContainer.ReleaseAllIcons();
            Step = ParseStep.Page;
        }

        public void ResetProgress()
        {
            AppearProgress = 0f;
            foreach (TextAnimatedTag animTag in AnimatedTags)
                animTag.Reset();
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
                // NOTE: Tags sometimes apply logically to the left part of the text, such as closing tags ([SPED=-1], [/b], [C8C8C8][HSHD], ...)
                // ... and sometimes to the right part of the text ([SPED=2], [b], [C8B040][HSHD], ...)
                // TODO: Take opening tags into account and don't remove them when "ParsedTagList[i].TextOffset == pos + length"
                if (ParsedTagList[i].TextOffset > pos)
                {
                    if (ParsedTagList[i].TextOffset <= pos + length)
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

        public void ReplaceTag(Int32 tagIndex, String replaceStr, FFIXTextTag[] tagsBefore = null, FFIXTextTag[] tagsAfter = null)
        {
            if (tagIndex < 0 || tagIndex >= ParsedTagList.Count)
                return;
            Int32 insertPos = ParsedTagList[tagIndex].TextOffset;
            ParsedText = ParsedText.Insert(insertPos, replaceStr);
            ParsedTagList.RemoveAt(tagIndex);
            for (Int32 i = tagIndex; i < ParsedTagList.Count; i++)
                ParsedTagList[i].TextOffset += replaceStr.Length;
            if (tagsBefore != null)
                foreach (FFIXTextTag tag in tagsBefore)
                    InsertTag(tag, insertPos);
            if (tagsAfter != null)
            {
                insertPos += replaceStr.Length;
                foreach (FFIXTextTag tag in tagsAfter)
                    tag.TextOffset = insertPos;
                Int32 insertTagPos = ParsedTagList.FindIndex(tag => tag.TextOffset >= insertPos);
                if (insertTagPos < 0)
                    insertTagPos = ParsedTagList.Count;
                ParsedTagList.InsertRange(insertTagPos, tagsAfter);
            }
        }

        public void AdvanceProgressToMax(Boolean markChange = true)
        {
            AdvanceProgress(AppearProgressMax, markChange);
        }

        public Boolean AdvanceProgress(Single progress, Boolean markChange = true)
        {
            if (AppearProgressMax == 0f)
                Parse(ParseStep.Render);
            if (AppearProgress >= AppearProgressMax)
                return true;
            List<FFIXTextTag> appearTags = new List<FFIXTextTag>();
            Single nextStep = AppearProgress + progress;
            foreach (TextAnimatedTag animTag in AnimatedTags)
                if (animTag.Tag.AppearStep >= AppearProgress && animTag.Tag.AppearStep <= nextStep)
                    animTag.AppearTime = RealTime.time;
            foreach (FFIXTextTag tag in ParsedTagList)
                if ((tag.AppearStep <= 0f && AppearProgress <= 0f && nextStep > 0f) || (tag.AppearStep > AppearProgress && tag.AppearStep <= nextStep))
                    appearTags.Add(tag);
            appearTags.Sort((a, b) => a.AppearStep.CompareTo(b.AppearStep));
            foreach (FFIXTextTag tag in appearTags)
                DialogBoxSymbols.OnAppearTag(tag, LabelContainer.DialogWindow, LabelContainer);
            AppearProgress = nextStep;
            if (markChange)
                LabelContainer.MarkAsChanged();
            return AppearProgress >= AppearProgressMax;
        }

        public void ApplyRenderProgress()
        {
            for (Int32 i = 0; i < VertexAppearStep.size; i++)
            {
                if (VertexAppearStep[i] > AppearProgress && VertexAppearStep[i] < AppearProgress + FadingPreviewTime)
                    Colors.buffer[i].a = (Byte)Math.Round((AppearProgress + FadingPreviewTime - VertexAppearStep[i]) / FadingPreviewTime * FinalAlpha[i]);
                else
                    Colors.buffer[i].a = VertexAppearStep[i] <= AppearProgress ? FinalAlpha[i] : (Byte)0;
            }
            LabelContainer.StartCoroutine(UpdateIconDisplay());
        }

        private IEnumerator UpdateIconDisplay()
        {
            yield return new WaitForEndOfFrame();
            foreach (DialogImage dialogImage in SpecialImages)
            {
                if (!dialogImage.IsRegistered || dialogImage.AppearStep >= AppearProgress + FadingPreviewTime)
                    LabelContainer.HideIcon(dialogImage);
                else if (dialogImage.AppearStep > AppearProgress)
                    LabelContainer.PrintIcon(dialogImage, dialogImage.Alpha * (AppearProgress + FadingPreviewTime - dialogImage.AppearStep) / FadingPreviewTime);
                else
                    LabelContainer.PrintIcon(dialogImage, dialogImage.Alpha);
            }
            yield break;
        }

        public void AddVertex(Vector3 pos, Vector2 uv, Color32 col, Single appearStep)
        {
            Byte alpha = col.a;
            col.a = 0;
            Vertices.Add(pos);
            UVs.Add(uv);
            Colors.Add(col);
            FinalAlpha.Add(alpha);
            VertexAppearStep.Add(appearStep);
        }

        public void AddSegment(Vector3 pos1, Vector3 pos2, Color32 col1, Color32 col2, Single appearStep)
        {
            const Single LINE_WIDTH = 1f;
            Vector3 lineOff = LINE_WIDTH * new Vector3(pos1.y - pos2.y, pos2.x - pos1.x, 0f).normalized;
            Vector2 uvPoint = NGUIText.StrikeGlyphMidUV;
            AddVertex(pos1 + lineOff, uvPoint, col1, appearStep);
            AddVertex(pos1 - lineOff, uvPoint, col1, appearStep);
            AddVertex(pos2 - lineOff, uvPoint, col2, appearStep);
            AddVertex(pos2 + lineOff, uvPoint, col2, appearStep);
        }

        public void AddRectangle(Rect rect, Color32 topLeft, Color32 bottomLeft, Color32 bottomRight, Color32 topRight, Single appearStep)
        {
            Vector2 uvPoint = NGUIText.StrikeGlyphMidUV;
            AddVertex(new Vector3(rect.xMin, rect.yMin, 0f), uvPoint, topLeft, appearStep);
            AddVertex(new Vector3(rect.xMin, rect.yMax, 0f), uvPoint, bottomLeft, appearStep);
            AddVertex(new Vector3(rect.xMax, rect.yMax, 0f), uvPoint, bottomRight, appearStep);
            AddVertex(new Vector3(rect.xMax, rect.yMin, 0f), uvPoint, topRight, appearStep);
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
                dialogImage.LocalPosition.y -= offset.y;
            }
        }

        public Rect GetLineRenderRect(Int32 line)
        {
            if (line < 0 || line >= LineInfo.Count || Step < ParseStep.Render)
                return new Rect();
            Int32 firstIndex = LineInfo[line].FirstVertexIndex;
            Int32 endIndex = LineInfo[line].EndVertexIndex;
            List<Vector3> lineVertices = new List<Vector3>();
            for (Int32 i = firstIndex; i < endIndex; i++)
                lineVertices.Add(Vertices[i]);
            foreach (DialogImage dialogImage in SpecialImages)
            {
                if (dialogImage.PrintedLine == line)
                {
                    Single x = dialogImage.LocalPosition.x;
                    Single y = -dialogImage.LocalPosition.y;
                    lineVertices.Add(new Vector3(x, y));
                    lineVertices.Add(new Vector3(x + dialogImage.Size.x, y - dialogImage.Size.y));
                }
            }
            return NGUIMath.GetBoundingBox(lineVertices);
        }

        public Rect GetCharacterRenderRect(Int32 index)
        {
            if (index >= 0 && index < ParsedText.Length && NGUIText.IsSpace(ParsedText[index]))
            {
                Int32 lastVert = index - 1;
                Int32 nextVert = index + 1;
                while (lastVert >= 0 && NGUIText.IsSpace(ParsedText[lastVert]))
                    lastVert--;
                while (nextVert < ParsedText.Length && NGUIText.IsSpace(ParsedText[nextVert]))
                    nextVert++;
                if (lastVert < 0 || nextVert >= ParsedText.Length || ParsedText[lastVert] == '\n' || ParsedText[nextVert] == '\n')
                    return new Rect();
                Int32 nextVertIndex = CharToVertexIndex[nextVert];
                if (nextVertIndex + 1 >= Vertices.size || nextVertIndex <= 0)
                    return new Rect();
                List<Vector3> charVertices = new List<Vector3>();
                for (Int32 i = nextVertIndex - 1; i < nextVertIndex + 2; i++)
                    charVertices.Add(Vertices[i]);
                return NGUIMath.GetBoundingBox(charVertices);
            }
            else
            {
                Int32 firstIndex = index < 0 ? 0 : (index >= CharToVertexIndex.Count ? Vertices.size : CharToVertexIndex[index]);
                Int32 endIndex = index + 1 < 0 ? 0 : (index + 1 >= CharToVertexIndex.Count ? Vertices.size : CharToVertexIndex[index + 1]);
                List<Vector3> charVertices = new List<Vector3>();
                for (Int32 i = firstIndex; i < endIndex; i++)
                    charVertices.Add(Vertices[i]);
                return NGUIMath.GetBoundingBox(charVertices);
            }
        }

        /// <summary>Currently, BIDI is used only if the base language is not left-to-right for speed performance</summary>
        private Boolean ShouldUseBIDI => NGUIText.readingDirection != UnicodeBIDI.LanguageReadingDirection.LeftToRight;

        public class Line
        {
            public Line() { }
            public Line(Single baseY, Single baseHeight)
            {
                BaseY = baseY;
                HeightBelow = baseHeight;
            }

            /// <summary>Approximate line width</summary>
            public Single Width = 0f;
            /// <summary>The top position of characters, with positive pointing downward</summary>
            public Single BaseY = 0f;
            /// <summary>Approximate line height</summary>
            public Single HeightBelow = 0f;

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
