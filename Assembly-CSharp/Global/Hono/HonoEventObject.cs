using System;
using UnityEngine;

public class HonoEventObject : HonoBehavior
{
    public new Boolean IsVisibled()
    {
        return this._isVisibled;
    }

    public new Boolean IsEnabled()
    {
        return this._isEnabled;
    }

    public new void SetVisible(Boolean value)
    {
        if (value == this._isVisibled)
        {
            return;
        }
        Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>();
        Renderer[] array = componentsInChildren;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            Renderer renderer = array[i];
            renderer.enabled = value;
        }
        this._isVisibled = value;
    }

    public new void SetEnabled(Boolean value)
    {
        if (value == this._isEnabled)
        {
            return;
        }
        this._isEnabled = value;
    }

    private Boolean _isVisibled = true;

    private Boolean _isEnabled = true;
}
