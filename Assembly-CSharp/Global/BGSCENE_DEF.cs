using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using Memoria.Prime;
using Memoria.Assets;
using UnityEngine;
using Memoria.Prime.PsdFile;
//using Memoria.Prime.Log;
using Memoria.Assets.Import.Graphics;
using Memoria.Scripts;
using Global.TileSystem;
using System.Linq;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

public class BGSCENE_DEF
{
    private static readonly Int32 TileSize = Configuration.Graphics.TileSize;


    public UInt16 sceneLength;

    public UInt16 depthBitShift;

    public UInt16 animCount;

    public UInt16 overlayCount;

    public UInt16 lightCount;

    public UInt16 cameraCount;

    public UInt32 animOffset;

    public UInt32 overlayOffset;

    public UInt32 lightOffset;

    public UInt32 cameraOffset;

    public Int16 orgZ;

    public Int16 curZ;

    public Int16 orgX;

    public Int16 orgY;

    public Int16 curX;

    public Int16 curY;

    public Int16 minX;

    public Int16 maxX;

    public Int16 minY;

    public Int16 maxY;

    public Int16 scrX;

    public Int16 scrY;

    public String name;

    public Byte[] ebgBin;

    public List<BGOVERLAY_DEF> overlayList;

    public List<BGANIM_DEF> animList;

    public List<BGLIGHT_DEF> lightList;

    public List<BGCAM_DEF> cameraList;

    public Dictionary<String, Material> materialList;

    public PSXVram vram;

    public Texture2D atlas;

    public Texture2D atlasAlpha;

    public UInt32 SPRITE_W = 16u;

    public UInt32 SPRITE_H = 16u;

    public UInt32 ATLAS_W = 1024u;

    public UInt32 ATLAS_H = 1024u;

    public Boolean combineMeshes = true;

    private Int32 spriteCount;

    private Boolean useUpscaleFM;

    private Int32 initialCorrection;

    private String mapName;

    private String atlasPath;

    private DateTime atlasTimestamp;

    public BGSCENE_DEF(Boolean useUpscaleFm)
    {
        this.useUpscaleFM = useUpscaleFm;
        this.name = String.Empty;
        this.ebgBin = null;
        this.overlayList = new List<BGOVERLAY_DEF>();
        this.animList = new List<BGANIM_DEF>();
        this.lightList = new List<BGLIGHT_DEF>();
        this.cameraList = new List<BGCAM_DEF>();
        this.materialList = new Dictionary<String, Material>();
    }

    private void InitPSXTextureAtlas()
    {
        this.vram = new PSXVram(true);
        this.atlas = new Texture2D((Int32)this.ATLAS_W, (Int32)this.ATLAS_H)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        this.SPRITE_W = 16u;
        this.SPRITE_H = 16u;
    }

    public void ReadData(BinaryReader reader)
    {
        this.ExtractHeaderData(reader);
        this.ExtractOverlayData(reader);
        this.ExtractSpriteData(reader);
        this.ExtractAnimationData(reader);
        this.ExtractAnimationFrameData(reader);
        this.ExtractLightData(reader);
        this.ExtractCameraData(reader);
    }

    private void ExtractHeaderData(BinaryReader reader)
    {
        this.sceneLength = reader.ReadUInt16();
        this.depthBitShift = reader.ReadUInt16();
        this.animCount = reader.ReadUInt16();
        this.overlayCount = reader.ReadUInt16();
        this.lightCount = reader.ReadUInt16();
        this.cameraCount = reader.ReadUInt16();
        this.animOffset = reader.ReadUInt32();
        this.overlayOffset = reader.ReadUInt32();
        this.lightOffset = reader.ReadUInt32();
        this.cameraOffset = reader.ReadUInt32();
        this.orgZ = reader.ReadInt16();
        this.curZ = reader.ReadInt16();
        this.orgX = reader.ReadInt16();
        this.orgY = reader.ReadInt16();
        this.curX = reader.ReadInt16();
        this.curY = reader.ReadInt16();
        this.minX = reader.ReadInt16();
        this.maxX = reader.ReadInt16();
        this.minY = reader.ReadInt16();
        this.maxY = reader.ReadInt16();
        this.scrX = reader.ReadInt16();
        this.scrY = reader.ReadInt16();
    }

