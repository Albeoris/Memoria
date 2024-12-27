using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Scripts;
using Memoria.Prime;

namespace Memoria.Assets
{
    public class ModelImporter
    {
        // This class can be used to import 3D models from external files
        // The features that are supported are in the following sub-classes
        private class CustomModelMinimalInfos
        {
            public String name;
            public Int32[] matIndex;
            public Vector3[][] vert;
            public Int32[][] tri;
            public String[] shader;
        }

        private class CustomModelTextured
        {
            public Vector2[][] uv;
            public String[] texturePath;
        }

        private class CustomModelAnimated
        {
            public BoneWeight[][] bw;
            public UInt32[] boneId;
            public Int32[] boneParentId;
            public Vector3[] bonePos;
            public Quaternion[] boneRot;
            public Vector3[] boneScale;
        }

        private class CustomModelExtraInfos
        {
            public String[] meshName = null;
            public Vector3[][] normal = null;
            public Vector4[][] tangent = null;
            public Color[][] color = null;
            public Vector2[][] uv2 = null;
        }

        public static GameObject CreateCustomModelFromFbx(String completePath)
        {
            try
            {
                FbxDocument fbx = FbxIO.ReadFlexible(completePath);
                if (fbx == null)
                    return null;
                String folderPath = Path.GetDirectoryName(completePath);
                List<FbxGeometry> geometries = fbx.GetGeometries();
                List<FbxMaterial> materials = fbx.GetMaterials();
                FbxSkeleton skeleton = fbx.GetSkeleton(geometries);
                CustomModelMinimalInfos baseMesh = new CustomModelMinimalInfos();
                CustomModelTextured texture = new CustomModelTextured();
                CustomModelAnimated anim = new CustomModelAnimated();
                CustomModelExtraInfos extra = new CustomModelExtraInfos();
                Boolean hasTexture = false;
                Boolean hasAnim = false;
                baseMesh.name = Path.GetFileNameWithoutExtension(completePath);
                baseMesh.matIndex = new Int32[geometries.Count];
                baseMesh.vert = new Vector3[geometries.Count][];
                baseMesh.tri = new Int32[geometries.Count][];
                texture.uv = new Vector2[geometries.Count][];
                anim.bw = new BoneWeight[geometries.Count][];
                extra.normal = new Vector3[geometries.Count][];
                extra.tangent = new Vector4[geometries.Count][];
                extra.color = new Color[geometries.Count][];
                extra.meshName = new String[geometries.Count];
                anim.boneId = new UInt32[skeleton.Bones.Count];
                anim.boneParentId = new Int32[skeleton.Bones.Count];
                anim.bonePos = new Vector3[skeleton.Bones.Count];
                anim.boneRot = new Quaternion[skeleton.Bones.Count];
                anim.boneScale = new Vector3[skeleton.Bones.Count];
                baseMesh.shader = new String[materials.Count];
                texture.texturePath = new String[materials.Count];
                for (Int32 i = 0; i < geometries.Count; i++)
                {
                    FbxGeometry geo = geometries[i];
                    baseMesh.vert[i] = geo.GetVertices();
                    baseMesh.tri[i] = geo.GetTriangleIndices();
                    baseMesh.matIndex[i] = fbx.GetMaterialIndex(geo, materials);
                    extra.meshName[i] = geo.Name;
                    extra.normal[i] = geo.GetNormals(baseMesh.vert[i], baseMesh.tri[i]);
                    extra.tangent[i] = geo.GetTangents(baseMesh.vert[i], baseMesh.tri[i]);
                    extra.color[i] = geo.GetColors(baseMesh.vert[i], baseMesh.tri[i]);
                    texture.uv[i] = geo.GetUVs(baseMesh.vert[i], baseMesh.tri[i]);
                    if (!hasTexture && texture.uv[i] != null)
                        hasTexture = true;
                    anim.bw[i] = geo.GetBoneWeights(baseMesh.vert[i], skeleton);
                    if (!hasAnim && anim.bw[i] != null)
                        hasAnim = true;
                }
                for (Int32 i = 0; i < skeleton.Bones.Count; i++)
                {
                    FbxBone bone = skeleton.Bones[i];
                    anim.boneId[i] = bone.Id;
                    anim.boneParentId[i] = (Int32?)bone.Parent?.Id ?? -1;
                    anim.bonePos[i] = bone.Position;
                    anim.boneRot[i] = bone.Rotation;
                    anim.boneScale[i] = bone.Scale;
                }
                for (Int32 i = 0; i < materials.Count; i++)
                {
                    baseMesh.shader[i] = materials[i].Shader;
                    if (materials[i].TexturePath != null)
                        texture.texturePath[i] = AssetManager.UsePathWithDefaultFolder(folderPath, materials[i].TexturePath);
                }
                if (!hasTexture)
                    texture = null;
                if (!hasAnim)
                    anim = null;
                GameObject go = CreateCustomModel(baseMesh, texture, anim, extra);
                go.AddComponent<Animation>();
                return go;
            }
            catch (Exception err)
            {
                Log.Error($"When importing FBX '{completePath}':\n{err}");
            }
            return null;
        }

