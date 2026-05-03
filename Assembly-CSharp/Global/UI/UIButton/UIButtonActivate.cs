using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Activate")]
public class UIButtonActivate : MonoBehaviour
{
    private void OnClick()
    {
        if (this.target != (UnityEngine.Object)null)
        {
            NGUITools.SetActive(this.target, this.state);
        }
    }

    public GameObject target;

    public Boolean state = true;
}
