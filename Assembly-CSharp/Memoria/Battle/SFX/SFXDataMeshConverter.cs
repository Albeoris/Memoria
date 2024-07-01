using Memoria.Scripts;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

// This class is used for debugging purposes only for now
// It aims at exporting SFXMesh into a convenient format but mostly fails at doing so
public class SFXDataMeshConverter
{
    public Dictionary<Int32, Frame> frameMesh = new Dictionary<Int32, Frame>();
    public SFXDataMesh.ModelSequence.Sprite spriteMesh = new SFXDataMesh.ModelSequence.Sprite();
    public UInt32 key;
    public String texturePath;
    public Vector4 textureParam;
    public Color colorIntensity;
    public Single threshold;
    public String shaderName;
    public Byte snapshotOrientation; // 0: unknown camera movement, 1: absolute position, others: fixed camera along an axis (workflow data to interpolate absolute position)
    public Byte positionTypeUnit = 0; // 0: absolute, 1: on caster, 2: on target, 3: between caster and target (per vertex)
    public SByte positionTypeCasterBone = SByte.MinValue; // 0: root (bone000), positive: fixed bone, -1: none, -2: monbone0, -3: monbone1, -5: weapon
    public SByte positionTypeTargetBone = SByte.MinValue; // 0: root (bone000), positive: fixed bone, -1: none, -2: monbone0, -3: monbone1, -4: trgcpos (average position)
    public Vector3 centerOffset = default(Vector3); // offset compared to the position type
    public Byte offsetType = 0; // 0: no rotation, 1: flip if watching like player character (btl.rot closer to 180 than 0), 2: rotate according to btl.rot, 3: rotate according to bone
    public Boolean depthIsFixed = false;
    public Boolean worldCoord = false;

    public static void ExportPositionType(ICollection<SFXDataMeshConverter> meshList, String path)
    {
        String txt = "";
        foreach (SFXDataMeshConverter mesh in meshList)
            txt += $"Key={mesh.key:X8} ; AttachingUnit={AttachingUnitString(mesh.positionTypeUnit)} ; CasterAttachment={AttachmentBoneString(mesh.positionTypeCasterBone)} ; TargetAttachment={AttachmentBoneString(mesh.positionTypeTargetBone)}\n";
        File.WriteAllText(path, txt);
    }

