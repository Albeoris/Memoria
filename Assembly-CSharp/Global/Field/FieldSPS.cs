using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Scripts;
using UnityEngine;

public class FieldSPS : MonoBehaviour
{
    public void Init()
    {
        this.attr = 1;
        this.arate = 15;
        this.fade = 128;
        this.refNo = -1;
        this.charNo = -1;
        this.boneNo = 0;
        this.lastFrame = -1;
        this.curFrame = 0;
        this.frameCount = 0;
        this.frameRate = 16;
        this.pos = Vector3.zero;
        this.scale = 4096;
        this.rot = Vector3.zero;
        this.rotArg = Vector3.zero;
        this.zOffset = 0;
        this.posOffset = Vector3.zero;
        this.depthOffset = 0;
        this.spsIndex = -1;
        this.spsTransform = null;
        this.meshRenderer = null;
        this.meshFilter = null;
        this.spsBin = null;
        this.works = new FieldSPS.FieldSPSWork();
        this.spsPrims = new List<FieldSPS.FieldSPSPrim>();
        this.spsActor = null;
        this._vertices = new List<Vector3>();
        this._colors = new List<Color>();
        this._uv = new List<Vector2>();
        this._indices = new List<Int32>();
        this.materials =
        [
            new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_0")),
            new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_1")),
            new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_2")),
            new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_3")),
            new Material(ShadersLoader.Find("PSX/FieldSPS_Abr_None"))
        ];
        this.charTran = null;
        this.boneTran = null;
    }

    public void Unload()
    {
        this.spsBin = null;
        if (this.meshRenderer != null)
            this.meshRenderer.enabled = false;
        this.charTran = null;
        this.boneTran = null;
        this.pngTexture = null;
        this.tcbAreaComputed = false;
    }

    public Rect GetRelevantPartOfTCB()
    {
        if (tcbAreaComputed)
            return tcbArea;
        Single minx = Single.MaxValue;
        Single miny = Single.MaxValue;
        Single maxx = Single.MinValue;
        Single maxy = Single.MinValue;
        Int32 frameCnt = this.frameCount >> 4;
        Int32 framebackup = this.curFrame;
        for (Int32 frame = 0; frame < frameCnt; frame++)
        {
            this.curFrame = frame << 4;
            this.LoadSPS();
            foreach (FieldSPSPrim prim in this.spsPrims)
            {
                minx = Math.Min(minx, prim.uv0.x);
                miny = Math.Min(miny, prim.uv0.y);
                maxx = Math.Max(maxx, prim.uv3.x);
                maxy = Math.Max(maxy, prim.uv3.y);
            }
        }
        this.curFrame = framebackup;
        if (minx == Single.MaxValue)
            tcbArea = new Rect(0, 0, 0, 0);
        tcbArea = new Rect(minx, miny, maxx - minx, maxy - miny);
        tcbAreaComputed = true;
        return tcbArea;
    }

    public void GenerateSPS()
    {
        this.LoadSPS();
        this._GenerateSPSMesh();
    }

