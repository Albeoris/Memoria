using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Interaction/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
    public void SetActive(Boolean active, Boolean fromStart)
    {
        this.mActive = active;
        this.enabled = active;
        if (this.mActive)
            this.mPreviousTime = RealTime.time;
        if (fromStart && this.mLabel != null)
            this.mLabel.Parser.ResetProgress();
    }

    private void Update()
    {
        if (!this.mActive)
            return;
        if (this.mReset)
        {
            this.mLabel = base.GetComponent<UILabel>();
            if (this.mLabel != null)
            {
                this.mDialog = this.mLabel.DialogWindow;
                this.mPreviousTime = RealTime.time;
                this.mReset = false;
            }
        }
        if (this.mLabel == null)
            return;
        if (this.mDialog != null && this.mDialog.CurrentState != Dialog.State.TextAnimation)
            return;

        Single currentTime = RealTime.time;
        Single progress = (currentTime - this.mPreviousTime) * (HonoBehaviorSystem.Instance.IsFastForwardModeActive() ? FF9StateSystem.Settings.FastForwardFactor : 1f);
        this.mPreviousTime = currentTime;
        if (this.mLabel.Parser.AdvanceProgress(progress) && this.mDialog != null)
            this.mDialog.AfterSentenseShown();
    }

    // All dummied except mDialog, mLabel, mPreviousTime, mActive and mReset
    public static TypewriterEffect current;

    public Single charsPerSecond = 20f;
    public Single fadeInTime;
    public Single delayOnPeriod;
    public Single delayOnNewLine;

    public UIScrollView scrollView;

    public Boolean keepFullDimensions;

    public List<EventDelegate> onFinished;
    public TypewriterEffect.IntDelegate onCharacterFinished;

    [NonSerialized]
    private Dialog mDialog;
    private UILabel mLabel;

    [NonSerialized]
    private Single mPreviousTime;

    private String mFullText = String.Empty;
    private Int32 mCurrentOffset;
    private Int32 ff9Signal;
    private Single mNextChar;
    private Boolean mReset = true;
    private Boolean mActive = false;

    private Dictionary<Int32, Single> mDynamicCharsPerSecond;
    private Dictionary<Int32, Single> mWaitList;
    private BetterList<TypewriterEffect.FadeEntry> mFade;

    private struct FadeEntry
    {
        public Int32 index;
        public String text;
        public Single alpha;
    }

    public delegate void IntDelegate(GameObject go, Int32 value);
}