    public static void ExportAsBinary(ICollection<SFXDataMeshConverter> meshList, String path)
    {
        JSONClass rootNode = new JSONClass();
        JSONArray rawArray = new JSONArray();
        rootNode.Add("FramedMesh", rawArray);
        foreach (SFXDataMeshConverter mesh in meshList)
        {
            JSONClass rawClass = new JSONClass();
            rawArray.Add(rawClass);
            JSONClass matClass = new JSONClass();
            rawClass.Add("Material", matClass);
            matClass.Add("Key", mesh.key.ToString("X8"));
            if (SFXKey.IsTexture(mesh.key))
            {
                matClass.Add("TextureMode", (SFXKey.GetTextureMode(mesh.key) + 1).ToString());
                matClass.Add("TexturePath", mesh.texturePath);
                matClass.Add("TextureParameter", ConvertToString(mesh.textureParam));
            }
            else
            {
                matClass.Add("TextureMode", 0.ToString());
            }
            matClass.Add("GlobalColor", ConvertToString(mesh.colorIntensity));
            matClass.Add("Threshold", mesh.threshold.ToString());
            matClass.Add("Shader", mesh.shaderName);
            JSONClass posClass = new JSONClass();
            rawClass.Add("Position", posClass);
            posClass.Add("AttachingUnit", AttachingUnitString(mesh.positionTypeUnit));
            if (mesh.positionTypeUnit == 1 || mesh.positionTypeUnit == 3)
                posClass.Add("CasterAttachment", AttachmentBoneString(mesh.positionTypeCasterBone));
            if (mesh.positionTypeUnit == 2 || mesh.positionTypeUnit == 3)
                posClass.Add("TargetAttachment", AttachmentBoneString(mesh.positionTypeTargetBone));
            if (mesh.centerOffset != default(Vector3))
                posClass.Add("OffsetVector", ConvertToString(mesh.centerOffset));
            if (mesh.offsetType != 0)
                posClass.Add("Orientation", OrientationString(mesh.offsetType));
            JSONArray frameArray = new JSONArray();
            rawClass.Add("Mesh", frameArray);
            foreach (KeyValuePair<Int32, Frame> p in mesh.frameMesh)
            {
                Frame frame = p.Value;
                JSONClass frameClass = new JSONClass();
                frameArray.Add(frameClass);
                frameClass.Add("Frame", p.Key.ToString());
                JSONArray vertArray = new JSONArray();
                frameClass.Add("Vertices", vertArray);
                for (Int32 i = 0; i < frame.vertex.Length; i++)
                    vertArray.Add(ConvertToString(frame.vertex[i]));
                JSONArray colorArray = new JSONArray();
                frameClass.Add("Colors", colorArray);
                for (Int32 i = 0; i < frame.color.Length; i++)
                    colorArray.Add(ConvertToString(frame.color[i]));
                if (frame.uv.Length > 0)
                {
                    JSONArray uvArray = new JSONArray();
                    frameClass.Add("UV", uvArray);
                    for (Int32 i = 0; i < frame.uv.Length; i++)
                        uvArray.Add(ConvertToString(frame.uv[i]));
                }
                JSONArray indexArray = new JSONArray();
                frameClass.Add("Indices", indexArray);
                for (Int32 i = 0; i < frame.index.Length; i++)
                    indexArray.Add(frame.index[i].ToString());
                //if (mesh.positionTypeUnit == 3)
                //{
                //	JSONArray ctArray = new JSONArray();
                //	frameClass.Add("CasterTargetCursor", ctArray);
                //	for (Int32 i = 0; i < frame.casterTargetCursor.Length; i++)
                //		ctArray.Add(frame.casterTargetCursor[i].ToString());
                //}
            }
        }
        rootNode.SaveToFile(path);
    }
    public static void ExportAsSFXModel(ICollection<SFXDataMeshConverter> meshList, String path)
    {
        Int32 indentCount = 1;
        String json = "{\n";
        String jsonFramed = "";
        String jsonSprite = "";
        Boolean hasFramed = false;
        Boolean hasSprite = false;
        foreach (SFXDataMeshConverter mesh in meshList)
        {
            if (mesh.frameMesh.Count > 0)
                hasFramed = true;
            if (mesh.spriteMesh.duration > 0)
                hasSprite = true;
        }
        if (hasSprite)
        {
            Int32 spriteCount = 0;
            foreach (SFXDataMeshConverter mesh in meshList)
                if (mesh.spriteMesh.duration > 0)
                    spriteCount++;
            jsonSprite += WriteIndent(indentCount) + "\"Sprite\":\n";
            jsonSprite += WriteIndent(indentCount++) + "[{\n";
            foreach (SFXDataMeshConverter mesh in meshList)
            {
                if (mesh.spriteMesh.duration == 0)
                    continue;
                spriteCount--;
                jsonSprite += WriteIndent(indentCount) + $"\"Duration\":\"{mesh.spriteMesh.duration}\",\n";
                jsonSprite += WriteIndent(indentCount) + $"\"ScreenSize\":\"{mesh.spriteMesh.useScreenSize}\",\n";
                jsonSprite += WriteIndent(indentCount) + "\"Material\":\n";
                jsonSprite += WriteIndent(indentCount++) + "{\n";
                jsonSprite += WriteIndent(indentCount) + $"\"Key\":\"{mesh.key:X8}\",\n";
                if (SFXKey.IsTexture(mesh.key))
                {
                    jsonSprite += WriteIndent(indentCount) + $"\"TextureMode\":\"{SFXKey.GetTextureMode(mesh.key) + 1}\",\n";
                    jsonSprite += WriteIndent(indentCount) + $"\"TexturePath\":\"{mesh.texturePath}\",\n";
                    jsonSprite += WriteIndent(indentCount) + $"\"TextureParameter\":\"{ConvertToString(mesh.textureParam)}\"\n";
                }
                else
                {
                    jsonSprite += WriteIndent(indentCount) + $"\"TextureMode\":\"{0}\",\n";
                }
                jsonSprite += WriteIndent(indentCount) + $"\"Shader\":\"{mesh.shaderName}\",\n";
                jsonSprite += WriteIndent(indentCount) + $"\"GlobalColor\":\"{ConvertToString(mesh.colorIntensity)}\",\n";
                jsonSprite += WriteIndent(indentCount) + $"\"Threshold\":\"{mesh.threshold}\",\n";
                jsonSprite += WriteIndent(--indentCount) + "},\n";
                jsonSprite += WriteIndent(indentCount) + "\"Position\":\n";
                jsonSprite += WriteIndent(indentCount++) + "{\n";
                jsonSprite += WriteIndent(indentCount) + $"\"AttachingUnit\":\"{AttachingUnitString(mesh.positionTypeUnit)}\",\n";
                if (mesh.positionTypeUnit == 1 || mesh.positionTypeUnit == 3)
                    jsonSprite += WriteIndent(indentCount) + $"\"CasterAttachment\":\"{AttachmentBoneString(mesh.positionTypeCasterBone)}\",\n";
                if (mesh.positionTypeUnit == 2 || mesh.positionTypeUnit == 3)
                    jsonSprite += WriteIndent(indentCount) + $"\"TargetAttachment\":\"{AttachmentBoneString(mesh.positionTypeTargetBone)}\",\n";
                if (mesh.centerOffset != default(Vector3))
                    jsonSprite += WriteIndent(indentCount) + $"\"OffsetVector\":\"{ConvertToString(mesh.centerOffset)}\",\n";
                jsonSprite += WriteIndent(indentCount) + $"\"Orientation\":\"{OrientationString(mesh.offsetType)}\"\n";
                jsonSprite += WriteIndent(--indentCount) + "},\n";
                jsonSprite += WriteIndent(indentCount) + "\"Vertices\":[";
                for (Int32 i = 0; i < mesh.spriteMesh.vertex.Length; i++)
                    jsonSprite += (i == 0 ? " " : ", ") + $"\"{ConvertToString(mesh.spriteMesh.vertex[i])}\"";
                jsonSprite += " ],\n";
                jsonSprite += WriteIndent(indentCount) + "\"Indices\":[";
                for (Int32 i = 0; i < mesh.spriteMesh.index.Length; i++)
                    jsonSprite += (i == 0 ? " " : ", ") + $"\"{mesh.spriteMesh.index[i]}\"";
                jsonSprite += " ],\n";
                jsonSprite += WriteIndent(indentCount) + "\"TextureAnimation\":\n";
                jsonSprite += WriteIndent(indentCount) + "[\n";
                Int32 tanimCount = mesh.spriteMesh.uv.Count;
                foreach (KeyValuePair<Int32, Vector2[]> tanim in mesh.spriteMesh.uv)
                {
                    tanimCount--;
                    jsonSprite += WriteIndent(indentCount + 1) + "{ \"Frame\":\"" + tanim.Key + "\", \"UV\":[";
                    for (Int32 i = 0; i < tanim.Value.Length; i++)
                        jsonSprite += (i == 0 ? " " : ", ") + $"\"{ConvertToString(tanim.Value[i])}\"";
                    jsonSprite += tanimCount > 0 ? " ]},\n" : " ]}\n";
                }
                jsonSprite += WriteIndent(indentCount) + "],\n";
                jsonSprite += WriteIndent(indentCount) + "\"ScaleAnimation\":\n";
                jsonSprite += WriteIndent(indentCount) + "[\n";
                Int32 sclAnimCount = mesh.spriteMesh.scaling.Count;
                foreach (KeyValuePair<Int32, Single> scl in mesh.spriteMesh.scaling)
                {
                    sclAnimCount--;
                    jsonSprite += WriteIndent(indentCount + 1) + "{ \"Frame\":\"" + scl.Key + "\", \"Scale\":\"" + scl.Value + "\"";
                    jsonSprite += sclAnimCount > 0 ? " },\n" : " }\n";
                }
                jsonSprite += WriteIndent(indentCount) + "],\n";
                jsonSprite += WriteIndent(indentCount) + "\"Emission\":\n";
                jsonSprite += WriteIndent(indentCount++) + "[{\n";
                Int32 temissionCount = mesh.spriteMesh.emission.Count;
                foreach (SFXDataMesh.ModelSequence.Sprite.Emission em in mesh.spriteMesh.emission)
                {
                    temissionCount--;
                    jsonSprite += WriteIndent(indentCount) + $"\"Frame\":\"{em.frame}\",\n";
                    jsonSprite += WriteIndent(indentCount) + $"\"Count\":\"{em.count}\",\n";
                    jsonSprite += WriteIndent(indentCount - 1) + (temissionCount > 0 ? "},\n" + WriteIndent(indentCount - 1) + "{\n" : "}]\n");
                }
                indentCount--;
            }
        }
        if (hasFramed)
        {
            Int32 meshCount = 0;
            foreach (SFXDataMeshConverter mesh in meshList)
                if (mesh.frameMesh.Count > 0)
                    meshCount++;
            jsonFramed += WriteIndent(indentCount) + "\"FramedMesh\":\n";
            jsonFramed += WriteIndent(indentCount++) + "[{\n";
            foreach (SFXDataMeshConverter mesh in meshList)
            {
                if (mesh.frameMesh.Count == 0)
                    continue;
                meshCount--;
                jsonFramed += WriteIndent(indentCount) + "\"Material\":\n";
                jsonFramed += WriteIndent(indentCount++) + "{\n";
                jsonFramed += WriteIndent(indentCount) + $"\"Key\":\"{mesh.key:X8}\",\n";
                if (SFXKey.IsTexture(mesh.key))
                {
                    jsonFramed += WriteIndent(indentCount) + $"\"TextureMode\":\"{SFXKey.GetTextureMode(mesh.key) + 1}\",\n";
                    jsonFramed += WriteIndent(indentCount) + $"\"TexturePath\":\"{mesh.texturePath}\",\n";
                    jsonFramed += WriteIndent(indentCount) + $"\"TextureParameter\":\"{ConvertToString(mesh.textureParam)}\"\n";
                }
                else
                {
                    jsonFramed += WriteIndent(indentCount) + $"\"TextureMode\":\"{0}\",\n";
                }
                jsonFramed += WriteIndent(indentCount) + $"\"Shader\":\"{mesh.shaderName}\",\n";
                jsonFramed += WriteIndent(indentCount) + $"\"GlobalColor\":\"{ConvertToString(mesh.colorIntensity)}\",\n";
                jsonFramed += WriteIndent(indentCount) + $"\"Threshold\":\"{mesh.threshold}\",\n";
                jsonFramed += WriteIndent(--indentCount) + "},\n";
                jsonFramed += WriteIndent(indentCount) + "\"Position\":\n";
                jsonFramed += WriteIndent(indentCount++) + "{\n";
                jsonFramed += WriteIndent(indentCount) + $"\"AttachingUnit\":\"{AttachingUnitString(mesh.positionTypeUnit)}\",\n";
                if (mesh.positionTypeUnit == 1 || mesh.positionTypeUnit == 3)
                    jsonFramed += WriteIndent(indentCount) + $"\"CasterAttachment\":\"{AttachmentBoneString(mesh.positionTypeCasterBone)}\",\n";
                if (mesh.positionTypeUnit == 2 || mesh.positionTypeUnit == 3)
                    jsonFramed += WriteIndent(indentCount) + $"\"TargetAttachment\":\"{AttachmentBoneString(mesh.positionTypeTargetBone)}\",\n";
                if (mesh.centerOffset != default(Vector3))
                    jsonFramed += WriteIndent(indentCount) + $"\"OffsetVector\":\"{ConvertToString(mesh.centerOffset)}\",\n";
                jsonFramed += WriteIndent(indentCount) + $"\"Orientation\":\"{OrientationString(mesh.offsetType)}\"\n";
                jsonFramed += WriteIndent(--indentCount) + "},\n";
                jsonFramed += WriteIndent(indentCount) + "\"Mesh\":\n";
                jsonFramed += WriteIndent(indentCount++) + "[{\n";
                Int32 frameCount = mesh.frameMesh.Count;
                foreach (KeyValuePair<Int32, Frame> p in mesh.frameMesh)
                {
                    frameCount--;
                    Frame frame = p.Value;
                    jsonFramed += WriteIndent(indentCount) + $"\"Frame\":\"{p.Key}\",\n";
                    jsonFramed += WriteIndent(indentCount++) + $"\"Vertices\":[";
                    for (Int32 i = 0; i < frame.vertex.Length; i++)
                        jsonFramed += (i % 10 == 0 ? "\n" + WriteIndent(indentCount) : " ") + $"\"{ConvertToString(frame.vertex[i])}\"" + (i + 1 == frame.vertex.Length ? "" : ",");
                    jsonFramed += "\n" + WriteIndent(--indentCount) + "],\n";
                    jsonFramed += WriteIndent(indentCount++) + $"\"Colors\":[";
                    for (Int32 i = 0; i < frame.color.Length; i++)
                        jsonFramed += (i % 10 == 0 ? "\n" + WriteIndent(indentCount) : " ") + $"\"{ConvertToString(frame.color[i])}\"" + (i + 1 == frame.color.Length ? "" : ",");
                    jsonFramed += "\n" + WriteIndent(--indentCount) + "],\n";
                    if (frame.uv.Length > 0)
                    {
                        jsonFramed += WriteIndent(indentCount++) + $"\"UV\":[";
                        for (Int32 i = 0; i < frame.uv.Length; i++)
                            jsonFramed += (i % 10 == 0 ? "\n" + WriteIndent(indentCount) : " ") + $"\"{ConvertToString(frame.uv[i])}\"" + (i + 1 == frame.uv.Length ? "" : ",");
                        jsonFramed += "\n" + WriteIndent(--indentCount) + "],\n";
                    }
                    jsonFramed += WriteIndent(indentCount++) + $"\"Indices\":[";
                    for (Int32 i = 0; i < frame.index.Length; i++)
                        jsonFramed += (i % 30 == 0 ? "\n" + WriteIndent(indentCount) : " ") + $"\"{frame.index[i]}\"" + (i + 1 == frame.index.Length ? "" : ",");
                    jsonFramed += "\n" + WriteIndent(--indentCount) + "]\n";
                    jsonFramed += WriteIndent(indentCount - 1) + (frameCount > 0 ? "},\n" + WriteIndent(indentCount - 1) + "{\n" : "}]\n");
                }
                indentCount--;
                jsonFramed += WriteIndent(indentCount - 1) + (meshCount > 0 ? "},\n" + WriteIndent(indentCount - 1) + "{\n" : "}]\n");
            }
        }
        json += jsonSprite;
        json += jsonFramed;
        json += "}\n";
        File.WriteAllText(path, json);
    }

