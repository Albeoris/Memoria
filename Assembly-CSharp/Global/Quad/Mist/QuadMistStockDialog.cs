using System;
using UnityEngine;

public class QuadMistStockDialog : MonoBehaviour
{
    private void Start()
    {
        base.gameObject.SetActive(false);
        QuadMistStockDialog.main = this;
    }

    public static void Show(Vector3 position, String message)
    {
        if (QuadMistStockDialog.main.dialog != null)
        {
            Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistStockDialog.main.dialog);
            QuadMistStockDialog.main.dialog = null;
        }
        QuadMistStockDialog.main.dialog = Singleton<DialogManager>.Instance.AttachDialog($"[STRT=0,1][CENT][NANI][IMME]{message}[TIME=-1]", 0, 1, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
        QuadMistStockDialog.main.dialog.transform.localPosition = new Vector3(-235f, 290f + (position.y - 0.6003987f) * 3.125f * 155f);
    }

    public static void Hide()
    {
        QuadMistStockDialog.main.dialog.transform.localPosition = new Vector3(-3000f, -3000f);
        Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistStockDialog.main.dialog);
        QuadMistStockDialog.main.dialog = null;
    }

    private static QuadMistStockDialog main;

    public TextMesh Message;
    private Dialog dialog;
}
