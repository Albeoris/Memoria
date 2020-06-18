using System;
using System.Collections.Generic;
using UnityEngine;

public class btlshadow
{
	public static void ff9battleShadowInit(Int32 shadowCount)
	{
		GameObject[] array = FF9StateSystem.Battle.FF9Battle.map.shadowArray = new GameObject[shadowCount];
		for (Int32 i = 0; i < shadowCount; i++)
		{
			List<Vector3> list = new List<Vector3>();
			List<Color> list2 = new List<Color>();
			List<Vector2> list3 = new List<Vector2>();
			List<Int32> list4 = new List<Int32>();
			list.Add(new Vector3(-1f, 0f, -1f));
			list.Add(new Vector3(1f, 0f, -1f));
			list.Add(new Vector3(1f, 0f, 1f));
			list.Add(new Vector3(-1f, 0f, 1f));
			Color item = new Color(1f, 1f, 1f, 0.6f);
			list2.Add(item);
			list2.Add(item);
			list2.Add(item);
			list2.Add(item);
			list3.Add(new Vector2(0f, 0f));
			list3.Add(new Vector2(1f, 0f));
			list3.Add(new Vector2(1f, 1f));
			list3.Add(new Vector2(0f, 1f));
			list4.Add(2);
			list4.Add(1);
			list4.Add(0);
			list4.Add(3);
			list4.Add(2);
			list4.Add(0);
			Mesh mesh = new Mesh();
			mesh.vertices = list.ToArray();
			mesh.colors = list2.ToArray();
			mesh.uv = list3.ToArray();
			mesh.triangles = list4.ToArray();
			GameObject gameObject = array[i] = new GameObject("shadow" + i.ToString("D3"));
			gameObject.transform.localScale = new Vector3(224f, 0f, 192f);
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			Shader shadowShader = FF9StateSystem.Battle.shadowShader;
			Material material = new Material(shadowShader);
			material.SetColor("_TintColor", new Color32(64, 64, 64, Byte.MaxValue));
			String[] pngInfo;
			material.mainTexture = AssetManager.Load<Texture2D>("CommonAsset/Common/shadow_plate", out pngInfo, false);
			meshRenderer.material = material;
			gameObject.SetActive(false);
		}
	}

	public static void FF9ShadowSetScaleBattle(UInt32 charNo, Byte x, Byte z)
	{
		GameObject[] shadowArray = FF9StateSystem.Battle.FF9Battle.map.shadowArray;
		Vector3 localScale = shadowArray[(Int32)((UIntPtr)charNo)].transform.localScale;
		localScale.x = (Single)(224 * x) * 1f / 16f;
		localScale.z = (Single)(192 * z) * 1f / 16f;
		shadowArray[(Int32)((UIntPtr)charNo)].transform.localScale = localScale;
	}

	public const Int32 FF9SHADOW_RADIUS = 16;

	public const Int32 FF9SHADOW_SCALE_X = 224;

	public const Int32 FF9SHADOW_SCALE_Z = 192;
}
