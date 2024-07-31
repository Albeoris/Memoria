using Assets.Scripts.Common;
using Memoria;
using Memoria.Scripts;
using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeStaticMemberQualifier

public class SFX_Rush
{
    private static readonly Int32 StartFadeOutFrame = Configuration.Graphics.BattleSwirlFrames;
    private static readonly Int32 RushParamLastFrame = StartFadeOutFrame + 5;

    private static Single _px;
    private static Single _py;
    private static Texture2D _result;

    private readonly RenderTexture[] _texture;
    private readonly Boolean _isRandomEncounter;
    private readonly Single _addColDec;
    private readonly Single _subColDec;
    private readonly Single _rotInc;

    private Int32 _rushSeq;
    private Single _addCol;
    private Single _subCol;
    private Single _rot;
    private Single _scale;
    private Single _scaleAdd;
    private Boolean _isUpdate;

    public SFX_Rush()
    {
        _isRandomEncounter = FF9StateSystem.Battle.isRandomEncounter;
        _rot = 0.0f;
        _rotInc = 0.03926991f;
        _subCol = 0.0f;
        _scale = 1f;

        if (!_isRandomEncounter)
        {
            _addCol = 0.1f;
            _addColDec = 1.0f / 500.0f;
            _subColDec = 0.0008f;
            _scaleAdd = -0.008f;
        }
        else
        {
            _addCol = 0.1f;
            _addColDec = 1.0f / 500.0f;
            _subColDec = 0.0008f;
            _scaleAdd = 3.0f / 500.0f;
        }

        if (Random.value > 0.5)
            _rotInc *= -1f;

        Rect screenSize = GetScreenSize();
        _texture = new RenderTexture[2];
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.MultMatrix(Matrix4x4.Scale(new Vector3(1f, 2f, 1f)));
        GL.Viewport(new Rect(0.0f, 0.0f, Screen.width, Screen.height));
        FilterMode filterMode = FilterMode.Point;
        if (Configuration.Graphics.SFXSmoothTexture == 1) filterMode = FilterMode.Bilinear;
        if (Configuration.Graphics.SFXSmoothTexture == 2) filterMode = FilterMode.Trilinear;
        for (Int32 index = 0; index < 2; ++index)
        {
            _texture[index] = new RenderTexture((Int32)screenSize.width, (Int32)screenSize.height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = false,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = filterMode,
                name = "Rush_RT_" + index
            };
            _texture[index].Create();
            Graphics.Blit(_result, _texture[index]);
        }

        GL.PopMatrix();
        SceneDirector.FF9Wipe_FadeOutEx(5);
    }

    public void ReleaseRenderTarget()
    {
        for (Int32 index = 0; index < 2; ++index)
        {
            if (_texture[index] != null)
            {
                _texture[index].Release();
                _texture[index] = null;
            }
        }
    }

    public Boolean update()
    {
        if (_rushSeq++ > RushParamLastFrame)
            return true;
        if (_rushSeq > StartFadeOutFrame)
            SceneDirector.ServiceFade();
        _isUpdate = true;
        return false;
    }

    public void PostProcess(RenderTexture src, RenderTexture dest)
    {
        Int32 index1 = 0;
        Int32 index2 = 1;

        if (_isUpdate)
        {
            _isUpdate = false;

            Material mat1 = new Material(ShadersLoader.Find("SFX_RUSH_SUB"));
            mat1.SetVector("_Param", new Vector4(_rot, _scale, _subCol, 0.0f));
            Graphics.Blit(_texture[index1], _texture[index2], mat1);

            Material mat2 = new Material(ShadersLoader.Find("SFX_RUSH_ADD"));
            mat2.SetVector("_Center", new Vector4(_px, _py, 0.0f, 0.0f));
            mat2.SetVector("_Param", new Vector4(_rot, _scale, _addCol, 0.0f));
            Graphics.Blit(_texture[index1], _texture[index2], mat2);
            Graphics.Blit(_texture[index2], _texture[index1]);

            if ((_rushSeq & 1) != 0)
                _px += _px >= 0.5 ? -0.009375f : 0.009375f;

            if ((_rushSeq & 2) != 0)
                _py += _py >= 0.5 ? -0.01339286f : 0.01339286f;

            if (!_isRandomEncounter)
            {
                Single num = (Single)Math.Sin(_rushSeq * 14.0 * Math.PI / 180.0);
                _px += num * (0.5f - _px);
                _py += num * (0.5f - _py);
            }

            if (!_isRandomEncounter && _rushSeq >= 16)
            {
                _addCol -= _addColDec;
                if (_addCol < 0.0)
                    _addCol = 0.0f;
            }

            if (_rushSeq >= 1)
            {
                _subCol += _subColDec;
                if (_subCol > 1.0)
                    _subCol = 1f;
            }

            _rot += _rotInc;
            _scale += _scaleAdd;
        }

        Graphics.Blit(_texture[index1], dest);
    }

