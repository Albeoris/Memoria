using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Snapshot Point")]
[ExecuteInEditMode]
public class UISnapshotPoint : MonoBehaviour
{
    private void Start()
    {
        if (base.tag != "EditorOnly")
        {
            base.tag = "EditorOnly";
        }
    }

    public Boolean isOrthographic = true;

    public Single nearClip = -100f;

    public Single farClip = 100f;

    [Range(10f, 80f)]
    public Int32 fieldOfView = 35;

    public Single orthoSize = 30f;

    public Texture2D thumbnail;
}
