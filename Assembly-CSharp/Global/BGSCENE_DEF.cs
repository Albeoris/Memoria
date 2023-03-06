using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using UnityEngine;
using Memoria.Prime;
using Memoria.Assets;
using Memoria.Prime.PsdFile;
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
    public const String MemoriaBGXExtension = ".bgx";

    private const Single UVBorderShift = 0.5f;
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
    public Boolean isMemoriaScene = false;

    private Int32 spriteCount;
    private Boolean useUpscaleFM;
    private Int32 initialCorrection;

    private String mapName;
    private String atlasPath;
    public String memoriaDirectory;

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

    public void ReadMemoriaBGS(String path)
    {
        this.isMemoriaScene = true;
        this.memoriaDirectory = Path.GetDirectoryName(path) + "/";
        Char[] operationSeparator = new Char[] { ':' };
        Char[] argumentSeparator = new Char[] { ',' };
        String[] moddedBackground = File.ReadAllLines(path);
        String folder = Path.GetDirectoryName(path);
        System.Object currentObj = null;
        Int32 lineCount = 0;
        foreach (String line in moddedBackground)
        {
            lineCount++;
            String trimmedLine = line.Trim();
            if (String.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#") || trimmedLine.StartsWith("//"))
                continue;
            String[] split = trimmedLine.Split(operationSeparator, 2);
            String operation = split[0].Trim();
            String[] args = split.Length > 1 ? split[1].Split(argumentSeparator) : new String[0];
            for (Int32 i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();
            Boolean processOk = true;
            if (operation == "OVERLAY")
            {
                PushGenericObject(currentObj);
                BGOVERLAY_DEF bgOverlay = new BGOVERLAY_DEF();
                bgOverlay.isMemoria = true;
                bgOverlay.memoriaSize = new Vector2(16f, 16f);
                bgOverlay.minX = -32768;
                bgOverlay.maxX = 32767;
                bgOverlay.minY = -32768;
                bgOverlay.maxY = 32767;
                bgOverlay.indnum = (Byte)this.overlayList.Count;
                bgOverlay.flags = BGOVERLAY_DEF.OVERLAY_FLAG.Active;
                currentObj = bgOverlay;
            }
            else if (operation == "ANIMATION")
            {
                PushGenericObject(currentObj);
                BGANIM_DEF bgAnim = new BGANIM_DEF();
                bgAnim.flags = BGANIM_DEF.ANIM_FLAG.SingleFrame;
                bgAnim.frameRate = 256;
                currentObj = bgAnim;
            }
            else if (operation == "CAMERA")
            {
                PushGenericObject(currentObj);
                BGCAM_DEF bgCamera = new BGCAM_DEF();
                bgCamera.r[0, 0] = 4096;
                bgCamera.r[1, 1] = 4096;
                bgCamera.r[2, 2] = 4096;
                currentObj = bgCamera;
            }
            else if (currentObj is BGOVERLAY_DEF)
			{
                processOk = this.ProcessMemoriaOverlay(currentObj as BGOVERLAY_DEF, operation, args);
            }
            else if (currentObj is BGANIM_DEF)
            {
                processOk = this.ProcessMemoriaAnimation(currentObj as BGANIM_DEF, operation, args);
            }
            else if (currentObj is BGCAM_DEF)
            {
                processOk = this.ProcessMemoriaCamera(currentObj as BGCAM_DEF, operation, args);
            }
            else
            {
                processOk = false;
            }
            if (!processOk)
                Log.Warning($"[{typeof(BGSCENE_DEF)}] Unable to parse the operation {operation} at line {lineCount} of '{path}'");
        }
        PushGenericObject(currentObj);
        this.overlayCount = (UInt16)this.overlayList.Count;
        this.animCount = (UInt16)this.animList.Count;
        this.lightCount = (UInt16)this.lightList.Count;
        this.cameraCount = (UInt16)this.cameraList.Count;
    }

    private void PushGenericObject(System.Object obj)
	{
        if (obj == null)
            return;
        if (obj is BGOVERLAY_DEF)
            this.overlayList.Add(obj as BGOVERLAY_DEF);
        else if (obj is BGANIM_DEF)
            this.animList.Add(obj as BGANIM_DEF);
        else if (obj is BGLIGHT_DEF)
            this.lightList.Add(obj as BGLIGHT_DEF);
        else if (obj is BGCAM_DEF)
            this.cameraList.Add(obj as BGCAM_DEF);
    }

    private Boolean ProcessMemoriaOverlay(BGOVERLAY_DEF bgOverlay, String operation, String[] arguments)
	{
        if (operation == "CameraId" && arguments.Length >= 1)
        {
            Byte.TryParse(arguments[0], out bgOverlay.camNdx);
        }
        else if (operation == "ViewportId" && arguments.Length >= 1)
        {
            Byte.TryParse(arguments[0], out bgOverlay.viewportNdx);
        }
        else if (operation == "Position" && arguments.Length >= 3)
        {
            Int16.TryParse(arguments[0], out bgOverlay.orgX);
            Int16.TryParse(arguments[1], out bgOverlay.orgY);
            UInt16.TryParse(arguments[2], out bgOverlay.orgZ);
            bgOverlay.curX = bgOverlay.orgX;
            bgOverlay.curY = bgOverlay.orgY;
            bgOverlay.curZ = bgOverlay.orgZ;
        }
        else if (operation == "Size" && arguments.Length >= 2)
        {
            Int16.TryParse(arguments[0], out Int16 width);
            Int16.TryParse(arguments[1], out Int16 height);
            bgOverlay.memoriaSize = new Vector2(width, height);
        }
        //else if (operation == "ScrollWithOffset" && arguments.Length >= 3)
        //{
        //    Int16.TryParse(arguments[0], out bgOverlay.dX);
        //    Int16.TryParse(arguments[1], out bgOverlay.dY);
        //    Boolean.TryParse(arguments[2], out Boolean isXOffset);
        //    bgOverlay.flags |= BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset;
        //}
        else if (operation == "Image" && arguments.Length >= 1)
        {
            bgOverlay.memoriaImage = AssetManager.LoadTextureGeneric(File.ReadAllBytes(this.memoriaDirectory + arguments[0]));
            if (bgOverlay.memoriaMaterial != null)
                bgOverlay.memoriaMaterial.mainTexture = bgOverlay.memoriaImage;
        }
        else if (operation == "Shader" && arguments.Length >= 1)
        {
            Shader shader = ShadersLoader.Find(arguments[0]);
            if (shader == null)
                shader = ShadersLoader.Find("PSX/FieldMap_Abr_0");
            bgOverlay.memoriaMaterial = new Material(shader);
            if (bgOverlay.memoriaImage != null)
                bgOverlay.memoriaMaterial.mainTexture = bgOverlay.memoriaImage;
        }
        else
        {
            return false;
        }
        return true;
    }

    private Boolean ProcessMemoriaAnimation(BGANIM_DEF bgAnim, String operation, String[] arguments)
    {
        if (operation == "CameraId" && arguments.Length >= 1)
        {
            Byte.TryParse(arguments[0], out bgAnim.camNdx);
        }
        else if (operation == "FrameRate" && arguments.Length >= 1)
        {
            Int16.TryParse(arguments[0], out bgAnim.frameRate);
        }
        else if (operation == "Loop")
        {
            bgAnim.flags |= BGANIM_DEF.ANIM_FLAG.Animate | BGANIM_DEF.ANIM_FLAG.Loop;
        }
        else if (operation == "Palindrome")
        {
            bgAnim.flags |= BGANIM_DEF.ANIM_FLAG.Palindrome;
        }
        else if (operation == "Overlays")
        {
            foreach (String s in arguments)
			{
                BGANIMFRAME_DEF frame = new BGANIMFRAME_DEF();
                Byte.TryParse(s, out frame.target);
                bgAnim.frameList.Add(frame);
            }
            bgAnim.frameCount = bgAnim.frameList.Count;
        }
        else
        {
            return false;
        }
        return true;
    }

    private Boolean ProcessMemoriaCamera(BGCAM_DEF bgCamera, String operation, String[] arguments)
    {
        if (operation == "ViewDistance" && arguments.Length >= 1)
        {
            UInt16.TryParse(arguments[0], out bgCamera.proj);
        }
        else if (operation == "CenterOffset" && arguments.Length >= 2)
        {
            Int16.TryParse(arguments[0], out bgCamera.centerOffset[0]);
            Int16.TryParse(arguments[1], out bgCamera.centerOffset[1]);
        }
        else if (operation == "Position" && arguments.Length >= 3)
        {
            Int32.TryParse(arguments[0], out bgCamera.t[0]);
            Int32.TryParse(arguments[1], out bgCamera.t[1]);
            Int32.TryParse(arguments[2], out bgCamera.t[2]);
        }
        else if (operation == "Range" && arguments.Length >= 2)
        {
            Int16.TryParse(arguments[0], out bgCamera.w);
            Int16.TryParse(arguments[1], out bgCamera.h);
        }
        else if (operation == "DepthOffset" && arguments.Length >= 1)
        {
            Int32.TryParse(arguments[0], out bgCamera.depthOffset);
        }
        else if (operation == "Viewport" && arguments.Length >= 4)
        {
            Int16.TryParse(arguments[0], out Int16 minX);
            Int16.TryParse(arguments[1], out Int16 maxX);
            Int16.TryParse(arguments[2], out bgCamera.vrpMinY);
            Int16.TryParse(arguments[3], out bgCamera.vrpMaxY);
            bgCamera.vrpMinX = minX;
            bgCamera.vrpMaxX = maxX;
        }
        else if (operation == "OrientationAngles" && arguments.Length >= 3)
        {
            Single.TryParse(arguments[0], out Single angleX);
            Single.TryParse(arguments[1], out Single angleY);
            Single.TryParse(arguments[2], out Single angleZ);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(angleX, angleY, angleZ), Vector3.one);
            for (Int32 i = 0; i < 3; i++)
                for (Int32 j = 0; j < 3; j++)
                    bgCamera.r[i, j] = (Int16)(4096 * rotationMatrix[i, j]);
        }
        else if (operation == "OrientationMatrix" && arguments.Length >= 9)
        {
            Int32 argIndex = 0;
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    Single.TryParse(arguments[argIndex++], out Single entry);
                    bgCamera.r[i, j] = (Int16)(entry * 4096);
                }
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    public void CreateMemoriaScene(Transform parent)
    {
        this.CreateScene_Background(parent);

        for (Int32 i = 0; i < this.overlayList.Count; i++)
        {
            BGOVERLAY_DEF bgOverlay = this.overlayList[i];
            this.CreateScene_OverlayGo(bgOverlay);
            if (bgOverlay.memoriaMaterial == null)
                bgOverlay.memoriaMaterial = new Material(ShadersLoader.Find("PSX/FieldMap_Abr_0"));
            GameObject meshGo = bgOverlay.transform.gameObject;
            List<Vector3> vertexList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<Int32> triangleList = new List<Int32>();
            vertexList.Add(new Vector3(0f, 0f, 0f));
            vertexList.Add(new Vector3(bgOverlay.memoriaSize[0], 0f, 0f));
            vertexList.Add(new Vector3(bgOverlay.memoriaSize[0], bgOverlay.memoriaSize[1], 0f));
            vertexList.Add(new Vector3(0f, bgOverlay.memoriaSize[1], 0f));
            uvList.Add(new Vector2(0f, 1f));
            uvList.Add(new Vector2(1f, 1f));
            uvList.Add(new Vector2(1f, 0f));
            uvList.Add(new Vector2(0f, 0f));
            triangleList.Add(2);
            triangleList.Add(1);
            triangleList.Add(0);
            triangleList.Add(3);
            triangleList.Add(2);
            triangleList.Add(0);
            Mesh mesh = new Mesh
            {
                vertices = vertexList.ToArray(),
                uv = uvList.ToArray(),
                triangles = triangleList.ToArray()
            };
            MeshRenderer meshRenderer = meshGo.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = meshGo.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshGo.name += $"_Depth({this.curZ + bgOverlay.curZ:D5})";
            meshGo.name += $"_[{bgOverlay.memoriaMaterial.shader.name}]";
            meshRenderer.material = bgOverlay.memoriaMaterial;
            bgOverlay.transform.gameObject.SetActive((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0);
        }

        this.CreateScene_CompleteAnimatedOverlayNames();
    }

    public void ExportMemoriaBGX(String bgxExportPath)
	{
        Boolean atlasIsReadable = true;
        try
		{
            Color firstPixel = this.atlas.GetPixel(0, 0);
        }
        catch (Exception err)
		{
            atlasIsReadable = false;
        }
        if (!atlasIsReadable)
            this.atlas = TextureHelper.CopyAsReadable(this.atlas);
        String folder = Path.GetDirectoryName(bgxExportPath);
        String fileName = Path.GetFileNameWithoutExtension(bgxExportPath);
        if (!folder.EndsWith("/"))
            folder += "/";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        String bgsStr = "";
        foreach (BGOVERLAY_DEF bgOverlay in this.overlayList)
        {
            bgsStr += $"OVERLAY\n";
            bgsStr += $"CameraId: {bgOverlay.camNdx}\n";
            bgsStr += $"ViewportId: {bgOverlay.viewportNdx}\n";
            if (bgOverlay.spriteList.Count > 0)
            {
                Int32 spriteMinX = Int32.MaxValue;
                Int32 spriteMinY = Int32.MaxValue;
                Int32 spriteMinZ = Int32.MaxValue;
                Int32 spriteMaxX = Int32.MinValue;
                Int32 spriteMaxY = Int32.MinValue;
                Int32 spriteMaxZ = Int32.MinValue;
                foreach (BGSPRITE_LOC_DEF bgSprite in bgOverlay.spriteList)
                {
                    spriteMinX = Math.Min(spriteMinX, bgSprite.offX);
                    spriteMaxX = Math.Max(spriteMaxX, bgSprite.offX + 16);
                    spriteMinY = Math.Min(spriteMinY, bgSprite.offY);
                    spriteMaxY = Math.Max(spriteMaxY, bgSprite.offY + 16);
                    spriteMinZ = Math.Min(spriteMinZ, bgSprite.depth);
                    spriteMaxZ = Math.Max(spriteMaxZ, bgSprite.depth);
                }
                Int32 textureWidth = (spriteMaxX - spriteMinX) / 16 * (Int32)this.SPRITE_W;
                Int32 textureHeight = (spriteMaxY - spriteMinY) / 16 * (Int32)this.SPRITE_H;
                Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
                Color32[] textureColor = texture.GetPixels32();
                for (Int32 i = 0; i < textureColor.Length; i++)
                    textureColor[i].a = 0;
                foreach (BGSPRITE_LOC_DEF bgSprite in bgOverlay.spriteList)
                {
                    Int32 textureX = (bgSprite.offX - spriteMinX) / 16 * (Int32)this.SPRITE_W;
                    Int32 textureY = (bgSprite.offY - spriteMinY) / 16 * (Int32)this.SPRITE_H;
                    for (Int32 x = 0; x < this.SPRITE_W; x++)
                    {
                        for (Int32 y = 0; y < this.SPRITE_H; y++)
                        {
                            Int32 index = textureX + x + (textureHeight - textureY - y - 1) * textureWidth;
                            textureColor[index] = this.atlas.GetPixel(bgSprite.atlasX + x, (Int32)this.ATLAS_H - (bgSprite.atlasY + y) - 1);
                        }
                    }
                }
                texture.SetPixels32(textureColor);
                texture.Apply();
                String textureName = $"{fileName}_{bgOverlay.indnum}.png";
                BGSPRITE_LOC_DEF bgFirstSprite = bgOverlay.spriteList[0];
                bgsStr += $"Position: {this.orgX + bgOverlay.orgX + spriteMinX}, {this.orgY + bgOverlay.orgY + spriteMinY}, {this.orgZ + bgOverlay.orgZ + spriteMinZ}\n";
                bgsStr += $"Size: {spriteMaxX - spriteMinX}, {spriteMaxY - spriteMinY}\n";
                if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset) != 0)
                    bgsStr += $"ScrollWithOffset: {bgOverlay.dX}, {bgOverlay.dY}\n";
                bgsStr += $"Image: {textureName}\n";
                bgsStr += $"Shader: PSX/FieldMap_Abr_{(bgFirstSprite.trans == 0 ? "None" : Math.Min(3, (Int32)bgFirstSprite.alpha).ToString())}\n";
                TextureHelper.WriteTextureToFile(texture, folder + textureName);
            }
            else if (this.isMemoriaScene)
            {
                String textureName = $"{fileName}_{bgOverlay.indnum}.png";
                bgsStr += $"Position: {this.orgX + bgOverlay.orgX}, {bgOverlay.orgY}, {bgOverlay.orgZ}\n";
                bgsStr += $"Size: {bgOverlay.memoriaSize.x}, {bgOverlay.memoriaSize.y}\n";
                if ((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset) != 0)
                    bgsStr += $"ScrollWithOffset: {bgOverlay.dX}, {bgOverlay.dY}\n";
                bgsStr += $"Image: {textureName}\n";
                bgsStr += $"Shader: {bgOverlay.memoriaMaterial.shader.name}\n";
                TextureHelper.WriteTextureToFile(bgOverlay.memoriaImage, folder + textureName);
            }
            else
			{
                bgsStr += $"Position: {this.orgX + bgOverlay.orgX}, {this.orgY + bgOverlay.orgY}, {this.orgZ + bgOverlay.orgZ}\n";
            }
            bgsStr += $"\n";
        }
        foreach (BGANIM_DEF bgAnim in this.animList)
        {
            bgsStr += $"ANIMATION\n";
            bgsStr += $"CameraId: {bgAnim.camNdx}\n";
            bgsStr += $"FrameRate: {bgAnim.frameRate}\n";
            bgsStr += $"Overlays: {String.Join(", ", bgAnim.frameList.Select(f => f.target.ToString()).ToArray())}\n";
            bgsStr += $"\n";
        }
        foreach (BGCAM_DEF bgCamera in this.cameraList)
        {
            bgsStr += $"CAMERA\n";
            bgsStr += $"ViewDistance: {bgCamera.proj}\n";
            bgsStr += $"CenterOffset: {bgCamera.centerOffset[0]}, {bgCamera.centerOffset[1]}\n";
            bgsStr += $"Position: {bgCamera.t[0]}, {bgCamera.t[1]}, {bgCamera.t[2]}\n";
            bgsStr += $"Range: {bgCamera.w}, {bgCamera.h}\n";
            bgsStr += $"DepthOffset: {bgCamera.depthOffset}\n";
            bgsStr += $"Viewport: {bgCamera.vrpMinX}, {bgCamera.vrpMaxX}, {bgCamera.vrpMinY}, {bgCamera.vrpMaxY}\n";
            Matrix4x4 matrixRT = bgCamera.GetMatrixRT();
            //Vector3 euler = BattleSPSSystem.QuaternionFromMatrix(matrixRT).eulerAngles;
            //bgsStr += $"OrientationAngles: {euler.x}, {euler.y}, {euler.z}\n";
            bgsStr += $"OrientationMatrix: {matrixRT[0, 0]}, {matrixRT[0, 1]}, {matrixRT[0, 2]}, {matrixRT[1, 0]}, {matrixRT[1, 1]}, {matrixRT[1, 2]}, {matrixRT[2, 0]}, {matrixRT[2, 1]}, {matrixRT[2, 2]}\n";
            bgsStr += $"\n";
        }
        File.WriteAllText(bgxExportPath, bgsStr);
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
        for (Int32 i = 0; i < this.cameraCount; i++)
        {
            BGCAM_DEF bgCamera = new BGCAM_DEF();
            bgCamera.ReadData(reader);
            this.cameraList.Add(bgCamera);
        }
    }

    private void ExtractLightData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.lightOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < this.lightCount; i++)
        {
            BGLIGHT_DEF bgLight = new BGLIGHT_DEF();
            bgLight.ReadData(reader);
            this.lightList.Add(bgLight);
        }
    }

    private void ExtractAnimationFrameData(BinaryReader reader)
    {
        for (Int32 i = 0; i < this.animCount; i++)
        {
            BGANIM_DEF bgAnim = this.animList[i];
            reader.BaseStream.Seek(bgAnim.offset, SeekOrigin.Begin);
            for (Int32 j = 0; j < bgAnim.frameCount; j++)
            {
                BGANIMFRAME_DEF bgFrame = new BGANIMFRAME_DEF();
                bgFrame.ReadData(reader);
                bgAnim.frameList.Add(bgFrame);
            }
        }
    }

    private void ExtractAnimationData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.animOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < this.animCount; i++)
        {
            BGANIM_DEF bgAnim = new BGANIM_DEF();
            bgAnim.ReadData(reader);
            this.animList.Add(bgAnim);
        }
    }

    private void ExtractSpriteData(BinaryReader reader)
    {
        this.spriteCount = this.overlayList.Sum(bgOverlay => bgOverlay.spriteCount);
        if (this.useUpscaleFM)
        {
            this.ATLAS_H = (UInt32)this.atlas.height;
            this.ATLAS_W = (UInt32)this.atlas.width;
        }
        Int32 countPerRow = this.atlas.width / (TileSize + 4);
        Int32 spriteIndex = 0;
        for (Int32 i = 0; i < this.overlayCount; i++)
        {
            BGOVERLAY_DEF bgOverlay = this.overlayList[i];
            reader.BaseStream.Seek(bgOverlay.prmOffset, SeekOrigin.Begin);
            for (Int32 j = 0; j < bgOverlay.spriteCount; j++)
            {
                BGSPRITE_LOC_DEF bgSprite = new BGSPRITE_LOC_DEF();
                bgSprite.ReadData_BGSPRITE_DEF(reader);
                bgOverlay.spriteList.Add(bgSprite);
            }
            reader.BaseStream.Seek(bgOverlay.locOffset, SeekOrigin.Begin);
            for (Int32 j = 0; j < bgOverlay.spriteCount; j++)
            {
                BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[j];
                bgSprite.ReadData_BGSPRITELOC_DEF(reader);
                if (this.useUpscaleFM)
                {
                    bgSprite.atlasX = (UInt16)(2 + spriteIndex % countPerRow * (TileSize + 4));
                    bgSprite.atlasY = (UInt16)(2 + spriteIndex / countPerRow * (TileSize + 4));
                    bgSprite.w = (ushort)TileSize;
                    bgSprite.h = (ushort)TileSize;
                    spriteIndex++;
                }
            }
        }
    }

    private void ExtractOverlayData(BinaryReader reader)
    {
        reader.BaseStream.Seek(this.overlayOffset, SeekOrigin.Begin);
        for (Int32 i = 0; i < this.overlayCount; i++)
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
            Int32 countPerRow = atlasWidth / (TileSize + 4);
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
                        spriteInfo.atlasX = (UInt16)(2 + spriteIndex % countPerRow * (TileSize + 4));
                        spriteInfo.atlasY = (UInt16)(2 + spriteIndex / countPerRow * (TileSize + 4));
                        spriteInfo.w = (UInt16)TileSize;
                        spriteInfo.h = (UInt16)TileSize;
                        spriteIndex++;
                    }
                }
            }
            for (Int32 i = startOvrIdx; i <= endOvrIdx; i++)
                sceneUS.overlayList[i] = this.overlayList[i];
        }
    }

    public void LoadResources(String path, String newName)
    {
        this.name = newName;
        String customBackgroundFilename = AssetManager.SearchAssetOnDisc(path + newName + MemoriaBGXExtension, true, false);
        if (!String.IsNullOrEmpty(customBackgroundFilename))
        {
            this.ReadMemoriaBGS(customBackgroundFilename);
            return;
		}

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
        try
        {
            this.mapName = newName;

            this.LoadResources(path, newName);
            if (this.isMemoriaScene)
            {
                this.CreateMemoriaScene(fieldMap.transform);
                return;
            }

            //FieldMapInfo.fieldmapExtraOffset.SetOffset(name, this.overlayList);
            //if (!this.useUpscaleFM)
            //{
            //    this.GenerateAtlasFromBinary();
            //}
            this.CreateMaterials();
            List<Int16> list = new List<Int16>
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
        catch (Exception err)
        {
            Log.Error(err);
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
        int factor = (int)tileSize / 16;

        UInt32 atlasX = padding, atlasY = padding;
        uint deltaX = this.SPRITE_W + 2 * padding;
        uint deltaY = this.SPRITE_H + 2 * padding;

        string pathDebugOverlay = Path.Combine(atlasPath, "debug");
        Directory.CreateDirectory(pathDebugOverlay);
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

            for (Int32 i = startLocaleOvrIdx; i <= endLocaleOvrIdx; i++)
            {
                BGOVERLAY_DEF bgOverlay = this.overlayList[i];
                Log.Message($"LocaleOverlay {i}, lang {symbol}, spriteCount {bgOverlay.spriteList.Count}");
                Texture2D overlaytexLocale = new Texture2D(bgOverlay.w * factor, bgOverlay.h * factor, TextureFormat.RGBA32, false);
                //Texture2D overlay = new Texture2D();
                this.CreateScene_OverlayGo(bgOverlay);
                for (Int32 j = 0; j < bgOverlay.spriteList.Count; j++)
                {
                    BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[j];
                    Single x1 = (atlasLocaleX - UVBorderShift) / atlasSide;
                    Single x2 = (atlasLocaleX - UVBorderShift + this.SPRITE_W) / atlasSide;
                    Single y1 = (atlasLocaleY + UVBorderShift + this.SPRITE_H) / atlasSide;
                    Single y2 = (atlasLocaleY + UVBorderShift) / atlasSide;
                    this.CreateScene_NonCombinedSprite(fieldMap, bgOverlay, bgSprite, x1, y1, x2, y2);

                    Color[] refSprite = localeReftexture.GetPixels((Int32)atlasLocaleX, (Int32)atlasLocaleY, (Int32)this.SPRITE_W, (Int32)this.SPRITE_H);
                    overlaytexLocale.SetPixels(bgSprite.offX * factor, overlaytexLocale.height - (bgSprite.offY + 16) * factor, (Int32)this.SPRITE_W, (Int32)this.SPRITE_H, refSprite);

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
                TextureHelper.WriteTextureToFile(overlaytexLocale, Path.Combine(pathDebugOverlay, $"overlay_{i}_{symbol.ToUpper()}.png"));
                bgOverlay.transform.gameObject.SetActive((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0);
            }
        }

        Log.Message($"Begin importOverlaysFromPsd, totalAtlases {totalAtlases}, tileSize {tileSize}, atlasSide {atlasSide}");
        int currentAtlas = 0;
        Texture2D reftexture = new Texture2D(atlasSide, atlasSide, TextureFormat.RGBA32, false);
        reftexture.LoadImage(File.ReadAllBytes(Path.Combine(atlasPath, $"atlas_{++currentAtlas}.png")));
        this.CreateMaterialsForOverlay(reftexture);

        for (Int32 i = 0; i < this.overlayList.Count; i++)
        {
            if (i >= startLocaleOvrIdx && i <= endLocaleOvrIdx)
                continue;

            BGOVERLAY_DEF bgOverlay = this.overlayList[i];
            Texture2D overlayTex = new Texture2D(bgOverlay.w * factor, bgOverlay.h * factor, TextureFormat.RGBA32, false);
            this.CreateScene_OverlayGo(bgOverlay);
            for (Int32 j = 0; j < bgOverlay.spriteList.Count; j++)
            {
                BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[j];
                Single x1 = (atlasX - UVBorderShift) / atlasSide;
                Single x2 = (atlasX - UVBorderShift + this.SPRITE_W) / atlasSide;
                Single y1 = (atlasY + UVBorderShift + this.SPRITE_H) / atlasSide;
                Single y2 = (atlasY + UVBorderShift) / atlasSide;
                this.CreateScene_NonCombinedSprite(fieldMap, bgOverlay, bgSprite, x1, y1, x2, y2);

                Color[] refSprite = reftexture.GetPixels((Int32)atlasX, (Int32)atlasY, (Int32)this.SPRITE_W, (Int32)this.SPRITE_H);
                overlayTex.SetPixels(bgSprite.offX * factor, overlayTex.height - (bgSprite.offY + 16) * factor, (Int32)this.SPRITE_W, (Int32)this.SPRITE_H, refSprite);

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
            TextureHelper.WriteTextureToFile(overlayTex, Path.Combine(pathDebugOverlay, $"overlay_{i}.png"));
            bgOverlay.transform.gameObject.SetActive((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0);
        }
    }

    private void handleOverlays(FieldMap fieldMap, Boolean UseUpscalFM, String path)
    {
        //Log.Message($"UseUpscalFM {UseUpscalFM}");
        for (Int32 i = 0; i < this.overlayList.Count; i++)
        {
            BGOVERLAY_DEF bgOverlay = this.overlayList[i];
            this.CreateScene_OverlayGo(bgOverlay);
            for (Int32 j = 0; j < bgOverlay.spriteList.Count; j++)
            {
                BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[j];
                Single atlasWidth = this.ATLAS_W;
                Single atlasHeight = this.ATLAS_H;
                Single x1, y1, x2, y2;
                if (UseUpscalFM)
                {
                    x1 = (bgSprite.atlasX - UVBorderShift) / atlasWidth;
                    y1 = (this.ATLAS_H - bgSprite.atlasY + UVBorderShift) / atlasHeight;
                    x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                    y2 = (this.ATLAS_H - (bgSprite.atlasY + this.SPRITE_H) + UVBorderShift) / atlasHeight;
                }
                else
                {
                    x1 = (bgSprite.atlasX + UVBorderShift) / atlasWidth;
                    y1 = (bgSprite.atlasY + UVBorderShift) / atlasHeight;
                    x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                    y2 = (bgSprite.atlasY + this.SPRITE_H - UVBorderShift) / atlasHeight;
                }
                this.CreateScene_NonCombinedSprite(fieldMap, bgOverlay, bgSprite, x1, y1, x2, y2);
            }
            bgOverlay.transform.gameObject.SetActive((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0);
        }
    }

    private void CreateScene(FieldMap fieldMap, Boolean UseUpscalFM, String path)
    {
        this.CreateScene_Background(fieldMap.transform);

        String externalPath = Path.Combine(Configuration.Import.Path, path);
        String psdPath = Path.Combine(externalPath, "test.psd");
        String atlasPath = Path.Combine(externalPath, "atlases\\atlas_1.png");
        String psdMetaPath = Path.Combine(externalPath, "psd.meta");

        if (Configuration.Export.Enabled && Configuration.Export.Field)
        {
            String newPath = Path.Combine(Configuration.Import.Path, path);
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
                this.importOverlaysFromPsd(fieldMap, UseUpscalFM, externalPath);
            else
                this.handleOverlays(fieldMap, UseUpscalFM, externalPath);
        }

        this.CreateScene_CompleteAnimatedOverlayNames();
    }

    private List<Layer> getOrderedLayerList(PsdFile psd, PsdInfo psdInfo)
    {
        String order = psdInfo.LayerOrderFromPsdSection;
        Boolean reverse = psdInfo.ReversedFromPsdSection == 1 ? true : false;
        if (!reverse)
            psd.Layers.Reverse();
        List<Layer> newPsdList = new List<Layer>();
        // rearrange layers
        if (order == "depth")
        {
            List<BGOVERLAY_DEF> myorder = new List<BGOVERLAY_DEF>();
            for (Int32 i = 0; i < this.overlayList.Count; i++)
                myorder.Push(this.overlayList[i]);
            myorder.Sort((x, y) => (x.curZ == y.curZ) ? x.indnum.CompareTo(y.indnum) : x.curZ.CompareTo(y.curZ));

            // sort overlaylist by depth
            Layer[] newLayerList = new Layer[this.overlayList.Count];
            for (Int32 i = 0; i < myorder.Count; i++)
                newLayerList[myorder[i].indnum] = psd.Layers[i];

            newPsdList.AddRange(newLayerList);
        }
        else
        {
            newPsdList = psd.Layers;
        }
        return newPsdList;
    }

    private Byte[] generateEmptyAtlasArray(Int32 atlasSide)
    {
        Int32 product = atlasSide * atlasSide * 4;
        Byte[] atlasArray = new Byte[product];
        for (Int32 i = 0; i < product; i++)
            atlasArray[i] = 0;
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

    public void CreateSeparateOverlay(FieldMap fieldMap, Boolean UseUpscalFM, Int32 overlayNdx)
    {
        BGOVERLAY_DEF bgOverlay = this.overlayList[overlayNdx];
        if (bgOverlay.isCreated && !bgOverlay.canCombine)
            return;
        bgOverlay.canCombine = false;
        Boolean noDepth = false;
        MeshFilter previousFilter = bgOverlay.transform.GetComponent<MeshFilter>();
        if (previousFilter != null)
            UnityEngine.Object.Destroy(previousFilter);
        MeshRenderer previousRenderer = bgOverlay.transform.GetComponent<MeshRenderer>();
        if (previousRenderer != null)
            UnityEngine.Object.Destroy(previousRenderer);
        for (Int32 i = 0; i < bgOverlay.spriteList.Count; i++)
        {
            BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[i];
            Single atlasWidth = this.ATLAS_W;
            Single atlasHeight = this.ATLAS_H;
            Single x1, y1, x2, y2;
            if (UseUpscalFM)
            {
                x1 = (bgSprite.atlasX - UVBorderShift) / atlasWidth;
                y1 = (this.ATLAS_H - bgSprite.atlasY + UVBorderShift) / atlasHeight;
                x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                y2 = (this.ATLAS_H - (bgSprite.atlasY + this.SPRITE_H) + UVBorderShift) / atlasHeight;
            }
            else
            {
                x1 = (bgSprite.atlasX + UVBorderShift) / atlasWidth;
                y1 = (bgSprite.atlasY + UVBorderShift) / atlasHeight;
                x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                y2 = (bgSprite.atlasY + this.SPRITE_H - UVBorderShift) / atlasHeight;
            }
            this.CreateScene_NonCombinedSprite(fieldMap, bgOverlay, bgSprite, x1, y1, x2, y2, noDepth);
        }
    }

    public void CreateSeparateSprites(FieldMap fieldMap, Boolean UseUpscalFM, Int32 overlayNdx, List<Int32> spriteIdx)
    {
        Boolean noDepth = false;
        BGOVERLAY_DEF bgOverlay = this.overlayList[overlayNdx];
        for (Int32 i = 0; i < spriteIdx.Count; i++)
        {
            BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[spriteIdx[i]];
            Single atlasWidth = this.ATLAS_W;
            Single atlasHeight = this.ATLAS_H;
            Single x1, y1, x2, y2;
            if (UseUpscalFM)
            {
                x1 = (bgSprite.atlasX - UVBorderShift) / atlasWidth;
                y1 = (this.ATLAS_H - bgSprite.atlasY + UVBorderShift) / atlasHeight;
                x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                y2 = (this.ATLAS_H - (bgSprite.atlasY + this.SPRITE_H) + UVBorderShift) / atlasHeight;
            }
            else
            {
                x1 = (bgSprite.atlasX + UVBorderShift) / atlasWidth;
                y1 = (bgSprite.atlasY + UVBorderShift) / atlasHeight;
                x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                y2 = (bgSprite.atlasY + this.SPRITE_H - UVBorderShift) / atlasHeight;
            }
            this.CreateScene_NonCombinedSprite(fieldMap, bgOverlay, bgSprite, x1, y1, x2, y2, noDepth);
        }
    }


    private void CreateSceneCombined(FieldMap fieldMap, Boolean UseUpscalFM)
    {
        Boolean noDepth = false;
        this.CreateScene_Background(fieldMap.transform, noDepth);
        FieldMap.EbgCombineMeshData currentCombineMeshData = fieldMap.GetCurrentCombineMeshData();
        List<Int32> overlaySkip = null;
        if (currentCombineMeshData != null)
            overlaySkip = currentCombineMeshData.skipOverlayList;
        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<Int32> triangleList = new List<Int32>();
        for (Int32 i = 0; i < this.overlayList.Count; i++)
        {
            BGOVERLAY_DEF bgOverlay = this.overlayList[i];
            this.CreateScene_OverlayGo(bgOverlay, noDepth);
            vertexList.Clear();
            uvList.Clear();
            triangleList.Clear();
            bgOverlay.canCombine = true;
            bgOverlay.isCreated = false;
            if ((bgOverlay.flags & (BGOVERLAY_DEF.OVERLAY_FLAG.Loop | BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset)) != 0)
            {
                bgOverlay.canCombine = false;
            }
            else if (bgOverlay.spriteList.Count > 1)
            {
                if (overlaySkip == null || !overlaySkip.Contains(i))
                {
                    Int32 maxDepthDelta = fieldMap.IsCurrentFieldMapHasCombineMeshProblem() ? 164 : 512;
                    Int32 maxDepth = 4096;
                    Int32 minDepth = -4096;
                    for (Int32 j = 0; j < bgOverlay.spriteList.Count; j++)
                    {
                        maxDepth = Mathf.Min(maxDepth, bgOverlay.spriteList[j].depth);
                        minDepth = Mathf.Max(minDepth, bgOverlay.spriteList[j].depth);
                        if (minDepth - maxDepth > maxDepthDelta)
                        {
                            bgOverlay.canCombine = false;
                            break;
                        }
                    }
                }
                else
                {
                    bgOverlay.canCombine = false;
                }
                if (FF9StateSystem.Common.FF9.fldMapNo == 552) // Lindblum/Main Street
                    bgOverlay.canCombine = i == 17 ? true : true;
            }
            if (!bgOverlay.canCombine)
            {
                this.CreateSeparateOverlay(fieldMap, UseUpscalFM, i);
                bgOverlay.transform.gameObject.SetActive((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0);
                bgOverlay.isCreated = true;
            }
            else
            {
                List<Int32> separateSprites = null;
                if (FF9StateSystem.Common.FF9.fldMapNo == 552 && i == 17) // Lindblum/Main Street
                    separateSprites = new List<Int32> { 202, 203, 214, 215 };
                for (Int32 j = 0; j < bgOverlay.spriteList.Count; j++)
                {
                    if (separateSprites == null || !separateSprites.Contains(j))
                    {
                        BGSPRITE_LOC_DEF bgSprite = bgOverlay.spriteList[j];
                        Vector3 spritePos = noDepth ?
                            new Vector3(bgOverlay.scrX + bgSprite.offX, bgOverlay.scrY + bgSprite.offY + 16, 0f) :
                            new Vector3(bgSprite.offX, bgSprite.offY + 16, bgSprite.depth);
                        Int32 vertexIndex = vertexList.Count;
                        vertexList.Add(new Vector3(0f, -16f, 0f) + spritePos);
                        vertexList.Add(new Vector3(16f, -16f, 0f) + spritePos);
                        vertexList.Add(new Vector3(16f, 0f, 0f) + spritePos);
                        vertexList.Add(new Vector3(0f, 0f, 0f) + spritePos);
                        Single atlasWidth = this.ATLAS_W;
                        Single atlasHeight = this.ATLAS_H;
                        Single x1, y1, x2, y2;
                        if (UseUpscalFM)
                        {
                            x1 = (bgSprite.atlasX - UVBorderShift) / atlasWidth;
                            y1 = (this.ATLAS_H - bgSprite.atlasY + UVBorderShift) / atlasHeight;
                            x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                            y2 = (this.ATLAS_H - (bgSprite.atlasY + this.SPRITE_H) + UVBorderShift) / atlasHeight;
                        }
                        else
                        {
                            x1 = (bgSprite.atlasX + UVBorderShift) / atlasWidth;
                            y1 = (bgSprite.atlasY + UVBorderShift) / atlasHeight;
                            x2 = (bgSprite.atlasX + this.SPRITE_W - UVBorderShift) / atlasWidth;
                            y2 = (bgSprite.atlasY + this.SPRITE_H - UVBorderShift) / atlasHeight;
                        }
                        uvList.Add(new Vector2(x1, y1));
                        uvList.Add(new Vector2(x2, y1));
                        uvList.Add(new Vector2(x2, y2));
                        uvList.Add(new Vector2(x1, y2));
                        triangleList.Add(vertexIndex + 2);
                        triangleList.Add(vertexIndex + 1);
                        triangleList.Add(vertexIndex);
                        triangleList.Add(vertexIndex + 3);
                        triangleList.Add(vertexIndex + 2);
                        triangleList.Add(vertexIndex);
                    }
                }

                if (bgOverlay.spriteList.Count > 0)
                    this.CreateScene_Mesh(fieldMap, bgOverlay, bgOverlay.spriteList[0], vertexList, uvList, triangleList, bgOverlay.transform.gameObject, false);

                bgOverlay.transform.gameObject.SetActive((bgOverlay.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0);
                if (separateSprites != null)
                    this.CreateSeparateSprites(fieldMap, this.useUpscaleFM, i, separateSprites);
                bgOverlay.isCreated = true;
            }
        }
        this.CreateScene_CompleteAnimatedOverlayNames();
    }

    private void CreateScene_Background(Transform parent, Boolean noDepth = false)
	{
        GameObject backgroundGo = new GameObject("Background");
        backgroundGo.transform.parent = parent;
        backgroundGo.transform.localPosition = new Vector3(this.curX - FieldMap.HalfFieldWidth, -(this.curY - FieldMap.HalfFieldHeight), noDepth ? 0f : this.curZ);
        backgroundGo.transform.localScale = new Vector3(1f, -1f, 1f);

        for (Int32 i = 0; i < this.cameraList.Count; i++)
        {
            BGCAM_DEF bgCamera = this.cameraList[i];
            GameObject cameraGo = new GameObject($"Camera_{i:D2} : {bgCamera.vrpMaxX + FieldMap.HalfFieldWidth} x {bgCamera.vrpMaxY + FieldMap.HalfFieldHeight}");
            cameraGo.transform.parent = backgroundGo.transform;
            bgCamera.transform = cameraGo.transform;
            bgCamera.transform.localPosition = Vector3.zero;
            bgCamera.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void CreateScene_CompleteAnimatedOverlayNames()
	{
        for (Int32 i = 0; i < this.animList.Count; i++)
        {
            BGANIM_DEF bgAnim = this.animList[i];
            for (Int32 j = 0; j < bgAnim.frameList.Count; j++)
            {
                GameObject overlayGo = this.overlayList[bgAnim.frameList[j].target].transform.gameObject;
                overlayGo.name += $"_[anim_{i:D2}]_[frame_{j:D2}_of_{bgAnim.frameList.Count:D2}]";
            }
        }
    }

    private void CreateScene_OverlayGo(BGOVERLAY_DEF bgOverlay, Boolean noDepth = false)
	{
        GameObject overlayGo = new GameObject($"Overlay_{bgOverlay.indnum:D2}");
        Transform overlayTransf = overlayGo.transform;
        overlayTransf.parent = this.cameraList[bgOverlay.camNdx].transform;
        overlayTransf.localPosition = new Vector3(bgOverlay.curX, bgOverlay.curY, noDepth ? 0f : bgOverlay.curZ);
        overlayTransf.localScale = new Vector3(1f, 1f, 1f);
        bgOverlay.transform = overlayTransf;
    }

    private void CreateScene_NonCombinedSprite(FieldMap fieldMap, BGOVERLAY_DEF bgOverlay, BGSPRITE_LOC_DEF bgSprite, Single x1, Single y1, Single x2, Single y2, Boolean noDepth = false)
    {
        Int32 spriteIndex = bgOverlay.spriteList.IndexOf(bgSprite);
        Int32 spriteDepth = noDepth ? 0 : bgSprite.depth;

        // TODO Check Native: #147
        if (FF9StateSystem.Common.FF9.fldMapNo == 2714) // Pand./Maze -> FBG_N42_PDMN_MAP734_PD_MZM_0
            if (spriteIndex == 64 || spriteIndex == 65 || spriteIndex == 66 || spriteIndex == 80 || spriteIndex == 81)
                spriteDepth = 400;

        GameObject spriteGo = new GameObject($"{bgOverlay.transform.name}_Sprite_{spriteIndex:D3}");
        spriteGo.transform.parent = bgOverlay.transform;
        if (noDepth)
            spriteGo.transform.localPosition = new Vector3(bgOverlay.scrX + bgSprite.offX, bgOverlay.scrY + bgSprite.offY + 16, 0f);
        else
            spriteGo.transform.localPosition = new Vector3(bgSprite.offX, bgSprite.offY + 16, spriteDepth);
        spriteGo.transform.localScale = new Vector3(1f, 1f, 1f);
        bgSprite.transform = spriteGo.transform;

        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<Int32> triangleList = new List<Int32>();
        vertexList.Add(new Vector3(0f, -16f, 0f));
        vertexList.Add(new Vector3(16f, -16f, 0f));
        vertexList.Add(new Vector3(16f, 0f, 0f));
        vertexList.Add(new Vector3(0f, 0f, 0f));
        uvList.Add(new Vector2(x1, y1));
        uvList.Add(new Vector2(x2, y1));
        uvList.Add(new Vector2(x2, y2));
        uvList.Add(new Vector2(x1, y2));
        triangleList.Add(2);
        triangleList.Add(1);
        triangleList.Add(0);
        triangleList.Add(3);
        triangleList.Add(2);
        triangleList.Add(0);

        this.CreateScene_Mesh(fieldMap, bgOverlay, bgSprite, vertexList, uvList, triangleList, spriteGo);
    }

    private void CreateScene_Mesh(FieldMap fieldMap, BGOVERLAY_DEF bgOverlay, BGSPRITE_LOC_DEF bgSprite, List<Vector3> vertexList, List<Vector2> uvList, List<Int32> triangleList, GameObject meshGo, Boolean nameWithDepth = true)
    {
        Mesh mesh = new Mesh
        {
            vertices = vertexList.ToArray(),
            uv = uvList.ToArray(),
            triangles = triangleList.ToArray()
        };
        MeshRenderer meshRenderer = meshGo.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshGo.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        if (nameWithDepth)
            meshGo.name += $"_Depth({this.curZ + bgOverlay.curZ + bgSprite.depth:D5})";
        String shaderName = bgSprite.trans != 0 ? $"abr_{Math.Min(3, (Int32)bgSprite.alpha)}" : "abr_none";
        if (fieldMap.mapName == "FBG_N39_UUVL_MAP671_UV_DEP_0" && bgOverlay.indnum == 14u) // Oeilvert/Star Display
            shaderName = "abr_none";
        meshGo.name += $"_[{shaderName}]";
        meshRenderer.material = this.materialList[shaderName];
    }
}