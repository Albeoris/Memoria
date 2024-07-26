using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CommonSPSSystem
{
    public void ChangeFieldOrigin(String newMapName)
    {
        foreach (SPSEffect sps in this.SpsList)
            if (sps.pngTexture == null) // SPS without their pngTexture initialised rely on the current TCB loaded in Vram; dismiss them
                sps.Unload();
        FieldMapInfo.fieldmapSPSExtraOffset.SetSPSOffset(newMapName, this.SpsList);
        this.LoadMapTextureInVram(newMapName);
    }

    public Boolean SetupSPSBinary(SPSEffect sps, KeyValuePair<String, Int32> spsID, Boolean tryLoad)
    {
        Boolean success;
        Byte[] spsBin;
        if (tryLoad)
            success = this.LoadSPSBin(spsID, out spsBin);
        else
            success = this._spsBinDict.TryGetValue(spsID, out spsBin);
        if (!success)
            return false;
        sps.mapName = spsID.Key;
        sps.spsBin = spsBin;
        sps.frameCount = CommonSPSSystem.GetSpsFrameCount(sps.spsBin);
        sps.spsId = spsID.Value;
        this.SetupSPSTexture(sps);
        if (String.Equals(sps.mapName, "FromPrototype"))
        {
            SPSPrototype prototype = CommonSPSSystem.SPSPrototypes[sps.spsId];
            sps.abr = prototype.ShaderType;
            sps.useBattleFactors = true;
            sps.battleScaleFactor = prototype.BattleScale;
            sps.battleDistanceFactor = prototype.BattleDistance;
        }
        return true;
    }

    public Boolean LoadSPSBin(KeyValuePair<String, Int32> spsID, out Byte[] spsBin)
    {
        if (this._spsBinDict.TryGetValue(spsID, out spsBin))
            return true;
        spsBin = AssetManager.LoadBytes(CommonSPSSystem.GetBinaryPath(spsID));
        if (spsBin == null)
            return false;
        this._spsBinDict.Add(spsID, spsBin);
        return true;
    }

    public void LoadMapTextureInVram(String mapName)
    {
        Byte[] binAsset = AssetManager.LoadBytes(CommonSPSSystem.GetTCBPath(mapName), true);
        if (binAsset == null)
            return;
        PSXTextureMgr.LoadTCBInVram(binAsset);
        this._loadedTCBMapName = mapName;
        this.IsTCBReady = true;
    }

    public Boolean SetupSPSTexture(SPSEffect sps)
    {
        Boolean canRetrieveFromTCB = true;
        String texturePath;
        if (String.Equals(sps.mapName, "FromPrototype"))
        {
            canRetrieveFromTCB = false;
            texturePath = CommonSPSSystem.SPSPrototypes[sps.spsId].TexturePath;
            if (this._spsTextureDict.TryGetValue(texturePath, out sps.pngTexture))
                return true;
            sps.pngTexture = AssetManager.Load<Texture2D>(texturePath, false);
        }
        else if (String.Equals(sps.mapName, "WorldMap"))
        {
            texturePath = $"WorldSPS/fx{sps.spsId:D2}";
            if (this._spsTextureDict.TryGetValue(texturePath, out sps.pngTexture))
                return true;
            sps.pngTexture = AssetManager.Load<Texture2D>($"EmbeddedAsset/{texturePath}.png", true);
        }
        else
        {
            texturePath = SPSConst.GetFieldSPSTexture(sps.spsId);
            if (texturePath == null)
                return false;
            if (this._spsTextureDict.TryGetValue(texturePath, out sps.pngTexture))
                return true;
            String spsTextureId = texturePath.Split('/')[1];
            sps.pngTexture = AssetManager.Load<Texture2D>($"FieldMaps/FieldSPS/{spsTextureId}.png", true);
        }
        if (sps.pngTexture == null && canRetrieveFromTCB)
        {
            if (String.IsNullOrEmpty(sps.mapName))
                return false;
            if (!String.Equals(this._loadedTCBMapName, sps.mapName))
                this.ChangeFieldOrigin(sps.mapName);
            sps.pngTexture = sps.GetTextureFromCurrentTCB();
        }
        if (sps.pngTexture == null)
            return false;
        sps.pngTexture.filterMode = FilterMode.Bilinear;
        this._spsTextureDict.Add(texturePath, sps.pngTexture);
        return true;
    }

    private Vector3 adjustSpsPrecise(String name, Vector3 pos)
    {
        Int16 MapNo = FF9StateSystem.Common.FF9.fldMapNo;
        //Log.Message("if (MapNo == " + MapNo + " && name == \"" + name + "\" && pos.x == " + pos.x + " && pos.y == " + pos.y + " && pos.z == " + pos.z + ")"); // 3D positions, pos.y calculated from bottom
        if (MapNo == 2215 && name == "SPS_0008" && pos.x == 1330 && pos.y == 1150 && pos.z == 1125)
        {
            pos.y = 1025;
        }
        return pos;
    }

    public void SetObjParm(SPSEffect sps, Int32 ParmType, Int32 Arg0, Int32 Arg1, Int32 Arg2)
    {
        if (ParmType == SPSConst.OPERATION_CHANGE_FIELD)
        {
            if (Arg0 >= 9000)
                this.ChangeFieldOrigin("WorldMap");
            else
                this.ChangeFieldOrigin(EventEngineUtils.eventIDToFBGID[Arg0]);
            return;
        }
        if (ParmType == SPSConst.OPERATION_LOAD)
        {
            if (Arg0 != SPSConst.REF_DELETE)
            {
                KeyValuePair<String, Int32> spsID = new KeyValuePair<String, Int32>(Arg1 == 0 ? this._loadedTCBMapName : "FromPrototype", Arg0);
                if (Arg1 != 0 && !CommonSPSSystem.SPSPrototypes.ContainsKey(Arg0))
                {
                    Log.Error($"[{nameof(CommonSPSSystem)}] Event script tries to use the SPS {Arg0} which does not exists");
                    return;
                }
                this.SetupSPSBinary(sps, spsID, true);
                sps.attr |= SPSConst.ATTR_UPDATE_ANY_FRAME;
                if (FF9StateSystem.Common.FF9.fldMapNo == 2553 && (sps.spsId == 464 || sps.spsId == 467 || sps.spsId == 506 || sps.spsId == 510))
                {
                    // Wind Shrine/Interior
                    sps.spsBin = null;
                }
            }
            else
            {
                if ((FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911) && (sps.spsId == 33 || sps.spsId == 34))
                {
                    // Treno/Queen's House
                    sps.pos = Vector3.zero;
                    sps.scale = SPSConst.SCALE_ONE;
                    sps.rot = Vector3.zero;
                    sps.rotArg = Vector3.zero;
                }
                sps.Unload();
            }
        }
        else if (ParmType == SPSConst.OPERATION_ATTR)
        {
            if (Arg1 == 0)
                sps.attr &= (Byte)~Arg0;
            else
                sps.attr |= (Byte)Arg0;

            if ((sps.attr & SPSConst.ATTR_VISIBLE) == 0)
            {
                sps.meshRenderer.enabled = false;
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 1206 || FF9StateSystem.Common.FF9.fldMapNo == 1223 || FF9StateSystem.Common.FF9.fldMapNo == 2928)
            {
                // Hill of Despair
                // A. Castle/Queen's Chamber
                if (sps.spsBin != null)
                    sps.meshRenderer.enabled = true;
            }
            else
            {
                sps.meshRenderer.enabled = true;
            }
        }
        else if (ParmType == SPSConst.OPERATION_POS)
        {
            Vector3 adjustedFloatPos = adjustSpsPrecise(sps.name, new Vector3((Single)Arg0, -(Single)Arg1, (Single)Arg2));
            if (FF9StateSystem.Common.FF9.fldMapNo == 911 || FF9StateSystem.Common.FF9.fldMapNo == 1911)
            {
                // Treno/Queen's House
                if (sps.spsBin != null)
                    sps.pos = adjustedFloatPos;
            }
            else
            {
                sps.pos = adjustedFloatPos;
            }
        }
        else if (ParmType == SPSConst.OPERATION_ROT)
        {
            sps.rot = new Vector3(Arg0 / 4096f * 360f, Arg1 / 4096f * 360f, Arg2 / 4096f * 360f);
        }
        else if (ParmType == SPSConst.OPERATION_SCALE)
        {
            if ((FF9StateSystem.Common.FF9.fldMapNo == 50 && sps.name == "SPS_0001") || (FF9StateSystem.Common.FF9.fldMapNo == 51 && sps.name == "SPS_0000")) // candle lights made a bit bigger
                sps.scale = (Int32)(Arg0 * 1.30);
            else
                sps.scale = Arg0;
        }
        else if (ParmType == SPSConst.OPERATION_CHAR)
        {
            Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(Arg0);
            sps.charNo = Arg0;
            sps.boneNo = Arg1;
            sps.charTran = objUID.go.transform;
            sps.boneTran = objUID.go.transform.GetChildByName($"bone{sps.boneNo:D3}");
        }
        else if (ParmType == SPSConst.OPERATION_FADE)
        {
            sps.fade = (Byte)Arg0;
        }
        else if (ParmType == SPSConst.OPERATION_ABR)
        {
            sps.abr = (Byte)Arg0;
        }
        else if (ParmType == SPSConst.OPERATION_FRAMERATE)
        {
            sps.frameRate = Arg0;
        }
        else if (ParmType == SPSConst.OPERATION_FRAME)
        {
            sps.curFrame = Arg0 << 4;
        }
        else if (ParmType == SPSConst.OPERATION_POSOFFSET)
        {
            sps.posOffset = new Vector3(Arg0, -Arg1, Arg2);
        }
        else if (ParmType == SPSConst.OPERATION_DEPTHOFFSET)
        {
            sps.depthOffset = Arg0;
        }
    }

    public Boolean IsTCBReady = false;
    public List<SPSEffect> SpsList = new List<SPSEffect>();

    private String _loadedTCBMapName = null;
    private Dictionary<KeyValuePair<String, Int32>, Byte[]> _spsBinDict = new Dictionary<KeyValuePair<String, Int32>, Byte[]>();
    private Dictionary<String, Texture2D> _spsTextureDict = new Dictionary<String, Texture2D>();

    public static Dictionary<Int32, SPSPrototype> SPSPrototypes;
    public static Dictionary<Int32, SHPPrototype> SHPPrototypes;

    static CommonSPSSystem()
    {
        SPSPrototypes = LoadSPSPrototypes();
        SHPPrototypes = LoadSHPPrototypes();
    }

    private static Dictionary<Int32, SPSPrototype> LoadSPSPrototypes()
    {
        try
        {
            String inputPath = DataResources.SpecialEffects.PureDirectory + DataResources.SpecialEffects.SPSPrototypeFile;
            Dictionary<Int32, SPSPrototype> result = new Dictionary<Int32, SPSPrototype>();
            foreach (SPSPrototype[] prototypeList in AssetManager.EnumerateCsvFromLowToHigh<SPSPrototype>(inputPath))
                foreach (SPSPrototype prototype in prototypeList)
                    result[prototype.Id] = prototype;
            for (Int32 i = 0; i < 8; i++)
                if (!result.ContainsKey(i))
                    throw new NotSupportedException($"You must define at least the 8 SPS, with IDs between 0 and 7.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"[{nameof(CommonSPSSystem)}] Load SPS prototypes failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static Dictionary<Int32, SHPPrototype> LoadSHPPrototypes()
    {
        try
        {
            String inputPath = DataResources.SpecialEffects.PureDirectory + DataResources.SpecialEffects.SHPPrototypeFile;
            Dictionary<Int32, SHPPrototype> result = new Dictionary<Int32, SHPPrototype>();
            foreach (SHPPrototype[] prototypeList in AssetManager.EnumerateCsvFromLowToHigh<SHPPrototype>(inputPath))
                foreach (SHPPrototype prototype in prototypeList)
                    result[prototype.Id] = prototype;
            for (Int32 i = 0; i < 4; i++)
                if (!result.ContainsKey(i))
                    throw new NotSupportedException($"You must define at least the 4 SHP, with IDs between 0 and 3.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"[{nameof(CommonSPSSystem)}] Load SHP prototypes failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    public static Int32 GetSpsFrameCount(Byte[] spsBin)
    {
        return (BitConverter.ToUInt16(spsBin, 0) & 0x7FFF) << 4;
    }

    public static String GetTCBPath(String mapName)
    {
        if (String.Equals(mapName, "WorldMap"))
            return "EmbeddedAsset/WorldSPS/fx.tcb";
        return $"FieldMaps/{mapName}/spt.tcb";
    }

    public static String GetBinaryPath(KeyValuePair<String, Int32> spsID)
    {
        if (String.Equals(spsID.Key, "FromPrototype"))
            return CommonSPSSystem.SPSPrototypes[spsID.Value].BinaryPath;
        if (String.Equals(spsID.Key, "WorldMap"))
            return $"EmbeddedAsset/WorldSPS/fx{spsID.Value:D2}.sps";
        return $"FieldMaps/{spsID.Key}/{spsID.Value}.sps";
    }

    public static void ExportAllSPSTextures(String exportFolder)
    {
        HashSet<String> uniqueTexture = new HashSet<String>(SPSConst.SPSTexture.Values);
        GameObject sharedSPSgo = new GameObject("AllSPSTexturesExporter");
        SPSEffect sharedSPS = sharedSPSgo.AddComponent<SPSEffect>();
        Byte[] tcbBin;
        // Field SPS
        sharedSPS.Init(0);
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
                        tcbBin = bundle.assetBundle.LoadAsset<TextAsset>($"Assets/Resources/FieldMaps/{mapName}/spt.tcb.bytes")?.bytes;
                        if (tcbBin == null)
                            continue;
                        PSXTextureMgr.LoadTCBInVram(tcbBin);
                        tcbLoaded = mapName;
                    }
                    sharedSPS.Unload();
                    sharedSPS.spsBin = bundle.assetBundle.LoadAsset<TextAsset>(assetName)?.bytes;
                    if (sharedSPS.spsBin == null)
                        continue;
                    sharedSPS.mapName = mapName;
                    sharedSPS.spsId = spsId;
                    sharedSPS.curFrame = 0;
                    sharedSPS.frameCount = CommonSPSSystem.GetSpsFrameCount(sharedSPS.spsBin);
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
        // World SPS
        tcbBin = Resources.Load<TextAsset>($"EmbeddedAsset/WorldSPS/fx.tcb")?.bytes;
        if (tcbBin == null)
            return;
        PSXTextureMgr.LoadTCBInVram(tcbBin);
        for (Int32 spsNo = 0; spsNo < SPSConst.WORLD_DEFAULT_OBJLOAD; spsNo++)
        {
            sharedSPS.Unload();
            sharedSPS.spsBin = Resources.Load<TextAsset>($"EmbeddedAsset/WorldSPS/fx{spsNo:D2}.sps")?.bytes;
            if (sharedSPS.spsBin == null)
                continue;
            sharedSPS.spsId = spsNo;
            sharedSPS.curFrame = 0;
            sharedSPS.frameCount = CommonSPSSystem.GetSpsFrameCount(sharedSPS.spsBin);
            Texture2D spstexture = sharedSPS.GetTextureFromCurrentTCB();
            if (spstexture == null)
                continue;
            Directory.CreateDirectory($"{exportFolder}/WorldSPS");
            File.WriteAllBytes($"{exportFolder}/WorldSPS/fx{spsNo:D2}.png", spstexture.EncodeToPNG());
        }
        // Prototype SPS and SHP (battle statuses)
        foreach (SHPPrototype prototype in CommonSPSSystem.SHPPrototypes.Values)
        {
            Directory.CreateDirectory($"{exportFolder}/Status/{prototype.Comment}");
            for (Int32 i = 0; i < prototype.TextureCount; i++)
            {
                Texture2D shpTexture = AssetManager.Load<Texture2D>($"{prototype.TextureBasePath}_{i + 1}");
                if (shpTexture == null)
                    continue;
                File.WriteAllBytes($"{exportFolder}/Status/{prototype.Comment}/{Path.GetFileName(prototype.TextureBasePath)}_{i + 1}", TextureHelper.CopyAsReadable(shpTexture).EncodeToPNG());
            }
        }
        foreach (SPSPrototype prototype in CommonSPSSystem.SPSPrototypes.Values)
        {
            sharedSPS.Unload();
            KeyValuePair<String, Int32> spsID = new KeyValuePair<String, Int32>($"FromPrototype", prototype.Id);
            sharedSPS.spsBin = AssetManager.LoadBytes(CommonSPSSystem.GetBinaryPath(spsID));
            if (sharedSPS.spsBin == null)
                continue;
            sharedSPS.spsId = prototype.Id;
            sharedSPS.curFrame = 0;
            sharedSPS.frameCount = CommonSPSSystem.GetSpsFrameCount(sharedSPS.spsBin);
            Texture2D spstexture = AssetManager.Load<Texture2D>(prototype.TexturePath);
            if (spstexture == null)
                continue;
            if (spstexture.width == 256 && spstexture.height == 256)
            {
                Rect area = sharedSPS.GetRelevantPartOfTCB();
                spstexture = TextureHelper.GetFragment(TextureHelper.CopyAsReadable(spstexture), (Int32)area.x, (Int32)area.y, (Int32)area.width, (Int32)area.height);
            }
            Directory.CreateDirectory($"{exportFolder}/Status");
            File.WriteAllBytes($"{exportFolder}/Status/{Path.GetFileName(prototype.TexturePath)}", spstexture.EncodeToPNG());
            //sharedSPS.pngTexture = spstexture;
            //sharedSPS.ExportAsSFXModel($"{exportFolder}/Status/{Path.GetFileName(prototype.BinaryPath)}.sfxmodel", Path.GetFileName(prototype.TexturePath));
        }
        GameObject.Destroy(sharedSPSgo);
    }
}

