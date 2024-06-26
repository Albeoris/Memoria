using System;
using UnityEngine;
using Object = System.Object;

public class BubbleGrid : MonoBehaviour
{
    private void Start()
    {
        this.myTransform = base.transform;
        this.childList = new BubbleButton[this.myTransform.childCount];
        Byte b = 0;
        foreach (Object obj in this.myTransform)
        {
            Transform transform = (Transform)obj;
            this.childList[(Int32)b] = transform.GetComponent<BubbleButton>();
            b = (Byte)(b + 1);
        }
    }

    public void Reposition(Byte[] indexes)
    {
        if ((Int32)indexes.Length == 2)
        {
            try
            {
                this.ArrangeCell(indexes);
            }
            catch
            {
            }
        }
        else if ((Int32)indexes.Length == 1)
        {
            try
            {
                this.childList[(Int32)indexes[0]].transform.localPosition = Vector3.zero;
                this.childList[(Int32)indexes[0]].transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            catch
            {
            }
        }
        else if (indexes.Length == 0)
        {

        }
        else
        {
            global::Debug.LogWarning("This version is not supported to arrange bubble over 3 indexes.");
        }
    }

    private void ArrangeCell(Byte[] indexes)
    {
        Byte b = indexes[1];
        this.childList[(Int32)b].transform.localPosition = new Vector3((this.childList[(Int32)b].widget.localSize.x + this.paddingX) / 2f, this.childList[(Int32)b].transform.localPosition.y, this.childList[(Int32)b].transform.localPosition.z);
        this.childList[(Int32)b].transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 5f));
        b = indexes[0];
        this.childList[(Int32)b].transform.localPosition = new Vector3(-((this.childList[(Int32)b].widget.localSize.x + this.paddingX) / 2f), this.childList[(Int32)b].transform.localPosition.y, this.childList[(Int32)b].transform.localPosition.z);
        this.childList[(Int32)b].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 5f));
    }

    private Transform myTransform;

    [SerializeField]
    private BubbleButton[] childList;

    public Single paddingX = 5f;
}
