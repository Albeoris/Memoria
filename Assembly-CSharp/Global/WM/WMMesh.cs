using System;
using UnityEngine;

public struct WMMesh
{
    public Int32 Id;

    public Transform Transform;

    public Vector3[] Vertices;

    public Int32[] Triangles;

    public Vector3[] Normals;

    public Vector4[] Tangents;

    public Vector3[] TriangleNormals;
}
