using System;
using System.Collections.Generic;
using UnityEngine;

public class WMBeeOnWillRenderObject : Singleton<WMBeeOnWillRenderObject>
{
	protected override void Awake()
	{
		base.Awake();
		base.gameObject.AddComponent<MeshRenderer>();
	}

	private void Start()
	{
		base.transform.parent = WMScriptDirector.Instance.transform;
	}

	private void OnWillRenderObject()
	{
		this.cameraRotation = Camera.current.transform.rotation;
		List<WorldSPS> spsList = ff9.world.WorldSPSSystem.SpsList;
		for (Int32 i = 0; i < spsList.Count; i++)
		{
			WorldSPS worldSPS = spsList[i];
			if (worldSPS.spsBin != null)
			{
				Vector3 rot = worldSPS.rot;
				if (worldSPS.no != -1)
				{
					if (rot == Vector3.zero)
					{
						worldSPS.transform.LookAt(worldSPS.transform.position + this.cameraRotation * Vector3.back, this.cameraRotation * Vector3.up);
					}
					else
					{
						worldSPS.transform.rotation = Quaternion.Euler(worldSPS.rot);
					}
				}
			}
		}
	}

	private void LateUpdate()
	{
		base.transform.position = ff9.w_moveActorPtr.pos;
	}

	public Quaternion cameraRotation;
}
