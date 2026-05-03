using Memoria.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldMapActor : HonoBehavior
{
    public FieldMapActor()
    {
        this.charDef = new BGI_CHAR_DEF();
        this.charDef.actor = this;
        this.shadowHeightOffset = 5f;
        this._charRadius = 120f;
        this.shadowScale = Vector3.one * this._charRadius;
        this.shadowTran = (Transform)null;
        this.shadowZ = 0;
        this._rootBone000 = base.transform.FindChild("bone000");
        this._geoAttachData = new GeoActorAttachData();
        this._geoAttachData.parentActor = (FieldMapActor)null;
        this._geoAttachData.parentNode = (Transform)null;
        this._geoAttachData.childNode = (Transform)null;
        this._geoAttachData.attachOffset = Vector3.zero;
        this.forcedZ = 0;
    }

    public Single CharRadius
    {
        get
        {
            return this._charRadius;
        }
        set
        {
            this._charRadius = value;
        }
    }

    public static FieldMapActor CreateFieldMapActor(GameObject gObj, Actor actor)
    {
        FieldMapActor fieldMapActor = gObj.AddComponent<FieldMapActor>();
        actor.fieldMapActor = fieldMapActor;
        fieldMapActor.actor = actor;
        fieldMapActor.CharRadius = (Single)actor.collRad;
        fieldMapActor.meshRenderer = gObj.GetComponentsInChildren<Renderer>();
        fieldMapActor.CreateShadowMesh();
        return fieldMapActor;
    }

    public void SetRenderQueue(Int32 renderQueue)
    {
        SkinnedMeshRenderer[] componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
        {
            Material[] materials = componentsInChildren[i].materials;
            for (Int32 j = 0; j < (Int32)materials.Length; j++)
            {
                Material material = materials[j];
                material.renderQueue = renderQueue;
            }
        }
        MeshRenderer[] componentsInChildren2 = base.GetComponentsInChildren<MeshRenderer>();
        for (Int32 k = 0; k < (Int32)componentsInChildren2.Length; k++)
        {
            Material[] materials2 = componentsInChildren2[k].materials;
            for (Int32 l = 0; l < (Int32)materials2.Length; l++)
            {
                Material material2 = materials2[l];
                material2.renderQueue = renderQueue;
            }
        }
    }

    public override void HonoStart()
    {
        GameObject gameObject = GameObject.Find("FieldMap");
        if (gameObject != (UnityEngine.Object)null)
        {
            this._fieldMap = gameObject.GetComponent<FieldMap>();
        }
        if (!FF9StateSystem.Field.isDebugWalkMesh)
        {
            this.ForcedNonCullingMesh();
        }
        if (FF9StateSystem.Field.isDebug)
        {
            SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
            {
                Material[] materials = componentsInChildren[i].materials;
                for (Int32 j = 0; j < (Int32)materials.Length; j++)
                {
                    Material material = materials[j];
                    material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1f));
                }
            }
            MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
            for (Int32 k = 0; k < (Int32)componentsInChildren2.Length; k++)
            {
                Material[] materials2 = componentsInChildren2[k].materials;
                for (Int32 l = 0; l < (Int32)materials2.Length; l++)
                {
                    Material material2 = materials2[l];
                    material2.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1f));
                }
            }
        }
    }

    public override void HonoUpdate()
    {
    }

    public override void HonoLateUpdate()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 70) // Opening-For FMV
            return;
        Matrix4x4 cam = FF9StateSystem.Common.FF9.cam;
        UInt16 proj = FF9StateSystem.Common.FF9.proj;
        Vector2 projectionOffset = FF9StateSystem.Common.FF9.projectionOffset;
        Vector3 position = base.transform.position;
        this.projectedPos = PSX.CalculateGTE_RTPT_POS(position, Matrix4x4.identity, cam, (Single)proj, projectionOffset, true);
        this.projectedDepth = this.projectedPos.z / 4f + (Single)FF9StateSystem.Field.FF9Field.loc.map.charOTOffset;
        this.charOTOffset = FF9StateSystem.Field.FF9Field.loc.map.charOTOffset;
        //if (this.projectedDepth < 100f || this.projectedDepth > 3996f)
        //{
        //}
        Single psxZ = PSX.CalculateGTE_RTPTZ(base.transform.position, Matrix4x4.identity, cam, proj, projectionOffset);
        Single psxDepth = psxZ / 4f + (Single)FF9StateSystem.Field.FF9Field.loc.map.charOTOffset;
        if (this.meshRenderer != null)
        {
            if (this.actor != null)
            {
                if (psxDepth < 100f || psxDepth > 3996f)
                {
                    if (FF9StateSystem.Common.FF9.fldMapNo == 1413)
                    {
                        // Fossil Roo/Nest
                        FieldMapActorController component = base.GetComponent<FieldMapActorController>();
                        this.actor.frontCamera = component.originalActor.sid == 12; // Zidane
                    }
                    else if (FF9StateSystem.Common.FF9.fldMapNo == 1414)
                    {
                        // Fossil Roo/Nest
                        FieldMapActorController component2 = base.GetComponent<FieldMapActorController>();
                        this.actor.frontCamera = component2.originalActor.sid == 16; // Zidane
                    }
                    else if (FF9StateSystem.Common.FF9.fldMapNo != 2752 && FF9StateSystem.Common.FF9.fldMapNo != 1707)
                    {
                        // Invincible/Bridge or Mdn. Sari/Secret Room
                        this.actor.frontCamera = false;
                    }
                    return;
                }
                this.actor.frontCamera = true;
            }
            for (Int32 i = 0; i < this.meshRenderer.Length; i++)
            {
                Renderer renderer = this.meshRenderer[i];
                if (renderer.enabled)
                {
                    this.charPsxZ = MBG.MarkCharacterDepth ? 8f : psxZ;
                    this.charZ = (Single)(((Int32)this.charPsxZ / 4 + FF9StateSystem.Field.FF9Field.loc.map.charOTOffset) * 1);
                    this.charZ = (Single)(-(Single)((Int32)this.charZ));
                    if (FF9StateSystem.Common.FF9.fldMapNo == 2510 && this.actor.uid == 8)
                    {
                        // I. Castle/Mural Room, Water_Mirror
                        renderer.material.SetFloat("_CharZ", 20f);
                    }
                    if (FF9StateSystem.Common.FF9.fldMapNo == 2363 && this.actor.uid == 33)
                    {
                        // Gulug/Path, Thorn
                        renderer.material.SetFloat("_CharZ", 20f);
                    }
                }
            }
        }
        if (this.actor == null)
            return;
        FF9Shadow ff9Shadow = FF9StateSystem.Field.FF9Field.loc.map.shadowArray[this.actor.uid];
        Vector3 shadowOff = Vector3.zero;
        if (FF9StateSystem.Common.FF9.fldMapNo == 661 && this.actor.uid == 3)
            shadowOff = new Vector3(-39f, -14f, 80f); // Marsh/Master's House, Quale
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1659 && this.actor.uid == 128)
            shadowOff = new Vector3(0f, -66f, 0f); // Iifa Tree/Seashore, Queen_Brahne (1)
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1659 && this.actor.uid == 129)
            shadowOff = new Vector3(0f, -21f, 0f); // Iifa Tree/Seashore, Queen_Brahne (2)
        else if (FF9StateSystem.Common.FF9.fldMapNo == 2363 && (this.actor.uid == 16 || this.actor.uid == 15 || this.actor.uid == 32 || this.actor.uid == 33))
            shadowOff = new Vector3(0f, -15f, 0f); // Gulug/Path, Zorn or Thorn
        Vector3 shadowPos = this.GetShadowCurrentPos();
        if ((FF9StateSystem.Common.FF9.fldMapNo == 2107 && this.actor.uid == 5) || (FF9StateSystem.Common.FF9.fldMapNo == 2102 && this.actor.uid == 4))
        {
            // Lindblum/Square or Lindblum/Main Street, Pickaxe
            shadowPos = base.transform.position;
            shadowPos.y = 0f;
        }
        this.shadowTran.localPosition = shadowPos + new Vector3(ff9Shadow.xOffset, this.shadowHeightOffset * 1f, ff9Shadow.zOffset) + shadowOff;
        Single shZ = PSX.CalculateGTE_RTPTZ(this.shadowTran.position, Matrix4x4.identity, cam, (Single)proj, projectionOffset);
        shZ = (Int32)shZ / 4 + FF9StateSystem.Field.FF9Field.loc.map.charOTOffset;
        this.shadowZ = -(Int32)shZ;
        this.charAbsZ = base.transform.position.z;
    }

    private void LateUpdate()
    {
        if (!base.IsVisibled())
            return;
        this.UpdateGeoAttach();
    }

    public void UpdateGeoAttach()
    {
        if (this._geoAttachData.parentActor != (UnityEngine.Object)null)
        {
            base.transform.localPosition = this._geoAttachData.attachOffset;
            base.transform.localRotation = Quaternion.identity;
            base.transform.localScale = Vector3.one;
            this._geoAttachData.childNode.localPosition = Vector3.zero;
            this._geoAttachData.childNode.localRotation = Quaternion.identity;
            this._geoAttachData.childNode.localScale = Vector3.one;
        }
    }

    public void GeoAttachOffset(Vector3 attachOffset)
    {
        this._geoAttachData.attachOffset = attachOffset;
    }

    public void GeoAttach(FieldMapActor parentActor, Transform parentNode)
    {
        Transform childByName = base.transform.GetChildByName("bone000");
        if (childByName == (UnityEngine.Object)null)
        {
            return;
        }
        FieldMapActorController component = base.GetComponent<FieldMapActorController>();
        component.curPosBeforeAttach = component.curPos;
        component.localScaleBeforeAttach = base.transform.localScale;
        Vector3 curPos = component.curPos;
        component.curPos = Vector3.zero;
        component.lastPos = Vector3.zero;
        component.SyncPosToTransform();
        component.SetActive(false);
        if (FF9StateSystem.Common.FF9.fldMapNo >= 1400 && FF9StateSystem.Common.FF9.fldMapNo <= 1425)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 1412)
            {
                if (component.isPlayer)
                {
                    component.curPos = curPos;
                }
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo != 1410 && component.isPlayer)
            {
                component.curPos = parentNode.position;
            }
        }
        this._geoAttachData.parentActor = parentActor;
        this._geoAttachData.parentNode = parentNode;
        this._geoAttachData.childNode = childByName;
        Vector3 attachOffset = this._geoAttachData.attachOffset;
        bool flag = FF9StateSystem.Common.FF9.fldMapNo == 2954 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 8;
        this._geoAttachData.attachOffset = parentActor._geoAttachData.attachOffset;
        if (flag)
        {
            this._geoAttachData.attachOffset = new Vector3(30f, 150f, 0f);
        }
        base.transform.parent = parentNode.transform;
        base.transform.localPosition = this._geoAttachData.attachOffset;
        base.transform.localRotation = Quaternion.identity;
        base.transform.localScale = Vector3.one;
        this._geoAttachData.childNode.localPosition = Vector3.zero;
        this._geoAttachData.childNode.localRotation = Quaternion.identity;
        this._geoAttachData.childNode.localScale = Vector3.one;
    }

    public void GeoDetach(Boolean restorePosAndScale)
    {
        FieldMapActorController component = base.GetComponent<FieldMapActorController>();
        component.SetActive(true);
        if (this._geoAttachData.parentActor != (UnityEngine.Object)null)
        {
            base.transform.parent = this._geoAttachData.parentActor.transform.parent;
        }
        this.actor.go.transform.localRotation = Quaternion.AngleAxis(this.actor.rotAngle[2], Vector3.back) * Quaternion.AngleAxis(this.actor.rotAngle[1], Vector3.up) * Quaternion.AngleAxis(this.actor.rotAngle[0], Vector3.left);
        if (restorePosAndScale)
        {
            component.curPos = component.curPosBeforeAttach;
            base.transform.localPosition = component.curPos;
            base.transform.localScale = component.localScaleBeforeAttach;
        }
        else
        {
            base.transform.localScale = new Vector3(-1f, -1f, 1f);
        }
        this._geoAttachData.parentActor = (FieldMapActor)null;
        this._geoAttachData.parentNode = (Transform)null;
        if (FF9StateSystem.Common.FF9.fldMapNo == 3002 && component.originalActor.sid == 2 && component.originalActor.anim == 11022)
        {
            HonoBehaviorSystem.ExtraLoopCount = 1;
        }
    }

    private void ForcedNonCullingMesh()
    {
        Camera mainCamera = this._fieldMap.GetMainCamera();
        Vector3 position = mainCamera.transform.position;
        Vector3 a = Vector3.Normalize(mainCamera.transform.forward);
        Single d = (mainCamera.farClipPlane - mainCamera.nearClipPlane) / 2f + mainCamera.nearClipPlane;
        Vector3 position2 = position + a * d;
        Vector3 center = base.transform.InverseTransformPoint(position2);
        SkinnedMeshRenderer[] componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        SkinnedMeshRenderer[] array = componentsInChildren;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = array[i];
            skinnedMeshRenderer.localBounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
        }
        MeshFilter[] componentsInChildren2 = base.GetComponentsInChildren<MeshFilter>(true);
        MeshFilter[] array2 = componentsInChildren2;
        for (Int32 j = 0; j < (Int32)array2.Length; j++)
        {
            MeshFilter meshFilter = array2[j];
            meshFilter.sharedMesh.bounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
        }
        if (this.shadowTran != (UnityEngine.Object)null)
        {
            this.shadowTran.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
        }
    }

    private void CreateShadowMesh()
    {
        List<Vector3> list = new List<Vector3>();
        List<Color> list2 = new List<Color>();
        List<Vector2> list3 = new List<Vector2>();
        List<Int32> list4 = new List<Int32>();
        list.Add(new Vector3(-1f, 0f, -1f));
        list.Add(new Vector3(1f, 0f, -1f));
        list.Add(new Vector3(1f, 0f, 1f));
        list.Add(new Vector3(-1f, 0f, 1f));
        Color item = new Color(1f, 1f, 1f, 0.6f);
        list2.Add(item);
        list2.Add(item);
        list2.Add(item);
        list2.Add(item);
        list3.Add(new Vector2(0f, 0f));
        list3.Add(new Vector2(1f, 0f));
        list3.Add(new Vector2(1f, 1f));
        list3.Add(new Vector2(0f, 1f));
        list4.Add(2);
        list4.Add(1);
        list4.Add(0);
        list4.Add(3);
        list4.Add(2);
        list4.Add(0);
        Mesh mesh = new Mesh();
        mesh.vertices = list.ToArray();
        mesh.colors = list2.ToArray();
        mesh.uv = list3.ToArray();
        mesh.triangles = list4.ToArray();
        this.shadowObj = new GameObject(base.gameObject.name + "_Shadow");
        this.shadowObj.transform.parent = PersistenSingleton<EventEngine>.Instance.fieldmap.transform;
        MeshRenderer meshRenderer = this.shadowObj.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = this.shadowObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        Shader shader = ShadersLoader.Find("PSX/FieldMapActorShadow");
        Material material = new Material(shader);
        this.shadowTex = AssetManager.Load<Texture2D>("CommonAsset/Common/shadow_plate", false);
        material.mainTexture = this.shadowTex;
        meshRenderer.material = material;
        this.shadowTran = this.shadowObj.transform;
        this.shadowMeshRenderer = meshRenderer;
        this.shadowMeshRenderer.material.color = Color.black;
        this.shadowTran.position = this.GetShadowCurrentPos() + new Vector3(0f, this.shadowHeightOffset * 1f, 0f);
        this.shadowTran.localScale = this.shadowScale;
    }

    public void DestroySelfShadow()
    {
        UnityEngine.Object.Destroy(this.shadowObj);
    }

    public Vector3 GetShadowCurrentPos()
    {
        Vector3 vector = (!(this._rootBone000 != (UnityEngine.Object)null)) ? base.transform.localPosition : this._rootBone000.transform.position;
        Vector3 result = new Vector3(vector.x, base.transform.localPosition.y, vector.z);
        return result;
    }

    private const Int32 CharOTZBuffer = 100;

    public BGI_CHAR_DEF charDef;

    public Single shadowHeightOffset;

    public Texture2D shadowTex;

    public Transform shadowTran;

    public MeshRenderer shadowMeshRenderer;

    public Int32 shadowZ;

    private GameObject shadowObj;

    public Vector3 projectedPos;

    public Single projectedDepth;

    public Int32 charOTOffset;

    private Vector3 shadowScale;

    public Int32 forcedZ;

    public Single charZ;

    public Single charPsxZ;

    public Single charAbsZ;

    public Single charOffsetY;

    public Actor actor;

    public Single _charRadius;

    private FieldMap _fieldMap;

    private GeoActorAttachData _geoAttachData;

    public Renderer[] meshRenderer;

    private Transform _rootBone000;
}