    private static String WriteIndent(Int32 indentCnt)
    {
        String str = "";
        for (Int32 i = 0; i < indentCnt; i++)
            str += "\t";
        return str;
    }
    private static String ConvertToString(Vector2 v)
    {
        return "(" + v.x + ", " + v.y + ")";
    }
    private static String ConvertToString(Vector3 v)
    {
        return "(" + v.x + ", " + v.y + ", " + v.z + ")";
    }
    private static String ConvertToString(Vector4 v)
    {
        return "(" + v.x + ", " + v.y + ", " + v.z + ", " + v.w + ")";
    }
    private static String ConvertToString(Color c)
    {
        return "(" + c.r + ", " + c.g + ", " + c.b + ", " + c.a + ")";
    }
    private static String AttachingUnitString(Byte t)
    {
        return t == 0 ? "None"
             : t == 1 ? "Caster"
             : t == 2 ? "Target"
             : t == 3 ? "Between" : "Other";
    }
    private static String AttachmentBoneString(SByte t)
    {
        return t == 0 ? "Root"
             : t == -1 ? "Base"
             : t == -2 ? "Average"
             : t == -3 ? "TargetBone"
             : t == -4 ? "Weapon"
             : t == -5 ? "FirstBone"
             : t == -6 ? "SecondBone"
             : t == SByte.MinValue ? "None"
             : "Bone" + t.ToString("D3");
    }
    private static String OrientationString(Byte t)
    {
        return t == 0 ? "None"
             : t == 1 ? "FrontOrBack"
             : t == 2 ? "Unit"
             : t == 3 ? "Bone" : "Other";
    }

