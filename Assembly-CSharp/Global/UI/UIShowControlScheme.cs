using System;
using UnityEngine;

public class UIShowControlScheme : MonoBehaviour
{
    private void OnEnable()
    {
        UICamera.onSchemeChange = (UICamera.OnSchemeChange)Delegate.Combine(UICamera.onSchemeChange, new UICamera.OnSchemeChange(this.OnScheme));
        this.OnScheme();
    }

    private void OnDisable()
    {
        UICamera.onSchemeChange = (UICamera.OnSchemeChange)Delegate.Remove(UICamera.onSchemeChange, new UICamera.OnSchemeChange(this.OnScheme));
    }

    private void OnScheme()
    {
        if (this.target != (UnityEngine.Object)null)
        {
            UICamera.ControlScheme currentScheme = UICamera.currentScheme;
            if (currentScheme == UICamera.ControlScheme.Mouse)
            {
                this.target.SetActive(this.mouse);
            }
            else if (currentScheme == UICamera.ControlScheme.Touch)
            {
                this.target.SetActive(this.touch);
            }
            else if (currentScheme == UICamera.ControlScheme.Controller)
            {
                this.target.SetActive(this.controller);
            }
        }
    }

    public GameObject target;

    public Boolean mouse;

    public Boolean touch;

    public Boolean controller = true;
}
