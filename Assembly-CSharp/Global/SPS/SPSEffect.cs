using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Assets;
using Memoria.Scripts;
using UnityEngine;

public class SPSEffect : MonoBehaviour
{
    public void Init(Int32 mode)
    {
        this.gMode = mode;
        this.attr = SPSConst.ATTR_VISIBLE;
        this.abr = SPSConst.ABR_OFF;
        this.fade = 128;
        this.spsId = -1;
        this.charNo = -1;
        this.boneNo = 0;
        this.lastFrame = -1;
        this.curFrame = 0;
        this.frameCount = 0;
        this.duration = -1;
        this.frameRate = SPSConst.FRAMERATE_ONE;
        this.prm0 = 0;
        this.pos = Vector3.zero;
        this.scale = SPSConst.SCALE_ONE;
        this.rot = Vector3.zero;
        this.rotArg = Vector3.zero;
        this.zOffset = 0;
        this.posOffset = Vector3.zero;
        this.depthOffset = 0;
        this.useBattleFactors = false;
        this.battleScaleFactor = 4f;
        this.battleDistanceFactor = 5f;
        this.spsIndex = -1;
        this.spsTransform = null;
        this.meshRenderer = null;
        this.meshFilter = null;
        this.spsBin = null;
        this.works = new SPSEffect.FieldSPSWork();
        this.works.wFactor = 1f;
        this.works.hFactor = 1f;
        this.spsPrims = new List<SPSEffect.FieldSPSPrim>();
        this.spsActor = null;
        this._vertices = new List<Vector3>();
        this._colors = new List<Color>();
        this._uv = new List<Vector2>();
        this._indices = new List<Int32>();
        switch (mode)
        {
            case 0: // Model Viewer
            case 1: // Field
                this.materials =
                [
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_0"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_1"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_2"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_3"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_None")
                ];
                break;
            case 2: // Battle
                this.materials =
                [
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_0"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_1"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_2"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_3"),
                    ShadersLoader.CreateShaderMaterial("PSX/FieldSPS_Abr_None")
                ];
                break;
            case 3: // World
                this.materials =
                [
                    ShadersLoader.CreateShaderMaterial("WorldMap/SPS_Abr_0"),
                    ShadersLoader.CreateShaderMaterial("WorldMap/SPS_Abr_1"),
                    ShadersLoader.CreateShaderMaterial("WorldMap/SPS_Abr_2"),
                    ShadersLoader.CreateShaderMaterial("WorldMap/SPS_Abr_3"),
                    ShadersLoader.CreateShaderMaterial("WorldMap/SPS_Abr_None")
                ];
                break;
        }
        this.charTran = null;
        this.boneTran = null;
        this.mapName = null;
        this.pngTexture = null;
        this.tcbAreaComputed = false;
    }

    public void Unload()
    {
        this.spsBin = null;
        if (this.meshRenderer != null)
            this.meshRenderer.enabled = false;
        this.charTran = null;
        this.boneTran = null;
        this.mapName = null;
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
        for (Int32 frame = 0; frame < frameCnt; frame++)
        {
            this.LoadSPS(frame);
            foreach (FieldSPSPrim prim in this.spsPrims)
            {
                minx = Math.Min(minx, prim.uv0.x);
                miny = Math.Min(miny, prim.uv0.y);
                maxx = Math.Max(maxx, prim.uv3.x);
                maxy = Math.Max(maxy, prim.uv3.y);
            }
        }
        if (minx == Single.MaxValue)
            tcbArea = new Rect(0, 0, 0, 0);
        tcbArea = new Rect(minx, miny, maxx - minx, maxy - miny);
        tcbAreaComputed = true;
        return tcbArea;
    }

    public Texture2D GetTextureFromCurrentTCB()
    {
        Rect spsArea = this.GetRelevantPartOfTCB();
        if (spsArea.width == 0 || spsArea.height == 0)
            return null;
        TIMUtils.TPage worktpage = this.works.tpage;
        TIMUtils.Clut workclut = this.works.clut;
        Texture2D fulltexture = PSXTextureMgr.GetTexture(worktpage.FlagTP, worktpage.FlagTY, worktpage.FlagTX, workclut.FlagClutY, workclut.FlagClutX)?.texture;
        if (fulltexture == null)
            return null;
        Color[] relevantPart = fulltexture.GetPixels((Int32)spsArea.x, (Int32)spsArea.y, (Int32)spsArea.width, (Int32)spsArea.height);
        Texture2D spstexture = new Texture2D((Int32)spsArea.width, (Int32)spsArea.height, TextureFormat.ARGB32, false);
        spstexture.SetPixels(relevantPart);
        spstexture.Apply();
        return spstexture;
    }

    public void GenerateSPS()
    {
        this.LoadSPS(this.curFrame >> 4);
        this._GenerateSPSMesh();
    }

    public void LoadSPS(Int32 frame)
    {
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.spsBin)))
        {
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
            Byte shaderABR = this.abr > 3 ? Byte.MaxValue : this.abr;
            this.works.tpage.value = (UInt16)(binaryReader.ReadUInt16() | (shaderABR & 3) << 5);
            this.works.clut.value = binaryReader.ReadUInt16();
            this.works.h = (binaryReader.ReadByte() - 1) * 2;
            this.works.w = (binaryReader.ReadByte() - 1) * 2;
            this.works.code = (Byte)(0x2C | ((shaderABR != Byte.MaxValue) ? 2 : 0));
            binaryReader.BaseStream.Seek(frameOffset, SeekOrigin.Begin);
            Int32 primOffset = binaryReader.ReadUInt16();
            binaryReader.BaseStream.Seek(primOffset, SeekOrigin.Begin);
            this.works.primCount = binaryReader.ReadByte();
            this._GenerateSPSPrims(binaryReader, primOffset + 1);
        }
    }

    private void _GenerateSPSPrims(BinaryReader reader, Int32 primOffset)
    {
        Int32 fldMapNo = this.gMode == 1 ? FF9StateSystem.Common.FF9.fldMapNo : -1;
        SPSEffect.FieldSPSPrim prim = default;
        this.spsPrims.Clear();
        reader.BaseStream.Seek(primOffset, SeekOrigin.Begin);
        Single distanceFactor = 1f;
        if (this.gMode == 2 && this.useBattleFactors)
        {
            Camera battleCamera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            Vector3 cameraPosition = battleCamera.worldToCameraMatrix.inverse.GetColumn(3);
            Single distanceToCamera = Vector3.Distance(cameraPosition, base.transform.localPosition);
            distanceFactor = Math.Max(0.33f, 1f / Math.Max(1f, distanceToCamera * this.battleDistanceFactor * 0.000259551482f));
        }
        Int32 halfWidth = (Int32)(this.works.w * this.works.wFactor);
        Int32 halfHeight = (Int32)(this.works.h * this.works.hFactor);
        for (Int32 i = 0; i < this.works.primCount; i++)
        {
            prim.code = this.works.code;
            prim.tpage = this.works.tpage;
            prim.clut = this.works.clut;
            Int32 posX = (Int32)((reader.ReadSByte() << 2) * distanceFactor);
            Int32 posY = (Int32)((reader.ReadSByte() << 2) * distanceFactor);
            Int32 xmin = posX - halfWidth;
            Int32 xmax = posX + halfWidth;
            Int32 ymin = posY - halfHeight;
            Int32 ymax = posY + halfHeight;
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
            Int32 uvmaxx = uvminx + (this.works.w >> 1);
            Int32 uvmaxy = uvminy + (this.works.h >> 1);
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
            if (this.fade >= 0)
            {
                UInt32 ufade = (UInt32)this.fade;
                r = Math.Min(r * ufade >> 8, 0x7FFF);
                g = Math.Min(g * ufade >> 8, 0x7FFF);
                b = Math.Min(b * ufade >> 8, 0x7FFF);
                if ((fldMapNo == 2901 && (this.spsId == 644 || this.spsId == 736)) // Memoria/Entrance, save sphere
                 || (fldMapNo == 2913 && (this.spsId == 646 || this.spsId == 737)) // Memoria/Portal, save sphere
                 || (fldMapNo == 2925 && (this.spsId == 990 || this.spsId == 988))) // Crystal World, save sphere
                {
                    basef = 255f;
                }
            }
            prim.color = new Color(r / basef, g / basef, b / basef, 1f);
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
        Boolean usePNGTexture = this.pngTexture != null;
        Boolean useScreenPositionHack = this.gMode == 1 && FF9StateSystem.Common.FF9.fldMapNo == 2929; // last/cw mbg a, teleportation SPS after Necron battle
        Single uvShrink = this.gMode <= 1 ? 0.5f : 0f;
        BGCAM_DEF currentBgCamera = null;
        if (this.gMode == 1)
        {
            currentBgCamera = this.fieldMap.GetCurrentBgCamera();
            if (currentBgCamera == null)
                return;
        }
        Single scalef = this.scale / 4096f;
        Matrix4x4 localRTS = Matrix4x4.identity;
        Boolean isBehindCamera = false;
        if (this.gMode == 3)
        {
            scalef *= 0.00390625f;
            base.transform.position = this.pos;
            base.transform.localScale = new Vector3(scalef, scalef, scalef);
        }
        else if (this.gMode == 2)
        {
            Camera battleCamera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            Matrix4x4 cameraMatrix = battleCamera.worldToCameraMatrix.inverse;
            Vector3 directionForward = cameraMatrix.MultiplyVector(Vector3.forward);
            Vector3 directionRight = cameraMatrix.MultiplyVector(Vector3.right);
            Vector3 directionDown = Vector3.Cross(directionForward, directionRight);
            if (this.useBattleFactors)
            {
                Single distanceToCamera = Vector3.Distance(cameraMatrix.GetColumn(3), this.pos);
                Single distanceFactor = Math.Min(3f, distanceToCamera * this.battleScaleFactor * 0.000259551482f);
                scalef *= distanceFactor;
            }
            base.transform.localScale = new Vector3(-scalef, -scalef, scalef);
            base.transform.localRotation = Quaternion.Euler(this.rot.x, this.rot.y, this.rot.z);
            base.transform.localPosition = this.pos;
            base.transform.LookAt(base.transform.position + directionForward, -directionDown);
        }
        else if (useScreenPositionHack)
        {
            localRTS = Matrix4x4.TRS(this.pos * 0.9925f, Quaternion.Euler(-this.rot.x / 2f, -this.rot.y / 2f, this.rot.z / 2f), new Vector3(scalef, -scalef, 1f));
        }
        else if (this.gMode == 1)
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
        else
        {
            Camera viewerCamera = Camera.main ? Camera.main : GameObject.Find("FieldMap Camera").GetComponent<Camera>();
            Matrix4x4 cameraMatrix = viewerCamera.worldToCameraMatrix.inverse;
            Vector3 directionForward = cameraMatrix.MultiplyVector(Vector3.forward);
            Vector3 directionRight = cameraMatrix.MultiplyVector(Vector3.right);
            Vector3 directionDown = Vector3.Cross(directionForward, directionRight);
            base.transform.localScale = new Vector3(-scalef, -scalef, scalef);
            base.transform.localPosition = this.pos;
            base.transform.LookAt(base.transform.position + directionForward, -directionDown);
        }
        this._vertices.Clear();
        this._colors.Clear();
        this._uv.Clear();
        this._indices.Clear();
        for (Int32 i = 0; i < this.spsPrims.Count; i++)
        {
            if (isBehindCamera)
                break;
            SPSEffect.FieldSPSPrim prim = this.spsPrims[i];
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
                if (this.useBattleFactors && this.pngTexture.width == 256 && this.pngTexture.height == 256)
                {
                    // In this situation, assume the status SPS texture is a VRam screenshot
                    this._uv.Add(prim.uv0 * 0.00390625f);
                    this._uv.Add(prim.uv1 * 0.00390625f);
                    this._uv.Add(prim.uv2 * 0.00390625f);
                    this._uv.Add(prim.uv3 * 0.00390625f);
                }
                else
                {
                    Rect tcbArea = this.GetRelevantPartOfTCB();
                    Vector2 pngRatio = new Vector2(1f / tcbArea.width, 1f / tcbArea.height);
                    this._uv.Add(Vector2.Scale(prim.uv0 - tcbArea.min + new Vector2(uvShrink, uvShrink), pngRatio));
                    this._uv.Add(Vector2.Scale(prim.uv1 - tcbArea.min + new Vector2(-uvShrink, uvShrink), pngRatio));
                    this._uv.Add(Vector2.Scale(prim.uv2 - tcbArea.min + new Vector2(uvShrink, -uvShrink), pngRatio));
                    this._uv.Add(Vector2.Scale(prim.uv3 - tcbArea.min + new Vector2(-uvShrink, -uvShrink), pngRatio));
                }
            }
            else
            {
                this._uv.Add((prim.uv0 + new Vector2(uvShrink, uvShrink)) * 0.00390625f); // 1/256 (texture dimension)
                this._uv.Add((prim.uv1 + new Vector2(-uvShrink, uvShrink)) * 0.00390625f);
                this._uv.Add((prim.uv2 + new Vector2(uvShrink, -uvShrink)) * 0.00390625f);
                this._uv.Add((prim.uv3 + new Vector2(-uvShrink, -uvShrink)) * 0.00390625f);
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
        TIMUtils.TPage worktpage = this.works.tpage;
        TIMUtils.Clut workclut = this.works.clut;
        Int32 shindex = Math.Min((Int32)this.abr, 4);
        if (usePNGTexture)
        {
            this.materials[shindex].mainTexture = this.pngTexture;
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

    /// <summary>Not really operational in the current state</summary>
    public void ExportAsSFXModel(String exportPath, String texturePath)
    {
        String shaderName = new String[] { "SFX_OPA_GT", "SFX_ADD_GT", "SFX_SUB_GT" }[Math.Min((Byte)2, this.abr)];
        String result = $"{{\r\n\t\"Sprite\":\r\n\t[{{\r\n\t\t\"Material\":\r\n\t\t{{\r\n\t\t\t\"TextureKind\":\"1\",\r\n\t\t\t\"TexturePath\":\"{texturePath}\",\r\n\t\t\t\"TextureParameter\":\"(0, 0, {this.pngTexture?.width ?? 256}, {this.pngTexture?.height ?? 256})\",\r\n\t\t\t\"Shader\":\"{shaderName}\",\r\n\t\t\t\"GlobalColor\":\"(1, 1, 1, 1)\",\r\n\t\t\t\"Threshold\":\"0.0295\"\r\n\t\t}},\r\n\t\t\"ScreenSize\":\"false\",\r\n";
        List<String> textureAnimStr = new List<String>();
        List<String> colorAnimStr = new List<String>();
        List<String> infoStr = new List<String>();
        Int32 frameCnt = this.frameCount >> 4;
        Vector3 firstPos = default;
        Vector3 lastPos = default;
        Boolean firstPosSet = false;
        if (this.works.w != this.works.h)
            result += $"\t\t\"Vertices\":[\"({-this.works.w}, {-this.works.h})\", \"({this.works.w}, {-this.works.h})\", \"({-this.works.w}, {this.works.h})\", \"({this.works.w}, {this.works.h})\"],\r\n";
        else
            result += $"\t\t\"Scale\":\"{this.works.w}\",\r\n";
        for (Int32 frame = 0; frame < frameCnt; frame++)
        {
            this.LoadSPS(frame);
            if (this.spsPrims.Count > 0)
            {
                Rect tcbArea = this.GetRelevantPartOfTCB();
                Vector4 colorVector = new Vector4(this.spsPrims[0].color.r, this.spsPrims[0].color.g, this.spsPrims[0].color.b, this.spsPrims[0].color.a);
                textureAnimStr.Add($"{{\r\n\t\t\t\"Frame\":\"{frame}\",\r\n\t\t\t\"UV\":[\"{this.spsPrims[0].uv0 - tcbArea.min:F0}\", \"{this.spsPrims[0].uv1 - tcbArea.min:F0}\", \"{this.spsPrims[0].uv2 - tcbArea.min:F0}\", \"{this.spsPrims[0].uv3 - tcbArea.min:F0}\"]\r\n\t\t}}");
                colorAnimStr.Add($"{{\r\n\t\t\t\"Frame\":\"{frame}\",\r\n\t\t\t\"VertexColors\":[\"{colorVector}\", \"{colorVector}\", \"{colorVector}\", \"{colorVector}\"]\r\n\t\t}}");
                infoStr.Add($"\t\t\t{{ \"Frame\":\"{frame}\", \"PrimCount\":\"{this.spsPrims.Count}\" }}");
                lastPos = (this.spsPrims[0].v0 + this.spsPrims[0].v3) / 2f;
                if (!firstPosSet)
                {
                    firstPos = lastPos;
                    firstPosSet = true;
                }
            }
        }
        result += "\t\t\"TextureAnimation\":\r\n\t\t[" + String.Join(",\r\n\t\t", textureAnimStr.ToArray()) + "],\r\n";
        result += "\t\t\"ColorAnimation\":\r\n\t\t[" + String.Join(",\r\n\t\t", colorAnimStr.ToArray()) + "],\r\n";
        result += $"\t\t\"Movement\":\r\n\t\t{{\r\n\t\t\t\"Duration\":\"{frameCnt}\",\r\n\t\t\t\"OriginX\":\"CasterPositionX + {firstPos.x:F0}\",\r\n\t\t\t\"OriginY\":\"CasterPositionY + {firstPos.y:F0}\",\r\n\t\t\t\"OriginZ\":\"CasterPositionZ + {firstPos.z:F0}\",\r\n\t\t\t\"DestinationX\":\"CasterPositionX + {lastPos.x:F0}\",\r\n\t\t\t\"DestinationY\":\"CasterPositionY + {lastPos.y:F0}\",\r\n\t\t\t\"DestinationZ\":\"CasterPositionZ + {lastPos.z:F0}\"\r\n\t\t}},\r\n";
        result += $"\t\t\"Duration\":\"{frameCnt}\",\r\n";
        result += $"\t\t\"Emission\":\r\n\t\t[{{\r\n\t\t\t\"Frame\":\"0\",\r\n\t\t\t\"Count\":\"1\",\r\n\t\t\t\"ParameterMin1\":\"0.0\",\r\n\t\t\t\"ParameterMax1\":\"1.0\"\r\n\t\t}}],\r\n";
        result += "\t\t\"Infos\":\r\n\t\t[\r\n" + String.Join(",\r\n", infoStr.ToArray()) + "\r\n\t\t]\r\n";
        result += $"\t}}]\r\n}}\r\n";
        File.WriteAllText(exportPath, result);
    }

    public Int32 gMode; // 1: Field - 2: Battle - 3: World
    public FieldMap fieldMap;

    public Int32 spsIndex;
    public Int32 spsId;
    public SPSConst.WorldSPSEffect worldSpsId
    {
        get
        {
            if (spsId == -1)
                return SPSConst.WorldSPSEffect.NOT_REGISTERED;
            if (String.Equals(mapName, "WorldMap"))
                return (SPSConst.WorldSPSEffect)spsId;
            return SPSConst.WorldSPSEffect.NOT_WORLD_SPS;
        }
    }

    public Transform spsTransform;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public Byte[] spsBin;

    public SPSEffect.FieldSPSWork works;
    public List<SPSEffect.FieldSPSPrim> spsPrims;
    public FieldSPSActor spsActor;

    public Material[] materials;
    public Transform charTran;
    public Transform boneTran;

    public Byte attr;
    public Byte abr;
    public Int32 fade;

    public Int32 charNo;
    public Int32 boneNo;

    public Int32 lastFrame;
    public Int32 curFrame;
    public Int32 frameCount;
    public Int32 frameRate;
    public Int32 duration;

    public Vector3 pos;
    public Int32 scale;
    public Vector3 rot;
    public Int32 zOffset;
    public Vector3 rotArg;
    public Vector3 posOffset;
    public Int32 depthOffset;
    public Boolean useBattleFactors;
    public Single battleDistanceFactor; // This doesn't change the scale of sprites but rather their closeness to each other
    public Single battleScaleFactor;
    public Int32 prm0; // Dummied? for twister circular movement

    private List<Vector3> _vertices;
    private List<Color> _colors;
    private List<Vector2> _uv;
    private List<Int32> _indices;

    public String mapName;
    public Texture2D pngTexture;
    private Boolean tcbAreaComputed;
    private Rect tcbArea;

    public Boolean _smoothUpdateRegistered = false;
    public Vector3 _smoothUpdatePosPrevious;
    public Vector3 _smoothUpdatePosActual;
    public Quaternion _smoothUpdateRotPrevious;
    public Quaternion _smoothUpdateRotActual;
    public Int32 _smoothUpdateScalePrevious;
    public Int32 _smoothUpdateScaleActual;

    public class FieldSPSWork
    {
        public Int32 pt;
        public Int32 rgb;

        public Int32 h;
        public Int32 w;
        public TIMUtils.TPage tpage;
        public TIMUtils.Clut clut;

        public Int32 primCount;
        public Byte code;
        public Single wFactor;
        public Single hFactor;
    }

    public struct FieldSPSPrim
    {
        public Boolean FlagSemitrans => (this.code & 2) != 0;
        public Boolean FlagShadeTex => (this.code & 1) != 0;

        public Byte code;
        public TIMUtils.TPage tpage;
        public TIMUtils.Clut clut;
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
}
