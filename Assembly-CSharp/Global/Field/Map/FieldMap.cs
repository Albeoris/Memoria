using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FF9;
using Memoria;
using Memoria.Scripts;
using Object = System.Object;
using Memoria.Prime;
using Assets.Scripts.Common;

[Flags]
public enum FieldMapFlags : uint
{
    None = 0,
    Unknown1 = 1,
    Unknown2 = 2,
    Unknown4 = 4,
    Generic7 = Unknown1 | Unknown2 | Unknown4,
    RotationScroll = 8,
    Generic15 = Unknown1 | Unknown2 | Unknown4 | RotationScroll,
    Unknown16 = 16,
    Locked = 32,
    Active = 64,
    GenericInitial = Locked | Active,
    Generic127 = Unknown128 - 1,
    Unknown128 = 128,
}

public class FieldMap : HonoBehavior
{
    public FieldMap()
    {
        this.debugObjName = "Player";
        this.debugTriIdx = -1;
        this.CharArray = new List<FF9Char>();
    }

    public bool IsCurrentFieldMapHasCombineMeshProblem()
    {
        return false;
    }

    public FieldMap.EbgCombineMeshData GetCurrentCombineMeshData()
    {
        return null;
    }

    public static String GetMapResourcePath(String mapName)
    {
        return "FieldMaps/" + mapName + "/";
    }

    private static Boolean HasAreaTitle(String mapName)
    {
        return FieldMap.fieldMapNameWithAreaTitle.Contains(mapName);
    }

    private static String GetLocalizeNameSubfix(String language)
    {
        switch (language)
        {
            case "English(UK)":
                return "_uk";
            case "English(US)":
                return "_us";
            case "German":
                return "_gr";
            case "Spanish":
                return "_es";
            case "French":
                return "_fr";
            case "Italian":
                return "_it";
            case "Japanese":
                return "_jp";
        }
        return "_us";
    }

    public static void SetFieldMapAtlasName(String mapName, out String atlasName, out String atlasAlphaName)
    {
        atlasName = "atlas";
        atlasAlphaName = "atlas_a";
        if (FieldMap.HasAreaTitle(mapName))
        {
            String text = FF9StateSystem.Settings.CurrentLanguage;
            if (text == "English(UK)" && !mapName.Equals("FBG_N16_STGT_MAP330_SG_RND_0")) // South Gate/Bohden Gate, guarded entrance
                text = "English(US)";
            String localizeNameSubfix = FieldMap.GetLocalizeNameSubfix(text);
            atlasName += localizeNameSubfix;
            atlasAlphaName += localizeNameSubfix;
        }
    }

    public override void HonoAwake()
    {
        // DEBUG
        // debugRender = true;
        // FF9StateSystem.Field.isDebugWalkMesh = true;

        this.debugTriPosObj = new GameObject("TriPosObj").transform;
        this.debugTriPosObj.position = Vector3.zero;
        GameObject goCamera = GameObject.Find("FieldMap Camera");
        if (goCamera != null)
        {
            this.mainCamera = goCamera.GetComponent<Camera>();
            this.rainRenderer = goCamera.AddComponent<FieldRainRenderer>();
        }
        this.camIdx = 0;
        //Log.Message("HonoAwake() this.camIdx = 0;");
        this.curCamIdx = -1;
        this.charAimHeight = 324;
        this.mapName = FF9StateSystem.Field.SceneName;
        this.UseUpscalFM = FF9StateSystem.Field.UseUpscalFM;
        if (this.mapName == String.Empty)
            this.mapName = "FBG_N00_TSHP_MAP001_TH_CGR_0";
        this.LoadFieldMap(this.mapName);
        HonoBehaviorSystem.FrameSkipEnabled = false;
        HonoBehaviorSystem.TargetFrameTime = 0.0333333351f;
        this.attachList = new EBG_ATTACH_DEF[10];
        this.isBattleBackupPos = false;
        this.BG_init();
        if (FF9StateSystem.Common.FF9.fldMapNo == 2507) // I. Castle/Stairwell, room with ladders and stairs
        {
            base.StartCoroutine(this.DelayedActiveTri());
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo >= 3009 && FF9StateSystem.Common.FF9.fldMapNo <= 3011)
        {
            // Ending/TH
            HonoBehaviorSystem.FrameSkipEnabled = true;
            HonoBehaviorSystem.TargetFrameTime = 0.0333333351f;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2356)
        {
            // Gulug/Room (with a Red Dragon bursting through wall)
            this.walkMesh.BGI_triSetActive(78u, 0u);
            this.walkMesh.BGI_triSetActive(79u, 0u);
            this.walkMesh.BGI_triSetActive(80u, 0u);
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2161)
        {
            // L. Castle/Guest Room (disc 3)
            this.walkMesh.BGI_triSetActive(69u, 0u);
        }
        this.fmEditor = base.gameObject.AddComponent<FieldMapEditor>();
        this.fmEditor.Init(this);
        this.debugPosMarker = new Vector3[5];
        for (Int32 i = 0; i < (Int32)this.debugPosMarker.Length; i++)
            this.debugPosMarker[i] = SettingUtils.fieldMapSettings.debugPosMarker[i];
        if (SettingUtils.fieldMapSettings.enable)
        {
            this.debugObjName = SettingUtils.fieldMapSettings.debugObjName;
            this.debugTriIdx = SettingUtils.fieldMapSettings.debugTriIdx;
        }
    }

    private IEnumerator DelayedActiveTri()
    {
        yield return new WaitForSeconds(0.5f);
        if (FF9StateSystem.Common.FF9.fldMapNo == 2507) // I. Castle/Stairwell, room with ladders and stairs
        {
            FieldMapActorController[] facs = UnityEngine.Object.FindObjectsOfType<FieldMapActorController>();
            for (Int32 i = 0; i < facs.Length; i++)
                if (!facs[i].isPlayer)
                    this.walkMesh.BGI_charSetActive(facs[i], 0u);
            this.walkMesh.BGI_triSetActive(174u, 0u);
            this.walkMesh.BGI_triSetActive(175u, 0u);
            this.walkMesh.BGI_triSetActive(177u, 0u);
            this.walkMesh.BGI_triSetActive(178u, 0u);
        }
        yield break;
    }

    public override void HonoOnDestroy()
    {
        HonoBehaviorSystem.FrameSkipEnabled = false;
        HonoBehaviorSystem.TargetFrameTime = 0.0333333351f;
    }

    public Camera GetMainCamera()
    {
        return this.mainCamera;
    }