    public static void ExportAsObj(String path, IEnumerable<SFXDataMeshConverter> dataMesh, Int32 frame)
    {
        String obj = "mtllib " + Path.GetFileName(path) + ".mtl\n";
        List<String> mat = new List<String>();
        String pngPath;
        Int32 voff = 1, uvoff = 1;
        foreach (SFXDataMeshConverter data in dataMesh)
        {
            if (!data.frameMesh.ContainsKey(frame))
                continue;
            Frame f = data.frameMesh[frame];
            obj += f.ExportAsObj(voff, uvoff, data.key.ToString("X8"));
            voff += f.vertex.Length;
            uvoff += f.uv.Length;
            pngPath = "texture" + data.key.ToString("X8") + ".png";
            mat.Add("newmtl " + data.key.ToString("X8") + "\nKa 1 1 1\nKd 1 1 1\nKs 0 0 0\nd 1\nmap_Ka " + pngPath + "\nmap_Kd " + pngPath + "\nmap_Ks " + pngPath + "\n");
        }
        if (voff == 1)
            return;
        File.WriteAllText(Path.ChangeExtension(path, ".obj"), obj);
        File.WriteAllText(Path.ChangeExtension(path, ".mtl"), String.Join("\n\n", mat.ToArray()));
    }

    // Generating proper .sfxmesh was done in several steps:
    // 1) Export the geometry of solid meshes with different camera angles; merge them to add the missing vertices each time (since hidden faces are not generated by SFX)
    // 2) Export non-solid meshes; With a camera from above and distinguished positions, find which connected component are tied to caster/target and related bones (some of them)
    // 3) Same setting as 3) but to distinguish other bones; compute position informations connected component-wise
    public void UpdateWithNewCaption(SFXDataMeshConverter newCaption, Boolean updateGeometry, Boolean updatePosType1, Boolean updatePosType2)
    {
        if (updateGeometry)
        {
            foreach (KeyValuePair<Int32, Frame> p in frameMesh)
            {
                Frame oldf = p.Value;
                Frame newf;
                if (!newCaption.frameMesh.TryGetValue(p.Key, out newf))
                    continue;
                foreach (Frame f in new Frame[] { oldf, newf })
                {
                    if (f.parent.snapshotOrientation == 0x11)
                    {
                        for (Int32 vi = 0; vi < f.vertex.Length; vi++)
                            f.vertex[vi] = f.vertex[vi];
                        // z -= 2320
                    }
                }
            }
            snapshotOrientation = 0x10;
            return;
        }
        if (updatePosType1)
        {
            if (newCaption.positionTypeUnit == 0)
                return;
            positionTypeUnit = (Byte)(newCaption.positionTypeUnit + 0x10);
            positionTypeCasterBone = newCaption.positionTypeCasterBone;
            positionTypeTargetBone = newCaption.positionTypeTargetBone;
            return;
        }
        if (updatePosType2)
        {
            if (newCaption.positionTypeUnit != 0)
            {
                positionTypeUnit = newCaption.positionTypeUnit;
                if (newCaption.positionTypeCasterBone <= -4)
                    positionTypeCasterBone = newCaption.positionTypeCasterBone;
                if (newCaption.positionTypeTargetBone <= -4)
                    positionTypeTargetBone = newCaption.positionTypeTargetBone;
            }
            if (newCaption.centerOffset != default(Vector3))
                centerOffset = newCaption.centerOffset;
            if (newCaption.offsetType != 0)
                offsetType = newCaption.offsetType;
        }
    }

    public void TryFixDepth()
    {
        if (!depthIsFixed)
        {
            foreach (Frame frame in frameMesh.Values)
                frame.TryFixDepth();
            depthIsFixed = true;
        }
    }

    public void SwitchToWorldCoordinates()
    {
        if (!worldCoord)
        {
            foreach (Frame frame in frameMesh.Values)
                frame.SwitchToWorldCoordinates();
            worldCoord = true;
        }
    }

