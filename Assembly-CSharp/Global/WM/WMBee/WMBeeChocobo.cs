using System;
using UnityEngine;

public class WMBeeChocobo : MonoBehaviour
{
	public WMActor Actor { get; private set; }

	private void Intialize()
	{
		this.Actor = base.GetComponent<WMActor>();
		this.renderers = base.GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		if (!this.didInitialize)
		{
			this.Intialize();
			this.didInitialize = true;
		}
	}

	public void SetType(Int32 status)
	{
		if (!this.didInitialize)
		{
			this.Intialize();
			this.didInitialize = true;
		}
		if (status < 3 || status > 7)
		{
			global::Debug.Log("Uh oh!");
		}
		switch (status)
		{
		case 3:
			this.renderers[0].material = this.NormalChocoboMaterials[0];
			this.renderers[1].material = this.NormalChocoboMaterials[1];
			this.renderers[2].material = this.NormalChocoboMaterials[2];
			this.Actor.originalActor.index = 3;
			break;
		case 4:
			this.renderers[0].material = this.AsaseChocoboMaterials[0];
			this.renderers[1].material = this.AsaseChocoboMaterials[1];
			this.renderers[2].material = this.AsaseChocoboMaterials[2];
			this.Actor.originalActor.index = 4;
			break;
		case 5:
			this.renderers[0].material = this.YamaChocoboMaterials[0];
			this.renderers[1].material = this.YamaChocoboMaterials[1];
			this.renderers[2].material = this.YamaChocoboMaterials[2];
			this.Actor.originalActor.index = 5;
			break;
		case 6:
			this.renderers[0].material = this.UmiChocoboMaterials[0];
			this.renderers[1].material = this.UmiChocoboMaterials[1];
			this.renderers[2].material = this.UmiChocoboMaterials[2];
			this.Actor.originalActor.index = 6;
			break;
		case 7:
			this.renderers[0].material = this.SoraChocoboMaterials[0];
			this.renderers[1].material = this.SoraChocoboMaterials[1];
			this.renderers[2].material = this.SoraChocoboMaterials[2];
			this.Actor.originalActor.index = 7;
			break;
		}
	}

	public Material[] NormalChocoboMaterials;

	public Material[] AsaseChocoboMaterials;

	public Material[] YamaChocoboMaterials;

	public Material[] UmiChocoboMaterials;

	public Material[] SoraChocoboMaterials;

	public Renderer[] renderers;

	private Boolean didInitialize;
}
