using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag and Drop Container")]
public class UIDragDropContainer : MonoBehaviour
{
    protected virtual void Start()
    {
        if (this.reparentTarget == (UnityEngine.Object)null)
        {
            this.reparentTarget = base.transform;
        }
    }

    public Transform reparentTarget;
}