    public void ConvertToSprites()
    {
        Dictionary<Int32, List<List<Int32>>> ccs = new Dictionary<Int32, List<List<Int32>>>();
        Dictionary<Int32, List<Vector3>> spriteCenter = new Dictionary<Int32, List<Vector3>>();
        Dictionary<Int32, List<Single>> spriteDiameter = new Dictionary<Int32, List<Single>>();
        Dictionary<Int32, List<Int32>> spriteVList = new Dictionary<Int32, List<Int32>>();
        Boolean hasUV = SFXKey.IsTexture(key);
        Int32 firstParticleFrame = Int32.MaxValue;
        Int32 lastParticleFrame = Int32.MinValue;
        foreach (KeyValuePair<Int32, Frame> p in frameMesh)
        {
            Frame frame = p.Value;
            Boolean hasSprite = false;
            for (Int32 i = 0; i < frame.faceType.Length; i++)
                if (SFXMesh.IsFaceTypeParticle(frame.faceType[i]))
                    hasSprite = true;
            if (!hasSprite)
                continue;
            List<List<Int32>> fccs = frame.GetConnectedComponents();
            List<List<Int32>> spriteccs = new List<List<Int32>>();
            List<Int32> vlist = new List<Int32>();
            ccs[p.Key] = spriteccs;
            spriteCenter[p.Key] = new List<Vector3>();
            spriteDiameter[p.Key] = new List<Single>();
            spriteVList[p.Key] = vlist;
            foreach (List<Int32> cc in fccs)
            {
                if (!SFXMesh.IsFaceTypeParticle(frame.faceType[cc[0]]))
                    continue;
                spriteccs.Add(cc);
                Vector3 minpos = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
                Vector3 maxpos = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
                Single diameter2 = 0f;
                foreach (Int32 vId in cc)
                {
                    minpos.x = Math.Min(minpos.x, frame.vertex[vId].x);
                    minpos.y = Math.Min(minpos.y, frame.vertex[vId].y);
                    minpos.z = Math.Min(minpos.z, frame.vertex[vId].z);
                    maxpos.x = Math.Max(maxpos.x, frame.vertex[vId].x);
                    maxpos.y = Math.Max(maxpos.y, frame.vertex[vId].y);
                    maxpos.z = Math.Max(maxpos.z, frame.vertex[vId].z);
                    foreach (Int32 vId2 in cc)
                        diameter2 = Math.Max(diameter2, (frame.vertex[vId] - frame.vertex[vId2]).sqrMagnitude);
                    vlist.Add(vId);
                }
                spriteCenter[p.Key].Add((minpos + maxpos) / 2);
                spriteDiameter[p.Key].Add((Single)Math.Sqrt(diameter2));
            }
            firstParticleFrame = Math.Min(firstParticleFrame, p.Key);
            lastParticleFrame = Math.Min(lastParticleFrame, p.Key);
        }
        if (spriteVList.Count == 0)
            return;
        foreach (KeyValuePair<Int32, List<List<Int32>>> p in ccs)
        {
            Int32 count = p.Value.Count;
            Int32 lastCount = ccs.ContainsKey(p.Key - 1) ? ccs[p.Key - 1].Count : 0;
            if (spriteMesh.duration == 0 && count - lastCount < 0)
            {
                spriteMesh.duration = p.Key - firstParticleFrame;
            }
            else
            {
                Int32 expectedCount = 0;
                if (spriteMesh.duration != 0)
                    for (Int32 f = p.Key - 1; f >= p.Key - spriteMesh.duration; f--)
                        if (ccs.ContainsKey(f))
                            expectedCount += ccs[f].Count;
                Int32 extraParticle = count - expectedCount;
                if (extraParticle <= 0)
                    continue;
                SFXDataMesh.ModelSequence.Sprite.Emission em = new SFXDataMesh.ModelSequence.Sprite.Emission();
                spriteMesh.emission[p.Key] = em;
                em.count = extraParticle;
            }
        }
        if (spriteMesh.duration == 0)
            spriteMesh.duration = lastParticleFrame - firstParticleFrame + 1;
        // TODO: vertex uv index scaling (diameter)


        List<Int32> frameToDelete = new List<Int32>();
        foreach (KeyValuePair<Int32, List<Int32>> p in spriteVList)
        {
            Frame frame = frameMesh[p.Key];
            List<Vector3> newVert = new List<Vector3>();
            List<Vector2> newUV = new List<Vector2>();
            List<Color32> newColor = new List<Color32>();
            List<Single> newCTPos = new List<Single>();
            List<SFXMesh.FaceType> newFT = new List<SFXMesh.FaceType>();
            Int32[] newVIndex = new Int32[frame.vertex.Length];
            for (Int32 i = 0; i < frame.vertex.Length; i++)
                newVIndex[i] = i;
            for (Int32 i = 0; i < frame.vertex.Length; i++)
            {
                if (p.Value.Contains(i))
                {
                    newVIndex[i] = -1;
                    for (Int32 j = i + 1; j < frame.vertex.Length; j++)
                        newVIndex[j]--;
                }
                else
                {
                    newVert.Add(frame.vertex[i]);
                    if (hasUV)
                        newUV.Add(frame.uv[i]);
                    newColor.Add(frame.color[i]);
                    newCTPos.Add(frame.casterTargetCursor[i]);
                    newFT.Add(frame.faceType[i]);
                }
            }
            if (newVert.Count == 0)
            {
                frameToDelete.Add(p.Key);
            }
            else
            {
                frame.vertex = newVert.ToArray();
                frame.uv = newUV.ToArray();
                frame.color = newColor.ToArray();
                frame.casterTargetCursor = newCTPos.ToArray();
                frame.faceType = newFT.ToArray();
                List<Int32> newIndex = new List<Int32>();
                for (Int32 i = 0; i < frame.index.Length; i++)
                    if (newVIndex[frame.index[i]] >= 0)
                        newIndex.Add(newVIndex[frame.index[i]]);
                frame.index = newIndex.ToArray();
            }
        }
        foreach (Int32 f in frameToDelete)
            frameMesh.Remove(f);
    }

