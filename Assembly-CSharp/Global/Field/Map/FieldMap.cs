using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FF9;
using Memoria;
using Memoria.Scripts;
using Object = System.Object;

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
        this.CharArray = new FF9Char[]
        {
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char(),
            new FF9Char()
        };
    }

    public bool IsCurrentFieldMapHasCombineMeshProblem()
    {
        return false;
    }

    public FieldMap.EbgCombineMeshData GetCurrentCombineMeshData()
    {
        return (FieldMap.EbgCombineMeshData)null;
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
        String empty = String.Empty;
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
            if (text == "English(UK)" && !mapName.Equals("FBG_N16_STGT_MAP330_SG_RND_0"))
            {
                text = "English(US)";
            }
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

        GameObject gameObject = new GameObject("TriPosObj");
        this.debugTriPosObj = gameObject.transform;
        this.debugTriPosObj.position = Vector3.zero;
        GameObject gameObject2 = GameObject.Find("FieldMap Camera");
        if (gameObject2 != (UnityEngine.Object)null)
        {
            this.mainCamera = gameObject2.GetComponent<Camera>();
            this.rainRenderer = gameObject2.AddComponent<FieldRainRenderer>();
        }
        this.camIdx = 0;
        this.curCamIdx = -1;
        this.charAimHeight = 324;
        this.lastFrame = 0;
        this.cumulativeTime = 0f;
        this.mapName = FF9StateSystem.Field.SceneName;
        this.UseUpscalFM = FF9StateSystem.Field.UseUpscalFM;
        if (this.mapName == String.Empty)
        {
            this.mapName = "FBG_N00_TSHP_MAP001_TH_CGR_0";
        }
        this.LoadFieldMap(this.mapName);
        this.frameTime = 0.0333333351f;
        HonoBehaviorSystem.FrameSkipEnabled = false;
        HonoBehaviorSystem.TargetFrameTime = this.frameTime;
        this.attachList = new EBG_ATTACH_DEF[10];
        this.isBattleBackupPos = false;
        this.EBG_init();
        if (FF9StateSystem.Common.FF9.fldMapNo == 2507)
        {
            base.StartCoroutine(this.DelayedActiveTri());
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo >= 3009 && FF9StateSystem.Common.FF9.fldMapNo <= 3011)
        {
            HonoBehaviorSystem.FrameSkipEnabled = true;
            HonoBehaviorSystem.TargetFrameTime = this.frameTime;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2356)
        {
            this.walkMesh.BGI_triSetActive(78u, 0u);
            this.walkMesh.BGI_triSetActive(79u, 0u);
            this.walkMesh.BGI_triSetActive(80u, 0u);
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2161)
        {
            this.walkMesh.BGI_triSetActive(69u, 0u);
        }
        this.fmEditor = base.gameObject.AddComponent<FieldMapEditor>();
        this.fmEditor.Init(this);
        this.debugPosMarker = new Vector3[5];
        for (Int32 i = 0; i < (Int32)this.debugPosMarker.Length; i++)
        {
            this.debugPosMarker[i] = SettingUtils.fieldMapSettings.debugPosMarker[i];
        }
        if (SettingUtils.fieldMapSettings.enable)
        {
            this.debugObjName = SettingUtils.fieldMapSettings.debugObjName;
            this.debugTriIdx = SettingUtils.fieldMapSettings.debugTriIdx;
        }
    }

    private IEnumerator DelayedActiveTri()
    {
        yield return new WaitForSeconds(0.5f);
        if (FF9StateSystem.Common.FF9.fldMapNo == 2507)
        {
            FieldMapActorController[] facs = UnityEngine.Object.FindObjectsOfType<FieldMapActorController>();
            for (Int32 i = 0; i < (Int32)facs.Length; i++)
            {
                FieldMapActorController fac = facs[i];
                if (!fac.isPlayer)
                {
                    this.walkMesh.BGI_charSetActive(fac, 0u);
                }
            }
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
        HonoBehaviorSystem.TargetFrameTime = this.frameTime;
    }

    public Camera GetMainCamera()
    {
        return this.mainCamera;
    }

    public BGCAM_DEF GetCurrentBgCamera()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 70)
        {
            return (BGCAM_DEF)null;
        }
        if (this.curCamIdx < 0 || this.curCamIdx > this.scene.cameraList.Count)
        {
            return (BGCAM_DEF)null;
        }
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
        this.lastFrame = 0;
        this.cumulativeTime = 0f;
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
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[overlayNdx];
        Material material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_None"));
        for (Int32 i = 0; i < 4; i++)
        {
            GameObject gameObject = new GameObject("border" + i.ToString("D3"));
            gameObject.transform.parent = bgoverlay_DEF.transform;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            List<Vector3> list = new List<Vector3>();
            List<Vector2> list2 = new List<Vector2>();
            List<Int32> list3 = new List<Int32>();
            list.Clear();
            list2.Clear();
            list3.Clear();
            Int32 num4;
            Int32 num3;
            Int32 num2;
            Int32 num = num2 = (num3 = (num4 = 0));
            Int32 index = this.curCamIdx;
            switch (i)
            {
                case 0:
                    num3 = (Int32)(-this.scene.cameraList[index].vrpMinX);
                    num4 = (Int32)bgoverlay_DEF.h;
                    num2 = 0;
                    num = num4;
                    break;
                case 1:
                    num3 = (Int32)this.scene.cameraList[index].vrpMinX;
                    num4 = (Int32)bgoverlay_DEF.h;
                    num2 = (Int32)bgoverlay_DEF.w;
                    num = num4;
                    break;
                case 2:
                    num3 = (Int32)bgoverlay_DEF.w;
                    num4 = (Int32)this.scene.cameraList[index].vrpMinY;
                    num2 = 0;
                    num = 0;
                    break;
                case 3:
                    num3 = (Int32)bgoverlay_DEF.w;
                    num4 = (Int32)(-(Int32)this.scene.cameraList[0].vrpMinY);
                    num2 = 0;
                    num = (Int32)bgoverlay_DEF.h;
                    break;
            }
            list.Add(new Vector3((Single)num2, (Single)(num - num4), 0f));
            list.Add(new Vector3((Single)(num2 + num3), (Single)(num - num4), 0f));
            list.Add(new Vector3((Single)(num2 + num3), (Single)num, 0f));
            list.Add(new Vector3((Single)num2, (Single)num, 0f));
            list2.Add(new Vector2(0f, 0f));
            list2.Add(new Vector2(1f, 0f));
            list2.Add(new Vector2(1f, 1f));
            list2.Add(new Vector2(0f, 1f));
            list3.Add(2);
            list3.Add(1);
            list3.Add(0);
            list3.Add(3);
            list3.Add(2);
            list3.Add(0);
            Mesh mesh = new Mesh();
            mesh.vertices = list.ToArray();
            mesh.uv = list2.ToArray();
            mesh.triangles = list3.ToArray();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
            Texture2D texture2D = new Texture2D(1, 1);
            Color[] pixels = texture2D.GetPixels();
            for (Int32 j = 0; j < (Int32)pixels.Length; j++)
            {
                pixels[j] = new Color32(r, g, b, Byte.MaxValue);
            }
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
        this.cumulativeTime = 0f;
        this.EBG_animationService();
        this.EBG_attachService();
    }

    public override void HonoOnGUI()
    {
        if (this.walkMesh != null)
        {
            this.walkMesh.RenderWalkMeshNormal();
            if (this.debugTriIdx < -1)
            {
                this.debugTriIdx = -1;
            }
            else if (this.debugTriIdx >= this.walkMesh.tris.Count)
            {
                this.debugTriIdx = this.walkMesh.tris.Count - 1;
            }
            this.walkMesh.RenderWalkMeshTris(this.debugTriIdx);
            if (this.debugTriIdx != -1)
            {
                WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[this.debugTriIdx];
                this.debugTriPosObj.position = walkMeshTriangle.originalCenter;
            }
            for (Int32 i = 0; i < (Int32)this.debugPosMarker.Length; i++)
            {
                DebugUtil.DebugDrawMarker(this.debugPosMarker[i], 20f, Color.yellow);
            }
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
                    if (actor.go != (UnityEngine.Object)null)
                    {
                        actor.go.transform.localRotation = Quaternion.AngleAxis(actor.rotAngle[2], Vector3.back) * Quaternion.AngleAxis(actor.rotAngle[1], Vector3.up) * Quaternion.AngleAxis(actor.rotAngle[0], Vector3.left);
                    }
                }
            }
        }
        foreach (Int32 key in FF9StateSystem.Common.FF9.charArray.Keys)
        {
            FF9Char ff9Char = FF9StateSystem.Common.FF9.charArray[key];
            if (ff9Char != null && ff9Char.geo != (UnityEngine.Object)null && FF9Char.ff9charptr_attr_test(ff9Char, 4096) == 0)
            {
                GeoTexAnim component = ff9Char.geo.GetComponent<GeoTexAnim>();
                if (component != (UnityEngine.Object)null && component.geoTexAnimGetCount() >= 2 && !component.ff9fieldCharIsTexAnimActive())
                {
                    component.geoTexAnimPlay(2);
                }
            }
        }
        fldmcf.ff9fieldMCFService();
        if ((FF9StateSystem.Common.FF9.attr & 2u) == 0u)
        {
            for (Int32 i = 0; i < 16; i++)
            {
                this.CharArray[i].evt = (PosObj)null;
            }
            Int32 charCount = FieldMap.ff9fieldCharGetActiveList(this.CharArray);
            FF9Snd.ff9fieldSoundCharService(this.CharArray, charCount);
        }
        fldchar.ff9fieldCharEffectService();
    }

    public static Int32 ff9fieldCharGetActiveList(FF9Char[] CharArray)
    {
        Int32 result = 0;
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        for (ObjList objList = instance.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (instance.objIsVisible(obj) && obj.cid == 4)
            {
                CharArray[result++].evt = (PosObj)obj;
            }
        }
        return result;
    }

    public override void HonoLateUpdate()
    {
        this.EBG_sceneService2DScroll();
        this.EBG_sceneService3DScroll();
        this.EBG_sceneServiceScroll(this.scene);
        if (Configuration.Graphics.InitializeWidescreenSupport())
        {
            OnWidescreenSupportChanged();
        }
        this.CenterCameraOnPlayer();
        this.UpdateOverlayAll();
    }

    public Int32 GetCurrentCameraIndex()
    {
        return this.camIdx;
    }

    public void SetCurrentCameraIndex(Int32 newCamIdx)
    {
        if (this.camIdx == newCamIdx)
        {
            return;
        }
        this.camIdx = newCamIdx;
        this.ActivateCamera();
        this.walkMesh.ProjectedWalkMesh = this.GetCurrentBgCamera().projectedWalkMesh;
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.camIdx];
        Vector2 centerOffset = bgcam_DEF.GetCenterOffset();
        this.offset.x = (Single)(bgcam_DEF.w / 2) + centerOffset.x;
        this.offset.y = -((Single)(bgcam_DEF.h / 2) + centerOffset.y);
        this.offset.x = this.offset.x - HalfFieldWidth;
        this.offset.y = this.offset.y + HalfFieldHeight;
        Shader.SetGlobalFloat("_OffsetX", this.offset.x);
        Shader.SetGlobalFloat("_OffsetY", this.offset.y);
        Shader.SetGlobalFloat("_MulX", ShaderMulX);
        Shader.SetGlobalFloat("_MulY", ShaderMulY);
        Shader.SetGlobalMatrix("_MatrixRT", bgcam_DEF.GetMatrixRT());
        Shader.SetGlobalFloat("_ViewDistance", bgcam_DEF.GetViewDistance());
        Shader.SetGlobalFloat("_DepthOffset", (Single)bgcam_DEF.depthOffset);
        FF9StateSystem.Field.FF9Field.loc.map.charOTOffset = bgcam_DEF.depthOffset;
        FF9StateSystem.Common.FF9.cam = bgcam_DEF.GetMatrixRT();
        FF9StateSystem.Common.FF9.proj = bgcam_DEF.proj;
        FF9StateSystem.Common.FF9.projectionOffset = this.offset;
        BGCAM_DEF bgcam_DEF2 = this.scene.cameraList[this.camIdx];
        this.scene.maxX = bgcam_DEF2.vrpMaxX;
        this.scene.maxY = bgcam_DEF2.vrpMaxY;
        this.flags |= FieldMapFlags.Unknown128;
        this.walkMesh.ProcessBGI();
    }

    public static Boolean IsNarrowMap()
    {
        return NarrowMapList.IsCurrentMapNarrow();
    }

    public void LoadFieldMap(String name)
    {
        Transform transform = base.transform;
        transform.localScale = new Vector3(1f, 1f, 1f);
        this.scene = new BGSCENE_DEF(this.UseUpscalFM);
        this.scene.LoadEBG(this, FieldMap.GetMapResourcePath(name), name);
        this.bgi = new BGI_DEF();
        this.bgi.LoadBGI(this, FieldMap.GetMapResourcePath(name), name);
        this.ActivateCamera();
        if (FF9StateSystem.Common.FF9.fldMapNo == 70)
        {
            return;
        }
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.camIdx];
        Vector2 centerOffset = bgcam_DEF.GetCenterOffset();
        this.offset.x = (Single)(bgcam_DEF.w / 2) + centerOffset.x;
        this.offset.y = -((Single)(bgcam_DEF.h / 2) + centerOffset.y);
        this.offset.x = this.offset.x - HalfFieldWidth;
        this.offset.y = this.offset.y + HalfFieldHeight;
        Shader.SetGlobalFloat("_OffsetX", this.offset.x);
        Shader.SetGlobalFloat("_OffsetY", this.offset.y);
        Shader.SetGlobalFloat("_MulX", ShaderMulX);
        Shader.SetGlobalFloat("_MulY", ShaderMulY);
        Shader.SetGlobalMatrix("_MatrixRT", bgcam_DEF.GetMatrixRT());
        Shader.SetGlobalFloat("_ViewDistance", bgcam_DEF.GetViewDistance());
        Shader.SetGlobalFloat("_DepthOffset", (Single)bgcam_DEF.depthOffset);
        FF9StateSystem.Field.FF9Field.loc.map.charOTOffset = bgcam_DEF.depthOffset;
        FF9StateSystem.Common.FF9.cam = bgcam_DEF.GetMatrixRT();
        FF9StateSystem.Common.FF9.proj = bgcam_DEF.proj;
        FF9StateSystem.Common.FF9.projectionOffset = this.offset;
        this.walkMesh = new WalkMesh(this);
        this.walkMesh.CreateWalkMesh();
        this.walkMesh.CreateProjectedWalkMesh();
        this.walkMesh.BGI_simInit();
    }

    public void ActivateCamera()
    {
        if (this.camIdx == this.curCamIdx)
        {
            return;
        }
        if (this.camIdx >= this.scene.cameraList.Count)
        {
            this.camIdx = this.curCamIdx;
            return;
        }
        this.curCamIdx = this.camIdx;
        this.lastFrame = 0;
        for (Int32 i = 0; i < this.scene.cameraList.Count; i++)
        {
            BGCAM_DEF bgcam_DEF = this.scene.cameraList[i];
            if (i == this.camIdx)
            {
                bgcam_DEF.transform.gameObject.SetActive(true);
            }
            else
            {
                bgcam_DEF.transform.gameObject.SetActive(false);
            }
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
            Vector3 startPos = FF9StateSystem.Field.startPos;
            fieldMapActorController.SetPosition(FF9StateSystem.Field.startPos, true, true);
            fieldMapActorController.transform.localRotation = Quaternion.Euler(new Vector3(0f, FF9StateSystem.Field.startRot, 0f));
        }
        this.player = actor;
        this.playerController = fieldMapActorController;
        if (FF9StateSystem.Field.isDebugWalkMesh)
        {
            gameObject.transform.localScale = new Vector3(-1f, -1f, 1f);
            Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
            for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                renderer.material.shader = ShadersLoader.Find("Unlit/Transparent Cutout");
            }
        }
        else
        {
            gameObject.transform.localScale = new Vector3(-1f, -1f, 1f);
            Renderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<Renderer>();
            for (Int32 j = 0; j < (Int32)componentsInChildren2.Length; j++)
            {
                Renderer renderer2 = componentsInChildren2[j];
                renderer2.material.shader = ShadersLoader.Find("PSX/FieldMapActor");
            }
        }
    }

    public void RestoreModels(GameObject playerGO, Actor actorOfObj)
    {
        FieldMapActorController component = playerGO.GetComponent<FieldMapActorController>();
        component.charFlags = (UInt16)actorOfObj.charFlags;
        component.activeFloor = (Int32)actorOfObj.activeFloor;
        component.activeTri = (Int32)actorOfObj.activeTri;
    }

    public void RestoreAttachModel(GameObject playerGO, Actor actorOfObj)
    {
        if (actorOfObj.attatchTargetUid != -1)
        {
            Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(actorOfObj.attatchTargetUid);
            geo.geoAttach(playerGO, objUID.go, actorOfObj.attachTargetBoneIndex);
        }
    }

    public void RestoreShadowOff(int actorUid, Actor actorOfObj)
    {
        if (actorOfObj.isShadowOff)
        {
            ff9shadow.FF9ShadowOffField(actorUid);
        }
    }

    private void SetCharScale(Actor actorOfObj, int sx, int sy, int sz)
    {
        int num = 18;
        if (actorOfObj != null)
        {
            if (actorOfObj.go != (UnityEngine.Object)null)
            {
                geo.geoScaleSetXYZ(actorOfObj.go, sx << 24 >> num, sy << 24 >> num, sz << 24 >> num);
            }
            actorOfObj.scaley = (byte)sy;
        }
    }

    public void AddFieldChar(GameObject playerGO, Vector3 pos, Quaternion rot, Boolean isPlayer, Actor actorOfObj, Boolean needRestore = false)
    {
        playerGO.transform.parent = base.transform;
        playerGO.transform.localScale = new Vector3(1f, 1f, 1f);
        FieldMapActor fieldMapActor = FieldMapActor.CreateFieldMapActor(playerGO, actorOfObj);
        FieldMapActorController fieldMapActorController = playerGO.AddComponent<FieldMapActorController>();
        actorOfObj.fieldMapActorController = fieldMapActorController;

        FF9BattleDBHeightAndRadius.TryFindNeckBoneIndex(actorOfObj.model, ref actorOfObj.neckBoneIndex);

        if (needRestore)
        {
            this.RestoreModels(playerGO, actorOfObj);
        }

        fieldMapActorController.fieldMap = this;
        fieldMapActorController.walkMesh = this.walkMesh;
        fieldMapActorController.actor = fieldMapActor;
        fieldMapActorController.originalActor = actorOfObj;
        fieldMapActorController.isPlayer = isPlayer;
        fieldMapActorController.SetPosition(pos, true, !needRestore);
        fieldMapActorController.radius = (Single)actorOfObj.bgiRad * 4f;
        playerGO.transform.localRotation = rot;

        if (needRestore)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 1508)
            {
                this.RestoreAttachModel(playerGO, actorOfObj);
            }
            if (FF9StateSystem.Common.FF9.fldMapNo == 1508 || FF9StateSystem.Common.FF9.fldMapNo == 1706)
            {
                this.RestoreShadowOff((int)fieldMapActor.actor.uid, actorOfObj);
            }
        }

        if (isPlayer)
        {
            this.player = fieldMapActor;
            this.playerController = fieldMapActorController;
        }
        if (FF9StateSystem.Field.isDebugWalkMesh)
        {
            playerGO.transform.localScale = new Vector3(-1f, -1f, 1f);
            Renderer[] componentsInChildren = playerGO.GetComponentsInChildren<Renderer>();
            for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                renderer.material.shader = ShadersLoader.Find("Unlit/Transparent Cutout");
            }
        }
        else
        {
            playerGO.transform.localScale = new Vector3(-1f, -1f, 1f);
            if (actorOfObj.model == 395)
            {
                Renderer[] componentsInChildren2 = playerGO.GetComponentsInChildren<Renderer>();
                for (Int32 j = 0; j < (Int32)componentsInChildren2.Length; j++)
                {
                    Renderer renderer2 = componentsInChildren2[j];
                    Material[] materials = renderer2.materials;
                    for (Int32 k = 0; k < (Int32)materials.Length; k++)
                    {
                        Material material = materials[k];
                        material.shader = ShadersLoader.Find("PSX/Actor_Abr_1");
                    }
                }
            }
            else
            {
                Renderer[] componentsInChildren3 = playerGO.GetComponentsInChildren<Renderer>();
                for (Int32 l = 0; l < (Int32)componentsInChildren3.Length; l++)
                {
                    Renderer renderer3 = componentsInChildren3[l];
                    Material[] materials2 = renderer3.materials;
                    for (Int32 m = 0; m < (Int32)materials2.Length; m++)
                    {
                        Material material2 = materials2[m];
                        material2.shader = ShadersLoader.Find("PSX/FieldMapActor");
                    }
                }
            }
        }
        if (needRestore && FF9StateSystem.Common.FF9.fldMapNo == 1706)
        {
            if (fieldMapActor.actor.uid == 4 && FF9StateSystem.Settings.CurrentLanguage == "Japanese")
            {
                this.SetCharScale(fieldMapActor.actor, 40, 40, 40);
            }
            else if (fieldMapActor.actor.uid == 3 || fieldMapActor.actor.uid == 5)
            {
                this.SetCharScale(fieldMapActor.actor, 80, 80, 80);
            }
        }
        if (FF9StateSystem.Common.FF9.fldMapNo == 2924 && fieldMapActorController.originalActor.sid == 8)
        {
            fieldMapActor.SetRenderQueue(2000);
        }
        else
        {
            fieldMapActor.SetRenderQueue(-1);
        }
        if (fieldMapActor.GetComponent<Animation>().GetClip(FF9DBAll.AnimationDB.GetValue((Int32)fieldMapActorController.originalActor.idle)) != (UnityEngine.Object)null)
        {
            fieldMapActor.GetComponent<Animation>().Play(FF9DBAll.AnimationDB.GetValue((Int32)fieldMapActorController.originalActor.idle));
        }
    }

    public void updatePlayer(GameObject playerGO)
    {
        FieldMapActor component = playerGO.GetComponent<FieldMapActor>();
        FieldMapActorController component2 = playerGO.GetComponent<FieldMapActorController>();
        component2.isPlayer = true;
        this.player = component;
        this.playerController = component2;
    }

    public void CenterCameraOnPlayer()
    {
        if (!MBG.IsNull && !MBG.Instance.IsFinished())
        {
            return;
        }
        Camera camera = this.GetMainCamera();
        Int16 map = FF9StateSystem.Common.FF9.fldMapNo;
        if (map == 70)
        {
            return;
        }
        if (this.curCamIdx < 0 || this.curCamIdx > this.scene.cameraList.Count)
        {
            return;
        }
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.camIdx];
        Vector3 localPosition = camera.transform.localPosition;
        localPosition.x = bgcam_DEF.centerOffset[0] + this.charOffset.x;
        localPosition.y = bgcam_DEF.centerOffset[1] - this.charOffset.y;

        if (Configuration.Graphics.InitializeWidescreenSupport())
        {
            int threshmargin = (int)(Math.Min((bgcam_DEF.w - PsxFieldWidth), 0)); //offset value for fields that are between 320 & 398
            if (!IsNarrowMap() && map != 507)
            {

                foreach (KeyValuePair<int, int> entry in mapCameraMargin)
                {
                    if (map == entry.Key)
                    {
                        threshmargin = entry.Value;
                    }
                }

                int threshright = (int)(bgcam_DEF.w - PsxFieldWidth - threshmargin);

                if (map == 103 || map == 1853 || map == 2053 || map == 2606) // exception in alex center, branbal
                {
                    threshmargin = (int)(threshmargin + 16);
                }
                if (map == 2903) // exception in memoria castle
                {
                    threshright = (int)(threshright - 32);
                }
                if (map == 2923) // exception in crystal world
                {
                    threshmargin = (int)(threshmargin + 20);
                }

                localPosition.x = (int)(Math.Max(threshmargin, localPosition.x));
                localPosition.x = (int)(Math.Min(threshright, localPosition.x));
            }
            else if (map == 1205 || map == 154 || map == 1214 || map == 1807 || map == 1652 || map == 2552)
            {
                if (map == 1652 && this.camIdx == 0)
                {
                    threshmargin = threshmargin + 16;
                }
                
                int threshright = (int)(bgcam_DEF.w - PsxFieldWidth - threshmargin);
                
                localPosition.x = (int)(Math.Max(threshmargin, localPosition.x));
                localPosition.x = (int)(Math.Min(threshright, localPosition.x));
            }
            else if (IsNarrowMap())
            {
                foreach (KeyValuePair<int, int> entry in actualNarrowMapWidthDict)
                {
                    if (map == entry.Key)
                    {
                        localPosition.x = (int)((bgcam_DEF.w - entry.Value) / 2);
                    }
                }
                switch (map) // offsets for scrolling maps stretched to WS
                {
                    case 456:
                        localPosition.x = 160;
                        break;
                    case 505: //Cargo ship offset
                        localPosition.x = 105;
                        break;
                    case 1153: //Rose Rouge cockpit offset
                        localPosition.x = 175;
                        break;
                    default:
                        break;
                }
            }
        }

        camera.transform.localPosition = localPosition;
    }


    private static readonly Dictionary<int, int> mapCameraMargin = new Dictionary<int, int>
    {
        //{mapNo,pixels on each side to crop because of scrollable}
        {1051,8},
        {1057,16},
        {1060,16},
        {1652,16},
        {1653,16},
    };

    public static readonly Dictionary<int, int> actualNarrowMapWidthDict = new Dictionary<int, int>
    {
        //{mapNo,(actualWidth - 2)}
        {203,334},
        {502,334},
        {503,334},
        {760,334},
        {814,334},
        {816,334},
        {1151,334},
        {1458,334},
        {1500,334},
        {1506,334},
        {1605,334},
        {1606,334},
        {1608,334},
        {1660,334},
        {1661,334},
        {1662,334},
        {1705,334},
        {1707,334},
        {1751,334},
        {2202,334},
        {2204,334},
        {2205,334},
        {2208,334},
        {2254,334},
        {2257,334},
        {2303,334},
        {2365,334},
        {2513,334},
        {2756,334},
        {2932,334},
        {3057,334},
        {114,350},
        {550,350},
        {620,350},
        {802,350},
        {803,350},
        {1212,350},
        {1300,350},
        {1370,350},
        {1508,350},
        {1650,350},
        {1752,350},
        {1757,350},
        {1863,350},
        {1951,350},
        {1952,350},
        {2000,350},
        {2055,350},
        {2771,350},
        {2203,350},
        {2261,350},
        {2356,350},
        {2362,350},
        {2500,350},
        {2501,350},
        {2654,350},
        {60,366},
        {150,366},
        {161,366},
        {201,366},
        {262,366},
        {565,366},
        {911,366},
        {1213,366},
        {1222,366},
        {1251,366},
        {1254,366},
        {1312,366},
        {1403,366},
        {1803,366},
        {1814,366},
        {1817,366},
        {1911,366},
        {1953,366},
        {2002,366},
        {2004,366},
        {2006,366},
        {2112,366},
        {2400,366},
        {2502,366},
        {2503,366},
        {2650,366},
        {2904,366},
        {2913,366},
        {2928,366},
        {3100,366},
        {102,382},
        {109,382},
        {162,382},
        {206,382},
        {207,382},
        {251,382},
        {252,382},
        {407,382},
        {553,382},
        {556,382},
        {705,382},
        {751,382},
        {813,382},
        {950,382},
        {1017,382},
        {1018,382},
        {1058,380},
        {1108,380},
        {1201,382},
        {1210,382},
        {1303,382},
        {1404,382},
        {1452,382},
        {1453,382},
        {1509,382},
        {1656,382},
        {1820,382},
        {1852,382},
        {1858,382},
        {2052,382},
        {2103,382},
        {2200,382},
        {2222,382},
        {2355,382},
        {2406,382},
        {2451,382},
        {2657,382},
        {2851,382},
        {2855,382},
        {2856,382},
        {2915,382},
        {3052,382},
        {55,398},
        {157,398},
        {405,398},
        {456,398},
        {505,398},
        {561,398},
        {566,398},
        {568,398},
        {569,398},
        {571,398},
        {613,398},
        {656,398},
        {657,398},
        {658,398},
        {659,398},
        {663,398},
        {753,398},
        {755,398},
        {806,398},
        {851,398},
        {855,398},
        {901,398},
        {913,398},
        {1054,398},
        {1104,398},
        {1153,398},
        {1218,398},
        {1313,398},
        {1363,398},
        {1408,398},
        {1414,398},
        {1424,398},
        {1456,398},
        {1600,398},
        {1601,398},
        {1602,398},
        {1700,398},
        {1701,398},
        {1702,398},
        {1810,398},
        {1901,398},
        {1913,398},
        {2113,398},
        {2163,398},
        {2212,398},
        {2213,398},
        {2352,398},
        {2353,398},
        {2551,398},
        {2601,398},
        {2658,398},
        {2706,398},
        {2906,398},
        {3005,398},
        {3055,398},
        {1205,384},
        {154,352},
        {1215,352},
        {1805,352},
        {1807,352},
        {1652,336},
        {2552,352},
    };

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
    }

    private void PSVSet(ref ff9.SVECTOR vRot, Int16 thetaX, Int32 b, Int16 thetaZ)
    {
        vRot.vx = thetaX;
        vRot.vy = (Int16)b;
        vRot.vz = thetaZ;
    }

    private void EBG_init()
    {
        this.EBG_stateInit();
        if (this.EBG_sceneInit() != 1)
        {
            return;
        }
        this.EBG_animationInit();
        this.EBG_attachInit();
    }

    private void EBG_stateInit()
    {
        this.flags = FieldMapFlags.GenericInitial;
        this.curVRP = Vector2.zero;
        this.charOffset = Vector2.zero;
        this.startPoint = Vector2.zero;
        this.endPoint = Vector2.zero;
        this.curFrame = 0;
        this.prevScr = Vector2.zero;
        this.charAimHeight = 324;
    }

    private Int32 EBG_sceneInit()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 70)
        {
            return 0;
        }
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.curCamIdx];
        Single num = (Single)((bgcam_DEF.vrpMinX + bgcam_DEF.vrpMaxX) / 2 - bgcam_DEF.centerOffset[0]) - HalfFieldWidth;
        Single num2 = (Single)((bgcam_DEF.vrpMinY + bgcam_DEF.vrpMaxY) / 2 + bgcam_DEF.centerOffset[1]) - HalfFieldHeight;
        Int32 index = 0;
        Single value = num;
        this.parallaxOrg[0] = value;
        this.curVRP[index] = value;
        Int32 index2 = 1;
        value = num2;
        this.parallaxOrg[1] = value;
        this.curVRP[index2] = value;
        this.scrollWindowPos = new Int16[4][];
        this.scrollWindowDim = new Int16[4][];
        this.scrollWindowAlphaX = new Int16[4];
        this.scrollWindowAlphaY = new Int16[4];
        for (Int32 i = 0; i < 4; i++)
        {
            this.scrollWindowPos[i] = new Int16[2];
            this.scrollWindowDim[i] = new Int16[2];
            this.scrollWindowPos[i][0] = 0;
            this.scrollWindowPos[i][1] = 0;
            this.scrollWindowDim[i][0] = bgcam_DEF.w;
            this.scrollWindowDim[i][1] = bgcam_DEF.h;
            this.scrollWindowAlphaX[i] = 256;
            this.scrollWindowAlphaY[i] = 256;
        }
        return 1;
    }

    private Int32 EBG_animationInit()
    {
        List<BGANIM_DEF> animList = this.scene.animList;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        BGSCENE_DEF bgscene_DEF = this.scene;
        Int32 animCount = (Int32)bgscene_DEF.animCount;
        for (Int32 i = 0; i < animCount; i++)
        {
            BGANIM_DEF bganim_DEF = animList[i];
            bganim_DEF.curFrame = 0;
            bganim_DEF.frameRate = 256;
            bganim_DEF.counter = 0;
            bganim_DEF.flags = 1;
            List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
            overlayList[(Int32)frameList[0].target].SetFlags(2, true);
        }
        return 1;
    }

    public Int32 EBG_attachInit()
    {
        this.attachCount = 0;
        for (UInt16 num = 0; num < 10; num = (UInt16)(num + 1))
        {
            this.attachList[(Int32)num] = new EBG_ATTACH_DEF();
            this.attachList[(Int32)num].surroundMode = -1;
            this.attachList[(Int32)num].r = 128;
            this.attachList[(Int32)num].g = 128;
            this.attachList[(Int32)num].b = 128;
            this.attachList[(Int32)num].ndx = -1;
            this.attachList[(Int32)num].x = 0;
            this.attachList[(Int32)num].y = 0;
        }
        return 1;
    }

    public Int32 EBG_sceneGetVRP(ref Int16 x, ref Int16 y)
    {
        if (this.scene.cameraList.Count <= 0)
        {
            return 0;
        }
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.curCamIdx];
        x = (Int16)(this.curVRP[0] + (Single)bgcam_DEF.centerOffset[0] + HalfFieldWidth);
        y = (Int16)(this.curVRP[1] - (Single)bgcam_DEF.centerOffset[1] + HalfFieldHeight);
        return 1;
    }

    public Int32 EBG_overlaySetActive(Int32 overlayNdx, Int32 activeFlag)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[overlayNdx];
        BGSCENE_DEF bgscene_DEF = this.scene;
        if (activeFlag != 0)
        {
            bgoverlay_DEF.SetFlags(2, true);
        }
        else
        {
            bgoverlay_DEF.SetFlags(2, false);
        }
        return 1;
    }

    public Int32 EBG_overlayDefineViewport(UInt32 viewportNdx, Int16 x, Int16 y, Int16 w, Int16 h)
    {
        this.scrollWindowPos[(Int32)viewportNdx][0] = x;
        this.scrollWindowPos[(Int32)viewportNdx][1] = y;
        this.scrollWindowDim[(Int32)viewportNdx][0] = w;
        this.scrollWindowDim[(Int32)viewportNdx][1] = h;
        return 1;
    }

    public Int32 EBG_overlayDefineViewportAlpha(UInt32 viewportNdx, Int32 alphaX, Int32 alphaY)
    {
        this.scrollWindowAlphaX[(Int32)((UIntPtr)viewportNdx)] = (Int16)alphaX;
        this.scrollWindowAlphaY[(Int32)((UIntPtr)viewportNdx)] = (Int16)alphaY;
        return 1;
    }

    public Int32 EBG_overlaySetViewport(UInt32 overlayNdx, UInt32 viewportNdx)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[(Int32)overlayNdx];
        bgoverlay_DEF.viewportNdx = (Byte)viewportNdx;
        return 1;
    }

    public Int32 EBG_overlaySetLoop(UInt32 overlayNdx, UInt32 flag, Int32 dx, Int32 dy)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[(Int32)overlayNdx];
        if (flag != 0u)
        {
            BGOVERLAY_DEF bgoverlay_DEF2 = bgoverlay_DEF;
            bgoverlay_DEF2.flags = (Byte)(bgoverlay_DEF2.flags | 4);
            if (this.scene.combineMeshes)
            {
                this.scene.CreateSeparateOverlay(this, this.UseUpscalFM, overlayNdx);
            }
        }
        else
        {
            BGOVERLAY_DEF bgoverlay_DEF3 = bgoverlay_DEF;
            bgoverlay_DEF3.flags = (Byte)(bgoverlay_DEF3.flags & 251);
        }
        bgoverlay_DEF.dX = (Int16)dx;
        bgoverlay_DEF.fracX = 0;
        bgoverlay_DEF.dY = (Int16)dy;
        bgoverlay_DEF.fracY = 0;
        return 1;
    }

    public Int32 EBG_overlaySetLoopType(UInt32 overlayNdx, UInt32 isScreenAnchored)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[(Int32)overlayNdx];
        if (isScreenAnchored != 0u)
        {
            BGOVERLAY_DEF bgoverlay_DEF2 = bgoverlay_DEF;
            bgoverlay_DEF2.flags = (Byte)(bgoverlay_DEF2.flags | 1);
        }
        else
        {
            BGOVERLAY_DEF bgoverlay_DEF3 = bgoverlay_DEF;
            bgoverlay_DEF3.flags = (Byte)(bgoverlay_DEF3.flags & 254);
        }
        return 1;
    }

    public Int32 EBG_overlaySetScrollWithOffset(UInt32 overlayNdx, UInt32 flag, Int32 delta, Int32 offset, UInt32 isXOffset)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[(Int32)overlayNdx];
        if (flag != 0u)
        {
            BGOVERLAY_DEF bgoverlay_DEF2 = bgoverlay_DEF;
            bgoverlay_DEF2.flags = (Byte)(bgoverlay_DEF2.flags | 128);
            if (this.scene.combineMeshes)
            {
                this.scene.CreateSeparateOverlay(this, this.UseUpscalFM, overlayNdx);
            }
        }
        else
        {
            BGOVERLAY_DEF bgoverlay_DEF3 = bgoverlay_DEF;
            bgoverlay_DEF3.flags = (Byte)(bgoverlay_DEF3.flags & 127);
        }
        if (isXOffset != 0u)
        {
            bgoverlay_DEF.dX = (Int16)offset;
            bgoverlay_DEF.dY = (Int16)delta;
            bgoverlay_DEF.isXOffset = 1;
        }
        else
        {
            bgoverlay_DEF.dX = (Int16)delta;
            bgoverlay_DEF.dY = (Int16)offset;
            bgoverlay_DEF.isXOffset = 0;
        }
        bgoverlay_DEF.fracX = 0;
        bgoverlay_DEF.fracY = 0;
        return 1;
    }

    public Int32 EBG_charAttachOverlay(UInt32 overlayNdx, Int16 attachX, Int16 attachY, SByte surroundMode, Byte r, Byte g, Byte b)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        this.attachList[(Int32)this.attachCount].ndx = (Int16)overlayNdx;
        this.attachList[(Int32)this.attachCount].x = attachX;
        this.attachList[(Int32)this.attachCount].y = attachY;
        this.attachList[(Int32)this.attachCount].surroundMode = surroundMode;
        if ((Int32)surroundMode >= 0)
        {
            this.attachList[(Int32)this.attachCount].r = r;
            this.attachList[(Int32)this.attachCount].g = g;
            this.attachList[(Int32)this.attachCount].b = b;
            this.CreateBorder((Int32)overlayNdx, r, g, b);
        }
        this.attachCount = (UInt16)(this.attachCount + 1);
        return 1;
    }

    public Int32 EBG_animAnimate(Int32 animNdx, Int32 frameNdx)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        BGANIM_DEF bganim_DEF2 = bganim_DEF;
        bganim_DEF2.flags = (Byte)(bganim_DEF2.flags | 2);
        bganim_DEF.curFrame = frameNdx << 8;
        bganim_DEF.counter = 0;
        return 1;
    }

    public Int32 EBG_animShowFrame(Int32 animNdx, Int32 frameNdx)
    {
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        BGSCENE_DEF bgscene_DEF = this.scene;
        for (Int32 i = 0; i < bganim_DEF.frameCount; i++)
        {
            BGANIMFRAME_DEF bganimframe_DEF = frameList[i];
            if (i == frameNdx)
            {
                overlayList[(Int32)bganimframe_DEF.target].SetFlags(2, true);
            }
            else
            {
                overlayList[(Int32)bganimframe_DEF.target].SetFlags(2, false);
            }
        }
        bganim_DEF.flags = 1;
        return 1;
    }

    public Int32 EBG_animSetActive(Int32 animNdx, Int32 flag)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        if (flag != 0)
        {
            BGANIM_DEF bganim_DEF2 = bganim_DEF;
            bganim_DEF2.flags = (Byte)(bganim_DEF2.flags | 6);
        }
        else
        {
            BGANIM_DEF bganim_DEF3 = bganim_DEF;
            bganim_DEF3.flags = (Byte)(bganim_DEF3.flags & 249);
        }
        return 1;
    }

    public Int32 EBG_animSetFrameRate(Int32 animNdx, Int32 frameRate)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        bganim_DEF.frameRate = (Int16)frameRate;
        bganim_DEF.CalculateActualFrameCount();
        return 1;
    }

    public Int32 EBG_animSetFrameWaitAll(UInt32 animNdx, Int32 frameWait)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF = this.scene.animList[(Int32)animNdx];
        List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
        Int32 num = bganim_DEF.frameCount;
        for (Int32 i = 0; i < num; i++)
        {
            frameList[i].value = (SByte)frameWait;
        }
        return 1;
    }

    public Int32 EBG_animSetFrameWait(Int32 animNdx, Int32 frameNdx, Int32 frameWait)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
        frameList[frameNdx].value = (SByte)frameWait;
        return 1;
    }

    public Int32 EBG_animSetFlags(Int32 animNdx, Int32 flags)
    {
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF2 = bganim_DEF;
        bganim_DEF2.flags = (Byte)(bganim_DEF2.flags | (Byte)(flags & 48));
        return 1;
    }

    public Int32 EBG_animSetPlayRange(Int32 animNdx, Int32 frameStart, Int32 frameEnd)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
        BGANIM_DEF bganim_DEF2 = bganim_DEF;
        bganim_DEF2.flags = (Byte)(bganim_DEF2.flags | 6);
        bganim_DEF.curFrame = frameStart << 8;
        bganim_DEF.counter = 0;
        frameList[frameEnd].value = -1;
        if ((bganim_DEF.frameRate > 0 && frameEnd < frameStart) || (bganim_DEF.frameRate < 0 && frameEnd > frameStart))
        {
            bganim_DEF.frameRate = (Int16)(-bganim_DEF.frameRate);
        }
        return 1;
    }

    public Int32 EBG_animSetVisible(Int32 animNdx, Int32 isVisible)
    {
        BGANIM_DEF bganim_DEF = this.scene.animList[animNdx];
        List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
        BGSCENE_DEF bgscene_DEF = this.scene;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        bganim_DEF.curFrame = 0;
        bganim_DEF.frameRate = 256;
        bganim_DEF.counter = 0;
        bganim_DEF.flags = 1;
        for (Int32 i = 0; i < bganim_DEF.frameCount; i++)
        {
            if (isVisible != 0 && i == 0)
            {
                overlayList[(Int32)frameList[i].target].SetFlags(2, true);
            }
            else
            {
                overlayList[(Int32)frameList[i].target].SetFlags(2, false);
            }
        }
        return 1;
    }

    public Int32 EBG_cameraSetViewport(UInt32 camNdx, Int16 minX, Int16 maxX, Int16 minY, Int16 maxY)
    {
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[(Int32)camNdx];
        BGSCENE_DEF bgscene_DEF = this.scene;
        bgcam_DEF.vrpMinX = (Int16)(minX + HalfFieldWidthNative);
        if (bgcam_DEF.vrpMinX > bgcam_DEF.w - HalfFieldWidthNative)
        {
            bgcam_DEF.vrpMinX = (Int16)(bgcam_DEF.w - HalfFieldWidthNative);
        }
        bgcam_DEF.vrpMaxX = (Int16)(maxX - HalfFieldWidthNative);
        if (bgcam_DEF.vrpMaxX < HalfFieldWidthNative)
        {
            bgcam_DEF.vrpMaxX = HalfFieldWidthNative;
        }
        bgcam_DEF.vrpMinY = (Int16)(minY + HalfFieldHeight);
        if (bgcam_DEF.vrpMinY > bgcam_DEF.h - HalfFieldHeight)
        {
            bgcam_DEF.vrpMinY = (Int16)(bgcam_DEF.h - HalfFieldHeight);
        }
        bgcam_DEF.vrpMaxY = (Int16)(maxY - HalfFieldHeight);
        if (bgcam_DEF.vrpMaxY < HalfFieldHeight)
        {
            bgcam_DEF.vrpMaxY = HalfFieldHeight;
        }
        return 1;
    }

    public bool EBG_isCombineMesh(BGOVERLAY_DEF overlayPtr)
    {
        return overlayPtr.transform.GetComponent<MeshRenderer>() != (UnityEngine.Object)null;
    }

    public int EBG_overlaySetSpriteShadeColorOffScreen(BGSCENE_DEF scenePtr, uint overlayNdx, byte r, byte g, byte b)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[(int)overlayNdx];
        List<BGSPRITE_LOC_DEF> spriteList = bgoverlay_DEF.spriteList;
        if (spriteList.Count == 0)
        {
            return 1;
        }
        if (this.EBG_isCombineMesh(bgoverlay_DEF))
        {
            Material material = bgoverlay_DEF.transform.gameObject.GetComponent<MeshRenderer>().material;
            material.SetColor("_Color", new Color((float)r / 128f, (float)g / 128f, (float)b / 128f, 1f));
            bgoverlay_DEF.transform.gameObject.GetComponent<MeshRenderer>().material = material;
        }
        else
        {
            Material material2 = spriteList[0].transform.gameObject.GetComponent<MeshRenderer>().material;
            ushort spriteCount = bgoverlay_DEF.spriteCount;
            int num;
            if (FF9StateSystem.Common.FF9.id != 0)
            {
                num = (int)spriteCount;
            }
            else
            {
                num = 0;
            }
            material2.SetColor("_Color", new Color((float)r / 128f, (float)g / 128f, (float)b / 128f, 1f));
            for (ushort num2 = 0; num2 < spriteCount; num2 = (ushort)(num2 + 1))
            {
                spriteList[(int)num2 + num].transform.gameObject.GetComponent<MeshRenderer>().material = material2;
            }
        }
        return 1;
    }

    public Int32 EBG_overlaySetShadeColor(UInt32 overlayNdx, Byte r, Byte g, Byte b)
    {
        BGSCENE_DEF scenePtr = this.scene;
        this.EBG_overlaySetSpriteShadeColorOffScreen(scenePtr, overlayNdx, r, g, b);
        return 1;
    }

    public Int32 EBG_overlayMove(Int32 overlayNdx, Int16 dx, Int16 dy, Int16 dz)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[overlayNdx];
        BGSCENE_DEF bgscene_DEF = this.scene;
        FieldMapInfo.fieldmapExtraOffset.UpdateOverlayOffset(this.mapName, overlayNdx, ref dz);
        Int16 num = (Int16)(bgoverlay_DEF.orgX + dx);
        Int16 num2 = (Int16)(bgoverlay_DEF.orgY + dy);
        
        // TODO Check Native: #147
        Int16 num3;
        if (FF9StateSystem.Common.FF9.fldMapNo == 2351 && overlayNdx >= 3 && overlayNdx <= 17)
        {
            num3 = 3000;
        }
        else
        {
            num3 = (Int16)(bgoverlay_DEF.orgZ + (UInt16)dz);
        }
        
        if (num < bgoverlay_DEF.minX)
        {
            num = bgoverlay_DEF.minX;
        }
        else if (num > bgoverlay_DEF.maxX)
        {
            num = bgoverlay_DEF.maxX;
        }
        if (num2 < bgoverlay_DEF.minY)
        {
            num2 = bgoverlay_DEF.minY;
        }
        else if (num2 > bgoverlay_DEF.maxY)
        {
            num2 = bgoverlay_DEF.maxY;
        }
        bgoverlay_DEF.orgX = num;
        bgoverlay_DEF.orgY = num2;
        bgoverlay_DEF.orgZ = (UInt16)num3;
        bgoverlay_DEF.curX = num;
        bgoverlay_DEF.curY = num2;
        bgoverlay_DEF.curZ = (UInt16)num3;
        bgoverlay_DEF.transform.localPosition = new Vector3((Single)num, (Single)num2, (Single)num3);
        return 1;
    }

    public Int32 EBG_overlaySetOrigin(Int32 overlayNdx, Int32 orgX, Int32 orgY)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGOVERLAY_DEF bgoverlay_DEF = bgscene_DEF.overlayList[overlayNdx];
        bgoverlay_DEF.curX = (Int16)orgX;
        bgoverlay_DEF.curY = (Int16)orgY;
        bgoverlay_DEF.orgX = bgoverlay_DEF.curX;
        bgoverlay_DEF.orgY = bgoverlay_DEF.curY;
        this.flags |= FieldMapFlags.Unknown128;
        return 1;
    }

    public Int32 EBG_overlaySetParallax(UInt32 overlayNdx, UInt32 flag, Int32 dx, Int32 dy)
    {
        BGSCENE_DEF bgscene_DEF = this.scene;
        BGOVERLAY_DEF bgoverlay_DEF = this.scene.overlayList[(Int32)overlayNdx];
        if (flag != 0u)
        {
            BGOVERLAY_DEF bgoverlay_DEF2 = bgoverlay_DEF;
            bgoverlay_DEF2.flags = (Byte)(bgoverlay_DEF2.flags | 8);
        }
        else
        {
            BGOVERLAY_DEF bgoverlay_DEF3 = bgoverlay_DEF;
            bgoverlay_DEF3.flags = (Byte)(bgoverlay_DEF3.flags & 247);
        }
        bgoverlay_DEF.dX = (Int16)dx;
        bgoverlay_DEF.dY = (Int16)dy;
        return 1;
    }

    public void UpdateOverlayAll()
    {
        if (this.scene.cameraList.Count <= 0)
        {
            return;
        }
        if (this.curCamIdx == -1)
        {
            return;
        }
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.curCamIdx];
        Vector2 zero = Vector2.zero;
        zero[0] = this.curVRP[0] + (float)bgcam_DEF.centerOffset[0] + HalfFieldWidth;
        zero[1] = this.curVRP[1] - (float)bgcam_DEF.centerOffset[1] + HalfFieldHeight;
        this.scene.scrX = (short)((float)this.scene.curX + HalfFieldWidth - zero[0]);
        this.scene.scrY = (short)((float)this.scene.curY + HalfFieldHeight - zero[1]);
        ushort overlayCount = this.scene.overlayCount;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        for (int i = 0; i < (int)overlayCount; i++)
        {
            this.UpdateOverlay((uint)i, overlayList[i], zero);
        }
    }

    private void UpdateOverlay(uint ovrNdx, BGOVERLAY_DEF overlayPtr, Vector2 realVrp)
    {
        ushort spriteCount = overlayPtr.spriteCount;
        List<BGSPRITE_LOC_DEF> spriteList = overlayPtr.spriteList;
        BGSCENE_DEF bgscene_DEF = this.scene;
        short num = (short)(overlayPtr.curX + bgscene_DEF.scrX);
        short num2 = (short)(overlayPtr.curY + bgscene_DEF.scrY);
        short num3 = (short)(overlayPtr.curZ + (ushort)bgscene_DEF.curZ);
        if ((overlayPtr.flags & 4) != 0)
        {
            short num4;
            short num5;
            if ((overlayPtr.flags & 1) != 0)
            {
                num4 = this.scrollWindowPos[(int)overlayPtr.viewportNdx][0];
                num5 = this.scrollWindowPos[(int)overlayPtr.viewportNdx][1];
            }
            else
            {
                num4 = (short)(HalfFieldWidth - realVrp[0] + (float)this.scrollWindowPos[(int)overlayPtr.viewportNdx][0]);
                num5 = (short)(HalfFieldHeight - realVrp[1] + (float)this.scrollWindowPos[(int)overlayPtr.viewportNdx][1]);
            }
            short num6 = this.scrollWindowDim[(int)overlayPtr.viewportNdx][0];
            short num7 = this.scrollWindowDim[(int)overlayPtr.viewportNdx][1];
            if (overlayPtr.dX < 0)
            {
                short num8 = (short)(256 - (overlayPtr.dX << 8 >> 8));
                num = (short)((((int)overlayPtr.curX << 8 | (int)overlayPtr.fracX) + (int)num8 >> 8) + (int)bgscene_DEF.scrX);
            }
            if (overlayPtr.dY < 0)
            {
                short num9 = (short)(256 - (overlayPtr.dX << 8 >> 8));
                num2 = (short)((((int)overlayPtr.curY << 8 | (int)overlayPtr.fracY) + (int)num9 >> 8) + (int)bgscene_DEF.scrY);
            }
            if (overlayPtr.dX != 0)
            {
                num = (short)((num - (num6 - (short)overlayPtr.w)) % (short)overlayPtr.w + (num6 - (short)overlayPtr.w));
            }
            if (overlayPtr.dY != 0)
            {
                num2 = (short)((num2 - (num7 - (short)overlayPtr.h)) % (short)overlayPtr.h + (num7 - (short)overlayPtr.h));
            }
            bool flag = this.mapName == "FBG_N18_GTRE_MAP360_GT_GRD_0";
            for (short num10 = 0; num10 < (short)spriteCount; num10 = (short)(num10 + 1))
            {
                BGSPRITE_LOC_DEF bgsprite_LOC_DEF = spriteList[(int)num10];
                Vector3 cacheLocalPos = bgsprite_LOC_DEF.cacheLocalPos;
                if ((overlayPtr.flags & 1) != 0)
                {
                    short num11 = (short)(num + (short)bgsprite_LOC_DEF.offX);
                    if (overlayPtr.dX != 0)
                    {
                        if (num11 + 16 >= (short)overlayPtr.w)
                        {
                            num11 = (short)(num11 - (short)overlayPtr.w);
                        }
                        else if (num11 <= -16)
                        {
                            num11 = (short)(num11 + (short)overlayPtr.w);
                        }
                    }
                    short num12 = (short)(num2 + (short)bgsprite_LOC_DEF.offY);
                    if (overlayPtr.dY != 0)
                    {
                        if (num12 + 16 >= (short)overlayPtr.h)
                        {
                            num12 = (short)(num12 - (short)overlayPtr.h);
                        }
                        else if (num12 <= -16)
                        {
                            num12 = (short)(num12 + (short)overlayPtr.h);
                        }
                    }
                    cacheLocalPos.x = (float)(num11 + num4);
                    cacheLocalPos.y = (float)(num12 + num5);
                }
                else
                {
                    short num11 = (short)(num + (short)bgsprite_LOC_DEF.offX);
                    if (overlayPtr.dX != 0)
                    {
                        if (num11 + 16 >= (short)overlayPtr.w)
                        {
                            num11 = (short)(num11 - (short)overlayPtr.w);
                        }
                        else if (num11 <= -16)
                        {
                            num11 = (short)(num11 + (short)overlayPtr.w);
                        }
                        cacheLocalPos.x = (float)(num11 + num4);
                    }
                    else
                    {
                        cacheLocalPos.x = (float)num11;
                    }
                    short num12 = (short)(num2 + (short)bgsprite_LOC_DEF.offY);
                    if (overlayPtr.dY != 0)
                    {
                        if (num12 + 16 >= (short)overlayPtr.h)
                        {
                            num12 = (short)(num12 - (short)overlayPtr.h);
                        }
                        else if (num12 <= -16)
                        {
                            num12 = (short)(num12 + (short)overlayPtr.h);
                        }
                        cacheLocalPos.y = (float)(num12 + num5);
                    }
                    else
                    {
                        cacheLocalPos.y = (float)num12;
                    }
                }
                cacheLocalPos.y += 16f;
                if (flag)
                {
                    cacheLocalPos.x += 8f;
                }
                cacheLocalPos.x -= (float)(this.scene.scrX + overlayPtr.curX);
                cacheLocalPos.y -= (float)(this.scene.scrY + overlayPtr.curY);
                bgsprite_LOC_DEF.cacheLocalPos = cacheLocalPos;
                bgsprite_LOC_DEF.transform.localPosition = cacheLocalPos;
            }
            overlayPtr.transform.localPosition = new Vector3((float)overlayPtr.curX * 1f, (float)overlayPtr.curY * 1f, overlayPtr.transform.localPosition.z);
        }
        else if ((overlayPtr.flags & 128) != 0)
        {
            short num4;
            short num5;
            if ((overlayPtr.flags & 1) != 0)
            {
                num4 = this.scrollWindowPos[(int)overlayPtr.viewportNdx][0];
                num5 = this.scrollWindowPos[(int)overlayPtr.viewportNdx][1];
            }
            else
            {
                num4 = (short)(HalfFieldWidth - realVrp[0] + (float)this.scrollWindowPos[(int)overlayPtr.viewportNdx][0]);
                num5 = (short)(HalfFieldHeight - realVrp[1] + (float)this.scrollWindowPos[(int)overlayPtr.viewportNdx][1]);
            }
            short num6 = this.scrollWindowDim[(int)overlayPtr.viewportNdx][0];
            short num7 = this.scrollWindowDim[(int)overlayPtr.viewportNdx][1];
            if (overlayPtr.isXOffset != 0)
            {
                num2 = (short)((num2 - (num7 - (short)overlayPtr.h)) % (short)overlayPtr.h + (num7 - (short)overlayPtr.h));
                num = (short)(num + num2 * overlayPtr.dX / (short)overlayPtr.h % (short)overlayPtr.w);
            }
            else
            {
                num = (short)((num - (num6 - (short)overlayPtr.w)) % (short)overlayPtr.w + (num6 - (short)overlayPtr.w));
                num2 = (short)(num2 + num * overlayPtr.dY / (short)overlayPtr.w % (short)overlayPtr.h);
            }
            for (short num10 = 0; num10 < (short)spriteCount; num10 = (short)(num10 + 1))
            {
                BGSPRITE_LOC_DEF bgsprite_LOC_DEF2 = spriteList[(int)num10];
                Vector3 localPosition = bgsprite_LOC_DEF2.transform.localPosition;
                if (overlayPtr.isXOffset != 0)
                {
                    short num13 = 0;
                    short num14 = (short)(num2 + (short)bgsprite_LOC_DEF2.offY);
                    if (num14 + 16 >= (short)overlayPtr.h)
                    {
                        num14 = (short)(num14 - (short)overlayPtr.h);
                        num13 = (short)(-overlayPtr.dX);
                    }
                    else if (num14 <= -16)
                    {
                        num14 = (short)(num14 + (short)overlayPtr.h);
                        num13 = overlayPtr.dX;
                    }
                    short num15 = (short)(num + (short)bgsprite_LOC_DEF2.offX + num13);
                    localPosition.x = (float)num15;
                    localPosition.y = (float)(num14 + num5);
                }
                else
                {
                    short num13 = 0;
                    short num15 = (short)(num + (short)bgsprite_LOC_DEF2.offX);
                    if (num15 + 16 >= (short)overlayPtr.w)
                    {
                        num15 = (short)(num15 - (short)overlayPtr.w);
                        num13 = (short)(-overlayPtr.dY);
                    }
                    else if (num15 <= -16)
                    {
                        num15 = (short)(num15 + (short)overlayPtr.w);
                        num13 = overlayPtr.dY;
                    }
                    short num14 = (short)(num2 + (short)bgsprite_LOC_DEF2.offY + num13);
                    localPosition.x = (float)(num15 + num4);
                    localPosition.y = (float)num14;
                }
                localPosition.y += 16f;
                localPosition.x -= (float)(this.scene.scrX + overlayPtr.curX);
                localPosition.y -= (float)(this.scene.scrY + overlayPtr.curY);
                bgsprite_LOC_DEF2.transform.localPosition = localPosition;
            }
            overlayPtr.transform.localPosition = new Vector3((float)overlayPtr.curX * 1f, (float)overlayPtr.curY * 1f, overlayPtr.transform.localPosition.z);
        }
        else
        {
            float num16;
            float num17;
            if ((overlayPtr.flags & 8) != 0 && overlayPtr.isSpecialParallax)
            {
                num16 = overlayPtr.parallaxCurX;
                num17 = overlayPtr.parallaxCurY;
            }
            else
            {
                num16 = (float)overlayPtr.curX;
                num17 = (float)overlayPtr.curY;
            }
            overlayPtr.transform.localPosition = new Vector3(num16 * 1f, num17 * 1f, overlayPtr.transform.localPosition.z);
        }
        overlayPtr.scrX = num;
        overlayPtr.scrY = num2;
    }

    public void UpdateSortingOrder(BGOVERLAY_DEF overlayPtr, BGSPRITE_LOC_DEF sprite, Int32 i)
    {
    }

    public void EBG_scene2DScroll(Int16 destX, Int16 destY, UInt16 frameCount, UInt32 scrollType)
    {
        if (!IsActive)
            return;

        this.startPoint[0] = (Single)((Int16)this.curVRP[0]);
        this.startPoint[1] = (Single)((Int16)this.curVRP[1]);
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.curCamIdx];

        if (Configuration.Graphics.WidescreenSupport)
        {
            if (destX > bgcam_DEF.vrpMaxX)
                destX = bgcam_DEF.vrpMaxX;
            else if (destX < bgcam_DEF.vrpMinX)
                destX = bgcam_DEF.vrpMinX;
        }

        this.endPoint[0] = (Single)destX;
        this.endPoint[1] = (Single)destY;
        this.frameCount = (Int16)frameCount;
        this.curFrame = 1;
        this.flags = (this.flags & ~FieldMapFlags.Generic15);
        if (scrollType == (UInt64)FieldMapFlags.RotationScroll)
        {
            IsRotationScroll = true;
        }
        this.flags |= FieldMapFlags.Unknown1;
    }

    public void EBG_scene2DScrollRelease(Int32 frameCount, UInt32 scrollType)
    {
        if (!IsActive)
            return;

        BGSCENE_DEF bgscene_DEF = this.scene;
        BGCAM_DEF bgcam_DEF = bgscene_DEF.cameraList[this.curCamIdx];
        this.startPoint[0] = (Single)((Int16)this.curVRP[0]);
        this.startPoint[1] = (Single)((Int16)this.curVRP[1]);
        BGCAM_DEF bgcam_DEF2 = this.scene.cameraList[this.camIdx];
        Vector3 vertex = Vector3.zero;
        if (FF9StateSystem.Common.FF9.fldMapNo == 1656 && this.playerController == (UnityEngine.Object)null)
        {
            this.playerController = ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(8)).fieldMapActorController;
            this.extraOffset = Vector2.zero;
        }
        if (this.playerController != (UnityEngine.Object)null)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 813 && this.playerController == ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(8)).fieldMapActorController)
            {
                this.playerController = ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(2)).fieldMapActorController;
            }
            Camera camera = this.GetMainCamera();
            vertex = this.playerController.curPos;
            vertex.y += (Single)this.charAimHeight;
            vertex = PSX.CalculateGTE_RTPT(vertex, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), this.offset);
        }
        else
        {
            vertex.x += this.offset.x;
            vertex.y += this.offset.y;
        }
        Single num = (Single)((Int32)vertex.x);
        Single num2 = (Single)((Int32)vertex.y);
        Single num3 = (Single)((bgcam_DEF.w >> 1) + (Int32)bgcam_DEF.centerOffset[0]) + (num - HalfFieldWidth);
        Single num4 = (Single)((bgcam_DEF.h >> 1) + (Int32)bgcam_DEF.centerOffset[1]) + (num2 - HalfFieldHeight);
        num3 -= this.offset.x - HalfFieldWidth;
        num4 += this.offset.y - HalfFieldHeight;
        num4 *= -1f;
        if (num3 < (Single)bgcam_DEF.vrpMinX)
        {
            num3 = (Single)bgcam_DEF.vrpMinX;
        }
        else if (num3 > (Single)bgcam_DEF.vrpMaxX)
        {
            num3 = (Single)bgcam_DEF.vrpMaxX;
        }
        if (num4 < (Single)bgcam_DEF.vrpMinY)
        {
            num4 = (Single)bgcam_DEF.vrpMinY;
        }
        else if (num4 > (Single)bgcam_DEF.vrpMaxY)
        {
            num4 = (Single)bgcam_DEF.vrpMaxY;
        }
        this.endPoint[0] = (Single)((Int16)num3);
        this.endPoint[1] = (Single)((Int16)num4);
        if (frameCount == -1)
            this.frameCount = 30;
        else
            this.frameCount = (Int16)frameCount;
        this.curFrame = 1;

        this.flags &= ~FieldMapFlags.Unknown4;
        this.flags |= FieldMapFlags.Unknown1 | FieldMapFlags.Unknown2;

        if (scrollType != UInt32.MaxValue)
            IsRotationScroll = scrollType == (UInt64)FieldMapFlags.RotationScroll;
    }

    public Int32 EBG_animationService()
    {
        for (Int32 i = 0; i < this.scene.animList.Count; i++)
        {
            Byte b = 4;
            BGANIM_DEF bganim_DEF = this.scene.animList[i];
            Int32 num = bganim_DEF.curFrame;
            if ((bganim_DEF.flags & 2) != 0 && (bganim_DEF.flags & 20) != 0 && (Int32)bganim_DEF.camNdx == this.camIdx && this.animIdx[i])
            {
                List<BGANIMFRAME_DEF> frameList = bganim_DEF.frameList;
                Int32 num2 = bganim_DEF.frameCount;
                for (Int32 j = 0; j < num2; j++)
                {
                    this.scene.overlayList[(Int32)frameList[j].target].SetFlags(2, false);
                }
                Byte target = frameList[num >> 8].target;
                this.scene.overlayList[(Int32)target].SetFlags(2, true);
                if ((Int32)frameList[num >> 8].value < 0)
                {
                    frameList[num >> 8].value = 0;
                    bganim_DEF.counter = 0;
                    BGANIM_DEF bganim_DEF2 = bganim_DEF;
                    bganim_DEF2.flags = (Byte)(bganim_DEF2.flags & (Byte)(~b));
                    return 1;
                }
                Int32 fastForwardFactor = FF9StateSystem.Settings.FastForwardFactor;
                Single num3 = 1f / (Single)fastForwardFactor;
                if (this.animIdx[i])
                {
                    BGANIM_DEF bganim_DEF3 = this.scene.animList[i];
                    bganim_DEF3.counter = (UInt16)(bganim_DEF3.counter + 1);
                }
                if ((Int32)bganim_DEF.counter >= (Int32)frameList[num >> 8].value)
                {
                    bganim_DEF.counter = 0;
                    if (fastForwardFactor != 1 && (bganim_DEF.frameCount % fastForwardFactor == 0 || fastForwardFactor % bganim_DEF.frameCount == 0))
                    {
                        bganim_DEF.curFrame = num + (Int32)((Single)bganim_DEF.frameRate * num3 * 2f);
                    }
                    else
                    {
                        bganim_DEF.curFrame = num + (Int32)bganim_DEF.frameRate;
                    }
                    if (bganim_DEF.curFrame >> 8 >= bganim_DEF.frameCount)
                    {
                        if ((bganim_DEF.flags & 32) != 0)
                        {
                            bganim_DEF.curFrame = bganim_DEF.frameCount - 1 << 8;
                            this.EBG_animSetFrameRate(i, (Int32)(-(Int32)bganim_DEF.frameRate));
                        }
                        else
                        {
                            bganim_DEF.curFrame = 0;
                        }
                        BGANIM_DEF bganim_DEF4 = bganim_DEF;
                        bganim_DEF4.flags = (Byte)(bganim_DEF4.flags & (Byte)(~b));
                    }
                    else if (bganim_DEF.curFrame >> 8 < 0)
                    {
                        if ((bganim_DEF.flags & 32) != 0)
                        {
                            bganim_DEF.curFrame = 0;
                            this.EBG_animSetFrameRate(i, (Int32)(-(Int32)bganim_DEF.frameRate));
                        }
                        else
                        {
                            bganim_DEF.curFrame = bganim_DEF.frameCount - 1 << 8;
                        }
                        BGANIM_DEF bganim_DEF5 = bganim_DEF;
                        bganim_DEF5.flags = (Byte)(bganim_DEF5.flags & (Byte)(~b));
                    }
                }
            }
        }
        return 1;
    }

    private Int32 EBG_attachService()
    {
        if (this.attachCount == 0)
        {
            return 0;
        }
        BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.camIdx];
        BGSCENE_DEF bgscene_DEF = this.scene;
        List<BGOVERLAY_DEF> overlayList = this.scene.overlayList;
        Vector3 vertex = this.playerController.curPos;
        vertex.y += (Single)this.charAimHeight;
        Camera camera = this.GetMainCamera();
        vertex = PSX.CalculateGTE_RTPT(vertex, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), this.offset);
        vertex.y *= -1f;
        EBG_ATTACH_DEF ebg_ATTACH_DEF = this.attachList[0];
        Byte b = 0;
        while ((UInt16)b < this.attachCount)
        {
            ebg_ATTACH_DEF = this.attachList[(Int32)b];
            UInt16 index = (UInt16)ebg_ATTACH_DEF.ndx;
            if ((overlayList[(Int32)index].flags & 2) != 0 && (Int32)overlayList[(Int32)index].camNdx == this.curCamIdx)
            {
                Int16 x = ebg_ATTACH_DEF.x;
                Int16 y = ebg_ATTACH_DEF.y;
                Int16 num = overlayList[(Int32)index].curX = (Int16)(vertex.x - (Single)bgscene_DEF.curX - (Single)x + (Single)bgcam_DEF.vrpMinX);
                Int16 num2 = overlayList[(Int32)index].curY = (Int16)(vertex.y - (Single)bgscene_DEF.curY - (Single)y + (Single)bgcam_DEF.vrpMinY);
                overlayList[(Int32)index].transform.localPosition = new Vector3((Single)num * 1f, (Single)num2 * 1f, 0f);
            }
            b = (Byte)(b + 1);
        }
        return 1;
    }

    public Int32 EBG_sceneServiceScroll(BGSCENE_DEF scenePtr)
    {
        Int32 overlayCount = (Int32)scenePtr.overlayCount;
        List<BGOVERLAY_DEF> overlayList = scenePtr.overlayList;
        for (Int32 i = 0; i < overlayCount; i++)
        {
            BGOVERLAY_DEF bgoverlay_DEF = overlayList[i];
            if ((bgoverlay_DEF.flags & 4) != 0)
            {
                if (bgoverlay_DEF.dX != 0 && bgoverlay_DEF.dX != 32767)
                {
                    Int32 num = (Int32)(bgoverlay_DEF.curX - bgoverlay_DEF.orgX) << 8 | (Int32)(bgoverlay_DEF.fracX & 255);
                    num += (Int32)bgoverlay_DEF.dX;
                    bgoverlay_DEF.curX = (Int16)((num >> 8) % (Int32)bgoverlay_DEF.w + (Int32)bgoverlay_DEF.orgX);
                    bgoverlay_DEF.fracX = (Int16)(num & 255);
                }
                if (bgoverlay_DEF.dY != 0 && bgoverlay_DEF.dY != 32767)
                {
                    Int32 num = (Int32)(bgoverlay_DEF.curY - bgoverlay_DEF.orgY) << 8 | (Int32)(bgoverlay_DEF.fracY & 255);
                    num += (Int32)bgoverlay_DEF.dY;
                    bgoverlay_DEF.curY = (Int16)((num >> 8) % (Int32)bgoverlay_DEF.h + (Int32)bgoverlay_DEF.orgY);
                    bgoverlay_DEF.fracY = (Int16)(num & 255);
                }
            }
            if ((bgoverlay_DEF.flags & 128) != 0)
            {
                if (bgoverlay_DEF.isXOffset != 0)
                {
                    if (bgoverlay_DEF.dY != 32767)
                    {
                        Int32 num = (Int32)bgoverlay_DEF.curY << 8 | (Int32)(bgoverlay_DEF.fracY & 255);
                        num += (Int32)bgoverlay_DEF.dY;
                        bgoverlay_DEF.curY = (Int16)((num >> 8) % (Int32)bgoverlay_DEF.h);
                        bgoverlay_DEF.fracY = (Int16)(num & 255);
                    }
                }
                else if (bgoverlay_DEF.dX != 32767)
                {
                    Int32 num = (Int32)bgoverlay_DEF.curX << 8 | (Int32)(bgoverlay_DEF.fracX & 255);
                    num += (Int32)bgoverlay_DEF.dX;
                    bgoverlay_DEF.curX = (Int16)((num >> 8) % (Int32)bgoverlay_DEF.w);
                    bgoverlay_DEF.fracX = (Int16)(num & 255);
                }
            }
            if ((bgoverlay_DEF.flags & 8) != 0)
            {
                Int32 num = (Int32)((Single)(bgoverlay_DEF.orgX << 8) + (this.curVRP[0] - this.parallaxOrg[0]) * (Single)bgoverlay_DEF.dX);
                bgoverlay_DEF.curX = (Int16)(num >> 8);
                bgoverlay_DEF.fracX = (Int16)(num & 255);
                num = (Int32)((Single)(bgoverlay_DEF.orgY << 8) + (this.curVRP[1] - this.parallaxOrg[1]) * (Single)bgoverlay_DEF.dY);
                bgoverlay_DEF.curY = (Int16)(num >> 8);
                bgoverlay_DEF.fracY = (Int16)(num & 255);
                if (FF9StateSystem.Common.FF9.fldMapNo == 1251 || FF9StateSystem.Common.FF9.fldMapNo == 150 || FF9StateSystem.Common.FF9.fldMapNo == 805 || FF9StateSystem.Common.FF9.fldMapNo == 808 || FF9StateSystem.Common.FF9.fldMapNo == 2953 || FF9StateSystem.Common.FF9.fldMapNo == 2952 || FF9StateSystem.Common.FF9.fldMapNo == 1009 || FF9StateSystem.Common.FF9.fldMapNo == 1108 || FF9StateSystem.Common.FF9.fldMapNo == 1758 || FF9StateSystem.Common.FF9.fldMapNo == 1651 || FF9StateSystem.Common.FF9.fldMapNo == 2851 || FF9StateSystem.Common.FF9.fldMapNo == 3100 || FF9StateSystem.Common.FF9.fldMapNo == 2720 || FF9StateSystem.Common.FF9.fldMapNo == 1908 || FF9StateSystem.Common.FF9.fldMapNo == 908)
                {
                    bgoverlay_DEF.isSpecialParallax = true;
                    Single num2 = (Single)(bgoverlay_DEF.orgX * 256) + (this.curVRP[0] - this.parallaxOrg[0]) * (Single)bgoverlay_DEF.dX;
                    bgoverlay_DEF.parallaxCurX = num2 / 256f;
                    bgoverlay_DEF.fracX = (Int16)((Int32)num2 & 255);
                    num2 = (Single)(bgoverlay_DEF.orgY * 256) + (this.curVRP[1] - this.parallaxOrg[1]) * (Single)bgoverlay_DEF.dY;
                    bgoverlay_DEF.parallaxCurY = num2 / 256f;
                    bgoverlay_DEF.fracY = (Int16)((Int32)num2 & 255);
                }
            }
        }
        if ((this.flags & FieldMapFlags.Unknown128) != 0u)
        {
            this.orgVRP[0] = this.curVRP[0];
            this.orgVRP[1] = this.curVRP[1];
            this.flags &= FieldMapFlags.Generic127;
        }
        return 1;
    }

    public void EBG_sceneService2DScroll()
    {
        if (!IsActive)
            return;

        FieldMapFlags fl = this.flags & FieldMapFlags.Generic7;
        if (fl == 0 || fl >= FieldMapFlags.Unknown4)
            return;

        Int16 currentFrame = this.curFrame;
        Int16 totalFrames = this.frameCount;
        BGCAM_DEF currentCamera = this.scene.cameraList[this.curCamIdx];
        Int16 aimX = (Int16)(this.endPoint.x - currentCamera.centerOffset[0] - HalfFieldWidth - this.startPoint.x);
        Int16 aimY = (Int16)(this.endPoint.y + currentCamera.centerOffset[1] - HalfFieldHeight - this.startPoint.y);
        Single viewportX = this.curVRP.x;
        Single viewportY = this.curVRP.y;
        if (IsRotationScroll)
        {
            Int32 fixedPointAngle = 2048 * currentFrame / totalFrames + 2048;
            Int32 rcos = ff9.rcos(fixedPointAngle) + 4096;
            Int32 rotX = aimX * rcos / 8192;
            Int32 rotY = aimY * rcos / 8192;
            this.curVRP[0] = this.startPoint[0] + rotX;
            this.curVRP[1] = this.startPoint[1] + rotY;
        }
        else
        {
            this.curVRP[0] = this.startPoint[0] + aimX * currentFrame / (Single)totalFrames;
            this.curVRP[1] = this.startPoint[1] + aimY * currentFrame / (Single)totalFrames;
        }

        viewportX = this.curVRP[0] - viewportX;
        viewportY = this.curVRP[1] - viewportY;

        UpdateOverlayXY((Int16)viewportX, (Int16)viewportY);

        this.charOffset = new Vector2(this.curVRP[0], this.curVRP[1]);

        if (fl == FieldMapFlags.Unknown1)
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

    private void UpdateOverlayXY(Int16 dx, Int16 dy)
    {
        for (Int32 overlayIndex = 0; overlayIndex < this.scene.overlayCount; overlayIndex++)
        {
            BGOVERLAY_DEF overlay = this.scene.overlayList[overlayIndex];
            if ((overlay.flags & 4) != 0)
            {
                if (overlay.dX != 0)
                    overlay.curX = (Int16)(overlay.curX + dx);
                if (overlay.dY != 0)
                    overlay.curY = (Int16)(overlay.curY + dy);

                this.EBG_alphaScaleX(overlay, dx);
                this.EBG_alphaScaleY(overlay, dy);
            }
            else if ((overlay.flags & 128) != 0)
            {
                if (overlay.isXOffset != 0)
                    overlay.curY = (Int16)(overlay.curY + dy);
                else
                    overlay.curX = (Int16)(overlay.curX + dx);

                this.EBG_alphaScaleX(overlay, dx);
                this.EBG_alphaScaleY(overlay, dy);
            }
        }
    }

    private void EBG_sceneService3DScroll()
    {
        if (!IsScene3dScrollAllowed())
            return;

        CrutchForIpsenMap(); // EVT_IPSEN_IP_CNT_2
        CrutchForEvaMap(); // EVT_EVA1_IF_PTS_1

        Vector3 prevScrOffset = Vector3.zero;
        BGCAM_DEF currentCamera = this.scene.cameraList[this.curCamIdx];

        if (this.playerController != null)
        {
            prevScrOffset = this.playerController.curPos;
            prevScrOffset.y += this.charAimHeight;
            prevScrOffset = PSX.CalculateGTE_RTPT(prevScrOffset, Matrix4x4.identity, currentCamera.GetMatrixRT(), currentCamera.GetViewDistance(), this.offset);
        }
        else
        {
            prevScrOffset.x += this.offset.x + this.extraOffset.x;
            prevScrOffset.y += this.offset.y + this.extraOffset.y;
        }

        this.prevScr = prevScrOffset;
        Single aimX = (currentCamera.w >> 1) + currentCamera.centerOffset[0] + (prevScrOffset.x - HalfFieldWidth);
        Single aimY = (currentCamera.h >> 1) + currentCamera.centerOffset[1] + (prevScrOffset.y - HalfFieldHeight);
        Single prevScrX = prevScrOffset.x;
        Single prevScrY = prevScrOffset.y;
        aimX -= this.offset.x - HalfFieldWidth;
        aimY += this.offset.y - HalfFieldHeight;
        aimY *= -1f;

        if (aimX < currentCamera.vrpMinX)
            prevScrX = this.offset.x + HalfFieldWidth - ((currentCamera.w >> 1) + currentCamera.centerOffset[0] + HalfFieldWidth - currentCamera.vrpMinX);
        else if (aimX > currentCamera.vrpMaxX)
            prevScrX = this.offset.x + HalfFieldWidth - ((currentCamera.w >> 1) + currentCamera.centerOffset[0] + HalfFieldWidth - currentCamera.vrpMaxX);

        if (aimY < currentCamera.vrpMinY)
            prevScrY = -(-this.offset.y + HalfFieldHeight - ((currentCamera.h >> 1) + currentCamera.centerOffset[1] + HalfFieldHeight - currentCamera.vrpMinY));
        else if (aimY > currentCamera.vrpMaxY)
            prevScrY = -(-this.offset.y + HalfFieldHeight - ((currentCamera.h >> 1) + currentCamera.centerOffset[1] + HalfFieldHeight - currentCamera.vrpMaxY));

        this.charOffset.x = prevScrX - currentCamera.centerOffset[0];
        this.charOffset.y = -(prevScrY - currentCamera.centerOffset[1]);

        Int16 dx, dy;
        this.EBG_lookAtPoint(currentCamera, aimX, aimY, out dx, out dy);

        UpdateOverlayXY(dx, dy);
    }

    private void CrutchForEvaMap()
    {
        const Int32 evaMapIndex = 1656; //EVT_EVA1_IF_PTS_1

        if (FF9StateSystem.Common.FF9.fldMapNo != evaMapIndex)
            return;

        Int32 isNeedOffset = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(7385);
        if (isNeedOffset == 1)
        {
            this.playerController = null;
            this.extraOffset.x = -16f;
            this.extraOffset.y = -8f;
        }
    }

    private void CrutchForIpsenMap()
    {
        const Int32 ipsenMapIndex = 2512; // EVT_IPSEN_IP_CNT_2

        if (FF9StateSystem.Common.FF9.fldMapNo == ipsenMapIndex && this.playerController == null)
        {
            this.playerController = ((Actor)PersistenSingleton<EventEngine>.Instance.GetObjUID(2)).fieldMapActorController;
        }
    }

    private Boolean IsScene3dScrollAllowed()
    {
        if ((this.flags & FieldMapFlags.Generic7) != 0u)
            return false;
        if (!IsActive)
            return false;
        if (FF9StateSystem.Common.FF9.fldMapNo == 70)
            return false;
        if (this.curCamIdx < 0 || this.curCamIdx > this.scene.cameraList.Count)
            return false;

        return true;
    }

    public static Int32 f1616_mul(Int32 a, Int32 b)
    {
        return Math3D.Float2Fixed(Math3D.Fixed2Float(a) * Math3D.Fixed2Float(b));
    }

    public Int16 EBG_alphaScaleX(BGOVERLAY_DEF oPtr, Int16 val)
    {
        Int32 num = (Int32)val << 16;
        Int32 num2 = (Int32)this.scrollWindowAlphaX[(Int32)oPtr.viewportNdx] << 8;
        if (num2 == 65536)
        {
            return oPtr.curX;
        }
        if (num2 < 0)
        {
            num2 = -num2;
            num = ((Int32)oPtr.curX << 16) - FieldMap.f1616_mul(num, num2);
            oPtr.curX = (Int16)(num >> 16);
            oPtr.fracX = (Int16)(num >> 8 & 255);
        }
        else
        {
            num = ((Int32)oPtr.curX << 16) + FieldMap.f1616_mul(num, num2);
            oPtr.curX = (Int16)(num >> 16);
            oPtr.fracX = (Int16)(num >> 8 & 255);
        }
        return oPtr.curX;
    }

    public Int16 EBG_alphaScaleY(BGOVERLAY_DEF oPtr, Int16 val)
    {
        Int32 num = (Int32)val << 16;
        Int32 num2 = (Int32)this.scrollWindowAlphaY[(Int32)oPtr.viewportNdx] << 8;
        if (num2 == 65536)
        {
            return oPtr.curY;
        }
        if (num2 < 0)
        {
            num2 = -num2;
            num = ((Int32)oPtr.curY << 16) - FieldMap.f1616_mul(num, num2);
            oPtr.curY = (Int16)(num >> 16);
            oPtr.fracY = (Int16)(num >> 8 & 255);
        }
        else
        {
            num = ((Int32)oPtr.curY << 16) + FieldMap.f1616_mul(num, num2);
            oPtr.curY = (Int16)(num >> 16);
            oPtr.fracY = (Int16)(num >> 8 & 255);
        }
        return oPtr.curY;
    }

    public Int32 EBG_lookAtPoint(BGCAM_DEF camPtr, Single aimX, Single aimY, out Int16 dX, out Int16 dY)
    {
        if (!IsActive)
        {
            dX = 0;
            dY = 0;
            return 1;
        }
        Single x = this.curVRP.x;
        Single y = this.curVRP.y;
        if (aimX < (Single)camPtr.vrpMinX)
        {
            this.curVRP[0] = (Single)camPtr.vrpMinX;
        }
        else if (aimX > (Single)camPtr.vrpMaxX)
        {
            this.curVRP[0] = (Single)camPtr.vrpMaxX;
        }
        else
        {
            this.curVRP[0] = aimX;
        }
        if (aimY < (Single)camPtr.vrpMinY)
        {
            this.curVRP[1] = (Single)camPtr.vrpMinY;
        }
        else if (aimY > (Single)camPtr.vrpMaxY)
        {
            this.curVRP[1] = (Single)camPtr.vrpMaxY;
        }
        else
        {
            this.curVRP[1] = aimY;
        }
        this.curVRP[0] = this.curVRP[0] - (Single)camPtr.centerOffset[0] - HalfFieldWidth;
        this.curVRP[1] = this.curVRP[1] + (Single)camPtr.centerOffset[1] - HalfFieldHeight;
        dX = (Int16)(this.curVRP.x - x);
        dY = (Int16)(this.curVRP.y - y);
        return 1;
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

        BGSCENE_DEF bgscene_DEF = this.scene;
        if (bgscene_DEF != null)
        {
            BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.curCamIdx];
            this.origVRPMinX = bgcam_DEF.vrpMinX;
            this.origVRPMaxX = bgcam_DEF.vrpMaxX;
            this.origVRPMinY = bgcam_DEF.vrpMinY;
            this.origVRPMaxY = bgcam_DEF.vrpMaxY;
            bgcam_DEF.vrpMinX = (Int16)this.SHRT_MIN;
            bgcam_DEF.vrpMaxX = (Int16)this.SHRT_MAX;
            bgcam_DEF.vrpMinY = (Int16)this.SHRT_MIN;
            bgcam_DEF.vrpMaxY = (Int16)this.SHRT_MAX;
            IsLocked = false;
        }
    }

    public void EBG_charLookAtLock()
    {
        if (IsLocked)
            return;

        BGSCENE_DEF bgscene_DEF = this.scene;
        if (bgscene_DEF != null)
        {
            BGCAM_DEF bgcam_DEF = this.scene.cameraList[this.curCamIdx];
            bgcam_DEF.vrpMinX = this.origVRPMinX;
            bgcam_DEF.vrpMaxX = this.origVRPMaxX;
            bgcam_DEF.vrpMinY = this.origVRPMinY;
            bgcam_DEF.vrpMaxY = this.origVRPMaxY;
            IsLocked = true;
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

    private Int32 lastFrame;

    private Int32[] frameCountList;

    private Single cumulativeTime;

    private Single frameTime;

    public Boolean isBattleBackupPos;

    private UInt16 attachCount;

    private EBG_ATTACH_DEF[] attachList;

    public Boolean UseUpscalFM;

    public FieldMapEditor fmEditor;

    private static readonly Dictionary<int, FieldMap.EbgCombineMeshData> combineMeshDict = new Dictionary<int, FieldMap.EbgCombineMeshData>
    {
        {
            351,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            358,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    13,
                    14
                }
            }
        },
        {
            450,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            407,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            55,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    5
                }
            }
        },
        {
            57,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            60,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    15,
                    16
                }
            }
        },
        {
            111,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    10
                }
            }
        },
        {
            153,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            154,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            307,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    6,
                    8
                }
            }
        },
        {
            308,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            309,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            507,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    8,
                    9,
                    10
                }
            }
        },
        {
            551,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    13
                }
            }
        },
        {
            556,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    27
                }
            }
        },
        {
            566,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            576,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            603,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    35
                }
            }
        },
        {
            612,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            662,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            705,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    25
                }
            }
        },
        {
            706,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    13
                }
            }
        },
        {
            707,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            751,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            755,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            766,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            802,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            810,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    6,
                    7
                }
            }
        },
        {
            815,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            910,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    12,
                    13,
                    14,
                    15,
                    16,
                    17,
                    19
                }
            }
        },
        {
            1910,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    12,
                    13,
                    14,
                    15,
                    16,
                    17,
                    19
                }
            }
        },
        {
            916,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            951,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            952,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            957,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1056,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    19,
                    24
                }
            }
        },
        {
            1106,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    19,
                    24
                }
            }
        },
        {
            1153,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    6,
                    7
                }
            }
        },
        {
            1206,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11
                }
            }
        },
        {
            1207,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    7,
                    8
                }
            }
        },
        {
            1214,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1215,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1222,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    21,
                    22,
                    23,
                    24,
                    25
                }
            }
        },
        {
            1223,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11
                }
            }
        },
        {
            1301,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    13
                }
            }
        },
        {
            1307,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1312,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1355,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1362,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1455,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            3054,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1505,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    0,
                    8,
                    13
                }
            }
        },
        {
            1950,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1225,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11
                }
            }
        },
        {
            1801,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11
                }
            }
        },
        {
            1802,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    7,
                    8
                }
            }
        },
        {
            3002,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    7,
                    8
                }
            }
        },
        {
            1806,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1807,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1814,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    21,
                    22,
                    23,
                    24,
                    25
                }
            }
        },
        {
            1816,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    15,
                    16,
                    17,
                    18,
                    19
                }
            }
        },
        {
            1823,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1852,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            1860,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    10
                }
            }
        },
        {
            1865,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    17,
                    18,
                    20
                }
            }
        },
        {
            2000,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2001,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    7,
                    8
                }
            }
        },
        {
            2101,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    13
                }
            }
        },
        {
            565,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2112,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            605,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2155,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2162,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2200,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    12,
                    13,
                    14,
                    15,
                    16,
                    17,
                    18,
                    19,
                    21,
                    22,
                    23,
                    24,
                    25
                }
            }
        },
        {
            2217,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2220,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    4,
                    5,
                    6,
                    7
                }
            }
        },
        {
            2221,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    2,
                    4,
                    5,
                    26,
                    29
                }
            }
        },
        {
            2404,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2453,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    1
                }
            }
        },
        {
            2853,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12,
                    13,
                    14,
                    15
                }
            }
        },
        {
            2502,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2506,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            2509,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    2,
                    3
                }
            }
        },
        {
            2652,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    4
                }
            }
        },
        {
            2906,
            (FieldMap.EbgCombineMeshData)null
        },
        {
            3100,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    38,
                    40,
                    46,
                    47,
                    48,
                    49,
                    50,
                    52
                }
            }
        },
        {
            2107,
            new FieldMap.EbgCombineMeshData
            {
                skipOverlayList = new List<int>
                {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    12
                }
            }
        }
    };

    public static List<String> fieldMapNameWithAreaTitle = new List<String>
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

    private FF9Char[] CharArray;

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
        if (Configuration.Graphics.InitializeWidescreenSupport() && IsNarrowMap())
        {
            foreach (KeyValuePair<int, int> entry in actualNarrowMapWidthDict)
            {
                if (FF9StateSystem.Common.FF9.fldMapNo == entry.Key)
                {
                    PsxFieldWidth = (Int16)(entry.Value);
                    PsxScreenWidth = PsxFieldWidth;
                }
            }
        }
        HalfFieldWidth = (Int16)(PsxFieldWidth / 2);
        HalfScreenWidth = (Int16)(PsxScreenWidth / 2);
        ShaderMulX = CalcShaderMulX();
        ShaderMulY = CalcShaderMulY();
        Shader.SetGlobalFloat("_MulX", ShaderMulX);
        Shader.SetGlobalFloat("_MulY", ShaderMulY);
    }

    private static Int16 CalcPsxFieldWidth() => Configuration.Graphics.WidescreenSupport ? (Int16)(PsxFieldHeightNative * Screen.width / Screen.height) : PsxFieldWidthNative;
    private static Int16 CalcPsxScreenWidth() => Configuration.Graphics.WidescreenSupport ? (Int16)(PsxScreenHeightNative * Screen.width / Screen.height) : PsxFieldWidthNative;
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
}
