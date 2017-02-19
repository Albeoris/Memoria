using System;
using UnityEngine;

public class RealTime : MonoBehaviour
{
	public static Single time
	{
		get
		{
			return Time.unscaledTime;
		}
	}

	public static Single deltaTime
	{
		get
		{
			return Time.unscaledDeltaTime;
		}
	}
}
