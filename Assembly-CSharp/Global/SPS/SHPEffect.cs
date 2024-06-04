using System;
using Memoria.Scripts;
using UnityEngine;

public class SHPEffect : MonoBehaviour
{
    public void Init(SHPEffect.Prototype shpEntry)
    {
        Int32 shaderIndex = Math.Min((Int32)shpEntry.abr, 4);
        this.cycleDuration = shpEntry.cycleDuration;
        this.shpGo = new GameObject[shpEntry.textures.Length];
        for (Int32 i = 0; i < shpEntry.textures.Length; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.transform.parent = base.transform;
            go.transform.localPosition = shpEntry.extraPos;
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = new Vector3(10f, 10f, 10f);
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            renderer.material = ShadersLoader.CreateShaderMaterial(SHPEffect._shaderNames[shaderIndex]);
            renderer.material.mainTexture = shpEntry.textures[i];
            this.shpGo[i] = go;
            this.shpGo[i].SetActive(false);
        }
        this.attr = 0;
        this.frame = 0;
        this.pos = Vector3.zero;
        this.scale = Vector3.one;
        this.rot = Vector3.zero;
    }

    public void AnimateSHP()
    {
        if ((this.attr & SPSConst.ATTR_VISIBLE) == 0)
            return;
        if ((this.attr & SPSConst.ATTR_UPDATE_PER_FRAME) != 0)
        {
            Camera battleCamera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            Matrix4x4 cameraMatrix = battleCamera.worldToCameraMatrix.inverse;
            Single distanceToCamera = Vector3.Distance(cameraMatrix.GetColumn(3), this.pos);
            Single distanceFactor = Mathf.Clamp(distanceToCamera * 0.000259551482f, 0.3f, 1f);
            base.transform.localScale = this.scale * distanceFactor;
            base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z);
            base.transform.localPosition = this.pos;
            Vector3 directionForward = cameraMatrix.MultiplyVector(Vector3.forward);
            Vector3 directionRight = cameraMatrix.MultiplyVector(Vector3.right);
            Vector3 directionDown = Vector3.Cross(directionForward, directionRight);
            base.transform.LookAt(base.transform.position + directionForward, -directionDown);
            foreach (GameObject go in this.shpGo)
                go.SetActive(false);
            Int32 shpIndex = (this.frame * this.shpGo.Length / this.cycleDuration) % this.shpGo.Length;
            this.shpGo[shpIndex].SetActive(true);
            this.attr &= unchecked((Byte)~SPSConst.ATTR_UPDATE_PER_FRAME);
        }
        else
        {
            foreach (GameObject go in this.shpGo)
                go.SetActive(false);
        }
    }

    public GameObject[] shpGo;

    public Byte attr;
    public Int32 frame;
    public Int32 cycleDuration;

    public Vector3 pos;
    public Vector3 scale;
    public Vector3 rot;

    private static String[] _shaderNames =
    [
        "PSX/FieldSPS_Abr_0",
        "PSX/FieldSPS_Abr_1",
        "PSX/FieldSPS_Abr_2",
        "PSX/FieldSPS_Abr_3",
        "PSX/FieldSPS_Abr_None"
    ];

    public class Prototype
    {
        public Prototype(String path, Int32 count, Byte abr, Vector3 extraPos, Int32 cycleDuration)
        {
            this.abr = abr;
            this.extraPos = extraPos;
            this.cycleDuration = cycleDuration;
            this.textures = new Texture2D[count];
            for (Int32 i = 0; i < count; i++)
            {
                this.textures[i] = AssetManager.Load<Texture2D>($"{path}_{i + 1}");
                if (this.textures[i] == null)
                    this.textures[i] = new Texture2D(0, 0);
            }
        }

        public Byte abr;
        public Vector3 extraPos;
        public Int32 cycleDuration;
        public Texture2D[] textures;
    }

    public static SHPEffect.Prototype[] Database =
    [
        new SHPEffect.Prototype("EmbeddedAsset/BattleMap/Status/Slow/Slow", 6, SPSConst.ABR_50ADD, new Vector3(212f, 0f, 0f), 9),
        new SHPEffect.Prototype("EmbeddedAsset/BattleMap/Status/Haste/Haste", 6, SPSConst.ABR_50ADD, new Vector3(-148f, 0f, 0f), 9),
        new SHPEffect.Prototype("EmbeddedAsset/BattleMap/Status/Silence/Silence", 3, SPSConst.ABR_50ADD, new Vector3(-92f, 0f, 0f), 9),
        new SHPEffect.Prototype("EmbeddedAsset/BattleMap/Status/Trouble/Trouble", 4, SPSConst.ABR_50ADD, new Vector3(92f, 0f, 0f), 9),
        //new SHPEffect.Prototype("customfireorb", "shp", 3, new Vector3(400f, 0f, 0f), 5f, 5f),
        //new SHPEffect.Prototype("customthunderorb", "shp", 4, new Vector3(-200f, 0f, -346.41f), 5f, 5f),
        //new SHPEffect.Prototype("customiceorb", "shp", 4, new Vector3(-200f, 0f, 346.41f), 5f, 5f)
    ];
}
