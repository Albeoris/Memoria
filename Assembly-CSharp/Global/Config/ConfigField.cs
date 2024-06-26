using System;
using UnityEngine;

public class ConfigField
{
	public ConfigField()
	{
		this.ConfigChoice = new BetterList<GameObject>();
		this.Value = -1f;
	}

	public ConfigUI.Configurator Configurator;

	public GameObject ConfigParent;

	public ButtonGroupState Button;

	public BetterList<GameObject> ConfigChoice;

	public Boolean IsSlider;

	public Single Value;
}
