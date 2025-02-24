using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections;
using UnityEngine;
using Object = System.Object;

public class QuadMistCardNameDialogSlider : MonoBehaviour
{
    public Boolean IsShowCardName
    {
        get
        {
            return this.isShowCardName;
        }
        set
        {
            this.isShowCardName = value;
        }
    }

    public Boolean IsShowing
    {
        get
        {
            return this.isShowing;
        }
    }

    public Boolean IsReady
    {
        get
        {
            return this.isReady;
        }
    }

    public void ShowCardNameDialog(Hand playerHand)
    {
        if (playerHand.SelectedUI == (UnityEngine.Object)null)
        {
            this.HideCardNameDialog(playerHand);
        }
        else
        {
            this.isShowing = true;
            base.StartCoroutine(this.ShowDialogWithCoroutine(playerHand));
        }
    }

    public void HideCardNameDialog(Hand playerHand)
    {
        this.isShowing = false;
        if (this.dialog != (UnityEngine.Object)null)
        {
            base.StartCoroutine(this.HideDialogWithCoroutine(playerHand));
        }
    }

    private IEnumerator ShowDialogWithCoroutine(Hand playerHand)
    {
        this.isReady = false;
        if (this.dialog != null)
            Singleton<DialogManager>.Instance.ReleaseDialogToPool(this.dialog);
        String cardName = FF9TextTool.CardName(playerHand.SelectedUI.Data.id);
        this.dialog = Singleton<DialogManager>.Instance.AttachDialog($"[STRT=0,1][CENT][NANI][IMME]{cardName}[TIME=-1]", 0, 1, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
        this.dialog.Panel.depth -= 2;
        this.dialog.phrasePanel.depth -= 2;
        while (this.dialog.CurrentState != Dialog.State.CompleteAnimation)
            yield return new WaitForEndOfFrame();
        Vector2 targetPosition = this.CalculateDialogTargetPosition(playerHand.Select, playerHand.Count);
        TweenPosition tweenPos = this.dialog.GetComponent<TweenPosition>();
        if (tweenPos == null)
            tweenPos = this.dialog.gameObject.AddComponent<TweenPosition>();
        tweenPos.ignoreTimeScale = false;
        tweenPos.from = new Vector3(targetPosition.x + 800f, targetPosition.y);
        tweenPos.to = targetPosition;
        tweenPos.ResetToBeginning();
        tweenPos.duration = 0.3f;
        tweenPos.enabled = true;
        tweenPos.animationCurve = this.AnimationCurv;
        Single countDown = tweenPos.duration;
        while (countDown >= 0f)
        {
            countDown -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        UnityEngine.Object.Destroy(tweenPos);
        this.isReady = true;
        yield break;
    }

    private IEnumerator HideDialogWithCoroutine(Hand playerHand)
    {
        this.isReady = false;
        Vector2 targetPosition = this.CalculateDialogTargetPosition(playerHand.Select, playerHand.Count);
        TweenPosition tweenPos = this.dialog.GetComponent<TweenPosition>();
        if (tweenPos == (UnityEngine.Object)null)
        {
            tweenPos = this.dialog.gameObject.AddComponent<TweenPosition>();
        }
        Single duration = 0.3f;
        tweenPos.from = targetPosition;
        tweenPos.to = new Vector3(targetPosition.x + 800f, targetPosition.y);
        tweenPos.ResetToBeginning();
        tweenPos.duration = duration;
        tweenPos.enabled = true;
        tweenPos.animationCurve = this.AnimationCurv;
        Single countDown = duration;
        while (countDown >= 0f)
        {
            countDown -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        UnityEngine.Object.Destroy(tweenPos);
        Singleton<DialogManager>.Instance.ReleaseDialogToPool(this.dialog);
        this.dialog = (Dialog)null;
        this.isReady = true;
        yield break;
    }

    private Vector2 CalculateDialogTargetPosition(Int32 currentCardIndex, Int32 totalCards)
    {
        Single num;
        Single num2;
        if (totalCards == 5)
        {
            num = 564f;
            if (currentCardIndex == 4)
            {
                num2 = -292f;
                num2 += this.dialog.Size.y * 0.5f;
            }
            else
            {
                Single num3 = 286f;
                num2 = num3 - 202f * (Single)currentCardIndex;
                num2 -= this.dialog.Size.y * 0.5f;
            }
        }
        else
        {
            num = 583f;
            if (currentCardIndex == 3)
            {
                num2 = -236f;
                num2 += this.dialog.Size.y * 0.5f;
            }
            else
            {
                Single num4 = 236f;
                Single num5 = 500f;
                num2 = num4 - num5 / 2f * (Single)currentCardIndex;
                num2 -= this.dialog.Size.y * 0.5f;
            }
        }
        if (num + this.dialog.Size.x / 2f > 771.5f)
        {
            num = 771.5f - this.dialog.Size.x / 2f - 20f;
        }
        return new Vector2(num, num2);
    }

    public AnimationCurve AnimationCurv;

    private Dialog dialog;

    private Boolean isShowCardName;

    private Boolean isShowing;

    private Boolean isReady = true;

    public HonoTweenPosition TweenPosition;
}
