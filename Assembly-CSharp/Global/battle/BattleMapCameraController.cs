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

    private Material _postEffectMat;
    private RenderTexture m_dummyRT;
    private RenderTexture _SsaoRT;
    private Matrix4x4 viewMatrix;
    private Matrix4x4 projectionMatrix;
    private Material copyDepthMat;
    private Texture2D _blueNoise;
    int _CustomTempRT;
    
    private static ControlPanel infoPanel;
    
    private static Int32 SSAOPanel;
    private float _aoStrength =1.32f;
    private float _aoPower =1.77f;
    private float _aoRadius = 90f;
    private float _aoFallOffset = 1.28f;
    private bool FF9_ENABLE_SSAO = false;
    private static void SetScalingFactor(Single factor)
    {
    }
    
    private void SetupGraphicDebugMenu()
    {
        infoPanel = new ControlPanel(PersistenSingleton<UIManager>.Instance.transform, "Graphics Panel");
        infoPanel.BasePanel.SetRect(-300f, 0f, 600f, 580f);
        SSAOPanel = infoPanel.CreateSubPanel("SSAO");
        infoPanel.PanelAddRow(SSAOPanel);
        var aoStrength = infoPanel.AddSlider("_aoStrength", 1f, val => ControlSlider.LinearScaleOut(val, 0, 3.5f),
            t => ControlSlider.LinearScaleOut(t, 0, 3.5f), (val) => { _aoStrength = val;} ,SSAOPanel);
        infoPanel.PanelAddRow(SSAOPanel);
        var aoPower = infoPanel.AddSlider("_aoPower", 1f, val => ControlSlider.LinearScaleOut(val, 0.1f, 3.5f),
            t => ControlSlider.LinearScaleOut(t, 0, 3.5f), (val) => { _aoPower = val;} ,SSAOPanel);
        infoPanel.PanelAddRow(SSAOPanel);
        var aoRadius = infoPanel.AddSlider("_aoRadius", 90f, val => ControlSlider.LinearScaleOut(val, 50f, 200f),
            t => ControlSlider.LinearScaleOut(t, 50f, 200f), (val) => { _aoRadius = val;} ,SSAOPanel);
        infoPanel.PanelAddRow(SSAOPanel);
        var aoFalloff = infoPanel.AddSlider("_aoFallOffset", 1f, val => ControlSlider.LinearScaleOut(val, 1f, 30f),
            t => ControlSlider.LinearScaleOut(t, 1f, 30f), (val) => { _aoFallOffset = val;} ,SSAOPanel);
        infoPanel.PanelAddRow(SSAOPanel);
        infoPanel.EndInitialization(UIWidget.Pivot.BottomRight);
        infoPanel.SetActivePanel(true, SSAOPanel);
        infoPanel.Show = true;
    }
    
    private void Awake()
    {
        FF9_ENABLE_SSAO = Configuration.Graphics.EnableSSAO == 1;
        //SetupGraphicDebugMenu();
		this.defaultCamID = UnityEngine.Random.Range(0, 3);
		this.psxCam = new PsxCamera();
		this.mainCam = base.GetComponent<Camera>();
		this.rainRenderer = base.GetComponent<BattleRainRenderer>();
		this.SetDefaultCamera(this.defaultCamID);
        Byte[] raw = File.ReadAllBytes("StreamingAssets/Assets/Resources/Textures/BlueNoise470.png");
        _blueNoise = AssetManager.LoadTextureGeneric(raw);
        this._postEffectMat = new Material(ShadersLoader.Find("PSX/PostEffect"));
        this.copyDepthMat = new Material(ShadersLoader.Find("PSX/CopyGlobalDepth"));
        if (FF9_ENABLE_SSAO)
        {
            if (m_dummyRT == null)
                m_dummyRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
            if (_SsaoRT == null)
                _SsaoRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
        
            _CustomTempRT = Shader.PropertyToID("_tempRT");
            var commandBufferDrawDepth = new CommandBuffer { name = "Render to Custom Depth Texture" };
            commandBufferDrawDepth.SetGlobalTexture("_CustomViewZMap", m_dummyRT);
            commandBufferDrawDepth.GetTemporaryRT(_CustomTempRT, Screen.width, Screen.height);
            commandBufferDrawDepth.Blit(_CustomTempRT, _SsaoRT, _postEffectMat);
            commandBufferDrawDepth.SetGlobalTexture("_SSAO", _SsaoRT);
            commandBufferDrawDepth.ReleaseTemporaryRT(_CustomTempRT);
            this.mainCam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBufferDrawDepth);   
        }
        else
        {
            if (_SsaoRT == null)
                _SsaoRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
           
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = _SsaoRT;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = rt;
            Shader.SetGlobalTexture("_SSAO", _SsaoRT);
        }
    }
    private void CheckSupport(RenderTextureFormat format)
    {
        Log.Message(format + "support status  = "+SystemInfo.SupportsRenderTextureFormat(format));
    }
	private void Update()
	{
        Vector3 _MainLightDirection = new Vector3(-0.6f, 1, 0.6f);
        Vector3 _EnemyMainLightDirection = new Vector3(-0.6f, 2f, 0.6f);
        Shader.SetGlobalVector("_MainLightDirection", _MainLightDirection);
        Shader.SetGlobalVector("_EnemyMainLightDirection", _EnemyMainLightDirection);
        if (FF9_ENABLE_SSAO)
        {
            var originFar = this.mainCam.farClipPlane;
            var originNear = this.mainCam.nearClipPlane; 
            var originRT = this.mainCam.targetTexture != null ? this.mainCam.targetTexture : null;

            this.mainCam.targetTexture = m_dummyRT;
            this.mainCam.Render();
            this.mainCam.targetTexture = originRT;
            viewMatrix = this.mainCam.worldToCameraMatrix;
            projectionMatrix =  GL.GetGPUProjectionMatrix(this.mainCam.projectionMatrix, false);
        
            this.mainCam.farClipPlane = originFar;
            this.mainCam.nearClipPlane = originNear;
        }
	}

	private void LateUpdate()
	{
		SFX.LateUpdatePlugin();
	}

    private void OnPreRender()
    {
        if (FF9_ENABLE_SSAO)
        {
            if (this.mainCam.targetTexture == m_dummyRT)
            {
                this.mainCam.depthTextureMode = DepthTextureMode.Depth;
                // Combine the view and projection matrices
                Matrix4x4 viewProjectionMatrix = projectionMatrix * viewMatrix;
                // Invert the combined matrix to get the inverse view projection matrix
                Matrix4x4 inverseViewProjectionMatrix = viewProjectionMatrix.inverse;
                //_postEffectMat.SetMatrix("_InvVP", inverseViewProjectionMatrix);
                _postEffectMat.SetMatrix("_InvP",  projectionMatrix.inverse);
                _postEffectMat.SetTexture("_BlueNoise", _blueNoise);
                _postEffectMat.SetFloat("_aoStrength", _aoStrength);
                _postEffectMat.SetFloat("_aoPower", _aoPower);
                _postEffectMat.SetFloat("_radius", _aoRadius);
                _postEffectMat.SetFloat("_bias", 0.1f);
                _postEffectMat.SetFloat("_debug", 1);
                _postEffectMat.SetFloat("_fallOffset", _aoFallOffset);
                //_postEffectMat.SetTexture("_CustomViewZMap", m_dummyRT);
            }
            else
            {
                this.mainCam.depthTextureMode = DepthTextureMode.DepthNormals;
            }   
        }
    }

    private void OnPostRender()
	{
        if (FF9_ENABLE_SSAO)
        {
            if (this.mainCam.targetTexture == m_dummyRT)
            {
                return;
            }
        }

        this.rainRenderer.nf_BbgRain();
		SFX.PostRender();
		UnifiedBattleSequencer.LoopRender();
	}
    

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
        if (FF9_ENABLE_SSAO)
        {
            if (dest == m_dummyRT)
            {
                Graphics.Blit(src, dest, copyDepthMat);
                return;
            }
        }
        PSXTextureMgr.PostBlur(src, dest);
	}

	private void OnGUI()
	{
        
        /*if (!_SsaoRT)
            
        {
            return;
        }*/
        //GUI.DrawTexture(new Rect(0, 0, 512, 512), _SsaoRT, ScaleMode.ScaleToFit, false);
        //GUI.DrawTexture(new Rect(530, 0, 512, 512), m_dummyRT, ScaleMode.ScaleToFit, false);
        //GUI.DrawTexture(new Rect(530, 0, 512, 512), m_dummyRT, ScaleMode.ScaleToFit, false);
		SFX.DebugOnGUI();
	}

	private const Int32 numDefaultCam = 3;

	public Camera mainCam;

	public PsxCamera psxCam;

	private Int32 defaultCamID;

	private BattleRainRenderer rainRenderer;
}
