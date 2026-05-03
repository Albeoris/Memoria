using System;
using UnityEngine;

public class DragOneDirectionScrollView : UIScrollView
{
    public override void MoveRelative(Vector3 relative)
    {
        if (relative.y < 0f)
        {
            relative.y = 0f;
        }
        base.MoveRelative(relative);
    }
}
