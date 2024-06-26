using System;
using UnityEngine;

public class BubbleMappingInfo
{
    public static void GetActorInfo(PosObj po, out Transform bone, out Vector3 offset)
    {
        bone = (Transform)null;
        offset = Vector3.zero;
        bone = po.go.transform;
        offset = new Vector3(0f, (Single)((-po.eye * (Int16)po.scaley >> 6) + 50), 0f);
    }
}