    public void ComputePositionInformations()
    {
        Byte newPosType = positionTypeUnit;
        SByte newCasterBone = positionTypeCasterBone;
        SByte newTargetBone = positionTypeTargetBone;
        BtlPositions btlCameraPos = new BtlPositions
        ( // btl_init was temporarily modified to put PC at (0, -30000, 0) and the single enemy to (0, 30000, 0); also SFX.Callback(Get Bone Stance) put x's positions to -30000/30000 for monbone0/1 or for weapon/average
            new Vector3(0f, -850f, 0f), // caster
            new Vector3(0f, 1000f, 0f), // target
            new Vector3(800f, -850f, 0f), // caster monbone0
            new Vector3(800f, 1000f, 0f), // target monbone0
            new Vector3(-600f, -850f, 0f), // caster monbone1
            new Vector3(-600f, 1000f, 0f), // target monbone1
            new Vector3(800f, -850f, 0f), // caster weapon
            new Vector3(-600f, 1000f, 0f) // target average
        );
        if (newPosType == 1 || newPosType == 2 || newPosType == 3)
        {
            foreach (SFXDataMeshConverter.Frame sfxFrame in frameMesh.Values)
            {
                Single newCasterTargetPos = newPosType == 1 ? 0f : 1f;
                Single[] newCasterTargetPosArr = null;
                if (newPosType == 3)
                    newCasterTargetPosArr = new Single[sfxFrame.vertex.Length];
                List<List<Int32>> connectedComponent = sfxFrame.GetConnectedComponents();
                foreach (List<Int32> cc in connectedComponent)
                {
                    Single meanY = 0f;
                    if (newPosType == 3)
                    {
                        Single factor = 0f;
                        foreach (Int32 cci in cc)
                        {
                            meanY = (sfxFrame.GetVertexPosition(cci, btlCameraPos).y + factor * meanY) / (factor + 1f);
                            factor += 1f;
                        }
                    }
                    foreach (Int32 cci in cc)
                    {
                        if (newPosType == 3)
                            newCasterTargetPos = (meanY - btlCameraPos.caster.y) / (btlCameraPos.target.y - btlCameraPos.caster.y);
                        Vector3 absolutePos = sfxFrame.vertex[cci];
                        Vector3 basePos = Frame.GetVertexPosition(Vector3.zero, btlCameraPos, newPosType, newCasterBone, newTargetBone, newCasterTargetPos);
                        sfxFrame.vertex[cci] = absolutePos - basePos;
                        if (newPosType == 3)
                            newCasterTargetPosArr[cci] = newCasterTargetPos;
                    }
                }
                sfxFrame.casterTargetCursor = newCasterTargetPosArr;
            }
        }
    }

    public class Frame
    {
        public SFXDataMeshConverter parent;
        public Vector3[] vertex;
        public Color32[] color;
        public Vector2[] uv;
        public Int32[] index;
        public Single[] casterTargetCursor;
        public SFXMesh.FaceType[] faceType;

        public Frame(SFXDataMeshConverter p)
        {
            parent = p;
        }

        public void ConvertFromMesh(SFXMesh mesh)
        {
            vertex = new Vector3[mesh.VbOffset];
            color = new Color32[mesh.VbOffset];
            uv = new Vector2[SFXKey.IsTexture(mesh._key) ? mesh.VbOffset : 0];
            index = new Int32[mesh.IbOffset];
            Array.Copy(mesh.VbPos, vertex, vertex.Length);
            Array.Copy(mesh.VbCol, color, color.Length);
            Array.Copy(mesh.VbTex, uv, uv.Length);
            Array.Copy(mesh.IbIndex, index, index.Length);
            parent.textureParam = mesh._constTexParam;
            parent.colorIntensity = SFXMesh.ColorData[SFX.colIntensity];
            parent.threshold = SFX.colThreshold != 0 ? 0.05f : 0.0295f;
            parent.shaderName = SFXMesh.shaderNames[mesh._shaderIndex];
            faceType = mesh.faceType.ToArray();
        }

        public void SwitchToWorldCoordinates(Boolean isScreenSize = false)
        {
            Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            for (Int32 i = 0; i < vertex.Length; i++)
            {
                Memoria.Prime.Log.Message("[Converter] Old " + vertex[i]);
                vertex[i].x *= (Single)camera.pixelWidth / (Single)FieldMap.PsxFieldWidth;
                vertex[i].y *= (Single)camera.pixelHeight / (Single)FieldMap.PsxFieldHeightNative;
                vertex[i].z *= -1;
                if (!isScreenSize)
                {
                    vertex[i].x *= vertex[i].z / 7000f;
                    vertex[i].y *= vertex[i].z / 7000f;
                }
                vertex[i] = camera.ScreenToWorldPoint(vertex[i]);
                Memoria.Prime.Log.Message("[Converter] New " + vertex[i]);
            }
        }

        public String ExportAsObj(Int32 indexOffset, Int32 uvOffset, String mtlName)
        {
            Boolean isTexture = SFXKey.IsTexture(parent.key);
            String obj = isTexture ? "usemtl " + mtlName + "\n" : "";
            Int32 i;
            obj += "o " + mtlName + "\n";
            for (i = 0; i < vertex.Length; i++)
                obj += "v " + vertex[i].x + " " + vertex[i].y + " " + vertex[i].z + "\n";
            if (isTexture)
                for (i = 0; i < uv.Length; i++)
                    obj += "vt " + (uv[i].x / 256f) + " " + (uv[i].y / 256f) + "\n";
            if (SFXKey.isLinePolygon(parent.key))
            {
                obj += "l";
                for (i = 0; i < index.Length; i++)
                    obj += " " + index[i] + 1;
                obj += "\n";
            }
            else
            {
                for (i = 0; i < index.Length; i += 3)
                    obj += "f " + (index[i] + indexOffset) + (isTexture ? "/" + (index[i] + uvOffset) : "")
                        + " " + (index[i + 1] + indexOffset) + (isTexture ? "/" + (index[i + 1] + uvOffset) : "")
                        + " " + (index[i + 2] + indexOffset) + (isTexture ? "/" + (index[i + 2] + uvOffset) : "") + "\n";
            }
            return obj;
        }

