using Memoria;
using Memoria.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// To have a good looking shading on low poly character model, We need to 'smooth' out the normal vector on character model.
// This operation are pretty slow and Ideally should perform on a worker-thread.

// However since we only perform the smooth operation on the first time the character mesh loaded & FF9's mesh poly count is very low.
// With modern PC, player should not notice any drop of framerate, even we perform these operation on main thread.
public static class NormalSolver
{
    //Source : https://medium.com/@fra3point/runtime-normals-recalculation-in-unity-a-complete-approach-db42490a5644
    /// <summary>
    ///     Recalculate the normals of a mesh based on an angle threshold. This takes
    ///     into account distinct vertices that have the same position.
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="angle">
    ///     The smoothing angle. Note that triangles that already share
    ///     the same vertex will be smooth regardless of the angle! 
    /// </param>
    public static void RecalculateNormals(this Mesh mesh, float angle)
    {
        //UnweldVertices(mesh);

        float cosineThreshold = Mathf.Cos(angle * Mathf.Deg2Rad);

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];

        // Holds the normal of each triangle in each sub mesh.
        Vector3[][] triNormals = new Vector3[mesh.subMeshCount][];

        Dictionary<VertexKey, List<VertexEntry>> dictionary =
            new Dictionary<VertexKey, List<VertexEntry>>(vertices.Length);

