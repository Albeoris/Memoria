using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TimeoutHandler))]
[RequireComponent(typeof(OnScreenButton))]
[RequireComponent(typeof(UIWidget))]
public class BubbleButton : MonoBehaviour
{
    public Single Timeout
    {
        get
        {
            return this.timeout;
        }
        set
        {
            this.timeout = value;
        }
    }

    public OnScreenButton ScreenButton
    {
        get
        {
            return this.onScreenButton;
        }
    }

    public UISprite MainSprite
    {
        get
        {
            return this.mainSprite;
        }
    }

    private void Start()
    {
        if (this.mStart)
        {
            return;
        }
        this.widget = base.GetComponent<UIWidget>();
        this.timeoutHandler = base.GetComponent<TimeoutHandler>();
        this.mainSprite = base.transform.GetChild(0).GetComponent<UISprite>();
        if (this.buttonType != BubbleUI.Flag.CURSOR)
        {
            this.backgroundTweenScale.duration = 0.175f;
            this.onScreenButton = base.gameObject.GetComponent<OnScreenButton>();
            if (this.onScreenButton == (UnityEngine.Object)null)
            {
                this.onScreenButton = base.gameObject.AddComponent<OnScreenButton>();
            }
            this.onScreenButton.AlwaysShow = true;
            switch (this.buttonType)
            {
                case BubbleUI.Flag.DUEL:
                    this.onScreenButton.KeyCommand = Control.Special;
                    goto IL_FB;
                case BubbleUI.Flag.BEACH:
                    this.onScreenButton.KeyCommand = Control.Cancel;
                    goto IL_FB;
            }
            if (this.onScreenButton.KeyCommand == Control.None)
            {
                this.onScreenButton.KeyCommand = Control.Confirm;
            }
        }
    IL_FB:
        this.mStart = true;
    }

    public void Show()
    {
        if (this.currentState == BubbleButton.State.Show)
        {
            return;
        }
        this.currentState = BubbleButton.State.Show;
        base.StopAllCoroutines();
        if (!this.mStart)
        {
            this.Start();
        }
        if (this.timeout != -1f)
        {
            this.timeoutHandler.Timeout = this.timeout;
            this.timeoutHandler.enabled = true;
        }
        else
        {
            this.timeoutHandler.enabled = false;
        }
        if (this.buttonType == BubbleUI.Flag.CURSOR)
        {
            base.gameObject.SetActive(true);
            return;
        }
        base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, -1f);
        if (this.signTweenScale != (UnityEngine.Object)null)
        {
            this.signTweenScale.duration = Singleton<BubbleUI>.Instance.AnimationDuration;
            this.signTweenScale.from = this.scaleFrom;
            this.signTweenScale.to = this.scaleTo;
            this.signTweenScale.ResetToBeginning();
            this.signTweenScale.enabled = true;
        }
        base.gameObject.SetActive(true);
        if (base.gameObject.activeInHierarchy && this.buttonType != BubbleUI.Flag.CURSOR)
        {
            base.StartCoroutine("ShowProcess");
        }
    }

    public void Hide()
    {
        if (!base.gameObject.activeSelf)
        {
            return;
        }
        if (this.currentState == BubbleButton.State.Hide)
        {
            return;
        }
        this.currentState = BubbleButton.State.Hide;
        base.StopAllCoroutines();
        if (this.buttonType == BubbleUI.Flag.CURSOR)
        {
            this.Deactivate();
            return;
        }
        if (this.signTweenScale != (UnityEngine.Object)null)
        {
            this.signTweenScale.duration = Singleton<BubbleUI>.Instance.AnimationDuration;
            this.signTweenScale.from = this.scaleTo;
            this.signTweenScale.to = this.scaleFrom;
            this.signTweenScale.ResetToBeginning();
            this.signTweenScale.enabled = true;
        }
        if (this.backgroundTweenScale != (UnityEngine.Object)null)
        {
            this.backgroundTweenScale.duration = Singleton<BubbleUI>.Instance.AnimationDuration / 2f;
            this.backgroundTweenScale.from = this.scaleTo;
            this.backgroundTweenScale.to = this.scaleFrom;
            this.backgroundTweenScale.ResetToBeginning();
            this.backgroundTweenScale.enabled = true;
        }
        base.gameObject.SetActive(true);
        if (base.gameObject.activeInHierarchy)
        {
            base.StartCoroutine("DisappearProcess");
        }
    }

    private IEnumerator ShowProcess()
    {
        this.backgroundTweenScale.duration = Singleton<BubbleUI>.Instance.AnimationDuration / 4f;
        this.backgroundTweenScale.from = this.scaleFrom;
        this.backgroundTweenScale.to = BubbleButton.ShowAnimationScale1;
        this.backgroundTweenScale.ResetToBeginning();
        this.backgroundTweenScale.enabled = true;
        yield return new WaitForSeconds(this.backgroundTweenScale.duration);
        this.backgroundTweenScale.from = this.backgroundTweenScale.to;
        this.backgroundTweenScale.to = BubbleButton.ShowAnimationScale2;
        this.backgroundTweenScale.ResetToBeginning();
        this.backgroundTweenScale.enabled = true;
        yield return new WaitForSeconds(this.backgroundTweenScale.duration);
        this.backgroundTweenScale.duration = Singleton<BubbleUI>.Instance.AnimationDuration / 2f;
        this.backgroundTweenScale.from = this.backgroundTweenScale.to;
        this.backgroundTweenScale.to = this.scaleTo;
        this.backgroundTweenScale.ResetToBeginning();
        this.backgroundTweenScale.enabled = true;
        yield break;
    }

    private IEnumerator DisappearProcess()
    {
        Single counter = 0f;
        while (counter < this.backgroundTweenScale.duration)
        {
            counter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.Deactivate();
        yield break;
    }

    private void Deactivate()
    {
        base.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        this.currentState = BubbleButton.State.Idle;
    }

    public BubbleUI.Flag buttonType;

    public UIWidget widget;

    public TweenScale signTweenScale;

    public TweenScale backgroundTweenScale;

    public BubbleButton.State currentState;

    public static readonly Vector3 ShowAnimationScale1 = new Vector3(0.4f, 1f, 1f);

    public static readonly Vector3 ShowAnimationScale2 = new Vector3(0.5f, 0.4f, 1f);

    private Vector3 scaleFrom = Vector3.zero;

    private Vector3 scaleTo = Vector3.one;

    private TimeoutHandler timeoutHandler;

    private Single timeout = -1f;

    private OnScreenButton onScreenButton;

    private UISprite mainSprite;

    private Boolean mStart;

    public enum State
    {
        Idle,
        Show,
        Hide
    }
}
