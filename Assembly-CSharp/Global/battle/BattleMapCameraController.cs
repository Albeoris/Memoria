using Memoria;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Scripts;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BattleMapCameraController : MonoBehaviour
{
    public void SetDefaultPsxCamera0()
    {
        this.mainCam.transform.position = new Vector3(-2200f, 2900f, 4600f);
        this.mainCam.transform.rotation = Quaternion.Euler(new Vector3(25f, 155f, 0f));
        this.mainCam.transform.localScale = new Vector3(1f, -1f, 1f);
        this.mainCam.nearClipPlane = 400f;
        this.mainCam.farClipPlane = 16383f;
    }

    public void SetDefaultPsxCamera1()
    {
        this.mainCam.transform.position = new Vector3(-2100f, 900f, -4500f);
        this.mainCam.transform.rotation = Quaternion.Euler(new Vector3(3f, 30f, 0f));
        this.mainCam.transform.localScale = new Vector3(1f, -1f, 1f);
        this.mainCam.nearClipPlane = 360f;
        this.mainCam.farClipPlane = 16383f;
    }

    public void SetDefaultPsxCamera2()
    {
        this.mainCam.transform.position = new Vector3(0f, 1000f, -4500f);
        this.mainCam.transform.rotation = Quaternion.Euler(new Vector3(10f, 0f, 0f));
        this.mainCam.transform.localScale = new Vector3(1f, -1f, 1f);
        this.mainCam.nearClipPlane = 300f;
        this.mainCam.farClipPlane = 16383f;
    }

    private void SetDefaultCamera(Int32 camID)
    {
        switch (camID)
        {
            case 0:
                this.SetDefaultPsxCamera0();
                break;
            case 1:
                this.SetDefaultPsxCamera1();
                break;
            case 2:
                this.SetDefaultPsxCamera2();
                break;
            default:
                global::Debug.Log("Default Camera ID " + camID + " not found");
                break;
        }
    }

    public void SetNextDefaultCamera()
    {
        this.defaultCamID++;
        if (this.defaultCamID >= 3)
        {
            this.defaultCamID = 0;
        }
        this.SetDefaultCamera(this.defaultCamID);
    }

    public void SetPrevDefaultCamera()
    {
        this.defaultCamID--;
        if (this.defaultCamID < 0)
        {
            this.defaultCamID = 2;
        }
        this.SetDefaultCamera(this.defaultCamID);
    }

	public Int32 GetCurrDefaultCamID()
	{
		return this.defaultCamID;
	}
    
    private void Awake()
    {
		this.defaultCamID = UnityEngine.Random.Range(0, 3);
		this.psxCam = new PsxCamera();
		this.mainCam = base.GetComponent<Camera>();
		this.rainRenderer = base.GetComponent<BattleRainRenderer>();
		this.SetDefaultCamera(this.defaultCamID);
        this.mainCam.hdr = true;
    }

    private float myTime;
    
	private void Update()
	{
        if (!this.mainCam.hdr)
        {
            this.mainCam.hdr = true;
        }

        myTime += 0.016f;
        Vector3 _MainLightDirection = new Vector3(-0.6f, 1, 0.6f);
        Vector3 _EnemyMainLightDirection = new Vector3(-0.6f, 2f, 0.6f);
        float t = 0.06f * Time.unscaledTime;
        float t2 =  0.06f * Time.unscaledTime + 0.5f;
        float alphaX = t - Mathf.FloorToInt(t)-0.5f;
        float alphaY = t2 - Mathf.FloorToInt(t2) - 0.5f;
        Shader.SetGlobalFloat("_AlphaX", alphaX);
        Shader.SetGlobalFloat("_AlphaY", alphaY);
        Shader.SetGlobalVector("_FixedWorldSpaceCameraPos", new Vector3(0f, 1000f, -4500f));
        Shader.SetGlobalVector("_MainLightDirection", _MainLightDirection);
        Shader.SetGlobalVector("_EnemyMainLightDirection", _EnemyMainLightDirection);
	}

    private void LateUpdate()
    {
        SFX.LateUpdatePlugin();
    }

    private void OnPostRender()
    {
        this.rainRenderer.nf_BbgRain();
        SFX.PostRender();
        UnifiedBattleSequencer.LoopRender();
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        PSXTextureMgr.PostBlur(src, dest);
    }

    private void OnGUI()
    {
        SFX.DebugOnGUI();
    }

    private const Int32 numDefaultCam = 3;

    public Camera mainCam;

    public PsxCamera psxCam;

    private Int32 defaultCamID;

    private BattleRainRenderer rainRenderer;
}
