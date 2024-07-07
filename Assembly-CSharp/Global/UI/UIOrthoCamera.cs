using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Orthographic Camera")]
[RequireComponent(typeof(Camera))]
public class UIOrthoCamera : MonoBehaviour
{
    private void Start()
    {
        this.mCam = base.GetComponent<Camera>();
        this.mTrans = base.transform;
        this.mCam.orthographic = true;
    }

    private void Update()
    {
        Single num = this.mCam.rect.yMin * (Single)Screen.height;
        Single num2 = this.mCam.rect.yMax * (Single)Screen.height;
        Single num3 = (num2 - num) * 0.5f * this.mTrans.lossyScale.y;
        if (!Mathf.Approximately(this.mCam.orthographicSize, num3))
        {
            this.mCam.orthographicSize = num3;
        }
    }

    private Camera mCam;

    private Transform mTrans;
}
