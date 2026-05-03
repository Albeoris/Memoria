using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
    public Boolean isEnabled
    {
        get
        {
            Collider component = base.gameObject.GetComponent<Collider>();
            return component && component.enabled;
        }
        set
        {
            Collider component = base.gameObject.GetComponent<Collider>();
            if (!component)
            {
                return;
            }
            if (component.enabled != value)
            {
                component.enabled = value;
                this.UpdateImage();
            }
        }
    }

    private void OnEnable()
    {
        if (this.target == (UnityEngine.Object)null)
        {
            this.target = base.GetComponentInChildren<UISprite>();
        }
        this.UpdateImage();
    }

    private void OnValidate()
    {
        if (this.target != (UnityEngine.Object)null)
        {
            if (String.IsNullOrEmpty(this.normalSprite))
            {
                this.normalSprite = this.target.spriteName;
            }
            if (String.IsNullOrEmpty(this.hoverSprite))
            {
                this.hoverSprite = this.target.spriteName;
            }
            if (String.IsNullOrEmpty(this.pressedSprite))
            {
                this.pressedSprite = this.target.spriteName;
            }
            if (String.IsNullOrEmpty(this.disabledSprite))
            {
                this.disabledSprite = this.target.spriteName;
            }
        }
    }

    private void UpdateImage()
    {
        if (this.target != (UnityEngine.Object)null)
        {
            if (this.isEnabled)
            {
                this.SetSprite((!UICamera.IsHighlighted(base.gameObject)) ? this.normalSprite : this.hoverSprite);
            }
            else
            {
                this.SetSprite(this.disabledSprite);
            }
        }
    }

    private void OnHover(Boolean isOver)
    {
        if (this.isEnabled && this.target != (UnityEngine.Object)null)
        {
            this.SetSprite((!isOver) ? this.normalSprite : this.hoverSprite);
        }
    }

    private void OnPress(Boolean pressed)
    {
        if (pressed)
        {
            this.SetSprite(this.pressedSprite);
        }
        else
        {
            this.UpdateImage();
        }
    }

    private void SetSprite(String sprite)
    {
        if (this.target.atlas == (UnityEngine.Object)null || this.target.atlas.GetSprite(sprite) == null)
        {
            return;
        }
        this.target.spriteName = sprite;
        if (this.pixelSnap)
        {
            this.target.MakePixelPerfect();
        }
    }

    public UISprite target;

    public String normalSprite;

    public String hoverSprite;

    public String pressedSprite;

    public String disabledSprite;

    public Boolean pixelSnap = true;
}
