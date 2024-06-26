using System;
using UnityEngine;
using Object = System.Object;

internal class MaskFrame
{
    public override String ToString()
    {
        return String.Concat(new Object[]
        {
            "\"frame\":{",
            this.frame.x,
            ", ",
            this.frame.y,
            ", ",
            this.frame.z,
            ", ",
            this.frame.w,
            "}"
        });
    }

    public Vector4 frame;

    public Vector4 sourceSize;

    public Int32 sheetID;
}
