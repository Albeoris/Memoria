using Memoria;
using System;
using System.IO;
using UnityEngine;

public class WMTweaker : Singleton<WMTweaker>
{
    public Single CloudSpeed { get; set; }

    public Single cloudOffsetY { get; set; }

    public Single skydomeOffsetY { get; set; }

    public Boolean w_frameFog { get; set; }

    public Boolean HaskEffectBlockEva
    {
        get
        {
            return this.HaskEffectBlockEva_ && ff9.IsBeeScene;
        }
    }

    public Single QuicksandScrollSpeed { get; set; }

    public Single WaterShrineScrollSpeed { get; set; }

    public Single ShadowScale { get; set; }

    protected override void Awake()
    {
        this.CloudSpeed = 0.003f;
        this.w_cameraWorldEye_Y = 0;
        this.w_cameraWorldAim_Y = 0;
        //this.skydomeOffsetY = 20f; // these cause the flickering of github#820 but for ships
        //this.cloudOffsetY = 20f;
        this.QuicksandScrollSpeed = 0.007f;
        this.WaterShrineScrollSpeed = 0.007f;
        this.ShadowScale = 1f;
        //this.SetShaderParameters();
    }

    private void Start()
    {
        //this.SetShaderParameters();
    }

    /*
    private void SetShaderParameters()
    {
        Single fogStartMul = Configuration.Worldmap.FogStartDistance / 100f;
        Single fogEndMul = Configuration.Worldmap.FogEndDistance / 100f;
        Shader.SetGlobalFloat("_FogStartDistance", this._FogStartDistance * fogStartMul);
        Shader.SetGlobalFloat("_FogEndDistance", this._FogEndDistance * fogEndMul);
    }
    */

    [ContextMenu("Reset Camera Matrix")]
    private void ResetCameraMatrix()
    {
        ff9.w_frameCameraPtr.ResetWorldToCameraMatrix();
    }

    private void SaveScreenShot(KeyCode key, String fileName)
    {
        if (Input.GetKeyDown(key))
        {
            Int32 width = Screen.width;
            Int32 height = Screen.height;
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture2D.ReadPixels(new Rect(0f, 0f, (Single)width, (Single)height), 0, 0, false);
            texture2D.Apply();
            Byte[] bytes = texture2D.EncodeToPNG();
            String path = "D:/ " + fileName + ".png";
            File.WriteAllBytes(WMUtils.EnsurePath(path), bytes);
        }
    }

    public Int32 w_cameraDebugDist = 4096;

    public Boolean CustomCamera;

    public Int32 w_cameraWorldEye_Y;

    public Int32 w_cameraWorldAim_Y;

    public Int32 FixTypeCamEyeY;

    public Int32 FixTypeCamAimY;

    public Int32 denominator = 5;

    public Int32 FixTypeCamEyeYCurrent;

    public Int32 FixTypeCamEyeYPrevious;

    public Int32 FixTypeCamEyeYTarget;

    public Int32 FixTypeCamAimYCurrent;

    public Int32 FixTypeCamAimYPrevious;

    public Int32 FixTypeCamAimYTarget;

    public Int32 w_frameScenePtr;

    public Boolean HaskEffectBlockEva_ = true;

    public Single _FogStartDistance = 47.9f;

    public Single _FogEndDistance = 59f;

    public Int32 TreeRadius = 2500;

    public Int32 testIndex;
}
