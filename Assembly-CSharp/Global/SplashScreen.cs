using System;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    private void Awake()
    {
        global::Debug.Log("20 SplashScreen.Awake");
        UnityEngine.Object.DontDestroyOnLoad(this);
        Boolean flag = false;
        if (Application.platform == RuntimePlatform.IPhonePlayer || flag)
        {
            Application.LoadLevel("Bundle-iOS");
        }
        else
        {
            Application.LoadLevel("Bundle");
        }
    }
}
