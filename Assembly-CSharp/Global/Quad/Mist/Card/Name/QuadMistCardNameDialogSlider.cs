using System;
using System.Collections;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using UnityEngine;

public class QuadMistCardNameDialogSlider : MonoBehaviour
{
    public Boolean IsShowCardName
    {
        get => this.isShowCardName;
        set => this.isShowCardName = value;
    }

    public Boolean IsShowing => this.isShowing;
    public Boolean IsReady => this.isReady;

    public void ShowCardNameDialog(Hand playerHand)
    {
        if (playerHand.SelectedUI == null)
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
        if (this.dialog != null)
            base.StartCoroutine(this.HideDialogWithCoroutine(playerHand));
    }

    public void OnLocalize()
    {
        if (!isActiveAndEnabled)
            return;
        if (this.dialog != null && this.dialog.CurrentState > Dialog.State.Initial && this.dialog.CurrentState < Dialog.State.StartHide)
            this.dialog.ChangePhraseSoft($"[STRT=0,1][CENT][NANI][IMME]{FF9TextTool.CardName(this.cardId)}[TIME=-1]");
    }

    private IEnumerator ShowDialogWithCoroutine(Hand playerHand)
    {
        this.isReady = false;
        if (this.dialog != null)
            Singleton<DialogManager>.Instance.ReleaseDialogToPool(this.dialog);
        this.cardId = playerHand.SelectedUI.Data.id;
        this.dialog = Singleton<DialogManager>.Instance.AttachDialog($"[STRT=0,1][CENT][NANI][IMME]{FF9TextTool.CardName(this.cardId)}[TIME=-1]", 0, 1, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
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
        if (tweenPos == null)
            tweenPos = this.dialog.gameObject.AddComponent<TweenPosition>();
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
        this.dialog = null;
        this.isReady = true;
        yield break;
    }

    private Vector2 CalculateDialogTargetPosition(Int32 currentCardIndex, Int32 totalCards)
    {
        Single x;
        Single y;
        if (totalCards == 5)
        {
            x = 564f;
            if (currentCardIndex == 4)
            {
                y = -292f;
                y += this.dialog.Size.y * 0.5f;
            }
            else
            {
                y = 286f - 202f * currentCardIndex;
                y -= this.dialog.Size.y * 0.5f;
            }
        }
        else
        {
            x = 583f;
            if (currentCardIndex == 3)
            {
                y = -236f;
                y += this.dialog.Size.y * 0.5f;
            }
            else
            {
                y = 236f - 500f / 2f * currentCardIndex;
                y -= this.dialog.Size.y * 0.5f;
            }
        }
        if (x + this.dialog.Size.x / 2f > 771.5f)
            x = 771.5f - this.dialog.Size.x / 2f - 20f;
        return new Vector2(x, y);
    }

    public AnimationCurve AnimationCurv;
    private Dialog dialog;
    private Boolean isShowCardName;
    private Boolean isShowing;
    private Boolean isReady = true;
    public HonoTweenPosition TweenPosition;

    [NonSerialized]
    private TetraMasterCardId cardId;
}
