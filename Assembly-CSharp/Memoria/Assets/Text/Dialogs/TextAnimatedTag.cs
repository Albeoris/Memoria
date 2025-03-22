using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    public class TextAnimatedTag
    {
        public FFIXTextTag Tag;
        public Boolean Loop = false;
        public Boolean IgnorePause = false;
        public Single AppearTime = -1f;

        public Single MaxTime => Frames.Count > 0 ? Frames[Frames.Count - 1].TimeEnd : 1f;

        private List<TagFrame> Frames = new List<TagFrame>();
        private Boolean ReachedEnd = false;

        public TextAnimatedTag(FFIXTextTag animTag, FFIXTextTag animatedTag)
        {
            Tag = animatedTag;
            Int32 paramIndex = 1;
            if (animTag.StringParam(paramIndex) == "Loop")
            {
                paramIndex++;
                Loop = true;
            }
            if (animTag.StringParam(paramIndex) == "IgnorePause")
            {
                paramIndex++;
                IgnorePause = true;
            }
            Single time = 0f;
            for (Int32 i = paramIndex; i + 1 < animTag.Param.Length; i += 2)
            {
                if (Int32.TryParse(animTag.Param[i], out _) || !ParametricMovement.TryParseInterpolateType(animTag.Param[i], out ParametricMovement.InterpolateType interpolation))
                    break;
                TagFrame frame = new TagFrame();
                frame.Type = interpolation;
                frame.TimeStart = time;
                frame.TimeEnd = Math.Max(time, animTag.SingleParam(i + 1));
                Frames.Add(frame);
                time = frame.TimeEnd;
            }
            paramIndex += Frames.Count * 2;
            if (Frames.Count == 0)
                return;
            Int32 paramCount = (animTag.Param.Length - paramIndex) / (Frames.Count + 1);
            List<String> param = new List<String>(paramCount);
            for (Int32 i = 0; i < paramCount; i++)
                param.Add(animTag.Param[paramIndex++]);
            for (Int32 i = 0; i < Frames.Count; i++)
            {
                Frames[i].ParamStart = param;
                param = new List<String>(paramCount);
                for (Int32 j = 0; j < paramCount; j++)
                    param.Add(animTag.Param[paramIndex++]);
                Frames[i].ParamEnd = param;
            }
            ApplyTimeUnshifted(0f);
        }

        public void Reset()
        {
            AppearTime = -1f;
            ReachedEnd = false;
            ApplyTimeUnshifted(0f);
        }

        public IEnumerable<DialogImage> GetImages()
        {
            String[] paramBackup = Tag.Param;
            DialogImage img;
            foreach (TagFrame frame in Frames)
            {
                img = null;
                Tag.Param = frame.GetParameters(frame.TimeStart);
                if (DialogBoxSymbols.ParseImageTag(Tag, ref img) && img != null)
                    yield return img;
            }
            if (Frames.Count > 0)
            {
                img = null;
                Tag.Param = Frames[Frames.Count - 1].GetParameters(Frames[Frames.Count - 1].TimeEnd);
                if (DialogBoxSymbols.ParseImageTag(Tag, ref img) && img != null)
                    yield return img;
            }
            Tag.Param = paramBackup;
        }

        public Boolean ApplyTime(Single time)
        {
            if (AppearTime < 0 || ReachedEnd)
                return false;
            if (!IgnorePause && PersistenSingleton<UIManager>.Instance.IsPause)
            {
                AppearTime += RealTime.deltaTime;
                return false;
            }
            ApplyTimeUnshifted(time - AppearTime);
            return true;
        }

        private void ApplyTimeUnshifted(Single time)
        {
            if (Frames.Count == 0)
                return;
            TagFrame currentFrame = null;
            if (Loop)
                time %= MaxTime;
            else
                ReachedEnd = time >= MaxTime;
            foreach (TagFrame frame in Frames)
            {
                currentFrame = frame;
                if (time < frame.TimeEnd)
                    break;
            }
            Tag.Param = currentFrame.GetParameters(time);
        }

        private class TagFrame
        {
            public Single TimeStart = 0f;
            public Single TimeEnd = 1f;
            public List<String> ParamStart = new List<String>();
            public List<String> ParamEnd = new List<String>();
            public ParametricMovement.InterpolateType Type = ParametricMovement.InterpolateType.Linear;

            public String[] GetParameters(Single time)
            {
                time = Mathf.Clamp(time, TimeStart, TimeEnd);
                List<String> param = new List<String>(ParamStart.Count);
                Single factor1 = ParametricMovement.Factor1(time - TimeStart, TimeEnd - TimeStart, Type);
                Single factor2 = ParametricMovement.Factor2(time - TimeStart, TimeEnd - TimeStart, Type);
                for (Int32 i = 0; i < ParamStart.Count; i++)
                {
                    if (i >= ParamEnd.Count)
                    {
                        param.Add(ParamStart[i]);
                        continue;
                    }
                    if (time == TimeStart)
                        param.Add(ParamStart[i]);
                    else if (time == TimeEnd)
                        param.Add(ParamEnd[i]);
                    else if (Single.TryParse(ParamStart[i], out Single start) && Single.TryParse(ParamEnd[i], out Single end))
                        param.Add((factor1 * start + factor2 * end).ToString());
                    else
                        param.Add(ParamStart[i]);
                }
                return param.ToArray();
            }
        }
    }
}
