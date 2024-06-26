using System;
using System.Collections.Generic;
using UnityEngine;

public class btlshadow
{
	public static void ff9battleShadowInit(BTL_DATA btl)
	{
		GameObject shadow = new GameObject("shadow" + btl.bi.slot_no.ToString("D3"));
		FF9StateSystem.Battle.FF9Battle.map.shadowArray[btl] = shadow;
		List<Vector3> vertList = new List<Vector3>();
		List<Color> colList = new List<Color>();
		List<Vector2> uvList = new List<Vector2>();
		List<Int32> triList = new List<Int32>();
		vertList.Add(new Vector3(-1f, 0f, -1f));
		vertList.Add(new Vector3(1f, 0f, -1f));
		vertList.Add(new Vector3(1f, 0f, 1f));
		vertList.Add(new Vector3(-1f, 0f, 1f));
		Color item = new Color(1f, 1f, 1f, 0.6f);
		colList.Add(item);
		colList.Add(item);
		colList.Add(item);
		colList.Add(item);
		uvList.Add(new Vector2(0f, 0f));
		uvList.Add(new Vector2(1f, 0f));
		uvList.Add(new Vector2(1f, 1f));
		uvList.Add(new Vector2(0f, 1f));
		triList.Add(2);
		triList.Add(1);
		triList.Add(0);
		triList.Add(3);
		triList.Add(2);
		triList.Add(0);
		Mesh mesh = new Mesh();
		mesh.vertices = vertList.ToArray();
		mesh.colors = colList.ToArray();
		mesh.uv = uvList.ToArray();
		mesh.triangles = triList.ToArray();
		shadow.transform.localScale = new Vector3(224f, 0f, 192f);
		MeshRenderer meshRenderer = shadow.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = shadow.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		Shader shadowShader = FF9StateSystem.Battle.shadowShader;
		Material material = new Material(shadowShader);
		material.SetColor("_TintColor", new Color32(64, 64, 64, Byte.MaxValue));
		material.mainTexture = AssetManager.Load<Texture2D>("CommonAsset/Common/shadow_plate", false);
		meshRenderer.material = material;
		shadow.SetActive(false);
	}

	public static void FF9ShadowSetScaleBattle(BTL_DATA btl, Byte x, Byte z)
	{
		GameObject shadow = FF9StateSystem.Battle.FF9Battle.map.shadowArray[btl];
		if (shadow == null || !shadow.activeSelf)
			return;
		Vector3 localScale = shadow.transform.localScale;
		localScale.x = (Single)(224 * x) * 1f / 16f;
		localScale.z = (Single)(192 * z) * 1f / 16f;
		shadow.transform.localScale = localScale;
	}

	public const Int32 FF9SHADOW_RADIUS = 16;

	public const Int32 FF9SHADOW_SCALE_X = 224;

	public const Int32 FF9SHADOW_SCALE_Z = 192;
}
