using System;
using UnityEngine;

public class WalkMeshTriangle
{
	public WalkMeshTriangle()
	{
		this.floorIdx = -1;
		this.triIdx = -1;
		this.originalVertices = new Vector3[3];
		this.originalCenter = Vector3.zero;
		this.transformedVertices = new Vector3[3];
		this.transformedCenter = Vector3.zero;
		this.transformedRect = default(Rect);
		this.transformedZ = 0f;
		this.neighborIdx = new Int32[3];
		this.neighborIdx[0] = -1;
		this.neighborIdx[1] = -1;
		this.neighborIdx[2] = -1;
		this.cost = 1;
		this.pathCost = 0;
		this.next = (WalkMeshTriangle)null;
		this.nextListElem = (WalkMeshTriangle)null;
		this.triFlags = 1;
	}

	public void ResetPathLink()
	{
		this.pathCost = 0;
		this.next = (WalkMeshTriangle)null;
		this.nextListElem = (WalkMeshTriangle)null;
	}

	public void CalculateTransformedRect()
	{
		Vector3 vector = this.transformedVertices[0];
		this.transformedRect.xMin = vector.x;
		this.transformedRect.xMax = vector.x;
		this.transformedRect.yMin = vector.y;
		this.transformedRect.yMax = vector.y;
		for (Int32 i = 1; i < 3; i++)
		{
			vector = this.transformedVertices[i];
			if (vector.x < this.transformedRect.xMin)
			{
				this.transformedRect.xMin = vector.x;
			}
			if (vector.y < this.transformedRect.yMin)
			{
				this.transformedRect.yMin = vector.y;
			}
			if (vector.x > this.transformedRect.xMax)
			{
				this.transformedRect.xMax = vector.x;
			}
			if (vector.y > this.transformedRect.yMax)
			{
				this.transformedRect.yMax = vector.y;
			}
		}
	}

	public static void GetVertexIdxForEdge(Int32 edgeIdx, out Int32 v0, out Int32 v1)
	{
		if (edgeIdx == 0)
		{
			v0 = 2;
			v1 = 0;
		}
		else if (edgeIdx == 1)
		{
			v0 = 0;
			v1 = 1;
		}
		else
		{
			v0 = 1;
			v1 = 2;
		}
	}

	public Int32 floorIdx;

	public Int32 triIdx;

	public Vector3[] originalVertices;

	public Vector3 originalCenter;

	public Vector3[] transformedVertices;

	public Vector3 transformedCenter;

	public Rect transformedRect;

	public Single transformedZ;

	public Int32[] neighborIdx;

	public Int32 cost;

	public Int32 pathCost;

	public WalkMeshTriangle next;

	public WalkMeshTriangle nextListElem;

	public UInt16 triFlags;
}
