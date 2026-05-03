using System;
using UnityEngine;
using Object = System.Object;

public class PersistenSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (PersistenSingleton<T>.instance == (UnityEngine.Object)null)
            {
                PersistenSingleton<T>.instance = (T)((Object)UnityEngine.Object.FindObjectOfType(typeof(T)));
                if ((Int32)UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
                {
                    UnityEngine.Object.DontDestroyOnLoad(PersistenSingleton<T>.instance.gameObject);
                    return PersistenSingleton<T>.instance;
                }
                if (PersistenSingleton<T>.instance == (UnityEngine.Object)null)
                {
                    GameObject gameObject = new GameObject(typeof(T).Name);
                    PersistenSingleton<T>.instance = gameObject.AddComponent<T>();
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                }
                else
                {
                    UnityEngine.Object.DontDestroyOnLoad(PersistenSingleton<T>.instance.gameObject);
                }
            }
            return PersistenSingleton<T>.instance;
        }
    }

    protected virtual void Awake()
    {
        if (PersistenSingleton<T>.instance != (UnityEngine.Object)null)
        {
            T[] array = UnityEngine.Object.FindObjectsOfType<T>();
            if ((Int32)array.Length > 1)
            {
                T[] array2 = array;
                for (Int32 i = 0; i < (Int32)array2.Length; i++)
                {
                    T t = array2[i];
                    if (t != PersistenSingleton<T>.instance)
                    {
                        UnityEngine.Object.Destroy(t.gameObject);
                    }
                }
            }
        }
    }

    private static T instance;
}