        public static GameObject CreateCustomModelFromRawText(String completePath)
        {
            try
            {
                // The format there is just used for tests and will change in the future
                String folderPath = Path.GetDirectoryName(completePath);
                CustomModelMinimalInfos baseMesh = new CustomModelMinimalInfos();
                CustomModelTextured texture = null;
                CustomModelAnimated anim = null;
                CustomModelExtraInfos extra = null;
                Boolean hasTexture = true;
                Boolean hasAnim = true;
                Boolean hasMeshName = false;
                Boolean hasNormal = true;
                Boolean hasTangent = false;
                Boolean hasColor = false;
                Boolean hasUV2 = false;
                Int32 meshCount = 1;
                Int32 materialCount = 1;
                Int32 boneCount = 0;

                String[] customMeshFile = File.ReadAllLines(completePath);
                Int32 meshFilei = 9;

                baseMesh.name = Path.GetFileNameWithoutExtension(completePath);
                baseMesh.matIndex = new Int32[meshCount];
                baseMesh.vert = new Vector3[meshCount][];
                baseMesh.tri = new Int32[meshCount][];
                baseMesh.shader = new String[materialCount];
                if (hasTexture)
                {
                    texture = new CustomModelTextured();
                    texture.uv = new Vector2[meshCount][];
                    texture.texturePath = new String[materialCount];
                }
                if (hasAnim)
                {
                    anim = new CustomModelAnimated();
                    anim.bw = new BoneWeight[meshCount][];
                    anim.boneId = new UInt32[boneCount];
                    anim.boneParentId = new Int32[boneCount];
                    anim.bonePos = new Vector3[boneCount];
                    anim.boneRot = new Quaternion[boneCount];
                    anim.boneScale = new Vector3[boneCount];
                }
                if (hasMeshName || hasNormal || hasTangent || hasColor || hasUV2)
                {
                    extra = new CustomModelExtraInfos();
                    if (hasMeshName)
                        extra.meshName = new String[meshCount];
                    if (hasNormal)
                        extra.normal = new Vector3[meshCount][];
                    if (hasTangent)
                        extra.tangent = new Vector4[meshCount][];
                    if (hasColor)
                        extra.color = new Color[meshCount][];
                    if (hasUV2)
                        extra.uv2 = new Vector2[meshCount][];
                }
                for (Int32 i = 0; i < meshCount; i++)
                {
                    if (hasMeshName)
                        extra.meshName[i] = customMeshFile[0];
                    Int32.TryParse(customMeshFile[0], out Int32 vCount);
                    Int32.TryParse(customMeshFile[6], out Int32 tCount);

                    baseMesh.matIndex[i] = 0;
                    baseMesh.vert[i] = new Vector3[vCount];
                    baseMesh.tri[i] = new Int32[tCount];
                    for (Int32 j = 0; j < vCount; j++)
                    {
                        String[] line = customMeshFile[meshFilei++].Split(' ');
                        Single.TryParse(line[0], out baseMesh.vert[i][j].x);
                        Single.TryParse(line[1], out baseMesh.vert[i][j].y);
                        Single.TryParse(line[2], out baseMesh.vert[i][j].z);
                        baseMesh.vert[i][j] *= 75f;
                    }
                    if (hasTexture)
                    {
                        texture.uv[i] = new Vector2[vCount];
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            Single.TryParse(line[0], out texture.uv[i][j].x);
                            Single.TryParse(line[1], out texture.uv[i][j].y);
                        }
                    }
                    if (hasUV2)
                    {
                        extra.uv2[i] = new Vector2[vCount];
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            Single.TryParse(line[0], out extra.uv2[i][j].x);
                            Single.TryParse(line[1], out extra.uv2[i][j].y);
                        }
                    }
                    if (hasNormal)
                    {
                        extra.normal[i] = new Vector3[vCount];
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            Single.TryParse(line[0], out extra.normal[i][j].x);
                            Single.TryParse(line[1], out extra.normal[i][j].y);
                            Single.TryParse(line[2], out extra.normal[i][j].z);
                        }
                    }
                    if (hasTangent)
                    {
                        extra.tangent[i] = new Vector4[vCount];
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            Single.TryParse(line[0], out extra.tangent[i][j].x);
                            Single.TryParse(line[1], out extra.tangent[i][j].y);
                            Single.TryParse(line[2], out extra.tangent[i][j].z);
                            Single.TryParse(line[3], out extra.tangent[i][j].w);
                        }
                    }
                    if (hasColor)
                    {
                        extra.color[i] = new Color[vCount];
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            Single.TryParse(line[0], out extra.color[i][j].r);
                            Single.TryParse(line[1], out extra.color[i][j].g);
                            Single.TryParse(line[2], out extra.color[i][j].b);
                            Single.TryParse(line[3], out extra.color[i][j].a);
                        }
                    }
                    for (Int32 j = 0; j < tCount; j++)
                        Int32.TryParse(customMeshFile[meshFilei++], out baseMesh.tri[i][j]);
                    if (hasAnim)
                    {
                        anim.bw[i] = new BoneWeight[vCount];
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            anim.bw[i][j].boneIndex0 = Int32.Parse(line[0]);
                            anim.bw[i][j].boneIndex1 = Int32.Parse(line[1]);
                            anim.bw[i][j].boneIndex2 = Int32.Parse(line[2]);
                            anim.bw[i][j].boneIndex3 = Int32.Parse(line[3]);
                        }
                        for (Int32 j = 0; j < vCount; j++)
                        {
                            String[] line = customMeshFile[meshFilei++].Split(' ');
                            anim.bw[i][j].weight0 = Single.Parse(line[0]);
                            anim.bw[i][j].weight1 = Single.Parse(line[1]);
                            anim.bw[i][j].weight2 = Single.Parse(line[2]);
                            anim.bw[i][j].weight3 = Single.Parse(line[3]);
                        }
                    }
                }

                for (Int32 i = 0; i < boneCount; i++)
                {
                    anim.boneId[i] = (UInt32)i;
                    anim.boneParentId[i] = i - 1;
                    anim.bonePos[i] = Vector3.zero;
                    anim.boneRot[i] = Quaternion.identity;
                    anim.boneScale[i] = Vector3.one;
                }

                for (Int32 i = 0; i < materialCount; i++)
                {
                    baseMesh.shader[i] = "Default";
                    if (hasTexture)
                        texture.texturePath[i] = AssetManager.UsePathWithDefaultFolder(folderPath, "mesh0_1.png");
                }

                return CreateCustomModel(baseMesh, texture, anim, extra);
            }
            catch (Exception err)
            {
                Log.Error($"When importing custom model '{completePath}':\n{err}");
                return null;
            }
        }

        private static GameObject CreateCustomModel(CustomModelMinimalInfos baseMesh, CustomModelTextured texture = null, CustomModelAnimated anim = null, CustomModelExtraInfos extra = null)
        {
            GameObject baseObject = new GameObject(baseMesh.name);
            Int32 meshCount = baseMesh.vert.Length;
            Int32 materialCount = baseMesh.shader.Length;
            Material[] materials = new Material[materialCount];
            Transform[] bones = null;
            Matrix4x4[] bindPoses = null;
            if (anim != null)
            {
                Int32 boneCount = anim.boneId.Length;
                baseObject.AddComponent<Animation>();
                bones = new Transform[boneCount];
                bindPoses = new Matrix4x4[boneCount];
                for (Int32 i = 0; i < boneCount; i++)
                {
                    bones[i] = new GameObject($"bone{anim.boneId[i]:D3}").transform;
                    bones[i].localPosition = anim.bonePos[i];
                    bones[i].localRotation = anim.boneRot[i];
                    bones[i].localScale = anim.boneScale[i];
                    bindPoses[i] = bones[i].worldToLocalMatrix * baseObject.transform.localToWorldMatrix;
                }
                for (Int32 i = 0; i < boneCount; i++)
                {
                    if (anim.boneParentId[i] < 0)
                    {
                        bones[i].parent = baseObject.transform;
                        continue;
                    }
                    for (Int32 j = 0; j < boneCount; j++)
                    {
                        if (anim.boneParentId[i] == anim.boneId[j])
                        {
                            bones[i].parent = bones[j];
                            break;
                        }
                    }
                    if (bones[i].parent == null)
                        throw new IndexOutOfRangeException($"Model skeleton: bone{anim.boneId[i]:D3} has invalid parent bone ({anim.boneParentId[i]:D3})");
                }
            }
            for (Int32 i = 0; i < materialCount; i++)
            {
                materials[i] = ShadersLoader.CreateShaderMaterial(GetShaderPathFromType(baseMesh.shader[i], PersistenSingleton<EventEngine>.Instance.gMode));
                if (materials[i] == null)
                    throw new FileNotFoundException($"Unknown shader {baseMesh.shader[i]}");
                if (!String.IsNullOrEmpty(texture?.texturePath?[i]))
                    materials[i].mainTexture = AssetManager.LoadFromDisc<Texture2D>(texture.texturePath[i]);
            }
            for (Int32 i = 0; i < meshCount; i++)
            {
                String meshName = !String.IsNullOrEmpty(extra?.meshName?[i]) ? extra.meshName[i] : $"mesh{i}";
                Int32 vCount = baseMesh.vert[i].Length;
                Mesh mesh = new Mesh();
                mesh.name = meshName;
                mesh.vertices = baseMesh.vert[i];
                mesh.triangles = baseMesh.tri[i];
                if (texture?.uv?[i] != null && texture.uv[i].Length == vCount)
                    mesh.uv = texture.uv[i];
                if (extra?.normal?[i] != null && extra.normal[i].Length == vCount)
                    mesh.normals = extra.normal[i];
                if (extra?.tangent?[i] != null && extra.tangent[i].Length == vCount)
                    mesh.tangents = extra.tangent[i];
                if (extra?.color?[i] != null && extra.color[i].Length == vCount)
                    mesh.colors = extra.color[i];
                if (extra?.uv2?[i] != null && extra.uv2[i].Length == vCount)
                    mesh.uv2 = extra.uv2[i];
                if (anim?.bw?[i] != null && anim.bw[i].Length == vCount)
                {
                    mesh.boneWeights = anim.bw[i];
                    mesh.bindposes = bindPoses;
                }
                GameObject meshGo = new GameObject(meshName);
                meshGo.transform.parent = baseObject.transform;
                SkinnedMeshRenderer meshRenderer = meshGo.AddComponent<SkinnedMeshRenderer>();
                if (baseMesh.matIndex[i] < 0 || baseMesh.matIndex[i] >= materialCount)
                    throw new IndexOutOfRangeException($"Model mesh: mesh {i} has invalid material index {baseMesh.matIndex[i]} (max is {materialCount - 1})");
                meshRenderer.material = materials[baseMesh.matIndex[i]];
                meshRenderer.sharedMesh = mesh;
                if (anim != null)
                    meshRenderer.bones = bones;
            }
            return baseObject;
        }

        private static String GetShaderPathFromType(String shaderType, Int32 mode)
        {
            if (shaderType == "Default")
            {
                if (mode == 3)
                    return "WorldMap/Actor";
                if (mode == 2)
                    return "BattleMap_Common";
                return "Unlit/Transparent Cutout";
            }
            if (shaderType.StartsWith("SPS_ABR_"))
            {
                String abrType = shaderType.Substring("SPS_ABR_".Length);
                return (mode == 3 ? "WorldMap/SPS_Abr_" : "PSX/FieldSPS_Abr_") + abrType;
            }
            return shaderType;
        }
    }
}
