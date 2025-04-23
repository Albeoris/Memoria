﻿using Memoria.Assets;
using Memoria.Data;
using System;
using System.IO;
using UnityEngine;

public static class PSXTextureMgr
{
    public static void InitOnce()
    {
        if (PSXTextureMgr.isOnce)
        {
            global::Debug.Log("PSXTexture initialize should call once in a game");
            return;
        }
        PSXTextureMgr.isOnce = true;
        PSXTextureMgr.originalVram = new UInt16[524288]; // 1024*512
        for (Int32 i = 0; i < 524288; i++)
            PSXTextureMgr.originalVram[i] = 255;
        PSXTextureMgr.pixels256x256 = new Color32[65536];
        PSXTextureMgr.list = new PSXTexture[PSXTextureMgr.SST_MAX_TEXTURE];
        for (Int32 j = 0; j < PSXTextureMgr.SST_MAX_TEXTURE; j++)
            PSXTextureMgr.list[j] = new PSXTexture();
        PSXTextureMgr.genTexture = new RenderTexture(PSXTextureMgr.GEN_TEXTURE_W, PSXTextureMgr.GEN_TEXTURE_H, 0, RenderTextureFormat.RGB565);
        PSXTextureMgr.genTexture.enableRandomWrite = false;
        PSXTextureMgr.genTexture.wrapMode = TextureWrapMode.Clamp;
        PSXTextureMgr.genTexture.filterMode = FilterMode.Bilinear;
        PSXTextureMgr.genTexture.Create();
        PSXTextureMgr.bgTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        PSXTextureMgr.bgTexture.enableRandomWrite = false;
        PSXTextureMgr.bgTexture.wrapMode = TextureWrapMode.Clamp;
        PSXTextureMgr.bgTexture.filterMode = FilterMode.Bilinear;
        PSXTextureMgr.bgTexture.Create();
        PSXTextureMgr.bgParam = new Int32[7];
        PSXTextureMgr.isBgCapture = false;
        PSXTextureMgr.blurTexture = new RenderTexture(Screen.width / BLUR_SCALE, Screen.height / BLUR_SCALE, 0, RenderTextureFormat.Default);
        PSXTextureMgr.blurTexture.enableRandomWrite = false;
        PSXTextureMgr.blurTexture.wrapMode = TextureWrapMode.Clamp;
        PSXTextureMgr.blurTexture.filterMode = FilterMode.Bilinear;
        PSXTextureMgr.blurTexture.Create();
        PSXTextureMgr.isCaptureBlur = false;
    }

    public static void Reset()
    {
        PSXTextureMgr.bgKey = 0u;
        PSXTextureMgr.isBgCapture = false;
        PSXTextureMgr.isCaptureSS = false;
        PSXTextureMgr.isCreateGenTexture = false;
        PSXTextureMgr.isCaptureBlur = false;
        for (Int32 i = 0; i < PSXTextureMgr.SST_MAX_TEXTURE; i++)
            PSXTextureMgr.list[i].key = PSXTexture.EMPTY_KEY;
    }

    public static void BeginRender()
    {
    }

    public static void ClearObject()
    {
        for (Int32 i = 0; i < PSXTextureMgr.SST_MAX_TEXTURE; i++)
        {
            if (PSXTextureMgr.list[i] != null)
            {
                PSXTextureMgr.list[i].key = PSXTexture.EMPTY_KEY;
                PSXTextureMgr.list[i].texture = (Texture2D)null;
            }
        }
    }

    public static void Release()
    {
        PSXTextureMgr.eff435Tex = null;
        PSXTextureMgr.eff435Key = null;
    }

    private static void ClearKey(Int32 x, Int32 y)
    {
        x >>= 6;
        y >>= 8;
        for (Int32 i = 0; i < PSXTextureMgr.SST_MAX_TEXTURE; i++)
        {
            PSXTexture psxtexture = PSXTextureMgr.list[i];
            if (psxtexture.key != PSXTexture.EMPTY_KEY && psxtexture.tx == x && psxtexture.ty == y)
                psxtexture.key = PSXTexture.EMPTY_KEY;
        }
    }

