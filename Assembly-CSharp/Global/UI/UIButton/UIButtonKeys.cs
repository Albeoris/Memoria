using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Keys (Legacy)")]
[ExecuteInEditMode]
public class UIButtonKeys : UIKeyNavigation
{
	protected override void OnEnable()
	{
		this.Upgrade();
		base.OnEnable();
	}

	public void Upgrade()
	{
		if (this.onClick == (UnityEngine.Object)null && this.selectOnClick != (UnityEngine.Object)null)
		{
			this.onClick = this.selectOnClick.gameObject;
			this.selectOnClick = (UIButtonKeys)null;
			NGUITools.SetDirty(this);
		}
		if (this.onLeft == (UnityEngine.Object)null && this.selectOnLeft != (UnityEngine.Object)null)
		{
			this.onLeft = this.selectOnLeft.gameObject;
			this.selectOnLeft = (UIButtonKeys)null;
			NGUITools.SetDirty(this);
		}
		if (this.onRight == (UnityEngine.Object)null && this.selectOnRight != (UnityEngine.Object)null)
		{
			this.onRight = this.selectOnRight.gameObject;
			this.selectOnRight = (UIButtonKeys)null;
			NGUITools.SetDirty(this);
		}
		if (this.onUp == (UnityEngine.Object)null && this.selectOnUp != (UnityEngine.Object)null)
		{
			this.onUp = this.selectOnUp.gameObject;
			this.selectOnUp = (UIButtonKeys)null;
			NGUITools.SetDirty(this);
		}
		if (this.onDown == (UnityEngine.Object)null && this.selectOnDown != (UnityEngine.Object)null)
		{
			this.onDown = this.selectOnDown.gameObject;
			this.selectOnDown = (UIButtonKeys)null;
			NGUITools.SetDirty(this);
		}
	}

	public UIButtonKeys selectOnClick;

	public UIButtonKeys selectOnUp;

	public UIButtonKeys selectOnDown;

	public UIButtonKeys selectOnLeft;

	public UIButtonKeys selectOnRight;
}
