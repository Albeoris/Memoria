﻿using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteDisplay))]
public class SpriteClickable : MonoBehaviour
{
	public Boolean Contains(Vector3 worldPoint)
	{
		worldPoint.z = base.transform.position.z;
		if (base.GetComponent<SpriteRenderer>().sprite == (UnityEngine.Object)null)
		{
			base.GetComponent<SpriteDisplay>().ID = 0;
		}
		return base.GetComponent<SpriteRenderer>().bounds.Contains(worldPoint);
	}
}
