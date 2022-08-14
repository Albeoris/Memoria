using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria;

public class HonoBehaviorSystem : MonoBehaviour
{
	public static HonoBehaviorSystem Instance
	{
		get
		{
			if (HonoBehaviorSystem._instance != (UnityEngine.Object)null)
			{
				return HonoBehaviorSystem._instance;
			}
			HonoBehaviorSystem._instance = (HonoBehaviorSystem)UnityEngine.Object.FindObjectOfType(typeof(HonoBehaviorSystem));
			if ((Int32)UnityEngine.Object.FindObjectsOfType(typeof(HonoBehaviorSystem)).Length > 1)
			{
				return HonoBehaviorSystem._instance;
			}
			GameObject gameObject = new GameObject(typeof(HonoBehaviorSystem).Name);
			HonoBehaviorSystem._instance = gameObject.AddComponent<HonoBehaviorSystem>();
			HonoBehaviorSystem._instance._Init();
			return HonoBehaviorSystem._instance;
		}
	}

	private void _Init()
	{
		this._behaviorList = new List<HonoBehavior>();
		this._justAddLists = new List<HonoBehavior>[2];
		this._justAddLists[0] = new List<HonoBehavior>();
		this._justAddLists[1] = new List<HonoBehavior>();
		this._justRemoveList = new List<HonoBehavior>();
		this._justDisposeList = new List<GameObject>();
		this._curJustAddListIndex = 0;
		this._fastMode = false;
		this._nextFastMode = this._fastMode;
	}

	private void _SwapJustAddList()
	{
		if (this._curJustAddListIndex == 0)
		{
			this._curJustAddListIndex = 1;
		}
		else
		{
			this._curJustAddListIndex = 0;
		}
	}

	private List<HonoBehavior> _GetCurrentJustAddList()
	{
		return this._justAddLists[this._curJustAddListIndex];
	}

	public void StartFastForwardMode()
	{
		this._nextFastMode = true;
	}

	public void StopFastForwardMode()
	{
		this._nextFastMode = false;
	}

	public Boolean IsFastForwardModeActive()
	{
		return this._fastMode && this._nextFastMode;
	}

	public Int32 GetFastForwardFactor()
	{
		return FF9StateSystem.Settings.FastForwardFactor;
	}

