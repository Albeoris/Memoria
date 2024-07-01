using Memoria.Data;
using Memoria.Scripts;
using System;
using UnityEngine;

public class SHPEffect : MonoBehaviour
{
    public void Init(SHPPrototype prototype)
    {
        Int32 shaderIndex = Math.Min((Int32)prototype.ShaderType, 4);
        this.shpId = prototype.Id;
        this.attr = SPSConst.ATTR_VISIBLE;
        this.frame = 0;
        this.pos = Vector3.zero;
        this.scale = Vector3.one;
        this.rot = Vector3.zero;
        this.attach = null;
        this.posOffset = Vector3.zero;
        this.duration = -1;
        this.cycleDuration = Math.Max(1, prototype.CycleDuration);
        this.shpGo = new GameObject[prototype.TextureCount];
        for (Int32 i = 0; i < prototype.TextureCount; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.transform.parent = base.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = new Vector3(10f, 10f, 10f);
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            renderer.material = ShadersLoader.CreateShaderMaterial(SHPEffect._shaderNames[shaderIndex]);
            renderer.material.mainTexture = prototype.Textures[i];
            this.shpGo[i] = go;
            this.shpGo[i].SetActive(false);
        }
    }

    public void Unload()
    {
        this.shpId = -1;
        this.attr = 0;
        foreach (GameObject go in this.shpGo)
            go.SetActive(false);
    }

    public void AnimateSHP()
    {
        if ((this.attr & SPSConst.ATTR_VISIBLE) == 0)
            return;
        if ((this.attr & (SPSConst.ATTR_UPDATE_THIS_FRAME | SPSConst.ATTR_UPDATE_ANY_FRAME)) != 0)
        {
            if (this.attach != null)
                this.pos = this.attach.position + this.posOffset;
            Camera battleCamera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            Matrix4x4 cameraMatrix = battleCamera.worldToCameraMatrix.inverse;
            Single distanceToCamera = Vector3.Distance(cameraMatrix.GetColumn(3), this.pos);
            Single distanceFactor = Mathf.Clamp(distanceToCamera * 0.000259551482f, 0.3f, 1f);
            Vector3 directionForward = cameraMatrix.MultiplyVector(Vector3.forward);
            Vector3 directionRight = cameraMatrix.MultiplyVector(Vector3.right);
            Vector3 directionDown = Vector3.Cross(directionForward, directionRight);
            base.transform.localScale = this.scale * distanceFactor;
            base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z);
            base.transform.localPosition = this.pos;
            base.transform.LookAt(base.transform.position + directionForward, -directionDown);
            foreach (GameObject go in this.shpGo)
                go.SetActive(false);
            Int32 shpIndex = this.cycleDuration >= 0 ? (this.frame * this.shpGo.Length / this.cycleDuration) % this.shpGo.Length
                             : this.shpGo.Length - 1 + (this.frame * this.shpGo.Length / this.cycleDuration) % this.shpGo.Length;
            this.shpGo[shpIndex].SetActive(true);
            this.attr &= unchecked((Byte)~SPSConst.ATTR_UPDATE_THIS_FRAME);
        }
        else
        {
            foreach (GameObject go in this.shpGo)
                go.SetActive(false);
        }
    }

    public Int32 shpId = -1;

    public GameObject[] shpGo;

    public Byte attr;
    public Int32 frame;
    public Int32 duration;
    public Int32 cycleDuration;

    public Vector3 pos;
    public Vector3 scale;
    public Vector3 rot;
    public Transform attach;
    public Vector3 posOffset;

    private static String[] _shaderNames =
    [
        "PSX/FieldSPS_Abr_0",
        "PSX/FieldSPS_Abr_1",
        "PSX/FieldSPS_Abr_2",
        "PSX/FieldSPS_Abr_3",
        "PSX/FieldSPS_Abr_None"
    ];
}