    public unsafe static void LoadImage(Int32 x, Int32 y, Int32 w, Int32 h, UInt16* p)
    {
        PSXTextureMgr.ClearKey(x, y);
        for (Int32 i = 0; i < h; i++)
        {
            Int32 num = (y + i) * 1024 + x;
            for (Int32 j = 0; j < w; j++)
            {
                PSXTextureMgr.originalVram[num] = *(p++);
                num++;
            }
        }
        if (SFX.isDebugPng)
        {
            PSXTexture psxtexture = new PSXTexture();
            psxtexture.GenTexture(3, 0, 0, 0, 0, 1024, 512, FilterMode.Point, TextureWrapMode.Clamp);
            File.WriteAllBytes(GetDebugExportPath() + "vram.png", psxtexture.texture.EncodeToPNG());
        }
    }

    public static void LoadImageBin(Int32 x, Int32 y, Int32 w, Int32 h, BinaryReader data)
    {
        for (Int32 i = 0; i < h; i++)
        {
            Int32 num = (y + i) * 1024 + x;
            for (Int32 j = 0; j < w; j++)
            {
                UInt16 num2 = (UInt16)data.ReadByte();
                UInt16 num3 = (UInt16)data.ReadByte();
                PSXTextureMgr.originalVram[num] = (UInt16)((Int32)num3 << 8 | (Int32)num2);
                num++;
            }
        }
    }

    public unsafe static void StoreImage(Int32 x, Int32 y, Int32 w, Int32 h, UInt16* p)
    {
        for (Int32 i = 0; i < h; i++)
        {
            Int32 num = (y + i) * 1024 + x;
            for (Int32 j = 0; j < w; j++)
            {
                *(p++) = PSXTextureMgr.originalVram[num];
                num++;
            }
        }
    }

    public unsafe static void MoveImage(Int32 dx, Int32 dy, Int16* p)
    {
        //if (SFX.currentEffectID == SpecialEffect.Stop)
        //	return;
        if (SFX.currentEffectID == SpecialEffect.Devour)
        {
            PSXTextureMgr.isCaptureSS = true;
            return;
        }
        PSXTextureMgr.ClearKey(dx, dy);
        Int16 x = *p;
        Int16 y = p[1];
        Int16 w = p[2];
        Int16 h = p[3];
        for (Int32 i = 0; i < (Int32)h; i++)
        {
            Int32 num5 = (dy + i) * 1024 + dx;
            Int32 num6 = ((Int32)y + i) * 1024 + (Int32)x;
            for (Int32 j = 0; j < (Int32)w; j++)
            {
                PSXTextureMgr.originalVram[num5] = PSXTextureMgr.originalVram[num6];
                num5++;
                num6++;
            }
        }
    }

    public static PSXTexture GetTexture(Int32 TP, Int32 TY, Int32 TX, Int32 clutY, Int32 clutX)
    {
        if (TP == 2)
            clutY = (clutX = 0);
        UInt32 num = SFXKey.GenerateKey(TP, TX, TY, clutX, clutY);
        for (Int32 i = 0; i < PSXTextureMgr.SST_MAX_TEXTURE; i++)
            if (PSXTextureMgr.list[i].key == num && PSXTextureMgr.list[i].texture)
                return PSXTextureMgr.list[i];
        // Generate it from Vram if not present
        for (Int32 j = 0; j < PSXTextureMgr.SST_MAX_TEXTURE; j++)
        {
            if (PSXTextureMgr.list[j].key == PSXTexture.EMPTY_KEY)
            {
                PSXTextureMgr.list[j].key = num;
                PSXTextureMgr.list[j].GenTexture(TP, TY, TX, clutY, clutX, 256, 256, FilterMode.Point, TextureWrapMode.Clamp);
                if (SFX.isDebugPng)
                {
                    Byte[] bytes = PSXTextureMgr.list[j].texture.EncodeToPNG();
                    String path = GetDebugExportPath() + "texture" + num.ToString("X8") + ".png";
                    File.WriteAllBytes(path, bytes);
                }
                return PSXTextureMgr.list[j];
            }
        }
        global::Debug.Assert(false);
        return (PSXTexture)null;
    }

