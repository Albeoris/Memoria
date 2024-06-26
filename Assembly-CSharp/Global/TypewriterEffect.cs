using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Interaction/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
    public Boolean isActive => this.mActive;

    public Dictionary<Int32, Single> DynamicCharsPerSecond
    {
        set => this.mDynamicCharsPerSecond = value;
    }

    public Dictionary<Int32, Single> WaitList
    {
        set => this.mWaitList = value;
    }

    public Int32 CurrentOffset => this.mCurrentOffset;

    public Int32 FullTextOffset => this.mFullText.Length;

    public void ResetToBeginning()
    {
        this.Finish();
        this.Restart();
        this.Update();
    }

    public void Pause()
    {
        this.mActive = false;
    }

    public void Resume()
    {
        this.mActive = true;
        this.mNextChar = RealTime.time;
    }

    public void Restart()
    {
        this.mReset = true;
        this.mActive = false;
        this.mNextChar = 0f;
        this.mCurrentOffset = 0;
    }

    public void Finish()
    {
        if (this.mActive)
        {
            this.mActive = false;
            if (!this.mReset)
            {
                this.mCurrentOffset = this.mFullText.Length;
                this.mFade.Clear();
                this.mLabel.text = this.mFullText;
            }
            if (this.keepFullDimensions && this.scrollView != (UnityEngine.Object)null)
            {
                this.scrollView.UpdatePosition();
            }
            TypewriterEffect.current = this;
            TypewriterEffect.current = (TypewriterEffect)null;
        }
    }

    private void OnEnable()
    {
        this.mReset = true;
        this.mActive = true;
    }

    private void OnDisable()
    {
        this.Finish();
    }

    private void Update()
    {
        if (!this.mActive)
        {
            return;
        }
        if (this.mReset)
        {
            this.mCurrentOffset = 0;
            this.ff9Signal = 0;
            this.mReset = false;
            this.mLabel = base.GetComponent<UILabel>();
            this.mFullText = this.mLabel.processedText;
            this.mFade.Clear();
            if (this.keepFullDimensions && this.scrollView != (UnityEngine.Object)null)
            {
                this.scrollView.UpdatePosition();
            }
        }
        if (String.IsNullOrEmpty(this.mFullText))
        {
            return;
        }
        while (this.mCurrentOffset < this.mFullText.Length && this.mNextChar <= RealTime.time)
        {
            Int32 num = this.mCurrentOffset;
            this.charsPerSecond = this.mDynamicCharsPerSecond.ContainsKey(this.mCurrentOffset) ? this.mDynamicCharsPerSecond[this.mCurrentOffset] : Mathf.Max(1f, this.charsPerSecond);
            if (this.mWaitList.ContainsKey(this.mCurrentOffset))
            {
                if (this.mWaitList[this.mCurrentOffset] > 0f)
                {
                    Dictionary<Int32, Single> dictionary2;
                    Dictionary<Int32, Single> dictionary = dictionary2 = this.mWaitList;
                    Int32 key2;
                    Int32 key = key2 = this.mCurrentOffset;
                    Single num2 = dictionary2[key2];
                    dictionary[key] = num2 - ((!HonoBehaviorSystem.Instance.IsFastForwardModeActive()) ? Time.deltaTime : (Time.deltaTime * (Single)FF9StateSystem.Settings.FastForwardFactor));
                    break;
                }
                this.mNextChar = RealTime.time;
            }
            Dialog.DialogImage dialogImage = (Dialog.DialogImage)null;
            if (this.mLabel.supportEncoding)
            {
                while (NGUIText.ParseSymbol(this.mFullText, ref this.mCurrentOffset, ref this.ff9Signal, ref dialogImage))
                {
                }
            }
            if (this.onCharacterFinished != null && dialogImage != null)
            {
                this.onCharacterFinished(base.gameObject, dialogImage.TextPosition);
            }
            this.mCurrentOffset++;
            if (this.mCurrentOffset > this.mFullText.Length)
            {
                break;
            }
            Single num3 = HonoBehaviorSystem.Instance.IsFastForwardModeActive() ? 1f / (this.charsPerSecond * FF9StateSystem.Settings.FastForwardFactor) : 1f / this.charsPerSecond;
            Char c = (Char)((num >= this.mFullText.Length) ? '\n' : this.mFullText[num]);
            if (c == '\n')
            {
                num3 += this.delayOnNewLine;
            }
            else if (num + 1 == this.mFullText.Length || this.mFullText[num + 1] <= ' ')
            {
                if (c == '.')
                {
                    if (num + 2 < this.mFullText.Length && this.mFullText[num + 1] == '.' && this.mFullText[num + 2] == '.')
                    {
                        num3 += this.delayOnPeriod * 3f;
                        num += 2;
                    }
                    else
                    {
                        num3 += this.delayOnPeriod;
                    }
                }
                else if (c == '!' || c == '?')
                {
                    num3 += this.delayOnPeriod;
                }
            }
            if (this.mNextChar == 0f)
            {
                this.mNextChar = RealTime.time + num3;
            }
            else
            {
                this.mNextChar += num3;
            }
            if (this.fadeInTime != 0f)
            {
                TypewriterEffect.FadeEntry item = default(TypewriterEffect.FadeEntry);
                item.index = num;
                item.alpha = 0f;
                item.text = this.mFullText.Substring(num, this.mCurrentOffset - num);
                this.mFade.Add(item);
            }
            else
            {
                this.mLabel.text = ((!this.keepFullDimensions) ? this.mFullText.Substring(0, this.mCurrentOffset) : (this.mFullText.Substring(0, this.mCurrentOffset) + "[00]" + this.mFullText.Substring(this.mCurrentOffset)));
                if (!this.keepFullDimensions && this.scrollView != (UnityEngine.Object)null)
                {
                    this.scrollView.UpdatePosition();
                }
            }
        }
        if (this.mCurrentOffset >= this.mFullText.Length)
        {
            this.mLabel.text = this.mFullText;
        }
        else if (this.mFade.size != 0)
        {
            Int32 i = 0;
            while (i < this.mFade.size)
            {
                TypewriterEffect.FadeEntry value = this.mFade[i];
                value.alpha += RealTime.deltaTime / this.fadeInTime;
                if (value.alpha < 1f)
                {
                    this.mFade[i] = value;
                    i++;
                }
                else
                {
                    this.mFade.RemoveAt(i);
                }
            }
            if (this.mFade.size == 0)
            {
                if (this.keepFullDimensions)
                {
                    this.mLabel.text = this.mFullText.Substring(0, this.mCurrentOffset) + "[00]" + this.mFullText.Substring(this.mCurrentOffset);
                }
                else
                {
                    this.mLabel.text = this.mFullText.Substring(0, this.mCurrentOffset);
                }
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (Int32 j = 0; j < this.mFade.size; j++)
                {
                    TypewriterEffect.FadeEntry fadeEntry = this.mFade[j];
                    if (j == 0)
                    {
                        stringBuilder.Append(this.mFullText.Substring(0, fadeEntry.index));
                    }
                    stringBuilder.Append('[');
                    stringBuilder.Append(NGUIText.EncodeAlpha(fadeEntry.alpha));
                    stringBuilder.Append(']');
                    stringBuilder.Append(fadeEntry.text);
                }
                if (this.keepFullDimensions)
                {
                    stringBuilder.Append("[00]");
                    stringBuilder.Append(this.mFullText.Substring(this.mCurrentOffset));
                }
                this.mLabel.text = stringBuilder.ToString();
            }
        }
        if (this.mCurrentOffset >= this.mFullText.Length)
        {
            TypewriterEffect.current = this;
            EventDelegate.Execute(this.onFinished);
            TypewriterEffect.current = (TypewriterEffect)null;
            this.mActive = false;
        }
    }

    public static TypewriterEffect current;

    public Single charsPerSecond = 20f;

    public Single fadeInTime;

    public Single delayOnPeriod;

    public Single delayOnNewLine;

    public UIScrollView scrollView;

    public Boolean keepFullDimensions;

    public List<EventDelegate> onFinished = new List<EventDelegate>();

    public TypewriterEffect.IntDelegate onCharacterFinished;

    private UILabel mLabel;

    private String mFullText = String.Empty;

    private Int32 mCurrentOffset;

    private Int32 ff9Signal;

    private Single mNextChar;

    private Boolean mReset = true;

    private Boolean mActive;

    private Dictionary<Int32, Single> mDynamicCharsPerSecond = new Dictionary<Int32, Single>();

    private Dictionary<Int32, Single> mWaitList = new Dictionary<Int32, Single>();

    private BetterList<TypewriterEffect.FadeEntry> mFade = new BetterList<TypewriterEffect.FadeEntry>();

    private struct FadeEntry
    {
        public Int32 index;

        public String text;

        public Single alpha;
    }

    public delegate void IntDelegate(GameObject go, Int32 value);
}
