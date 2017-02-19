using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag and Drop Root")]
public class UIDragDropRoot : MonoBehaviour
{
	private void OnEnable()
	{
		UIDragDropRoot.root = base.transform;
	}

	private void OnDisable()
	{
		if (UIDragDropRoot.root == base.transform)
		{
			UIDragDropRoot.root = (Transform)null;
		}
	}

	public static Transform root;
}
