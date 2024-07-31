using System;
using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    private void Start()
    {
        this.myTrans = base.transform;
        this.mStart = true;
    }

    private void Update()
    {
        this.UpdateUIPosition();
    }

    private Boolean isNull()
    {
        return this.worldCam == null || this.uiCam == null || this.target == null;
    }

    public void UpdateUIPosition()
    {
        if (!this.mStart)
            this.Start();
        if (!this.isNull() && (this.lastPosition != this.target.position || !this.worldCam.worldToCameraMatrix.Equals(this.lastWorldMatrix) || this.updateEveryFrame))
        {
            if (this.target.gameObject.activeInHierarchy)
                this.lastPosition = this.target.position;
            this.lastWorldMatrix = this.worldCam.worldToCameraMatrix;
            Vector3 screenPos = this.worldCam.WorldToScreenPoint(this.lastPosition + this.targetTransformOffset);
            if (this.clampToScreen)
            {
                Single cameraMinX = (Screen.width - this.worldCam.pixelWidth) / 2f;
                Single cameraMinY = (Screen.height - this.worldCam.pixelHeight) / 2f;
                Single cameraMaxX = cameraMinX + this.worldCam.pixelWidth;
                Single cameraMaxY = cameraMinY + this.worldCam.pixelHeight;
                Single textWidth2 = 50f;
                Single textHeight2 = 20f;
                screenPos.x = Mathf.Clamp(screenPos.x, cameraMinX + textWidth2, cameraMaxX - textWidth2);
                screenPos.y = Mathf.Clamp(screenPos.y, cameraMinY + textHeight2, cameraMaxY - textHeight2);
            }
            Vector3 worldPos = this.uiCam.ScreenToWorldPoint(screenPos);
            this.myTrans.position = new Vector3(worldPos.x, worldPos.y, 0f);
            this.myTrans.localPosition += this.UIOffset;
        }
    }

    public Camera worldCam;
    public Camera uiCam;

    public Transform target;
    public Vector3 targetTransformOffset;
    public Vector3 UIOffset;

    public Boolean updateEveryFrame;

    private Transform myTrans;

    private Vector3 lastPosition;
    private Matrix4x4 lastWorldMatrix;

    private Boolean mStart;

    [NonSerialized]
    public Boolean clampToScreen;
}