    public void LoadSPS()
    {
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.spsBin)))
        {
            Int32 frame = this.curFrame >> 4;
            Int32 frameCnt = binaryReader.ReadUInt16() & 0x7FFF;
            if (frame >= frameCnt)
                frame %= frameCnt;
            Int32 frameOffset = frame * 2 + 8;
            Int32 workOffset = frameCnt * 2 + 8;
            binaryReader.BaseStream.Seek(workOffset, SeekOrigin.Begin);
            Int32 rgbOffset = binaryReader.ReadUInt16();
            this.works.pt = workOffset + 2;
            this.works.rgb = this.works.pt + rgbOffset * 2 + 2;
            binaryReader.BaseStream.Seek(2, SeekOrigin.Begin);
            Byte shaderABR = this.arate > 3 ? Byte.MaxValue : this.arate;
            this.works.tpage.value = (UInt16)(binaryReader.ReadUInt16() | (shaderABR & 3) << 5);
            this.works.clut.value = binaryReader.ReadUInt16();
            this.works.w = (binaryReader.ReadByte() - 1) * 2;
            this.works.h = (binaryReader.ReadByte() - 1) * 2;
            this.works.code = (Byte)(0x2C | ((shaderABR != Byte.MaxValue) ? 2 : 0));
            this.works.fade = this.fade << 4;
            binaryReader.BaseStream.Seek(frameOffset, SeekOrigin.Begin);
            Int32 primOffset = binaryReader.ReadUInt16();
            binaryReader.BaseStream.Seek(primOffset, SeekOrigin.Begin);
            this.works.primCount = binaryReader.ReadByte();
            this._GenerateSPSPrims(binaryReader, primOffset + 1);
        }
    }

    private void _GenerateSPSPrims(BinaryReader reader, Int32 primOffset)
    {
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        FieldSPS.FieldSPSPrim prim = default;
        this.spsPrims.Clear();
        reader.BaseStream.Seek(primOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < this.works.primCount; i++)
        {
            prim.code = this.works.code;
            prim.tpage = this.works.tpage;
            prim.clut = this.works.clut;
            Int32 posX = reader.ReadSByte() << 2;
            Int32 posY = reader.ReadSByte() << 2;
            // w and h are switched there
            // More generally, the X/Y coordinates of TIM images (from PSX image format) seem to be mixed up in many places
            Int32 xmin = posX - this.works.h;
            Int32 xmax = posX + this.works.h;
            Int32 ymin = posY - this.works.w;
            Int32 ymax = posY + this.works.w;
            prim.v0 = new Vector3(xmin, ymin, 0f);
            prim.v1 = new Vector3(xmax, ymin, 0f);
            prim.v2 = new Vector3(xmin, ymax, 0f);
            prim.v3 = new Vector3(xmax, ymax, 0f);
            Byte texpos = reader.ReadByte();
            Int64 nextprimpos = reader.BaseStream.Position;
            Int32 uvpos = this.works.pt + ((texpos & 0xF) << 1);
            reader.BaseStream.Seek(uvpos, SeekOrigin.Begin);
            Int32 uvminx = reader.ReadByte();
            Int32 uvminy = reader.ReadByte();
            Int32 uvmaxx = uvminx + (this.works.h >> 1); // w and h are switched there as well
            Int32 uvmaxy = uvminy + (this.works.w >> 1);
            prim.uv0 = new Vector2(uvminx, uvminy);
            prim.uv1 = new Vector2(uvmaxx, uvminy);
            prim.uv2 = new Vector2(uvminx, uvmaxy);
            prim.uv3 = new Vector2(uvmaxx, uvmaxy);
            Int32 colpos = this.works.rgb + ((texpos >> 4) << 2);
            reader.BaseStream.Seek(colpos, SeekOrigin.Begin);
            UInt32 r = reader.ReadByte();
            UInt32 g = reader.ReadByte();
            UInt32 b = reader.ReadByte();
            Single basef = 127f;
            if (this.works.fade >= 0)
            {
                UInt32 ufade = (UInt32)this.works.fade;
                r = Math.Min(r * ufade >> 12, 0x7FFF);
                g = Math.Min(g * ufade >> 12, 0x7FFF);
                b = Math.Min(b * ufade >> 12, 0x7FFF);
                if ((fldMapNo == 2901 && (this.refNo == 644 || this.refNo == 736)) // Memoria/Entrance, save sphere
                 || (fldMapNo == 2913 && (this.refNo == 646 || this.refNo == 737)) // Memoria/Portal, save sphere
                 || (fldMapNo == 2925 && (this.refNo == 990 || this.refNo == 988))) // Crystal World, save sphere
                {
                    basef = 255f;
                }
            }
            prim.color = new Color(r / basef, g / basef, b / basef, 1f);
            prim.otz = 0;
            this.spsPrims.Add(prim);
            reader.BaseStream.Seek(nextprimpos, SeekOrigin.Begin);

            // TODO Check Native: #147
            // Alexandria Castle/Library (the 3 versions)
            if ((fldMapNo == 155 || fldMapNo == 1216 || fldMapNo == 1808) && base.name.Equals("SPS_0008"))
                this.zOffset = 700;
        }
    }

    private void _GenerateSPSMesh()
    {
        if (this.spsPrims.Count == 0)
            return;
        Boolean usePNGTexture = pngTexture != null;
        Boolean useScreenPositionHack = FF9StateSystem.Common.FF9.fldMapNo == 2929; // last/cw mbg a, teleportation SPS after Necron battle
        BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
        if (currentBgCamera == null)
            return;
        Single scalef = this.scale / 4096f;
        Matrix4x4 localRTS = Matrix4x4.identity;
        Boolean isBehindCamera = false;
        if (useScreenPositionHack)
        {
            localRTS = Matrix4x4.TRS(this.pos * 0.9925f, Quaternion.Euler(-this.rot.x / 2f, -this.rot.y / 2f, this.rot.z / 2f), new Vector3(scalef, -scalef, 1f));
        }
        else
        {
            Vector3 localPos = PSX.CalculateGTE_RTPT_POS(this.pos, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset(), true);
            scalef *= currentBgCamera.GetViewDistance() / localPos.z;
            if (localPos.z < 0f)
                isBehindCamera = true;
            localPos.z /= 4f;
            localPos.z += currentBgCamera.depthOffset;
            base.transform.localPosition = new Vector3(localPos.x, localPos.y, localPos.z + this.zOffset);
            base.transform.localScale = new Vector3(scalef, -scalef, 1f);
            base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, -this.rot.z);
        }
        this._vertices.Clear();
        this._colors.Clear();
        this._uv.Clear();
        this._indices.Clear();
        for (Int32 i = 0; i < this.spsPrims.Count; i++)
        {
            if (isBehindCamera)
                break;
            FieldSPS.FieldSPSPrim prim = this.spsPrims[i];
            Int32 vertCounter = this._vertices.Count;
            if (useScreenPositionHack)
            {
                Vector3 pv0 = PSX.CalculateGTE_RTPT(prim.v0, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
                Vector3 pv1 = PSX.CalculateGTE_RTPT(prim.v1, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
                Vector3 pv2 = PSX.CalculateGTE_RTPT(prim.v2, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
                Vector3 pv3 = PSX.CalculateGTE_RTPT(prim.v3, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
                Single shiftZ = PSX.CalculateGTE_RTPTZ(Vector3.zero, localRTS, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
                shiftZ /= 4f;
                shiftZ += currentBgCamera.depthOffset;
                Vector3 layerOrderShift = new Vector3(0f, 0f, shiftZ - i / 100f);
                this._vertices.Add(pv0 + layerOrderShift);
                this._vertices.Add(pv1 + layerOrderShift);
                this._vertices.Add(pv2 + layerOrderShift);
                this._vertices.Add(pv3 + layerOrderShift);
            }
            else
            {
                Vector3 layerOrderShift = new Vector3(0f, 0f, (this.spsPrims.Count - i) / 100f);
                this._vertices.Add(prim.v0 + layerOrderShift);
                this._vertices.Add(prim.v1 + layerOrderShift);
                this._vertices.Add(prim.v2 + layerOrderShift);
                this._vertices.Add(prim.v3 + layerOrderShift);
            }
            this._colors.Add(prim.color);
            this._colors.Add(prim.color);
            this._colors.Add(prim.color);
            this._colors.Add(prim.color);
            if (usePNGTexture)
            {
                Rect tcbArea = this.GetRelevantPartOfTCB();
                Vector2 pngRatio = new Vector2(1f / tcbArea.width, 1f / tcbArea.height);
                this._uv.Add(Vector2.Scale(prim.uv0 - tcbArea.min + new Vector2(0.5f, 0.5f), pngRatio));
                this._uv.Add(Vector2.Scale(prim.uv1 - tcbArea.min + new Vector2(-0.5f, 0.5f), pngRatio));
                this._uv.Add(Vector2.Scale(prim.uv2 - tcbArea.min + new Vector2(0.5f, -0.5f), pngRatio));
                this._uv.Add(Vector2.Scale(prim.uv3 - tcbArea.min + new Vector2(-0.5f, -0.5f), pngRatio));
            }
            else
            {
                this._uv.Add((prim.uv0 + new Vector2(0.5f, 0.5f)) * 0.00390625f); // 1/256 (texture dimension)
                this._uv.Add((prim.uv1 + new Vector2(-0.5f, 0.5f)) * 0.00390625f);
                this._uv.Add((prim.uv2 + new Vector2(0.5f, -0.5f)) * 0.00390625f);
                this._uv.Add((prim.uv3 + new Vector2(-0.5f, -0.5f)) * 0.00390625f);
            }
            this._indices.Add(vertCounter);
            this._indices.Add(vertCounter + 1);
            this._indices.Add(vertCounter + 2);
            this._indices.Add(vertCounter + 1);
            this._indices.Add(vertCounter + 3);
            this._indices.Add(vertCounter + 2);
        }
        Mesh mesh = this.meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = this._vertices.ToArray();
        mesh.colors = this._colors.ToArray();
        mesh.uv = this._uv.ToArray();
        mesh.triangles = this._indices.ToArray();
        this.meshFilter.mesh = mesh;
        PSX_TPage worktpage = this.works.tpage;
        PSX_Clut workclut = this.works.clut;
        Int32 shindex = Math.Min((Int32)this.arate, 4);
        if (usePNGTexture)
        {
            pngTexture.filterMode = FilterMode.Bilinear;
            this.materials[shindex].mainTexture = pngTexture;
        }
        else
        {
            PSXTexture texture = PSXTextureMgr.GetTexture(worktpage.FlagTP, worktpage.FlagTY, worktpage.FlagTX, workclut.FlagClutY, workclut.FlagClutX);
            texture.SetFilter(FilterMode.Bilinear);
            this.materials[shindex].mainTexture = texture.texture;
        }
        this.meshRenderer.material = this.materials[shindex];
        if (this.spsActor != null)
            this.spsActor.spsPos = this.pos;
    }

    public FieldMap fieldMap;

    public Int32 spsIndex;

    public Transform spsTransform;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public Byte[] spsBin;

    public FieldSPS.FieldSPSWork works;
    public List<FieldSPS.FieldSPSPrim> spsPrims;
    public FieldSPSActor spsActor;

    public Material[] materials;
    public Transform charTran;
    public Transform boneTran;

    public Byte attr;
    public Byte arate;
    public Byte fade;

    public Int32 refNo;
    public Int32 charNo;
    public Int32 boneNo;

    public Int32 lastFrame;
    public Int32 curFrame;
    public Int32 frameCount;
    public Int32 frameRate;

    public Vector3 pos;
    public Int32 scale;
    public Vector3 rot;
    public Int32 zOffset;
    public Vector3 rotArg;

    public Vector3 posOffset;
    public Int32 depthOffset;

    private List<Vector3> _vertices;
    private List<Color> _colors;
    private List<Vector2> _uv;
    private List<Int32> _indices;

    // Memoria fields
    [NonSerialized]
    public Texture2D pngTexture = null;
    [NonSerialized]
    private Boolean tcbAreaComputed = false;
    [NonSerialized]
    private Rect tcbArea = default;

    public class FieldSPSWork
    {
        public Int32 pt;
        public Int32 rgb;

        public Int32 w;
        public Int32 h;
        public PSX_TPage tpage;
        public PSX_Clut clut;

        public Int32 fade;
        public Int32 primCount;
        public Byte code;
    }

    public struct FieldSPSPrim
    {
        public Boolean FlagSemitrans => (this.code & 2) != 0;
        public Boolean FlagShadeTex => (this.code & 1) != 0;

        public Byte code;
        public PSX_TPage tpage;
        public PSX_Clut clut;
        public Int32 otz;
        public Color color;

        public Vector3 v0;
        public Vector2 uv0;

        public Vector3 v1;
        public Vector2 uv1;

        public Vector3 v2;
        public Vector2 uv2;

        public Vector3 v3;
        public Vector2 uv3;
    }

    // TODO: create a separate file for them and use them in other TIM image related structures
    public struct PSX_TPage
    {
        public Int32 FlagTP => this.value >> 7 & 3;
        public Int32 FlagABR => this.value >> 5 & 3;
        public Int32 FlagTY => this.value >> 4 & 1;
        public Int32 FlagTX => this.value & 0xF;

        public UInt16 value;
    }

    public struct PSX_Clut
    {
        public Int32 FlagClutY => this.value >> 6 & 0x1FF;
        public Int32 FlagClutX => this.value & 0x3F;

        public UInt16 value;
    }
}
