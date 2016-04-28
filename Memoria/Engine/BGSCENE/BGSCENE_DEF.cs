using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using UnityEngine;

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
    public ushort sceneLength;

    public ushort depthBitShift;

    public ushort animCount;

    public ushort overlayCount;

    public ushort lightCount;

    public ushort cameraCount;

    public uint animOffset;

    public uint overlayOffset;

    public uint lightOffset;

    public uint cameraOffset;

    public short orgZ;

    public short curZ;

    public short orgX;

    public short orgY;

    public short curX;

    public short curY;

    public short minX;

    public short maxX;

    public short minY;

    public short maxY;

    public short scrX;

    public short scrY;

    public string name;

    public byte[] ebgBin;

    public List<BGOVERLAY_DEF> overlayList;

    public List<BGANIM_DEF> animList;

    public List<BGLIGHT_DEF> lightList;

    public List<BGCAM_DEF> cameraList;

    public Dictionary<string, Material> materialList;

    public PSXVram vram;

    public Texture2D atlas;

    public Texture2D atlasAlpha;

    public uint SPRITE_W = 16u;

    public uint SPRITE_H = 16u;

    public uint ATLAS_W = 1024u;

    public uint ATLAS_H = 1024u;

    private int spriteCount;

    private bool useUpscaleFM;

    public BGSCENE_DEF(bool useUpscaleFm)
    {
        this.useUpscaleFM = useUpscaleFm;
        this.name = string.Empty;
        this.ebgBin = null;
        this.overlayList = new List<BGOVERLAY_DEF>();
        this.animList = new List<BGANIM_DEF>();
        this.lightList = new List<BGLIGHT_DEF>();
        this.cameraList = new List<BGCAM_DEF>();
        this.materialList = new Dictionary<string, Material>();
    }

    private void InitPSXTextureAtlas()
    {
        this.vram = new PSXVram(true);
        this.atlas = new Texture2D((int)this.ATLAS_W, (int)this.ATLAS_H)
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
        for (int i = 0; i < (int)this.cameraCount; i++)
        {
            BGCAM_DEF bGCAM_DEF = new BGCAM_DEF();
            bGCAM_DEF.ReadData(reader);
            this.cameraList.Add(bGCAM_DEF);
        }
    }

    private void ExtractLightData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.lightOffset, SeekOrigin.Begin);
        for (int i = 0; i < (int)this.lightCount; i++)
        {
            BGLIGHT_DEF bGLIGHT_DEF = new BGLIGHT_DEF();
            bGLIGHT_DEF.ReadData(reader);
            this.lightList.Add(bGLIGHT_DEF);
        }
    }

    private void ExtractAnimationFrameData(BinaryReader reader)
    {
        for (int i = 0; i < (int)this.animCount; i++)
        {
            BGANIM_DEF bGANIM_DEF = this.animList[i];
            reader.BaseStream.Seek(bGANIM_DEF.offset, SeekOrigin.Begin);
            for (int j = 0; j < bGANIM_DEF.frameCount; j++)
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
        for (int i = 0; i < (int)this.animCount; i++)
        {
            BGANIM_DEF bGANIM_DEF = new BGANIM_DEF();
            bGANIM_DEF.ReadData(reader);
            this.animList.Add(bGANIM_DEF);
        }
    }

    private void ExtractSpriteData(BinaryReader reader)
    {
        this.spriteCount = 0;
        for (int i = 0; i < (int)this.overlayCount; i++)
        {
            BGOVERLAY_DEF bGOVERLAY_DEF = this.overlayList[i];
            this.spriteCount += bGOVERLAY_DEF.spriteCount;
        }
        if (this.useUpscaleFM)
        {
            this.ATLAS_H = (uint)this.atlas.height;
            this.ATLAS_W = (uint)this.atlas.width;
        }
        int num = this.atlas.width / 36;
        int num2 = 0;
        for (int j = 0; j < (int)this.overlayCount; j++)
        {
            BGOVERLAY_DEF bGOVERLAY_DEF2 = this.overlayList[j];
            reader.BaseStream.Seek(bGOVERLAY_DEF2.prmOffset, SeekOrigin.Begin);
            for (int k = 0; k < (int)bGOVERLAY_DEF2.spriteCount; k++)
            {
                BGSPRITE_LOC_DEF bGSPRITE_LOC_DEF = new BGSPRITE_LOC_DEF();
                bGSPRITE_LOC_DEF.ReadData_BGSPRITE_DEF(reader);
                bGOVERLAY_DEF2.spriteList.Add(bGSPRITE_LOC_DEF);
            }
            reader.BaseStream.Seek(bGOVERLAY_DEF2.locOffset, SeekOrigin.Begin);
            for (int l = 0; l < (int)bGOVERLAY_DEF2.spriteCount; l++)
            {
                BGSPRITE_LOC_DEF bGSPRITE_LOC_DEF2 = bGOVERLAY_DEF2.spriteList[l];
                bGSPRITE_LOC_DEF2.ReadData_BGSPRITELOC_DEF(reader);
                if (this.useUpscaleFM)
                {
                    bGSPRITE_LOC_DEF2.atlasX = (ushort)(2 + num2 % num * 36);
                    bGSPRITE_LOC_DEF2.atlasY = (ushort)(2 + num2 / num * 36);
                    bGSPRITE_LOC_DEF2.w = 32;
                    bGSPRITE_LOC_DEF2.h = 32;
                    num2++;
                }
            }
        }
    }

    private void ExtractOverlayData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.overlayOffset, SeekOrigin.Begin);
        for (int i = 0; i < (int)this.overlayCount; i++)
        {
            BGOVERLAY_DEF bGOVERLAY_DEF = new BGOVERLAY_DEF();
            bGOVERLAY_DEF.ReadData(reader);
            bGOVERLAY_DEF.minX = -32768;
            bGOVERLAY_DEF.maxX = 32767;
            bGOVERLAY_DEF.minY = -32768;
            bGOVERLAY_DEF.maxY = 32767;
            this.overlayList.Add(bGOVERLAY_DEF);
        }
    }

    private void _LoadDummyEBG(BGSCENE_DEF sceneUS, string path, string newName, FieldMapLocalizeAreaTitleInfo info, string localizeSymbol)
    {
        this.name = newName;
        TextAsset textAsset = AssetManager.Load<TextAsset>(string.Concat(path, newName, "_", localizeSymbol, ".bgs"), false);
        if (textAsset == null)
        {
            return;
        }
        this.ebgBin = textAsset.bytes;
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.ebgBin)))
        {
            this.ExtractHeaderData(binaryReader);
            this.ExtractOverlayData(binaryReader);
            int atlasWidth = info.atlasWidth;
            int startOvrIdx = info.startOvrIdx;
            int endOvrIdx = info.endOvrIdx;
            int spriteStartIndex = info.GetSpriteStartIndex(localizeSymbol);
            int num = atlasWidth / 36;
            int num2 = spriteStartIndex;
            for (int i = startOvrIdx; i <= endOvrIdx; i++)
            {
                BGOVERLAY_DEF bGOVERLAY_DEF = this.overlayList[i];
                binaryReader.BaseStream.Seek(bGOVERLAY_DEF.prmOffset, SeekOrigin.Begin);
                for (int j = 0; j < (int)bGOVERLAY_DEF.spriteCount; j++)
                {
                    BGSPRITE_LOC_DEF bGSPRITE_LOC_DEF = new BGSPRITE_LOC_DEF();
                    bGSPRITE_LOC_DEF.ReadData_BGSPRITE_DEF(binaryReader);
                    bGOVERLAY_DEF.spriteList.Add(bGSPRITE_LOC_DEF);
                }
                binaryReader.BaseStream.Seek(bGOVERLAY_DEF.locOffset, SeekOrigin.Begin);
                for (int k = 0; k < (int)bGOVERLAY_DEF.spriteCount; k++)
                {
                    BGSPRITE_LOC_DEF bGSPRITE_LOC_DEF2 = bGOVERLAY_DEF.spriteList[k];
                    bGSPRITE_LOC_DEF2.ReadData_BGSPRITELOC_DEF(binaryReader);
                    if (this.useUpscaleFM)
                    {
                        bGSPRITE_LOC_DEF2.atlasX = (ushort)(2 + num2 % num * 36);
                        bGSPRITE_LOC_DEF2.atlasY = (ushort)(2 + num2 / num * 36);
                        bGSPRITE_LOC_DEF2.w = 32;
                        bGSPRITE_LOC_DEF2.h = 32;
                        num2++;
                    }
                }
            }
            for (int l = startOvrIdx; l <= endOvrIdx; l++)
            {
                sceneUS.overlayList[l] = this.overlayList[l];
            }
        }
    }

    public void LoadEBG(FieldMap fieldMap, string path, string newName)
    {
        this.name = newName;
        if (!this.useUpscaleFM)
        {
            this.InitPSXTextureAtlas();
        }
        else
        {
            Texture2D x = AssetManager.Load<Texture2D>(Path.Combine(path, "atlas"), false);
            if (x != null)
            {
                this.atlas = x;
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    this.atlasAlpha = AssetManager.Load<Texture2D>(Path.Combine(path, "atlas_a"), false);
                }
                else
                {
                    this.atlasAlpha = null;
                }
                this.SPRITE_W = 32u;
                this.SPRITE_H = 32u;
            }
            else
            {
                this.useUpscaleFM = false;
                this.InitPSXTextureAtlas();
            }
        }
        if (!this.useUpscaleFM)
        {
            this.vram.LoadTIMs(path);
        }
        TextAsset textAsset;
        if (!FieldMapEditor.useOriginalVersion)
        {
            textAsset = AssetManager.Load<TextAsset>(path + FieldMapEditor.GetFieldMapModName(newName) + ".bgs", false);
            if (textAsset == null)
            {
                Debug.Log("Cannot find MOD version.");
                textAsset = AssetManager.Load<TextAsset>(path + newName + ".bgs", false);
            }
        }
        else
        {
            textAsset = AssetManager.Load<TextAsset>(path + newName + ".bgs", false);
        }

        if (textAsset == null)
            return;

        this.ebgBin = textAsset.bytes;
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.ebgBin)))
        {
            this.ReadData(binaryReader);
        }
        string symbol = Localization.GetSymbol();
        if (symbol != "US")
        {
            FieldMapLocalizeAreaTitleInfo info = FieldMapInfo.localizeAreaTitle.GetInfo(newName);
            if (info != null)
            {
                if (symbol != "UK" || info.hasUK)
                {
                    BGSCENE_DEF bGSCENE_DEF = new BGSCENE_DEF(this.useUpscaleFM);
                    bGSCENE_DEF._LoadDummyEBG(this, path, newName, info, symbol);
                }
            }
        }
        FieldMapInfo.fieldmapExtraOffset.SetOffset(newName, this.overlayList);
        if (!this.useUpscaleFM)
        {
            this.GenerateAtlasFromBinary();
        }

        this.CreateMaterials();
        if (fieldMap != null)
            this.CreateScene(fieldMap, this.useUpscaleFM);
    }

    private static Rect CalculateExpectedTextureAtlasSize(int spriteCount)
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
        for (int i = 0; i < array2.Length; i++)
        {
            Rect result = array2[i];
            int num = (int)result.width / 36;
            int num2 = (int)result.height / 36;
            if (num * num2 >= spriteCount)
            {
                return result;
            }
        }
        throw new ArgumentException("Unexpected size of atlas texture");
    }

    private void GenerateAtlasFromBinary()
    {
        uint num = this.ATLAS_W * this.ATLAS_H;
        Color32[] array = new Color32[num];
        uint num2 = 0u;
        uint num3 = 1u;
        for (int i = 0; i < (int)this.overlayCount; i++)
        {
            BGOVERLAY_DEF bGOVERLAY_DEF = this.overlayList[i];
            for (int j = 0; j < (int)bGOVERLAY_DEF.spriteCount; j++)
            {
                BGSPRITE_LOC_DEF bGSPRITE_LOC_DEF = bGOVERLAY_DEF.spriteList[j];
                bGSPRITE_LOC_DEF.atlasX = (ushort)num2;
                bGSPRITE_LOC_DEF.atlasY = (ushort)num3;
                if (bGSPRITE_LOC_DEF.res == 0)
                {
                    int index = ArrayUtil.GetIndex(bGSPRITE_LOC_DEF.clutX * 16, bGSPRITE_LOC_DEF.clutY, (int)this.vram.width, (int)this.vram.height);
                    for (uint num4 = 0u; num4 < (uint)bGSPRITE_LOC_DEF.h; num4 += 1u)
                    {
                        int index2 = ArrayUtil.GetIndex(bGSPRITE_LOC_DEF.texX * 64 + bGSPRITE_LOC_DEF.u / 4, (int)(bGSPRITE_LOC_DEF.texY * 256u + bGSPRITE_LOC_DEF.v + num4), (int)this.vram.width, (int)this.vram.height);
                        int index3 = ArrayUtil.GetIndex((int)num2, (int)(num3 + num4), (int)this.ATLAS_W, (int)this.ATLAS_H);
                        uint num5 = 0u;
                        while (num5 < (ulong)(bGSPRITE_LOC_DEF.w / 2))
                        {
                            byte b = this.vram.rawData[index2 * 2 + (int)num5];
                            byte b2 = (byte)(b & 15);
                            byte b3 = (byte)(b >> 4 & 15);
                            int num6 = (index + b2) * 2;
                            ushort num7 = (ushort)(this.vram.rawData[num6] | this.vram.rawData[num6 + 1] << 8);
                            int num8 = index3 + (int)(num5 * 2u);
                            ConvertColor16toColor32(num7, out array[num8]);
                            if (bGSPRITE_LOC_DEF.trans != 0 && num7 != 0)
                            {
                                if (bGSPRITE_LOC_DEF.alpha == 0)
                                {
                                    array[num8].a = 127;
                                }
                                else if (bGSPRITE_LOC_DEF.alpha == 3)
                                {
                                    array[num8].a = 63;
                                }
                            }
                            num6 = (index + b3) * 2;
                            num7 = (ushort)(this.vram.rawData[num6] | this.vram.rawData[num6 + 1] << 8);
                            num8 = index3 + (int)(num5 * 2u) + 1;
                            ConvertColor16toColor32(num7, out array[num8]);
                            if (bGSPRITE_LOC_DEF.trans != 0 && num7 != 0)
                            {
                                if (bGSPRITE_LOC_DEF.alpha == 0)
                                {
                                    array[num8].a = 127;
                                }
                                else if (bGSPRITE_LOC_DEF.alpha == 3)
                                {
                                    array[num8].a = 63;
                                }
                            }
                            num5 += 1u;
                        }
                    }
                }
                else if (bGSPRITE_LOC_DEF.res == 1)
                {
                    int index4 = ArrayUtil.GetIndex(bGSPRITE_LOC_DEF.clutX * 16, bGSPRITE_LOC_DEF.clutY, (int)this.vram.width, (int)this.vram.height);
                    for (uint num9 = 0u; num9 < (uint)bGSPRITE_LOC_DEF.h; num9 += 1u)
                    {
                        int index5 = ArrayUtil.GetIndex(bGSPRITE_LOC_DEF.texX * 64 + bGSPRITE_LOC_DEF.u / 2, (int)(bGSPRITE_LOC_DEF.texY * 256u + bGSPRITE_LOC_DEF.v + num9), (int)this.vram.width, (int)this.vram.height);
                        int index6 = ArrayUtil.GetIndex((int)num2, (int)(num3 + num9), (int)this.ATLAS_W, (int)this.ATLAS_H);
                        for (uint num10 = 0u; num10 < (uint)bGSPRITE_LOC_DEF.w; num10 += 1u)
                        {
                            byte b4 = this.vram.rawData[index5 * 2 + (int)num10];
                            int num11 = (index4 + b4) * 2;
                            ushort num12 = (ushort)(this.vram.rawData[num11] | this.vram.rawData[num11 + 1] << 8);
                            int num13 = index6 + (int)num10;
                            ConvertColor16toColor32(num12, out array[num13]);
                            if (bGSPRITE_LOC_DEF.trans != 0 && num12 != 0)
                            {
                                if (bGSPRITE_LOC_DEF.alpha == 0)
                                {
                                    array[num13].a = 127;
                                }
                                else if (bGSPRITE_LOC_DEF.alpha == 3)
                                {
                                    array[num13].a = 63;
                                }
                            }
                        }
                    }
                }
                for (uint num14 = 0u; num14 < (uint)bGSPRITE_LOC_DEF.h; num14 += 1u)
                {
                    int index7 = ArrayUtil.GetIndex((int)(num2 + this.SPRITE_W), (int)(num3 + num14), (int)this.ATLAS_W, (int)this.ATLAS_H);
                    array[index7] = array[index7 - 1];
                }
                for (uint num15 = 0u; num15 < (uint)bGSPRITE_LOC_DEF.w; num15 += 1u)
                {
                    int index8 = ArrayUtil.GetIndex((int)(num2 + num15), (int)num3, (int)this.ATLAS_W, (int)this.ATLAS_H);
                    int index9 = ArrayUtil.GetIndex((int)(num2 + num15), (int)(num3 - 1u), (int)this.ATLAS_W, (int)this.ATLAS_H);
                    array[index9] = array[index8];
                }
                int index10 = ArrayUtil.GetIndex((int)(num2 + this.SPRITE_W - 1u), (int)num3, (int)this.ATLAS_W, (int)this.ATLAS_H);
                int index11 = ArrayUtil.GetIndex((int)(num2 + this.SPRITE_W), (int)(num3 - 1u), (int)this.ATLAS_W, (int)this.ATLAS_H);
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

    private void CreateMaterials()
    {
        Material material = new Material(Shader.Find("PSX/FieldMap_Abr_None")) {mainTexture = this.atlas};
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_none", material);
        material = new Material(Shader.Find("PSX/FieldMap_Abr_0")) {mainTexture = this.atlas};
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_0", material);
        material = new Material(Shader.Find("PSX/FieldMap_Abr_1")) {mainTexture = this.atlas};
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_1", material);
        material = new Material(Shader.Find("PSX/FieldMap_Abr_2")) {mainTexture = this.atlas};
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_2", material);
        material = new Material(Shader.Find("PSX/FieldMap_Abr_3")) {mainTexture = this.atlas};
        if (this.atlasAlpha != null)
        {
            material.SetTexture("_AlphaTex", this.atlasAlpha);
        }
        this.materialList.Add("abr_3", material);
    }

    private void CreateScene(FieldMap fieldMap, bool UseUpscalFM)
    {
        GameObject gameObject = new GameObject("Background");
        gameObject.transform.parent = fieldMap.transform;
        gameObject.transform.localPosition = new Vector3(this.curX - 160f, -(this.curY - 112f), this.curZ);
        gameObject.transform.localScale = new Vector3(1f, -1f, 1f);
        for (int i = 0; i < this.cameraList.Count; i++)
        {
            BGCAM_DEF bGCAM_DEF = this.cameraList[i];
            GameObject gameObject2 = new GameObject(string.Concat("Camera_", i.ToString("D2"), " : ", bGCAM_DEF.vrpMaxX + 160f, " x ", bGCAM_DEF.vrpMaxY + 112f));
            Transform transform = gameObject2.transform;
            transform.parent = gameObject.transform;
            bGCAM_DEF.transform = transform;
            bGCAM_DEF.transform.localPosition = Vector3.zero;
            bGCAM_DEF.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        List<Vector3> list = new List<Vector3>();
        List<Vector2> list2 = new List<Vector2>();
        List<int> list3 = new List<int>();
        for (int j = 0; j < this.overlayList.Count; j++)
        {
            BGOVERLAY_DEF bGOVERLAY_DEF = this.overlayList[j];
            string str = "Overlay_" + j.ToString("D2");
            GameObject gameObject3 = new GameObject(str);
            Transform transform2 = gameObject3.transform;
            transform2.parent = this.cameraList[bGOVERLAY_DEF.camNdx].transform;
            transform2.localPosition = new Vector3(bGOVERLAY_DEF.curX * 1f, bGOVERLAY_DEF.curY * 1f, bGOVERLAY_DEF.curZ);
            transform2.localScale = new Vector3(1f, 1f, 1f);
            bGOVERLAY_DEF.transform = transform2;
            for (int k = 0; k < bGOVERLAY_DEF.spriteList.Count; k++)
            {
                BGSPRITE_LOC_DEF bGSPRITE_LOC_DEF = bGOVERLAY_DEF.spriteList[k];
                var num = bGSPRITE_LOC_DEF.depth;
                GameObject gameObject4 = new GameObject(str + "_Sprite_" + k.ToString("D3"));
                Transform transform3 = gameObject4.transform;
                transform3.parent = transform2;
                {
                    transform3.localPosition = new Vector3(bGSPRITE_LOC_DEF.offX * 1f, (bGSPRITE_LOC_DEF.offY + 16) * 1f, num);
                }
                transform3.localScale = new Vector3(1f, 1f, 1f);
                bGSPRITE_LOC_DEF.transform = transform3;
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
                    x = (bGSPRITE_LOC_DEF.atlasX - num4) / num2;
                    y = (this.ATLAS_H - bGSPRITE_LOC_DEF.atlasY + num4) / num3;
                    x2 = (bGSPRITE_LOC_DEF.atlasX + this.SPRITE_W - num4) / num2;
                    y2 = (this.ATLAS_H - (bGSPRITE_LOC_DEF.atlasY + this.SPRITE_H) + num4) / num3;
                }
                else
                {
                    float num5 = 0.5f;
                    x = (bGSPRITE_LOC_DEF.atlasX + num5) / num2;
                    y = (bGSPRITE_LOC_DEF.atlasY + num5) / num3;
                    x2 = (bGSPRITE_LOC_DEF.atlasX + this.SPRITE_W - num5) / num2;
                    y2 = (bGSPRITE_LOC_DEF.atlasY + this.SPRITE_H - num5) / num3;
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
                int num6 = this.curZ + (short)bGOVERLAY_DEF.curZ + bGSPRITE_LOC_DEF.depth;
                GameObject expr_5B4 = gameObject4;
                expr_5B4.name = expr_5B4.name + "_Depth(" + num6.ToString("D5") + ")";
                string text;
                if (bGSPRITE_LOC_DEF.trans != 0)
                {
                    if (bGSPRITE_LOC_DEF.alpha == 0)
                    {
                        text = "abr_0";
                    }
                    else if (bGSPRITE_LOC_DEF.alpha == 1)
                    {
                        text = "abr_1";
                    }
                    else if (bGSPRITE_LOC_DEF.alpha == 2)
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
            }
            bGOVERLAY_DEF.transform.gameObject.SetActive((bGOVERLAY_DEF.flags & 2) != 0);
        }
        for (int l = 0; l < this.animList.Count; l++)
        {
            BGANIM_DEF bGANIM_DEF = this.animList[l];
            for (int m = 0; m < bGANIM_DEF.frameList.Count; m++)
            {
                GameObject gameObject5 = this.overlayList[bGANIM_DEF.frameList[m].target].transform.gameObject;
                GameObject expr_754 = gameObject5;
                expr_754.name = expr_754.name + "_[anim_" + l.ToString("D2") + "]";
                GameObject expr_77C = gameObject5;
                string text2 = expr_77C.name;
                expr_77C.name = string.Concat(text2, "_[frame_", m.ToString("D2"), "_of_", bGANIM_DEF.frameList.Count.ToString("D2"), "]");
            }
        }
    }

    private static void ConvertColor16toColor32(ushort raw, out Color32 col)
    {
        byte num1 = (byte)((raw >> 10 & 31) << 3);
        byte num2 = (byte)((raw >> 5 & 31) << 3);
        byte num3 = (byte)((raw & 31) << 3);
        byte num4 = byte.MaxValue;
        if (raw == 0)
            num4 = 0;
        col.a = num4;
        col.r = num3;
        col.g = num2;
        col.b = num1;
    }
}