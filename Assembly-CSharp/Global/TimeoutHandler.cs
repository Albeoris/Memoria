using System;
using UnityEngine;

public class TimeoutHandler : MonoBehaviour
{
    public Single Timeout
    {
        set
        {
            this.timeout = Mathf.Clamp(value, 0f, Single.MaxValue);
        }
    }

    private void Update()
    {
        if (this.timeout > 0f)
        {
            this.timeout -= Time.deltaTime;
            return;
        }
        this.targetListener.SendMessage(this.message);
        base.enabled = false;
    }

    public GameObject targetListener;

    public String message;

    [SerializeField]
    private Single timeout = 100f;
}
