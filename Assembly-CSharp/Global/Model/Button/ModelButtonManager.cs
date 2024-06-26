using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria;

public class ModelButtonManager : MonoBehaviour
{
	public Camera WorldCamera
	{
		get
		{
			return this.worldCamera;
		}
		set
		{
			this.worldCamera = value;
		}
	}

	private void Awake()
	{
		this.inactivePointer = new Dictionary<Int32, ModelButton>();
		this.activePointer = new Dictionary<Int32, ModelButton>();
		for (Int32 i = 0; i < 8; i++)
		{
			ModelButton component = this.ModelButtons[i].GetComponent<ModelButton>();
			this.inactivePointer.Add(i, component);
			component.gameObject.SetActive(false);
			UIEventListener uieventListener = UIEventListener.Get(component.gameObject);
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
			UIEventListener uieventListener2 = UIEventListener.Get(component.gameObject);
			uieventListener2.onHover = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onHover, new UIEventListener.BoolDelegate(this.onHover));
		}
	}

	public void Show(Transform targetModel, Int32 index, Boolean isEnemy, Single width, Single height)
	{
		if (this.inactivePointer.Count > 0)
		{
			ModelButton modelButton;
			if (!this.inactivePointer.TryGetValue(index, out modelButton))
			{
				if (this.activePointer.TryGetValue(index, out modelButton))
				{
					return;
				}
				global::Debug.LogError("Pointer index(" + index + ") not available!");
			}
			modelButton.worldCam = this.worldCamera;
			modelButton.gameObject.SetActive(true);
			modelButton.Show(index, isEnemy, targetModel, width, height, this.scale);
			this.inactivePointer.Remove(index);
			this.activePointer.Add(index, modelButton);
		}
		else
		{
			global::Debug.LogWarning("ModelPointerManager : Create() no pointer available!");
		}
	}

	public void Hide(Int32 index)
	{
		global::Debug.LogWarning("ModelPointerManager : Hide(" + index + ")");
		ModelButton modelButton;
		if (this.activePointer.TryGetValue(index, out modelButton))
		{
			modelButton.gameObject.SetActive(false);
			this.activePointer.Remove(index);
			this.inactivePointer.Add(index, modelButton);
		}
		else
		{
			global::Debug.LogWarning("ModelPointerManager : Hide(" + index + ") input id invalid!");
		}
	}

	public void Reset()
	{
		foreach (KeyValuePair<Int32, ModelButton> keyValuePair in this.activePointer)
		{
			ModelButton value = keyValuePair.Value;
			Int32 key = keyValuePair.Key;
			value.gameObject.SetActive(false);
			this.inactivePointer.Add(key, value);
		}
		this.activePointer.Clear();
	}

	public GameObject GetGameObject(Int32 index)
	{
		ModelButton modelButton;
		return (!this.activePointer.TryGetValue(index, out modelButton)) ? null : modelButton.gameObject;
	}

	public Transform GetTargetTransform(Int32 index)
	{
		ModelButton modelButton;
		return (!this.activePointer.TryGetValue(index, out modelButton)) ? null : modelButton.GetTargetTransform();
	}

	public List<Int32> GetAllPlayerIndex()
	{
		List<Int32> list = new List<Int32>();
		foreach (KeyValuePair<Int32, ModelButton> keyValuePair in this.activePointer)
		{
			if (!keyValuePair.Value.isEnemy)
			{
				list.Add(keyValuePair.Value.index);
			}
		}
		return list;
	}

	public List<Int32> GetAllEnemyIndex()
	{
		List<Int32> list = new List<Int32>();
		foreach (KeyValuePair<Int32, ModelButton> keyValuePair in this.activePointer)
		{
			if (keyValuePair.Value.isEnemy)
			{
				list.Add(keyValuePair.Value.index);
			}
		}
		return list;
	}

	public List<Int32> GetAllIndex()
	{
		List<Int32> list = new List<Int32>();
		foreach (KeyValuePair<Int32, ModelButton> keyValuePair in this.activePointer)
		{
			list.Add(keyValuePair.Value.index);
		}
		return list;
	}

	public void UpdateModelButtonPosition()
	{
		foreach (KeyValuePair<Int32, ModelButton> keyValuePair in this.activePointer)
		{
			keyValuePair.Value.UpdateModelButton();
		}
	}

	private void onClick(GameObject go)
	{
		if (Configuration.Control.DisableMouseInBattles)
			return;
		if (UIKeyTrigger.IsOnlyTouchAndLeftClick())
			UIManager.Battle.VerifyTarget(go.GetComponent<ModelButton>().index);
	}

	private void onHover(GameObject go, Boolean isHover)
	{
		if (Configuration.Control.DisableMouseInBattles)
			return;
		if (isHover && this.currentHoverIndex != go.GetComponent<ModelButton>().index)
		{
			this.currentHoverIndex = go.GetComponent<ModelButton>().index;
			UICamera.Notify(PersistenSingleton<UIManager>.Instance.gameObject, "OnItemSelect", go);
		}
	}

	public Single scale = 1.5f;

	public GameObject[] ModelButtons;

	private Camera worldCamera;

	private Dictionary<Int32, ModelButton> inactivePointer;

	private Dictionary<Int32, ModelButton> activePointer;

	private Int32 currentHoverIndex = -1;
}
