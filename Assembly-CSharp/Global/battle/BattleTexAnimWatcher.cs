using System;
using System.Collections;
using FF9;
using Memoria.Data;
using UnityEngine;

public class BattleTexAnimWatcher : MonoBehaviour
{
	private void Update()
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
		{
			if (!Status.checkCurStat(next, BattleStatus.Jump))
			{
				this.CheckRenderTextures(next.texanimptr);
				this.CheckRenderTextures(next.tranceTexanimptr);
			}
		}
	}

	public void CheckRenderTextures(GEOTEXHEADER texHeader)
	{
		if (texHeader == null)
			return;
        texHeader.CheckRenderTextures(this);
	}

	public static void ForcedNonCullingMesh(GameObject go)
	{
		Camera component = GameObject.Find("Battle Camera").GetComponent<Camera>();
		Vector3 position = component.transform.position;
		Vector3 a = Vector3.Normalize(component.transform.forward);
		Single d = (component.farClipPlane - component.nearClipPlane) / 2f + component.nearClipPlane;
		Vector3 position2 = position + a * d;
		Vector3 center = go.transform.InverseTransformPoint(position2);
		SkinnedMeshRenderer[] componentsInChildren = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		SkinnedMeshRenderer[] array = componentsInChildren;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = array[i];
			skinnedMeshRenderer.localBounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
		}
		MeshFilter[] componentsInChildren2 = go.GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array2 = componentsInChildren2;
		for (Int32 j = 0; j < (Int32)array2.Length; j++)
		{
			MeshFilter meshFilter = array2[j];
			meshFilter.sharedMesh.bounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
		}
	}
}
