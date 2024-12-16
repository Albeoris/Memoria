using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    /// <summary>
    /// API for Geometry nodes
    /// </summary>
    public class FbxGeometry
    {
        public FbxNode GeometryNode;

        public String Name => ((String)GeometryNode.Properties[1]).Replace("Geometry::", "");

        public FbxGeometry(FbxNode geometryNode)
        {
            GeometryNode = geometryNode;
        }

        public Vector3[] GetVertices()
        {
            Array arr = GeometryNode["Vertices"]?.AsArray;
            if (arr == null)
                return null;
            Int32 vertCount = arr.Length / 3;
            Vector3[] vec = new Vector3[vertCount];
            var enumerator = arr.GetEnumerator();
            for (Int32 i = 0; i < vertCount; i++)
                vec[i] = EntryReader<Vector3>(enumerator);
            return vec;
        }

        public Int32[] GetTriangleIndices()
        {
            Array arr = GeometryNode["PolygonVertexIndex"]?.AsArray;
            if (arr == null)
                return null;
            List<Int32> tri = new List<Int32>(arr.Length);
            var enumerator = arr.GetEnumerator();
            Int32 vertexCounter = 0;
            for (Int32 i = 0; i < arr.Length; i++)
            {
                Int32 index = EntryReader<Int32>(enumerator);
                if (index < 0)
                {
                    tri.Add(-index - 1);
                    vertexCounter = 0;
                }
                else
                {
                    tri.Add(index);
                    vertexCounter++;
                }
                if (vertexCounter >= 3)
                {
                    tri.Add(tri[tri.Count - 3]);
                    tri.Add(tri[tri.Count - 2]);
                }
            }
            return tri.ToArray();
        }

        public Vector2[] GetUVs(Vector3[] vertice, Int32[] triangles)
        {
            return GetArrayFromLayer<Vector2>(vertice, triangles, "LayerElementUV", "UV", "UVIndex");
        }

        public Vector3[] GetNormals(Vector3[] vertice, Int32[] triangles)
        {
            return GetArrayFromLayer<Vector3>(vertice, triangles, "LayerElementNormal", "Normals", "NormalsIndex");
        }

        public Vector4[] GetTangents(Vector3[] vertice, Int32[] triangles)
        {
            return GetArrayFromLayer<Vector4>(vertice, triangles, "LayerElementTangent", "Tangents", "TangentsIndex");
        }

        public Color[] GetColors(Vector3[] vertice, Int32[] triangles)
        {
            // TODO: is it 0-1 range? 0-255? Need a FBX real example using it to know
            return GetArrayFromLayer<Color>(vertice, triangles, "LayerElementColor", "Colors", "ColorIndex");
        }

        public BoneWeight[] GetBoneWeights(Vector3[] vertice, FbxSkeleton skeleton)
        {
            Boolean hasDeformers = skeleton.Bones.Any(b => b.SubdeformerNodes.Count > 0);
            if (!hasDeformers)
                return null;
            BoneWeight[] boneWeights = new BoneWeight[vertice.Length];
            Int32 geoId = GeometryNode.Id;
            for (Int32 boneIndex = 0; boneIndex < skeleton.Bones.Count; boneIndex++)
            {
                FbxBone bone = skeleton.Bones[boneIndex];
                if (bone.SubdeformerNodes.TryGetValue(geoId, out FbxDeformer deformer) && deformer.GetWeights(out Int32[] indices, out Single[] weights))
                    for (Int32 i = 0; i < indices.Length; i++)
                        FbxSkeleton.RegisterWeight(ref boneWeights[indices[i]], boneIndex, weights[i]);
            }
            return boneWeights;
        }

        private T[] GetArrayFromLayer<T>(Vector3[] vertice, Int32[] triangles, String layerName, String arrayName, String indexName)
        {
            FbxNode layerNode = GeometryNode[layerName];
            if (layerNode == null)
                return null;
            Array arr = layerNode[arrayName]?.AsArray;
            if (arr == null)
                return null;
            String mapping = (String)layerNode["MappingInformationType"]?.Value ?? "ByVertex";
            String reference = (String)layerNode["ReferenceInformationType"]?.Value ?? "Direct";
            T[] result = new T[vertice.Length];
            Boolean useDirectReference = reference == "Direct";
            if (reference == "IndexToDirect" || reference == "Index")
            {
                Array indices = layerNode[indexName]?.AsArray;
                if (indices != null)
                {
                    Int32 entryCount = arr.Length / EntryPerElement(typeof(T));
                    T[] entries = new T[entryCount];
                    var enumerator = arr.GetEnumerator();
                    for (Int32 i = 0; i < entryCount; i++)
                        entries[i] = EntryReader<T>(enumerator);
                    var indexEnumerator = indices.GetEnumerator();
                    for (Int32 i = 0; i < indices.Length; i++)
                        MapEntryToVertex(i, result, entries[EntryReader<Int32>(indexEnumerator)], mapping, triangles);
                }
                else
                {
                    useDirectReference = true;
                }
            }
            if (useDirectReference)
            {
                Int32 entryCount = arr.Length / EntryPerElement(typeof(T));
                var enumerator = arr.GetEnumerator();
                for (Int32 i = 0; i < entryCount; i++)
                    MapEntryToVertex(i, result, EntryReader<T>(enumerator), mapping, triangles);
            }
            return result;
        }

        private static Int32 EntryPerElement(Type t)
        {
            if (t == typeof(Vector2)) return 2;
            if (t == typeof(Vector3)) return 3;
            if (t == typeof(Vector4)) return 3;
            if (t == typeof(Color)) return 3;
            return 1;
        }

        private static T EntryReader<T>(IEnumerator enumerator)
        {
            if (typeof(T) == typeof(Int32))
            {
                enumerator.MoveNext();
                return (T)(object)Convert.ToInt32(enumerator.Current);
            }
            else if (typeof(T) == typeof(Single))
            {
                enumerator.MoveNext();
                return (T)(object)Convert.ToSingle(enumerator.Current);
            }
            else if (typeof(T) == typeof(Vector2))
            {
                enumerator.MoveNext();
                object x = enumerator.Current;
                enumerator.MoveNext();
                object y = enumerator.Current;
                return (T)(object)FbxNode.Vector2FromObjects(x, y);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                enumerator.MoveNext();
                object x = enumerator.Current;
                enumerator.MoveNext();
                object y = enumerator.Current;
                enumerator.MoveNext();
                object z = enumerator.Current;
                return (T)(object)FbxNode.Vector3FromObjects(x, y, z);
            }
            else if (typeof(T) == typeof(Vector4))
            {
                enumerator.MoveNext();
                object x = enumerator.Current;
                enumerator.MoveNext();
                object y = enumerator.Current;
                enumerator.MoveNext();
                object z = enumerator.Current;
                return (T)(object)FbxNode.Vector4FromObjects(x, y, z, 1f); // W parameter is ignored (could be stored separately)
            }
            else if (typeof(T) == typeof(Color))
            {
                enumerator.MoveNext();
                object r = enumerator.Current;
                enumerator.MoveNext();
                object g = enumerator.Current;
                enumerator.MoveNext();
                object b = enumerator.Current;
                return (T)(object)FbxNode.ColorFromObjects(r, g, b, 1f); // Alpha parameter is ignored (could be stored separately?)
            }
            throw new NotSupportedException();
        }

        private static void MapEntryToVertex<T>(Int32 index, T[] arr, T entry, String mapping, Int32[] triangles)
        {
            switch (mapping)
            {
                case "ByVertex":
                case "ByVertice":
                    arr[index] = entry;
                    break;
                case "ByPolygonVertex":
                    arr[triangles[index]] = entry;
                    break;
                case "ByPolygon":
                    arr[triangles[3 * index]] = entry;
                    arr[triangles[3 * index + 1]] = entry;
                    arr[triangles[3 * index + 2]] = entry;
                    break;
                case "ByEdge":
                    throw new NotSupportedException();
                case "AllSame":
                    for (Int32 i = 0; i < arr.Length; i++)
                        arr[i] = entry;
                    break;
            }
        }
    }
}
