using System;
using System.Collections;
using UnityEngine;

public class HonoTweenPosition : MonoBehaviour
{
    public Vector3[] DestinationPosition
    {
        get
        {
            return this.stopedPositionList;
        }
        set
        {
            this.stopedPositionList = value;
        }
    }

    private void Awake()
    {
        this.Setup();
    }

    private void Setup()
    {
        this.dialogTweenPosition = new TweenPosition[(Int32)this.dialogList.Length];
        this.stopedPositionList = new Vector3[(Int32)this.dialogList.Length];
        this.dialogWidgetList = new UIWidget[(Int32)this.dialogList.Length];
        this.updateAnchor = new UIRect.AnchorUpdate[(Int32)this.dialogList.Length];
        this.anchorTransform = new Transform[(Int32)this.dialogList.Length];
        Byte b = 0;
        this.busy = false;
        GameObject[] array = this.dialogList;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            GameObject gameObject = array[i];
            if (!(gameObject == (UnityEngine.Object)null))
            {
                TweenPosition tweenPosition = gameObject.GetComponent<TweenPosition>();
                if (tweenPosition == (UnityEngine.Object)null)
                {
                    tweenPosition = gameObject.AddComponent<TweenPosition>();
                }
                this.dialogWidgetList[(Int32)b] = gameObject.GetComponent<UIWidget>();
                if (this.dialogWidgetList[(Int32)b] != (UnityEngine.Object)null)
                {
                    this.updateAnchor[(Int32)b] = this.dialogWidgetList[(Int32)b].updateAnchors;
                    this.anchorTransform[(Int32)b] = this.dialogWidgetList[(Int32)b].leftAnchor.target;
                    this.dialogWidgetList[(Int32)b].updateAnchors = UIRect.AnchorUpdate.OnStart;
                }
                tweenPosition.ignoreTimeScale = false;
                tweenPosition.enabled = false;
                this.dialogTweenPosition[(Int32)b] = tweenPosition;
                this.stopedPositionList[(Int32)b] = gameObject.transform.localPosition;
                base.StartCoroutine(this.SetDefaultPos(gameObject.transform, (Int32)b));
                b = (Byte)(b + 1);
            }
        }
    }

    private IEnumerator SetDefaultPos(Transform target, Int32 widgetIndex)
    {
        yield return new WaitForEndOfFrame();
        if (this.changeStartScenePosition)
        {
            if (this.snapYPosition)
            {
                target.localPosition = new Vector3(this.animatedInStartPosition.x, target.localPosition.y, this.animatedInStartPosition.z);
            }
            else
            {
                target.localPosition = this.animatedInStartPosition;
            }
        }
        if (this.dialogWidgetList[widgetIndex] != (UnityEngine.Object)null)
        {
            this.dialogWidgetList[widgetIndex].updateAnchors = this.updateAnchor[widgetIndex];
        }
        yield break;
    }

    private void Reset()
    {
        this.busy = false;
        TweenPosition[] array = this.dialogTweenPosition;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            TweenPosition tweenPosition = array[i];
            if (!(tweenPosition == (UnityEngine.Object)null))
            {
                tweenPosition.enabled = false;
                if (!this.doNotResetPositionWhenDisable)
                {
                    tweenPosition.transform.localPosition = this.animatedInStartPosition;
                }
                if (this.debug)
                {
                    tweenPosition.gameObject.SetActive(false);
                }
                tweenPosition.animationCurve = this.transitionCurve;
            }
        }
    }

    public void TweenIn(Byte[] dialogIndexes, UIScene.SceneVoidDelegate callBack = null)
    {
        if (base.gameObject.activeInHierarchy && !this.busy)
        {
            this.busy = true;
            Single num = 0f;
            for (Int32 i = 0; i < (Int32)dialogIndexes.Length; i++)
            {
                Byte b = dialogIndexes[i];
                Vector3 from = this.animatedInStartPosition;
                Vector3 to = this.stopedPositionList[(Int32)b];
                if (this.animatePositionType == HonoTweenPosition.AnimatePositionType.ABSOLUTE)
                {
                    from.x = this.stopedPositionList[(Int32)b].x;
                    from.y = this.stopedPositionList[(Int32)b].y;
                    to.x = this.stopedPositionList[(Int32)b].x + this.animatedInStartPosition.x;
                    to.y = this.stopedPositionList[(Int32)b].y + this.animatedInStartPosition.y;
                }
                else if (this.snapYPosition)
                {
                    from.y = this.stopedPositionList[(Int32)b].y;
                }
                else
                {
                    from.y = this.animatedInStartPosition.y;
                }
                this.Animate(b, from, to, this.delayList[(Int32)b]);
                if (num < this.delayList[(Int32)b])
                {
                    num = this.delayList[(Int32)b];
                }
            }
            HonoTweenPosition.WaitInput value = new HonoTweenPosition.WaitInput(dialogIndexes, this.duration + num, HonoTweenPosition.TweenType.IN, callBack);
            base.StartCoroutine("WaitForAnimation", value);
        }
    }

    public void TweenOut(Byte[] dialogIndexes, UIScene.SceneVoidDelegate callBack = null)
    {
        if (base.gameObject.activeInHierarchy && !this.busy)
        {
            this.busy = true;
            Single num = 0f;
            Vector3 from = Vector3.zero;
            Vector3 zero = Vector3.zero;
            for (Int32 i = 0; i < (Int32)dialogIndexes.Length; i++)
            {
                Byte b = dialogIndexes[i];
                from = this.stopedPositionList[(Int32)b];
                zero = this.animatedOutEndPosition;
                if (this.animatePositionType == HonoTweenPosition.AnimatePositionType.ABSOLUTE)
                {
                    from.x = this.stopedPositionList[(Int32)b].x + this.animatedInStartPosition.x;
                    zero.x = this.stopedPositionList[(Int32)b].x + this.animatedOutEndPosition.x;
                    zero.y = this.stopedPositionList[(Int32)b].y + this.animatedOutEndPosition.y;
                }
                else if (this.snapYPosition)
                {
                    zero.y = this.stopedPositionList[(Int32)b].y;
                }
                else
                {
                    zero.y = this.animatedOutEndPosition.y;
                }
                this.Animate(b, from, zero, this.delayList[(Int32)b]);
                if (num < this.delayList[(Int32)b])
                {
                    num = this.delayList[(Int32)b];
                }
            }
            HonoTweenPosition.WaitInput value = new HonoTweenPosition.WaitInput(dialogIndexes, this.duration + num, HonoTweenPosition.TweenType.OUT, callBack);
            base.StartCoroutine("WaitForAnimation", value);
        }
    }

    public void TweenPingPong(Byte[] dialogIndexes, UIScene.SceneVoidDelegate afterTweenInCallBack = null, UIScene.SceneVoidDelegate afterTweenOutCallBack = null)
    {
        if (base.gameObject.activeInHierarchy && !this.busyPingPong)
        {
            this.busyPingPong = true;
            base.StartCoroutine(this.PingPongProcess(dialogIndexes, afterTweenInCallBack, afterTweenOutCallBack));
        }
    }

    private IEnumerator PingPongProcess(Byte[] dialogIndexes, UIScene.SceneVoidDelegate afterTweenInCallBack = null, UIScene.SceneVoidDelegate afterTweenOutCallBack = null)
    {
        Single maxDelayTime = 0f;
        Single[] array = this.delayList;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            Single index = array[i];
            if (maxDelayTime < index)
            {
                maxDelayTime = index;
            }
        }
        this.TweenIn(dialogIndexes, afterTweenInCallBack);
        yield return new WaitForSeconds(maxDelayTime + this.duration + this.resetAnchorDelay);
        this.TweenOut(dialogIndexes, afterTweenOutCallBack);
        yield return new WaitForSeconds(maxDelayTime + this.duration + this.resetAnchorDelay);
        this.busyPingPong = false;
        yield break;
    }

    private void Animate(Byte index, Vector3 from, Vector3 to, Single delay)
    {
        TweenPosition tweenPosition = this.dialogTweenPosition[(Int32)index];
        if (this.dialogWidgetList[(Int32)index] != (UnityEngine.Object)null)
        {
            this.dialogWidgetList[(Int32)index].SetAnchor((Transform)null);
            this.dialogWidgetList[(Int32)index].updateAnchors = UIRect.AnchorUpdate.OnStart;
        }
        if (tweenPosition != (UnityEngine.Object)null)
        {
            tweenPosition.transform.localPosition = from;
            tweenPosition.from = from;
            tweenPosition.to = to;
            tweenPosition.duration = this.duration;
            tweenPosition.delay = delay;
            tweenPosition.animationCurve = this.transitionCurve;
            tweenPosition.enabled = true;
            tweenPosition.ResetToBeginning();
            tweenPosition.gameObject.SetActive(true);
        }
    }

    private IEnumerator WaitForAnimation(HonoTweenPosition.WaitInput input)
    {
        yield return new WaitForSeconds(input.duration + this.resetAnchorDelay);
        Byte[] dialogIndexes = input.dialogIndexes;
        for (Int32 i = 0; i < (Int32)dialogIndexes.Length; i++)
        {
            Byte index = dialogIndexes[i];
            if (!(this.dialogWidgetList[(Int32)index] == (UnityEngine.Object)null))
            {
                if (input.tweenType == HonoTweenPosition.TweenType.OUT && this.deactiveAfterTweenOut)
                {
                    this.dialogList[(Int32)index].SetActive(false);
                }
                this.dialogWidgetList[(Int32)index].updateAnchors = this.updateAnchor[(Int32)index];
                this.dialogWidgetList[(Int32)index].SetAnchor(this.anchorTransform[(Int32)index]);
            }
        }
        this.busy = false;
        if (input.callBack != null)
        {
            input.callBack();
        }
        yield break;
    }

    private void OnDisable()
    {
        this.Reset();
    }

    public void StopAnimation()
    {
        if (base.gameObject.activeInHierarchy)
        {
            this.busy = false;
            base.StopCoroutine("WaitForAnimation");
            TweenPosition[] array = this.dialogTweenPosition;
            for (Int32 i = 0; i < (Int32)array.Length; i++)
            {
                TweenPosition tweenPosition = array[i];
                tweenPosition.enabled = false;
            }
        }
    }

    public AnimationCurve transitionCurve;

    public HonoTweenPosition.AnimatePositionType animatePositionType;

    public Vector3 animatedInStartPosition = new Vector3(1543f, 0f);

    public Vector3 animatedOutEndPosition = new Vector3(-1543f, 0f);

    public Boolean snapYPosition;

    public Single resetAnchorDelay = 0.2f;

    public Single duration = 0.4f;

    public Single[] delayList;

    public GameObject[] dialogList;

    private TweenPosition[] dialogTweenPosition;

    private UIWidget[] dialogWidgetList;

    private Vector3[] stopedPositionList;

    private Boolean busy;

    private Boolean busyPingPong;

    public Boolean deactiveAfterTweenOut = true;

    public Boolean doNotResetPositionWhenDisable;

    public Boolean changeStartScenePosition;

    public Boolean debug;

    private UIRect.AnchorUpdate[] updateAnchor;

    private Transform[] anchorTransform;

    public enum TweenType
    {
        IN,
        OUT
    }

    public enum AnimatePositionType
    {
        LOCAL,
        ABSOLUTE
    }

    public class WaitInput
    {
        public WaitInput(Byte[] dialogIndexes, Single duration, HonoTweenPosition.TweenType tweenType, UIScene.SceneVoidDelegate callBack = null)
        {
            this.dialogIndexes = dialogIndexes;
            this.tweenType = tweenType;
            this.duration = duration;
            this.callBack = callBack;
        }

        public Byte[] dialogIndexes;

        public HonoTweenPosition.TweenType tweenType;

        public Single duration;

        public UIScene.SceneVoidDelegate callBack;
    }
}
