using System;
using UnityEngine;

public class SharedDataLoadingAnimationDebug : MonoBehaviour
{
    private void Start()
    {
        this.v = 0f;
        this.r = 0.5f;
        this.s = 5f;
        this.pRef = base.gameObject.transform.position;
        this.speed = 100f;
    }

    private void Update()
    {
        Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
        eulerAngles.y += this.speed * Time.deltaTime;
        base.gameObject.transform.eulerAngles = eulerAngles;
    }

    private Single v;

    private Single r;

    private Single s;

    private Vector3 pRef;

    private Single speed;
}
