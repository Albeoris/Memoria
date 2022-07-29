using System;
using System.Collections.Generic;
using UnityEngine;

public class WMBlock : MonoBehaviour
{
	public List<WMMesh> ActiveWalkMeshes
	{
		get
		{
			if (this.Form == 1)
				return this.Form1WalkMeshes;
			if (this.Form == 2)
				return this.Form2WalkMeshes;
			return null;
		}
	}

	public Int32 Form
	{
		get
		{
			return this._form;
		}
		private set
		{
			this._form = value;
		}
	}

	public Vector3 PositionAsCenter
	{
		get
		{
			Vector3 position = base.transform.position;
			position.x += 32f;
			position.z -= 32f;
			return position;
		}
	}

	private void Start()
	{
		this._transform = base.transform;
	}

	public void AddWalkMeshForm1(Mesh mesh)
	{
		this.AddWalkMesh(this.Form1WalkMeshes, mesh);
	}

	public void AddWalkMeshForm2(Mesh mesh)
	{
		this.AddWalkMesh(this.Form2WalkMeshes, mesh);
	}

	private void AddWalkMesh(List<WMMesh> walkMeshes, Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Int32[] triangles = mesh.triangles;
		Vector3[] normals = mesh.normals;
		Vector4[] tangents = mesh.tangents;
		if ((Int32)vertices.Length != (Int32)triangles.Length && (Int32)vertices.Length != (Int32)tangents.Length)
		{
			global::Debug.LogWarning("All vertices, triangles, tangents .Length must be equal");
		}
		List<Vector3> list = new List<Vector3>();
		for (Int32 i = 0; i < (Int32)vertices.Length / 3; i++)
		{
			Vector3 b = vertices[triangles[i * 3]];
			Vector3 a = vertices[triangles[i * 3 + 1]];
			Vector3 a2 = vertices[triangles[i * 3 + 2]];
			Vector3 item = Vector3.Cross(a - b, a2 - b);
			item.Normalize();
			list.Add(item);
		}
		WMMesh item2 = new WMMesh
		{
			Id = this.Number,
			Transform = base.transform,
			Vertices = vertices,
			Triangles = triangles,
			Normals = normals,
			Tangents = tangents,
			TriangleNormals = list.ToArray()
		};
		walkMeshes.Add(item2);
	}

	public void AddForm1Transform(Transform form1Transform)
	{
		this.Form1Transforms.Add(form1Transform);
	}

	public void AddForm2Transform(Transform form2Transform)
	{
		this.Form2Transforms.Add(form2Transform);
	}

	public void SetForm(Int32 form)
	{
		if (!this.IsSwitchable)
		{
			return;
		}
		if (form != 1 && form != 2)
		{
			global::Debug.Log("There are only two forms (form 1 and form 2.)");
		}
		this.Form = form;
	}

	public void ApplyForm()
	{
		if (this.Form == 1)
		{
			foreach (Transform transform in this.Form2Transforms)
				transform.gameObject.SetActive(false);
			foreach (Transform transform in this.Form1Transforms)
				transform.gameObject.SetActive(true);
		}
		else if (this.Form == 2)
		{
			foreach (Transform transform in this.Form1Transforms)
				transform.gameObject.SetActive(false);
			foreach (Transform transform in this.Form2Transforms)
				transform.gameObject.SetActive(true);
		}
	}

	public Transform Transform
	{
		get
		{
			return this._transform;
		}
	}