    public static PSXTexture GetTexture(UInt32 key)
    {
        for (Int32 i = 0; i < PSXTextureMgr.SST_MAX_TEXTURE; i++)
            if (PSXTextureMgr.list[i].key == key && PSXTextureMgr.list[i].texture)
                return PSXTextureMgr.list[i];
        // Generate it from Vram if not present
        for (Int32 j = 0; j < PSXTextureMgr.SST_MAX_TEXTURE; j++)
        {
            if (PSXTextureMgr.list[j].key == PSXTexture.EMPTY_KEY)
            {
                PSXTextureMgr.list[j].key = key;
                Int32 clutX = (Int32)(key & 0x3F);
                Int32 clutY = (Int32)(key >> 6 & 0x1FF);
                Int32 tx = (Int32)(key >> 16 & 0xF);
                Int32 ty = (Int32)(key >> 20 & 1);
                Int32 tp = (Int32)(key >> 21 & 3);
                PSXTextureMgr.list[j].GenTexture(tp, ty, tx, clutY, clutX, 256, 256, FilterMode.Point, TextureWrapMode.Clamp);
                if (SFX.isDebugPng)
                {
                    Byte[] bytes = PSXTextureMgr.list[j].texture.EncodeToPNG();
                    String path = GetDebugExportPath() + "texture" + key.ToString("X8") + ".png";
                    File.WriteAllBytes(path, bytes);
                }
                return PSXTextureMgr.list[j];
            }
        }
        global::Debug.Assert(false);
        return (PSXTexture)null;
    }

    public static Boolean HasTextureKey(UInt32 key)
    {
        for (Int32 i = 0; i < PSXTextureMgr.SST_MAX_TEXTURE; i++)
            if (PSXTextureMgr.list[i].key == key && PSXTextureMgr.list[i].texture)
                return true;
        return false;
    }

    public static void MoveImage(Int32 rx, Int32 ry, Int32 rw, Int32 rh, Int32 x, Int32 y)
    {
        for (Int32 i = 0; i < rh; i++)
        {
            Int32 num = (i + y << 10) + x;
            Int32 num2 = (i + ry << 10) + rx;
            for (Int32 j = 0; j < rw; j++)
            {
                PSXTextureMgr.originalVram[num] = PSXTextureMgr.originalVram[num2];
                num2++;
                num++;
            }
        }
        PSXTextureMgr.ClearKey(x, y);
    }

    public static void CaptureBG(Vector2 screen, Vector3 offset)
    {
        MeshRenderer[] btlChildren = FF9StateSystem.Battle.FF9Battle.map.btlBGPtr.GetComponentsInChildren<MeshRenderer>();
        MeshRenderer background = null;
        for (Int32 i = 0; i < btlChildren.Length; i++)
            if (battlebg.getBbgAttr(btlChildren[i].name) == battlebg.BBG_ATTR_GROUND)
            {
                background = btlChildren[i];
                break;
            }
        if (background == null)
        {
            GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
            return;
        }
        GL.Viewport(new Rect(0f, 0f, 256f, 256f));
        GL.LoadIdentity();
        GL.Clear(false, true, new Color(0.02734375f, 0.02734375f, 0.02734375f, 0f));
        Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
        camera.worldToCameraMatrix = Matrix4x4.TRS(offset, Quaternion.Euler(-90f, 0f, 0f), new Vector3(1f, 1f, 1f));
        Matrix4x4 mat = PsxCamera.PerspectiveOffCenter(screen.x, screen.x + 256f, screen.y - 256f, screen.y, 512f, 65536f);
        GL.LoadProjectionMatrix(mat);
        Mesh mesh = background.GetComponent<MeshFilter>().mesh;
        background.material.SetPass(0);
        Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
        SFX.ResetViewPort();
    }

    public static void CaptureBG()
    {
        if (PSXTextureMgr.isBgCapture)
        {
            PSXTextureMgr.isBgCapture = false;
            PSXTextureMgr.bgKey = (uint)(PSXTextureMgr.bgParam[1] >> 8 << 20 | PSXTextureMgr.bgParam[0] >> 6 << 16);
            Single num = (Single)(PSXTextureMgr.bgParam[0] % 64);
            Single num2 = (Single)(PSXTextureMgr.bgParam[1] % 256);
            Single num3 = (Single)(PSXTextureMgr.bgParam[2] >> 1);
            Single num4 = (Single)(PSXTextureMgr.bgParam[3] >> 1);
            Single num5 = 256f / (Single)PSXTextureMgr.bgParam[2];
            Single num6 = 256f / (Single)PSXTextureMgr.bgParam[3];
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = PSXTextureMgr.bgTexture;
            CaptureBG(new Vector2(-num3 - num, num4 + num2), new Vector3(-PSXTextureMgr.bgParam[4], PSXTextureMgr.bgParam[5], -(10496 + PSXTextureMgr.bgParam[6])));
            RenderTexture.active = active;
        }
    }

