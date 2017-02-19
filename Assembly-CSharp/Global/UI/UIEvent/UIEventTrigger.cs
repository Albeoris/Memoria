using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	public Boolean isColliderEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			if (component != (UnityEngine.Object)null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != (UnityEngine.Object)null && component2.enabled;
		}
	}

	private void OnHover(Boolean isOver)
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (isOver)
		{
			EventDelegate.Execute(this.onHoverOver);
		}
		else
		{
			EventDelegate.Execute(this.onHoverOut);
		}
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnPress(Boolean pressed)
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (pressed)
		{
			EventDelegate.Execute(this.onPress);
		}
		else
		{
			EventDelegate.Execute(this.onRelease);
		}
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnSelect(Boolean selected)
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (selected)
		{
			EventDelegate.Execute(this.onSelect);
		}
		else
		{
			EventDelegate.Execute(this.onDeselect);
		}
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnClick()
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onClick);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnDoubleClick()
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDoubleClick);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnDragStart()
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragStart);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnDragEnd()
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragEnd);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnDragOver(GameObject go)
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragOver);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnDragOut(GameObject go)
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragOut);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	private void OnDrag(Vector2 delta)
	{
		if (UIEventTrigger.current != (UnityEngine.Object)null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDrag);
		UIEventTrigger.current = (UIEventTrigger)null;
	}

	public static UIEventTrigger current;

	public List<EventDelegate> onHoverOver = new List<EventDelegate>();

	public List<EventDelegate> onHoverOut = new List<EventDelegate>();

	public List<EventDelegate> onPress = new List<EventDelegate>();

	public List<EventDelegate> onRelease = new List<EventDelegate>();

	public List<EventDelegate> onSelect = new List<EventDelegate>();

	public List<EventDelegate> onDeselect = new List<EventDelegate>();

	public List<EventDelegate> onClick = new List<EventDelegate>();

	public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

	public List<EventDelegate> onDragStart = new List<EventDelegate>();

	public List<EventDelegate> onDragEnd = new List<EventDelegate>();

	public List<EventDelegate> onDragOver = new List<EventDelegate>();

	public List<EventDelegate> onDragOut = new List<EventDelegate>();

	public List<EventDelegate> onDrag = new List<EventDelegate>();
}
