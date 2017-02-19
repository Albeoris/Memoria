using System;
using UnityEngine;

public class FieldSPSActor : HonoBehavior
{
	public override void HonoStart()
	{
		GameObject gameObject = GameObject.Find("FieldMap");
		if (gameObject != (UnityEngine.Object)null)
		{
			this._fieldMap = gameObject.GetComponent<FieldMap>();
		}
	}

	public override void HonoLateUpdate()
	{
	}

	private void ForcedNonCullingMesh()
	{
		Camera mainCamera = this._fieldMap.GetMainCamera();
		Vector3 position = mainCamera.transform.position;
		Vector3 a = Vector3.Normalize(mainCamera.transform.forward);
		Single d = (mainCamera.farClipPlane - mainCamera.nearClipPlane) / 2f + mainCamera.nearClipPlane;
		Vector3 position2 = position + a * d;
		Vector3 center = base.transform.InverseTransformPoint(position2);
		SkinnedMeshRenderer[] componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		SkinnedMeshRenderer[] array = componentsInChildren;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = array[i];
			skinnedMeshRenderer.localBounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
		}
		MeshFilter[] componentsInChildren2 = base.GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array2 = componentsInChildren2;
		for (Int32 j = 0; j < (Int32)array2.Length; j++)
		{
			MeshFilter meshFilter = array2[j];
			meshFilter.sharedMesh.bounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
		}
	}

	public Single charZ;

	public Single charPsxZ;

	public Single charAbsZ;

	public Single charOffsetY;

	public Vector3 spsPos;

	public FieldSPS sps;

	private FieldMap _fieldMap;
}