    public BGCAM_DEF GetCurrentBgCamera()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 70 || this.curCamIdx < 0 || this.curCamIdx > this.scene.cameraList.Count)
            return null;
        return this.scene.cameraList[this.camIdx];
    }

    public Vector2 GetProjectionOffset()
    {
        return this.offset;
    }

    public Vector2 GetCameraOffset()
    {
        Camera camera = this.GetMainCamera();
        return camera.transform.localPosition;
    }

    public void ChangeFieldMap(String name)
    {
        this.mapName = name;
        this.camIdx = 0;
        this.curCamIdx = -1;
        foreach (Object obj in base.transform)
        {
            Transform transform = (Transform)obj;
            UnityEngine.Object.Destroy(transform.gameObject);
        }
        this.LoadFieldMap(this.mapName);
        this.ActivateCamera();
    }

    public void CreateBorder(Int32 overlayNdx, Byte r, Byte g, Byte b)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        Material material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_None"));
        for (Int32 i = 0; i < 4; i++)
        {
            GameObject gameObject = new GameObject("border" + i.ToString("D3"));
            gameObject.transform.parent = bgOverlay.transform;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            List<Vector3> vertList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<Int32> triList = new List<Int32>();
            vertList.Clear();
            uvList.Clear();
            triList.Clear();
            Int32 height = 0;
            Int32 width = 0;
            Int32 posX = 0;
            Int32 posY = 0;
            Int32 index = this.curCamIdx;
            switch (i)
            {
                case 0:
                    width = -this.scene.cameraList[index].vrpMinX;
                    height = bgOverlay.h;
                    posX = 0;
                    posY = height;
                    break;
                case 1:
                    width = this.scene.cameraList[index].vrpMinX;
                    height = bgOverlay.h;
                    posX = bgOverlay.w;
                    posY = height;
                    break;
                case 2:
                    width = bgOverlay.w;
                    height = this.scene.cameraList[index].vrpMinY;
                    posX = 0;
                    posY = 0;
                    break;
                case 3:
                    width = bgOverlay.w;
                    height = -this.scene.cameraList[0].vrpMinY;
                    posX = 0;
                    posY = bgOverlay.h;
                    break;
            }
            vertList.Add(new Vector3(posX, posY - height, 0f));
            vertList.Add(new Vector3(posX + width, posY - height, 0f));
            vertList.Add(new Vector3(posX + width, posY, 0f));
            vertList.Add(new Vector3(posX, posY, 0f));
            uvList.Add(new Vector2(0f, 0f));
            uvList.Add(new Vector2(1f, 0f));
            uvList.Add(new Vector2(1f, 1f));
            uvList.Add(new Vector2(0f, 1f));
            triList.Add(2);
            triList.Add(1);
            triList.Add(0);
            triList.Add(3);
            triList.Add(2);
            triList.Add(0);
            Mesh mesh = new Mesh();
            mesh.vertices = vertList.ToArray();
            mesh.uv = uvList.ToArray();
            mesh.triangles = triList.ToArray();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
            Texture2D texture2D = new Texture2D(1, 1);
            Color[] pixels = texture2D.GetPixels();
            for (Int32 j = 0; j < pixels.Length; j++)
                pixels[j] = new Color32(r, g, b, Byte.MaxValue);
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            meshRenderer.material.mainTexture = texture2D;
        }
    }

    public override void HonoUpdate()
    {
        if (this.animIdx == null || (Int32)this.animIdx.Length != this.scene.animList.Count)
        {
            this.animIdx = new Boolean[this.scene.animList.Count];
            this.frameCountList = new Int32[this.scene.animList.Count];
            for (Int32 j = 0; j < this.scene.animList.Count; j++)
            {
                this.animIdx[j] = true;
                this.frameCountList[j] = 0;
            }
        }
        this.ActivateCamera();
        this.BgAnimationService();
        this.BgAttachService();
    }

    public override void HonoLateUpdate()
    {
        this.SceneService2DScroll();
        this.SceneService3DScroll();
        if (Configuration.Graphics.InitializeWidescreenSupport())
            OnWidescreenSupportChanged();
        this.CenterCameraOnPlayer();
        this.SceneServiceScroll(this.scene);
        this.UpdateOverlayAll();
    }

    public override void HonoOnGUI()
    {
        if (this.walkMesh != null)
        {
            this.walkMesh.RenderWalkMeshNormal();
            if (this.debugTriIdx < -1)
                this.debugTriIdx = -1;
            else if (this.debugTriIdx >= this.walkMesh.tris.Count)
                this.debugTriIdx = this.walkMesh.tris.Count - 1;
            this.walkMesh.RenderWalkMeshTris(this.debugTriIdx);
            if (this.debugTriIdx != -1)
            {
                WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[this.debugTriIdx];
                this.debugTriPosObj.position = walkMeshTriangle.originalCenter;
            }
            for (Int32 i = 0; i < this.debugPosMarker.Length; i++)
                DebugUtil.DebugDrawMarker(this.debugPosMarker[i], 20f, Color.yellow);
        }
    }

    public void ff9fieldCharService()
    {
        for (ObjList objList = PersistenSingleton<EventEngine>.Instance.GetActiveObjList(); objList != null; objList = objList.next)
        {
            if (objList.obj != null)
            {
                Obj obj = objList.obj;
                if (obj is Actor)
                {
                    Actor actor = (Actor)obj;
                    if (actor.go != null)
                        actor.go.transform.localRotation = Quaternion.AngleAxis(actor.rotAngle[2], Vector3.back) * Quaternion.AngleAxis(actor.rotAngle[1], Vector3.up) * Quaternion.AngleAxis(actor.rotAngle[0], Vector3.left);
                }
            }
        }
        foreach (Int32 key in FF9StateSystem.Common.FF9.charArray.Keys)
        {
            FF9Char ff9Char = FF9StateSystem.Common.FF9.charArray[key];
            if (ff9Char != null && ff9Char.geo != null && FF9Char.ff9charptr_attr_test(ff9Char, 4096) == 0)
            {
                GeoTexAnim component = ff9Char.geo.GetComponent<GeoTexAnim>();
                if (component != null && component.geoTexAnimGetCount() >= 2 && !component.ff9fieldCharIsTexAnimActive())
                    component.geoTexAnimPlay(2);
            }
        }
        fldmcf.ff9fieldMCFService();
        if ((FF9StateSystem.Common.FF9.attr & 2u) == 0u)
        {
            for (Int32 i = 0; i < this.CharArray.Count; i++)
                this.CharArray[i].evt = null;
            Int32 charCount = FieldMap.ff9fieldCharGetActiveList(this.CharArray);
            FF9Snd.ff9fieldSoundCharService(this.CharArray, charCount);
        }
        fldchar.ff9fieldCharEffectService();
    }

    public static Int32 ff9fieldCharGetActiveList(List<FF9Char> CharArray)
    {
        Int32 result = 0;
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        for (ObjList objList = instance.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (instance.objIsVisible(obj) && obj.cid == 4)
            {
                if (result >= CharArray.Count)
                    CharArray.Add(new FF9Char());
                CharArray[result++].evt = (PosObj)obj;
            }
        }
        return result;
    }

    public void SetCurrentCameraIndex(Int32 newCamIdx)
    {
        if (this.camIdx == newCamIdx)
            return;
        SmoothFrameUpdater_Field.Skip = 1;
        this.camIdx = newCamIdx;
        this.ActivateCamera();
        this.walkMesh.ProjectedWalkMesh = this.GetCurrentBgCamera().projectedWalkMesh;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.camIdx];
        Vector2 centerOffset = bgCamera.GetCenterOffset();
        this.offset.x = centerOffset.x + bgCamera.w / 2 - HalfFieldWidth;
        if (dbug) Log.Message("SetCurrentCameraIndex(" + newCamIdx + ") | this.offset.x(" + this.offset.x + ") = centerOffset.x(" + centerOffset.x + ") + bgCamera.w(" + bgCamera.w + ") / 2 - HalfFieldWidth(" + HalfFieldWidth + ")");
        this.offset.y = -centerOffset.y - bgCamera.h / 2 + HalfFieldHeight;
        Shader.SetGlobalFloat("_OffsetX", this.offset.x);
        Shader.SetGlobalFloat("_OffsetY", this.offset.y);
        Shader.SetGlobalFloat("_MulX", ShaderMulX);
        Shader.SetGlobalFloat("_MulY", ShaderMulY);
        Shader.SetGlobalMatrix("_MatrixRT", bgCamera.GetMatrixRT());
        Shader.SetGlobalFloat("_ViewDistance", bgCamera.GetViewDistance());
        Shader.SetGlobalFloat("_DepthOffset", bgCamera.depthOffset);
        FF9StateSystem.Field.FF9Field.loc.map.charOTOffset = bgCamera.depthOffset;
        FF9StateSystem.Common.FF9.cam = bgCamera.GetMatrixRT();
        FF9StateSystem.Common.FF9.proj = bgCamera.proj;
        FF9StateSystem.Common.FF9.projectionOffset = this.offset;
        this.scene.maxX = bgCamera.vrpMaxX;
        this.scene.maxY = bgCamera.vrpMaxY;
        this.flags |= FieldMapFlags.Unknown128;
        this.walkMesh.ProcessBGI();
        this.walkMesh.UpdateActiveCameraWalkmesh();
        SmoothCamDelay = 4;
        SmoothCamActive = (!SmoothCamExcludeMaps.Contains(FF9StateSystem.Common.FF9.fldMapNo));
        String camIdxIfCam = this.scene.cameraList.Count > 1 ? "-" + this.camIdx : "";
        PlayerWindow.Instance.SetTitle($"Map: {FF9StateSystem.Common.FF9.fldMapNo}{camIdxIfCam} ({FF9StateSystem.Common.FF9.mapNameStr}) | Index/Counter: {PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR)}/{PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR)} | Loc: {FF9StateSystem.Common.FF9.fldLocNo}");
        if (dbug) Log.Message(" |_ SetCurrentCameraIndex | ShaderMulX: " + ShaderMulX + " | bgCamera.depthOffset: " + bgCamera.depthOffset + " | bgCamera.vrpMaxX " + bgCamera.vrpMaxX + " | bgCamera.depthOffset: " + bgCamera.depthOffset + " | this.scene.maxX: " + this.scene.maxX);
    }

    public static Boolean IsNarrowMap()
    {
        return NarrowMapList.IsCurrentMapNarrow((Int32)CalcPsxFieldWidth());
    }

    public void LoadFieldMap(String name)
    {
        Single loadStartTime = Time.realtimeSinceStartup;
        Transform transform = base.transform;
        transform.localScale = new Vector3(1f, 1f, 1f);
        this.scene = new BGSCENE_DEF(this.UseUpscalFM);
        this.scene.LoadEBG(this, FieldMap.GetMapResourcePath(name), name);
        this.bgi = new BGI_DEF();
        this.bgi.LoadBGI(this, FieldMap.GetMapResourcePath(name), name);
        this.ActivateCamera();
        if (FF9StateSystem.Common.FF9.fldMapNo == 70) // Opening-For FMV
            return;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.camIdx];
        Vector2 centerOffset = bgCamera.GetCenterOffset();
        this.offset.x = centerOffset.x + bgCamera.w / 2 - HalfFieldWidth;
        if (dbug) Log.Message("LoadFieldMap(" + FF9StateSystem.Common.FF9.fldMapNo + " | this.offset.x(" + this.offset.x + ") = centerOffset.x(" + centerOffset.x + ") + bgCamera.w(" + bgCamera.w + ") / 2 - HalfFieldWidth(" + HalfFieldWidth + ")");
        this.offset.y = -centerOffset.y - bgCamera.h / 2 + HalfFieldHeight;
        Shader.SetGlobalFloat("_OffsetX", this.offset.x);
        Shader.SetGlobalFloat("_OffsetY", this.offset.y);
        Shader.SetGlobalFloat("_MulX", ShaderMulX);
        Shader.SetGlobalFloat("_MulY", ShaderMulY);
        Shader.SetGlobalMatrix("_MatrixRT", bgCamera.GetMatrixRT());
        Shader.SetGlobalFloat("_ViewDistance", bgCamera.GetViewDistance());
        Shader.SetGlobalFloat("_DepthOffset", bgCamera.depthOffset);
        FF9StateSystem.Field.FF9Field.loc.map.charOTOffset = bgCamera.depthOffset;
        FF9StateSystem.Common.FF9.cam = bgCamera.GetMatrixRT();
        FF9StateSystem.Common.FF9.proj = bgCamera.proj;
        FF9StateSystem.Common.FF9.projectionOffset = this.offset;
        this.walkMesh = new WalkMesh(this);
        this.walkMesh.CreateWalkMesh();
        this.walkMesh.CreateProjectedWalkMesh();
        this.walkMesh.BGI_simInit();
        SmoothCamDelay = 4;
        SmoothCamActive = (!SmoothCamExcludeMaps.Contains(FF9StateSystem.Common.FF9.fldMapNo));
        FPSManager.DelayMainLoop(Time.realtimeSinceStartup - loadStartTime);
        if (dbug) Log.Message("_ LoadFieldMap | ShaderMulX: " + ShaderMulX + " | bgCamera.depthOffset: " + bgCamera.depthOffset + " | bgCamera.vrpMaxX " + bgCamera.vrpMaxX + " | bgCamera.depthOffset: " + bgCamera.depthOffset + " | this.scene.maxX: " + this.scene.maxX);
    }
    public static readonly HashSet<Int32> SmoothCamExcludeMaps = new HashSet<Int32>()
    {
        767, // Burmecia, the queen slides from her layer
        1754, // fast scroll on Iifa platform buggy
    };

    public void ActivateCamera()
    {
        if (this.camIdx == this.curCamIdx)
            return;
        if (this.camIdx >= this.scene.cameraList.Count)
        {
            this.camIdx = this.curCamIdx;
            return;
        }
        this.curCamIdx = this.camIdx;
        for (Int32 i = 0; i < this.scene.cameraList.Count; i++)
        {
            BGCAM_DEF bgCamera = this.scene.cameraList[i];
            bgCamera.transform.gameObject.SetActive(i == this.camIdx);
        }
    }

    public void AddPlayer()
    {
        GameObject gameObject = ModelFactory.CreateModel("Models/main/GEO_MAIN_F0_ZDN/GEO_MAIN_F0_ZDN", false);
        AnimationFactory.AddAnimToGameObject(gameObject, "GEO_MAIN_F0_ZDN");
        gameObject.name = "Player";
        gameObject.transform.parent = base.transform;
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        FieldMapActor actor = gameObject.AddComponent<FieldMapActor>();
        Boolean flag = true;
        if (flag)
        {
            GeoTexAnim geoTexAnim = gameObject.AddComponent<GeoTexAnim>();
            geoTexAnim.Load("Models/GeoTexAnim/GEO_MAIN_B0_000", 1, 1, 4);
        }
        FieldMapActorController fieldMapActorController = gameObject.AddComponent<FieldMapActorController>();
        fieldMapActorController.fieldMap = this;
        fieldMapActorController.walkMesh = this.walkMesh;
        fieldMapActorController.actor = actor;
        fieldMapActorController.isPlayer = true;
        if (FF9StateSystem.Field.isDebug)
        {
            fieldMapActorController.SetPosition(this.bgi.charPos.ToVector3(), true, true);
        }
        else
        {
            fieldMapActorController.SetPosition(FF9StateSystem.Field.startPos, true, true);
            fieldMapActorController.transform.localRotation = Quaternion.Euler(new Vector3(0f, FF9StateSystem.Field.startRot, 0f));
        }
        this.player = actor;
        this.playerController = fieldMapActorController;
        if (FF9StateSystem.Field.isDebugWalkMesh)
        {
            gameObject.transform.localScale = new Vector3(-1f, -1f, 1f);
            Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
            for (Int32 i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                renderer.material.shader = ShadersLoader.Find("Unlit/Transparent Cutout");
            }
        }
        else
        {
            gameObject.transform.localScale = new Vector3(-1f, -1f, 1f);
            Renderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<Renderer>();
            for (Int32 j = 0; j < componentsInChildren2.Length; j++)
            {
                Renderer renderer2 = componentsInChildren2[j];
                renderer2.material.shader = ShadersLoader.Find("PSX/FieldMapActor");
            }
        }
    }

    public void RestoreModels(GameObject modelGo, Actor actorOfObj)
    {
        FieldMapActorController component = modelGo.GetComponent<FieldMapActorController>();
        component.charFlags = (UInt16)actorOfObj.charFlags;
        component.activeFloor = (Int32)actorOfObj.activeFloor;
        component.activeTri = (Int32)actorOfObj.activeTri;
    }

    public void RestoreAttachModel(GameObject modelGo, Actor actorOfObj)
    {
        if (actorOfObj.attatchTargetUid != -1)
        {
            Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(actorOfObj.attatchTargetUid);
            geo.geoAttach(modelGo, objUID.go, actorOfObj.attachTargetBoneIndex);
        }
    }

    public void RestoreShadowOff(int actorUid, Actor actorOfObj)
    {
        if (actorOfObj.isShadowOff)
            ff9shadow.FF9ShadowOffField(actorUid);
    }

    private void SetCharScale(Actor actorOfObj, int sx, int sy, int sz)
    {
        int scalingFactor = 18;
        if (actorOfObj != null)
        {
            if (actorOfObj.go != null)
                geo.geoScaleSetXYZ(actorOfObj.go, sx << 24 >> scalingFactor, sy << 24 >> scalingFactor, sz << 24 >> scalingFactor);
            actorOfObj.scaley = (byte)sy;
        }
    }

    public void AddFieldChar(GameObject modelGo, Vector3 pos, Quaternion rot, Boolean isPlayer, Actor actorOfObj, Boolean needRestore = false)
    {
        modelGo.transform.parent = base.transform;
        modelGo.transform.localScale = new Vector3(1f, 1f, 1f);
        FieldMapActor fieldMapActor = FieldMapActor.CreateFieldMapActor(modelGo, actorOfObj);
        FieldMapActorController fieldMapActorController = modelGo.AddComponent<FieldMapActorController>();
        actorOfObj.fieldMapActorController = fieldMapActorController;

        FF9BattleDBHeightAndRadius.TryFindNeckBoneIndex(actorOfObj.model, ref actorOfObj.neckBoneIndex);

        if (needRestore)
            this.RestoreModels(modelGo, actorOfObj);

        fieldMapActorController.fieldMap = this;
        fieldMapActorController.walkMesh = this.walkMesh;
        fieldMapActorController.actor = fieldMapActor;
        fieldMapActorController.originalActor = actorOfObj;
        fieldMapActorController.isPlayer = isPlayer;
        fieldMapActorController.SetPosition(pos, true, !needRestore);
        fieldMapActorController.radius = actorOfObj.bgiRad * 4f;
        modelGo.transform.localRotation = rot;

        if (needRestore)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 1508) // Conde Petie/Inn
                this.RestoreAttachModel(modelGo, actorOfObj);
            if (FF9StateSystem.Common.FF9.fldMapNo == 1508 || FF9StateSystem.Common.FF9.fldMapNo == 1706) // Conde Petie/Inn or Mdn. Sari/Kitchen
                this.RestoreShadowOff(fieldMapActor.actor.uid, actorOfObj);
        }

        if (isPlayer)
        {
            this.player = fieldMapActor;
            this.playerController = fieldMapActorController;
        }
        if (FF9StateSystem.Field.isDebugWalkMesh)
        {
            modelGo.transform.localScale = new Vector3(-1f, -1f, 1f);
            Renderer[] renderers = modelGo.GetComponentsInChildren<Renderer>();
            for (Int32 i = 0; i < renderers.Length; i++)
                renderers[i].material.shader = ShadersLoader.Find("Unlit/Transparent Cutout");
        }
        else
        {
            modelGo.transform.localScale = new Vector3(-1f, -1f, 1f);
            if (actorOfObj.model == 395) // BlueMagicLight
            {
                Renderer[] renderers = modelGo.GetComponentsInChildren<Renderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                {
                    Material[] materials = renderers[i].materials;
                    for (Int32 j = 0; j < materials.Length; j++)
                        materials[j].shader = ShadersLoader.Find("PSX/Actor_Abr_1");
                }
            }
            else
            {
                Renderer[] renderers = modelGo.GetComponentsInChildren<Renderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                {
                    Material[] materials = renderers[i].materials;
                    for (Int32 j = 0; j < materials.Length; j++)
                        materials[j].shader = ShadersLoader.Find("PSX/FieldMapActor");
                }
            }
        }
        if (needRestore && FF9StateSystem.Common.FF9.fldMapNo == 1706) // Mdn. Sari/Kitchen
        {
            if (fieldMapActor.actor.uid == 4 && FF9StateSystem.Settings.CurrentLanguage == "Japanese")
                this.SetCharScale(fieldMapActor.actor, 40, 40, 40);
            else if (fieldMapActor.actor.uid == 3 || fieldMapActor.actor.uid == 5)
                this.SetCharScale(fieldMapActor.actor, 80, 80, 80);
        }
        if (FF9StateSystem.Common.FF9.fldMapNo == 2924 && fieldMapActorController.originalActor.sid == 8)
            fieldMapActor.SetRenderQueue(2000); // Crystal World (3rd area), Zidane
        else
            fieldMapActor.SetRenderQueue(-1);
        String animIdle = FF9DBAll.AnimationDB.GetValue(fieldMapActorController.originalActor.idle);
        if (fieldMapActor.GetComponent<Animation>().GetClip(animIdle) != null)
            fieldMapActor.GetComponent<Animation>().Play(animIdle);
    }

    public void updatePlayer(GameObject playerModelGo)
    {
        FieldMapActor p = playerModelGo.GetComponent<FieldMapActor>();
        FieldMapActorController pc = playerModelGo.GetComponent<FieldMapActorController>();
        pc.isPlayer = true;
        this.player = p;
        this.playerController = pc;
    }

    public void CenterCameraOnPlayer()
    {
        if (!MBG.IsNull && !MBG.Instance.IsFinished())
            return;
        Camera camera = this.GetMainCamera();
        Int16 map = FF9StateSystem.Common.FF9.fldMapNo;
        if (map == 70) // Opening-For FMV
            return;
        if (this.curCamIdx < 0 || this.curCamIdx > this.scene.cameraList.Count)
            return;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.camIdx];
        Vector3 localPosition = camera.transform.localPosition;
        float CamPositionX = bgCamera.centerOffset[0] + this.charOffset.x;
        float CamPositionY = bgCamera.centerOffset[1] - this.charOffset.y;
        //Log.Message("bgCamera.centerOffset[0] begin center camera on player " + bgCamera.centerOffset[0]);

        if (Configuration.Graphics.InitializeWidescreenSupport() && map != 931)
        {
            Int32 mapWidth = NarrowMapList.MapWidth(map);

            Int32 threshmargin = Math.Min((Int32)bgCamera.w - PsxFieldWidth, 0); // Offset value for fields that are between 320 & 398
            //if (dbug) Log.Message("PsxFieldWidth" + PsxFieldWidth);
            if (mapWidth > PsxFieldWidth && map != 507) // Cargo Ship/Deck
            {
                foreach (KeyValuePair<Int32, Int32> entry in NarrowMapList.mapCameraMargin)
                    if (map == entry.Key)
                        threshmargin = entry.Value;

                Int32 threshright = bgCamera.w - PsxFieldWidth - threshmargin;

                if (map == 103 || map == 1853 || map == 2053 || map == 2606) // Exceptions in alex center, branbal
                    threshmargin += 16;
                else if (map == 2903) // Exception in memoria castle
                    threshright -= 32;
                else if (map == 2923) // Exception in crystal world
                    threshmargin += 20;

                CamPositionX = (float)Math.Max(threshmargin, CamPositionX);
                CamPositionX = (float)Math.Min(threshright, CamPositionX);
            }
            else if (map == 1205 || map == 1652 || map == 2552 || map == 154 || map == 1215 || map == 1807) // A. Castle/Chapel, Iifa Tree/Roots, Earth Shrine/Interior, Alex grand hall
            {
                if (map == 1652 && this.camIdx == 0) // Iifa Tree/Roots
                    threshmargin += 16;

                Int32 threshright = bgCamera.w - PsxFieldWidth - threshmargin;

                CamPositionX = (float)Math.Max(threshmargin, CamPositionX);
                CamPositionX = (float)Math.Min(threshright, CamPositionX);
            }
            else if (IsNarrowMap())
            {
                if (mapWidth <= PsxFieldWidth && mapWidth > 320)
                {
                    CamPositionX = (float)((bgCamera.w - mapWidth) / 2);
                }
            }
            if (map == 456 || map == 505 || map == 1153) // scenes extended left or right despite scrolling sky
            {
                switch (map) // offsets for scrolling maps stretched to WS
                {
                    case 456: // Dali Mountain/Summit
                        CamPositionX = 160;
                        break;
                    case 505: // Cargo ship offset
                        CamPositionX = 105;
                        break;
                    case 1153: // Rose Rouge cockpit offset
                        CamPositionX = 175;
                        break;
                    default:
                        break;
                }
                if (Configuration.Graphics.ScreenIs16to10())
                {
                    switch (map) // offsets for scrolling maps stretched to WS
                    {
                        case 456: // Dali Mountain/Summit
                            CamPositionX = CamPositionX + 35;
                            break;
                        case 505: // Cargo ship offset
                            CamPositionX = CamPositionX - 35;
                            break;
                        case 1153: // Rose Rouge cockpit offset
                            CamPositionX = CamPositionX - 35;
                            break;
                        default:
                            break;
                    }
                }
            }
            
        }

        if (SmoothCamActive)
        {
            if (SmoothCamDelay <= 0)
            {
                SmoothCamDelta.x = (Prev_CamPositionX - CamPositionX) * (float)(SmoothCamPercent() / 100f);
                CamPositionX += SmoothCamDelta.x;
                Prev_CamPositionX = CamPositionX;

                SmoothCamDelta.y = (Prev_CamPositionY - CamPositionY) * (float)(SmoothCamPercent() / 100f);
                CamPositionY += SmoothCamDelta.y;
                Prev_CamPositionY = CamPositionY;
            }
            else
            {
                SmoothCamDelay -= 1;
                Prev_CamPositionX = CamPositionX;
                Prev_CamPositionY = CamPositionY;
            }
        }
            
        localPosition.x = CamPositionX;
        localPosition.y = CamPositionY;

        if (dbug) Log.Message("CenterCameraOnPlayer | X: " + CamPositionX + " Y:" + CamPositionY);

        camera.transform.localPosition = localPosition;
    }

    public void ff9fieldInternalBattleEncountService()
    {
        FF9StateFieldSystem ff9Field = FF9StateSystem.Field.FF9Field;
        FF9StateFieldMap map = ff9Field.loc.map;
        if (map.encStatus == 0)
        {
            FF9StateFieldMap ff9StateFieldMap = map;
            ff9StateFieldMap.encStatus = (Byte)(ff9StateFieldMap.encStatus + 1);
            global::Debug.Log("battle if");
        }
        else
        {
            global::Debug.Log("battle else");
            if ((ff9Field.loc.attr & 1) == 0)
            {
                ff9Field.loc.attr |= 1;
            }
        }
        if ((ff9Field.loc.attr & 1) != 0)
        {
            FF9StateSystem.Common.FF9.attr &= 4294967168u;
            FF9StateSystem.Field.FF9Field.attr &= 4294960870u;
            PersistenSingleton<FF9StateSystem>.Instance.attr |= 2u;
        }
        if (dbug) Log.Message("ff9fieldInternalBattleEncountStart");
    }

    public void ff9fieldInternalBattleEncountStart()
    {
        FF9StateFieldSystem ff9Field = FF9StateSystem.Field.FF9Field;
        FF9StateFieldMap map = ff9Field.loc.map;
        map.encStatus = 0;
        FieldMap.FF9FieldAttr.field[1, 3] = 4;
        FieldMap.FF9FieldAttr.ff9[0, 0] = 67;
        FieldMap.FF9FieldAttr.ff9[0, 2] = 60;
        FieldMap.FF9FieldAttr.field[0, 2] = 6425;
        if (dbug) Log.Message("ff9fieldInternalBattleEncountStart");
    }

    private void BG_init()
    {
        // EBG_stateInit();
        this.flags = FieldMapFlags.GenericInitial;
        this.curVRP = Vector2.zero;
        this.charOffset = Vector2.zero;
        this.startPoint = Vector2.zero;
        this.endPoint = Vector2.zero;
        this.SmoothCamDelta = Vector2.zero;
        this.curFrame = 0;
        this.prevScr = Vector2.zero;
        this.charAimHeight = 324;

        if (FF9StateSystem.Common.FF9.fldMapNo == 70) // Opening-For FMV)
            return;

        // EBG_stateInit()
        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
        Single centerOffsetX = (Single)((bgCamera.vrpMinX + bgCamera.vrpMaxX) / 2 - bgCamera.centerOffset[0]) - HalfFieldWidth;
        Single centerOffsetY = (Single)((bgCamera.vrpMinY + bgCamera.vrpMaxY) / 2 + bgCamera.centerOffset[1]) - HalfFieldHeight;
        this.parallaxOrg.x = centerOffsetX;
        this.curVRP.x = centerOffsetX;
        this.parallaxOrg.y = centerOffsetY;
        this.curVRP.y = centerOffsetY;
        this.scrollWindowPos = new Int16[4][];
        this.scrollWindowDim = new Int16[4][];
        this.scrollWindowAlphaX = new Int16[4];
        this.scrollWindowAlphaY = new Int16[4];
        for (Int32 i = 0; i < 4; i++)
        {
            this.scrollWindowPos[i] = new Int16[2];
            this.scrollWindowPos[i][0] = 0;
            this.scrollWindowPos[i][1] = 0;
            this.scrollWindowDim[i] = new Int16[2];
            this.scrollWindowDim[i][0] = bgCamera.w;
            this.scrollWindowDim[i][1] = bgCamera.h;
            this.scrollWindowAlphaX[i] = 256;
            this.scrollWindowAlphaY[i] = 256;
        }
        // EBG_animationInit();
        List<BGANIM_DEF> animList = this.scene.animList;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        Int32 animCount = this.scene.animCount;
        for (Int32 i = 0; i < animCount; i++)
        {
            BGANIM_DEF bgAnim = animList[i];
            bgAnim.curFrame = 0;
            bgAnim.frameRate = 256;
            bgAnim.counter = 0;
            bgAnim.flags = BGANIM_DEF.ANIM_FLAG.SingleFrame;
            List<BGANIMFRAME_DEF> frameList = bgAnim.frameList;
            overlayList[frameList[0].target].SetFlags(BGOVERLAY_DEF.OVERLAY_FLAG.Active, true);
        }
        // EBG_attachInit();
        this.attachCount = 0;
        for (Int32 i = 0; i < 10; i++)
        {
            this.attachList[i] = new EBG_ATTACH_DEF();
            this.attachList[i].surroundMode = -1;
            this.attachList[i].r = 128;
            this.attachList[i].g = 128;
            this.attachList[i].b = 128;
            this.attachList[i].ndx = -1;
            this.attachList[i].x = 0;
            this.attachList[i].y = 0;
        }
        if (dbug) Log.Message("EBG_init()");
    }

    public Int32 EBG_sceneGetVRP(ref Int16 x, ref Int16 y)
    {
        if (this.scene.cameraList.Count <= 0)
            return 0;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
        x = (Int16)(this.curVRP.x + bgCamera.centerOffset[0] + HalfFieldWidth);
        y = (Int16)(this.curVRP.y - bgCamera.centerOffset[1] + HalfFieldHeight);
        //Log.Message("EBG_sceneGetVRP curVRP" + x);
        return 1;
    }

    public Int32 EBG_overlaySetActive(Int32 overlayNdx, Int32 activeFlag)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        bgOverlay.SetFlags(BGOVERLAY_DEF.OVERLAY_FLAG.Active, activeFlag != 0);
        return 1;
    }

    public Int32 EBG_overlayDefineViewport(Int32 viewportNdx, Int16 x, Int16 y, Int16 w, Int16 h)
    {
        if (dbug) Log.Message("EBG_overlayDefineViewport " + viewportNdx + " | x:" + x + " y:" + y + " w:" + w + " h:" + h);

        if (!(viewportNdx >= 0 && viewportNdx < 4))
            viewportNdx = 0;

        this.scrollWindowPos[viewportNdx][0] = x;
        this.scrollWindowPos[viewportNdx][1] = y;
        this.scrollWindowDim[viewportNdx][0] = w;
        this.scrollWindowDim[viewportNdx][1] = h;
        return 1;
    }

    public Int32 EBG_overlayDefineViewportAlpha(Int32 viewportNdx, Int32 alphaX, Int32 alphaY)
    {
        if (dbug) Log.Message("EBG_overlayDefineViewportAlpha " + viewportNdx + " | alphaX:" + alphaX + " alphaY:" + alphaY);
        this.scrollWindowAlphaX[viewportNdx] = (Int16)alphaX;
        this.scrollWindowAlphaY[viewportNdx] = (Int16)alphaY;
        return 1;
    }

    public Int32 EBG_overlaySetViewport(Int32 overlayNdx, Int32 viewportNdx)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        bgOverlay.viewportNdx = (Byte)viewportNdx;

        if (dbug) Log.Message("EBG_overlaySetViewport | overlayNdx:" + overlayNdx + " viewportNdx:" + bgOverlay.viewportNdx);
        return 1;
    }

    public Int32 EBG_overlaySetLoop(Int32 overlayNdx, UInt32 flag, Int32 dx, Int32 dy)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        if (flag != 0u)
        {
            //SmoothCamActive = false;
            bgOverlay.flags |= BGOVERLAY_DEF.OVERLAY_FLAG.Loop;
            if (this.scene.combineMeshes)
                this.scene.CreateSeparateOverlay(this, this.UseUpscalFM, overlayNdx);
            if (dbug) Log.Message("EBG_overlaySetLoop " + overlayNdx + " | loop | dx:" + dx + " dy:" + dy);
        }
        else
        {
            bgOverlay.flags &= ~BGOVERLAY_DEF.OVERLAY_FLAG.Loop;
            if (dbug) Log.Message("EBG_overlaySetLoop " + overlayNdx + " | noloop | dx:" + dx + " dy:" + dy);
        }
        bgOverlay.scrollX = (Int16)dx;
        bgOverlay.scrollY = (Int16)dy;
        return 1;
    }

    public Int32 EBG_overlaySetLoopType(Int32 overlayNdx, UInt32 isScreenAnchored)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        if (isScreenAnchored != 0u)
        {
            if (dbug) Log.Message("EBG_overlaySetLoopType " + overlayNdx + " | + ScreenAnchored");
            bgOverlay.flags |= BGOVERLAY_DEF.OVERLAY_FLAG.ScreenAnchored;
        }
        else
        {
            if (dbug) Log.Message("EBG_overlaySetLoopType " + overlayNdx + " | - ScreenAnchored");
            bgOverlay.flags &= ~BGOVERLAY_DEF.OVERLAY_FLAG.ScreenAnchored;
        }
            
        return 1;
    }

    public Int32 EBG_overlaySetScrollWithOffset(Int32 overlayNdx, UInt32 flag, Int32 delta, Int32 offset, UInt32 isXOffset)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        if (flag != 0u)
        {
            if (dbug) Log.Message("EBG_overlaySetScrollWithOffset " + overlayNdx + " | ScrollWithOffset");
            bgOverlay.flags |= BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset;
            //SmoothCamActive = false;
            if (this.scene.combineMeshes)
                this.scene.CreateSeparateOverlay(this, this.UseUpscalFM, overlayNdx);
        }
        else
        {
            if (dbug) Log.Message("EBG_overlaySetScrollWithOffset " + overlayNdx + " | not ScrollWithOffset");
            bgOverlay.flags &= ~BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset;
        }
        if (isXOffset != 0u)
        {
            if (dbug) Log.Message("EBG_overlaySetScrollWithOffset " + overlayNdx + " | isXOffset");
            bgOverlay.scrollX = (Int16)offset;
            bgOverlay.scrollY = (Int16)delta;
            bgOverlay.isXOffset = 1;
        }
        else
        {
            if (dbug) Log.Message("EBG_overlaySetScrollWithOffset " + overlayNdx + " | not isXOffset");
            bgOverlay.scrollX = (Int16)delta;
            bgOverlay.scrollY = (Int16)offset;
            bgOverlay.isXOffset = 0;
        }
        return 1;
    }

    public Int32 EBG_charAttachOverlay(Int32 overlayNdx, Int16 attachX, Int16 attachY, SByte surroundMode, Byte r, Byte g, Byte b)
    {
        if (dbug) Log.Message("EBG_charAttachOverlay " + overlayNdx + " | attachX:" + attachX + " attachY:" + attachY);
        this.attachList[this.attachCount].ndx = (Int16)overlayNdx;
        this.attachList[this.attachCount].x = attachX;
        this.attachList[this.attachCount].y = attachY;
        this.attachList[this.attachCount].surroundMode = surroundMode;
        if (surroundMode >= 0)
        {
            if (dbug) Log.Message(" |_ EBG_charAttachOverlay surroundMode: RGB:" + r + g + b);
            this.attachList[this.attachCount].r = r;
            this.attachList[this.attachCount].g = g;
            this.attachList[this.attachCount].b = b;
            this.CreateBorder(overlayNdx, r, g, b);
        }
        this.attachCount++;
        return 1;
    }

    public Int32 EBG_animAnimate(Int32 animNdx, Int32 frameNdx)
    {
        if (dbug) Log.Message("EBG_animAnimate | anim:" + animNdx + " frame:" + frameNdx);
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        bgAnim.flags |= BGANIM_DEF.ANIM_FLAG.Animate;
        bgAnim.curFrame = frameNdx << 8;
        bgAnim.counter = 0;
        return 1;
    }

    public Int32 EBG_animShowFrame(Int32 animNdx, Int32 frameNdx)
    {
        if (dbug) Log.Message("EBG_animShowFrame | anim:" + animNdx + " frame:" + frameNdx);
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bgAnim.frameList;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        for (Int32 i = 0; i < bgAnim.frameCount; i++)
            overlayList[frameList[i].target].SetFlags(BGOVERLAY_DEF.OVERLAY_FLAG.Active, i == frameNdx);
        bgAnim.flags = BGANIM_DEF.ANIM_FLAG.SingleFrame;
        return 1;
    }

    public Int32 EBG_animSetActive(Int32 animNdx, Int32 flag)
    {
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        if (flag != 0)
        {
            if (dbug) Log.Message("EBG_animSetActive | PLAY anim:" + animNdx);
            bgAnim.flags |= BGANIM_DEF.ANIM_FLAG.StartPlay;
        }
        else
        {
            if (dbug) Log.Message("EBG_animSetActive | STOP anim:" + animNdx);
            bgAnim.flags &= ~BGANIM_DEF.ANIM_FLAG.StartPlay;
        }
        return 1;
    }

    public Int32 EBG_animSetFrameRate(Int32 animNdx, Int32 frameRate)
    {
        if (dbug) Log.Message("EBG_animSetFrameRate | anim:" + animNdx + " frameRate:" + frameRate);
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        bgAnim.frameRate = (Int16)frameRate;
        bgAnim.CalculateActualFrameCount();
        return 1;
    }

    public Int32 EBG_animSetFrameWait(Int32 animNdx, Int32 frameNdx, Int32 frameWait)
    {
        if (dbug) Log.Message("EBG_animSetFrameWait | anim:" + animNdx + " frame:" + frameNdx + " frameWait:" + frameWait);
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bgAnim.frameList;
        frameList[frameNdx].value = (SByte)frameWait;
        return 1;
    }

    public Int32 EBG_animSetFlags(Int32 animNdx, Int32 flags)
    {
        if (dbug) Log.Message("EBG_animSetFlags " + animNdx + " | flags:" + flags);
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        bgAnim.flags |= (BGANIM_DEF.ANIM_FLAG)flags & BGANIM_DEF.ANIM_FLAG.Modifiables;
        return 1;
    }

    public Int32 EBG_animSetPlayRange(Int32 animNdx, Int32 frameStart, Int32 frameEnd)
    {
        if (dbug) Log.Message("EBG_animSetPlayRange " + animNdx + " | frames: " + frameStart + " to " + frameEnd);
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bgAnim.frameList;
        bgAnim.flags |= BGANIM_DEF.ANIM_FLAG.StartPlay;
        bgAnim.curFrame = frameStart << 8;
        bgAnim.counter = 0;
        frameList[frameEnd].value = -1;
        if ((bgAnim.frameRate > 0 && frameEnd < frameStart) || (bgAnim.frameRate < 0 && frameEnd > frameStart))
            bgAnim.frameRate = (Int16)(-bgAnim.frameRate);
        return 1;
    }

    public Int32 EBG_animSetVisible(Int32 animNdx, Int32 isVisible)
    {
        if (dbug) Log.Message("EBG_animSetVisible | anim:"+ animNdx + " Visible:" + (isVisible != 0));
        BGANIM_DEF bgAnim = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bgAnim.frameList;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        bgAnim.curFrame = 0;
        bgAnim.frameRate = 256;
        bgAnim.counter = 0;
        bgAnim.flags = BGANIM_DEF.ANIM_FLAG.SingleFrame;
        for (Int32 i = 0; i < bgAnim.frameCount; i++)
            overlayList[frameList[i].target].SetFlags(BGOVERLAY_DEF.OVERLAY_FLAG.Active, isVisible != 0 && i == 0);
        return 1;
    }

    public Int32 EBG_cameraSetViewport(Int32 camNdx, Int16 minX, Int16 maxX, Int16 minY, Int16 maxY)
    {
        BGCAM_DEF bgCamera = this.scene.cameraList[camNdx];
        bgCamera.vrpMinX = (Int16)Math.Min(minX + HalfFieldWidthNative, bgCamera.w - HalfFieldWidthNative);
        bgCamera.vrpMaxX = (Int16)Math.Max(maxX - HalfFieldWidthNative, HalfFieldWidthNative);
        bgCamera.vrpMinY = (Int16)Math.Min(minY + HalfFieldHeight, bgCamera.h - HalfFieldHeight);
        bgCamera.vrpMaxY = (Int16)Math.Max(maxY - HalfFieldHeight, HalfFieldHeight);
        if (dbug) Log.Message("EBG_cameraSetViewport | vrpMinX:" + bgCamera.vrpMinX + " vrpMaxX:" + bgCamera.vrpMaxX + " vrpMinY:" + bgCamera.vrpMinY + " vrpMaxY:" + bgCamera.vrpMaxY);
        return 1;
    }

    public Int32 EBG_overlaySetShadeColor(Int32 overlayNdx, Byte r, Byte g, Byte b)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        List<BGSPRITE_LOC_DEF> spriteList = bgOverlay.spriteList;
        if (bgOverlay.transform.GetComponent<MeshRenderer>() != null) //EBG_isCombineMesh
        {
            Material material = bgOverlay.transform.gameObject.GetComponent<MeshRenderer>().material;
            material.SetColor("_Color", new Color(r / 128f, g / 128f, b / 128f, 1f));
            bgOverlay.transform.gameObject.GetComponent<MeshRenderer>().material = material;
        }
        else if (spriteList.Count > 0)
        {
            //if (dbug) Log.Message("EBG_overlaySetShadeColor | !EBG_isCombineMesh(bgOverlay) && (spriteList.Count > 0)");
            Material material = spriteList[0].transform.gameObject.GetComponent<MeshRenderer>().material;
            Int32 spriteCount = bgOverlay.spriteCount;
            Int32 indexShift = FF9StateSystem.Common.FF9.id != 0 ? spriteCount : 0;
            material.SetColor("_Color", new Color(r / 128f, g / 128f, b / 128f, 1f));
            for (Int32 i = 0; i < spriteCount; i++)
                spriteList[i + indexShift].transform.gameObject.GetComponent<MeshRenderer>().material = material;
        }
        if (dbug) Log.Message("EBG_overlaySetShadeColor " + overlayNdx + " | RGB:" + r + g + b);
        return 1;
    }

    public Int32 EBG_overlayMove(Int32 overlayNdx, Int16 dx, Int16 dy, Int16 dz)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        FieldMapInfo.fieldmapExtraOffset.UpdateOverlayOffset(this.mapName, overlayNdx, ref dz);
        float destX = (float)Mathf.Clamp(bgOverlay.orgX + dx, bgOverlay.minX, bgOverlay.maxX);
        float destY = (float)Mathf.Clamp(bgOverlay.orgY + dy, bgOverlay.minY, bgOverlay.maxY);

        // TODO Check Native: #147
        UInt16 destZ;
        if (FF9StateSystem.Common.FF9.fldMapNo == 2351 && overlayNdx >= 3 && overlayNdx <= 17) // official fix of the mine bucket
            destZ = 3000;
        else
            destZ = (UInt16)(bgOverlay.orgZ + (UInt16)dz);

        bgOverlay.orgX = destX;
        bgOverlay.orgY = destY;
        bgOverlay.orgZ = destZ;
        bgOverlay.curX = destX;
        bgOverlay.curY = destY;
        bgOverlay.curZ = destZ;
        bgOverlay.transform.localPosition = new Vector3(destX, destY, destZ);
        if (dbug) Log.Message("EBG_overlayMove " + overlayNdx + " | destX:" + destX + " destY:" + destY + " destZ:" + destZ);
        return 1;
    }

    public Int32 EBG_overlaySetOrigin(Int32 overlayNdx, Int32 orgX, Int32 orgY)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        bgOverlay.curX = (float)orgX;
        bgOverlay.curY = (float)orgY;
        bgOverlay.orgX = (float)orgX;
        bgOverlay.orgY = (float)orgY;
        this.flags |= FieldMapFlags.Unknown128;
        if (dbug) Log.Message("EBG_overlaySetOrigin " + overlayNdx + " | orgX:" + orgX + " orgY:" + orgY);
        return 1;
    }

    public Int32 EBG_overlaySetParallax(Int32 overlayNdx, UInt32 flag, Int32 dx, Int32 dy)
    {
        BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];
        if (flag != 0u)
        {
            if (dbug) Log.Message("EBG_overlaySetParallax " + overlayNdx + " | + Parallax | dx:" + dx + " dy:" + dy);
            bgOverlay.flags |= BGOVERLAY_DEF.OVERLAY_FLAG.Parallax;
        }
        else
        {
            if (dbug) Log.Message("EBG_overlaySetParallax " + overlayNdx + " | - Parallax | dx:" + dx + " dy:" + dy);
            bgOverlay.flags &= BGOVERLAY_DEF.OVERLAY_FLAG.Parallax;
        }
        bgOverlay.scrollX = (Int16)dx;
        bgOverlay.scrollY = (Int16)dy;
        return 1;
    }

    public void UpdateOverlayAll()
    {
        if (this.scene.cameraList.Count <= 0 || this.curCamIdx == -1)
            return;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
        Vector2 realVrp = new Vector2();
        realVrp.x = this.curVRP.x + bgCamera.centerOffset[0] + HalfFieldWidth;
        realVrp.y = this.curVRP.y - bgCamera.centerOffset[1] + HalfFieldHeight;
        this.scene.scrX = this.scene.curX + HalfFieldWidth - realVrp.x;
        this.scene.scrY = this.scene.curY + HalfFieldHeight - realVrp.y;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        for (int i = 0; i < this.scene.overlayCount; i++)
            this.UpdateOverlay(i, overlayList[i], realVrp);
    }

    private void UpdateOverlay(Int32 ovrNdx, BGOVERLAY_DEF bgOverlay, Vector2 realVrp)
    {
        //if (dbug && ovrNdx == 24) Log.Message("before: overlayPtr.curX:" + overlayPtr.curX);
        BGSCENE_DEF bgScene = this.scene;
        ushort spriteCount = bgOverlay.spriteCount;
        List<BGSPRITE_LOC_DEF> spriteList = bgOverlay.spriteList;
        float screenX = bgOverlay.curX + bgScene.scrX;
        float screenY = bgOverlay.curY + bgScene.scrY;
        Vector2 viewportSize = new Vector2(this.scrollWindowDim[(int)bgOverlay.viewportNdx][0], this.scrollWindowDim[(int)bgOverlay.viewportNdx][1]);
        Vector2 viewportPos = new Vector2(this.scrollWindowPos[(int)bgOverlay.viewportNdx][0], this.scrollWindowPos[(int)bgOverlay.viewportNdx][1]);
        if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Loop) != 0)
        {
            //if (dbug) Log.Message("UpdateOverlay | BGOVERLAY_DEF.OVERLAY_FLAG.Loop"); // example: scrolling sky 505
            float anchorX;
            float anchorY;
            if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScreenAnchored) != 0)
            {
                anchorX = viewportPos.x;
                anchorY = viewportPos.y;
                if (dbug) Log.Message("UpdateOverlay " + ovrNdx + " | Loop ScreenAnchored anchorX:" + anchorX + " anchorY:" + anchorY);
            }
            else
            {
                anchorX = (float)(HalfFieldWidth - realVrp.x + viewportPos.x);
                anchorY = (float)(HalfFieldHeight - realVrp.y + viewportPos.y);
            }
            if (bgOverlay.scrollX != 0)
            {
                if (bgOverlay.scrollX < 0)
                {
                    short deltaX = (short)(256 - ((short)(bgOverlay.scrollX) << 8 >> 8));
                    screenX = (float)((((int)bgOverlay.curX << 8) + (int)deltaX >> 8) + (int)bgScene.scrX);
                }
                screenX = (float)((screenX - (viewportSize.x - (float)bgOverlay.w)) % (float)bgOverlay.w + (viewportSize.x - (float)bgOverlay.w));
            }
            if (bgOverlay.scrollY != 0)
            {
                if (bgOverlay.scrollY < 0)
                {
                    short deltaY = (short)(256 - ((short)(bgOverlay.scrollX) << 8 >> 8));
                    screenY = (float)((((int)bgOverlay.curY << 8) + (int)deltaY >> 8) + (int)bgScene.scrY);
                }
                screenY = (float)((screenY - (viewportSize.y - (float)bgOverlay.h)) % (float)bgOverlay.h + (viewportSize.y - (float)bgOverlay.h));
            }
            
            for (short i = 0; i < spriteCount; i++)
            {
                BGSPRITE_LOC_DEF bgSprite = spriteList[i];
                Vector3 cacheLocalPos = bgSprite.cacheLocalPos;
                float anchoredX = screenX + bgSprite.offX;
                float anchoredY = screenY + bgSprite.offY;
                if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScreenAnchored) != 0)
                {
                    //if (dbug) Log.Message("UpdateOverlay " + ovrNdx + " | ScreenAnchored"); // example: 507, 931, 951
                    if (bgOverlay.scrollX != 0) // is moving on X axis
                    {
                        if (anchoredX + 16f - SmoothCamDelta.x >= bgOverlay.w)
                        {
                            anchoredX -= bgOverlay.w;
                        }
                        else if (anchoredX - SmoothCamDelta.x <= -16f)
                        {
                            anchoredX += bgOverlay.w;
                        }
                    }
                    if (bgOverlay.scrollY != 0) // is moving on Y axis
                    {
                        if (anchoredY + 16f + SmoothCamDelta.y >= bgOverlay.h)
                        {
                            anchoredY -= bgOverlay.h;
                        }
                        else if (anchoredY + SmoothCamDelta.y <= -16f)
                        {
                            anchoredY += bgOverlay.h;
                        }
                    }
                    cacheLocalPos.x = anchoredX + anchorX;
                    cacheLocalPos.y = anchoredY + anchorY;
                }
                else
                {
                    //if (dbug) Log.Message("UpdateOverlay " + ovrNdx + " | not enchored"); // example: 507, 931, 951
                    if (bgOverlay.scrollX != 0)
                    {
                        if (anchoredX + 16f >= bgOverlay.w) // - SmoothCamDelta.x
                        {
                            anchoredX -= bgOverlay.w;
                        }
                        else if (anchoredX <= -16f) // + SmoothCamDelta.x)
                        {
                            anchoredX += bgOverlay.w;
                        }
                        cacheLocalPos.x = anchoredX + anchorX;
                    }
                    else
                    {
                        cacheLocalPos.x = anchoredX;
                    }
                    if (bgOverlay.scrollY != 0)
                    {
                        if (anchoredY + 16f >= (short)bgOverlay.h) // - SmoothCamDelta.y
                        {
                            anchoredY -= bgOverlay.h;
                        }
                        else if (anchoredY <= -16f) // + SmoothCamDelta.y)
                        {
                            anchoredY += bgOverlay.h;
                        }
                        cacheLocalPos.y = anchoredY + anchorY;
                    }
                    else
                    {
                        cacheLocalPos.y = anchoredY;
                    }
                }
                cacheLocalPos.y += 16f;
                if (this.mapName == "FBG_N18_GTRE_MAP360_GT_GRD_0") // map 1000
                    cacheLocalPos.x += 8f;
                cacheLocalPos.x -= (this.scene.scrX + bgOverlay.curX);
                cacheLocalPos.y -= (this.scene.scrY + bgOverlay.curY);
                bgSprite.cacheLocalPos = cacheLocalPos;
                bgSprite.transform.localPosition = cacheLocalPos;
            }
            bgOverlay.transform.localPosition = new Vector3((float)bgOverlay.curX, (float)bgOverlay.curY, bgOverlay.transform.localPosition.z);
        }
        else if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset) != 0)
        {
            if (dbug) Log.Message("UpdateOverlay | BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset - current map: " + FF9StateSystem.Common.FF9.fldMapNo);
            float anchorX;
            float anchorY;
            if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScreenAnchored) != 0)
            {
                anchorX = viewportPos.x;
                anchorY = viewportPos.y;
                //if (dbug) Log.Message("UpdateOverlay | BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset | anchorX:" + anchorX + " anchorY:" + anchorY);
            }
            else
            {
                anchorX = HalfFieldWidth - realVrp.x + viewportPos.x;
                anchorY = HalfFieldHeight - realVrp.y + viewportPos.y;
            }
            if (bgOverlay.isXOffset != 0)
            {
                screenY = (screenY - (viewportSize.y - (short)bgOverlay.h)) % (short)bgOverlay.h + (viewportSize.y - (short)bgOverlay.h);
                screenX = screenX + screenY * bgOverlay.scrollX / (short)bgOverlay.h % (short)bgOverlay.w;
            }
            else
            {
                screenX = (screenX - (viewportSize.x - (short)bgOverlay.w)) % (short)bgOverlay.w + (viewportSize.x - (short)bgOverlay.w);
                screenY = screenY + screenX * bgOverlay.scrollY / (short)bgOverlay.w % (short)bgOverlay.h;
            }
            for (short i = 0; i < spriteCount; i++)
            {
                BGSPRITE_LOC_DEF bgSprite = spriteList[i];
                Vector3 localPosition = bgSprite.transform.localPosition;
                if (bgOverlay.isXOffset != 0)
                {
                    float xOffset = 0;
                    float yOffset = screenY + bgSprite.offY;
                    if (yOffset + 16 >= (short)bgOverlay.h)
                    {
                        yOffset = yOffset - bgOverlay.h;
                        xOffset = -bgOverlay.scrollX;
                    }
                    else if (yOffset <= -16)
                    {
                        yOffset = yOffset + bgOverlay.h;
                        xOffset = bgOverlay.scrollX;
                    }
                    float xOffsetAdjusted = (screenX + (short)bgSprite.offX + xOffset);
                    localPosition.x = (float)xOffsetAdjusted;
                    localPosition.y = (float)(yOffset + anchorY);
                }
                else
                {
                    short xOffset = 0;
                    short xOffsetAdjusted = (short)(screenX + (short)bgSprite.offX);
                    if (xOffsetAdjusted + 16 >= (short)bgOverlay.w)
                    {
                        xOffsetAdjusted = (short)(xOffsetAdjusted - (short)bgOverlay.w);
                        xOffset = (short)(-bgOverlay.scrollY);
                    }
                    else if (xOffsetAdjusted <= -16)
                    {
                        xOffsetAdjusted = (short)(xOffsetAdjusted + (short)bgOverlay.w);
                        xOffset = (short)(bgOverlay.scrollY);
                    }
                    localPosition.x = (float)(xOffsetAdjusted + anchorX);
                    localPosition.y = screenY + (float)bgSprite.offY + xOffset;
                }
                localPosition.y += 16f;
                localPosition.x -= (this.scene.scrX + bgOverlay.curX);
                localPosition.y -= (this.scene.scrY + bgOverlay.curY);
                bgSprite.transform.localPosition = localPosition;
            }
            bgOverlay.transform.localPosition = new Vector3((float)bgOverlay.curX * 1f, (float)bgOverlay.curY * 1f, bgOverlay.transform.localPosition.z);
        }
        else
        {
            bgOverlay.transform.localPosition = new Vector3(bgOverlay.curX, bgOverlay.curY, bgOverlay.transform.localPosition.z);
        }

        //if (dbug && ovrNdx == 24) Log.Message("after: overlayPtr.curX:" + overlayPtr.curX);
        bgOverlay.scrX = screenX;
        bgOverlay.scrY = screenY;

        if (this.mapName == "FBG_N18_GTRE_MAP360_GT_GRD_0" && ovrNdx == 12) // Clayra's Trunk text fix #367
        {
            bgOverlay.curZ = 0;
            bgOverlay.transform.localPosition = new Vector3(bgOverlay.transform.localPosition.x, bgOverlay.transform.localPosition.y, 0);
        }
    }

    public void EBG_scene2DScroll(Int16 destX, Int16 destY, UInt16 frameCount, UInt32 scrollType)
    {
        if (!IsActive)
            return;

        this.startPoint.x = this.curVRP.x;
        this.startPoint.y = this.curVRP.y;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];

        if (Configuration.Graphics.WidescreenSupport)
        {
            destX = (Int16)Mathf.Clamp(destX, bgCamera.vrpMinX, bgCamera.vrpMaxX);
        }

        this.endPoint.x = destX;
        this.endPoint.y = destY;
        this.frameCount = (Int16)frameCount;
        this.curFrame = 1;
        this.flags &= ~FieldMapFlags.Generic15;
        if (scrollType == (UInt32)FieldMapFlags.RotationScroll)
            IsRotationScroll = true;
        this.flags |= FieldMapFlags.Unknown1;
        if (dbug) Log.Message("EBG_scene2DScroll | X:" + startPoint.x + " -> " + endPoint.x + " | Y:" + startPoint.y + " -> " + endPoint.y + " |  frameCount:" + frameCount + " scrollType:" + scrollType);
    }

    public void EBG_scene2DScrollRelease(Int32 frameCount, UInt32 scrollType)
    {
        if (!IsActive)
            return;

        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
        this.startPoint.x = (float)this.curVRP.x;
        this.startPoint.y = (float)this.curVRP.y;
        Vector3 vertex = Vector3.zero;
        if (FF9StateSystem.Common.FF9.fldMapNo == 1656 && this.playerController == null)
        {
            // Iifa Tree/Eidolon Moun, select Zidane
            this.playerController = ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(8)).fieldMapActorController;
            this.extraOffset = Vector2.zero;
        }
        if (this.playerController != null)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 813 && this.playerController == ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(8)).fieldMapActorController)
            {
                // S. Gate/Berkmea, controlling Dagger -> switch to Mary instead
                this.playerController = ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(2)).fieldMapActorController;
            }
            vertex = this.playerController.curPos;
            vertex.y += (float)this.charAimHeight;
            vertex = PSX.CalculateGTE_RTPT(vertex, Matrix4x4.identity, bgCamera.GetMatrixRT(), bgCamera.GetViewDistance(), this.offset);
        }
        else
        {
            vertex.x += this.offset.x;
            vertex.y += this.offset.y;
        }
        float targetX = (bgCamera.w / 2) + bgCamera.centerOffset[0] + vertex.x - this.offset.x;
        float targetY = -( (bgCamera.h / 2) + bgCamera.centerOffset[1] + vertex.y + this.offset.y - (2 * HalfFieldHeight) );
        targetX = Mathf.Clamp(targetX, bgCamera.vrpMinX, bgCamera.vrpMaxX);
        targetY = Mathf.Clamp(targetY, bgCamera.vrpMinY, bgCamera.vrpMaxY);
        this.endPoint.x = targetX;
        this.endPoint.y = targetY;

        if (frameCount == -1)
            this.frameCount = 30;
        else
            this.frameCount = (Int16)frameCount;
        this.curFrame = 1;

        this.flags &= ~FieldMapFlags.Unknown4;
        this.flags |= FieldMapFlags.Unknown1 | FieldMapFlags.Unknown2;

        if (scrollType != UInt32.MaxValue)
            IsRotationScroll = scrollType == (UInt64)FieldMapFlags.RotationScroll;

        if (dbug) Log.Message("EBG_scene2DScrollRelease | targetX:" + targetX + " targetY:" + targetY);
    }

    public Int32 BgAnimationService()
    {
        for (Int32 i = 0; i < this.scene.animList.Count; i++)
        {
            BGANIM_DEF bgAnim = this.scene.animList[i];
            if ((bgAnim.flags & BGANIM_DEF.ANIM_FLAG.Animate) != 0 && (bgAnim.flags & BGANIM_DEF.ANIM_FLAG.ContinuePlay) != 0 && bgAnim.camNdx == this.camIdx && this.animIdx[i])
            {
                List<BGANIMFRAME_DEF> frameList = bgAnim.frameList;
                for (Int32 j = 0; j < bgAnim.frameCount; j++)
                    this.scene.overlayList[frameList[j].target].SetFlags(BGOVERLAY_DEF.OVERLAY_FLAG.Active, false);
                Int32 curFrame = bgAnim.curFrame >> 8;
                Byte target = frameList[curFrame].target;
                this.scene.overlayList[target].SetFlags(BGOVERLAY_DEF.OVERLAY_FLAG.Active, true);
                if (frameList[curFrame].value < 0)
                {
                    frameList[curFrame].value = 0;
                    bgAnim.counter = 0;
                    bgAnim.flags &= ~BGANIM_DEF.ANIM_FLAG.HasNotFinished;
                    return 1;
                }
                Int32 fastForwardFactor = FF9StateSystem.Settings.FastForwardFactor;
                Single fastForwardInvert = 1f / fastForwardFactor;
                if (this.animIdx[i])
                    this.scene.animList[i].counter++;
                if (bgAnim.counter >= frameList[curFrame].value)
                {
                    bgAnim.counter = 0;
                    if (fastForwardFactor != 1 && (bgAnim.frameCount % fastForwardFactor == 0 || fastForwardFactor % bgAnim.frameCount == 0))
                        bgAnim.curFrame += (Int32)(bgAnim.frameRate * fastForwardInvert * 2f);
                    else
                        bgAnim.curFrame += bgAnim.frameRate;
                    curFrame = bgAnim.curFrame >> 8;
                    if (curFrame >= bgAnim.frameCount)
                    {
                        if ((bgAnim.flags & BGANIM_DEF.ANIM_FLAG.Palindrome) != 0)
                        {
                            bgAnim.curFrame = bgAnim.frameCount - 1 << 8;
                            this.EBG_animSetFrameRate(i, -bgAnim.frameRate);
                        }
                        else
                        {
                            bgAnim.curFrame = 0;
                        }
                        bgAnim.flags &= ~BGANIM_DEF.ANIM_FLAG.HasNotFinished;
                    }
                    else if (curFrame < 0)
                    {
                        if ((bgAnim.flags & BGANIM_DEF.ANIM_FLAG.Palindrome) != 0)
                        {
                            bgAnim.curFrame = 0;
                            this.EBG_animSetFrameRate(i, -bgAnim.frameRate);
                        }
                        else
                        {
                            bgAnim.curFrame = bgAnim.frameCount - 1 << 8;
                        }
                        bgAnim.flags &= ~BGANIM_DEF.ANIM_FLAG.HasNotFinished;
                    }
                }
            }
        }
        //if (dbug) Log.Message("BgAnimationService");
        return 1;
    }

    private Int32 BgAttachService()
    {
        if (this.attachCount == 0)
            return 0;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.camIdx];
        BGSCENE_DEF bgScene = this.scene;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        Vector3 vertex = this.playerController.curPos;
        vertex.y += (Single)this.charAimHeight;
        vertex = PSX.CalculateGTE_RTPT(vertex, Matrix4x4.identity, bgCamera.GetMatrixRT(), bgCamera.GetViewDistance(), this.offset);
        vertex.y *= -1f;
        for (Byte i = 0; i < this.attachCount; i++)
        {
            EBG_ATTACH_DEF ebg_ATTACH_DEF = this.attachList[i];
            UInt16 index = (UInt16)ebg_ATTACH_DEF.ndx;
            if ((overlayList[index].flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0 && overlayList[index].camNdx == this.curCamIdx)
            {
                Int16 x = ebg_ATTACH_DEF.x;
                Int16 y = ebg_ATTACH_DEF.y;
                float overlayX = overlayList[index].curX = (Int16)(vertex.x - bgScene.curX - x + bgCamera.vrpMinX);
                float overlayY = overlayList[index].curY = (Int16)(vertex.y - bgScene.curY - y + bgCamera.vrpMinY);
                overlayList[index].transform.localPosition = new Vector3((short)overlayX, (short)overlayY, 0f);
            }
        }
        if (dbug) Log.Message("BgAttachService | vertex.x:" + vertex.x + " vertex.y:" + vertex.y);
        return 1;
    }

    public Int32 SceneServiceScroll(BGSCENE_DEF bgScene)
    {
        Int16 map = FF9StateSystem.Common.FF9.fldMapNo;
        Int32 overlayCount = (Int32)bgScene.overlayCount;
        List<BGOVERLAY_DEF> overlayList = bgScene.overlayList;
        for (Int32 i = 0; i < overlayCount; i++)
        {
            BGOVERLAY_DEF bgOverlay = overlayList[i];
            if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Loop) != 0)
            {
                if (bgOverlay.scrollX != 0 && bgOverlay.scrollX != 32767)
                {
                    bgOverlay.curX = (bgOverlay.curX % (Int32)bgOverlay.w) + (bgOverlay.scrollX / 256f);
                }
                if (bgOverlay.scrollY != 0 && bgOverlay.scrollY != 32767)
                {
                    bgOverlay.curY = (bgOverlay.curY % (Int32)bgOverlay.h) + (bgOverlay.scrollY / 256f);
                }
                if (dbug) Log.Message("SceneServiceScroll " + i + " | Loop | curX:" + bgOverlay.curX + " / curY:" + bgOverlay.curY);
            }
            if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset) != 0) // loop in diagonal (816) or loop + parallax (2904)
            {
                if (bgOverlay.isXOffset != 0)
                {
                    if (bgOverlay.scrollY != 32767)
                    {
                        bgOverlay.curY = bgOverlay.curY % (Int32)bgOverlay.h + (bgOverlay.scrollY / 256f);
                    }
                }
                else if (bgOverlay.scrollX != 32767)
                {
                    bgOverlay.curX = bgOverlay.curX % (Int32)bgOverlay.w + (bgOverlay.scrollX / 256f);
                }
                if (dbug) Log.Message("SceneServiceScroll " + i + " | ScrollWithOffset | X:" + bgOverlay.curX + " Y:" + bgOverlay.curY);
            }
            if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Parallax) != 0)
            {
                bgOverlay.curX = bgOverlay.orgX + ((this.curVRP.x + SmoothCamDelta.x - this.parallaxOrg.x) * (bgOverlay.scrollX / 256f));
                bgOverlay.curY = bgOverlay.orgY + ((this.curVRP.y - SmoothCamDelta.y - this.parallaxOrg.y) * (bgOverlay.scrollY / 256f));

                if (dbug) Log.Message("SceneServiceScroll " + i + " | Parallax | X:" + bgOverlay.curX + " Y:" + bgOverlay.curY + " SmoothCamDelta.x " + SmoothCamDelta.x);

                if (Configuration.Graphics.InitializeWidescreenSupport())
                {
                    switch (map)
                    {
                        case 1651: // 448
                            bgOverlay.transform.localScale = new Vector3(1.02f, 1.02f, 1f); bgOverlay.curX -= 4; break;
                        case 1657:
                            bgOverlay.curX = this.mainCamera.transform.localPosition.x * (bgOverlay.scrollX / 256); break;
                        case 1758: // 448
                            bgOverlay.transform.localScale = new Vector3(1.02f, 1.02f, 1f); bgOverlay.curX -= 4; break;
                        case 2600: // 464/416
                            bgOverlay.transform.localScale = new Vector3(1.12f, 1.12f, 1f); bgOverlay.curX -= 24; break;
                        case 2602: // 384/328
                            bgOverlay.transform.localScale = new Vector3(1.05f, 1.05f, 1f); bgOverlay.curX = 28; break;
                        case 2605: // 400/368
                            bgOverlay.transform.localScale = new Vector3(1.1f, 1.1f, 1f); bgOverlay.curX -= 16; break;
                        case 2606:
                            bgOverlay.curX = this.mainCamera.transform.localPosition.x * (bgOverlay.scrollX / 256); break;
                        case 2607: // 416/400
                            bgOverlay.transform.localScale = new Vector3(1.05f, 1.05f, 1f); bgOverlay.curX -= 8; bgOverlay.curY -= 8; break;
                        case 2651:
                            bgOverlay.transform.localScale = new Vector3(1.2f, 1.2f, 1f); bgOverlay.curX -= 56; bgOverlay.curY -= 16; break;
                        case 2660: // 536/528
                            bgOverlay.transform.localScale = new Vector3(1.02f, 1.02f, 1f); bgOverlay.curX -= 8; break;
                    }
                }
            }
        }
        if ((this.flags & FieldMapFlags.Unknown128) != 0u)
        {
            this.orgVRP.x = this.curVRP.x;
            this.orgVRP.y = this.curVRP.y;
            this.flags &= FieldMapFlags.Generic127;
        }
        return 1;
    }

    public void SceneService2DScroll()
    {
        if (!IsActive)
            return;

        FieldMapFlags flags = this.flags & FieldMapFlags.Generic7;
        if (flags == 0 || flags >= FieldMapFlags.Unknown4)
            return;

        Int16 currentFrame = this.curFrame;
        Int16 totalFrames = this.frameCount;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
        float aimX = (float)(this.endPoint.x - bgCamera.centerOffset[0] - HalfFieldWidth - this.startPoint.x);
        float aimY = (float)(this.endPoint.y + bgCamera.centerOffset[1] - HalfFieldHeight - this.startPoint.y);
        float viewportX = this.curVRP.x;
        float viewportY = this.curVRP.y;

        if (IsRotationScroll)
        {
            Int32 fixedPointAngle = (2048 * currentFrame / totalFrames) + 2048;
            Int32 rcos = ff9.rcos(fixedPointAngle) + 4096;
            float rotX = aimX * rcos / 8192;
            float rotY = aimY * rcos / 8192;
            this.curVRP.x = (float)(this.startPoint.x + rotX);
            this.curVRP.y = (float)(this.startPoint.y + rotY);
            //if (dbug) Log.Message("SceneService2DScroll(IsRotationScroll) | this.curVRP.x:" + this.curVRP.x + " this.curVRP.y:" + this.curVRP.y);
        }
        else
        {
            this.curVRP.x = this.startPoint.x + aimX * currentFrame / totalFrames;
            this.curVRP.y = this.startPoint.y + aimY * currentFrame / totalFrames;
            //if (dbug) Log.Message("SceneService2DScroll | this.curVRP.x:" + this.curVRP.x + " this.curVRP.y:" + this.curVRP.y);
        }

        viewportX = this.curVRP.x - viewportX;
        viewportY = this.curVRP.y - viewportY;
        //if (dbug) Log.Message("SceneService2DScroll | viewportX:" + viewportX + " viewportY:" + viewportY);

        UpdateOverlayXY(viewportX, viewportY);

        this.charOffset = new Vector2(this.curVRP.x, this.curVRP.y);

        if (flags == FieldMapFlags.Unknown1)
        {
            this.flags &= ~FieldMapFlags.Unknown1;
            this.flags |= FieldMapFlags.Unknown2;
        }

        if ((this.curFrame = (Int16)(this.curFrame + 1)) > totalFrames)
        {
            if ((this.flags & FieldMapFlags.Generic7) == FieldMapFlags.Unknown2)
            {
                this.flags &= ~FieldMapFlags.Generic7;
                this.flags |= FieldMapFlags.Unknown128 | FieldMapFlags.Unknown4;
            }
            else
            {
                this.flags &= ~FieldMapFlags.Generic7;
                this.flags |= FieldMapFlags.Unknown128;
            }
        }
    }

    private void UpdateOverlayXY(float dx, float dy)
    {
        for (Int32 overlayNdx = 0; overlayNdx < this.scene.overlayCount; overlayNdx++)
        {
            BGOVERLAY_DEF bgOverlay = this.scene.overlayList[overlayNdx];

            if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Loop) != 0)
            {
                if (bgOverlay.scrollX != 0)
                    bgOverlay.curX += dx;
                if (bgOverlay.scrollY != 0)
                    bgOverlay.curY += dy;
                if (dbug) Log.Message("UpdateOverlayXY " + overlayNdx + " | Loop | dx:" + dx + " scrollX:" + bgOverlay.scrollX + " curX:" + bgOverlay.curX + " scrollY:" + bgOverlay.scrollY + " curY:" + bgOverlay.curY);
                this.alphaScaleX(bgOverlay, dx);
                this.alphaScaleY(bgOverlay, dy);
            }
            else if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset) != 0)
            {
                if (bgOverlay.isXOffset != 0)
                    bgOverlay.curY += dy;
                else
                    bgOverlay.curX += dx;
                if (dbug) Log.Message("UpdateOverlayXY " + overlayNdx + " | dx:" + dx + " ScrollWithOffset | curX:" + bgOverlay.curX + " curY:" + bgOverlay.curY);
                this.alphaScaleX(bgOverlay, dx);
                this.alphaScaleY(bgOverlay, dy);
            }
        }
    }

    public float alphaScaleX(BGOVERLAY_DEF bgOverlay, float dx)
    {
        float scaledValue = dx * 65536;
        float ScaleFactor = (float)this.scrollWindowAlphaX[(Int32)bgOverlay.viewportNdx] * 256;
        if (ScaleFactor == 65536)
        {
            return bgOverlay.curX;
        }
        scaledValue = (bgOverlay.curX * 65536) - scaledValue * Math3D.Fixed2Float((int)Math.Abs(ScaleFactor));
        bgOverlay.curX = scaledValue / 65536;
        if (dbug) Log.Message("EBG_alphaScaleX | scaledValue:" + scaledValue + " ScaleFactor:" + ScaleFactor + " curX:" + bgOverlay.curX); 
        return bgOverlay.curX;
    }

    public float alphaScaleY(BGOVERLAY_DEF bgOverlay, float dy)
    {
        float scaledValue = dy * 65536;
        float ScaleFactor = (float)this.scrollWindowAlphaY[(Int32)bgOverlay.viewportNdx] * 256;
        if (ScaleFactor == 65536)
        {
            return bgOverlay.curY;
        }
        scaledValue = (bgOverlay.curY * 65536) - Math3D.Float2Fixed(Math3D.Fixed2Float((int)scaledValue) * Math3D.Fixed2Float((int)Math.Abs(ScaleFactor)));
        bgOverlay.curY = scaledValue / 65536;
        if (dbug) Log.Message("EBG_alphaScaleY | scaledValue:" + scaledValue + " ScaleFactor:" + ScaleFactor + " curY:" + bgOverlay.curY);
        return bgOverlay.curY;
    }

    private void SceneService3DScroll()
    {
        Int16 map = FF9StateSystem.Common.FF9.fldMapNo;

        if ((this.flags & FieldMapFlags.Generic7) != 0u || !this.IsActive || map == 70 || this.curCamIdx < 0 || this.curCamIdx > this.scene.cameraList.Count)
            return;

        if (map == 2512 && this.playerController == null) // CrutchForIpsenMap EVT_IPSEN_IP_CNT_2
        {
            this.playerController = ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(2)).fieldMapActorController;
            if (dbug) Log.Message("SceneService3DScroll | CrutchForIpsenMap");
        }

        if (map == 1656) // CrutchForEvaMap EVT_EVA1_IF_PTS_1
        {
            Int32 isNeedOffset = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(7385);
            if (isNeedOffset == 1)
            {
                this.playerController = null;
                this.extraOffset.x = -16f;
                this.extraOffset.y = -8f;
                if (dbug) Log.Message("SceneService3DScroll | CrutchForEvaMap | isNeedOffset");
            }
        }

        Vector3 prevScrOffset = Vector3.zero;
        BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];

        if (this.playerController != null)
        {
            prevScrOffset = this.playerController.curPos;
            prevScrOffset.y += this.charAimHeight;
            prevScrOffset = PSX.CalculateGTE_RTPT(prevScrOffset, Matrix4x4.identity, bgCamera.GetMatrixRT(), bgCamera.GetViewDistance(), this.offset);
        }
        else
        {
            prevScrOffset.x += this.offset.x + this.extraOffset.x;
            prevScrOffset.y += this.offset.y + this.extraOffset.y;
        }

        this.prevScr = prevScrOffset;
        float aimX = (bgCamera.w >> 1) + bgCamera.centerOffset[0] + prevScrOffset.x - HalfFieldWidth;
        float aimY = (bgCamera.h >> 1) + bgCamera.centerOffset[1] + prevScrOffset.y - HalfFieldHeight;
        float prevScrX = prevScrOffset.x;
        float prevScrY = prevScrOffset.y;
        aimX -= this.offset.x - HalfFieldWidth;
        aimY += this.offset.y - HalfFieldHeight;
        aimY *= -1f;

        if (aimX < bgCamera.vrpMinX)
            prevScrX = this.offset.x - (bgCamera.w >> 1) - bgCamera.centerOffset[0] + bgCamera.vrpMinX;
        else if (aimX > bgCamera.vrpMaxX)
            prevScrX = this.offset.x - (bgCamera.w >> 1) - bgCamera.centerOffset[0] + bgCamera.vrpMaxX;

        if (aimY < bgCamera.vrpMinY)
            prevScrY = this.offset.y + (bgCamera.h >> 1) + bgCamera.centerOffset[1] - bgCamera.vrpMinY;
        else if (aimY > bgCamera.vrpMaxY)
            prevScrY = this.offset.y + (bgCamera.h >> 1) + bgCamera.centerOffset[1] - bgCamera.vrpMaxY;

        this.charOffset.x = prevScrX - bgCamera.centerOffset[0];
        this.charOffset.y = -(prevScrY - bgCamera.centerOffset[1]);


        float prevVRPx = this.curVRP.x;
        float prevVRPy = this.curVRP.y;
        this.curVRP.x = Mathf.Clamp(aimX, bgCamera.vrpMinX, bgCamera.vrpMaxX) - bgCamera.centerOffset[0] - HalfFieldWidth;
        this.curVRP.y = Mathf.Clamp(aimY, bgCamera.vrpMinY, bgCamera.vrpMaxY) + bgCamera.centerOffset[1] - HalfFieldHeight;
        float dx = this.curVRP.x - prevVRPx;
        float dy = this.curVRP.y - prevVRPy;
        if (dbug) Log.Message("SceneService3DScroll | dx:" + dx + " dy:" + dy + " curVRP.x:" + this.curVRP.x + " curVRP.y:" + this.curVRP.y);

        UpdateOverlayXY(dx, dy);
    }

    public void EBG_char3DScrollSetActive(UInt32 isActive, Int32 frameCount, UInt32 scrollType)
    {
        if (isActive != 0u)
        {
            IsActive = true;
            if (frameCount > 0)
            {
                this.EBG_scene2DScrollRelease(frameCount, scrollType);
            }
        }
        else
        {
            IsActive = false;
        }
    }

    public void EBG_charLookAtUnlock()
    {
        if (!IsLocked)
            return;

        BGSCENE_DEF bgScene = this.scene;
        if (bgScene != null)
        {
            BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
            this.origVRPMinX = bgCamera.vrpMinX;
            this.origVRPMaxX = bgCamera.vrpMaxX;
            this.origVRPMinY = bgCamera.vrpMinY;
            this.origVRPMaxY = bgCamera.vrpMaxY;
            bgCamera.vrpMinX = (Int16)this.SHRT_MIN;
            bgCamera.vrpMaxX = (Int16)this.SHRT_MAX;
            bgCamera.vrpMinY = (Int16)this.SHRT_MIN;
            bgCamera.vrpMaxY = (Int16)this.SHRT_MAX;
            IsLocked = false;
            //if (dbug) Log.Message("EBG_charLookAtUnlock bgCamera.vrpMinX " + bgCamera.vrpMinX + " bgCamera.vrpMaxX " + bgCamera.vrpMaxX);
        }
    }

    public void EBG_charLookAtLock()
    {
        if (IsLocked)
            return;

        BGSCENE_DEF bgScene = this.scene;
        if (bgScene != null)
        {
            BGCAM_DEF bgCamera = this.scene.cameraList[this.curCamIdx];
            bgCamera.vrpMinX = this.origVRPMinX;
            bgCamera.vrpMaxX = this.origVRPMaxX;
            bgCamera.vrpMinY = this.origVRPMinY;
            bgCamera.vrpMaxY = this.origVRPMaxY;
            IsLocked = true;
            //if (dbug) Log.Message("EBG_charLookAtLock bgCamera.vrpMinX " + bgCamera.vrpMinX + " bgCamera.vrpMaxX " + bgCamera.vrpMaxX);
        }
    }

    public BGSCENE_DEF scene;

    public BGI_DEF bgi;

    public WalkMesh walkMesh;

    public FieldRainRenderer rainRenderer;

    public String mapName;

    public Boolean debugRender;

    public String debugObjName;

    public Int32 debugTriIdx;

    public Transform debugTriPosObj;

    public Vector3[] debugPosMarker;

    public Vector2 offset;

    public Vector2 extraOffset;

    public FieldMapActor player;

    public FieldMapActorController playerController;

    public Int32 camIdx;

    public Boolean[] animIdx;

    public Camera mainCamera;

    public FieldMapFlags flags;

    public Vector2 curVRP;

    public Vector2 orgVRP;

    public Vector2 charOffset;

    public Vector2 startPoint;

    public Vector2 endPoint;

    public Int16 curFrame;

    public Int16 frameCount;

    public Int16 origVRPMinX;

    public Int16 origVRPMaxX;

    public Int16 origVRPMinY;

    public Int16 origVRPMaxY;

    public Vector2 prevScr;

    public Int32 curCamIdx;

    public Int16 charAimHeight;

    public Vector2 parallaxOrg;

    public Int16[][] scrollWindowPos;

    public Int16[][] scrollWindowDim;

    public Int16[] scrollWindowAlphaX;

    public Int16[] scrollWindowAlphaY;

    private Int32[] frameCountList;

    public Boolean isBattleBackupPos;

    private UInt16 attachCount;

    private EBG_ATTACH_DEF[] attachList;

    public Boolean UseUpscalFM;

    public FieldMapEditor fmEditor;

    private static readonly Dictionary<int, FieldMap.EbgCombineMeshData> combineMeshDict = new Dictionary<int, FieldMap.EbgCombineMeshData>
    {
        {351, (FieldMap.EbgCombineMeshData)null},
        {358, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 13, 14 }}},
        {450, (FieldMap.EbgCombineMeshData)null },
        {407, (FieldMap.EbgCombineMeshData)null },
        {55, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 5 }}},
        {57, (FieldMap.EbgCombineMeshData)null },
        {60, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 15, 16 }}},
        {111, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 10 }}},
        {153, (FieldMap.EbgCombineMeshData)null },
        {154, (FieldMap.EbgCombineMeshData)null },
        {307, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 6, 8 }}},
        {308, (FieldMap.EbgCombineMeshData)null },
        {309, (FieldMap.EbgCombineMeshData)null },
        {507, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 8, 9, 10 }}},
        {551, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 13 }}},
        {556, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 27 }}},
        {566, (FieldMap.EbgCombineMeshData)null },
        {576, (FieldMap.EbgCombineMeshData)null },
        {603, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 35 }}},
        {612, (FieldMap.EbgCombineMeshData)null },
        {662, (FieldMap.EbgCombineMeshData)null },
        {705, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 25 }}},
        {706, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 13 }}},
        {707, (FieldMap.EbgCombineMeshData)null },
        {751, (FieldMap.EbgCombineMeshData)null },
        {755, (FieldMap.EbgCombineMeshData)null },
        {766, (FieldMap.EbgCombineMeshData)null },
        {802, (FieldMap.EbgCombineMeshData)null },
        {810, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 6, 7 }}},
        {815, (FieldMap.EbgCombineMeshData)null },
        {910, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 12, 13, 14, 15, 16, 17, 19 }}},
        {1910, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 12, 13, 14, 15, 16, 17, 19 }}},
        {916, (FieldMap.EbgCombineMeshData)null },
        {951, (FieldMap.EbgCombineMeshData)null },
        {952, (FieldMap.EbgCombineMeshData)null },
        {957, (FieldMap.EbgCombineMeshData)null },
        {1056, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 19, 24 }}},
        {1106, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 19, 24 }}},
        {1153, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 6, 7 }}},
        {1206, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 5, 6, 7, 8, 9, 10, 11 }}},
        {1207, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 7, 8 }}},
        {1214, (FieldMap.EbgCombineMeshData)null },
        {1215, (FieldMap.EbgCombineMeshData)null },
        {1222, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 21, 22, 23, 24, 25 }}},
        {1223, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 5, 6, 7, 8, 9, 10, 11 }}},
        {1301, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 13 }}},
        {1307, (FieldMap.EbgCombineMeshData)null },
        {1312, (FieldMap.EbgCombineMeshData)null },
        {1355, (FieldMap.EbgCombineMeshData)null },
        {1362, (FieldMap.EbgCombineMeshData)null },
        {1455, (FieldMap.EbgCombineMeshData)null },
        {3054, (FieldMap.EbgCombineMeshData)null },
        {1505, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 0, 8, 13 }}},
        {1950, (FieldMap.EbgCombineMeshData)null },
        {1225, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 5, 6, 7, 8, 9, 10, 11 }}},
        {1801, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 5, 6, 7, 8, 9, 10, 11 }}},
        {1802, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 7, 8 }}},
        {3002, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 7, 8 }}},
        {1806, (FieldMap.EbgCombineMeshData)null },
        {1807, (FieldMap.EbgCombineMeshData)null },
        {1814, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 21, 22, 23, 24, 25 }}},
        {1816, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 15, 16, 17, 18, 19 }}},
        {1823, (FieldMap.EbgCombineMeshData)null },
        {1852, (FieldMap.EbgCombineMeshData)null },
        {1860, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 10 }}},
        {1865, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 17, 18, 20 }}},
        {2000, (FieldMap.EbgCombineMeshData)null },
        {2001, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 7, 8 }}},
        {2101, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 13 }}},
        {565, (FieldMap.EbgCombineMeshData)null },
        {2112, (FieldMap.EbgCombineMeshData)null },
        {605, (FieldMap.EbgCombineMeshData)null },
        {2155, (FieldMap.EbgCombineMeshData)null },
        {2162, (FieldMap.EbgCombineMeshData)null },
        {2200, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25 }}},
        {2217, (FieldMap.EbgCombineMeshData)null },
        {2220, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 4, 5, 6, 7 }}},
        {2221, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 2, 4, 5, 26, 29 }}},
        {2404, (FieldMap.EbgCombineMeshData)null },
        {2453, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 1 }}},
        {2853, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }}},
        {2502, (FieldMap.EbgCombineMeshData)null },
        {2506, (FieldMap.EbgCombineMeshData)null },
        {2509, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 2, 3 }}},
        {2652, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 4 }}},
        {2906, (FieldMap.EbgCombineMeshData)null },
        {3100, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 38, 40, 46, 47, 48, 49, 50, 52 }}},
        {2107, new FieldMap.EbgCombineMeshData { skipOverlayList = new List<int> { 0, 1, 2, 3, 4, 5, 12 }}}
    };

    public static readonly List<String> fieldMapNameWithAreaTitle = new List<String>
    {
        "FBG_N01_ALXT_MAP016_AT_MSA_0",
        "FBG_N02_ALXC_MAP042_AC_GDR_0",
        "FBG_N04_EVFT_MAP072_EF_ENT_0",
        "FBG_N05_ICCV_MAP085_IC_ENT_0",
        "FBG_N06_VGDL_MAP098_DL_ENT_0",
        "FBG_N11_LDBM_MAP191_LB_ASD_0",
        "FBG_N12_GZML_MAP251_GZ_ENT_1",
        "FBG_N13_BRMC_MAP260_BU_ENT_0",
        "FBG_N14_CHCB_MAP300_CH_FST_0",
        "FBG_N14_CHCB_MAP301_CH_HLG_0",
        "FBG_N14_CHCB_MAP302_CH_FGD_0",
        "FBG_N14_CHCB_MAP303_CH_DWL_0",
        "FBG_N14_CHCB_MAP304_CH_PDS_0",
        "FBG_N15_KUIN_MAP310_KM_ENT_0",
        "FBG_N16_STGT_MAP330_SG_RND_0",
        "FBG_N18_GTRE_MAP360_GT_GRD_0",
        "FBG_N19_CLIR_MAP381_CY_TWN_2",
        "FBG_N20_TRNO_MAP400_TR_GAT_0",
        "FBG_N21_GRGR_MAP420_GR_CEN_0",
        "FBG_N23_QUNH_MAP443_QH_CAV_0",
        "FBG_N26_PNCL_MAP455_PR_GAL_0",
        "FBG_N27_FSLR_MAP465_FR_ENT_0",
        "FBG_N28_CDPT_MAP480_CP_ENT_0",
        "FBG_N29_CPMP_MAP510_CM_MP0_0",
        "FBG_N30_BMVL_MAP532_BV_ENT_0",
        "FBG_N31_IFTR_MAP550_IF_ENT_0",
        "FBG_N34_MDSR_MAP570_MS_ENT_0",
        "FBG_N35_ESTG_MAP600_EG_EXT_0",
        "FBG_N36_GLGV_MAP790_GV_WHL_0",
        "FBG_N38_SDPL_MAP650_SP_MP1_0",
        "FBG_N39_UUVL_MAP660_UV_EXT_0",
        "FBG_N40_TERA_MAP680_TA_THL_0",
        "FBG_N41_BRBL_MAP691_BB_SQR_0",
        "FBG_N42_PDMN_MAP721_PD_CAO_0",
        "FBG_N43_IPSN_MAP741_IP_EN1_0",
        "FBG_N44_EMSH_MAP760_ES_ENT_0",
        "FBG_N55_DGLO_MAP775_DG_ENT_0",
        "FBG_N56_MGNT_MAP810_MN_MOG_0"
    };

    public static FF9FieldAttrState FF9FieldAttr = new FF9FieldAttrState();

    private List<FF9Char> CharArray;

    public Int32 SHRT_MIN = -32768;

    public Int32 SHRT_MAX = 32767;

    public class EbgCombineMeshData
    {
        public EbgCombineMeshData()
        {
            this.skipOverlayList = null;
        }

        public List<int> skipOverlayList;
    }

    internal static readonly Int16 PsxFieldWidthNative = 320;
    internal static readonly Int16 PsxFieldHeightNative = 224;

    internal static volatile Int16 PsxFieldWidth = CalcPsxFieldWidth();
    internal static readonly Int16 HalfFieldHeight = (Int16)(PsxFieldHeightNative / 2);
    internal static volatile Int16 HalfFieldWidth = (Int16)(PsxFieldWidth / 2);
    internal static volatile Int16 HalfFieldWidthNative = (Int16)(PsxFieldWidthNative / 2);

    internal static readonly Int16 PsxScreenWidthNative = 320;
    internal static readonly Int16 PsxScreenHeightNative = 220;

    internal static volatile Int16 PsxScreenWidth = CalcPsxScreenWidth();
    internal static readonly Int16 HalfScreenHeight = (Int16)(PsxScreenHeightNative / 2);
    internal static volatile Int16 HalfScreenWidth = (Int16)(PsxScreenWidth / 2);

    internal static volatile Single ShaderMulX = CalcShaderMulX();
    internal static volatile Single ShaderMulY = CalcShaderMulY();

    public static void OnWidescreenSupportChanged()
    {
        PsxFieldWidth = CalcPsxFieldWidth();
        PsxScreenWidth = CalcPsxScreenWidth();
        if (Configuration.Graphics.InitializeWidescreenSupport())
        {
            int mapId = FF9StateSystem.Common.FF9.fldMapNo;
            Int32 mapWidth = NarrowMapList.MapWidth(mapId);
            //Log.Message("Configuration.Graphics.WidescreenSupport " + Configuration.Graphics.WidescreenSupport + " CalcPsxFieldWidth() " + CalcPsxFieldWidth() + " PsxScreenWidth 1 " + CalcPsxScreenWidth() + " Screen.width " + Screen.width + " Screen.height " + Screen.height + "mapWidth " + mapWidth);

            if (mapWidth <= PsxScreenWidth && PersistenSingleton<SceneDirector>.Instance.CurrentScene != "BattleMap")
            {
                PsxFieldWidth = (Int16)mapWidth;
                PsxScreenWidth = (Int16)mapWidth;
                //Log.Message("PsxScreenWidth 2 " + PsxScreenWidth);
            }
        }
        HalfFieldWidth = (Int16)(PsxFieldWidth / 2);
        HalfScreenWidth = (Int16)(PsxScreenWidth / 2);
        ShaderMulX = CalcShaderMulX();
        ShaderMulY = CalcShaderMulY();
        Shader.SetGlobalFloat("_MulX", ShaderMulX);
        Shader.SetGlobalFloat("_MulY", ShaderMulY);
        //Log.Message("OnWidescreenSupportChanged()");
        //Log.Message("HalfFieldWidth " + HalfFieldWidth + " HalfScreenWidth " + HalfScreenWidth + " ShaderMulX " + ShaderMulX + " PsxFieldWidth " + CalcShaderMulX() + " CalcShaderMulX() ");
    }

    private static Int16 CalcPsxFieldWidth() => Configuration.Graphics.InitializeWidescreenSupport() ? (Int16)(PsxFieldHeightNative * Screen.width / Screen.height) : PsxFieldWidthNative;
    private static Int16 CalcPsxScreenWidth() => Configuration.Graphics.InitializeWidescreenSupport() ? (Int16)(PsxScreenHeightNative * Screen.width / Screen.height) : PsxScreenWidthNative;
    private static Single CalcShaderMulX() => 1f / HalfFieldWidth;
    private static Single CalcShaderMulY() => 1f / HalfFieldHeight;

    private Boolean IsActive
    {
        get => (this.flags & FieldMapFlags.Active) == FieldMapFlags.Active;
        set
        {
            if (value)
                this.flags |= FieldMapFlags.Active;
            else
                this.flags &= ~FieldMapFlags.Active;
        }
    }

    private Boolean IsLocked
    {
        get => (this.flags & FieldMapFlags.Locked) == FieldMapFlags.Locked;
        set
        {
            if (value)
                this.flags |= FieldMapFlags.Locked;
            else
                this.flags &= ~FieldMapFlags.Locked;
        }
    }

    private Boolean IsRotationScroll
    {
        get => (this.flags & FieldMapFlags.RotationScroll) == FieldMapFlags.RotationScroll;
        set
        {
            if (value)
                this.flags |= FieldMapFlags.RotationScroll;
            else
                this.flags &= ~FieldMapFlags.RotationScroll;
        }
    }

    private bool dbug = false;

    private float Prev_CamPositionX, Prev_CamPositionY;
    private Int16 SmoothCamPercent()
    {
        return (Int16)Mathf.Clamp(Configuration.Graphics.CameraStabilizer, 0, 99);
    }
    private Int16 SmoothCamDelay;
    private Vector2 SmoothCamDelta;
    private Boolean SmoothCamActive;
}