	public Boolean Raycast(Ray ray, out WMRaycastHit hit, Single distance, out Int32 mapid, ff9.s_moveCHRCache cache)
	{
		Boolean flag = false;
		Boolean flag2 = false;
		Boolean flag3 = false;
		hit = default(WMRaycastHit);
		mapid = 0;
		Int32 num = -1;
		if (cache != null)
		{
			for (Int32 i = 0; i < 10; i++)
			{
				Int32 num2 = (cache.Number + i) % 10;
				if (!(cache.Blocks[num2] != this))
				{
					Int32 triangleIndex = cache.TriangleIndices[num2];
					Int32 walkMeshIndex = cache.WalkMeshIndices[num2];
					flag3 = (cache.OnObject[num2] == 1);
					if (this.RaycastOnSpecifiedTriangle(ray, this.ActiveWalkMeshes, walkMeshIndex, out hit, out mapid, triangleIndex))
					{
						flag2 = true;
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag && this.Raycast(ray, this.ActiveWalkMeshes, out hit, out mapid, out num))
		{
			flag2 = true;
			flag3 = false;
		}
		if (flag2)
		{
			if (cache != null && !flag)
			{
				cache.Number++;
				cache.Number %= 10;
				cache.TriangleIndices[cache.Number] = hit.triangleIndex;
				cache.WalkMeshIndices[cache.Number] = num;
				cache.Blocks[cache.Number] = this;
				cache.OnObject[cache.Number] = (Byte)((!flag3) ? 0 : 1);
			}
			return true;
		}
		return false;
	}

	private Boolean Raycast(Ray ray, List<WMMesh> walkMeshes, out WMRaycastHit hit, out Int32 mapid, out Int32 walkMeshIndex)
	{
		hit = default(WMRaycastHit);
		mapid = 0;
		walkMeshIndex = -1;
		for (Int32 i = 0; i < walkMeshes.Count; i++)
		{
			WMMesh mesh = walkMeshes[i];
			if (this.Raycast(ray, mesh, out hit, out mapid))
			{
				walkMeshIndex = i;
				return true;
			}
		}
		return false;
	}

	private Boolean Raycast(Ray ray, WMMesh mesh, out WMRaycastHit hit, out Int32 mapid)
	{
		if (WMPhysics.Raycast(ray, mesh, out hit))
		{
			Transform transform = mesh.Transform;
			Vector3[] vertices = mesh.Vertices;
			Int32[] triangles = mesh.Triangles;
			Vector4[] tangents = mesh.Tangents;
			mapid = (Int32)tangents[triangles[hit.triangleIndex * 3]].x;
            return mapid != 0x31EE || ff9.w_moveCHRControlPtr.type == 1;
        }
		mapid = 0;
		return false;
	}

	private Boolean RaycastOnSpecifiedTriangle(Ray ray, List<WMMesh> walkMeshes, Int32 walkMeshIndex, out WMRaycastHit hit, out Int32 mapid, Int32 triangleIndex)
	{
		WMMesh mesh = walkMeshes[walkMeshIndex];
		return this.RaycastOnSpecifiedTriangle(ray, mesh, out hit, out mapid, triangleIndex);
	}

	private Boolean RaycastOnSpecifiedTriangle(Ray ray, WMMesh mesh, out WMRaycastHit hit, out Int32 mapid, Int32 triangleIndex)
	{
		Transform transform = mesh.Transform;
		Vector3[] vertices = mesh.Vertices;
		Int32[] triangles = mesh.Triangles;
		Vector4[] tangents = mesh.Tangents;
		if (WMPhysics.RaycastOnSpecifiedTriangle(ray, mesh, triangleIndex, out hit))
		{
			mapid = (Int32)tangents[triangles[hit.triangleIndex * 3]].x;
            return mapid != 0x31EE || ff9.w_moveCHRControlPtr.type == 1;
        }
		mapid = 0;
		return false;
	}

	public List<WMMesh> Form1WalkMeshes;

	public List<WMMesh> Form2WalkMeshes;

	public List<Transform> Form1Transforms;

	public List<Transform> Form2Transforms;

	public Boolean IsSea;

	public Boolean HasSpecialObject;

	public Boolean IsSwitchable;

	public Boolean HasRiver;

	public Boolean HasRiverJoint;

	public Boolean HasStream;

	public Boolean HasFalls;

	public Boolean HasBeach1;

	public Boolean HasBeach2;

	public Boolean HasSea;

	public Boolean HasVolcanoCrater;

	public Boolean HasVolcanoLava;

	public Int32 InitialX;

	public Int32 InitialY;

	public Int32 Number;

	public Int32 CurrentX;

	public Int32 CurrentY;

	public Boolean IsInsideSight;

	public Color DebugColor = Color.yellow;

	public Bounds Bounds;

	public Boolean StartedLoadAsync;

	private Int32 _form = 1;

	public Boolean IsReady;

	private Transform _transform;
}