    private void ExtractCameraData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.cameraOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < (Int32)this.cameraCount; i++)
        {
            BGCAM_DEF bGCAM_DEF = new BGCAM_DEF();
            bGCAM_DEF.ReadData(reader);
            this.cameraList.Add(bGCAM_DEF);
        }
    }

    private void ExtractLightData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.lightOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < (Int32)this.lightCount; i++)
        {
            BGLIGHT_DEF bGLIGHT_DEF = new BGLIGHT_DEF();
            bGLIGHT_DEF.ReadData(reader);
            this.lightList.Add(bGLIGHT_DEF);
        }
    }

    private void ExtractAnimationFrameData(BinaryReader reader)
    {
        for (Int32 i = 0; i < (Int32)this.animCount; i++)
        {
            BGANIM_DEF bGANIM_DEF = this.animList[i];
            reader.BaseStream.Seek(bGANIM_DEF.offset, SeekOrigin.Begin);
            for (Int32 j = 0; j < bGANIM_DEF.frameCount; j++)
            {
                BGANIMFRAME_DEF bGANIMFRAME_DEF = new BGANIMFRAME_DEF();
                bGANIMFRAME_DEF.ReadData(reader);
                bGANIM_DEF.frameList.Add(bGANIMFRAME_DEF);
            }
        }
    }

    private void ExtractAnimationData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.animOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < (Int32)this.animCount; i++)
        {
            BGANIM_DEF bGANIM_DEF = new BGANIM_DEF();
            bGANIM_DEF.ReadData(reader);
            this.animList.Add(bGANIM_DEF);
        }
    }

    private void ExtractSpriteData(BinaryReader reader)
    {

        this.spriteCount = 0;
        for (Int32 i = 0; i < (Int32)this.overlayCount; i++)
        {
            BGOVERLAY_DEF overlayInfo = this.overlayList[i];
            this.spriteCount += overlayInfo.spriteCount;
        }
        if (this.useUpscaleFM)
        {
            this.ATLAS_H = (UInt32)this.atlas.height;
            this.ATLAS_W = (UInt32)this.atlas.width;
        }
        Int32 num = this.atlas.width / (TileSize + 4);
        Int32 num2 = 0;
        for (Int32 j = 0; j < (Int32)this.overlayCount; j++)
        {
            BGOVERLAY_DEF overlayInfo2 = this.overlayList[j];
            reader.BaseStream.Seek(overlayInfo2.prmOffset, SeekOrigin.Begin);
            for (Int32 k = 0; k < (Int32)overlayInfo2.spriteCount; k++)
            {
                BGSPRITE_LOC_DEF spriteInfo = new BGSPRITE_LOC_DEF();
                spriteInfo.ReadData_BGSPRITE_DEF(reader);
                overlayInfo2.spriteList.Add(spriteInfo);
            }
            reader.BaseStream.Seek(overlayInfo2.locOffset, SeekOrigin.Begin);
            for (Int32 l = 0; l < (Int32)overlayInfo2.spriteCount; l++)
            {
                BGSPRITE_LOC_DEF spriteInfo2 = overlayInfo2.spriteList[l];
                spriteInfo2.ReadData_BGSPRITELOC_DEF(reader);
                if (this.useUpscaleFM)
                {
                    spriteInfo2.atlasX = (UInt16)(2 + num2 % num * (TileSize + 4));
                    spriteInfo2.atlasY = (UInt16)(2 + num2 / num * (TileSize + 4));
                    spriteInfo2.w = (ushort)TileSize;
                    spriteInfo2.h = (ushort)TileSize;
                    num2++;
                }
            }
        }
    }

    private void ExtractOverlayData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.overlayOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < (Int32)this.overlayCount; i++)
        {
            BGOVERLAY_DEF overlayInfo = new BGOVERLAY_DEF();
            overlayInfo.ReadData(reader);
            overlayInfo.minX = -32768;
            overlayInfo.maxX = 32767;
            overlayInfo.minY = -32768;
            overlayInfo.maxY = 32767;
            overlayInfo.indnum = (Byte)i;
            this.overlayList.Add(overlayInfo);
        }
    }

    public void LoadLocale(BGSCENE_DEF sceneUS, String path, String newName, FieldMapLocalizeAreaTitleInfo info, String localizeSymbol)
    {
        this._LoadDummyEBG(sceneUS, path, newName, info, localizeSymbol);
    }

    private void _LoadDummyEBG(BGSCENE_DEF sceneUS, String path, String newName, FieldMapLocalizeAreaTitleInfo info, String localizeSymbol)
    {
        this.name = newName;
        path += $"{newName}_{localizeSymbol}.bgs";
        this.ebgBin = AssetManager.LoadBytes(path);
        if (this.ebgBin == null)
            return;
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.ebgBin)))
        {
            this.ExtractHeaderData(binaryReader);
            this.ExtractOverlayData(binaryReader);
            Int32 atlasWidth = (Int32)this.ATLAS_W;
            Int32 startOvrIdx = info.startOvrIdx;
            Int32 endOvrIdx = info.endOvrIdx;
            Int32 spriteStartIndex = info.GetSpriteStartIndex(localizeSymbol);
            Int32 num = atlasWidth / (TileSize + 4);
            Int32 spriteIndex = spriteStartIndex;
            for (Int32 i = startOvrIdx; i <= endOvrIdx; i++)
            {
                BGOVERLAY_DEF overlayInfo = this.overlayList[i];
                binaryReader.BaseStream.Seek(overlayInfo.prmOffset, SeekOrigin.Begin);
                for (Int32 j = 0; j < overlayInfo.spriteCount; j++)
                {
                    BGSPRITE_LOC_DEF spriteInfo = new BGSPRITE_LOC_DEF();
                    spriteInfo.ReadData_BGSPRITE_DEF(binaryReader);
                    overlayInfo.spriteList.Add(spriteInfo);
                }
                binaryReader.BaseStream.Seek(overlayInfo.locOffset, SeekOrigin.Begin);
                for (Int32 j = 0; j < overlayInfo.spriteCount; j++)
                {
                    BGSPRITE_LOC_DEF spriteInfo = overlayInfo.spriteList[j];
                    spriteInfo.ReadData_BGSPRITELOC_DEF(binaryReader);
                    if (this.useUpscaleFM)
                    {
                        spriteInfo.atlasX = (UInt16)(2 + spriteIndex % num * (TileSize + 4));
                        spriteInfo.atlasY = (UInt16)(2 + spriteIndex / num * (TileSize + 4));
                        spriteInfo.w = (ushort)TileSize;
                        spriteInfo.h = (ushort)TileSize;
                        spriteIndex++;
                    }
                }
            }
            for (Int32 l = startOvrIdx; l <= endOvrIdx; l++)
                sceneUS.overlayList[l] = this.overlayList[l];
        }
    }

    public void LoadResources(FieldMap fieldMap, String path, String newName)
    {
        this.name = newName;
        if (!this.useUpscaleFM)
        {
            this.InitPSXTextureAtlas();
        }
        else
        {
            Texture2D atlasTexture = AssetManager.Load<Texture2D>(Path.Combine(path, "atlas"), false);

            if (atlasTexture != null)
            {
                this.atlas = atlasTexture;
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
                    this.atlasAlpha = AssetManager.Load<Texture2D>(Path.Combine(path, "atlas_a"), false);
                else
                    this.atlasAlpha = null;
                this.SPRITE_W = (UInt16)TileSize;
                this.SPRITE_H = (UInt16)TileSize;
            }
            else
            {
                this.useUpscaleFM = false;
                this.InitPSXTextureAtlas();
            }
        }
        if (!this.useUpscaleFM)
            this.vram.LoadTIMs(path);
        Byte[] binAsset;
        if (!FieldMapEditor.useOriginalVersion)
        {
            binAsset = AssetManager.LoadBytes(path + FieldMapEditor.GetFieldMapModName(newName) + ".bgs");
            if (binAsset == null)
            {
                Debug.Log("Cannot find MOD version.");
                binAsset = AssetManager.LoadBytes(path + newName + ".bgs");
            }
        }
        else
        {
            binAsset = AssetManager.LoadBytes(path + newName + ".bgs");
        }

        if (binAsset == null)
            return;

        this.ebgBin = binAsset;
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.ebgBin)))
        {
            this.ReadData(binaryReader);
        }

        FieldMapInfo.fieldmapExtraOffset.SetOffset(name, this.overlayList);
        if (!this.useUpscaleFM)
            this.GenerateAtlasFromBinary();
    }

    private void loadLocalizationInfo(String newName, String path)
    {
        String symbol = Localization.GetSymbol();
        if (symbol == "US")
            return;
        
        FieldMapLocalizeAreaTitleInfo info = FieldMapInfo.localizeAreaTitle.GetInfo(newName);
        if (info == null)
            return;
        
        if (symbol != "UK" || info.hasUK)
        {
            BGSCENE_DEF bGSCENE_DEF = new BGSCENE_DEF(this.useUpscaleFM);
            bGSCENE_DEF.atlas = this.atlas;
            bGSCENE_DEF.ATLAS_W = this.ATLAS_W;
            bGSCENE_DEF.ATLAS_H = this.ATLAS_H;
            bGSCENE_DEF._LoadDummyEBG(this, path, newName, info, symbol);
        }
    }

    public void LoadEBG(FieldMap fieldMap, String path, String newName)
    {
        this.mapName = newName; 

        this.LoadResources(fieldMap, path, newName);

        //FieldMapInfo.fieldmapExtraOffset.SetOffset(name, this.overlayList);
        //if (!this.useUpscaleFM)
        //{
        //    this.GenerateAtlasFromBinary();
        //}
        this.CreateMaterials();
        List<short> list = new List<short>
        {
            1505, // Conde Petie/Shrine
            2605, // Terra/Treetop
            2653, // Bran Bal/Pond
            2259, // Oeilvert/Star Display
            153, // A. Castle/Hallway
            1806, // A. Castle/Hallway
            1214, // A. Castle/Hallway
            1823, // A. Castle/Hallway
            1752, // Iifa Tree/Inner Roots
            2922, // Crystal World
            2923, // Crystal World
            2924, // Crystal World
            2925, // Crystal World
            2926, // Crystal World
            1751, // Iifa Tree/Inner Roots
            1752, // Iifa Tree/Inner Roots
            1753, // Iifa Tree/Inner Roots
            2252, // Oeilvert/Hall
            2714 // Pand./Maze
        };
        this.combineMeshes = list.Contains(FF9StateSystem.Common.FF9.fldMapNo);
        if (this.combineMeshes && !Configuration.Import.Field)
        {
            this.loadLocalizationInfo(newName, path);
            this.CreateSceneCombined(fieldMap, this.useUpscaleFM);
        }
        else
        {
            this.CreateScene(fieldMap, this.useUpscaleFM, path);
        }
    }

    public bool GetUseUpscaleFM()
    {
        return this.useUpscaleFM;
    }

    private static Rect CalculateExpectedTextureAtlasSize(Int32 spriteCount)
    {
        Rect[] array =
        {
            new Rect(0f, 0f, 256f, 256f),
            new Rect(0f, 0f, 512f, 256f),
            new Rect(0f, 0f, 1024f, 256f),
            new Rect(0f, 0f, 512f, 256f),
            new Rect(0f, 0f, 512f, 512f),
            new Rect(0f, 0f, 1024f, 256f),
            new Rect(0f, 0f, 1024f, 512f),
            new Rect(0f, 0f, 2048f, 256f),
            new Rect(0f, 0f, 1024f, 1024f),
            new Rect(0f, 0f, 2048f, 512f),
            new Rect(0f, 0f, 2048f, 1024f),
            new Rect(0f, 0f, 2048f, 2048f)
        };
        Rect[] array2 = array;
        for (Int32 i = 0; i < array2.Length; i++)
        {
            Rect result = array2[i];
            Int32 num = (Int32)result.width / (TileSize + 4);
            Int32 num2 = (Int32)result.height / (TileSize + 4);
            if (num * num2 >= spriteCount)
            {
                return result;
            }
        }
        throw new ArgumentException("Unexpected size of atlas texture");
    }

    private void GenerateAtlasFromBinary()
    {
        UInt32 num = this.ATLAS_W * this.ATLAS_H;
        Color32[] array = new Color32[num];
        UInt32 num2 = 0u;
        UInt32 num3 = 1u;
        for (Int32 i = 0; i < (Int32)this.overlayCount; i++)
        {
            BGOVERLAY_DEF overlayInfo = this.overlayList[i];
            for (Int32 j = 0; j < (Int32)overlayInfo.spriteCount; j++)
            {
                BGSPRITE_LOC_DEF spriteInfo = overlayInfo.spriteList[j];
                spriteInfo.atlasX = (UInt16)num2;
                spriteInfo.atlasY = (UInt16)num3;
                if (spriteInfo.res == 0)
                {
                    Int32 index = ArrayUtil.GetIndex(spriteInfo.clutX * 16, spriteInfo.clutY, (Int32)this.vram.width, (Int32)this.vram.height);
                    for (UInt32 num4 = 0u; num4 < (UInt32)spriteInfo.h; num4 += 1u)
                    {
                        Int32 index2 = ArrayUtil.GetIndex(spriteInfo.texX * 64 + spriteInfo.u / 4, (Int32)(spriteInfo.texY * 256u + spriteInfo.v + num4), (Int32)this.vram.width, (Int32)this.vram.height);
                        Int32 index3 = ArrayUtil.GetIndex((Int32)num2, (Int32)(num3 + num4), (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                        UInt32 num5 = 0u;
                        while (num5 < (UInt64)(spriteInfo.w / 2))
                        {
                            Byte b = this.vram.rawData[index2 * 2 + (Int32)num5];
                            Byte b2 = (Byte)(b & 15);
                            Byte b3 = (Byte)(b >> 4 & 15);
                            Int32 num6 = (index + b2) * 2;
                            UInt16 num7 = (UInt16)(this.vram.rawData[num6] | this.vram.rawData[num6 + 1] << 8);
                            Int32 num8 = index3 + (Int32)(num5 * 2u);
                            PSX.ConvertColor16toColor32(num7, out array[num8]);
                            if (spriteInfo.trans != 0 && num7 != 0)
                            {
                                if (spriteInfo.alpha == 0)
                                {
                                    array[num8].a = 127;
                                }
                                else if (spriteInfo.alpha == 3)
                                {
                                    array[num8].a = 63;
                                }
                            }
                            num6 = (index + b3) * 2;
                            num7 = (UInt16)(this.vram.rawData[num6] | this.vram.rawData[num6 + 1] << 8);
                            num8 = index3 + (Int32)(num5 * 2u) + 1;
                            PSX.ConvertColor16toColor32(num7, out array[num8]);
                            if (spriteInfo.trans != 0 && num7 != 0)
                            {
                                if (spriteInfo.alpha == 0)
                                {
                                    array[num8].a = 127;
                                }
                                else if (spriteInfo.alpha == 3)
                                {
                                    array[num8].a = 63;
                                }
                            }
                            num5 += 1u;
                        }
                    }
                }
                else if (spriteInfo.res == 1)
                {
                    Int32 index4 = ArrayUtil.GetIndex(spriteInfo.clutX * 16, spriteInfo.clutY, (Int32)this.vram.width, (Int32)this.vram.height);
                    for (UInt32 num9 = 0u; num9 < (UInt32)spriteInfo.h; num9 += 1u)
                    {
                        Int32 index5 = ArrayUtil.GetIndex(spriteInfo.texX * 64 + spriteInfo.u / 2, (Int32)(spriteInfo.texY * 256u + spriteInfo.v + num9), (Int32)this.vram.width, (Int32)this.vram.height);
                        Int32 index6 = ArrayUtil.GetIndex((Int32)num2, (Int32)(num3 + num9), (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                        for (UInt32 num10 = 0u; num10 < (UInt32)spriteInfo.w; num10 += 1u)
                        {
                            Byte b4 = this.vram.rawData[index5 * 2 + (Int32)num10];
                            Int32 num11 = (index4 + b4) * 2;
                            UInt16 num12 = (UInt16)(this.vram.rawData[num11] | this.vram.rawData[num11 + 1] << 8);
                            Int32 num13 = index6 + (Int32)num10;
                            PSX.ConvertColor16toColor32(num12, out array[num13]);
                            if (spriteInfo.trans != 0 && num12 != 0)
                            {
                                if (spriteInfo.alpha == 0)
                                {
                                    array[num13].a = 127;
                                }
                                else if (spriteInfo.alpha == 3)
                                {
                                    array[num13].a = 63;
                                }
                            }
                        }
                    }
                }
                for (UInt32 num14 = 0u; num14 < (UInt32)spriteInfo.h; num14 += 1u)
                {
                    Int32 index7 = ArrayUtil.GetIndex((Int32)(num2 + this.SPRITE_W), (Int32)(num3 + num14), (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                    array[index7] = array[index7 - 1];
                }
                for (UInt32 num15 = 0u; num15 < (UInt32)spriteInfo.w; num15 += 1u)
                {
                    Int32 index8 = ArrayUtil.GetIndex((Int32)(num2 + num15), (Int32)num3, (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                    Int32 index9 = ArrayUtil.GetIndex((Int32)(num2 + num15), (Int32)(num3 - 1u), (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                    array[index9] = array[index8];
                }
                Int32 index10 = ArrayUtil.GetIndex((Int32)(num2 + this.SPRITE_W - 1u), (Int32)num3, (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                Int32 index11 = ArrayUtil.GetIndex((Int32)(num2 + this.SPRITE_W), (Int32)(num3 - 1u), (Int32)this.ATLAS_W, (Int32)this.ATLAS_H);
                array[index11] = array[index10];
                num2 += this.SPRITE_W + 1u;
                if (num2 >= this.ATLAS_W || this.ATLAS_W - num2 < this.SPRITE_W + 1u)
                {
                    num2 = 0u;
                    num3 += this.SPRITE_H + 1u;
                }
            }
        }
        this.atlas.SetPixels32(array);
        this.atlas.Apply();
    }

    private void CreateMaterialsForOverlay(Texture2D overlay)
    {
        Log.Message("Creating material for overlay");
        this.materialList.Clear();
        Material material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_None")) { mainTexture = overlay };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_none", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_0")) { mainTexture = overlay };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_0", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_1")) { mainTexture = overlay };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_1", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_2")) { mainTexture = overlay };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_2", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_3")) { mainTexture = overlay };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_3", material);
    }


    private void CreateMaterials()
    {
        Material material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_None")) { mainTexture = this.atlas };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_none", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_0")) { mainTexture = this.atlas };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_0", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_1")) { mainTexture = this.atlas };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_1", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_2")) { mainTexture = this.atlas };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_2", material);
        material = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_3")) { mainTexture = this.atlas };
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_3", material);
    }

    private void importOverlaysFromPsd(FieldMap fieldMap, Boolean UseUpscalFM, String externalPath)
    {
        // open meta, get total atlases
        string atlasPath = Path.Combine(externalPath, "atlases");
        AtlasInfo ai = AtlasInfo.Load(Path.Combine(atlasPath, "atlas.meta"));
 
        uint totalAtlases = (uint) ai.TotalAtlasesFromAtlasSection;
        uint tileSize = (uint)ai.TileSizeFromAtlasSection;
        int atlasSide = ai.AtlasSideFromAtlasSection;


        this.SPRITE_H = tileSize;
        this.SPRITE_W = tileSize;
        uint padding = tileSize / 16;
        int factor =(int) tileSize / 16;

        UInt32 atlasX = padding, atlasY = padding;
        uint deltaX = this.SPRITE_W + 2 * padding;
        uint deltaY = this.SPRITE_H + 2 * padding;

        string pathDebugOverlay = Path.Combine(atlasPath, "debug");
            Directory.CreateDirectory(pathDebugOverlay);
        List<Vector3> list = new List<Vector3>();
        List<Vector2> list2 = new List<Vector2>();
        List<Int32> list3 = new List<Int32>();
        // count how many tiles to pass
        // pass them
        // or maybe create your own overlays
        FieldMapLocalizeAreaTitleInfo info = FieldMapInfo.localizeAreaTitle.GetInfo(this.mapName);

        Int32 startLocaleOvrIdx = Int32.MaxValue;
        Int32 endLocaleOvrIdx = Int32.MinValue;

        if (info != null)
        {
            Log.Message("Start importing locale overlays");
            startLocaleOvrIdx = info.startOvrIdx;
            endLocaleOvrIdx = info.endOvrIdx;
            String symbol = Localization.GetSymbol();

            int currentLocaleAtlas = 0;
            Texture2D localeReftexture = new Texture2D(atlasSide, atlasSide, TextureFormat.RGBA32, false);
            localeReftexture.LoadImage(File.ReadAllBytes(Path.Combine(atlasPath, $"atlas_{symbol.ToUpper()}_{++currentLocaleAtlas}.png")));
            this.CreateMaterialsForOverlay(localeReftexture);

            UInt32 atlasLocaleX = padding, atlasLocaleY = padding;

            for (Int32 j = startLocaleOvrIdx; j <= endLocaleOvrIdx; j++)
            {
                BGOVERLAY_DEF overlayInfo = this.overlayList[j];
                Log.Message($"LocaleOverlay {j}, lang {symbol}, spriteCount {overlayInfo.spriteList.Count}");
                Texture2D overlaytexLocale = new Texture2D(overlayInfo.w * factor, overlayInfo.h * factor, TextureFormat.RGBA32, false);
                //Texture2D overlay = new Texture2D();
                String str = "Overlay_" + j.ToString("D2");
                GameObject gameObject3 = new GameObject(str);
                Transform transform2 = gameObject3.transform;
                transform2.parent = this.cameraList[overlayInfo.camNdx].transform;
                transform2.localPosition = new Vector3(overlayInfo.curX * 1f, overlayInfo.curY * 1f, overlayInfo.curZ);
                transform2.localScale = new Vector3(1f, 1f, 1f);
                overlayInfo.transform = transform2;
                for (Int32 k = 0; k < overlayInfo.spriteList.Count; k++)
                {
                    BGSPRITE_LOC_DEF spriteInfo = overlayInfo.spriteList[k];
                    var num = spriteInfo.depth;
                    GameObject gameObject4 = new GameObject(str + "_Sprite_" + k.ToString("D3"));
                    Transform transform3 = gameObject4.transform;
                    transform3.parent = transform2;
                    {
                        transform3.localPosition = new Vector3(spriteInfo.offX * 1f, (spriteInfo.offY + 16) * 1f, num);
                    }

                    transform3.localScale = new Vector3(1f, 1f, 1f);
                    spriteInfo.transform = transform3;
                    list.Clear();
                    list2.Clear();
                    list3.Clear();
                    list.Add(new Vector3(0f, -16f, 0f));
                    list.Add(new Vector3(16f, -16f, 0f));
                    list.Add(new Vector3(16f, 0f, 0f));
                    list.Add(new Vector3(0f, 0f, 0f));
                    Single num2 = this.ATLAS_W;
                    Single num3 = this.ATLAS_H;
                    Single x;
                    Single y;
                    Single x2;
                    Single y2;

                    Single num4 = 0.5f;
                    x = (float)(atlasLocaleX - num4) / atlasSide;
                    x2 = (float)(atlasLocaleX - num4 + this.SPRITE_W) / atlasSide;
                    y = (float)(atlasLocaleY + num4 + this.SPRITE_H) / atlasSide;
                    y2 = (float)(atlasLocaleY + num4) / atlasSide;
                    Color[] sprite = localeReftexture.GetPixels((int)atlasLocaleX, (int)atlasLocaleY, (int)this.SPRITE_W, (int)this.SPRITE_H);
                    overlaytexLocale.SetPixels(spriteInfo.offX * factor, overlaytexLocale.height - (spriteInfo.offY + 16) * factor, (int)this.SPRITE_W, (int)this.SPRITE_H, sprite);

                    list2.Add(new Vector2(x, y));
                    list2.Add(new Vector2(x2, y));
                    list2.Add(new Vector2(x2, y2));
                    list2.Add(new Vector2(x, y2));
                    list3.Add(2);
                    list3.Add(1);
                    list3.Add(0);
                    list3.Add(3);
                    list3.Add(2);
                    list3.Add(0);

                    Mesh mesh = new Mesh
                    {
                        vertices = list.ToArray(),
                        uv = list2.ToArray(),
                        triangles = list3.ToArray()
                    };

                    MeshRenderer meshRenderer = gameObject4.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter = gameObject4.AddComponent<MeshFilter>();
                    meshFilter.mesh = mesh;
                    Int32 num6 = this.curZ + (Int16)overlayInfo.curZ + spriteInfo.depth;
                    GameObject expr_5B4 = gameObject4;
                    expr_5B4.name = expr_5B4.name + "_Depth(" + num6.ToString("D5") + ")";
                    String text;
                    if (spriteInfo.trans != 0)
                    {
                        if (spriteInfo.alpha == 0)
                        {
                            text = "abr_0";
                        }
                        else if (spriteInfo.alpha == 1)
                        {
                            text = "abr_1";
                        }
                        else if (spriteInfo.alpha == 2)
                        {
                            text = "abr_2";
                        }
                        else
                        {
                            text = "abr_3";
                        }
                    }
                    else
                    {
                        text = "abr_none";
                    }
                    if (fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && j == 14)
                    {
                        text = "abr_none";
                    }
                    GameObject expr_671 = gameObject4;
                    expr_671.name = expr_671.name + "_[" + text + "]";
                    meshRenderer.material = this.materialList[text];

                    atlasLocaleX += deltaX;
                    if (atlasLocaleX + deltaX > atlasSide)
                    {
                        atlasLocaleX = padding;
                        atlasLocaleY += deltaY;
                    }
                    if (atlasLocaleY + deltaY > atlasSide)
                    {
                        // write atlas to file, flush atlas and so on
                        atlasLocaleX = padding;
                        atlasLocaleY = padding;
                        localeReftexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                        localeReftexture.LoadImage(File.ReadAllBytes(Path.Combine(atlasPath, $"atlas_{symbol.ToUpper()}_{++currentLocaleAtlas}.png")));
                        this.CreateMaterialsForOverlay(localeReftexture);
                    }

                }
                TextureHelper.WriteTextureToFile(overlaytexLocale, Path.Combine(pathDebugOverlay, $"overlay_{j}_{symbol.ToUpper()}.png"));
                overlayInfo.transform.gameObject.SetActive((overlayInfo.flags & 2) != 0);
            }
        }

        Log.Message($"Begin importOverlaysFromPsd, totalAtlases {totalAtlases}, tileSize {tileSize}, atlasSide {atlasSide}");
        int currentAtlas = 0;
        Texture2D reftexture = new Texture2D(atlasSide, atlasSide, TextureFormat.RGBA32, false);
        reftexture.LoadImage(File.ReadAllBytes(Path.Combine(atlasPath, $"atlas_{++currentAtlas}.png")));
        this.CreateMaterialsForOverlay(reftexture);

        for (Int32 j = 0; j < this.overlayList.Count; j++)
        {
            if (j >= startLocaleOvrIdx && j <= endLocaleOvrIdx)
                continue;

            BGOVERLAY_DEF overlayInfo = this.overlayList[j];
            Texture2D overlaytex = new Texture2D(overlayInfo.w * factor, overlayInfo.h * factor, TextureFormat.RGBA32, false);
            String str = "Overlay_" + j.ToString("D2");
            GameObject gameObject3 = new GameObject(str);
            Transform transform2 = gameObject3.transform;
            transform2.parent = this.cameraList[overlayInfo.camNdx].transform;
            transform2.localPosition = new Vector3(overlayInfo.curX * 1f, overlayInfo.curY * 1f, overlayInfo.curZ);
            transform2.localScale = new Vector3(1f, 1f, 1f);
            overlayInfo.transform = transform2;
            for (Int32 k = 0; k < overlayInfo.spriteList.Count; k++)
            {
                BGSPRITE_LOC_DEF spriteInfo = overlayInfo.spriteList[k];
                var num = spriteInfo.depth;
                GameObject gameObject4 = new GameObject(str + "_Sprite_" + k.ToString("D3"));
                Transform transform3 = gameObject4.transform;
                transform3.parent = transform2;
                {
                    transform3.localPosition = new Vector3(spriteInfo.offX * 1f, (spriteInfo.offY + 16) * 1f, num);
                }

                transform3.localScale = new Vector3(1f, 1f, 1f);
                spriteInfo.transform = transform3;
                list.Clear();
                list2.Clear();
                list3.Clear();
                list.Add(new Vector3(0f, -16f, 0f));
                list.Add(new Vector3(16f, -16f, 0f));
                list.Add(new Vector3(16f, 0f, 0f));
                list.Add(new Vector3(0f, 0f, 0f));
                Single num2 = this.ATLAS_W;
                Single num3 = this.ATLAS_H;
                Single x;
                Single y;
                Single x2;
                Single y2;

                Single num4 = 0.5f;
                x = (float)(atlasX - num4) / atlasSide;
                x2 = (float)(atlasX - num4 + this.SPRITE_W) / atlasSide;
                y = (float)(atlasY + num4 + this.SPRITE_H) / atlasSide;
                y2 = (float)(atlasY + num4) / atlasSide;
                Color[] sprite = reftexture.GetPixels((int)atlasX, (int)atlasY, (int)this.SPRITE_W, (int)this.SPRITE_H);
                overlaytex.SetPixels(spriteInfo.offX * factor, overlaytex.height - (spriteInfo.offY + 16) * factor, (int)this.SPRITE_W, (int)this.SPRITE_H, sprite);

                list2.Add(new Vector2(x, y));
                list2.Add(new Vector2(x2, y));
                list2.Add(new Vector2(x2, y2));
                list2.Add(new Vector2(x, y2));
                list3.Add(2);
                list3.Add(1);
                list3.Add(0);
                list3.Add(3);
                list3.Add(2);
                list3.Add(0);

                    Mesh mesh = new Mesh
                    {
                        vertices = list.ToArray(),
                        uv = list2.ToArray(),
                        triangles = list3.ToArray()
                    };

                MeshRenderer meshRenderer = gameObject4.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = gameObject4.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                Int32 num6 = this.curZ + (Int16)overlayInfo.curZ + spriteInfo.depth;
                GameObject expr_5B4 = gameObject4;
                expr_5B4.name = expr_5B4.name + "_Depth(" + num6.ToString("D5") + ")";
                String text;
                if (spriteInfo.trans != 0)
                {
                    if (spriteInfo.alpha == 0)
                    {
                        text = "abr_0";
                    }
                    else if (spriteInfo.alpha == 1)
                    {
                        text = "abr_1";
                    }
                    else if (spriteInfo.alpha == 2)
                    {
                        text = "abr_2";
                    }
                    else
                    {
                        text = "abr_3";
                    }
                }
                else
                {
                    text = "abr_none";
                }
                if (fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && j == 14)
                {
                    text = "abr_none";
                }
                GameObject expr_671 = gameObject4;
                expr_671.name = expr_671.name + "_[" + text + "]";
                meshRenderer.material = this.materialList[text];

                atlasX += deltaX;
                if (atlasX + deltaX > atlasSide)
                {
                    atlasX = padding;
                    atlasY += deltaY;
                }
                if (atlasY + deltaY > atlasSide)
                {
                    // write atlas to file, flush atlas and so on
                    atlasX = padding;
                    atlasY = padding;
                    reftexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    reftexture.LoadImage(File.ReadAllBytes(Path.Combine(atlasPath, $"atlas_{++currentAtlas}.png")));
                    this.CreateMaterialsForOverlay(reftexture);
                }

            }
            TextureHelper.WriteTextureToFile(overlaytex, Path.Combine(pathDebugOverlay, $"overlay_{j}.png"));
            overlayInfo.transform.gameObject.SetActive((overlayInfo.flags & 2) != 0);
        }
    }

    private void handleOverlays(FieldMap fieldMap, Boolean UseUpscalFM, String path)
    {
        //Log.Message($"UseUpscalFM {UseUpscalFM}");
        List<Vector3> list = new List<Vector3>();
        List<Vector2> list2 = new List<Vector2>();
        List<Int32> list3 = new List<Int32>();
        for (Int32 j = 0; j < this.overlayList.Count; j++)
        {
            BGOVERLAY_DEF overlayInfo = this.overlayList[j];
            String str = "Overlay_" + j.ToString("D2");
            GameObject gameObject3 = new GameObject(str);
            Transform transform2 = gameObject3.transform;
            transform2.parent = this.cameraList[overlayInfo.camNdx].transform;
            transform2.localPosition = new Vector3(overlayInfo.curX * 1f, overlayInfo.curY * 1f, overlayInfo.curZ);
            transform2.localScale = new Vector3(1f, 1f, 1f);
            overlayInfo.transform = transform2;
            for (Int32 k = 0; k < overlayInfo.spriteList.Count; k++)
            {
                BGSPRITE_LOC_DEF spriteInfo = overlayInfo.spriteList[k];
                var num = spriteInfo.depth;
                GameObject gameObject4 = new GameObject(str + "_Sprite_" + k.ToString("D3"));
                Transform transform3 = gameObject4.transform;
                transform3.parent = transform2;
                {
                    transform3.localPosition = new Vector3(spriteInfo.offX * 1f, (spriteInfo.offY + 16) * 1f, num);
                }
                transform3.localScale = new Vector3(1f, 1f, 1f);
                spriteInfo.transform = transform3;
                list.Clear();
                list2.Clear();
                list3.Clear();
                list.Add(new Vector3(0f, -16f, 0f));
                list.Add(new Vector3(16f, -16f, 0f));
                list.Add(new Vector3(16f, 0f, 0f));
                list.Add(new Vector3(0f, 0f, 0f));
                Single num2 = this.ATLAS_W;
                Single num3 = this.ATLAS_H;
                Single x;
                Single y;
                Single x2;
                Single y2;
                if (UseUpscalFM)
                {
                    Single num4 = 0.5f;
                    x = (spriteInfo.atlasX - num4) / num2;
                    y = (this.ATLAS_H - spriteInfo.atlasY + num4) / num3;
                    x2 = (spriteInfo.atlasX + this.SPRITE_W - num4) / num2;
                    y2 = (this.ATLAS_H - (spriteInfo.atlasY + this.SPRITE_H) + num4) / num3;
                }
                else
                {
                    Single num5 = 0.5f;
                    x = (spriteInfo.atlasX + num5) / num2;
                    y = (spriteInfo.atlasY + num5) / num3;
                    x2 = (spriteInfo.atlasX + this.SPRITE_W - num5) / num2;
                    y2 = (spriteInfo.atlasY + this.SPRITE_H - num5) / num3;
                }
                list2.Add(new Vector2(x, y));
                list2.Add(new Vector2(x2, y));
                list2.Add(new Vector2(x2, y2));
                list2.Add(new Vector2(x, y2));
                list3.Add(2);
                list3.Add(1);
                list3.Add(0);
                list3.Add(3);
                list3.Add(2);
                list3.Add(0);
                Mesh mesh = new Mesh
                {
                    vertices = list.ToArray(),
                    uv = list2.ToArray(),
                    triangles = list3.ToArray()
                };
                MeshRenderer meshRenderer = gameObject4.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = gameObject4.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                Int32 num6 = this.curZ + (Int16)overlayInfo.curZ + spriteInfo.depth;
                GameObject expr_5B4 = gameObject4;
                expr_5B4.name = expr_5B4.name + "_Depth(" + num6.ToString("D5") + ")";
                String text;
                if (spriteInfo.trans != 0)
                {
                    if (spriteInfo.alpha == 0)
                    {
                        text = "abr_0";
                    }
                    else if (spriteInfo.alpha == 1)
                    {
                        text = "abr_1";
                    }
                    else if (spriteInfo.alpha == 2)
                    {
                        text = "abr_2";
                    }
                    else
                    {
                        text = "abr_3";
                    }
                }
                else
                {
                    text = "abr_none";
                }
                if (fieldMap && fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && j == 14)
                {
                    text = "abr_none";
                }
                GameObject expr_671 = gameObject4;
                expr_671.name = expr_671.name + "_[" + text + "]";
                meshRenderer.material = this.materialList[text];
            }
            overlayInfo.transform.gameObject.SetActive((overlayInfo.flags & 2) != 0);
        }

    }

    private void CreateScene(FieldMap fieldMap, Boolean UseUpscalFM, String path)
    {
        GameObject gameObject = new GameObject("Background");
        gameObject.transform.parent = fieldMap?.transform;


        gameObject.transform.localPosition = new Vector3(this.curX - FieldMap.HalfFieldWidth, -(this.curY - FieldMap.HalfFieldHeight), this.curZ);
        gameObject.transform.localScale = new Vector3(1f, -1f, 1f);
        for (Int32 i = 0; i < this.cameraList.Count; i++)
        {
            BGCAM_DEF bGCAM_DEF = this.cameraList[i];
            GameObject gameObject2 = new GameObject(String.Concat("Camera_", i.ToString("D2"), " : ", bGCAM_DEF.vrpMaxX + FieldMap.HalfFieldWidth, " x ", bGCAM_DEF.vrpMaxY + FieldMap.HalfFieldHeight));
            Transform transform = gameObject2.transform;
            transform.parent = gameObject.transform;
            bGCAM_DEF.transform = transform;
            bGCAM_DEF.transform.localPosition = Vector3.zero;
            bGCAM_DEF.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        String externalPath = Path.Combine(Configuration.Import.Path, path);

        string psdPath = Path.Combine(externalPath, "test.psd");
        string atlasPath = Path.Combine(externalPath, "atlases\\atlas_1.png");
        string psdMetaPath = Path.Combine(externalPath, "psd.meta");

        if (Configuration.Export.Enabled && Configuration.Export.Field)
        {
            var newPath = Path.Combine(Configuration.Import.Path, path);
            this.handleOverlays(fieldMap, UseUpscalFM, newPath);
        }
        else
        {
            // check if atlas has already been created
            if (Configuration.Import.Field)
            {
                try
                {
                    if (File.Exists(psdPath) &&
                        (!File.Exists(atlasPath)
                        || File.GetLastWriteTimeUtc(psdPath) > File.GetLastWriteTimeUtc(atlasPath)))
                    {


                        PsdInfo psdInfo = PsdInfo.Load(psdMetaPath);
                        PsdFile psdfile = new PsdFile(psdPath, new LoadContext());

                        this.createAtlas(externalPath, path, psdfile, 2048, psdInfo, File.GetLastWriteTimeUtc(psdPath));
                    }
                    else Log.Message("No psd or no need to create atlas");
                }
                catch (Exception e)
                {
                    Log.Message($"{e}");
                }
            }
            this.loadLocalizationInfo(this.mapName, path);
            if (Configuration.Import.Field && !Configuration.Export.Field && File.Exists(atlasPath))
            {
                this.importOverlaysFromPsd(fieldMap, UseUpscalFM, externalPath);
            }
            else
            {
                this.handleOverlays(fieldMap, UseUpscalFM, externalPath);
            }
        }

        for (Int32 l = 0; l < this.animList.Count; l++)
        {
            BGANIM_DEF bGANIM_DEF = this.animList[l];
            for (Int32 m = 0; m < bGANIM_DEF.frameList.Count; m++)
            {
                GameObject gameObject5 = this.overlayList[bGANIM_DEF.frameList[m].target].transform.gameObject;
                GameObject expr_754 = gameObject5;
                expr_754.name = expr_754.name + "_[anim_" + l.ToString("D2") + "]";
                GameObject expr_77C = gameObject5;
                String text2 = expr_77C.name;
                expr_77C.name = String.Concat(text2, "_[frame_", m.ToString("D2"), "_of_", bGANIM_DEF.frameList.Count.ToString("D2"), "]");
            }
        }
    }

    private List<Layer> getOrderedLayerList(PsdFile psd, PsdInfo psdInfo)
    {
        string order = psdInfo.LayerOrderFromPsdSection;
        bool reverse = psdInfo.ReversedFromPsdSection == 1 ? true : false;
        if (!reverse)
            psd.Layers.Reverse();
        List<Layer> newPsdList = new List<Layer>();
        // rearrange layers
        if (order == "depth")
        {
            List<BGOVERLAY_DEF> myorder = new List<BGOVERLAY_DEF>();
            for (var j = 0; j < this.overlayList.Count; j++)
            {
                BGOVERLAY_DEF ovdef = this.overlayList[j];
                myorder.Push(ovdef);
            }
            myorder.Sort(delegate (BGOVERLAY_DEF x, BGOVERLAY_DEF y)
            {
                return x.curZ == y.curZ ?
                (x.indnum.CompareTo(y.indnum)) : x.curZ.CompareTo(y.curZ);
            });


            // sort overlaylist by depth

            Layer[] newLayerList = new Layer[this.overlayList.Count];
            for (var i = 0; i < myorder.Count; i++)
            {
                newLayerList[myorder[i].indnum] = psd.Layers[i];
            }

            newPsdList.AddRange(newLayerList);
        }
        else
        {
            newPsdList = psd.Layers;
        }
        return newPsdList;
    }

    private byte[] generateEmptyAtlasArray(int atlasSide)
    {
        var product = atlasSide * atlasSide * 4;
        byte[] atlasArray = new byte[product];
        for (var i = 0; i < product; i++)
        {
            atlasArray[i] = 0;
        }
        return atlasArray;
    }

    private uint getFactor(List<Layer> noLocalizationList)
    {
        Layer[] layers = noLocalizationList.ToArray();

        int firstNonEmptyIndex = 0;
        // find first non-empty overlay for comparison
        for (var j = 0; j < this.overlayList.Count; j++)
        {
            if (this.overlayList[j].spriteList.Count > 0)
            {
                firstNonEmptyIndex = j;
                break;
            }
        }
        uint factor = (uint)Math.Ceiling((double)layers[firstNonEmptyIndex].Rect.Height / (double)this.overlayList[firstNonEmptyIndex].h);
        return factor;
    }

    private void doCreateAtlas(List<Layer> layers, List<BGOVERLAY_DEF> overlays, 
        List<BGANIM_DEF> animationOverlays, List<BGLIGHT_DEF> lightOverlays,
        String atlasFilename, 
        CopyBytesHelper copyHelper, int atlasSide, uint factor, Texture2D atlasTexture)
    {
        int padding = Convert.ToInt32(factor);
        TileMap[] tileSystems = new TileMap[this.cameraCount];
        for (var i = 0; i < this.cameraCount; i++)
        {
            tileSystems[i] = new TileMap(FF9StateSystem.Common.FF9.fldMapNo, layers,
                overlays, animationOverlays, lightOverlays, i, factor);
            copyHelper.FillBackgroundOverlays(tileSystems[i]);
        }

        int atlasX = padding, atlasY = padding;
        int deltaX = Convert.ToInt32(copyHelper._tileWidth) + Convert.ToInt32(padding) * 2;
        int deltaY = Convert.ToInt32(copyHelper._tileHeight) + Convert.ToInt32(padding) * 2;
        // setup atlas variables
        byte[] atlasArray = generateEmptyAtlasArray(atlasSide);

        uint atlasesWritten = 0;

        for (Int32 j = 0; j < overlays.Count; j++)
        {
            BGOVERLAY_DEF overlayInfo = overlays[j];
            TileMap tileSystem = tileSystems[overlayInfo.camNdx];
            Overlay memoriaOverlay = tileSystem.GetOverlay(j);
            for (Int32 k = 0; k < overlayInfo.spriteList.Count; k++)
            {
                BGSPRITE_LOC_DEF spriteInfo = overlayInfo.spriteList[k];

                // okay guise let's get a tile

                int grabX = (overlayInfo.curX + spriteInfo.offX - tileSystem.MinX) / 16;
                int grabY = (overlayInfo.curY + spriteInfo.offY - tileSystem.MinY) / 16;
                Tile memoriaTile = memoriaOverlay.GetTile(grabX, grabY);

                copyHelper.CopyTile(atlasArray, atlasSide, atlasX, atlasY, memoriaTile, memoriaOverlay, false);
                foreach (var paddingType in EnumCache<PaddingType>.Values)
                {
                    if (copyHelper.PaddingNeeded(memoriaTile, memoriaOverlay, paddingType))
                    {
                        Padding memoriaPadding = tileSystem.GetPaddingForTile(paddingType, memoriaOverlay, grabX, grabY);
                        copyHelper.CopyPaddingByPixels(atlasArray, atlasSide, atlasX, atlasY, memoriaPadding);
                    }
                }

                atlasX += deltaX;
                if (atlasX + deltaX > atlasSide)
                {
                    atlasX = padding;
                    atlasY += deltaY;
                }
                if (atlasY + deltaY > atlasSide)
                {
                    // write atlas to file, flush atlas and so on

                    atlasTexture.LoadRawTextureData(atlasArray);
                    atlasTexture.Apply();
                    for (var i = 0; i < atlasSide * atlasSide * 4; i++)
                    {
                        atlasArray[i] = 0;
                    }
                    atlasX = padding;
                    atlasY = padding;
                    string fullAtlasPath = Path.Combine(Path.Combine(this.atlasPath, "atlases"), $"{atlasFilename}_{++atlasesWritten}.png");
                    TextureHelper.WriteTextureToFile(atlasTexture, fullAtlasPath);

                    // TODO: This is time of PSD, not atlas... is it okay?
                    File.SetLastWriteTimeUtc(fullAtlasPath, this.atlasTimestamp);
                }
            }
        }

        try
        {
            Log.Message($"Trying to write to atlas {atlasArray.Length}");
            atlasTexture.LoadRawTextureData(atlasArray);
        }
        catch (Exception e)
        {
            Log.Message($"{e}");
        }

        atlasTexture.Apply();

        string finalAtlasPath = Path.Combine(Path.Combine(this.atlasPath, "atlases"), $"{atlasFilename}_{++atlasesWritten}.png");
        TextureHelper.WriteTextureToFile(atlasTexture, finalAtlasPath);
        File.SetLastWriteTimeUtc(finalAtlasPath, this.atlasTimestamp);
        string strings = $"[AtlasSection]\nTotalAtlases={atlasesWritten}\nTileSize={16 * factor}\nAtlasSide={atlasSide}";
        System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(Path.Combine(this.atlasPath, "atlases"), "atlas.meta"));
        sw.WriteLine(strings);
        sw.Close();
        Log.Message("End create atlas");
    }

    private void createAtlas(String externalPath, String resourcePath, PsdFile psd, int atlasSide, PsdInfo psdInfo, DateTime timestamp)
    {
        this.atlasPath = externalPath;
        this.atlasTimestamp = timestamp;
        // decompose into smaller functions: rearrange layers, create atlas

        List<Layer> newPsdList = this.getOrderedLayerList(psd, psdInfo);
        List<Layer> noLocalizationList = newPsdList.Where(x => !x.Name.Contains('_')).ToList();

        Texture2D atlasTexture = new Texture2D(atlasSide, atlasSide, TextureFormat.RGBA32, false);
        if (!Directory.Exists(Path.Combine(externalPath, "atlases")))
        {
            Directory.CreateDirectory(Path.Combine(externalPath, "atlases"));
        }


        uint factor = getFactor(noLocalizationList);
        CopyBytesHelper copyHelper = new CopyBytesHelper(factor, 16 * factor, 16 * factor);

        // actual useful part
        FieldMapLocalizeAreaTitleInfo info = FieldMapInfo.localizeAreaTitle.GetInfo(this.mapName);

        List<BGOVERLAY_DEF> mainList = new List<BGOVERLAY_DEF>(this.overlayList);
        List<Layer> mainLayerList = new List<Layer>(noLocalizationList);

        if(info != null)
        {
            Int32 startOvrIdx = info.startOvrIdx;
            Int32 endOvrIdx = info.endOvrIdx;
            for(var i = endOvrIdx; i >= startOvrIdx; i--)
            {
                mainLayerList.RemoveAt(i);
                mainList.RemoveAt(i);
            }
        }

        this.doCreateAtlas(mainLayerList, mainList, this.animList, this.lightList, 
            "atlas", copyHelper, atlasSide, factor, atlasTexture);

        // create all other atlases
        if(info != null)
        {
            Int32 startOvrIdx = info.startOvrIdx;
            Int32 endOvrIdx = info.endOvrIdx;
            // find all overlay indices

            Log.Message($"Creating localization atlases for {this.mapName}");
            foreach (var language in Configuration.Export.Languages)
            {
                BGSCENE_DEF bb = new BGSCENE_DEF(this.useUpscaleFM);
                bb.overlayList = new List<BGOVERLAY_DEF>(this.overlayList);
                BGSCENE_DEF bGSCENE_DEF = new BGSCENE_DEF(this.useUpscaleFM);
                bGSCENE_DEF._LoadDummyEBG(bb, resourcePath, this.mapName, info, language);
                List<Layer> restrictedLayerList = new List<Layer>();
                List<BGOVERLAY_DEF> restrictedOverlayList = new List<BGOVERLAY_DEF>();
                for(var i = startOvrIdx; i <= endOvrIdx; i++)
                {
                    Log.Message($"Overlay {startOvrIdx}, localization for {language}, spriteCount {this.overlayList[i].spriteList.Count}");
                    restrictedLayerList.Add(newPsdList.First(x => x.Name == $"{noLocalizationList[i].Name}_{language}"));
                    restrictedOverlayList.Add(bb.overlayList[i]);
                }

                Log.Message($"Creating atlas for language {language}");
                this.doCreateAtlas(restrictedLayerList, restrictedOverlayList, new List<BGANIM_DEF>(), new List<BGLIGHT_DEF>(), 
                    $"atlas_{language}", copyHelper, atlasSide, factor, atlasTexture);
            }
        }
        //String symbol = Localization.GetSymbol();
        //BGSCENE_DEF dummy = new BGSCENE_DEF(this.useUpscaleFM);
        //dummy._LoadDummyEBG(this, resourcePath, this.mapName, info, symbol);
    }

    public void CreateSeparateOverlay(FieldMap fieldMap, bool UseUpscalFM, uint ovrNdx)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.overlayList[(int)ovrNdx];
        if (bgoverlay_DEF.isCreated && !bgoverlay_DEF.canCombine)
        {
            return;
        }
        bgoverlay_DEF.canCombine = false;
        bool flag = false;
        List<Vector3> list = new List<Vector3>();
        List<Vector2> list2 = new List<Vector2>();
        List<int> list3 = new List<int>();
        MeshFilter component = bgoverlay_DEF.transform.GetComponent<MeshFilter>();
        if (component != (UnityEngine.Object)null)
        {
            UnityEngine.Object.Destroy(component);
        }
        MeshRenderer component2 = bgoverlay_DEF.transform.GetComponent<MeshRenderer>();
        if (component2 != (UnityEngine.Object)null)
        {
            UnityEngine.Object.Destroy(component2);
        }
        for (int i = 0; i < bgoverlay_DEF.spriteList.Count; i++)
        {
            BGSPRITE_LOC_DEF bgsprite_LOC_DEF = bgoverlay_DEF.spriteList[i];
            int num = 0;
            if (!flag)
            {
                num = bgsprite_LOC_DEF.depth;
            }
            
            // TODO Check Native: #147
            if (FF9StateSystem.Common.FF9.fldMapNo == 2714) // Pand./Maze -> FBG_N42_PDMN_MAP734_PD_MZM_0
            {
                Int32 num23 = i;
                switch (num23)
                {
                    case 64:
                    case 65:
                    case 66:
                    case 80:
                    case 81:
                        num = 400;
                        break;
                }
            }
            GameObject gameObject = new GameObject(bgoverlay_DEF.transform.name + "_Sprite_" + i.ToString("D3"));
            Transform transform = gameObject.transform;
            transform.parent = bgoverlay_DEF.transform;
            if (flag)
            {
                transform.localPosition = new Vector3((float)(bgoverlay_DEF.scrX + (short)bgsprite_LOC_DEF.offX) * 1f, (float)(bgoverlay_DEF.scrY + (short)bgsprite_LOC_DEF.offY + 16) * 1f, 0f);
            }
            else
            {
                transform.localPosition = new Vector3((float)bgsprite_LOC_DEF.offX * 1f, (float)(bgsprite_LOC_DEF.offY + 16) * 1f, (float)num);
            }
            transform.localScale = new Vector3(1f, 1f, 1f);
            bgsprite_LOC_DEF.transform = transform;
            list.Clear();
            list2.Clear();
            list3.Clear();
            list.Add(new Vector3(0f, -16f, 0f));
            list.Add(new Vector3(16f, -16f, 0f));
            list.Add(new Vector3(16f, 0f, 0f));
            list.Add(new Vector3(0f, 0f, 0f));
            float num2 = this.ATLAS_W;
            float num3 = this.ATLAS_H;
            float x;
            float y;
            float x2;
            float y2;
            if (UseUpscalFM)
            {
                float num4 = 0.5f;
                x = ((float)bgsprite_LOC_DEF.atlasX - num4) / num2;
                y = (this.ATLAS_H - (uint)bgsprite_LOC_DEF.atlasY + num4) / num3;
                x2 = ((uint)bgsprite_LOC_DEF.atlasX + this.SPRITE_W - num4) / num2;
                y2 = (this.ATLAS_H - ((uint)bgsprite_LOC_DEF.atlasY + this.SPRITE_H) + num4) / num3;
            }
            else
            {
                float num5 = 0.5f;
                x = ((float)bgsprite_LOC_DEF.atlasX + num5) / num2;
                y = ((float)bgsprite_LOC_DEF.atlasY + num5) / num3;
                x2 = ((uint)bgsprite_LOC_DEF.atlasX + this.SPRITE_W - num5) / num2;
                y2 = ((uint)bgsprite_LOC_DEF.atlasY + this.SPRITE_H - num5) / num3;
            }
            list2.Add(new Vector2(x, y));
            list2.Add(new Vector2(x2, y));
            list2.Add(new Vector2(x2, y2));
            list2.Add(new Vector2(x, y2));
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
            int num6 = (int)(this.curZ + (short)bgoverlay_DEF.curZ) + bgsprite_LOC_DEF.depth;
            GameObject gameObject2 = gameObject;
            gameObject2.name = gameObject2.name + "_Depth(" + num6.ToString("D5") + ")";
            string text = string.Empty;
            if (bgsprite_LOC_DEF.trans != 0)
            {
                if (bgsprite_LOC_DEF.alpha == 0)
                {
                    text = "abr_0";
                }
                else if (bgsprite_LOC_DEF.alpha == 1)
                {
                    text = "abr_1";
                }
                else if (bgsprite_LOC_DEF.alpha == 2)
                {
                    text = "abr_2";
                }
                else
                {
                    text = "abr_3";
                }
            }
            else
            {
                text = "abr_none";
            }
            if (fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && ovrNdx == 14u)
            {
                text = "abr_none";
            }
            GameObject gameObject3 = gameObject;
            gameObject3.name = gameObject3.name + "_[" + text + "]";
            meshRenderer.material = this.materialList[text];
        }
    }

    public void CreateSeparateSprites(FieldMap fieldMap, bool UseUpscalFM, uint ovrNdx, List<int> spriteIdx)
    {
        BGOVERLAY_DEF bgoverlay_DEF = this.overlayList[(int)ovrNdx];
        bool flag = false;
        List<Vector3> list = new List<Vector3>();
        List<Vector2> list2 = new List<Vector2>();
        List<int> list3 = new List<int>();
        int num = (int)bgoverlay_DEF.transform.localPosition.z;
        for (int i = 0; i < spriteIdx.Count; i++)
        {
            BGSPRITE_LOC_DEF bgsprite_LOC_DEF = bgoverlay_DEF.spriteList[spriteIdx[i]];
            int num2 = 0;
            if (!flag)
            {
                num2 = bgsprite_LOC_DEF.depth + num;
            }
            GameObject gameObject = new GameObject(bgoverlay_DEF.transform.name + "_Sprite_" + i.ToString("D3"));
            Transform transform = gameObject.transform;
            transform.parent = bgoverlay_DEF.transform;
            if (flag)
            {
                transform.localPosition = new Vector3((float)(bgoverlay_DEF.scrX + (short)bgsprite_LOC_DEF.offX) * 1f, (float)(bgoverlay_DEF.scrY + (short)bgsprite_LOC_DEF.offY + 16) * 1f, 0f);
            }
            else
            {
                transform.localPosition = new Vector3((float)bgsprite_LOC_DEF.offX * 1f, (float)(bgsprite_LOC_DEF.offY + 16) * 1f, (float)num2);
            }
            transform.localScale = new Vector3(1f, 1f, 1f);
            bgsprite_LOC_DEF.transform = transform;
            list.Clear();
            list2.Clear();
            list3.Clear();
            list.Add(new Vector3(0f, -16f, 0f));
            list.Add(new Vector3(16f, -16f, 0f));
            list.Add(new Vector3(16f, 0f, 0f));
            list.Add(new Vector3(0f, 0f, 0f));
            float num3 = this.ATLAS_W;
            float num4 = this.ATLAS_H;
            float x;
            float y;
            float x2;
            float y2;
            if (UseUpscalFM)
            {
                float num5 = 0.5f;
                x = ((float)bgsprite_LOC_DEF.atlasX - num5) / num3;
                y = (this.ATLAS_H - (uint)bgsprite_LOC_DEF.atlasY + num5) / num4;
                x2 = ((uint)bgsprite_LOC_DEF.atlasX + this.SPRITE_W - num5) / num3;
                y2 = (this.ATLAS_H - ((uint)bgsprite_LOC_DEF.atlasY + this.SPRITE_H) + num5) / num4;
            }
            else
            {
                float num6 = 0.5f;
                x = ((float)bgsprite_LOC_DEF.atlasX + num6) / num3;
                y = ((float)bgsprite_LOC_DEF.atlasY + num6) / num4;
                x2 = ((uint)bgsprite_LOC_DEF.atlasX + this.SPRITE_W - num6) / num3;
                y2 = ((uint)bgsprite_LOC_DEF.atlasY + this.SPRITE_H - num6) / num4;
            }
            list2.Add(new Vector2(x, y));
            list2.Add(new Vector2(x2, y));
            list2.Add(new Vector2(x2, y2));
            list2.Add(new Vector2(x, y2));
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
            int num7 = (int)(this.curZ + (short)bgoverlay_DEF.curZ) + bgsprite_LOC_DEF.depth;
            GameObject gameObject2 = gameObject;
            gameObject2.name = gameObject2.name + "_Depth(" + num7.ToString("D5") + ")";
            string text = string.Empty;
            if (bgsprite_LOC_DEF.trans != 0)
            {
                if (bgsprite_LOC_DEF.alpha == 0)
                {
                    text = "abr_0";
                }
                else if (bgsprite_LOC_DEF.alpha == 1)
                {
                    text = "abr_1";
                }
                else if (bgsprite_LOC_DEF.alpha == 2)
                {
                    text = "abr_2";
                }
                else
                {
                    text = "abr_3";
                }
            }
            else
            {
                text = "abr_none";
            }
            if (fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && ovrNdx == 14u)
            {
                text = "abr_none";
            }
            GameObject gameObject3 = gameObject;
            gameObject3.name = gameObject3.name + "_[" + text + "]";
            meshRenderer.material = this.materialList[text];
        }
    }


    private void CreateSceneCombined(FieldMap fieldMap, bool UseUpscalFM)
    {
        bool flag = false;
        GameObject gameObject = new GameObject("Background");
        gameObject.transform.parent = fieldMap.transform;
        if (flag)
        {
            gameObject.transform.localPosition = new Vector3((float)this.curX - FieldMap.HalfFieldWidth, -((float)this.curY - FieldMap.HalfFieldHeight), 0f);
        }
        else
        {
            gameObject.transform.localPosition = new Vector3((float)this.curX - FieldMap.HalfFieldWidth, -((float)this.curY - FieldMap.HalfFieldHeight), (float)this.curZ);
        }
        gameObject.transform.localScale = new Vector3(1f, -1f, 1f);
        for (int i = 0; i < this.cameraList.Count; i++)
        {
            BGCAM_DEF bgcam_DEF = this.cameraList[i];
            GameObject gameObject2 = new GameObject(string.Concat(new object[]
            {
                "Camera_",
                i.ToString("D2"),
                " : ",
                (float)bgcam_DEF.vrpMaxX + FieldMap.HalfFieldWidth,
                " x ",
                (float)bgcam_DEF.vrpMaxY + FieldMap.HalfFieldHeight
            }));
            Transform transform = gameObject2.transform;
            transform.parent = gameObject.transform;
            bgcam_DEF.transform = transform;
            bgcam_DEF.transform.localPosition = Vector3.zero;
            bgcam_DEF.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        FieldMap.EbgCombineMeshData currentCombineMeshData = fieldMap.GetCurrentCombineMeshData();
        List<int> list = null;
        if (currentCombineMeshData != null)
        {
            list = currentCombineMeshData.skipOverlayList;
        }
        List<Vector3> list2 = new List<Vector3>();
        List<Vector2> list3 = new List<Vector2>();
        List<int> list4 = new List<int>();
        for (int j = 0; j < this.overlayList.Count; j++)
        {
            BGOVERLAY_DEF bgoverlay_DEF = this.overlayList[j];
            string text = "Overlay_" + j.ToString("D2");
            GameObject gameObject3 = new GameObject(text);
            Transform transform2 = gameObject3.transform;
            transform2.parent = this.cameraList[(int)bgoverlay_DEF.camNdx].transform;
            if (flag)
            {
                transform2.localPosition = new Vector3((float)bgoverlay_DEF.curX * 1f, (float)bgoverlay_DEF.curY * 1f, 0f);
            }
            else
            {
                transform2.localPosition = new Vector3((float)bgoverlay_DEF.curX * 1f, (float)bgoverlay_DEF.curY * 1f, (float)bgoverlay_DEF.curZ);
            }
            transform2.localScale = new Vector3(1f, 1f, 1f);
            bgoverlay_DEF.transform = transform2;
            list2.Clear();
            list3.Clear();
            list4.Clear();
            bgoverlay_DEF.canCombine = true;
            bgoverlay_DEF.isCreated = false;
            if ((bgoverlay_DEF.flags & 4) != 0)
            {
                bgoverlay_DEF.canCombine = false;
            }
            else if ((bgoverlay_DEF.flags & 128) != 0)
            {
                bgoverlay_DEF.canCombine = false;
            }
            else if (bgoverlay_DEF.spriteList.Count > 1)
            {
                bool flag2 = false;
                if (list != null && list.Contains(j))
                {
                    flag2 = true;
                }
                if (!flag2)
                {
                    int num = (int)((!fieldMap.IsCurrentFieldMapHasCombineMeshProblem()) ? 512 : 164);
                    BGSPRITE_LOC_DEF bgsprite_LOC_DEF = bgoverlay_DEF.spriteList[0];
                    int num2 = 4096;
                    int num3 = -4096;
                    for (int k = 0; k < bgoverlay_DEF.spriteList.Count; k++)
                    {
                        num2 = Mathf.Min(num2, bgoverlay_DEF.spriteList[k].depth);
                        num3 = Mathf.Max(num3, bgoverlay_DEF.spriteList[k].depth);
                        if (num3 - num2 > num)
                        {
                            bgoverlay_DEF.canCombine = false;
                            break;
                        }
                    }
                }
                else
                {
                    bgoverlay_DEF.canCombine = false;
                }
                if (FF9StateSystem.Common.FF9.fldMapNo == 552)
                {
                    if (j == 17)
                    {
                        bgoverlay_DEF.canCombine = true;
                    }
                    else
                    {
                        bgoverlay_DEF.canCombine = true;
                    }
                }
            }
            if (!bgoverlay_DEF.canCombine)
            {
                this.CreateSeparateOverlay(fieldMap, UseUpscalFM, (uint)j);
                if ((bgoverlay_DEF.flags & 2) != 0)
                {
                    bgoverlay_DEF.transform.gameObject.SetActive(true);
                }
                else
                {
                    bgoverlay_DEF.transform.gameObject.SetActive(false);
                }
                bgoverlay_DEF.isCreated = true;
            }
            else
            {
                List<int> list5 = null;
                if (FF9StateSystem.Common.FF9.fldMapNo == 552 && j == 17)
                {
                    list5 = new List<int>
                    {
                        202,
                        203,
                        214,
                        215
                    };
                }
                for (int l = 0; l < bgoverlay_DEF.spriteList.Count; l++)
                {
                    if (list5 == null || !list5.Contains(l))
                    {
                        BGSPRITE_LOC_DEF bgsprite_LOC_DEF2 = bgoverlay_DEF.spriteList[l];
                        int num4 = 0;
                        if (!flag)
                        {
                            num4 = bgsprite_LOC_DEF2.depth;
                        }
                        Vector3 zero = Vector3.zero;
                        if (flag)
                        {
                            zero = new Vector3((float)(bgoverlay_DEF.scrX + (short)bgsprite_LOC_DEF2.offX) * 1f, (float)(bgoverlay_DEF.scrY + (short)bgsprite_LOC_DEF2.offY + 16) * 1f, 0f);
                        }
                        else
                        {
                            zero = new Vector3((float)bgsprite_LOC_DEF2.offX * 1f, (float)(bgsprite_LOC_DEF2.offY + 16) * 1f, (float)num4);
                        }
                        int count = list2.Count;
                        list2.Add(new Vector3(0f, -16f, 0f) + zero);
                        list2.Add(new Vector3(16f, -16f, 0f) + zero);
                        list2.Add(new Vector3(16f, 0f, 0f) + zero);
                        list2.Add(new Vector3(0f, 0f, 0f) + zero);
                        float num5 = this.ATLAS_W;
                        float num6 = this.ATLAS_H;
                        float x;
                        float y;
                        float x2;
                        float y2;
                        if (UseUpscalFM)
                        {
                            float num7 = 0.5f;
                            x = ((float)bgsprite_LOC_DEF2.atlasX - num7) / num5;
                            y = (this.ATLAS_H - (uint)bgsprite_LOC_DEF2.atlasY + num7) / num6;
                            x2 = ((uint)bgsprite_LOC_DEF2.atlasX + this.SPRITE_W - num7) / num5;
                            y2 = (this.ATLAS_H - ((uint)bgsprite_LOC_DEF2.atlasY + this.SPRITE_H) + num7) / num6;
                        }
                        else
                        {
                            float num8 = 0.5f;
                            x = ((float)bgsprite_LOC_DEF2.atlasX + num8) / num5;
                            y = ((float)bgsprite_LOC_DEF2.atlasY + num8) / num6;
                            x2 = ((uint)bgsprite_LOC_DEF2.atlasX + this.SPRITE_W - num8) / num5;
                            y2 = ((uint)bgsprite_LOC_DEF2.atlasY + this.SPRITE_H - num8) / num6;
                        }
                        list3.Add(new Vector2(x, y));
                        list3.Add(new Vector2(x2, y));
                        list3.Add(new Vector2(x2, y2));
                        list3.Add(new Vector2(x, y2));
                        list4.Add(count + 2);
                        list4.Add(count + 1);
                        list4.Add(count);
                        list4.Add(count + 3);
                        list4.Add(count + 2);
                        list4.Add(count);
                    }
                }
                if (bgoverlay_DEF.spriteList.Count > 0)
                {
                    Mesh mesh = new Mesh();
                    mesh.vertices = list2.ToArray();
                    mesh.uv = list3.ToArray();
                    mesh.triangles = list4.ToArray();
                    MeshRenderer meshRenderer = gameObject3.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter = gameObject3.AddComponent<MeshFilter>();
                    meshFilter.mesh = mesh;
                    string text2 = string.Empty;
                    BGSPRITE_LOC_DEF bgsprite_LOC_DEF3 = bgoverlay_DEF.spriteList[0];
                    if (bgsprite_LOC_DEF3.trans != 0)
                    {
                        if (bgsprite_LOC_DEF3.alpha == 0)
                        {
                            text2 = "abr_0";
                        }
                        else if (bgsprite_LOC_DEF3.alpha == 1)
                        {
                            text2 = "abr_1";
                        }
                        else if (bgsprite_LOC_DEF3.alpha == 2)
                        {
                            text2 = "abr_2";
                        }
                        else
                        {
                            text2 = "abr_3";
                        }
                    }
                    else
                    {
                        text2 = "abr_none";
                    }
                    if (fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && j == 14)
                    {
                        text2 = "abr_none";
                    }
                    GameObject gameObject4 = gameObject3;
                    gameObject4.name = gameObject4.name + "_[" + text2 + "]";
                    meshRenderer.material = this.materialList[text2];
                }
                if ((bgoverlay_DEF.flags & 2) != 0)
                {
                    bgoverlay_DEF.transform.gameObject.SetActive(true);
                }
                else
                {
                    bgoverlay_DEF.transform.gameObject.SetActive(false);
                }
                if (list5 != null)
                {
                    this.CreateSeparateSprites(fieldMap, this.useUpscaleFM, (uint)j, list5);
                }
                bgoverlay_DEF.isCreated = true;
            }
        }
        for (int m = 0; m < this.animList.Count; m++)
        {
            BGANIM_DEF bganim_DEF = this.animList[m];
            for (int n = 0; n < bganim_DEF.frameList.Count; n++)
            {
                GameObject gameObject5 = this.overlayList[(int)bganim_DEF.frameList[n].target].transform.gameObject;
                GameObject gameObject6 = gameObject5;
                gameObject6.name = gameObject6.name + "_[anim_" + m.ToString("D2") + "]";
                GameObject gameObject7 = gameObject5;
                string text3 = gameObject7.name;
                gameObject7.name = string.Concat(new string[]
                {
                    text3,
                    "_[frame_",
                    n.ToString("D2"),
                    "_of_",
                    bganim_DEF.frameList.Count.ToString("D2"),
                    "]"
                });
            }
        }
    }
}