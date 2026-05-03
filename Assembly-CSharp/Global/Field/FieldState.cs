using System;
using UnityEngine;

public class FieldState : MonoBehaviour
{
    private void Awake()
    {
        this.Init();
    }

    public void SetTwistAD(Int32 twistAnalog, Int32 twistDigital)
    {
        Single x = (Single)(twistAnalog + 1) / 256f * 360f;
        Single y = (Single)(twistDigital + 1) / 256f * 360f;
        this.twist.x = x;
        this.twist.y = y;
    }

    public void Init()
    {
        this.SceneName = "FBG_N00_TSHP_MAP002_TH_CGR_1";
        this.index = 0;
        this.startPos = Vector3.zero;
        this.startRot = 0f;
        this.twist = Vector2.zero;
        this.UseUpscalFM = true;
        this.isOpenFieldMapDebugPanel = true;
        this.isEncount = true;
        this.isDebug = true;
        this.isDebugWalkMesh = false;
        this.FF9Field = new FF9StateFieldSystem();
        PersistenSingleton<FF9StateSystem>.Instance.mode = 1;
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        ff.fldMapNo = 100; // Alexandria/Main Street
        ff.fldLocNo = 33;
    }

    public String SceneName;

    public Int32 index;

    public Vector3 startPos;

    public Single startRot;

    public Vector2 twist;

    public Boolean UseUpscalFM;

    public Boolean isOpenFieldMapDebugPanel;

    public Boolean isEncount;

    public Boolean isDebug;

    public Boolean isDebugWalkMesh;

    public FF9StateFieldSystem FF9Field;
}
