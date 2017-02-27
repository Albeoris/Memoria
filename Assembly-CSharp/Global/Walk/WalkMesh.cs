using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class WalkMesh
{
	public WalkMesh(FieldMap fieldMap)
	{
		this.BGI_privateInit();
		this.BGI_publicInit();
        this.useCachedBounds = true;
        this.fieldMap = fieldMap;
		this.tris = new List<WalkMeshTriangle>();
		this.ProjectedWalkMesh = (GameObject)null;
		this.OriginalWalkMesh = (GameObject)null;
		this.trisPerCamera = new List<WalkMeshTriangle>[(Int32)fieldMap.scene.cameraCount];
		for (Int32 i = 0; i < (Int32)this.trisPerCamera.Length; i++)
		{
			this.trisPerCamera[i] = null;
		}
		this.ProcessBGI();
	}

    public void ProcessBGI()
    {
        if (this.trisPerCamera[this.fieldMap.camIdx] != null)
        {
            this.tris = this.trisPerCamera[this.fieldMap.camIdx];
            return;
        }
        BGSCENE_DEF scene = this.fieldMap.scene;
        BGI_DEF bgi = this.fieldMap.bgi;
        BGCAM_DEF bgcam_DEF = scene.cameraList[this.fieldMap.camIdx];
        this.trisPerCamera[this.fieldMap.camIdx] = new List<WalkMeshTriangle>();
        this.bgiCharPos = bgi.charPos.ToVector3();
        List<Vector3> list = new List<Vector3>();
        List<Vector3> list2 = new List<Vector3>();
        for (ushort num = 0; num < bgi.floorCount; num = (ushort)(num + 1))
        {
            BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(int)num];
            list.Clear();
            list2.Clear();
            for (int i = 0; i < bgi.vertexList.Count; i++)
            {
                Vector3 vector = bgi.vertexList[i].ToVector3() + bgi_FLOOR_DEF.orgPos.ToVector3() + bgi.orgPos.ToVector3();
                vector.y *= -1f;
                list.Add(vector);
                float num2 = PSX.CalculateGTE_RTPTZ(vector, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), this.fieldMap.offset);
                vector = PSX.CalculateGTE_RTPT(vector, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), this.fieldMap.offset);
                list2.Add(vector);
            }
            for (ushort num3 = 0; num3 < bgi_FLOOR_DEF.triCount; num3 = (ushort)(num3 + 1))
            {
                BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[bgi_FLOOR_DEF.triNdxList[(int)num3]];
                WalkMeshTriangle walkMeshTriangle = new WalkMeshTriangle();
                walkMeshTriangle.floorIdx = (int)num;
                walkMeshTriangle.triIdx = bgi_TRI_DEF.triIdx;
                for (int j = 0; j < 3; j++)
                {
                    walkMeshTriangle.originalVertices[j] = list[(int)bgi_TRI_DEF.vertexNdx[j]];
                    walkMeshTriangle.transformedVertices[j] = list2[(int)bgi_TRI_DEF.vertexNdx[j]];
                    walkMeshTriangle.neighborIdx[j] = (int)bgi_TRI_DEF.neighborNdx[j];
                }
                walkMeshTriangle.originalBounds = new Bounds(walkMeshTriangle.originalVertices[0], Vector3.zero);
                walkMeshTriangle.originalBounds.Encapsulate(walkMeshTriangle.originalVertices[1]);
                walkMeshTriangle.originalBounds.Encapsulate(walkMeshTriangle.originalVertices[2]);
                Vector3 size = walkMeshTriangle.originalBounds.size;
                size.y = 1f;
                walkMeshTriangle.originalBounds.size = size;
                Vector3 center = walkMeshTriangle.originalBounds.center;
                center.y = 0f;
                walkMeshTriangle.originalBounds.center = center;
                walkMeshTriangle.originalBoundsMin = walkMeshTriangle.originalBounds.min;
                walkMeshTriangle.originalBoundsMax = walkMeshTriangle.originalBounds.max;
                walkMeshTriangle.originalCenter = (walkMeshTriangle.originalVertices[0] + walkMeshTriangle.originalVertices[1] + walkMeshTriangle.originalVertices[2]) / 3f;
                walkMeshTriangle.transformedCenter = (walkMeshTriangle.transformedVertices[0] + walkMeshTriangle.transformedVertices[1] + walkMeshTriangle.transformedVertices[2]) / 3f;
                walkMeshTriangle.CalculateTransformedRect();
                walkMeshTriangle.transformedZ = PSX.CalculateGTE_RTPTZ(walkMeshTriangle.originalCenter, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), this.fieldMap.offset);
                this.trisPerCamera[this.fieldMap.camIdx].Add(walkMeshTriangle);
            }
        }
        this.tris = this.trisPerCamera[this.fieldMap.camIdx];
    }

    public void CreateProjectedWalkMesh()
	{
        if (!this.fieldMap.debugRender)
            return;

        BGSCENE_DEF scene = this.fieldMap.scene;
		BGI_DEF bgi = this.fieldMap.bgi;
		if (!FF9StateSystem.Field.isDebugWalkMesh)
		{
			GameObject gameObject = GameObject.Find("WalkMesh");
			if (gameObject != (UnityEngine.Object)null)
			{
				gameObject.SetActive(false);
			}
		}
		for (Int32 i = 0; i < (Int32)scene.cameraCount; i++)
		{
			BGCAM_DEF bgcam_DEF = scene.cameraList[i];
			Vector3 camPos = bgcam_DEF.GetCamPos();
			Vector2 centerOffset = bgcam_DEF.GetCenterOffset();
			Vector2 zero = Vector2.zero;
			zero.x = (Single)(bgcam_DEF.w / 2) + centerOffset.x;
			zero.y = -((Single)(bgcam_DEF.h / 2) + centerOffset.y);
			zero.x -= 160f;
			zero.y += 112f;
			GameObject gameObject2 = new GameObject("Projected_WalkMesh_Camera_" + i.ToString("D2"));
			gameObject2.transform.parent = this.fieldMap.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, (Single)scene.curZ);
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector2> list3 = new List<Vector2>();
			List<Int32> list4 = new List<Int32>();
			for (UInt16 num = 0; num < bgi.floorCount; num = (UInt16)(num + 1))
			{
				BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(Int32)num];
				String name = "Projected_Floor_" + num.ToString("D2");
				GameObject gameObject3 = new GameObject(name);
				Transform transform = gameObject3.transform;
				transform.parent = gameObject2.transform;
				transform.localPosition = new Vector3(0f, 0f, 0f);
				transform.localScale = new Vector3(1f, 1f, 1f);
				list.Clear();
				list2.Clear();
				list3.Clear();
				list4.Clear();
				for (Int32 j = 0; j < bgi.vertexList.Count; j++)
				{
					Vector3 vector = bgi.vertexList[j].ToVector3() + bgi_FLOOR_DEF.orgPos.ToVector3() + bgi.orgPos.ToVector3();
					vector.y *= -1f;
					list.Add(vector);
					Single num2 = PSX.CalculateGTE_RTPTZ(vector, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), this.fieldMap.offset);
					vector = PSX.CalculateGTE_RTPT(vector, Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), zero);
					list2.Add(vector);
					list3.Add(Vector2.zero);
				}
				for (UInt16 num3 = 0; num3 < bgi_FLOOR_DEF.triCount; num3 = (UInt16)(num3 + 1))
				{
					BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[bgi_FLOOR_DEF.triNdxList[(Int32)num3]];
					for (Int32 k = 0; k < 3; k++)
					{
						list4.Add((Int32)bgi_TRI_DEF.vertexNdx[k]);
					}
				}
				Mesh mesh = new Mesh();
				mesh.vertices = list2.ToArray();
				mesh.uv = list3.ToArray();
				mesh.triangles = list4.ToArray();
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				MeshFilter meshFilter = gameObject3.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = gameObject3.AddComponent<MeshRenderer>();
				meshFilter.mesh = mesh;
				meshRenderer.material = new Material(Shader.Find("Sprites/Default"))
				{
					color = new Color(1f, 1f, 1f, 0.5f)
				};
			}
			gameObject2.SetActive(false);
			bgcam_DEF.projectedWalkMesh = gameObject2;
		}
		BGCAM_DEF bgcam_DEF2 = scene.cameraList[this.fieldMap.camIdx];
		this.ProjectedWalkMesh = bgcam_DEF2.projectedWalkMesh;
	}

	public void CreateWalkMesh()
	{
        if (!this.fieldMap.debugRender)
            return;

        BGI_DEF bgi = this.fieldMap.bgi;
		GameObject gameObject = new GameObject("WalkMesh");
		gameObject.transform.parent = this.fieldMap.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(1f, -1f, 1f);
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<Int32> list3 = new List<Int32>();
		for (UInt16 num = 0; num < bgi.floorCount; num = (UInt16)(num + 1))
		{
			BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(Int32)num];
			String name = "Floor_" + num.ToString("D2");
			GameObject gameObject2 = new GameObject(name);
			Transform transform = gameObject2.transform;
			transform.parent = gameObject.transform;
			transform.localPosition = new Vector3(0f, 0f, 0f);
			transform.localScale = new Vector3(1f, 1f, 1f);
			list.Clear();
			list2.Clear();
			list3.Clear();
			for (Int32 i = 0; i < bgi.vertexList.Count; i++)
			{
				Vector3 item = bgi.vertexList[i].ToVector3() + bgi_FLOOR_DEF.orgPos.ToVector3() + bgi.orgPos.ToVector3();
				list.Add(item);
				list2.Add(Vector2.zero);
			}
			for (UInt16 num2 = 0; num2 < bgi_FLOOR_DEF.triCount; num2 = (UInt16)(num2 + 1))
			{
				BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[bgi_FLOOR_DEF.triNdxList[(Int32)num2]];
				for (Int32 j = 0; j < (Int32)bgi_TRI_DEF.vertexNdx.Length; j += 3)
				{
					Vector3 b = list[(Int32)bgi_TRI_DEF.vertexNdx[j]];
					Vector3 vector = list[(Int32)bgi_TRI_DEF.vertexNdx[j + 1]];
					Vector3 a = list[(Int32)bgi_TRI_DEF.vertexNdx[j + 2]];
					Vector3 lhs = Vector3.Cross(vector - b, a - vector);
					if (Vector3.Dot(lhs, Vector3.up) > 0f)
					{
						list3.Add((Int32)bgi_TRI_DEF.vertexNdx[j + 2]);
						list3.Add((Int32)bgi_TRI_DEF.vertexNdx[j + 1]);
						list3.Add((Int32)bgi_TRI_DEF.vertexNdx[j]);
					}
					else
					{
						list3.Add((Int32)bgi_TRI_DEF.vertexNdx[j]);
						list3.Add((Int32)bgi_TRI_DEF.vertexNdx[j + 1]);
						list3.Add((Int32)bgi_TRI_DEF.vertexNdx[j + 2]);
					}
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = list.ToArray();
			mesh.uv = list2.ToArray();
			mesh.triangles = list3.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			Material material;
			if (FF9StateSystem.Field.isDebugWalkMesh)
			{
				material = new Material(Shader.Find("Sprites/Default"));
			}
			else
			{
				material = new Material(Shader.Find("PSX/FieldMapActor"));
			}
			material.color = new Color(1f, 1f, 1f, 0.5f);
			meshRenderer.material = material;
			FieldMapActor fieldMapActor = gameObject2.AddComponent<FieldMapActor>();
		}
		this.OriginalWalkMesh = gameObject;
	}

	private void CreateWalls(FieldMap fieldMap)
	{
		GameObject gameObject = new GameObject("WalkMesh_Wall");
		gameObject.transform.parent = fieldMap.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<Int32> list3 = new List<Int32>();
		GameObject gameObject2 = new GameObject("Floor_Shared");
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
		list.Clear();
		list2.Clear();
		list3.Clear();
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			for (Int32 j = 0; j < 3; j++)
			{
				if (walkMeshTriangle.neighborIdx[j] == -1)
				{
					Int32 num = 0;
					Int32 num2 = 0;
					WalkMeshTriangle.GetVertexIdxForEdge(j, out num, out num2);
					Vector3 vector = walkMeshTriangle.originalVertices[num];
					Vector3 vector2 = walkMeshTriangle.originalVertices[num2];
					Int32 count = list.Count;
					list.Add(vector);
					list.Add(vector2);
					list.Add(vector2 + new Vector3(0f, 250f, 0f));
					list.Add(vector + new Vector3(0f, 250f, 0f));
					list2.Add(Vector2.zero);
					list2.Add(Vector2.zero);
					list2.Add(Vector2.zero);
					list2.Add(Vector2.zero);
					list3.Add(count + 2);
					list3.Add(count + 1);
					list3.Add(count);
					list3.Add(count + 3);
					list3.Add(count + 2);
					list3.Add(count);
					list3.Add(count);
					list3.Add(count + 2);
					list3.Add(count + 3);
					list3.Add(count);
					list3.Add(count + 1);
					list3.Add(count + 2);
				}
			}
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		Material material = new Material(Shader.Find("Sprites/Default"));
		material.color = new Color(1f, 1f, 1f, 0.5f);
		MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}

	public Int32 GetTriangleIndexAtScreenPos(Vector3 pos)
	{
		Int32 num = -1;
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			Vector3 vA = walkMeshTriangle.transformedVertices[0];
			Vector3 vB = walkMeshTriangle.transformedVertices[1];
			Vector3 vC = walkMeshTriangle.transformedVertices[2];
			BGI_TRI_DEF bgi_TRI_DEF = this.fieldMap.bgi.triList[i];
			if ((walkMeshTriangle.triFlags & 1) != 0)
			{
				if (walkMeshTriangle.transformedRect.Contains(pos))
				{
					if (Math3D.PointInsideTriangleTest(pos, vA, vB, vC) && (num == -1 || this.tris[i].transformedZ < this.tris[num].transformedZ) && this.tris[i].transformedZ > 0f)
					{
						num = i;
					}
				}
			}
		}
		return num;
	}

	public Int32 GetTriangleIndexAtScreenPosOrNearest(Vector3 pos)
	{
		Int32 triangleIndexAtScreenPos = this.GetTriangleIndexAtScreenPos(pos);
		if (triangleIndexAtScreenPos != -1)
		{
			return triangleIndexAtScreenPos;
		}
		Int32 num = -1;
		Single num2 = 0f;
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			for (Int32 j = 0; j < 3; j++)
			{
				Int32 num3;
				Int32 num4;
				WalkMeshTriangle.GetVertexIdxForEdge(j, out num3, out num4);
				Vector3 a = walkMeshTriangle.transformedVertices[num3];
				Vector3 b = walkMeshTriangle.transformedVertices[num4];
				Single num5 = Math3D.SqrDistanceToLine(pos, a, b);
				if (num == -1 || num5 < num2)
				{
					num = i;
					num2 = num5;
				}
			}
		}
		if (num2 > 900f)
		{
			return -1;
		}
		return num;
	}

	public void RenderWalkMeshTris(Int32 highlightTriIdx)
	{
        if (!this.fieldMap.debugRender)
            return;

        BGSCENE_DEF scene = this.fieldMap.scene;
		Vector3 zero = new Vector3(0f, 0f, (Single)scene.curZ);
		if (FF9StateSystem.Field.isDebugWalkMesh)
		{
			zero = Vector3.zero;
		}
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			Color white = Color.white;
			Vector3 a = walkMeshTriangle.transformedVertices[0];
			Vector3 a2 = walkMeshTriangle.transformedVertices[1];
			Vector3 a3 = walkMeshTriangle.transformedVertices[2];
			if (FF9StateSystem.Field.isDebugWalkMesh)
			{
				a = walkMeshTriangle.originalVertices[0];
				a2 = walkMeshTriangle.originalVertices[1];
				a3 = walkMeshTriangle.originalVertices[2];
			}
			global::Debug.DrawLine(a + zero, a2 + zero, white, 0f, true);
			global::Debug.DrawLine(a2 + zero, a3 + zero, white, 0f, true);
			global::Debug.DrawLine(a + zero, a3 + zero, white, 0f, true);
		}
		if (highlightTriIdx >= 0 && highlightTriIdx < this.tris.Count)
		{
			WalkMeshTriangle walkMeshTriangle2 = this.tris[highlightTriIdx];
			Color magenta = Color.magenta;
			Vector3 a4 = walkMeshTriangle2.transformedVertices[0];
			Vector3 a5 = walkMeshTriangle2.transformedVertices[1];
			Vector3 a6 = walkMeshTriangle2.transformedVertices[2];
			if (FF9StateSystem.Field.isDebugWalkMesh)
			{
				a4 = walkMeshTriangle2.originalVertices[0];
				a5 = walkMeshTriangle2.originalVertices[1];
				a6 = walkMeshTriangle2.originalVertices[2];
			}
			global::Debug.DrawLine(a4 + zero, a5 + zero, magenta, 0f, true);
			global::Debug.DrawLine(a5 + zero, a6 + zero, magenta, 0f, true);
			global::Debug.DrawLine(a4 + zero, a6 + zero, magenta, 0f, true);
		}
	}

	public void RenderWalkMeshNormal()
	{
        if (!this.fieldMap.debugRender)
            return;

        BGSCENE_DEF scene = this.fieldMap.scene;
		Boolean isDebugWalkMesh = FF9StateSystem.Field.isDebugWalkMesh;
		Vector3 zero = new Vector3(0f, 0f, (Single)scene.curZ);
		if (isDebugWalkMesh)
		{
			zero = Vector3.zero;
		}
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			Color blue = Color.blue;
			Vector3 vector = walkMeshTriangle.originalCenter;
			if (!isDebugWalkMesh)
			{
				vector = walkMeshTriangle.transformedCenter;
			}
			Int32 normalNdx = (Int32)this.fieldMap.bgi.triList[i].normalNdx;
			if (normalNdx >= 0)
			{
				Vector3 vector2 = this.fieldMap.bgi.normalList[normalNdx].ToVector3();
				vector2.Normalize();
				vector2.y *= -1f;
				vector2 *= 200f;
				global::Debug.DrawLine(vector, vector - vector2, blue, 0f, true);
			}
		}
	}

	public void RenderGraph()
	{
        if (!this.fieldMap.debugRender)
            return;

        BGSCENE_DEF scene = this.fieldMap.scene;
		Boolean isDebugWalkMesh = FF9StateSystem.Field.isDebugWalkMesh;
		Vector3 zero = new Vector3(0f, 0f, (Single)scene.curZ);
		if (!isDebugWalkMesh)
		{
			zero = Vector3.zero;
		}
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			Color blue = Color.blue;
			Vector3 vector = walkMeshTriangle.originalCenter;
			if (!isDebugWalkMesh)
			{
				vector = walkMeshTriangle.transformedCenter;
			}
			Int32 normalNdx = (Int32)this.fieldMap.bgi.triList[i].normalNdx;
			if (normalNdx >= 0)
			{
				Vector3 vector2 = this.fieldMap.bgi.normalList[normalNdx].ToVector3();
				vector2.Normalize();
				vector2.y *= -1f;
				vector2 *= 200f;
				global::Debug.DrawLine(vector, vector - vector2, blue, 0f, true);
			}
		}
		for (Int32 j = 0; j < this.tris.Count; j++)
		{
			WalkMeshTriangle walkMeshTriangle2 = this.tris[j];
			Color color = Color.green;
			for (Int32 k = 0; k < 3; k++)
			{
				if (walkMeshTriangle2.neighborIdx[k] != -1)
				{
					WalkMeshTriangle walkMeshTriangle3 = this.tris[walkMeshTriangle2.neighborIdx[k]];
					if (walkMeshTriangle2.floorIdx != walkMeshTriangle3.floorIdx)
					{
						color = Color.blue;
						Int32 num = 0;
						Int32 num2 = 0;
						WalkMeshTriangle.GetVertexIdxForEdge(k, out num, out num2);
						Vector3 a = walkMeshTriangle2.originalVertices[num];
						Vector3 a2 = walkMeshTriangle2.originalVertices[num2];
						if (!isDebugWalkMesh)
						{
							a = walkMeshTriangle2.transformedVertices[num];
							a2 = walkMeshTriangle2.transformedVertices[num2];
						}
						global::Debug.DrawLine(a + zero, a2 + zero, color, 0f, true);
					}
				}
				else
				{
					color = Color.black;
					Int32 num3 = 0;
					Int32 num4 = 0;
					WalkMeshTriangle.GetVertexIdxForEdge(k, out num3, out num4);
					Vector3 a3 = walkMeshTriangle2.originalVertices[num3];
					Vector3 a4 = walkMeshTriangle2.originalVertices[num4];
					if (!isDebugWalkMesh)
					{
						a3 = walkMeshTriangle2.transformedVertices[num3];
						a4 = walkMeshTriangle2.transformedVertices[num4];
					}
					global::Debug.DrawLine(a3 + zero, a4 + zero, color, 0f, true);
				}
			}
		}
	}

	private void DebugRenderEdge(BGI_TRI_DEF tri, Int32 edgeNdx)
	{
		BGSCENE_DEF scene = this.fieldMap.scene;
		Boolean isDebugWalkMesh = FF9StateSystem.Field.isDebugWalkMesh;
		Vector3 zero = new Vector3(0f, 0f, (Single)scene.curZ);
		if (!isDebugWalkMesh)
		{
			zero = Vector3.zero;
		}
		WalkMeshTriangle walkMeshTriangle = this.tris[tri.triIdx];
		Color red = Color.red;
		Int32 num = 0;
		Int32 num2 = 0;
		WalkMeshTriangle.GetVertexIdxForEdge(edgeNdx, out num, out num2);
		Vector3 a = walkMeshTriangle.originalVertices[num];
		Vector3 a2 = walkMeshTriangle.originalVertices[num2];
		if (!isDebugWalkMesh)
		{
			a = walkMeshTriangle.transformedVertices[num];
			a2 = walkMeshTriangle.transformedVertices[num2];
		}
		global::Debug.DrawLine(a + zero, a2 + zero, red, 1f, true);
	}

	public void DebugRenderTri(BGI_TRI_DEF tri)
	{
        if (!this.fieldMap.debugRender)
            return;

        WalkMeshTriangle tri2 = this.tris[tri.triIdx];
		this.DebugRenderTri(tri2);
	}

	public void DebugRenderTri(WalkMeshTriangle tri)
	{
		BGSCENE_DEF scene = this.fieldMap.scene;
		Boolean isDebugWalkMesh = FF9StateSystem.Field.isDebugWalkMesh;
		Vector3 zero = new Vector3(0f, 0f, (Single)scene.curZ);
		if (!isDebugWalkMesh)
		{
			zero = Vector3.zero;
		}
		Color red = Color.red;
		Vector3 a = tri.originalVertices[0];
		Vector3 a2 = tri.originalVertices[1];
		Vector3 a3 = tri.originalVertices[2];
		if (!isDebugWalkMesh)
		{
			a = tri.transformedVertices[0];
			a2 = tri.transformedVertices[1];
			a3 = tri.transformedVertices[2];
		}
		global::Debug.DrawLine(a + zero, a2 + zero, red, 10f, true);
		global::Debug.DrawLine(a2 + zero, a3 + zero, red, 10f, true);
		global::Debug.DrawLine(a + zero, a3 + zero, red, 10f, true);
	}

	public void DebugRenderTri(BGI_TRI_DEF tri, Color col)
	{
		WalkMeshTriangle tri2 = this.tris[tri.triIdx];
		this.DebugRenderTri(tri2, col);
	}

	public void DebugRenderTri(WalkMeshTriangle tri, Color col)
	{
		BGSCENE_DEF scene = this.fieldMap.scene;
		Boolean isDebugWalkMesh = FF9StateSystem.Field.isDebugWalkMesh;
		Vector3 zero = new Vector3(0f, 0f, (Single)scene.curZ);
		if (!isDebugWalkMesh)
		{
			zero = Vector3.zero;
		}
		Vector3 a = tri.originalVertices[0];
		Vector3 a2 = tri.originalVertices[1];
		Vector3 a3 = tri.originalVertices[2];
		if (!isDebugWalkMesh)
		{
			a = tri.transformedVertices[0];
			a2 = tri.transformedVertices[1];
			a3 = tri.transformedVertices[2];
		}
		global::Debug.DrawLine(a + zero, a2 + zero, col, 10f, true);
		global::Debug.DrawLine(a2 + zero, a3 + zero, col, 10f, true);
		global::Debug.DrawLine(a + zero, a3 + zero, col, 10f, true);
	}

	private void ResetPathLinks()
	{
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			this.tris[i].ResetPathLink();
		}
	}

	private Vector3 ForceForVector(WalkMeshTriangle targetTriangle, Single radius)
	{
		Single num = radius * radius;
		Vector3 vector = targetTriangle.originalCenter;
		List<Vector3> list = new List<Vector3>();
		Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
		List<WalkMeshTriangle> list2 = new List<WalkMeshTriangle>();
		queue.Enqueue(targetTriangle);
		list2.Add(targetTriangle);
		while (queue.Count > 0)
		{
			WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
			for (Int32 i = 0; i < 3; i++)
			{
				Int32 num2;
				Int32 num3;
				WalkMeshTriangle.GetVertexIdxForEdge(i, out num2, out num3);
				if (walkMeshTriangle.neighborIdx[i] != -1)
				{
					WalkMeshTriangle walkMeshTriangle2 = this.tris[walkMeshTriangle.neighborIdx[i]];
					if (!list2.Contains(this.tris[walkMeshTriangle.neighborIdx[i]]) && (num > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[1]) || num > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[1], walkMeshTriangle2.originalVertices[2]) || num > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[2])))
					{
						queue.Enqueue(this.tris[walkMeshTriangle.neighborIdx[i]]);
						list2.Add(this.tris[walkMeshTriangle.neighborIdx[i]]);
					}
				}
				else
				{
					Single num4 = Math3D.SqrDistanceToLine(vector, walkMeshTriangle.originalVertices[num2], walkMeshTriangle.originalVertices[num3]);
					if (num > num4)
					{
						Vector3 a = Math3D.ClosestPointToLine(vector, walkMeshTriangle.originalVertices[num2], walkMeshTriangle.originalVertices[num3]);
						Vector3 item = -a / Mathf.Sqrt(num4) * (Mathf.Sqrt(num) - Mathf.Sqrt(num4));
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			for (Int32 j = 0; j < list.Count; j++)
			{
				vector += list[j] * 0.5f;
			}
			list.Clear();
		}
		return vector;
	}

	public WalkMeshTriangle FindPathReversed(WalkMeshTriangle start, WalkMeshTriangle end, Single radius)
	{
		this.ResetPathLinks();
		MinHeap minHeap = new MinHeap();
		List<Int32> list = new List<Int32>();
		minHeap.Add(start);
		list.Add(start.triIdx);
		while (minHeap.HasNext())
		{
			WalkMeshTriangle walkMeshTriangle = minHeap.ExtractFirst();
			if (walkMeshTriangle.triIdx == end.triIdx)
			{
				return walkMeshTriangle;
			}
			for (Int32 i = 0; i < (Int32)walkMeshTriangle.neighborIdx.Length; i++)
			{
				Int32 num = walkMeshTriangle.neighborIdx[i];
				if (num != -1)
				{
					if (this.tris[num].triFlags != 0)
					{
						WalkMeshTriangle walkMeshTriangle2 = this.tris[num];
						if ((walkMeshTriangle2.triFlags & 1) != 0)
						{
							if (!list.Contains(walkMeshTriangle2.triIdx))
							{
								Vector3 vector = this.ForceForVector(end, radius) - this.ForceForVector(walkMeshTriangle, radius);
								vector.y = 0f;
								Int32 num2 = Mathf.FloorToInt(vector.magnitude);
								Int32 num3 = Mathf.FloorToInt(Mathf.Abs(vector.x) + Mathf.Abs(vector.z));
								walkMeshTriangle2.cost = num2 + num3;
								Int32 pathCost = walkMeshTriangle.pathCost + walkMeshTriangle2.cost;
								walkMeshTriangle2.pathCost = pathCost;
								walkMeshTriangle2.next = walkMeshTriangle;
								minHeap.Add(walkMeshTriangle2);
								list.Add(walkMeshTriangle2.triIdx);
							}
						}
					}
				}
			}
		}
		return (WalkMeshTriangle)null;
	}

	public WalkMeshTriangle FindPathReversed1(WalkMeshTriangle start, WalkMeshTriangle end)
	{
		this.ResetPathLinks();
		MinHeap minHeap = new MinHeap();
		List<Int32> list = new List<Int32>();
		minHeap.Add(start);
		list.Add(start.triIdx);
		while (minHeap.HasNext())
		{
			WalkMeshTriangle walkMeshTriangle = minHeap.ExtractFirst();
			if (walkMeshTriangle.triIdx == end.triIdx)
			{
				return walkMeshTriangle;
			}
			for (Int32 i = 0; i < (Int32)walkMeshTriangle.neighborIdx.Length; i++)
			{
				Int32 num = walkMeshTriangle.neighborIdx[i];
				if (num != -1)
				{
					WalkMeshTriangle walkMeshTriangle2 = this.tris[num];
					if (!list.Contains(walkMeshTriangle2.triIdx))
					{
						Int32 num2 = Mathf.FloorToInt((end.originalCenter - walkMeshTriangle.originalCenter).magnitude);
						Single num3 = Single.MaxValue;
						for (Int32 j = 0; j < 3; j++)
						{
							Int32 num4;
							Int32 num5;
							WalkMeshTriangle.GetVertexIdxForEdge(j, out num4, out num5);
							if (walkMeshTriangle2.neighborIdx[j] != -1)
							{
								num3 = Mathf.Min(num3, (walkMeshTriangle2.originalVertices[num4] - walkMeshTriangle2.originalVertices[num5]).magnitude);
							}
						}
						walkMeshTriangle2.cost = (Int32)((Single)num2 / (Single)Mathf.FloorToInt(num3));
						Int32 pathCost = walkMeshTriangle.pathCost + walkMeshTriangle2.cost;
						walkMeshTriangle2.pathCost = pathCost;
						walkMeshTriangle2.next = walkMeshTriangle;
						minHeap.Add(walkMeshTriangle2);
						list.Add(walkMeshTriangle2.triIdx);
					}
				}
			}
		}
		return (WalkMeshTriangle)null;
	}

	public Single GetTriangleHeightAtPos(Vector3 pos, out Boolean found)
	{
		found = false;
		Int32 num = -1;
		Int32 num2 = 0;
		for (Int32 i = 0; i < this.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.tris[i];
			Vector3 vA = walkMeshTriangle.originalVertices[0];
			Vector3 vB = walkMeshTriangle.originalVertices[1];
			Vector3 vC = walkMeshTriangle.originalVertices[2];
			if (Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC))
			{
				num = i;
				num2++;
			}
		}
		if (num != -1)
		{
			found = true;
			WalkMeshTriangle walkMeshTriangle2 = this.tris[num];
			return walkMeshTriangle2.originalVertices[1].y;
		}
		return 0f;
	}

	private void CopyLastPos(BGI_CHAR_DEF charDef)
	{
		charDef.lastPos = charDef.charPos;
	}

	private void RotateVector(ref Vector3 v, Single rotX, Single rotY)
	{
		Quaternion rotation = Quaternion.Euler(rotX, rotY, 0f);
		v = rotation * v;
	}

	public Single CollisionAngle(FieldMapActorController fmac, Obj obj, Single myrot)
	{
		PosObj posObj = (PosObj)obj;
		Single x = fmac.curPos.x;
		Single num = posObj.pos[0];
		Single z = fmac.curPos.z;
		Single num2 = posObj.pos[2];
		Single x2 = num - x;
		Single y = num2 - z;
		Single num3 = Mathf.Atan2(y, x2);
		return myrot - num3;
	}

	private Single disdif(Single x, Single z, Single r)
	{
		return x * x + z * z - r * r;
	}

	public PosObj Collision(FieldMapActorController fieldMapActorController, Int32 mode, out Single distance)
	{
		if (FF9StateSystem.Field.isDebug)
		{
			distance = 0f;
			return (PosObj)null;
		}
		PosObj result = (PosObj)null;
		Boolean flag = (mode & 4) != 0;
		Vector3 curPos = fieldMapActorController.curPos;
		Single num = (Single)(4 * (Byte)((!flag) ? fieldMapActorController.originalActor.collRad : fieldMapActorController.originalActor.talkRad));
		Single num2 = 0f;
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		for (ObjList objList = instance.GetActiveObjList(); objList != null; objList = objList.next)
		{
			Obj obj = objList.obj;
			Boolean flag2 = flag || (fieldMapActorController.originalActor.flags & (Byte)((obj.uid != instance.GetControlUID()) ? 4 : 2)) != 0;
			Boolean flag3 = (obj.flags & (Byte)((!flag) ? ((Byte)((fieldMapActorController.originalActor.uid != instance.GetControlUID()) ? 4 : 2)) : 8)) != 0;
			if (obj != fieldMapActorController.originalActor && (!flag2 || !flag3) && ((mode & 6) == 0 || instance.GetIP((Int32)obj.sid, (Int32)((!flag) ? 2 : 3), obj.ebData) != instance.nil) && obj.cid == 4)
			{
				Actor actor = (Actor)obj;
				Single[] pos = actor.pos;
				Single num3 = pos[1] - curPos.y;
				if (num3 > -400f && num3 < 400f)
				{
					Single num4 = pos[0] - curPos.x;
					Single num5 = pos[2] - curPos.z;
					if (((num4 < 0f) ? (-num4) : num4) < 2048f && ((num5 < 0f) ? (-num5) : num5) < 2048f)
					{
						Single num6 = (Single)(4 * (Byte)((!flag) ? actor.collRad : actor.talkRad));
						num6 += num;
						if ((mode & 6) != 0)
						{
							num6 += (Single)actor.speed + 60f;
						}
						Single num7 = this.disdif(num4, num5, num6);
						if (flag)
						{
							Single num8 = (Single)PersistenSingleton<EventEngine>.Instance.eBin.CollisionAngle(fieldMapActorController.originalActor, actor, fieldMapActorController.originalActor.rotAngle[1]);
							num8 = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)num8);
							if (num8 < 0f)
							{
								num8 = -num8;
							}
							num8 -= 640f;
							if (num8 < 0f)
							{
								num7 += num8;
								if (num2 > num7 && Mathf.Abs(Mathf.Abs(num2) - Mathf.Abs(num7)) > 1f)
								{
									num2 = num7;
									result = actor;
								}
							}
						}
						else if (num2 > num7 && Mathf.Abs(Mathf.Abs(num2) - Mathf.Abs(num7)) > 1f)
						{
							num2 = num7;
							result = actor;
						}
					}
				}
			}
		}
		distance = num2;
		return result;
	}

	private static void ShowDebugCollision(Actor actor1, Single r, Vector3 p0, Single r0)
	{
		for (Int32 i = 0; i < 10; i++)
		{
			Vector3 position = actor1.go.transform.position;
			Vector3 position2 = actor1.go.transform.position;
			Vector3 vector = new Vector3((Single)i, 0f, (Single)(9 - i));
			global::Debug.DrawLine(position, position2 + vector.normalized * r, Color.red, 0.5f, true);
			Vector3 position3 = actor1.go.transform.position;
			Vector3 position4 = actor1.go.transform.position;
			Vector3 vector2 = new Vector3((Single)(-(Single)i), 0f, (Single)(9 - i));
			global::Debug.DrawLine(position3, position4 + vector2.normalized * r, Color.red, 0.5f, true);
			Vector3 position5 = actor1.go.transform.position;
			Vector3 position6 = actor1.go.transform.position;
			Vector3 vector3 = new Vector3((Single)i, 0f, (Single)(-9 + i));
			global::Debug.DrawLine(position5, position6 + vector3.normalized * r, Color.red, 0.5f, true);
			Vector3 position7 = actor1.go.transform.position;
			Vector3 position8 = actor1.go.transform.position;
			Vector3 vector4 = new Vector3((Single)(-(Single)i), 0f, (Single)(-9 + i));
			global::Debug.DrawLine(position7, position8 + vector4.normalized * r, Color.red, 0.5f, true);
		}
		for (Int32 j = 0; j < 10; j++)
		{
			Vector3 vector5 = new Vector3((Single)j, 0f, (Single)(9 - j));
			global::Debug.DrawLine(p0, p0 + vector5.normalized * r0, Color.green, 0.5f, true);
			Vector3 vector6 = new Vector3((Single)(-(Single)j), 0f, (Single)(9 - j));
			global::Debug.DrawLine(p0, p0 + vector6.normalized * r0, Color.green, 0.5f, true);
			Vector3 vector7 = new Vector3((Single)j, 0f, (Single)(-9 + j));
			global::Debug.DrawLine(p0, p0 + vector7.normalized * r0, Color.green, 0.5f, true);
			Vector3 vector8 = new Vector3((Single)(-(Single)j), 0f, (Single)(-9 + j));
			global::Debug.DrawLine(p0, p0 + vector8.normalized * r0, Color.green, 0.5f, true);
		}
	}

	private void MovePC(FieldMapActor actor)
	{
	}

	public Int32 BGI_floorSetActive(UInt32 floorNdx, UInt32 isActive)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		if ((UInt64)floorNdx > (UInt64)((Int64)(bgi.floorList.Count - 1)))
		{
			return 0;
		}
		BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(Int32)floorNdx];
		UInt16 num = 1;
		if (isActive > 0u)
		{
			BGI_FLOOR_DEF bgi_FLOOR_DEF2 = bgi_FLOOR_DEF;
			bgi_FLOOR_DEF2.floorFlags = (UInt16)(bgi_FLOOR_DEF2.floorFlags | num);
			for (Int32 i = 0; i < (Int32)bgi_FLOOR_DEF.triCount; i++)
			{
				BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[bgi_FLOOR_DEF.triNdxList[i]];
				bgi_TRI_DEF.triFlags = (UInt16)(bgi_TRI_DEF.triFlags | num);
				WalkMeshTriangle walkMeshTriangle = this.tris[bgi_FLOOR_DEF.triNdxList[i]];
				walkMeshTriangle.triFlags = (UInt16)(walkMeshTriangle.triFlags | num);
			}
		}
		else
		{
			BGI_FLOOR_DEF bgi_FLOOR_DEF3 = bgi_FLOOR_DEF;
			bgi_FLOOR_DEF3.floorFlags = (UInt16)(bgi_FLOOR_DEF3.floorFlags & (UInt16)(~num));
			for (Int32 j = 0; j < (Int32)bgi_FLOOR_DEF.triCount; j++)
			{
				BGI_TRI_DEF bgi_TRI_DEF2 = bgi.triList[bgi_FLOOR_DEF.triNdxList[j]];
				bgi_TRI_DEF2.triFlags = (UInt16)(bgi_TRI_DEF2.triFlags & (UInt16)(~num));
				WalkMeshTriangle walkMeshTriangle2 = this.tris[bgi_FLOOR_DEF.triNdxList[j]];
				walkMeshTriangle2.triFlags = (UInt16)(walkMeshTriangle2.triFlags & (UInt16)(~num));
			}
		}
		return 1;
	}

	public Int32 BGI_triSetActive(UInt32 triNdx, UInt32 isActive)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[(Int32)triNdx];
		UInt16 num = 1;
		if (isActive > 0u)
		{
			BGI_TRI_DEF bgi_TRI_DEF2 = bgi_TRI_DEF;
			bgi_TRI_DEF2.triFlags = (UInt16)(bgi_TRI_DEF2.triFlags | num);
			WalkMeshTriangle walkMeshTriangle = this.tris[(Int32)triNdx];
			walkMeshTriangle.triFlags = (UInt16)(walkMeshTriangle.triFlags | num);
		}
		else
		{
			BGI_TRI_DEF bgi_TRI_DEF3 = bgi_TRI_DEF;
			bgi_TRI_DEF3.triFlags = (UInt16)(bgi_TRI_DEF3.triFlags & (UInt16)(~num));
			WalkMeshTriangle walkMeshTriangle2 = this.tris[(Int32)triNdx];
			walkMeshTriangle2.triFlags = (UInt16)(walkMeshTriangle2.triFlags & (UInt16)(~num));
		}
		return 1;
	}

	public Int32 BGI_charSetActive(FieldMapActorController fmac, UInt32 isActive)
	{
		UInt16 num = 1;
		if (isActive > 0u)
		{
			fmac.charFlags = (UInt16)(fmac.charFlags | num);
		}
		else
		{
			fmac.charFlags = (UInt16)(fmac.charFlags & (UInt16)(~num));
			fmac.activeFloor = -1;
			fmac.activeTri = -1;
			fmac.lastFloor = -1;
			fmac.lastTri = -1;
		}
		return 1;
	}

	public Int32 BGI_animShowFrame(UInt32 animNdx, UInt32 frameNdx)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		BGI_ANM_DEF bgi_ANM_DEF = bgi.anmList[(Int32)animNdx];
		UInt16 num = 1;
		if (animNdx >= (UInt32)bgi.anmCount)
		{
			return 0;
		}
		if (frameNdx >= (UInt32)bgi_ANM_DEF.frameCount)
		{
			return 0;
		}
		Int16 num2 = -1;
		List<BGI_FRAME_DEF> frameList = bgi_ANM_DEF.frameList;
		List<Int32> triIdxList;
		for (Int32 i = 0; i < (Int32)bgi_ANM_DEF.frameCount; i++)
		{
			triIdxList = frameList[i].triIdxList;
			for (Int32 j = 0; j < (Int32)frameList[i].triCount; j++)
			{
				BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[triIdxList[j]];
				if ((bgi_TRI_DEF.triFlags & num) != 0)
				{
					num2 = (Int16)i;
				}
				BGI_TRI_DEF bgi_TRI_DEF2 = bgi_TRI_DEF;
				bgi_TRI_DEF2.triFlags = (UInt16)(bgi_TRI_DEF2.triFlags & (UInt16)(~num));
				WalkMeshTriangle walkMeshTriangle = this.tris[bgi_TRI_DEF.triIdx];
				walkMeshTriangle.triFlags = (UInt16)(walkMeshTriangle.triFlags & (UInt16)(~num));
			}
		}
		BGI_FRAME_DEF bgi_FRAME_DEF = bgi.anmList[(Int32)animNdx].frameList[(Int32)frameNdx];
		triIdxList = bgi_FRAME_DEF.triIdxList;
		for (Int32 i = 0; i < (Int32)bgi_FRAME_DEF.triCount; i++)
		{
			BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[triIdxList[i]];
			BGI_TRI_DEF bgi_TRI_DEF3 = bgi_TRI_DEF;
			bgi_TRI_DEF3.triFlags = (UInt16)(bgi_TRI_DEF3.triFlags | num);
			WalkMeshTriangle walkMeshTriangle2 = this.tris[bgi_TRI_DEF.triIdx];
			walkMeshTriangle2.triFlags = (UInt16)(walkMeshTriangle2.triFlags | num);
			this.DebugRenderTri(bgi_TRI_DEF);
		}
		if ((UInt64)frameNdx != (UInt64)((Int64)num2))
		{
			this.BGI_animUpdateCharacters(bgi, bgi_ANM_DEF, bgi_FRAME_DEF, num2, (UInt16)frameNdx);
			this.BGI_animUpdateNeighbors(bgi, bgi_ANM_DEF, bgi_FRAME_DEF);
		}
		return 1;
	}

	public Int32 BGI_animGetFrame(UInt32 animNdx, ref Int32 frameNdx)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		if (animNdx >= (UInt32)bgi.anmCount)
		{
			return 0;
		}
		BGI_ANM_DEF bgi_ANM_DEF = bgi.anmList[(Int32)animNdx];
		frameNdx = bgi_ANM_DEF.curFrame >> 8;
		return 1;
	}

	public BGI_TRI_DEF BGI_findAccessibleTriangle(FieldMapActorController fmac, WalkMeshTriangle triPtr, UInt32 ndx)
	{
		if (triPtr.neighborIdx[(Int32)((UIntPtr)ndx)] == -1)
		{
			return (BGI_TRI_DEF)null;
		}
		BGI_TRI_DEF bgi_TRI_DEF = this.fieldMap.bgi.triList[triPtr.neighborIdx[(Int32)((UIntPtr)ndx)]];
		if ((bgi_TRI_DEF.triFlags & 1) == 0)
		{
			return (BGI_TRI_DEF)null;
		}
		Byte b = (Byte)(BGI.BGI_TRI_BITS_GET(bgi_TRI_DEF.triFlags) & this.fieldMap.bgi.attributeMask);
		if ((b & 192) == 0)
		{
			return bgi_TRI_DEF;
		}
		if (fmac.originalActor.sid == PersistenSingleton<EventEngine>.Instance.GetControlUID())
		{
			if ((b & 128) != 0)
			{
				return (BGI_TRI_DEF)null;
			}
		}
		else if ((b & 64) != 0)
		{
			return (BGI_TRI_DEF)null;
		}
		return bgi_TRI_DEF;
	}

	public Boolean BGI_pointInPoly(BGI_FLOOR_DEF floor, BGI_TRI_DEF tri, Vector3 vec)
	{
		Int32[] array = new Int32[2];
		Single[] array2 = new Single[2];
		BGI_VEC_DEF[] array3 = new BGI_VEC_DEF[2];
		Int16[] array4 = new Int16[3];
		BGI_VEC_DEF[] array5 = new BGI_VEC_DEF[3];
		for (Int32 i = 0; i < 3; i++)
		{
			array5[i] = new BGI_VEC_DEF();
		}
		BGI_DEF bgi = this.fieldMap.bgi;
		Int32 num = 0;
		Int32 num2 = 2;
		if (tri.normalNdx >= 0)
		{
			BGI_FVEC_DEF bgi_FVEC_DEF = bgi.normalList[(Int32)tri.normalNdx];
			if (bgi_FVEC_DEF.coord[1] > 20724 || bgi_FVEC_DEF.coord[1] < -20724)
			{
				num = 0;
				num2 = 2;
			}
			else if (Mathf.Abs(bgi_FVEC_DEF.coord[0]) > Mathf.Abs(bgi_FVEC_DEF.coord[2]))
			{
				num = 1;
				num2 = 2;
			}
			else
			{
				num = 0;
				num2 = 1;
			}
		}
		Boolean flag = false;
		List<BGI_VEC_DEF> vertexList = this.fieldMap.bgi.vertexList;
		array4[num] = (Int16)(bgi.curPos.coord[num] + floor.curPos.coord[num]);
		array4[num2] = (Int16)(bgi.curPos.coord[num2] + floor.curPos.coord[num2]);
		array[0] = 0;
		array[1] = 0;
		for (Int32 j = 0; j < 3; j++)
		{
			array5[j].coord[num] = (Int16)(array4[num] + vertexList[(Int32)tri.vertexNdx[j]].coord[num]);
			array5[j].coord[num2] = (Int16)(array4[num2] + vertexList[(Int32)tri.vertexNdx[j]].coord[num2]);
			if ((Single)array5[j].coord[num] == vec[num])
			{
				array[0]++;
				if ((Single)array5[j].coord[num2] <= vec[num2])
				{
					array[0] = -array[0];
				}
			}
			if ((Single)array5[j].coord[num2] == vec[num2])
			{
				array[1]++;
				if ((Single)array5[j].coord[num] <= vec[num])
				{
					array[1] = -array[1];
				}
			}
		}
		if (array[0] == -2 || array[1] == -2)
		{
			return true;
		}
		array3[0] = array5[2];
		array[0] = (Int32)((vec[num2] < (Single)array3[0].coord[num2]) ? 0 : 1);
		for (Int32 k = 0; k < 3; k++)
		{
			array3[1] = array5[k];
			array[1] = (Int32)((vec[num2] < (Single)array3[1].coord[num2]) ? 0 : 1);
			if (array[0] != array[1])
			{
				array2[0] = ((Single)array3[1].coord[num2] - vec[num2]) * (Single)(array3[0].coord[num] - array3[1].coord[num]);
				array2[1] = ((Single)array3[1].coord[num] - vec[num]) * (Single)(array3[0].coord[num2] - array3[1].coord[num2]);
				if (EventEngineUtils.nearlyEqual(array2[0], array2[1]))
				{
					return true;
				}
				if (array2[0] > array2[1] == (array[1] != 0))
				{
					flag = !flag;
				}
			}
			array3[0] = array3[1];
			array[0] = array[1];
		}
		return flag;
	}

	public Boolean BGI_traverseTriangles(FieldMapActorController fmac, ref BGI_TRI_DEF tri, Vector3 oldCPos, Vector3 cPos, Int32 recurseDepth, Single fracX, Single fracZ)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		Vector3 p = oldCPos;
		Vector3 vector = cPos;
		Int32 num = -1;
		Int32 num2 = -1;
		Int16 activeTri = (Int16)fmac.activeTri;
		Boolean flag = false;
		BGI_TRI_DEF bgi_TRI_DEF = tri;
		while (!flag)
		{
			Vector3 perp;
			this.BGI_calcSlopedCharPosition(bgi_TRI_DEF, p, ref cPos, out perp);
			Boolean flag2 = this.BGI_pointInPoly(bgi.floorList[(Int32)bgi_TRI_DEF.floorNdx], bgi_TRI_DEF, cPos);
			if (flag2)
			{
				flag = true;
			}
			else
			{
				flag2 = this.BGI_findCrossingEdge(bgi_TRI_DEF, cPos, oldCPos, out p, out num2, num2);
				if (flag2)
				{
					p.y = -p.y;
					BGI_TRI_DEF bgi_TRI_DEF2 = this.BGI_findAccessibleTriangle(fmac, this.tris[bgi_TRI_DEF.triIdx], (UInt32)num2);
					if (bgi_TRI_DEF2 == null)
					{
						fmac.activeTri = (Int32)activeTri;
						bgi_TRI_DEF = bgi.triList[fmac.activeTri];
						fmac.activeFloor = (Int32)bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
					if (bgi_TRI_DEF.normalNdx >= 0)
					{
						this.BGI_calcFlatCharPosition(bgi_TRI_DEF, p, ref cPos, perp);
					}
					if ((Int32)bgi_TRI_DEF.neighborNdx[num2] == num)
					{
						fmac.activeTri = (Int32)activeTri;
						bgi_TRI_DEF = bgi.triList[fmac.activeTri];
						fmac.activeFloor = (Int32)bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
					num = fmac.activeTri;
					fmac.activeTri = (Int32)bgi_TRI_DEF.neighborNdx[num2];
					BGI_EDGE_DEF bgi_EDGE_DEF = bgi.edgeList[(Int32)bgi_TRI_DEF.edgeNdx[num2]];
					bgi_TRI_DEF = bgi.triList[fmac.activeTri];
					fmac.activeFloor = (Int32)bgi_TRI_DEF.floorNdx;
					num2 = (Int32)bgi_EDGE_DEF.edgeClone;
					if (num2 == -1)
					{
						fmac.activeTri = (Int32)activeTri;
						bgi_TRI_DEF = bgi.triList[fmac.activeTri];
						fmac.activeFloor = (Int32)bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
				}
				else
				{
					this.BGI_triVisitClear();
					flag2 = this.BGI_scanForValidTriangle(fmac.actor.charDef, ref bgi_TRI_DEF, cPos, 0);
					if (!flag2)
					{
						if (recurseDepth < 6)
						{
							cPos = vector;
							this.BGI_extendVector(oldCPos, ref cPos, ref fracX, ref fracZ);
							fmac.curPos = cPos;
							fmac.activeTri = (Int32)activeTri;
							bgi_TRI_DEF = bgi.triList[fmac.activeTri];
							fmac.activeFloor = (Int32)bgi_TRI_DEF.floorNdx;
							if (!this.BGI_traverseTriangles(fmac, ref bgi_TRI_DEF, oldCPos, cPos, recurseDepth + 1, fracX, fracZ))
							{
								tri = bgi_TRI_DEF;
								return false;
							}
						}
						fmac.activeTri = (Int32)activeTri;
						bgi_TRI_DEF = bgi.triList[fmac.activeTri];
						fmac.activeFloor = (Int32)bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
					flag = true;
				}
			}
		}
		tri = bgi_TRI_DEF;
		return false;
	}

	private Int32 BGI_animApplyBarycentric(BGI_DEF bgiPtr, FieldMapActorController charPtr, BGI_TRI_DEF srcTri, BGI_TRI_DEF dstTri)
	{
		Single[] array = new Single[3];
		Single[] array2 = new Single[3];
		Single[] array3 = new Single[3];
		Single[] array4 = new Single[3];
		Single[] array5 = new Single[3];
		Single[] array6 = new Single[3];
		Single[] array7 = new Single[3];
		BGI_DEF bgi = this.fieldMap.bgi;
		BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(Int32)srcTri.floorNdx];
		array[0] = (Single)(bgiPtr.curPos.coord[0] + bgi_FLOOR_DEF.curPos.coord[0]);
		array[1] = (Single)(bgiPtr.curPos.coord[1] + bgi_FLOOR_DEF.curPos.coord[1]);
		array[2] = (Single)(bgiPtr.curPos.coord[2] + bgi_FLOOR_DEF.curPos.coord[2]);
		List<BGI_VEC_DEF> vertexList = bgi.vertexList;
		array2[0] = (Single)vertexList[(Int32)srcTri.vertexNdx[0]].coord[0];
		array3[0] = (Single)vertexList[(Int32)srcTri.vertexNdx[1]].coord[0];
		array4[0] = (Single)vertexList[(Int32)srcTri.vertexNdx[2]].coord[0];
		array5[0] = charPtr.curPos.x - array[0];
		array2[1] = (Single)vertexList[(Int32)srcTri.vertexNdx[0]].coord[1];
		array3[1] = (Single)vertexList[(Int32)srcTri.vertexNdx[1]].coord[1];
		array4[1] = (Single)vertexList[(Int32)srcTri.vertexNdx[2]].coord[1];
		array5[1] = charPtr.curPos.y - array[1];
		array2[2] = (Single)vertexList[(Int32)srcTri.vertexNdx[0]].coord[2];
		array3[2] = (Single)vertexList[(Int32)srcTri.vertexNdx[1]].coord[2];
		array4[2] = (Single)vertexList[(Int32)srcTri.vertexNdx[2]].coord[2];
		array5[2] = charPtr.curPos.z - array[2];
		Single num = (array3[0] - array2[0]) * (array4[2] - array2[2]);
		num -= (array4[0] - array2[0]) * (array3[2] - array2[2]);
		array6[0] = (array3[0] - array5[0]) * (array4[2] - array5[2]);
		array6[0] -= (array4[0] - array5[0]) * (array3[2] - array5[2]);
		array6[1] = (array4[0] - array5[0]) * (array2[2] - array5[2]);
		array6[1] -= (array2[0] - array5[0]) * (array4[2] - array5[2]);
		array6[2] = (array2[0] - array5[0]) * (array3[2] - array5[2]);
		array6[2] -= (array3[0] - array5[0]) * (array2[2] - array5[2]);
		bgi_FLOOR_DEF = bgi.floorList[(Int32)dstTri.floorNdx];
		array[0] = (Single)(bgiPtr.curPos.coord[0] + bgi_FLOOR_DEF.curPos.coord[0]);
		array[1] = (Single)(bgiPtr.curPos.coord[1] + bgi_FLOOR_DEF.curPos.coord[1]);
		array[2] = (Single)(bgiPtr.curPos.coord[2] + bgi_FLOOR_DEF.curPos.coord[2]);
		array2[0] = (Single)vertexList[(Int32)dstTri.vertexNdx[0]].coord[0];
		array3[0] = (Single)vertexList[(Int32)dstTri.vertexNdx[1]].coord[0];
		array4[0] = (Single)vertexList[(Int32)dstTri.vertexNdx[2]].coord[0];
		array2[1] = (Single)vertexList[(Int32)dstTri.vertexNdx[0]].coord[1];
		array3[1] = (Single)vertexList[(Int32)dstTri.vertexNdx[1]].coord[1];
		array4[1] = (Single)vertexList[(Int32)dstTri.vertexNdx[2]].coord[1];
		array2[2] = (Single)vertexList[(Int32)dstTri.vertexNdx[0]].coord[2];
		array3[2] = (Single)vertexList[(Int32)dstTri.vertexNdx[1]].coord[2];
		array4[2] = (Single)vertexList[(Int32)dstTri.vertexNdx[2]].coord[2];
		array7[0] = (array6[0] * array2[0] + array6[1] * array3[0] + array6[2] * array4[0]) / num;
		array7[2] = (array6[0] * array2[2] + array6[1] * array3[2] + array6[2] * array4[2]) / num;
		charPtr.curPos.x = array7[0] + array[0];
		charPtr.curPos.z = array7[2] + array[2];
		if (dstTri.normalNdx >= 0)
		{
			BGI_FVEC_DEF n = bgi.normalList[(Int32)dstTri.normalNdx];
			this.BGI_computeHeight(ref charPtr.curPos, n, dstTri.d);
			charPtr.curPos.y = charPtr.curPos.y + (Single)(bgi.floorList[(Int32)dstTri.floorNdx].curPos.coord[1] - bgi.floorList[(Int32)dstTri.floorNdx].orgPos.coord[1]);
		}
		else
		{
			charPtr.curPos.y = (Single)((Int32)this.BGI_retrieveFlatTriHeight(dstTri));
		}
		return 1;
	}

	private Int32 BGI_animUpdateCharacters(BGI_DEF bgiPtr, BGI_ANM_DEF anmPtr, BGI_FRAME_DEF framePtr, Int16 lastFrameNdx, UInt16 frameNdx)
	{
		if (lastFrameNdx == -1)
		{
			return 0;
		}
		BGI_FRAME_DEF bgi_FRAME_DEF = anmPtr.frameList[(Int32)lastFrameNdx];
		List<Int32> triIdxList = framePtr.triIdxList;
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		for (ObjList objList = instance.GetActiveObjList(); objList != null; objList = objList.next)
		{
			Obj obj = objList.obj;
			if (obj.cid == 4)
			{
				Actor actor = (Actor)obj;
				FieldMapActorController fieldMapActorController = actor.fieldMapActorController;
				if ((fieldMapActorController.charFlags & 1) != 0)
				{
					for (UInt16 num = 0; num < bgi_FRAME_DEF.triCount; num = (UInt16)(num + 1))
					{
						Int32 num2 = bgi_FRAME_DEF.triIdxList[(Int32)num];
						if (fieldMapActorController.activeTri == num2)
						{
							this.BGI_animApplyBarycentric(bgiPtr, fieldMapActorController, this.fieldMap.bgi.triList[num2], this.fieldMap.bgi.triList[triIdxList[(Int32)num]]);
							fieldMapActorController.activeTri = (Int32)((Int16)triIdxList[(Int32)num]);
							fieldMapActorController.activeFloor = (Int32)((Int16)this.fieldMap.walkMesh.tris[triIdxList[(Int32)num]].floorIdx);
						}
					}
				}
			}
		}
		return 1;
	}

	private Int32 BGI_animUpdateNeighbors(BGI_DEF bgiPtr, BGI_ANM_DEF anmPtr, BGI_FRAME_DEF framePtr)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		List<Int32> triIdxList = framePtr.triIdxList;
		for (UInt16 num = 0; num < framePtr.triCount; num = (UInt16)(num + 1))
		{
			BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[triIdxList[(Int32)num]];
			for (UInt16 num2 = 0; num2 < 3; num2 = (UInt16)(num2 + 1))
			{
				if (bgi_TRI_DEF.neighborNdx[(Int32)num2] >= 0)
				{
					BGI_TRI_DEF bgi_TRI_DEF2 = bgi.triList[(Int32)bgi_TRI_DEF.neighborNdx[(Int32)num2]];
					if (bgi_TRI_DEF.floorNdx != bgi_TRI_DEF2.floorNdx)
					{
						BGI_EDGE_DEF bgi_EDGE_DEF = bgi.edgeList[(Int32)bgi_TRI_DEF.edgeNdx[(Int32)num2]];
						if (bgi_EDGE_DEF.edgeClone >= 0)
						{
							UInt16 num3 = (UInt16)bgi_EDGE_DEF.edgeClone;
							bgi_TRI_DEF2.neighborNdx[(Int32)num3] = (Int16)triIdxList[(Int32)num];
							bgi_EDGE_DEF = bgi.edgeList[(Int32)bgi_TRI_DEF2.edgeNdx[(Int32)num3]];
							bgi_EDGE_DEF.edgeClone = (Int16)num2;
						}
					}
				}
			}
		}
		return 1;
	}

	public Int32 BGI_simInit()
	{
		this.BGISimList = new BGI_SIM_DEF[30];
		for (Int32 i = 0; i < 30; i++)
		{
			this.BGISimList[i] = new BGI_SIM_DEF();
			this.BGISimList[i].simFlags = 2;
			this.BGISimList[i].floorNdx = 0;
			this.BGISimList[i].axisNdx = 0;
			this.BGISimList[i].algorithmType = 0;
			this.BGISimList[i].frameCount = 0;
			this.BGISimList[i].frameNdx = 0;
			this.BGISimList[i].frameRate = 256;
			this.BGISimList[i].deltaMin = 0;
			this.BGISimList[i].deltaMax = 0;
			this.BGISimList[i].deltaCur = 0;
			this.BGISimList[i].deltaPrev = 0;
		}
		return 1;
	}

	public Int32 BGI_simService()
	{
		for (Int32 i = 0; i < 30; i++)
		{
			BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[i];
			if ((bgi_SIM_DEF.simFlags & 1) != 0 && (bgi_SIM_DEF.simFlags & 12) != 0)
			{
				BGI_FLOOR_DEF bgi_FLOOR_DEF = this.fieldMap.bgi.floorList[(Int32)bgi_SIM_DEF.floorNdx];
				bgi_FLOOR_DEF.curPos.coord[(Int32)bgi_SIM_DEF.axisNdx] = (Int16)(bgi_FLOOR_DEF.orgPos.coord[(Int32)bgi_SIM_DEF.axisNdx] - bgi_SIM_DEF.deltaCur);
				EventEngine instance = PersistenSingleton<EventEngine>.Instance;
				for (ObjList objList = instance.GetActiveObjList(); objList != null; objList = objList.next)
				{
					Obj obj = objList.obj;
					if (obj.cid == 4)
					{
						FieldMapActorController fieldMapActorController = ((Actor)obj).fieldMapActorController;
						if ((fieldMapActorController.charFlags & 1) != 0 && fieldMapActorController.activeFloor == (Int32)bgi_SIM_DEF.floorNdx && fieldMapActorController.activeTri >= 0)
						{
							FieldMapActorController fieldMapActorController2 = fieldMapActorController;
							Int32 axisNdx;
							Int32 index = axisNdx = (Int32)bgi_SIM_DEF.axisNdx;
							Single num = fieldMapActorController2.curPos[axisNdx];
							fieldMapActorController2.curPos[index] = num - (Single)(bgi_SIM_DEF.deltaCur - bgi_SIM_DEF.deltaPrev);
							switch (bgi_SIM_DEF.axisNdx)
							{
							case 0:
								fieldMapActorController.lastPos.x = fieldMapActorController.curPos.x;
								break;
							case 1:
								fieldMapActorController.lastPos.y = fieldMapActorController.curPos.y;
								break;
							case 2:
								fieldMapActorController.lastPos.z = fieldMapActorController.curPos.z;
								break;
							}
						}
					}
				}
				this.BGI_simEvaluate(i);
			}
		}
		return 1;
	}

	private Int32 BGI_simEvaluate(Int32 simNdx)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[simNdx];
		bgi_SIM_DEF.frameNdx += (Int32)bgi_SIM_DEF.frameRate;
		Int32 num = bgi_SIM_DEF.frameNdx >> 8;
		Int32 num2 = (Int32)(bgi_SIM_DEF.deltaMax - bgi_SIM_DEF.deltaMin);
		if (bgi_SIM_DEF.frameCount > 0 && bgi_SIM_DEF.frameRate != 0 && num2 > 0)
		{
			UInt16 algorithmType = bgi_SIM_DEF.algorithmType;
			Int32 num3;
			if (algorithmType != 0)
			{
				if (algorithmType != 1)
				{
					num3 = (Int32)((Int16)((Int32)bgi_SIM_DEF.deltaMin + num));
				}
				else
				{
					Int32 a = (Int32)((Int16)((num << 12) / num2) + 2048);
					Int32 num4 = (Int32)(BGI.BGI_rcos(a) + 4096f);
					num3 = (Int32)((Int16)((Single)bgi_SIM_DEF.deltaMin + (Single)(num2 * num4) / 8192f));
				}
			}
			else
			{
				num3 = (Int32)((Int16)((Int32)bgi_SIM_DEF.deltaMin + num));
			}
			bgi_SIM_DEF.deltaPrev = bgi_SIM_DEF.deltaCur;
			bgi_SIM_DEF.deltaCur = (Int16)num3;
			if (bgi_SIM_DEF.deltaCur > bgi_SIM_DEF.deltaMax)
			{
				if ((bgi_SIM_DEF.simFlags & 16) != 0)
				{
					bgi_SIM_DEF.deltaCur = bgi_SIM_DEF.deltaMax;
					bgi_SIM_DEF.frameRate = (Int16)(-bgi_SIM_DEF.frameRate);
				}
				else
				{
					bgi_SIM_DEF.deltaCur = bgi_SIM_DEF.deltaMin;
				}
				bgi_SIM_DEF.frameNdx = num2 - 1 << 8;
				BGI_SIM_DEF bgi_SIM_DEF2 = bgi_SIM_DEF;
				bgi_SIM_DEF2.simFlags = (UInt16)(bgi_SIM_DEF2.simFlags & 65531);
			}
			else if (bgi_SIM_DEF.deltaCur < bgi_SIM_DEF.deltaMin)
			{
				if ((bgi_SIM_DEF.simFlags & 16) != 0)
				{
					bgi_SIM_DEF.deltaCur = bgi_SIM_DEF.deltaMin;
					bgi_SIM_DEF.frameRate = (Int16)(-bgi_SIM_DEF.frameRate);
				}
				else
				{
					bgi_SIM_DEF.deltaCur = bgi_SIM_DEF.deltaMax;
				}
				bgi_SIM_DEF.frameNdx = 0;
				BGI_SIM_DEF bgi_SIM_DEF3 = bgi_SIM_DEF;
				bgi_SIM_DEF3.simFlags = (UInt16)(bgi_SIM_DEF3.simFlags & 65531);
			}
			return 1;
		}
		return 0;
	}

	public Int32 BGI_simSetFloor(UInt32 simNdx, UInt32 floorNdx)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		bgi_SIM_DEF.floorNdx = (UInt16)floorNdx;
		return 1;
	}

	public Int32 BGI_simSetActive(UInt32 simNdx, UInt32 isActive)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		if (isActive != 0u)
		{
			BGI_SIM_DEF bgi_SIM_DEF2 = bgi_SIM_DEF;
			bgi_SIM_DEF2.simFlags = (UInt16)(bgi_SIM_DEF2.simFlags | 5);
		}
		else
		{
			BGI_SIM_DEF bgi_SIM_DEF3 = bgi_SIM_DEF;
			bgi_SIM_DEF3.simFlags = (UInt16)(bgi_SIM_DEF3.simFlags & 65530);
		}
		this.BGI_simEvaluate((Int32)simNdx);
		return 1;
	}

	public Int32 BGI_simSetFlags(UInt32 simNdx, UInt32 flags)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		BGI_SIM_DEF bgi_SIM_DEF2 = bgi_SIM_DEF;
		bgi_SIM_DEF2.simFlags = (UInt16)(bgi_SIM_DEF2.simFlags & 65511);
		BGI_SIM_DEF bgi_SIM_DEF3 = bgi_SIM_DEF;
		bgi_SIM_DEF3.simFlags = (UInt16)(bgi_SIM_DEF3.simFlags | (UInt16)(flags & 24u));
		return 1;
	}

	public Int32 BGI_simSetFrameRate(UInt32 simNdx, Int16 frameRate)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		bgi_SIM_DEF.frameRate = frameRate;
		return 1;
	}

	public Int32 BGI_simSetAlgorithm(UInt32 simNdx, UInt32 algorithmType)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		bgi_SIM_DEF.algorithmType = (UInt16)algorithmType;
		return 1;
	}

	public Int32 BGI_simSetDelta(UInt32 simNdx, Int16 deltaMin, Int16 deltaMax)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		bgi_SIM_DEF.deltaMin = deltaMin;
		bgi_SIM_DEF.deltaMax = deltaMax;
		bgi_SIM_DEF.frameCount = (UInt16)(bgi_SIM_DEF.deltaMax - bgi_SIM_DEF.deltaMin);
		return 1;
	}

	public Int32 BGI_simSetAxis(UInt32 simNdx, UInt32 axisNdx)
	{
		BGI_SIM_DEF bgi_SIM_DEF = this.BGISimList[(Int32)((UIntPtr)simNdx)];
		bgi_SIM_DEF.axisNdx = (UInt16)axisNdx;
		return 1;
	}

	private void BGI_privateInit()
	{
		this.BGIForceCount = 0;
		this.BGIForces = new Vector3[16];
		for (Int32 i = 0; i < 16; i++)
		{
			this.BGIForces[i] = Vector3.zero;
		}
		this.BGIForceType = new Boolean[16];
		this.BGIForceDist = new Single[16];
		this.attributeMask = Byte.MaxValue;
	}

	private void BGI_resetPos(BGI_CHAR_DEF charDef)
	{
		charDef.charPos = charDef.lastPos;
	}

	private void BGI_triVisitClear()
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		UInt16 num = 128;
		for (Int32 i = 0; i < (Int32)bgi.triCount; i++)
		{
			BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[i];
			BGI_TRI_DEF bgi_TRI_DEF2 = bgi_TRI_DEF;
			bgi_TRI_DEF2.triFlags = (UInt16)(bgi_TRI_DEF2.triFlags & (UInt16)(~num));
		}
	}

	private void BGI_reverseRotatePoint(Single mag, Single magPrime, Vector3 lastPerp, out Vector3 vec)
	{
		Vector3 vector;
		vector.x = lastPerp.x * magPrime / mag;
		vector.y = 0f;
		vector.z = lastPerp.z * magPrime / mag;
		vec.x = vector.z;
		vec.y = 0f;
		vec.z = -vector.x;
	}

	private void BGI_calcFlatCharPosition(BGI_TRI_DEF tri, Vector3 P, ref Vector3 Q, Vector3 perp)
	{
		Vector3 b = Q - P;
		Single num = perp.x * perp.x + perp.z * perp.z;
		num = Mathf.Sqrt(num);
		if (Mathf.Approximately(num, 0f))
		{
			Q.y = P.y;
			return;
		}
		Single num2 = b.sqrMagnitude;
		num2 = Mathf.Sqrt(num2);
		this.BGI_reverseRotatePoint(num, num2, perp, out b);
		Q = P + b;
	}

	private Boolean BGI_radiusValid(BGI_CHAR_DEF charDef, Vector3 cPos, BGI_TRI_DEF tri, out Int32 edgeNdx)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		edgeNdx = 0;
		Vector3[] array = new Vector3[2];
		Vector3[] array2 = new Vector3[3];
		Vector3 zero = Vector3.zero;
		BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(Int32)tri.floorNdx];
		Vector3 a = bgi.curPos.ToVector3();
		Vector3 b = bgi_FLOOR_DEF.curPos.ToVector3();
		Vector3 a2 = a + b;
		for (Int32 i = 0; i < 3; i++)
		{
			array2[i] = a2 + bgi.vertexList[(Int32)tri.vertexNdx[i]].ToVector3();
		}
		array[0] = array2[2];
		Boolean flag = false;
		for (Int32 j = 0; j < 3; j++)
		{
			array[1] = array2[j];
			Boolean flag2;
			Single num = this.BGI_distanceToLine(cPos, array[0], array[1], out a2, out flag2);
			if (num <= charDef.charRadiusSquared)
			{
				BGI_TRI_DEF bgi_TRI_DEF = this.BGI_findAccessibleTriangle(charDef, tri, j);
				if (bgi_TRI_DEF != null)
				{
					if ((bgi_TRI_DEF.triFlags & 128) == 0)
					{
						BGI_TRI_DEF bgi_TRI_DEF2 = bgi_TRI_DEF;
						bgi_TRI_DEF2.triFlags = (UInt16)(bgi_TRI_DEF2.triFlags | 128);
						if (!this.BGI_radiusValid(charDef, cPos, bgi_TRI_DEF, out edgeNdx))
						{
							edgeNdx = j;
							flag = true;
						}
					}
				}
				else if (Mathf.Approximately(num, 0f))
				{
					flag = true;
				}
				else
				{
					num = Mathf.Sqrt(num);
					this.BGI_computeNewPoint(a2.x, cPos.x, charDef.charRadius, num, out zero.x);
					this.BGI_computeNewPoint(a2.z, cPos.z, charDef.charRadius, num, out zero.z);
					if (this.BGIForceCount + 1 < 16)
					{
						zero.x = a2.x - cPos.x + zero.x;
						zero.z = a2.z - cPos.z + zero.z;
						this.BGIForces[this.BGIForceCount].x = zero.x;
						this.BGIForces[this.BGIForceCount].z = zero.z;
						this.BGIForceType[this.BGIForceCount] = flag2;
						this.BGIForceDist[this.BGIForceCount] = num;
						this.BGIForceCount++;
					}
					tri.triFlags = (UInt16)(tri.triFlags | 128);
					edgeNdx = j;
					flag = true;
				}
			}
			array[0] = array[1];
		}
		return !flag;
	}

	private Boolean BGI_isRadiusValid(BGI_CHAR_DEF charDef, Vector3 cPos, BGI_TRI_DEF tri, out Int32 edgeNdx)
	{
		this.BGI_triVisitClear();
		this.BGIForceCount = 0;
		return this.BGI_radiusValid(charDef, cPos, tri, out edgeNdx);
	}

	private Boolean BGI_scanForValidTriangle(BGI_CHAR_DEF charDef, ref BGI_TRI_DEF tri, Vector3 cPos, Int32 depth)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		if (depth > 8)
		{
			return false;
		}
		BGI_TRI_DEF[] array = new BGI_TRI_DEF[2];
		array[0] = tri;
		if (this.BGI_pointInPoly(array[0], cPos))
		{
			return true;
		}
		for (Int32 i = 0; i < 3; i++)
		{
			array[1] = this.BGI_findAccessibleTriangle(charDef, array[0], i);
			if (array[1] != null && this.BGI_pointInPoly(array[1], cPos))
			{
				charDef.activeTri = array[0].neighborNdx[i];
				if (charDef.activeTri >= 0)
				{
					array[1] = bgi.triList[(Int32)charDef.activeTri];
					charDef.activeFloor = array[1].floorNdx;
					tri = array[1];
					return true;
				}
			}
		}
		for (Int32 j = 0; j < 3; j++)
		{
			array[1] = this.BGI_findAccessibleTriangle(charDef, array[0], j);
			if (array[1] != null && (array[1].triFlags & 128) == 0)
			{
				BGI_TRI_DEF bgi_TRI_DEF = array[1];
				bgi_TRI_DEF.triFlags = (UInt16)(bgi_TRI_DEF.triFlags | 128);
				Boolean flag = this.BGI_scanForValidTriangle(charDef, ref array[1], cPos, depth + 1);
				if (flag)
				{
					tri = array[1];
					return true;
				}
			}
		}
		return false;
	}

	private void BGI_extendVector(Vector3 pntA, ref Vector3 pntB, ref Single fracX, ref Single fracZ)
	{
		fracX = 0f;
		fracZ = 0f;
		Single num = pntB.x - pntA.x;
		Single num2 = pntB.z - pntA.z;
		if (Mathf.CeilToInt(num) != Mathf.FloorToInt(num))
		{
			num = Mathf.Ceil(num);
		}
		else
		{
			num += 1f;
		}
		if (Mathf.CeilToInt(num2) != Mathf.FloorToInt(num2))
		{
			num2 = Mathf.Ceil(num2);
		}
		else
		{
			num2 += 1f;
		}
		pntB.x = pntA.x + num;
		pntB.z = pntA.z + num2;
	}

	private Single BGI_distanceToLine(Vector3 p, Vector3 a, Vector3 b, out Vector3 intPnt, out Boolean isPerpDist)
	{
		intPnt = Vector3.zero;
		isPerpDist = false;
		Single num = Vector3.Dot(p - a, b - a);
		Single num2 = Vector3.Dot(p - b, a - b);
		Vector3 vector = Vector3.zero;
		Single num3;
		if (num <= 0f)
		{
			vector = a;
			if (Mathf.Approximately(num, 0f))
			{
				isPerpDist = true;
			}
			else
			{
				isPerpDist = false;
			}
		}
		else if (num2 <= 0f)
		{
			vector = b;
			if (Mathf.Approximately(num2, 0f))
			{
				isPerpDist = true;
			}
			else
			{
				isPerpDist = false;
			}
		}
		else
		{
			num3 = num + num2;
			vector = a + (b - a) * num / num3;
			isPerpDist = true;
		}
		num3 = (vector - p).sqrMagnitude;
		intPnt = vector;
		return num3;
	}

	private Boolean BGI_quickCrossingEdge(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Int32 C1, Int32 C2)
	{
		Single num = (B[C1] - A[C1]) * (D[C2] - C[C2]);
		num -= (B[C2] - A[C2]) * (D[C1] - C[C1]);
		Single num2 = (A[C2] - C[C2]) * (D[C1] - C[C1]);
		num2 -= (A[C1] - C[C1]) * (D[C2] - C[C2]);
		if (Mathf.Abs(num2) <= Mathf.Abs(num) && ((num2 >= 0f && num >= 0f) || (num2 <= 0f && num < 0f)))
		{
			Single num3 = (A[C2] - C[C2]) * (B[C1] - A[C1]);
			num3 -= (A[C1] - C[C1]) * (B[C2] - A[C2]);
			if ((num3 >= 0f && num >= 0f) || (num3 <= 0f && num < 0f))
			{
				return true;
			}
		}
		return false;
	}

	private BGI_TRI_DEF BGI_findAccessibleTriangle(BGI_CHAR_DEF charDef, BGI_TRI_DEF tri, Int32 ndx)
	{
		if (tri.neighborNdx[ndx] == -1)
		{
			return (BGI_TRI_DEF)null;
		}
		BGI_TRI_DEF bgi_TRI_DEF = this.fieldMap.bgi.triList[(Int32)tri.neighborNdx[ndx]];
		if ((bgi_TRI_DEF.triFlags & 1) == 0)
		{
			return (BGI_TRI_DEF)null;
		}
		Byte b = (Byte)(bgi_TRI_DEF.triFlags >> 8 & (Int32)this.attributeMask);
		if ((b & 192) == 0)
		{
			return bgi_TRI_DEF;
		}
		if ((b & 128) != 0)
		{
			return (BGI_TRI_DEF)null;
		}
		return bgi_TRI_DEF;
	}

	public Single BGI_retrieveFlatTriHeight(BGI_TRI_DEF tri)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		Vector3 vector = bgi.curPos.ToVector3();
		Vector3 vector2 = bgi.floorList[(Int32)tri.floorNdx].curPos.ToVector3();
		Vector3 vector3 = bgi.vertexList[(Int32)tri.vertexNdx[0]].ToVector3();
		return vector.y + vector2.y + vector3.y;
	}

	public void BGI_computeNewPoint(Single a, Single b, Single r, Single d, out Single n)
	{
		if (Mathf.Approximately(a, b))
		{
			n = 0f;
		}
		else
		{
			n = (b - a) * r / d;
		}
	}

	public void BGI_computeHeight(ref Vector3 v, BGI_FVEC_DEF n, Int32 d)
	{
		Single num = Math3D.Fixed2Float(n.coord[0]) * v.x;
		Single num2 = Math3D.Fixed2Float(n.coord[2]) * v.z;
		Single num3 = Math3D.Fixed2Float(d) - num - num2;
		v.y = num3 * Math3D.Fixed2Float(n.oneOverY * -1);
	}

	private void BGI_rotatePoint(Vector3 nrm, ref Vector3 vec, out Vector3 lastPerp)
	{
		Vector3 vector = vec;
		Vector3 vector2;
		vector2.x = -vector.z;
		vector2.y = 0f;
		vector2.z = vector.x;
		lastPerp.x = -vec.z;
		lastPerp.y = 0f;
		lastPerp.z = vec.x;
		nrm.y *= -1f;
		if (nrm.y >= 0f)
		{
			vector.x = nrm.y * vector2.z;
			vector.y = nrm.z * vector2.x - nrm.x * vector2.z;
			vector.z = -(nrm.y * vector2.x);
		}
		else
		{
			vector.x = -(nrm.y * vector2.z);
			vector.y = nrm.x * vector2.z - nrm.z * vector2.x;
			vector.z = nrm.y * vector2.x;
		}
		vec = vector;
	}

	private void BGI_calcSlopedCharPosition(BGI_TRI_DEF tri, Vector3 P, ref Vector3 Q, out Vector3 perp)
	{
		Vector3 b = Q - P;
		if (tri.normalNdx >= 0)
		{
			BGI_FVEC_DEF bgi_FVEC_DEF = this.fieldMap.bgi.normalList[(Int32)tri.normalNdx];
			Vector3 vector = bgi_FVEC_DEF.ToVector3();
			Single num = Vector3.Dot(vector, Vector3.up);
			if (num >= 0.9f)
			{
				this.BGI_rotatePoint(Vector3.up, ref b, out perp);
			}
			else
			{
				this.BGI_rotatePoint(vector, ref b, out perp);
			}
		}
		else
		{
			Vector3 up = Vector3.up;
			this.BGI_rotatePoint(up, ref b, out perp);
		}
		Q = P + b;
	}

	private Boolean BGI_traverseTriangles(BGI_CHAR_DEF charDef, ref BGI_TRI_DEF tri, Vector3 oldCPos, Vector3 cPos, Int32 recurseDepth, Single fracX, Single fracZ)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		Vector3 p = oldCPos;
		Vector3 vector = cPos;
		Int32 num = -1;
		Int32 num2 = -1;
		Int16 activeTri = charDef.activeTri;
		Boolean flag = false;
		BGI_TRI_DEF bgi_TRI_DEF = tri;
		while (!flag)
		{
			Vector3 perp;
			this.BGI_calcSlopedCharPosition(bgi_TRI_DEF, p, ref cPos, out perp);
			Boolean flag2 = this.BGI_pointInPoly(bgi_TRI_DEF, cPos);
			if (flag2)
			{
				flag = true;
			}
			else
			{
				flag2 = this.BGI_findCrossingEdge(bgi_TRI_DEF, cPos, oldCPos, out p, out num2, num2);
				if (flag2)
				{
					BGI_TRI_DEF bgi_TRI_DEF2 = this.BGI_findAccessibleTriangle(charDef, bgi_TRI_DEF, num2);
					if (bgi_TRI_DEF2 == null)
					{
						charDef.activeTri = activeTri;
						bgi_TRI_DEF = bgi.triList[(Int32)charDef.activeTri];
						charDef.activeFloor = bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
					if (bgi_TRI_DEF.normalNdx >= 0)
					{
						this.BGI_calcFlatCharPosition(bgi_TRI_DEF, p, ref cPos, perp);
					}
					if ((Int32)bgi_TRI_DEF.neighborNdx[num2] == num)
					{
						charDef.activeTri = activeTri;
						bgi_TRI_DEF = bgi.triList[(Int32)charDef.activeTri];
						charDef.activeFloor = bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
					num = (Int32)charDef.activeTri;
					charDef.activeTri = bgi_TRI_DEF.neighborNdx[num2];
					BGI_EDGE_DEF bgi_EDGE_DEF = bgi.edgeList[(Int32)bgi_TRI_DEF.edgeNdx[num2]];
					bgi_TRI_DEF = bgi.triList[(Int32)charDef.activeTri];
					charDef.activeFloor = bgi_TRI_DEF.floorNdx;
					num2 = (Int32)bgi_EDGE_DEF.edgeClone;
					if (num2 == -1)
					{
						charDef.activeTri = activeTri;
						bgi_TRI_DEF = bgi.triList[(Int32)charDef.activeTri];
						charDef.activeFloor = bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
				}
				else
				{
					this.BGI_triVisitClear();
					flag2 = this.BGI_scanForValidTriangle(charDef, ref bgi_TRI_DEF, cPos, 0);
					if (!flag2)
					{
						if (recurseDepth < 6)
						{
							cPos = vector;
							this.BGI_extendVector(oldCPos, ref cPos, ref fracX, ref fracZ);
							charDef.charPos = cPos;
							charDef.activeTri = activeTri;
							bgi_TRI_DEF = bgi.triList[(Int32)charDef.activeTri];
							charDef.activeFloor = bgi_TRI_DEF.floorNdx;
							if (!this.BGI_traverseTriangles(charDef, ref bgi_TRI_DEF, oldCPos, cPos, recurseDepth + 1, fracX, fracZ))
							{
								tri = bgi_TRI_DEF;
								return false;
							}
						}
						charDef.activeTri = activeTri;
						bgi_TRI_DEF = bgi.triList[(Int32)charDef.activeTri];
						charDef.activeFloor = bgi_TRI_DEF.floorNdx;
						tri = bgi_TRI_DEF;
						return true;
					}
					flag = true;
				}
			}
		}
		tri = bgi_TRI_DEF;
		return false;
	}

	private Boolean BGI_serviceForces(BGI_CHAR_DEF charDef, ref Vector3 cPos, ref BGI_TRI_DEF tri, ref Int32 edgeNdx)
	{
		if (this.BGIForceCount == 0)
		{
			return true;
		}
		Vector3 oldCPos = cPos;
		if (this.BGIForceCount == 1)
		{
			cPos.x += this.BGIForces[0].x;
			cPos.z += this.BGIForces[0].z;
			charDef.charPos.x = cPos.x;
			charDef.charPos.z = cPos.z;
		}
		else
		{
			Vector3 zero = Vector3.zero;
			Boolean flag = false;
			Int32 num = 0;
			for (Int32 i = 0; i < this.BGIForceCount; i++)
			{
				if (this.BGIForceType[i])
				{
					Boolean flag2 = false;
					for (Int32 j = 0; j < i; j++)
					{
						if (Mathf.Approximately(this.BGIForces[j].x, this.BGIForces[i].x) && Mathf.Approximately(this.BGIForces[j].z, this.BGIForces[i].z))
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						flag = true;
						zero.x += this.BGIForces[i].x;
						zero.z += this.BGIForces[i].z;
						num++;
					}
				}
			}
			if (flag)
			{
				if (num > 1)
				{
					zero.x /= (Single)num;
					zero.z /= (Single)num;
				}
			}
			else
			{
				num = 0;
				for (Int32 k = 0; k < this.BGIForceCount; k++)
				{
					Boolean flag2 = false;
					for (Int32 l = 0; l < k; l++)
					{
						if (Mathf.Approximately(this.BGIForces[l].x, this.BGIForces[k].x) && Mathf.Approximately(this.BGIForces[l].z, this.BGIForces[k].z))
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						num++;
						zero.x += this.BGIForces[k].x;
						zero.z += this.BGIForces[k].z;
					}
				}
				if (num > 0)
				{
					zero.x /= (Single)num;
					zero.z /= (Single)num;
				}
			}
			cPos.x += zero.x;
			cPos.z += zero.z;
			charDef.charPos.x = cPos.x;
			charDef.charPos.z = cPos.z;
		}
		this.BGIForceCount = 0;
		return !this.BGI_traverseTriangles(charDef, ref tri, oldCPos, cPos, 0, 0f, 0f);
	}

	private Int32 BGI_serviceChar(BGI_CHAR_DEF charDef)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		Int32 num = 2;
		Vector3 charPos = charDef.charPos;
		Vector3 lastPos = charDef.lastPos;
		if (Mathf.Approximately(charPos.x, lastPos.x) && Mathf.Approximately(charPos.y, lastPos.y) && Mathf.Approximately(charPos.z, lastPos.z))
		{
			num |= 4;
			charDef.lastFloor = charDef.activeFloor;
			charDef.lastTri = charDef.activeTri;
			return num;
		}
		Int16 activeTri = charDef.activeTri;
		BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[(Int32)activeTri];
		if (this.BGI_traverseTriangles(charDef, ref bgi_TRI_DEF, lastPos, charPos, 0, 0f, 0f))
		{
			charDef.activeTri = activeTri;
			if (charDef.activeTri >= 0)
			{
				charDef.activeFloor = bgi.triList[(Int32)charDef.activeTri].floorNdx;
			}
			else
			{
				charDef.activeFloor = -1;
			}
			global::Debug.Log(String.Concat(new Object[]
			{
				"000BGI_serviceChar: from : ",
				charDef.charPos,
				", to : ",
				charDef.lastPos,
				" activeTri : ",
				charDef.activeTri
			}));
			this.DebugRenderTri(bgi.triList[(Int32)charDef.activeTri]);
			this.BGI_resetPos(charDef);
			num |= 1;
			if (charDef.lastTri >= 0 && charDef.lastFloor >= 0 && !this.BGI_pointInPoly(bgi.triList[(Int32)charDef.lastTri], lastPos))
			{
				charDef.lastFloor = charDef.activeFloor;
				charDef.lastTri = charDef.activeTri;
			}
			return num;
		}
		Int32 num2;
		if (!this.BGI_isRadiusValid(charDef, charPos, bgi_TRI_DEF, out num2))
		{
			Boolean flag = this.BGI_serviceForces(charDef, ref charPos, ref bgi_TRI_DEF, ref num2);
			if (!flag)
			{
				charDef.activeTri = activeTri;
				if (charDef.activeTri >= 0)
				{
					charDef.activeFloor = bgi.triList[(Int32)charDef.activeTri].floorNdx;
				}
				else
				{
					charDef.activeFloor = -1;
				}
				global::Debug.Log(String.Concat(new Object[]
				{
					"111BGI_serviceChar: from : ",
					charDef.charPos,
					", to : ",
					charDef.lastPos,
					" activeTri : ",
					charDef.activeTri
				}));
				this.DebugRenderTri(bgi.triList[(Int32)charDef.activeTri]);
				this.BGI_resetPos(charDef);
				num |= 1;
				if (charDef.lastTri >= 0 && charDef.lastFloor >= 0 && !this.BGI_pointInPoly(bgi.triList[(Int32)charDef.lastTri], lastPos))
				{
					charDef.lastFloor = charDef.activeFloor;
					charDef.lastTri = charDef.activeTri;
				}
				return num;
			}
			num |= 9;
		}
		if (!Mathf.Approximately(charPos.x, charDef.charPos.x) || !Mathf.Approximately(charPos.z, charDef.charPos.z))
		{
			charDef.charPos.x = charPos.x;
			charDef.charPos.z = charPos.z;
			num |= 16;
		}
		if (bgi_TRI_DEF.normalNdx >= 0)
		{
			BGI_FVEC_DEF bgi_FVEC_DEF = bgi.normalList[(Int32)bgi_TRI_DEF.normalNdx];
			if (bgi_FVEC_DEF.coord[1] > 20724 || bgi_FVEC_DEF.coord[1] < -20724)
			{
				this.BGI_computeHeight(ref charDef.charPos, bgi_FVEC_DEF, bgi_TRI_DEF.d);
				Vector3 vector = bgi.floorList[(Int32)bgi_TRI_DEF.floorNdx].curPos.ToVector3();
				Vector3 vector2 = bgi.floorList[(Int32)bgi_TRI_DEF.floorNdx].orgPos.ToVector3();
				charDef.charPos.y = charDef.charPos.y + (vector.y - vector2.y);
			}
			else
			{
				charDef.charPos.y = charPos.y;
			}
		}
		else
		{
			charDef.charPos.y = this.BGI_retrieveFlatTriHeight(bgi_TRI_DEF);
		}
		charDef.lastTri = activeTri;
		if (charDef.lastTri >= 0)
		{
			charDef.lastFloor = bgi.triList[(Int32)charDef.lastTri].floorNdx;
		}
		else
		{
			charDef.lastFloor = -1;
		}
		return num;
	}

	private void BGI_publicInit()
	{
		this.actors = new List<FieldMapActor>();
	}

	public Single BGI_interceptCompute(Single aWanted, Single aKnown, Single bWanted, Single bKnown, Single cKnown)
	{
		return bWanted - (bKnown - cKnown) * (aWanted - bWanted) / (aKnown - bKnown);
	}

	public void BGI_charAddPlayer(FieldMapActor actor)
	{
		BGI_CHAR_DEF charDef = actor.charDef;
		charDef.charFlags = 1;
		charDef.heading = 0;
		charDef.activeFloor = -1;
		charDef.activeTri = -1;
		charDef.charRadius = 96f;
		charDef.charRadiusSquared = 9216f;
		charDef.charNdx = this.actors.Count;
		charDef.talkRad = 16f;
		charDef.collRad = 16f;
		charDef.speed = 30f;
		this.BGI_charSetPosition(charDef, this.bgiCharPos.x, this.bgiCharPos.y, this.bgiCharPos.z);
		this.actors.Add(actor);
	}

	public void BGI_charAddNonPlayer(FieldMapActor actor)
	{
		BGI_CHAR_DEF charDef = actor.charDef;
		charDef.charFlags = 1;
		charDef.heading = 0;
		this.BGI_charSetPosition(charDef, this.bgiCharPos.x, this.bgiCharPos.y, this.bgiCharPos.z);
		this.actors.Add(actor);
	}

	public void BGI_charResetToLastPos(BGI_CHAR_DEF charDef)
	{
		charDef.charPos = charDef.lastPos;
		charDef.activeFloor = charDef.lastFloor;
		charDef.activeTri = charDef.lastTri;
	}

	public Int32 BGI_charService(Int32 charNdx)
	{
		Int32 num = 0;
		if (charNdx >= this.actors.Count)
		{
			return num | 16384;
		}
		BGI_CHAR_DEF charDef = this.actors[charNdx].charDef;
		if ((charDef.charFlags & 1) == 0)
		{
			return 0;
		}
		if (charDef.activeFloor != -1 && charDef.activeTri != -1)
		{
			return this.BGI_serviceChar(charDef);
		}
		Vector3 charPos = charDef.charPos;
		if (!this.BGI_charSetPosition(charDef, charPos.x, charPos.y, charPos.z))
		{
			return num & -3;
		}
		return 2;
	}

	public Boolean BGI_charSetPosition(BGI_CHAR_DEF charDef, Single x, Single y, Single z)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		Vector3 vector = new Vector3(x, y, z);
		Single num = 32767f;
		Boolean flag = false;
		Int16 activeFloor = -1;
		Int16 activeTri = -1;
		for (Int32 i = 0; i < (Int32)bgi.triCount; i++)
		{
			BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[i];
			Boolean flag2 = false;
			if (this.BGI_pointAbovePoly(bgi_TRI_DEF, vector))
			{
				this.BGI_triVisitClear();
				this.BGIForceCount = 0;
				Int32 num2;
				if (this.BGI_radiusValid(charDef, vector, bgi_TRI_DEF, out num2))
				{
					flag2 = true;
				}
				else if (this.BGI_serviceForces(charDef, ref vector, ref bgi_TRI_DEF, ref num2))
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (bgi_TRI_DEF.normalNdx >= 0)
					{
						BGI_FVEC_DEF n = bgi.normalList[(Int32)bgi_TRI_DEF.normalNdx];
						Vector3 vector2 = vector;
						this.BGI_computeHeight(ref vector2, n, bgi_TRI_DEF.d);
						Vector3 vector3 = bgi.floorList[(Int32)bgi_TRI_DEF.floorNdx].curPos.ToVector3();
						Vector3 vector4 = bgi.floorList[(Int32)bgi_TRI_DEF.floorNdx].orgPos.ToVector3();
						vector2.y += vector3.y - vector4.y;
						vector.y = vector2.y;
					}
					else
					{
						vector.y = this.BGI_retrieveFlatTriHeight(bgi_TRI_DEF);
					}
					if (vector.y >= y && vector.y < num)
					{
						flag = true;
						num = vector.y;
						activeFloor = bgi_TRI_DEF.floorNdx;
						activeTri = (Int16)i;
					}
				}
			}
		}
		if (flag)
		{
			charDef.activeFloor = activeFloor;
			charDef.activeTri = activeTri;
			charDef.charPos.x = x;
			charDef.charPos.y = num;
			charDef.charPos.z = z;
			return true;
		}
		return false;
	}

	public Boolean BGI_pointAbovePoly(BGI_TRI_DEF tri, Vector3 pos)
	{
		WalkMeshTriangle walkMeshTriangle = this.tris[tri.triIdx];
		Vector3 planarFactor = new Vector3(1f, 0f, 1f);
		return Math3D.PointInsideTriangleTest2D(pos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2], planarFactor);
	}

	public Boolean BGI_pointInPoly(BGI_TRI_DEF tri, Vector3 pos)
	{
		WalkMeshTriangle walkMeshTriangle = this.tris[tri.triIdx];
		Vector3 planarFactor = new Vector3(1f, 0f, 1f);
		if (tri.normalNdx >= 0)
		{
			BGI_FVEC_DEF bgi_FVEC_DEF = this.fieldMap.bgi.normalList[(Int32)tri.normalNdx];
			if (bgi_FVEC_DEF.coord[1] > 20724 || bgi_FVEC_DEF.coord[1] < -20724)
			{
				planarFactor = new Vector3(1f, 0f, 1f);
			}
			else if (Mathf.Abs(bgi_FVEC_DEF.coord[0]) > Mathf.Abs(bgi_FVEC_DEF.coord[2]))
			{
				planarFactor = new Vector3(0f, 1f, 1f);
			}
			else
			{
				planarFactor = new Vector3(1f, 1f, 0f);
			}
		}
		return Math3D.PointInsideTriangleTest2D(pos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2], planarFactor);
	}

	public Boolean BGI_findCrossingEdge(BGI_TRI_DEF tri, Vector3 cPos, Vector3 oldCPos, out Vector3 intPnt, out Int32 edgeNdx, Int32 edgeException)
	{
		BGI_DEF bgi = this.fieldMap.bgi;
		BGI_FLOOR_DEF bgi_FLOOR_DEF = bgi.floorList[(Int32)tri.floorNdx];
		intPnt = Vector3.zero;
		edgeNdx = -1;
		Int32 num = 0;
		Int32 num2 = 2;
		Int32 index = 1;
		if (tri.normalNdx >= 0)
		{
			BGI_FVEC_DEF bgi_FVEC_DEF = bgi.normalList[(Int32)tri.normalNdx];
			if (bgi_FVEC_DEF.coord[1] > 20724 || bgi_FVEC_DEF.coord[1] < -20724)
			{
				num = 0;
				num2 = 2;
				index = 1;
			}
			else if (Mathf.Abs(bgi_FVEC_DEF.coord[0]) > Mathf.Abs(bgi_FVEC_DEF.coord[2]))
			{
				num = 1;
				num2 = 2;
				index = 0;
			}
			else
			{
				num = 0;
				num2 = 1;
				index = 2;
			}
		}
		Vector3 a = bgi.curPos.ToVector3() + bgi_FLOOR_DEF.curPos.ToVector3();
		Vector3[] array = new Vector3[3];
		for (Int32 i = 0; i < 3; i++)
		{
			array[i] = a + bgi.vertexList[(Int32)tri.vertexNdx[i]].ToVector3();
		}
		Vector3[] array2 = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};
		Int32[] array3 = new Int32[3];
		Int32 num3 = 0;
		Vector3[] array4 = new Vector3[3];
		array4[0] = array[2];
		for (Int32 j = 0; j < 3; j++)
		{
			array4[1] = array[j];
			if (j != edgeException)
			{
				Single num4 = (array4[1][num] - array4[0][num]) * (cPos[num2] - oldCPos[num2]);
				num4 -= (array4[1][num2] - array4[0][num2]) * (cPos[num] - oldCPos[num]);
				Single num5 = (array4[0][num2] - oldCPos[num2]) * (cPos[num] - oldCPos[num]);
				num5 -= (array4[0][num] - oldCPos[num]) * (cPos[num2] - oldCPos[num2]);
				if (Mathf.Abs(num5) <= Mathf.Abs(num4))
				{
					Single num6 = (array4[0][num2] - oldCPos[num2]) * (array4[1][num] - array4[0][num]);
					num6 -= (array4[0][num] - oldCPos[num]) * (array4[1][num2] - array4[0][num2]);
					if (Mathf.Abs(num6) <= Mathf.Abs(num4) && ((num5 >= 0f && num4 >= 0f) || (num5 <= 0f && num4 < 0f)) && ((num6 >= 0f && num4 >= 0f) || (num6 <= 0f && num4 < 0f)))
					{
						if (!Mathf.Approximately(num4, 0f))
						{
							array2[num3][num] = array4[0][num] + num5 * (array4[1][num] - array4[0][num]) / num4;
							array2[num3][num2] = array4[0][num2] + num5 * (array4[1][num2] - array4[0][num2]) / num4;
							if (Mathf.Abs(array4[1][num] - array4[0][num]) > Mathf.Abs(array4[1][num2] - array4[0][num2]))
							{
								array2[num3][index] = this.BGI_interceptCompute(array4[0][index], array4[0][num], array4[1][index], array4[1][num], array2[num3][num]);
							}
							else
							{
								array2[num3][index] = this.BGI_interceptCompute(array4[0][index], array4[0][num2], array4[1][index], array4[1][num2], array2[num3][num2]);
							}
						}
						array3[num3] = j;
						num3++;
					}
				}
			}
			array4[0] = array4[1];
		}
		switch (num3)
		{
		case 0:
			edgeNdx = -1;
			break;
		case 1:
			edgeNdx = array3[0];
			intPnt = array2[0];
			break;
		case 2:
			switch (array3[0] + array3[1])
			{
			case 1:
				array4[0] = array[1];
				array4[1] = array[2];
				array4[2] = array[0];
				break;
			case 2:
				array4[0] = array[1];
				array4[1] = array[0];
				array4[2] = array[2];
				break;
			case 3:
				array4[0] = array[2];
				array4[1] = array[0];
				array4[2] = array[1];
				break;
			}
			if (this.BGI_quickCrossingEdge(array4[0], cPos, array4[2], array4[1], num, num2))
			{
				edgeNdx = array3[0];
				intPnt = array2[0];
			}
			else
			{
				edgeNdx = array3[1];
				intPnt = array2[1];
			}
			break;
		case 3:
		{
			Int32 num7 = 0;
			Single num8 = Single.MaxValue;
			for (Int32 k = 0; k < 3; k++)
			{
				Single sqrMagnitude = array2[k].sqrMagnitude;
				if (sqrMagnitude < num8)
				{
					num7 = k;
					num8 = sqrMagnitude;
				}
			}
			edgeNdx = num7;
			intPnt = array2[num7];
			break;
		}
		default:
			edgeNdx = -1;
			break;
		}
		return edgeNdx != -1;
	}

	public List<WalkMeshTriangle>[] trisPerCamera;

	public List<WalkMeshTriangle> tris;

	public GameObject ProjectedWalkMesh;

	public GameObject OriginalWalkMesh;

    public Boolean useCachedBounds;

    private FieldMap fieldMap;

	private Vector3 bgiCharPos;

	private Int32 BGIForceCount;

	private Vector3[] BGIForces;

	private Boolean[] BGIForceType;

	private Single[] BGIForceDist;

	private Byte attributeMask;

	private BGI_SIM_DEF[] BGISimList;

	public List<FieldMapActor> actors;
}
