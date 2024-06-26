using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Toggled Objects")]
public class UIToggledObjects : MonoBehaviour
{
	private void Awake()
	{
		if (this.target != (UnityEngine.Object)null)
		{
			if (this.activate.Count == 0 && this.deactivate.Count == 0)
			{
				if (this.inverse)
				{
					this.deactivate.Add(this.target);
				}
				else
				{
					this.activate.Add(this.target);
				}
			}
			else
			{
				this.target = (GameObject)null;
			}
		}
		UIToggle component = base.GetComponent<UIToggle>();
		EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.Toggle));
	}

	public void Toggle()
	{
		Boolean value = UIToggle.current.value;
		if (base.enabled)
		{
			for (Int32 i = 0; i < this.activate.Count; i++)
			{
				this.Set(this.activate[i], value);
			}
			for (Int32 j = 0; j < this.deactivate.Count; j++)
			{
				this.Set(this.deactivate[j], !value);
			}
		}
	}

	private void Set(GameObject go, Boolean state)
	{
		if (go != (UnityEngine.Object)null)
		{
			NGUITools.SetActive(go, state);
		}
	}

	public List<GameObject> activate;

	public List<GameObject> deactivate;

	[HideInInspector]
	[SerializeField]
	private GameObject target;

	[HideInInspector]
	[SerializeField]
	private Boolean inverse;
}
