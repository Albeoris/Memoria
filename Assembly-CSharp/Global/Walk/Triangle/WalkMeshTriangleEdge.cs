using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkMeshTriangleEdge
{
	public WalkMeshTriangleEdge(Vector3 vert1, Vector3 vert2)
	{
		this.verts = new List<Vector3>();
		this.verts.Add(vert1);
		this.verts.Add(vert2);
	}

	public List<Vector3> verts;
}
