using System;
using UnityEngine;

public class QuadMistCardNameDialog : MonoBehaviour
{
    private void Start()
    {
        base.gameObject.SetActive(false);
        QuadMistCardNameDialog.main = this;
    }

    public static void Show(Vector3 position, String message)
    {
        QuadMistCardNameDialog.main.gameObject.SetActive(true);
        position.z = -5f;
        QuadMistCardNameDialog.main.transform.position = position;
        QuadMistCardNameDialog.main.Message.text = message;
    }

    public static void Hide()
    {
        QuadMistCardNameDialog.main.gameObject.SetActive(false);
    }

    private static QuadMistCardNameDialog main;

    public TextMesh Message;
}