        public static Vector3 GetVertexPosition(Vector3 vert, BtlPositions btlPos, Byte type, SByte casterBone, SByte targetBone, Single casTarFactor = 0f)
        {
            if (type == 0)
                return vert;
            Vector3 basePosCaster = Vector3.zero;
            Vector3 basePosTarget = Vector3.zero;
            if (type == 1 || type == 3)
            {
                if (casterBone == -2)
                    basePosCaster = btlPos.caster_monbone0;
                else if (casterBone == -3)
                    basePosCaster = btlPos.caster_monbone1;
                else if (casterBone == -5)
                    basePosCaster = btlPos.caster_weapon;
                else
                    basePosCaster = btlPos.caster;
            }
            if (type == 2 || type == 3)
            {
                if (targetBone == -2)
                    basePosTarget = btlPos.target_monbone0;
                else if (targetBone == -3)
                    basePosTarget = btlPos.target_monbone1;
                else if (targetBone == -4)
                    basePosTarget = btlPos.target_average;
                else
                    basePosTarget = btlPos.target;
            }
            if (type == 1)
                return basePosCaster + vert;
            if (type == 2)
                return basePosTarget + vert;
            if (type == 3)
                return (1f - casTarFactor) * basePosCaster + casTarFactor * basePosTarget + vert;
            return vert;
        }

        public Vector3 GetVertexPosition(Int32 vId, BtlPositions btlPos)
        {
            if (parent.positionTypeUnit == 3)
                return Frame.GetVertexPosition(vertex[vId], btlPos, parent.positionTypeUnit, parent.positionTypeCasterBone, parent.positionTypeTargetBone, casterTargetCursor[vId]);
            return Frame.GetVertexPosition(vertex[vId], btlPos, parent.positionTypeUnit, parent.positionTypeCasterBone, parent.positionTypeTargetBone);
        }

        public List<List<Int32>> GetConnectedComponents()
        {
            Boolean useVector2 = parent.snapshotOrientation != 1;
            List<List<Int32>> samePos;
            List<List<Int32>> directConnections;
            ConnectionSetup(false, false, out samePos, out directConnections);
            HashSet<Int32> connectedComponentRemainings = new HashSet<Int32>();
            List<List<Int32>> connectedComponent = new List<List<Int32>>();
            for (Int32 i = 0; i < vertex.Length; i++)
                connectedComponentRemainings.Add(i);
            for (Int32 i = 0; i < vertex.Length; i++)
            {
                if (!connectedComponentRemainings.Contains(i))
                    continue;
                List<Int32> cc = new List<Int32>();
                Stack<Int32> ccNew = new Stack<Int32>();
                ccNew.Push(i);
                while (ccNew.Count > 0)
                {
                    Int32 ccNewVert = ccNew.Pop();
                    if (!connectedComponentRemainings.Contains(ccNewVert))
                        continue;
                    cc.Add(ccNewVert);
                    connectedComponentRemainings.Remove(ccNewVert);
                    foreach (Int32 dc in directConnections[i])
                        ccNew.Push(dc);
                    if (SFXKey.isLinePolygon(parent.key))
                    {
                        List<Int32> samePosToken = samePos[ccNewVert];
                        if (!useVector2 || samePosToken.Count == 2) // useVector2: Vertices are not merged, thus exactly two points have the same position inside the line
                            foreach (Int32 sp in samePosToken)
                                ccNew.Push(sp);
                    }
                    else
                    {
                        List<Int32> samePosNew = samePos[ccNewVert];
                        foreach (Int32 sp in samePosNew)
                        {
                            if (!connectedComponentRemainings.Contains(sp))
                                continue;
                            if (useVector2 && directConnections[sp].Count == 2)
                            {
                                List<Int32> spNsp0 = samePos[directConnections[sp][0]];
                                List<Int32> spNsp1 = samePos[directConnections[sp][1]];
                                if (spNsp0.Contains(directConnections[ccNewVert][0]) || spNsp0.Contains(directConnections[ccNewVert][1]) || spNsp1.Contains(directConnections[ccNewVert][0]) || spNsp1.Contains(directConnections[ccNewVert][1]))
                                    ccNew.Push(sp);
                            }
                            else if (!useVector2)
                            {
                                ccNew.Push(sp);
                            }
                        }
                    }
                }
                connectedComponent.Add(cc);
            }
            return connectedComponent;
        }

