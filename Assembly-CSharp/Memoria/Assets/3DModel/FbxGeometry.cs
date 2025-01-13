using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    // Vertices and faces in this class are automatically modified compared to the raw datas, for better Unity compatibility
    // 1) UVs that are stored "ByPolygonVertex" are transformed into UVs stored by vertex, possibly duplicating vertices in the process
    // 2) Polygonal faces are transformed into triangular faces, possibly splitting polygons into multiple triangles
    // These don't affect FbxIO methods

    /// <summary>API for Geometry nodes</summary>
    public class FbxGeometry
    {
        public readonly FbxNode GeometryNode;
        public readonly FbxBone Bone; // Possibly null (although it better shouldn't)
        public readonly Int32 VertexCount;

        public String Name => ((String)GeometryNode.Properties[1]).Replace("Geometry::", "");

        public FbxGeometry(FbxNode geometryNode, FbxBone bone)
        {
            GeometryNode = geometryNode;
            Bone = bone;
            VertexCount = (GeometryNode["Vertices"]?.AsArray?.Length ?? 0) / 3;

            // Get triangle indices
            if (!ReadPolygonIndices())
                return;

            // If UVs are stored per face, do what is needed to have a unique UV per vertex
            String mapping = (String)GeometryNode["LayerElementUV"]?["MappingInformationType"]?.Value ?? "ByVertex";
            if (String.Equals(mapping, "ByPolygonVertex"))
            {
                _consistencyExtendedArray = new List<Vector2>(VertexCount);
                _consistencyVertexProcessed = new HashSet<Int32>();
                _vertexDuplicateIndices = new Dictionary<Int32, Int32>();
                _polygonDuplicateIndices = new Dictionary<Int32, Int32>();
                for (Int32 i = 0; i < VertexCount; i++)
                    _consistencyExtendedArray.Add(new Vector2());
                GetArrayFromLayer<Vector2>("LayerElementUV", "UV", "UVIndex", true);
                _uvs = _consistencyExtendedArray.ToArray();
                if (_consistencyExtendedArray.Count == VertexCount)
                {
                    // Nothing to do: UVs are already consistent
                    _vertexDuplicateIndices = null;
                }
                else
                {
                    // Duplicate vertices when required and register them as duplicates for normal/tangent/UV later linking
                    VertexCount = _consistencyExtendedArray.Count;
                    foreach (KeyValuePair<Int32, Int32> pair in _polygonDuplicateIndices)
                        _polygonRawIndices[pair.Key] = pair.Value;
                }
                _consistencyExtendedArray = null;
                _consistencyVertexProcessed = null;
                _polygonDuplicateIndices = null;
            }
            SetupTriangleIndices();
        }

        public Vector3[] GetVertices(Boolean relativeSpace = false)
        {
            Array arr = GeometryNode["Vertices"]?.AsArray;
            if (arr == null)
                return null;
            Int32 vertCount = arr.Length / 3;
            Vector3[] vec = new Vector3[VertexCount];
            var enumerator = arr.GetEnumerator();
            for (Int32 i = 0; i < vertCount; i++)
                vec[i] = EntryReader<Vector3>(enumerator);
            if (!relativeSpace && Bone != null)
            {
                Matrix4x4 transform = Bone.GetLocalToWorldMatrix();
                for (Int32 i = 0; i < vertCount; i++)
                    vec[i] = transform.MultiplyPoint3x4(vec[i]);
            }
            if (_vertexDuplicateIndices != null)
                foreach (KeyValuePair<Int32, Int32> pair in _vertexDuplicateIndices)
                    vec[pair.Value] = vec[pair.Key];
            return vec;
        }

        public Int32[] GetTriangleIndices()
        {
            return _triangleIndices;
        }

        public Vector2[] GetUVs()
        {
            if (_uvs == null)
                _uvs = GetArrayFromLayer<Vector2>("LayerElementUV", "UV", "UVIndex");
            return _uvs;
        }

        public Vector3[] GetNormals(Boolean relativeSpace = false)
        {
            Vector3[] normals = GetArrayFromLayer<Vector3>("LayerElementNormal", "Normals", "NormalsIndex");
            if (!relativeSpace && Bone != null && normals != null)
            {
                Matrix4x4 transform = Bone.GetLocalToWorldMatrix();
                for (Int32 i = 0; i < normals.Length; i++)
                    normals[i] = transform.MultiplyVector(normals[i]);
            }
            return normals;
        }

        public Vector4[] GetTangents(Boolean relativeSpace = false)
        {
            Vector4[] tangents = GetArrayFromLayer<Vector4>("LayerElementTangent", "Tangents", "TangentsIndex");
            if (!relativeSpace && Bone != null && tangents != null)
            {
                Matrix4x4 transform = Bone.GetLocalToWorldMatrix();
                for (Int32 i = 0; i < tangents.Length; i++)
                {
                    tangents[i] = transform.MultiplyVector(tangents[i]);
                    tangents[i].w = 1f;
                }
            }
            return tangents;
        }

        public Color[] GetColors()
        {
            // TODO: is it 0-1 range? 0-255? Need a FBX real example using it to know
            return GetArrayFromLayer<Color>("LayerElementColor", "Colors", "ColorIndex");
        }

        public BoneWeight[] GetBoneWeights(FbxSkeleton skeleton)
        {
            Boolean hasDeformers = skeleton.Bones.Any(b => b.SubdeformerNodes.Count > 0);
            if (!hasDeformers)
                return null;
            BoneWeight[] boneWeights = new BoneWeight[VertexCount];
            Int32 geoId = GeometryNode.Id;
            for (Int32 boneIndex = 0; boneIndex < skeleton.Bones.Count; boneIndex++)
            {
                FbxBone bone = skeleton.Bones[boneIndex];
                if (bone.SubdeformerNodes.TryGetValue(geoId, out FbxDeformer deformer) && deformer.GetWeights(out Int32[] indices, out Single[] weights))
                {
                    for (Int32 i = 0; i < indices.Length; i++)
                    {
                        FbxSkeleton.RegisterWeight(ref boneWeights[indices[i]], boneIndex, weights[i]);
                        if (_vertexDuplicateIndices != null)
                        {
                            Int32 vertIndex = indices[i];
                            while (_vertexDuplicateIndices.TryGetValue(vertIndex, out vertIndex))
                                FbxSkeleton.RegisterWeight(ref boneWeights[vertIndex], boneIndex, weights[i]);
                        }
                    }
                }
            }
            return boneWeights;
        }

        private T[] GetArrayFromLayer<T>(String layerName, String arrayName, String indexName, Boolean checkConsistency = false)
        {
            FbxNode layerNode = GeometryNode[layerName];
            if (layerNode == null)
                return null;
            Array arr = layerNode[arrayName]?.AsArray;
            if (arr == null)
                return null;
            Action<Int32, T[], T, String> entryMapper = checkConsistency ? CheckEntryUVConsistency<T> : MapEntryToVertex<T>;
            String mapping = (String)layerNode["MappingInformationType"]?.Value ?? "ByVertex";
            String reference = (String)layerNode["ReferenceInformationType"]?.Value ?? "Direct";
            T[] result = new T[VertexCount];
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
                        entryMapper(i, result, entries[EntryReader<Int32>(indexEnumerator)], mapping);
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
                    entryMapper(i, result, EntryReader<T>(enumerator), mapping);
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

        private void MapEntryToVertex<T>(Int32 index, T[] arr, T entry, String mapping)
        {
            switch (mapping)
            {
                case "ByVertex":
                case "ByVertice":
                    arr[index] = entry;
                    if (_vertexDuplicateIndices != null)
                        while (_vertexDuplicateIndices.TryGetValue(index, out index))
                            arr[index] = entry;
                    break;
                case "ByPolygonVertex":
                    arr[_polygonRawIndices[index]] = entry;
                    break;
                case "ByPolygon":
                {
                    Int32 end = index + 1 < _polygonSplitIndices.Length ? _polygonSplitIndices[index + 1] : _polygonRawIndices.Length;
                    for (Int32 i = _polygonSplitIndices[index]; i < end; i++)
                        arr[_polygonRawIndices[i]] = entry;
                    break;
                }
                case "ByEdge":
                    throw new NotSupportedException();
                case "AllSame":
                    for (Int32 i = 0; i < arr.Length; i++)
                        arr[i] = entry;
                    break;
            }
        }

        private void CheckEntryUVConsistency<T>(Int32 index, T[] arr, T entry, String mapping)
        {
            // assume mapping == "ByPolygonVertex" and T == Vector2
            Int32 vertIndex = _polygonRawIndices[index];
            while (_consistencyVertexProcessed.Contains(vertIndex))
            {
                if (entry.Equals(_consistencyExtendedArray[vertIndex]))
                {
                    if (vertIndex != _polygonRawIndices[index])
                        _polygonDuplicateIndices[index] = vertIndex;
                    return;
                }
                if (!_vertexDuplicateIndices.TryGetValue(vertIndex, out Int32 nextVertIndex))
                {
                    Int32 duplicateIndex = _consistencyExtendedArray.Count;
                    _vertexDuplicateIndices[vertIndex] = duplicateIndex;
                    _polygonDuplicateIndices[index] = duplicateIndex;
                    _consistencyExtendedArray.Add((Vector2)(object)entry);
                    _consistencyVertexProcessed.Add(duplicateIndex);
                    return;
                }
                vertIndex = nextVertIndex;
            }
            _consistencyExtendedArray[vertIndex] = (Vector2)(object)entry;
            _consistencyVertexProcessed.Add(vertIndex);
        }

        private Boolean ReadPolygonIndices()
        {
            Array arr = GeometryNode["PolygonVertexIndex"]?.AsArray;
            if (arr == null)
                return false;
            List<Int32> poly = new List<Int32>(arr.Length);
            List<Int32> polysplit = new List<Int32>(arr.Length / 3 + 1);
            var enumerator = arr.GetEnumerator();
            Int32 vertexCounter = 0;
            polysplit.Add(0);
            for (Int32 i = 0; i < arr.Length; i++)
            {
                Int32 index = EntryReader<Int32>(enumerator);
                if (index < 0)
                {
                    poly.Add(-index - 1);
                    polysplit.Add(i + 1);
                    vertexCounter = 0;
                }
                else
                {
                    poly.Add(index);
                    vertexCounter++;
                }
            }
            _polygonRawIndices = poly.ToArray();
            _polygonSplitIndices = polysplit.ToArray();
            return true;
        }

        private void SetupTriangleIndices()
        {
            List<Int32> tri = new List<Int32>(_polygonRawIndices.Length);
            Int32 triangleStart = 0;
            Int32 splitIndex = 1;
            for (Int32 i = 0; i < _polygonRawIndices.Length; i++)
            {
                if (i >= _polygonSplitIndices[splitIndex])
                    triangleStart = _polygonSplitIndices[splitIndex++];
                if (i >= triangleStart + 3)
                {
                    tri.Add(tri[tri.Count - 3]);
                    tri.Add(tri[tri.Count - 2]);
                }
                tri.Add(_polygonRawIndices[i]);
            }
            _triangleIndices = tri.ToArray();
        }

        private Int32[] _polygonRawIndices = null;
        private Int32[] _polygonSplitIndices = null;
        private Int32[] _triangleIndices = null;
        private Vector2[] _uvs = null;

        private List<Vector2> _consistencyExtendedArray;
        private HashSet<Int32> _consistencyVertexProcessed;
        private Dictionary<Int32, Int32> _vertexDuplicateIndices; // map: baseIndex -> duplicateIndex where indices are in [0, VertexCount-1]
        private Dictionary<Int32, Int32> _polygonDuplicateIndices; // map concerning only duplicated vertices and such that "_polygonDuplicateIndices[index] == _polygonRawIndices[index]"
    }
}
