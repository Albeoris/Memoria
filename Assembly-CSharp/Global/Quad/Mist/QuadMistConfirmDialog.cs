using System;
using UnityEngine;

public class QuadMistConfirmDialog : MonoBehaviour
{
    private void Start()
    {
        QuadMistConfirmDialog.main = this;
        base.gameObject.SetActive(false);
    }

    private Boolean Select
    {
        get
        {
            return _select;
        }
        set
        {
            _select = value;
            QuadMistConfirmDialog.main.okMesh.color = Color.white;
            QuadMistConfirmDialog.main.cancelMesh.color = Color.white;
            if (highlight)
            {
                if (!Select)
                {
                    QuadMistConfirmDialog.main.okMesh.color = Color.gray;
                }
                else
                {
                    QuadMistConfirmDialog.main.cancelMesh.color = Color.gray;
                }
            }
        }
    }

    public static void MessageSelect(Boolean ok)
    {
        QuadMistConfirmDialog.main.Select = ok;
    }

    public static Boolean MessageSelect(Vector3 worldPos)
    {
        worldPos.z = QuadMistConfirmDialog.main.ok.transform.position.z;
        if (QuadMistConfirmDialog.main.ok.bounds.Contains(worldPos))
        {
            QuadMistConfirmDialog.main.Select = true;
            return true;
        }
        worldPos.z = QuadMistConfirmDialog.main.cancel.transform.position.z;
        if (QuadMistConfirmDialog.main.cancel.bounds.Contains(worldPos))
        {
            QuadMistConfirmDialog.main.Select = false;
            return true;
        }
        return false;
    }

    public static void MessageShow(Vector3 pos, String msg, Boolean highlight = true, Boolean select = true)
    {
        QuadMistConfirmDialog.main.gameObject.SetActive(true);
        pos.z = -5f;
        QuadMistConfirmDialog.main.transform.position = pos;
        QuadMistConfirmDialog.main.message.text = msg;
        QuadMistConfirmDialog.main.highlight = highlight;
        QuadMistConfirmDialog.main.Select = select;
    }

    public static void MessageHide()
    {
        QuadMistConfirmDialog.main.gameObject.SetActive(false);
    }

    public static Boolean IsOK
    {
        get
        {
            return QuadMistConfirmDialog.main.Select;
        }
    }

    public static Boolean IsShowing
    {
        get
        {
            return QuadMistConfirmDialog.main.gameObject.activeSelf;
        }
    }

    private static QuadMistConfirmDialog main;

    public MeshCollider ok;

    public MeshCollider cancel;

    public TextMesh okMesh;

    public TextMesh cancelMesh;

    public TextMesh message;

    private Boolean _select;

    private Boolean highlight;
}