        public void TryFixDepth()
        {
            if (parent.snapshotOrientation == 1)
                return;
            List<List<Int32>> samePos;
            List<List<Int32>> directConnections;
            ConnectionSetup(false, true, out samePos, out directConnections);
            List<List<Int32>> sameVert = new List<List<Int32>>();
            Int32[] sameVertReverse = new Int32[vertex.Length];
            HashSet<Int32> remainingVertices = new HashSet<Int32>();
            for (Int32 i = 0; i < vertex.Length; i++)
                remainingVertices.Add(i);
            for (Int32 i = 0; i < vertex.Length; i++)
            {
                if (!remainingVertices.Contains(i))
                    continue;
                Int32 commonVertNewIndex = sameVert.Count;
                List<Int32> sameVerti = new List<Int32>();
                List<Int32> samePosVert = samePos[i];
                Stack<Int32> newSameVert = new Stack<Int32>();
                newSameVert.Push(i);
                while (newSameVert.Count > 0)
                {
                    Int32 nsv = newSameVert.Pop();
                    if (!remainingVertices.Contains(nsv))
                        continue;
                    sameVertReverse[nsv] = commonVertNewIndex;
                    sameVerti.Add(nsv);
                    remainingVertices.Remove(nsv);
                    if (SFXKey.isLinePolygon(parent.key))
                    {
                        if (samePosVert.Count == 2)
                        {
                            newSameVert.Push(samePosVert[0]);
                            newSameVert.Push(samePosVert[1]);
                        }
                    }
                    else
                    {
                        foreach (Int32 sp in samePosVert)
                        {
                            if (!remainingVertices.Contains(sp))
                                continue;
                            if (directConnections[nsv].FindIndex(nsdc => directConnections[sp].FindIndex(spdc => samePos[spdc].Contains(nsdc)) >= 0) >= 0)
                                newSameVert.Push(sp); // i and sp share the same edge (up to depth)
                        }
                    }
                }
                sameVert.Add(sameVerti);
            }
            Vector3[] newVertex = new Vector3[sameVert.Count];
            Color32[] newColor = new Color32[sameVert.Count];
            Vector2[] newUV = new Vector2[SFXKey.IsTexture(parent.key) ? sameVert.Count : 0];
            Single[] newCasterTargetCursor = null;
            if (parent.positionTypeUnit == 3)
                newCasterTargetCursor = new Single[sameVert.Count];
            SFXMesh.FaceType[] newFaceType = new SFXMesh.FaceType[sameVert.Count];
            for (Int32 i = 0; i < sameVert.Count; i++)
            {
                // Every triangle/segment has a unique depth, making them perpendicular to the sight ray
                // SFX_GetPrim returns geometry per depth
                // depth = multiple of 16 that minimizes [Formula]
                // [Formula] might be:
                //  sum(|z_i - depth|) => depth closest to the mid-point and between extremal points
                //  sum(|z_i - depth|²)
                //  |min(z_i) - depth|
                //  ...
                //  where z_i are the original depths of the triangle/segment
                // In any case, reversing the operation accuratly seems impossible (and is impossible from a single viewpoint)
                Single fixedDepth = 0f;
                for (Int32 j = 0; j < sameVert[i].Count; j++)
                    fixedDepth += vertex[sameVert[i][j]].z;
                fixedDepth /= sameVert[i].Count;
                newVertex[i] = new Vector3(vertex[sameVert[i][0]].x, vertex[sameVert[i][0]].y, fixedDepth /* / 32f */);
                newColor[i] = new Color32(color[sameVert[i][0]].r, color[sameVert[i][0]].g, color[sameVert[i][0]].b, color[sameVert[i][0]].a);
                if (SFXKey.IsTexture(parent.key))
                    newUV[i] = new Vector2(uv[sameVert[i][0]].x, uv[sameVert[i][0]].y);
                if (parent.positionTypeUnit == 3)
                    newCasterTargetCursor[i] = casterTargetCursor[sameVert[i][0]];
                newFaceType[i] = faceType[sameVert[i][0]];
            }
            vertex = newVertex;
            color = newColor;
            uv = newUV;
            if (parent.positionTypeUnit == 3)
                casterTargetCursor = newCasterTargetCursor;
            for (Int32 i = 0; i < index.Length; i++)
                index[i] = sameVertReverse[index[i]];
            faceType = newFaceType;
        }

        private void ConnectionSetup(Boolean checkUV, Boolean checkColor, out List<List<Int32>> samePos, out List<List<Int32>> directConnections)
        {
            const Single epsilon = 0.5f;
            Boolean useVector2 = parent.snapshotOrientation != 1;
            Boolean hasTexture = SFXKey.IsTexture(parent.key);
            samePos = new List<List<Int32>>();
            directConnections = new List<List<Int32>>();
            HashSet<Int32> remainingVertices = new HashSet<Int32>();
            for (Int32 i = 0; i < vertex.Length; i++)
            {
                remainingVertices.Add(i);
                samePos.Add(new List<Int32>());
            }
            for (Int32 i = 0; i < vertex.Length; i++)
            {
                if (remainingVertices.Contains(i))
                {
                    List<Int32> samePosi = samePos[i];
                    if (useVector2)
                    {
                        for (Int32 j = i + 1; j < vertex.Length; j++)
                        {
                            if (SFXMesh.IsFaceTypeParticle(faceType[i]) != SFXMesh.IsFaceTypeParticle(faceType[j]))
                                continue;
                            if (SFXMesh.IsFaceTypeLine(faceType[i]) != SFXMesh.IsFaceTypeLine(faceType[j]))
                                continue;
                            if (checkUV && hasTexture && uv[i] != uv[j]) // This check can't be useful if periodic (parts of) textures are unknown
                                continue;
                            if (checkColor && (color[i].r != color[j].r || color[i].g != color[j].g || color[i].b != color[j].b || color[i].a != color[j].a))
                                continue;
                            if (SFXMesh.IsFaceTypeParticle(faceType[i]) && vertex[i] != vertex[j])
                                continue;
                            if (useVector2 && (vertex[i].x - vertex[j].x) * (vertex[i].x - vertex[j].x) + (vertex[i].y - vertex[j].y) * (vertex[i].y - vertex[j].y) > epsilon)
                                continue;
                            if (!useVector2 && (vertex[i] - vertex[j]).sqrMagnitude > epsilon)
                                continue;
                            samePosi.Add(j);
                            remainingVertices.Remove(j);
                        }
                    }
                    foreach (Int32 j in samePosi)
                    {
                        samePos[j].Add(i);
                        foreach (Int32 jj in samePosi)
                            if (j != jj)
                                samePos[j].Add(jj);
                    }
                }
                directConnections.Add(new List<Int32>());
                for (Int32 j = 0; j < index.Length; j++)
                {
                    if (index[j] == i)
                    {
                        if (SFXKey.isLinePolygon(parent.key))
                        {
                            if (j % 2 != 0)
                                directConnections[i].Add(index[j - (j % 2)]);
                            else
                                directConnections[i].Add(index[j - (j % 2) + 1]);
                        }
                        else
                        {
                            if (j % 3 != 0)
                                directConnections[i].Add(index[j - (j % 3)]);
                            if (j % 3 != 1)
                                directConnections[i].Add(index[j - (j % 3) + 1]);
                            if (j % 3 != 2)
                                directConnections[i].Add(index[j - (j % 3) + 2]);
                        }
                    }
                }
            }
        }
    }

    public class BtlPositions
    {
        public Vector3 caster;
        public Vector3 target;
        public Vector3 caster_monbone0;
        public Vector3 target_monbone0;
        public Vector3 caster_monbone1;
        public Vector3 target_monbone1;
        public Vector3 caster_weapon;
        public Vector3 target_average;

        public BtlPositions(Vector3 c = default(Vector3), Vector3 t = default(Vector3),
                            Vector3 c0 = default(Vector3), Vector3 t0 = default(Vector3),
                            Vector3 c1 = default(Vector3), Vector3 t1 = default(Vector3),
                            Vector3 cw = default(Vector3), Vector3 ta = default(Vector3))
        {
            caster = c;
            target = t;
            caster_monbone0 = c0;
            target_monbone0 = t0;
            caster_monbone1 = c1;
            target_monbone1 = t1;
            caster_weapon = cw;
            target_average = ta;
        }
    }
}
