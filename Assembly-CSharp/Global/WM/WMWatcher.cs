using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = System.Object;

public class WMWatcher : Singleton<WMWatcher>
{
    protected override void Awake()
    {
        base.Awake();
        this.StartPosition = this.TextPosition;
        foreach (String text in this.StartupLabels)
        {
        }
        this.w_cameraEyeOffset = GameObject.Find("w_cameraEyeOffset").transform;
        this.w_cameraWorldAim = GameObject.Find("w_cameraWorldAim").transform;
        this.w_cameraWorldEye = GameObject.Find("w_cameraWorldEye").transform;
    }

    [Conditional("WATCHER_ENABLED")]
    public void SetText(String lable, String value)
    {
    }

    [Conditional("WATCHER_ENABLED")]
    public void SetText(String lable, String value, Color color)
    {
        if (!this.textDictionary.ContainsKey(lable))
        {
            GUIText guitext = new GameObject(lable).AddComponent<GUIText>();
            guitext.transform.position = this.TextPosition;
            guitext.transform.parent = base.transform;
            this.TextPosition = new Vector3(this.TextPosition.x, this.TextPosition.y - 0.025f, this.TextPosition.z);
            if (this.TextPosition.y <= 0.125f)
            {
                this.TextPosition.y = this.StartPosition.y;
                this.TextPosition.x = 0.6f;
            }
            this.textDictionary.Add(lable, guitext);
        }
        GUIText guitext2 = this.textDictionary[lable];
        guitext2.color = color;
        guitext2.text = value;
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Boolean value)
    {
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Boolean value, Color color)
    {
        String text = String.Format("{0}: {1}", lable, value);
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, String value)
    {
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, String value, Color color)
    {
        String text = String.Format("{0}: {1}", lable, value);
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Vector3 value)
    {
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Vector3 value, Color color)
    {
        String text = String.Format("{0}: {1}, {2}, {3}", new Object[]
        {
            lable,
            value.x,
            value.y,
            value.z
        });
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Int32 value)
    {
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Int32 value, Color color)
    {
        String text = String.Format("{0}: {1}", lable, value);
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Single value)
    {
    }

    [Conditional("WATCHER_ENABLED")]
    public void Set(String lable, Single value, Color color)
    {
        String text = String.Format("{0}: {1}", lable, value);
    }

    [Conditional("WATCHER_TRANSFORM_DEBUG_ENABLED")]
    public void Set_w_cameraEyeOffset(Vector3 position)
    {
        this.w_cameraEyeOffset.position = position;
    }

    [Conditional("WATCHER_TRANSFORM_DEBUG_ENABLED")]
    public void Set_w_cameraWorldAim(Vector3 position)
    {
        WMTweaker instance = Singleton<WMTweaker>.Instance;
        Vector3 b = new Vector3(0f, (Single)instance.w_cameraWorldAim_Y * 0.00390625f, 0f);
        this.w_cameraWorldAim.position = position + b;
    }

    [Conditional("WATCHER_TRANSFORM_DEBUG_ENABLED")]
    public void Set_w_cameraWorldEye(Vector3 position)
    {
        WMTweaker instance = Singleton<WMTweaker>.Instance;
        Vector3 b = new Vector3(0f, (Single)instance.w_cameraWorldEye_Y * 0.00390625f, 0f);
        this.w_cameraWorldEye.position = position + b;
    }

    public Transform w_cameraEyeOffset;

    public Transform w_cameraWorldAim;

    public Transform w_cameraWorldEye;

    public Vector3 TextPosition = new Vector3(0.01f, 0.975f, 0f);

    private Vector3 StartPosition;

    public List<String> StartupLabels = new List<String>();

    private readonly Dictionary<String, GUIText> textDictionary = new Dictionary<String, GUIText>();
}