    public static void SpEff435()
    {
        PSXTextureMgr.eff435Tex = new Texture[9];
        PSXTextureMgr.eff435Key = new UInt32[9];
        if (FF9StateSystem.Battle.battleMapIndex != 938)
        {
            PSXTextureMgr.eff435Tex[0] = AssetManager.Load<Texture2D>(DataResources.PureDataDirectory + "SpecialEffects/ef435/Background0.png", false);
            PSXTextureMgr.eff435Key[0] = SFXKey.GenerateKey(1, 10, 0, 0, 224);
            PSXTextureMgr.eff435Tex[1] = AssetManager.Load<Texture2D>(DataResources.PureDataDirectory + "SpecialEffects/ef435/Background1.png", false);
            PSXTextureMgr.eff435Key[1] = SFXKey.GenerateKey(1, 12, 0, 0, 225);
            PSXTextureMgr.eff435Tex[2] = AssetManager.Load<Texture2D>("BattleMap/BattleModel/BattleMap_All/BBG_B172/image0", false);
            PSXTextureMgr.eff435Key[2] = SFXKey.GenerateKey(1, 14, 0, 0, 228);
        }
        else
        {
            GameObject btlBGPtr = FF9StateSystem.Battle.FF9Battle.map.btlBGPtr;
            MeshRenderer[] componentsInChildren = btlBGPtr.GetComponentsInChildren<MeshRenderer>();
            for (Int32 i = 0; i < componentsInChildren.Length; i++)
            {
                Material[] materials = componentsInChildren[i].materials;
                for (Int32 j = 0; j < materials.Length; j++)
                {
                    if (materials[j].name == "135_tex9 (Instance)")
                    {
                        PSXTextureMgr.eff435Tex[0] = materials[j].mainTexture;
                        PSXTextureMgr.eff435Key[0] = SFXKey.GenerateKey(1, 10, 0, 0, 224);
                    }
                    if (materials[j].name == "135_tex10 (Instance)")
                    {
                        PSXTextureMgr.eff435Tex[1] = materials[j].mainTexture;
                        PSXTextureMgr.eff435Key[1] = SFXKey.GenerateKey(1, 12, 0, 0, 225);
                    }
                    if (materials[j].name == "135_tex0_2 (Instance)")
                    {
                        PSXTextureMgr.eff435Tex[2] = materials[j].mainTexture;
                        PSXTextureMgr.eff435Key[2] = SFXKey.GenerateKey(1, 14, 0, 0, 228);
                    }
                }
            }
        }
        PSXTextureMgr.eff435Tex[3] = AssetManager.Load<Texture2D>("SpecialEffects/geo_mon_b3_199_02", false);
        PSXTextureMgr.eff435Key[3] = SFXKey.GenerateKey(1, 0, 0, 0, 0);
        PSXTextureMgr.eff435Tex[4] = AssetManager.Load<Texture2D>("SpecialEffects/geo_mon_b3_199_00", false);
        PSXTextureMgr.eff435Key[4] = SFXKey.GenerateKey(1, 1, 0, 0, 0);
        PSXTextureMgr.eff435Tex[5] = AssetManager.Load<Texture2D>("SpecialEffects/geo_mon_b3_199_01", false);
        PSXTextureMgr.eff435Key[5] = SFXKey.GenerateKey(1, 2, 0, 0, 0);
        PSXTextureMgr.eff435Tex[6] = PSXTextureMgr.eff435Tex[5];
        PSXTextureMgr.eff435Key[6] = SFXKey.GenerateKey(1, 3, 0, 0, 0);
        PSXTextureMgr.eff435Tex[7] = PSXTextureMgr.eff435Tex[4];
        PSXTextureMgr.eff435Key[7] = SFXKey.GenerateKey(1, 4, 0, 0, 0);
        PSXTextureMgr.eff435Tex[8] = PSXTextureMgr.eff435Tex[4];
        PSXTextureMgr.eff435Key[8] = SFXKey.GenerateKey(1, 5, 0, 0, 0);
    }

