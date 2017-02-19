using System;
using UnityEngine;

public class CommonState : MonoBehaviour
{
	private void Awake()
	{
		this.Init();
	}

	public void Init()
	{
		this.FF9 = new FF9StateGlobal();
	}

	public FF9StateGlobal FF9;
}
