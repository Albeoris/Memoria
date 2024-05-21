using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldSPSSystem : HonoBehavior
{
    public override void HonoUpdate()
    {
        this.Service();
    }

    private void LateUpdate()
    {
        if (!PersistenSingleton<UIManager>.Instance.IsPause)
            this.GenerateSPS();
    }

    private void InitSPSInstance(Int32 index)
    {
        if (index < 0 || index > this._spsList.Count)
            return;
        if (index < this._spsList.Count)
        {
            this._spsList[index].Init();
            this._spsList[index].fieldMap = this._fieldMap;
            return;
        }
        GameObject spsGo = new GameObject($"SPS_{index:D4}");
        spsGo.transform.parent = base.transform;
        spsGo.transform.localScale = Vector3.one;
        spsGo.transform.localPosition = Vector3.zero;
        MeshRenderer meshRenderer = spsGo.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = spsGo.AddComponent<MeshFilter>();
        FieldSPS fieldSPS = spsGo.AddComponent<FieldSPS>();
        fieldSPS.Init();
        fieldSPS.fieldMap = this._fieldMap;
        fieldSPS.spsIndex = index;
        fieldSPS.spsTransform = spsGo.transform;
        fieldSPS.meshRenderer = meshRenderer;
        fieldSPS.meshFilter = meshFilter;
        FieldSPSActor fieldSPSActor = spsGo.AddComponent<FieldSPSActor>();
        fieldSPSActor.sps = fieldSPS;
        fieldSPS.spsActor = fieldSPSActor;
        this._spsList.Add(fieldSPS);
    }

    public void Init(FieldMap fieldMap)
    {
        this.rot = new Vector3(0f, 0f, 0f);
        this._isReady = false;
        this._spsList = new List<FieldSPS>();
        this._spsBinDict = new Dictionary<Int32, KeyValuePair<String, Byte[]>>();
        this._fieldMap = fieldMap;
        Int32 initCount = Math.Max(FieldSPSConst.FF9FIELDSPS_DEFAULT_OBJCOUNT, this._spsList.Count);
        for (Int32 i = 0; i < initCount; i++)
            this.InitSPSInstance(i);
        this.MapName = FF9StateSystem.Field.SceneName;
        FieldMapInfo.fieldmapSPSExtraOffset.SetSPSOffset(this.MapName, this._spsList);
        this._isReady = this._loadSPSTexture();
    }

    public void ChangeFieldOrigin(String newMapName)
    {
        foreach (FieldSPS fieldSPS in this._spsList)
            if (fieldSPS.pngTexture == null)
                fieldSPS.Unload();
        this.MapName = newMapName;
        FieldMapInfo.fieldmapSPSExtraOffset.SetSPSOffset(this.MapName, this._spsList);
        this._isReady = this._loadSPSTexture();
    }

    public void Service()
    {
        if (!this._isReady)
            return;
        for (Int32 i = 0; i < this._spsList.Count; i++)
        {
            FieldSPS fieldSPS = this._spsList[i];
            if (fieldSPS.spsBin != null && (fieldSPS.attr & FieldSPSConst.FF9FIELDSPSOBJ_ATTR_VISIBLE) != 0)
            {
                if (fieldSPS.lastFrame != -1)
                {
                    fieldSPS.lastFrame = fieldSPS.curFrame;
                    fieldSPS.curFrame += fieldSPS.frameRate;
                    if (fieldSPS.curFrame >= fieldSPS.frameCount)
                        fieldSPS.curFrame = 0;
                    else if (fieldSPS.curFrame < 0)
                        fieldSPS.curFrame = (fieldSPS.frameCount >> 4) - 1 << 4;
                }
            }
        }
    }

    public void GenerateSPS()
    {
        if (!this._isReady)
            return;
        for (Int32 i = 0; i < this._spsList.Count; i++)
        {
            FieldSPS fieldSPS = this._spsList[i];
            if (fieldSPS.spsBin != null && (fieldSPS.attr & FieldSPSConst.FF9FIELDSPSOBJ_ATTR_VISIBLE) != 0)
            {
                if (fieldSPS.charTran != null && fieldSPS.boneTran != null)
                {
                    FieldMapActor component = fieldSPS.charTran.GetComponent<FieldMapActor>();
                    if (component != null)
                        component.UpdateGeoAttach();
                    fieldSPS.pos = fieldSPS.boneTran.position + fieldSPS.posOffset;
                }
                fieldSPS.GenerateSPS();
                fieldSPS.lastFrame = fieldSPS.curFrame;
                fieldSPS.meshRenderer.enabled = true;
            }
        }
    }

    private Boolean _loadSPSTexture()
    {
        Byte[] binAsset = AssetManager.LoadBytes("FieldMaps/" + this.MapName + "/spt.tcb");
        if (binAsset == null)
            return false;
        FieldSPSSystem._loadTCB(binAsset);
        return true;
    }

    private Boolean _loadSPSBin(Int32 spsNo)
    {
        if (this._spsBinDict.ContainsKey(spsNo))
            return true;
        Byte[] binAsset = AssetManager.LoadBytes($"FieldMaps/{this.MapName}/{spsNo}.sps");
        if (binAsset == null)
            return false;
        this._spsBinDict.Add(spsNo, new KeyValuePair<String, Byte[]>(this.MapName, binAsset));
        return true;
    }

    public void FF9FieldSPSSetObjParm(Int32 ObjNo, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
    {
        if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FIELD)
        {
            this.ChangeFieldOrigin(EventEngineUtils.eventIDToFBGID[Arg0]);
            return;
        }
        if (ObjNo == this._spsList.Count)
            this.InitSPSInstance(ObjNo);
        if (ObjNo < 0 || ObjNo >= this._spsList.Count)
            return;
        FieldSPS fieldSPS = this._spsList[ObjNo];
        if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_REF)
        {
            if (Arg0 != FieldSPSConst.FF9FIELDSPS_PARM_REF_DELETE)
            {
                if (this._loadSPSBin(Arg0))
                {
                    fieldSPS.mapName = this._spsBinDict[Arg0].Key;
                    fieldSPS.spsBin = this._spsBinDict[Arg0].Value;
                    fieldSPS.curFrame = 0;
                    fieldSPS.lastFrame = -1;
                    fieldSPS.frameCount = FieldSPSSystem._getSpsFrameCount(fieldSPS.spsBin);
                }
                fieldSPS.refNo = Arg0;
                fieldSPS.LoadTexture();
                if (FF9StateSystem.Common.FF9.fldMapNo == 2553 && (fieldSPS.refNo == 464 || fieldSPS.refNo == 467 || fieldSPS.refNo == 506 || fieldSPS.refNo == 510))
                {
                    // Wind Shrine/Interior
                    fieldSPS.spsBin = null;
                }
            }
            else
            {
                if ((FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911) && (fieldSPS.refNo == 33 || fieldSPS.refNo == 34))
                {
                    // Treno/Queen's House
                    fieldSPS.pos = Vector3.zero;
                    fieldSPS.scale = FieldSPSConst.FF9FIELDSPS_PARM_SCALE_ONE;
                    fieldSPS.rot = Vector3.zero;
                    fieldSPS.rotArg = Vector3.zero;
                }
                fieldSPS.Unload();
            }
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_ATTR)
        {
            if (Arg1 == 0)
                fieldSPS.attr &= (Byte)~Arg0;
            else
                fieldSPS.attr |= (Byte)Arg0;

            if ((fieldSPS.attr & FieldSPSConst.FF9FIELDSPSOBJ_ATTR_VISIBLE) == 0)
            {
                fieldSPS.meshRenderer.enabled = false;
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 2928 || FF9StateSystem.Common.FF9.fldMapNo == 1206 || FF9StateSystem.Common.FF9.fldMapNo == 1223)
            {
                // Hill of Despair
                // A. Castle/Queen's Chamber
                if (fieldSPS.spsBin != null)
                    fieldSPS.meshRenderer.enabled = true;
            }
            else
            {
                fieldSPS.meshRenderer.enabled = true;
            }
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_POS)
        {
            if (FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911)
            {
                // Treno/Queen's House
                if (fieldSPS.spsBin != null)
                    fieldSPS.pos = new Vector3(Arg0, -Arg1, Arg2);
            }
            else
            {
                fieldSPS.pos = new Vector3(Arg0, -Arg1, Arg2);
            }
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_ROT)
        {
            fieldSPS.rot = new Vector3(Arg0 / 4096f * 360f, Arg1 / 4096f * 360f, Arg2 / 4096f * 360f);
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_SCALE)
        {
            fieldSPS.scale = Arg0;
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_CHAR)
        {
            Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(Arg0);
            fieldSPS.charNo = Arg0;
            fieldSPS.boneNo = Arg1;
            fieldSPS.charTran = objUID.go.transform;
            fieldSPS.boneTran = objUID.go.transform.GetChildByName("bone" + fieldSPS.boneNo.ToString("D3"));
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FADE)
        {
            fieldSPS.fade = (Byte)Arg0;
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_ARATE)
        {
            fieldSPS.abr = (Byte)Arg0;
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FRAMERATE)
        {
            fieldSPS.frameRate = Arg0;
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_FRAME)
        {
            fieldSPS.curFrame = Arg0 << 4;
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_POSOFFSET)
        {
            fieldSPS.posOffset = new Vector3(Arg0, -Arg1, Arg2);
        }
        else if (ParmType == FieldSPSConst.FF9FIELDSPS_PARMTYPE_DEPTHOFFSET)
        {
            fieldSPS.depthOffset = Arg0;
        }
    }

    public String MapName;

    private Boolean _isReady;
    private FieldMap _fieldMap;
    private List<FieldSPS> _spsList;
    private Dictionary<Int32, KeyValuePair<String, Byte[]>> _spsBinDict;

    public Vector3 rot;

    private static Int32 _getSpsFrameCount(Byte[] spsBin)
    {
        return (BitConverter.ToUInt16(spsBin, 0) & 0x7FFF) << 4;
    }

    private static void _loadTCB(Byte[] tcbBin)
    {
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(tcbBin)))
        {
            UInt32 secondBatchOffset = binaryReader.ReadUInt32();
            UInt32 firstBatchOffset = binaryReader.ReadUInt32();
            Int32 firstBatchCount = binaryReader.ReadInt32();
            for (Int32 i = 0; i < firstBatchCount; i++)
            {
                binaryReader.BaseStream.Seek(firstBatchOffset, SeekOrigin.Begin);
                Int32 x = binaryReader.ReadInt16();
                Int32 y = binaryReader.ReadInt16();
                Int32 w = binaryReader.ReadInt16();
                Int32 h = binaryReader.ReadInt16();
                PSXTextureMgr.LoadImageBin(x, y, w, h, binaryReader);
                firstBatchOffset += (UInt32)(w * h * 2 + 8);
            }
            binaryReader.BaseStream.Seek(secondBatchOffset, SeekOrigin.Begin);
            UInt32 secondBatchImgOffset = binaryReader.ReadUInt32();
            Int32 secondBatchCount = binaryReader.ReadInt32();
            secondBatchOffset += 8u;
            for (Int32 i = 0; i < secondBatchCount; i++)
            {
                binaryReader.BaseStream.Seek(secondBatchOffset, SeekOrigin.Begin);
                Int32 x = binaryReader.ReadInt16();
                Int32 y = binaryReader.ReadInt16();
                Int32 w = binaryReader.ReadInt16();
                Int32 h = binaryReader.ReadInt16();
                binaryReader.BaseStream.Seek(secondBatchImgOffset, SeekOrigin.Begin);
                PSXTextureMgr.LoadImageBin(x, y, w, h, binaryReader);
                secondBatchImgOffset += (UInt32)(w * h * 2);
                secondBatchOffset += 8u;
            }
        }
        PSXTextureMgr.ClearObject();
    }

    public static void ExportAllSPSTextures(String exportFolder)
    {
        HashSet<String> uniqueTexture = new HashSet<String>(FieldSPSConst.SPSTexture.Values);
        GameObject sharedSPSgo = new GameObject("AllSPSTexturesExporter");
        FieldSPS sharedSPS = sharedSPSgo.AddComponent<FieldSPS>();
        sharedSPS.Init();
        for (Int32 bundleId = 0; bundleId <= 9; bundleId++)
        {
            foreach (AssetManager.AssetFolder modFolder in AssetManager.FolderLowToHigh)
            {
                if (!modFolder.DictAssetBundleRefs.TryGetValue($"data1{bundleId}", out AssetManager.AssetBundleRef bundle) || bundle.assetBundle == null)
                    continue;
                List<String> assetsInBundle = new List<String>(bundle.assetBundle.GetAllAssetNames());
                assetsInBundle.Sort();
                String tcbLoaded = null;
                foreach (String assetName in assetsInBundle)
                {
                    if (!assetName.StartsWith($"Assets/Resources/FieldMaps/", StringComparison.OrdinalIgnoreCase) || !assetName.EndsWith($".sps.bytes", StringComparison.OrdinalIgnoreCase))
                        continue;
                    String[] path = assetName.Split('/');
                    if (path.Length != 5)
                        continue;
                    String mapName = path[3];
                    if (!Int32.TryParse(path[4].Remove(path[4].Length - 10), out Int32 spsId))
                        continue;
                    if (!uniqueTexture.Contains($"{mapName}/{spsId}"))
                        continue;
                    if (!String.Equals(tcbLoaded, mapName))
                    {
                        Byte[] tcbBin = bundle.assetBundle.LoadAsset<TextAsset>($"Assets/Resources/FieldMaps/{mapName}/spt.tcb.bytes")?.bytes;
                        if (tcbBin == null)
                            continue;
                        FieldSPSSystem._loadTCB(tcbBin);
                        tcbLoaded = mapName;
                    }
                    sharedSPS.Unload();
                    sharedSPS.spsBin = bundle.assetBundle.LoadAsset<TextAsset>(assetName)?.bytes;
                    if (sharedSPS.spsBin == null)
                        continue;
                    sharedSPS.mapName = mapName;
                    sharedSPS.refNo = spsId;
                    sharedSPS.curFrame = 0;
                    sharedSPS.frameCount = FieldSPSSystem._getSpsFrameCount(sharedSPS.spsBin);
                    Texture2D spstexture = sharedSPS.GetTextureFromCurrentTCB();
                    if (spstexture == null)
                        continue;
                    // Like all the TIM images, TCB textures are upside-down (vertical mirror)... not that it matters as there seldom are any up and down sides in these ones
                    // So we don't bother mirroring them back and forth
                    Directory.CreateDirectory($"{exportFolder}/{mapName}");
                    File.WriteAllBytes($"{exportFolder}/{mapName}/{spsId}.png", spstexture.EncodeToPNG());
                }
            }
        }
        GameObject.Destroy(sharedSPSgo);
    }
}
