using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIToggle))]
[AddComponentMenu("NGUI/Interaction/Toggled Components")]
[ExecuteInEditMode]
public class UIToggledComponents : MonoBehaviour
{
    private void Awake()
    {
        if (this.target != (UnityEngine.Object)null)
        {
            if (this.activate.Count == 0 && this.deactivate.Count == 0)
            {
                if (this.inverse)
                {
                    this.deactivate.Add(this.target);
                }
                else
                {
                    this.activate.Add(this.target);
                }
            }
            else
            {
                this.target = (MonoBehaviour)null;
            }
        }
        UIToggle component = base.GetComponent<UIToggle>();
        EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.Toggle));
    }

    public void Toggle()
    {
        if (base.enabled)
        {
            for (Int32 i = 0; i < this.activate.Count; i++)
            {
                MonoBehaviour monoBehaviour = this.activate[i];
                monoBehaviour.enabled = UIToggle.current.value;
            }
            for (Int32 j = 0; j < this.deactivate.Count; j++)
            {
                MonoBehaviour monoBehaviour2 = this.deactivate[j];
                monoBehaviour2.enabled = !UIToggle.current.value;
            }
        }
    }

    public List<MonoBehaviour> activate;

    public List<MonoBehaviour> deactivate;

    [HideInInspector]
    [SerializeField]
    private MonoBehaviour target;

    [HideInInspector]
    [SerializeField]
    private Boolean inverse;
}