        for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; ++subMeshIndex)
        {
            int[] triangles = mesh.GetTriangles(subMeshIndex);

            triNormals[subMeshIndex] = new Vector3[triangles.Length / 3];

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                // Calculate the normal of the triangle
                Vector3 p1 = vertices[i2] - vertices[i1];
                Vector3 p2 = vertices[i3] - vertices[i1];
                Vector3 normal = Vector3.Cross(p1, p2);
                float magnitude = normal.magnitude;
                if (magnitude > 0)
                {
                    normal /= magnitude;
                }

                int triIndex = i / 3;
                triNormals[subMeshIndex][triIndex] = normal;

                List<VertexEntry> entry;
                VertexKey key;

                if (!dictionary.TryGetValue(key = new VertexKey(vertices[i1]), out entry))
                {
                    entry = new List<VertexEntry>(4);
                    dictionary.Add(key, entry);
                }

                entry.Add(new VertexEntry(subMeshIndex, triIndex, i1));

                if (!dictionary.TryGetValue(key = new VertexKey(vertices[i2]), out entry))
                {
                    entry = new List<VertexEntry>();
                    dictionary.Add(key, entry);
                }

                entry.Add(new VertexEntry(subMeshIndex, triIndex, i2));

                if (!dictionary.TryGetValue(key = new VertexKey(vertices[i3]), out entry))
                {
                    entry = new List<VertexEntry>();
                    dictionary.Add(key, entry);
                }

                entry.Add(new VertexEntry(subMeshIndex, triIndex, i3));
            }
        }

        // Each entry in the dictionary represents a unique vertex position.

        foreach (List<VertexEntry> vertList in dictionary.Values)
        {
            for (int i = 0; i < vertList.Count; ++i)
            {
                Vector3 sum = new Vector3();
                VertexEntry lhsEntry = vertList[i];

                for (int j = 0; j < vertList.Count; ++j)
                {
                    VertexEntry rhsEntry = vertList[j];

                    if (lhsEntry.VertexIndex == rhsEntry.VertexIndex)
                    {
                        sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                    }
                    else
                    {
                        // The dot product is the cosine of the angle between the two triangles.
                        // A larger cosine means a smaller angle.
                        float dot = Vector3.Dot(
                            triNormals[lhsEntry.MeshIndex][lhsEntry.TriangleIndex],
                            triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex]);
                        if (dot >= cosineThreshold)
                        {
                            sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                        }
                    }
                }

                normals[lhsEntry.VertexIndex] = sum.normalized;
            }
        }

        mesh.normals = normals;
    }

    private struct VertexKey
    {
        private readonly long _x;
        private readonly long _y;
        private readonly long _z;

        // Change this if you require a different precision.
        private const int Tolerance = 100000;

        // Magic FNV values. Do not change these.
        private const long FNV32Init = 0x811c9dc5;
        private const long FNV32Prime = 0x01000193;

        public VertexKey(Vector3 position)
        {
            _x = (long)(Mathf.Round(position.x * Tolerance));
            _y = (long)(Mathf.Round(position.y * Tolerance));
            _z = (long)(Mathf.Round(position.z * Tolerance));
        }

        public override bool Equals(object obj)
        {
            VertexKey key = (VertexKey)obj;
            return _x == key._x && _y == key._y && _z == key._z;
        }

        public override int GetHashCode()
        {
            long rv = FNV32Init;
            rv ^= _x;
            rv *= FNV32Prime;
            rv ^= _y;
            rv *= FNV32Prime;
            rv ^= _z;
            rv *= FNV32Prime;

            return rv.GetHashCode();
        }
    }

    private struct VertexEntry
    {
        public int MeshIndex;
        public int TriangleIndex;
        public int VertexIndex;

        public VertexEntry(int meshIndex, int triIndex, int vertIndex)
        {
            MeshIndex = meshIndex;
            TriangleIndex = triIndex;
            VertexIndex = vertIndex;
        }
    }

    public static void SmoothCharacterMesh(Renderer[] renderers)
    {
        if (Configuration.Shaders.CustomShaderEnabled != 1)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            var meshFilter = renderer.GetComponent<MeshFilter>();
            var skinnedMesh = renderer.GetComponent<SkinnedMeshRenderer>();
            if (meshFilter)
            {
                var originMesh = meshFilter.sharedMesh;
                var smoothedMesh = new Mesh();
                CopyMesh(originMesh, smoothedMesh);
                smoothedMesh.RecalculateNormals();
                smoothedMesh.RecalculateNormals(120);
                meshFilter.sharedMesh = smoothedMesh;
            }
            else if (skinnedMesh)
            {
                var originMesh = skinnedMesh.sharedMesh;
                var smoothedMesh = new Mesh();
                CopyMesh(originMesh, smoothedMesh);
                smoothedMesh.RecalculateNormals();
                smoothedMesh.RecalculateNormals(120);
                skinnedMesh.sharedMesh = smoothedMesh;
            }
        }

        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_OutlineWidth", 2f);
            renderer.material.SetFloat("_ShowOutline", Configuration.Shaders.Shader_Field_Outlines == 1 ? 1f : 0f);
        }
    }

    public static void SmoothCharacterMesh(SkinnedMeshRenderer[] renderers)
    {
        if (Configuration.Shaders.CustomShaderEnabled != 1)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            var originMesh = renderer.sharedMesh;
            var smoothedMesh = new Mesh();
            CopyMesh(originMesh, smoothedMesh);
            smoothedMesh.RecalculateNormals();
            smoothedMesh.RecalculateNormals(120);
            renderer.sharedMesh = smoothedMesh;
        }
    }

    public static void SmoothCharacterMesh(MeshRenderer[] renderers)
    {
        if (Configuration.Shaders.CustomShaderEnabled != 1)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            var meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter)
            {
                var originMesh = meshFilter.sharedMesh;
                var smoothedMesh = new Mesh();
                CopyMesh(originMesh, smoothedMesh);
                smoothedMesh.RecalculateNormals();
                smoothedMesh.RecalculateNormals(120);
                meshFilter.sharedMesh = smoothedMesh;
            }
        }
    }

    public static void CopyMesh(Mesh source, Mesh destination)
    {
        Vector3[] v = new Vector3[source.vertices.Length];
        int[][] t = new int[source.subMeshCount][];
        Vector2[] u = new Vector2[source.uv.Length];
        Vector2[] u2 = new Vector2[source.uv2.Length];
        Vector4[] tan = new Vector4[source.tangents.Length];
        Vector3[] n = new Vector3[source.normals.Length];
        Color32[] c = new Color32[source.colors32.Length];
        Array.Copy(source.vertices, v, v.Length);

        for (int i = 0; i < t.Length; i++)
            t[i] = source.GetTriangles(i);

        destination.Clear();
        destination.name = source.name;

        destination.vertices = v;

        destination.subMeshCount = t.Length;

        for (int i = 0; i < t.Length; i++)
            destination.SetTriangles(t[i], i);

        //destination.uv = u;
        //destination.uv2 = u2;
        destination.tangents = tan;
        destination.normals = n;
        destination.colors32 = c;

        var bindPoses = new List<Matrix4x4>();
        var boneweights = new List<BoneWeight>();

        bindPoses = source.bindposes.ToList();
        boneweights = source.boneWeights.ToList();
        var uv0 = new List<Vector2>();
        var uv1 = new List<Vector2>();
        uv0 = source.uv.ToList();
        uv1 = source.uv2.ToList();
        destination.uv = uv0.ToArray();
        destination.uv2 = uv1.ToArray();
        destination.boneWeights = boneweights.ToArray();
        destination.bindposes = bindPoses.ToArray();
    }
}
