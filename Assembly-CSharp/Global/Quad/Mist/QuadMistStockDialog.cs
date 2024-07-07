using System;
using UnityEngine;
using Object = System.Object;

public class QuadMistStockDialog : MonoBehaviour
{
    private void Start()
    {
        base.gameObject.SetActive(false);
        QuadMistStockDialog.main = this;
    }

    public static void Show(Vector3 position, String message)
    {
        if (QuadMistStockDialog.main.dialog != (UnityEngine.Object)null)
        {
            Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistStockDialog.main.dialog);
            QuadMistStockDialog.main.dialog = (Dialog)null;
        }
        UILabel dialogLabel = Singleton<DialogManager>.Instance.GetDialogLabel();
        Int32 width = dialogLabel.width;
        dialogLabel.width = Convert.ToInt32(UIManager.UIContentSize.x);
        dialogLabel.ProcessText();
        dialogLabel.UpdateNGUIText();
        Int32 num = Convert.ToInt32((NGUIText.CalculatePrintedSize2(message).x + Dialog.DialogPhraseXPadding * 2f) / UIManager.ResourceXMultipier) + 1;
        dialogLabel.width = width;
        QuadMistStockDialog.main.dialog = Singleton<DialogManager>.Instance.AttachDialog(String.Concat(new Object[]
        {
            "[STRT=",
            num,
            ",1][CENT][NANI][IMME]",
            message,
            "[TIME=-1]"
        }), 0, 1, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
        Single x = -235f;
        Single num2 = 0.6003987f;
        Single num3 = 290f;
        Single num4 = -0.32f;
        Single num5 = -155f;
        Single num6 = position.y - num2;
        Single num7 = num6 / num4;
        Single y = num3 + num7 * num5;
        QuadMistStockDialog.main.dialog.transform.localPosition = new Vector3(x, y);
    }

    public static void Hide()
    {
        QuadMistStockDialog.main.dialog.transform.localPosition = new Vector3(-3000f, -3000f);
        Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistStockDialog.main.dialog);
        QuadMistStockDialog.main.dialog = (Dialog)null;
    }

    private static QuadMistStockDialog main;

    public TextMesh Message;

    private Dialog dialog;
}
