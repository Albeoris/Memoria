using System;
using UnityEngine;

[RequireComponent(typeof(UIInput))]
public class UIInputOnGUI : MonoBehaviour
{
	private void Awake()
	{
		this.mInput = base.GetComponent<UIInput>();
	}

	private void OnGUI()
	{
		if (Event.current.rawType == EventType.KeyDown)
		{
			this.mInput.ProcessEvent(Event.current);
		}
	}

	[NonSerialized]
	private UIInput mInput;
}