    public static Rect GetScreenSize()
    {
        if (Configuration.Graphics.WidescreenSupport)
        {
            Rect rect = new Rect
            {
                width = Screen.width,
                height = Screen.height,
                x = 0,
                y = 0
            };

            return rect;
        }
        else
        {

            Vector2 vector2 = new Vector2(FieldMap.PsxFieldWidth, FieldMap.PsxFieldHeightNative);
            if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
                vector2 = new Vector2(FieldMap.PsxScreenWidth, FieldMap.PsxScreenHeightNative);

            Single num = Mathf.Min(Screen.width / vector2.x, Screen.height / vector2.y);

            Single w = vector2.x * num;
            Single h = vector2.y * num;
            Rect rect = new Rect
            {
                width = w,
                height = h,
                x = (Single)((Screen.width - w) * 0.5),
                y = (Single)((Screen.height - h) * 0.5)
            };

            return rect;
        }
    }

    public static void CreateScreen()
    {
        Rect screenSize = GetScreenSize();
        Int32 screenX = (Int32)screenSize.x;
        Int32 screenY = (Int32)screenSize.y;
        Int32 screenW = (Int32)screenSize.width;
        Int32 screenH = (Int32)screenSize.height;
        _result = new Texture2D((Int32)screenSize.width, (Int32)screenSize.height, TextureFormat.ARGB32, false);

        Color color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        for (Int32 x = 0; x < screenW; ++x)
        {
            _result.SetPixel(x, 0, color);
            _result.SetPixel(x, screenH - 1, color);
        }

        for (Int32 y = 1; y < screenH - 1; ++y)
        {
            _result.SetPixel(0, y, color);
            _result.SetPixel(screenW - 1, y, color);
        }

        _result.ReadPixels(new Rect(screenX + 1, screenY + 1, screenW - 2, screenH - 2), 1, 1, false);
        _result.Apply();
    }

    public static void SetCenterPosition(Int32 type)
    {
        switch (type)
        {
            case 0:
                Obj objUid = PersistenSingleton<EventEngine>.Instance.GetObjUID(250);
                if (objUid != null && objUid.cid == 4)
                {
                    PosObj posObj = (PosObj)objUid;
                    FieldMap fieldMap = PersistenSingleton<EventEngine>.Instance.fieldmap;
                    Camera mainCamera = fieldMap.GetMainCamera();
                    BGCAM_DEF currentBgCamera = fieldMap.GetCurrentBgCamera();
                    Vector3 position = PSXCalculateGTE_RTPT(new Vector3(posObj.pos[0], posObj.pos[1], posObj.pos[2]), Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), fieldMap.GetProjectionOffset());
                    Vector3 vector3 = mainCamera.WorldToScreenPoint(position);
                    vector3.x /= mainCamera.pixelWidth;
                    vector3.y /= mainCamera.pixelHeight;
                    _px = vector3.x;
                    _py = vector3.y;
                    //Debug.Log(string.Concat("px : ", _px, " , py : ", _py));
                    break;
                }
                _px = 0.5f;
                _py = 0.5f;
                break;
            case 1:
                if (ff9.w_moveActorPtr != null)
                {
                    Vector3 pos = ff9.w_moveActorPtr.pos;
                    Camera camera = ff9.w_frameCameraPtr;
                    Vector3 vector3 = camera.WorldToScreenPoint(pos);
                    vector3.x /= camera.pixelWidth;
                    vector3.y /= camera.pixelHeight;
                    _px = vector3.x;
                    _py = vector3.y;
                    //Debug.Log(string.Concat("px : ", _px, " , py : ", _py));
                    break;
                }
                _px = 0.5f;
                _py = 0.5f;
                break;
            default:
                _px = 0.5f;
                _py = 0.5f;
                break;
        }
    }

    private static Vector3 PSXCalculateGTE_RTPT(Vector3 vertex, Matrix4x4 localRTS, Matrix4x4 globalRT, Single viewDist, Vector2 offset)
    {
        Vector3 vector3 = PSXCalculateGTE_RTPT_POS(vertex, localRTS, globalRT, viewDist, offset, true);
        vector3.z = 0.0f;
        return vector3;
    }

    private static Vector3 PSXCalculateGTE_RTPT_POS(Vector3 vertex, Matrix4x4 localRTS, Matrix4x4 globalRT, Single viewDist, Vector2 offset, Boolean useAbsZ)
    {
        Vector3 v = localRTS.MultiplyPoint(vertex);
        v.y *= -1f;

        Vector3 vector3 = globalRT.MultiplyPoint(v);
        vector3.y *= -1f;
        Single num = vector3.z;
        if (useAbsZ)
            num = Mathf.Abs(vector3.z);

        vector3.x *= viewDist / num;
        vector3.y *= viewDist / num;
        vector3.x += offset.x;
        vector3.y += offset.y;
        return vector3;
    }
}
