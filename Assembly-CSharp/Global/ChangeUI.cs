using System;
using UnityEngine;

public class ChangeUI : MonoBehaviour
{
	private void Awake()
	{
		this.Scene = FF9StateSystem.Field.SceneName;
		this.fieldMap = GameObject.Find("FieldMap").GetComponent<FieldMap>();
	}

	private void Update()
	{
		if ((Int32)this.AnimIdx.Length != (Int32)this.fieldMap.animIdx.Length)
		{
			this.AnimIdx = new Boolean[(Int32)this.fieldMap.animIdx.Length];
			for (Int32 i = 0; i < (Int32)this.fieldMap.animIdx.Length; i++)
			{
				this.AnimIdx[i] = this.fieldMap.animIdx[i];
			}
		}
		for (Int32 j = 0; j < (Int32)this.fieldMap.animIdx.Length; j++)
		{
			if (this.AnimIdx[j] != this.fieldMap.animIdx[j])
			{
				this.fieldMap.animIdx[j] = this.AnimIdx[j];
			}
		}
		if (this.fieldMap.camIdx != this.CamIdx)
		{
			BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
			currentBgCamera.projectedWalkMesh.SetActive(false);
			this.fieldMap.SetCurrentCameraIndex(this.CamIdx);
			BGCAM_DEF currentBgCamera2 = this.fieldMap.GetCurrentBgCamera();
		}
	}

	public String Scene = "FBG_N01_ALXT_MAP016_AT_MSA_0";

	public Int32 CamIdx;

	public Boolean[] AnimIdx;

	public Int32 zExtra;

	private FieldMap fieldMap;
}
