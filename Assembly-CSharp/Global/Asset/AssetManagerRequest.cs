using System;
using UnityEngine;

public class AssetManagerRequest
{
    public AssetManagerRequest(ResourceRequest resReq, AssetBundleRequest assReq)
    {
        this._resReq = resReq;
        this._assReq = assReq;
    }

    public Boolean allowSceneActivation
    {
        get
        {
            if (this._resReq != null)
            {
                return this._resReq.allowSceneActivation;
            }
            return this._assReq.allowSceneActivation;
        }
        set
        {
            if (this._resReq != null)
            {
                this._resReq.allowSceneActivation = value;
            }
            else
            {
                this._assReq.allowSceneActivation = value;
            }
        }
    }

    public Int32 priority
    {
        get
        {
            if (this._resReq != null)
            {
                return this._resReq.priority;
            }
            return this._assReq.priority;
        }
        set
        {
            if (this._resReq != null)
            {
                this._resReq.priority = value;
            }
            else
            {
                this._assReq.priority = value;
            }
        }
    }

    public Single progress
    {
        get
        {
            if (this._resReq != null)
            {
                return this._resReq.progress;
            }
            return this._assReq.progress;
        }
    }

    public Boolean isDone
    {
        get
        {
            if (this._resReq != null)
            {
                return this._resReq.isDone;
            }
            return this._assReq.isDone;
        }
    }

    public UnityEngine.Object asset
    {
        get
        {
            if (this._resReq != null)
            {
                return this._resReq.asset;
            }
            return this._assReq.asset;
        }
    }

    private ResourceRequest _resReq;

    private AssetBundleRequest _assReq;
}
