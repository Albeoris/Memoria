using System;
using UnityEngine;
using Object = System.Object;

public class QuadMistGetCardDialog : MonoBehaviour
{
    private void Start()
    {
        base.gameObject.SetActive(false);
        QuadMistGetCardDialog.main = this;
    }

    public static void Show(Vector3 position, String message)
    {
        if (QuadMistGetCardDialog.main.dialog != (UnityEngine.Object)null)
        {
            Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistGetCardDialog.main.dialog);
            QuadMistGetCardDialog.main.dialog = (Dialog)null;
        }
        UILabel dialogLabel = Singleton<DialogManager>.Instance.GetDialogLabel();
        Int32 width = dialogLabel.width;
        dialogLabel.width = Convert.ToInt32(UIManager.UIContentSize.x);
        dialogLabel.ProcessText();
        dialogLabel.UpdateNGUIText();
        Int32 num = Convert.ToInt32((NGUIText.CalculatePrintedSize2(message).x + Dialog.DialogPhraseXPadding * 2f) / UIManager.ResourceXMultipier) + 1;
        dialogLabel.width = width;
        QuadMistGetCardDialog.main.dialog = Singleton<DialogManager>.Instance.AttachDialog(String.Concat(new Object[]
        {
            "[STRT=",
            num,
            ",1][CENT][NANI][IMME]",
            message,
            "[TIME=-1]"
        }), 0, 1, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
        QuadMistGetCardDialog.main.dialog.transform.localPosition = new Vector3(0f, -220f);
    }

    public static void Hide()
    {
        QuadMistGetCardDialog.main.dialog.transform.localPosition = new Vector3(-3000f, -3000f);
        Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistGetCardDialog.main.dialog);
        QuadMistGetCardDialog.main.dialog = (Dialog)null;
    }

    private static QuadMistGetCardDialog main;

    public TextMesh Message;

    private Dialog dialog;
}
