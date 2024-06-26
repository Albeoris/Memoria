using System;
using UnityEngine;

public class FieldSPSActor : HonoBehavior
{
	public override void HonoStart()
	{
		GameObject fieldMapGo = GameObject.Find("FieldMap");
		if (fieldMapGo != null)
			this._fieldMap = fieldMapGo.GetComponent<FieldMap>();
	}

	public override void HonoLateUpdate()
	{
	}

	private void ForcedNonCullingMesh()
	{
		Camera mainCamera = this._fieldMap.GetMainCamera();
		Vector3 cameraPosition = mainCamera.transform.position;
		Vector3 cameraForward = Vector3.Normalize(mainCamera.transform.forward);
		Single viewRangeMiddle = (mainCamera.farClipPlane - mainCamera.nearClipPlane) / 2f + mainCamera.nearClipPlane;
		Vector3 center = base.transform.InverseTransformPoint(cameraPosition + cameraForward * viewRangeMiddle);
		SkinnedMeshRenderer[] meshRenderers = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in meshRenderers)
			skinnedMeshRenderer.localBounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
		MeshFilter[] meshFilters = base.GetComponentsInChildren<MeshFilter>(true);
		foreach (MeshFilter meshFilter in meshFilters)
			meshFilter.sharedMesh.bounds = new Bounds(center, Vector3.one * Single.MaxValue * 0.01f);
	}

	public Single charZ;
	public Single charPsxZ;
	public Single charAbsZ;
	public Single charOffsetY;

	public Vector3 spsPos;
	public SPSEffect sps;

	private FieldMap _fieldMap;
}
