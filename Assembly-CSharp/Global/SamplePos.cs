using System;
using UnityEngine;

public class SamplePos
{
    public SamplePos(BTL_DATA d, Vector3 o, Vector3 t, Int16 f)
    {
        this.endSample = false;
        this.data = d;
        this.orig = o;
        this.targ = t;
        this.frame = (Single)f;
        this.currentFrame = 0f;
    }

    public void Update(Single dt)
    {
        if (this.data == null || this.data.gameObject == (UnityEngine.Object)null)
        {
            this.endSample = true;
        }
        if (this.endSample)
        {
            return;
        }
        this.currentFrame += dt * 60f;
        Single d = Mathf.Clamp01(this.currentFrame / this.frame);
        Vector3 vector = this.orig + (this.targ - this.orig) * d;
        this.data.pos = vector;
        this.data.gameObject.transform.localPosition = vector;
        if (this.currentFrame > this.frame)
        {
            this.endSample = true;
        }
    }

    public Boolean EndSample
    {
        get
        {
            return this.endSample;
        }
    }

    private BTL_DATA data;

    private Vector3 orig;

    private Vector3 targ;

    private Single currentFrame;

    private Single frame;

    private Boolean endSample;
}
