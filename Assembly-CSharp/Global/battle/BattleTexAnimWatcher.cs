using System;
using System.Collections;
using FF9;
using UnityEngine;

public class BattleTexAnimWatcher : MonoBehaviour
{
	private void Update()
	{
		FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
		for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
		{
			if (!Status.checkCurStat(next, 1073741824u))
			{
				this.CheckRenderTexture(next.texanimptr);
				this.CheckRenderTexture(next.tranceTexanimptr);
			}
		}
	}

	public void CheckRenderTexture(GEOTEXHEADER texHeader)
	{
		if (texHeader == null)
		{
			return;
		}
		for (Int32 i = 0; i < (Int32)texHeader.count; i++)
		{
			RenderTexture renderTexture = texHeader.RenderTexMapping[texHeader._mainTextureIndexs[i]];
			if (!renderTexture.IsCreated())
			{
				if (texHeader.geotex != null)
				{
					GEOTEXANIMHEADER texAnimHeader = texHeader.geotex[i];
					base.StartCoroutine(this.WaitForRecreateRenderTexture(texAnimHeader, texHeader, i));
				}
			}
		}
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

	private void SetDefaultTextures(GEOTEXHEADER texHeader, Int32 i)
	{
		texHeader._smrs[texHeader._mainTextureIndexs[i]].material.mainTexture = texHeader.TextureMapping[texHeader._mainTextureIndexs[i]];
		texHeader._smrs[texHeader._subTextureIndexs[i]].material.mainTexture = texHeader.TextureMapping[texHeader._subTextureIndexs[i]];
	}

	private void SetBackRenderTexture(GEOTEXHEADER texHeader, Int32 i)
	{
		texHeader._smrs[texHeader._mainTextureIndexs[i]].material.mainTexture = texHeader.RenderTexMapping[texHeader._mainTextureIndexs[i]];
	}

	private IEnumerator WaitForRecreateRenderTexture(GEOTEXANIMHEADER texAnimHeader, GEOTEXHEADER texHeader, Int32 i)
	{
		this.SetDefaultTextures(texHeader, i);
		yield return new WaitForEndOfFrame();
		this.SetBackRenderTexture(texHeader, i);
		GeoTexAnim.RecreateMultiTexAnim(texAnimHeader, texHeader, i);
		yield break;
	}
}
