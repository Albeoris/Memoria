using System;
using UnityEngine;
using Object = System.Object;

public class HonoBehavior : MonoBehaviour
{
	public void RegisterHonoBehavior()
	{
		HonoBehaviorSystem.AddBehavior(this);
	}

	public void UnregisterHonoBehavior(Boolean dispose = false)
	{
		HonoBehaviorSystem.RemoveBehavior(this, dispose);
	}

	private void Awake()
	{
		this.RegisterHonoBehavior();
		if (base.GetComponent<HonoEventObject>() == (UnityEngine.Object)null)
		{
			base.gameObject.AddComponent<HonoEventObject>();
		}
	}

	public void HonoDefaultStartFastForwardMode()
	{
		Int32 fastForwardFactor = HonoBehaviorSystem.Instance.GetFastForwardFactor();
		foreach (Object obj in base.transform.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			animationState.speed = (Single)fastForwardFactor;
		}
	}

	public void HonoDefaultStopFastForwardMode()
	{
		foreach (Object obj in base.transform.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			animationState.speed = 1f;
		}
	}

	private void OnDestroy()
	{
		this.UnregisterHonoBehavior(false);
	}

	public virtual void HonoAwake()
	{
	}

	public virtual void HonoStart()
	{
	}

	public virtual void HonoUpdate()
	{
	}

	public virtual void HonoLateUpdate()
	{
	}

	public virtual void HonoOnGUI()
	{
	}

	public virtual void HonoOnDestroy()
	{
	}

	public virtual void HonoOnStartFastForwardMode()
	{
	}

	public virtual void HonoOnStopFastForwardMode()
	{
	}

	public Boolean IsVisibled()
	{
		HonoEventObject component = base.GetComponent<HonoEventObject>();
		return component.IsVisibled();
	}

	public Boolean IsEnabled()
	{
		HonoEventObject component = base.GetComponent<HonoEventObject>();
		return component.IsEnabled();
	}

	public void SetVisible(Boolean value)
	{
		HonoEventObject component = base.GetComponent<HonoEventObject>();
		component.SetVisible(value);
	}

	public void SetEnabled(Boolean value)
	{
		HonoEventObject component = base.GetComponent<HonoEventObject>();
		component.SetEnabled(value);
	}

	public void SetVisible(Boolean value, Boolean isBattle)
	{
		HonoEventObject component = base.GetComponent<HonoEventObject>();
		component.SetVisible(value);
		if (!value)
		{
			return;
		}
		if (isBattle)
		{
			Transform transform = component.transform.FindChild("field_model");
			if (transform == (UnityEngine.Object)null)
			{
				return;
			}
			Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				Renderer renderer = array[i];
				renderer.enabled = false;
			}
		}
		else
		{
			Transform transform2 = component.transform.FindChild("battle_model");
			if (transform2 == (UnityEngine.Object)null)
			{
				return;
			}
			Renderer[] componentsInChildren2 = transform2.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren2;
			for (Int32 j = 0; j < (Int32)array2.Length; j++)
			{
				Renderer renderer2 = array2[j];
				renderer2.enabled = false;
			}
		}
	}
}
