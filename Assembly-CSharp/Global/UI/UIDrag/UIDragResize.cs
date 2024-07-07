using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag-Resize Widget")]
public class UIDragResize : MonoBehaviour
{
    private void OnDragStart()
    {
        if (this.target != (UnityEngine.Object)null)
        {
            Vector3[] worldCorners = this.target.worldCorners;
            this.mPlane = new Plane(worldCorners[0], worldCorners[1], worldCorners[3]);
            Ray currentRay = UICamera.currentRay;
            Single distance;
            if (this.mPlane.Raycast(currentRay, out distance))
            {
                this.mRayPos = currentRay.GetPoint(distance);
                this.mLocalPos = this.target.cachedTransform.localPosition;
                this.mWidth = this.target.width;
                this.mHeight = this.target.height;
                this.mDragging = true;
            }
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (this.mDragging && this.target != (UnityEngine.Object)null)
        {
            Ray currentRay = UICamera.currentRay;
            Single distance;
            if (this.mPlane.Raycast(currentRay, out distance))
            {
                Transform cachedTransform = this.target.cachedTransform;
                cachedTransform.localPosition = this.mLocalPos;
                this.target.width = this.mWidth;
                this.target.height = this.mHeight;
                Vector3 b = currentRay.GetPoint(distance) - this.mRayPos;
                cachedTransform.position += b;
                Vector3 vector = Quaternion.Inverse(cachedTransform.localRotation) * (cachedTransform.localPosition - this.mLocalPos);
                cachedTransform.localPosition = this.mLocalPos;
                NGUIMath.ResizeWidget(this.target, this.pivot, vector.x, vector.y, this.minWidth, this.minHeight, this.maxWidth, this.maxHeight);
                if (this.updateAnchors)
                {
                    this.target.BroadcastMessage("UpdateAnchors");
                }
            }
        }
    }

    private void OnDragEnd()
    {
        this.mDragging = false;
    }

    public UIWidget target;

    public UIWidget.Pivot pivot = UIWidget.Pivot.BottomRight;

    public Int32 minWidth = 100;

    public Int32 minHeight = 100;

    public Int32 maxWidth = 100000;

    public Int32 maxHeight = 100000;

    public Boolean updateAnchors;

    private Plane mPlane;

    private Vector3 mRayPos;

    private Vector3 mLocalPos;

    private Int32 mWidth;

    private Int32 mHeight;

    private Boolean mDragging;
}
