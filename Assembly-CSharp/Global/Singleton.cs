using System;
using UnityEngine;
using Object = System.Object;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (Singleton<T>.instance == (UnityEngine.Object)null)
			{
				Singleton<T>.instance = (UnityEngine.Object.FindObjectOfType<T>() ?? new GameObject(typeof(T).Name).AddComponent<T>());
			}
			return Singleton<T>.instance;
		}
	}

	protected virtual void Awake()
	{
		if (Singleton<T>.instance == (UnityEngine.Object)null)
		{
			Singleton<T>.instance = (T)((Object)UnityEngine.Object.FindObjectOfType(typeof(T)));
		}
		if ((Int32)UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
		{
			global::Debug.LogWarning("Hey! " + typeof(T).Name + " is a singleton, so there should be 1 instance of it, right? Anyway, one of them is being used now, but please try to get rid of this warning!");
		}
	}

	private static T instance;
}
