using System;
using UnityEngine;

public class WMWorldSettings : Singleton<WMWorldSettings>
{
    private void Start()
    {
        RenderSettings.fog = this.EnableFog;
    }

    public Single FarClipPlaneForDetection = 30f;

    public Boolean CullBlocks;

    public Boolean DrawGizmos;

    public Boolean WrapWorld = true;

    public Boolean EnableFog;

    public Boolean ShowWalkMeshes;
}