    public static void PostBlur(RenderTexture src, RenderTexture dest)
    {
        if (PSXTextureMgr.isCaptureSS || PSXTextureMgr.isCaptureBlur)
        {
            PSXTextureMgr.isCaptureSS = false;
            PSXTextureMgr.isCaptureBlur = false;
            Graphics.Blit(src, PSXTextureMgr.blurTexture);
        }
        Graphics.Blit(src, dest);
    }

    public static Texture GetTexture435(UInt32 key, Single[] yw)
    {
        for (Int32 i = 0; i < (Int32)PSXTextureMgr.eff435Key.Length; i++)
        {
            if (key == PSXTextureMgr.eff435Key[i])
            {
                switch (i)
                {
                    case 3:
                    case 4:
                    case 5:
                        yw[0] = -128f;
                        yw[1] = -256f;
                        break;
                    default:
                        yw[0] = -256f;
                        yw[1] = -256f;
                        break;
                }
                return PSXTextureMgr.eff435Tex[i];
            }
        }
        return (Texture)null;
    }

    public static Color32 _ConvertABGR16toABGR32(UInt16 psxPixel)
    {
        Color32 result;
        result.r = (Byte)((psxPixel & 0x1F) << 3);
        result.g = (Byte)((psxPixel & 0x3E0) >> 2);
        result.b = (Byte)((psxPixel & 0x7C00) >> 7);
        if ((psxPixel & 0x8000) != 0)
            result.a = Byte.MaxValue;
        else
            result.a = 0;
        return result;
    }

    public static void _SaveVRamImage(String filename)
    {
        Color32[] array = new Color32[524288]; // 1024*512
        for (Int32 i = 0; i < 524288; i++)
        {
            UInt16 psxPixel = PSXTextureMgr.originalVram[i];
            array[i] = PSXTextureMgr._ConvertABGR16toABGR32(psxPixel);
        }
        Texture2D texture2D = new Texture2D(1024, 512);
        texture2D.SetPixels32(array);
        texture2D.Apply();
        Byte[] bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes(filename + ".png", bytes);
        Byte[] array2 = new Byte[(Int32)PSXTextureMgr.originalVram.Length * 2];
        Buffer.BlockCopy(PSXTextureMgr.originalVram, 0, array2, 0, (Int32)array2.Length);
        File.WriteAllBytes(filename + ".bin", array2);
    }

    public static void LoadTCBInVram(Byte[] tcbBin)
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

    public static String GetDebugExportPath()
    {
        String path = "SpecialEffects/" + (SFX.currentEffectID == SpecialEffect.Special_No_Effect ? "Common" : "ef" + ((Int32)SFX.currentEffectID).ToString("D3"));
        Directory.CreateDirectory(path);
        return path + "/";
    }

    public const Int32 BLUR_SCALE = 4;

    private static Boolean isOnce;

    private static Int32 SST_MAX_TEXTURE = 50;

    public static UInt16[] originalVram;

    public static Color32[] pixels256x256;

    private static PSXTexture[] list;

    public static Int32 GEN_TEXTURE_X = 384;

    public static Int32 GEN_TEXTURE_Y = 256;

    public static Int32 GEN_TEXTURE_W = 512;

    public static Int32 GEN_TEXTURE_H = 256;

    public static Boolean isCreateGenTexture;

    public static RenderTexture genTexture;

    public static RenderTexture bgTexture;

    public static Int32[] bgParam;

    public static Boolean isBgCapture;

    public static UInt32 bgKey;

    public static Single ssOffsetX;

    public static Single ssOffsetY;

    public static Boolean isCaptureSS;

    public static RenderTexture blurTexture;

    public static Boolean isCaptureBlur;

    public static Texture[] eff435Tex;

    public static UInt32[] eff435Key;

    public enum Kind
    {
        NONE,
        IMAGE,
        BLUR,
        BACKGROUND,
        GENERATED,
        SCREENSHOT
    }
}