	private void Awake()
	{
		if (HonoBehaviorSystem._instance == (UnityEngine.Object)null)
		{
			return;
		}
		HonoBehaviorSystem[] array = UnityEngine.Object.FindObjectsOfType<HonoBehaviorSystem>();
		if ((Int32)array.Length == 1)
		{
			return;
		}
		HonoBehaviorSystem[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			HonoBehaviorSystem honoBehaviorSystem = array2[i];
			if (honoBehaviorSystem != HonoBehaviorSystem._instance)
			{
				UnityEngine.Object.Destroy(honoBehaviorSystem.gameObject);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
		{
			return;
		}
		Int32 num = 1;
		if (this._fastMode)
		{
			num = this.GetFastForwardFactor();
		}
		for (Int32 i = 0; i < num; i++)
		{
			if (HonoBehaviorSystem.IdleLoopCount > 0)
			{
				HonoBehaviorSystem.IdleLoopCount--;
			}
			else
			{
				if (this._fastMode != this._nextFastMode)
				{
					if (this._nextFastMode)
					{
						for (Int32 j = 0; j < this._behaviorList.Count; j++)
						{
							this._behaviorList[j].HonoOnStartFastForwardMode();
						}
					}
					else
					{
						for (Int32 k = 0; k < this._behaviorList.Count; k++)
						{
							this._behaviorList[k].HonoOnStopFastForwardMode();
						}
					}
					this._fastMode = this._nextFastMode;
				}
				for (Int32 l = 0; l < this._behaviorList.Count; l++)
				{
					if (this._behaviorList[l].IsEnabled())
					{
						this._behaviorList[l].HonoUpdate();
					}
				}
				while (this._justAddLists[0].Count != 0 || this._justAddLists[1].Count != 0)
				{
					List<HonoBehavior> list = this._GetCurrentJustAddList();
					this._SwapJustAddList();
					for (Int32 m = 0; m < list.Count; m++)
					{
						list[m].HonoStart();
						if (this.IsFastForwardModeActive())
						{
							list[m].HonoOnStartFastForwardMode();
						}
						else
						{
							list[m].HonoOnStopFastForwardMode();
						}
					}
					for (Int32 n = 0; n < list.Count; n++)
					{
						if (list[n].IsEnabled())
						{
							list[n].HonoUpdate();
						}
						this._behaviorList.Add(list[n]);
					}
					list.Clear();
				}
				for (Int32 num2 = 0; num2 < this._behaviorList.Count; num2++)
				{
					if (this._behaviorList[num2].IsEnabled())
					{
						this._behaviorList[num2].HonoLateUpdate();
					}
				}
				for (Int32 num3 = 0; num3 < this._justRemoveList.Count; num3++)
				{
					this._justRemoveList[num3].HonoOnDestroy();
					Int32 num4 = this._behaviorList.IndexOf(this._justRemoveList[num3]);
					if (num4 != -1)
					{
						this._behaviorList.RemoveAt(num4);
						UnityEngine.Object.Destroy(this._justRemoveList[num3]);
					}
				}
				this._justRemoveList.Clear();
				for (Int32 num5 = 0; num5 < this._justDisposeList.Count; num5++)
				{
					HonoBehaviorSystem.RemoveGameObject(this._justDisposeList[num5]);
				}
				this._justDisposeList.Clear();
				if (HonoBehaviorSystem.ExtraLoopCount > 0)
				{
					i -= HonoBehaviorSystem.ExtraLoopCount;
					HonoBehaviorSystem.ExtraLoopCount = 0;
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (!HonoBehaviorSystem.FrameSkipEnabled)
		{
			return;
		}
		Single deltaTime = Time.deltaTime;
		HonoBehaviorSystem._cumulativeTime += deltaTime;
		Int32 num = Mathf.RoundToInt(HonoBehaviorSystem._cumulativeTime / HonoBehaviorSystem.TargetFrameTime);
		if (num == 0)
		{
			HonoBehaviorSystem.IdleLoopCount = 1;
			return;
		}
		if (num == 1)
		{
			HonoBehaviorSystem._cumulativeTime -= HonoBehaviorSystem.TargetFrameTime;
			return;
		}
		HonoBehaviorSystem._cumulativeTime -= HonoBehaviorSystem.TargetFrameTime * (Single)num;
		HonoBehaviorSystem.ExtraLoopCount = num - 1;
	}

	private void OnGUI()
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
		{
			return;
		}
		for (Int32 i = 0; i < this._behaviorList.Count; i++)
		{
			this._behaviorList[i].HonoOnGUI();
		}
	}

	private void OnDestroy()
	{
		for (Int32 i = 0; i < this._justAddLists[0].Count; i++)
		{
			this._justAddLists[0][i].HonoOnDestroy();
		}
		this._justAddLists[0].Clear();
		for (Int32 j = 0; j < this._justAddLists[1].Count; j++)
		{
			this._justAddLists[1][j].HonoOnDestroy();
		}
		this._justAddLists[1].Clear();
		for (Int32 k = 0; k < this._behaviorList.Count; k++)
		{
			this._behaviorList[k].HonoOnDestroy();
		}
		this._behaviorList.Clear();
		HonoBehaviorSystem._instance = (HonoBehaviorSystem)null;
	}

	public static void AddBehavior(HonoBehavior h)
	{
		Int32 num = HonoBehaviorSystem.Instance._GetCurrentJustAddList().IndexOf(h);
		if (num != -1)
		{
			return;
		}
		h.HonoAwake();
		HonoBehaviorSystem.Instance._GetCurrentJustAddList().Add(h);
	}

	public static void RemoveBehavior(HonoBehavior h, Boolean dispose = false)
	{
		if (HonoBehaviorSystem._instance == (UnityEngine.Object)null)
		{
			return;
		}
		Int32 num = HonoBehaviorSystem.Instance._justRemoveList.IndexOf(h);
		if (num != -1)
		{
			return;
		}
		HonoBehaviorSystem.Instance._justRemoveList.Add(h);
		if (dispose)
		{
			HonoBehaviorSystem.Instance._justDisposeList.Add(h.gameObject);
		}
	}

	private static void RemoveGameObject(GameObject gameObject)
	{
		if (gameObject == (UnityEngine.Object)null)
		{
			return;
		}
		HonoBehavior[] componentsInChildren = gameObject.GetComponentsInChildren<HonoBehavior>();
		HonoBehavior[] array = componentsInChildren;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			HonoBehavior honoBehavior = array[i];
			Int32 num = HonoBehaviorSystem.Instance._behaviorList.IndexOf(honoBehavior);
			if (num != -1)
			{
				HonoBehaviorSystem.Instance._behaviorList.RemoveAt(num);
				UnityEngine.Object.Destroy(honoBehavior);
			}
		}
		UnityEngine.Object.Destroy(gameObject);
	}

	private static HonoBehaviorSystem _instance;

	private List<HonoBehavior> _behaviorList;

	private List<HonoBehavior>[] _justAddLists;

	private List<HonoBehavior> _justRemoveList;

	private List<GameObject> _justDisposeList;

	private Int32 _curJustAddListIndex;

	private Boolean _fastMode;

	private Boolean _nextFastMode;

	public static Int32 IdleLoopCount;

	public static Int32 ExtraLoopCount;

	public static Boolean FrameSkipEnabled;

	public static Single TargetFrameTime = 1f / (float)Configuration.Graphics.BattleFPS;

	public static Single _cumulativeTime;
}
