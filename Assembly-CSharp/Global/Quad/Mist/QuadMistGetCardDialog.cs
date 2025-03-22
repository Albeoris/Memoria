using System;
using UnityEngine;

public class QuadMistGetCardDialog : MonoBehaviour
{
    private void Start()
    {
        base.gameObject.SetActive(false);
        QuadMistGetCardDialog.main = this;
    }

    public static void Show(Vector3 position, String message)
    {
        if (QuadMistGetCardDialog.main.dialog != null)
        {
            Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistGetCardDialog.main.dialog);
            QuadMistGetCardDialog.main.dialog = null;
        }
        QuadMistGetCardDialog.main.dialog = Singleton<DialogManager>.Instance.AttachDialog($"[STRT=0,1][CENT][NANI][IMME]{message}[TIME=-1]", 0, 1, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, new Vector2(10000f, 10000f), Dialog.CaptionType.None);
        QuadMistGetCardDialog.main.dialog.transform.localPosition = new Vector3(0f, -220f);
    }

    public static void Hide()
    {
        QuadMistGetCardDialog.main.dialog.transform.localPosition = new Vector3(-3000f, -3000f);
        Singleton<DialogManager>.Instance.ReleaseDialogToPool(QuadMistGetCardDialog.main.dialog);
        QuadMistGetCardDialog.main.dialog = null;
    }

    private static QuadMistGetCardDialog main;

    public TextMesh Message;
    private Dialog dialog;
}
