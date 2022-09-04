using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using Memoria;

public class HonoBehaviorSystem : MonoBehaviour
{
	public static HonoBehaviorSystem Instance
	{
		get
		{
			if (HonoBehaviorSystem._instance != null)
				return HonoBehaviorSystem._instance;
			HonoBehaviorSystem._instance = (HonoBehaviorSystem)UnityEngine.Object.FindObjectOfType(typeof(HonoBehaviorSystem));
			if (UnityEngine.Object.FindObjectsOfType(typeof(HonoBehaviorSystem)).Length > 1)
				return HonoBehaviorSystem._instance;
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
			this._curJustAddListIndex = 1;
		else
			this._curJustAddListIndex = 0;
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
		if (HonoBehaviorSystem._instance == null)
			return;
		HonoBehaviorSystem[] systemArray = UnityEngine.Object.FindObjectsOfType<HonoBehaviorSystem>();
		if (systemArray.Length == 1)
			return;
		for (Int32 i = 0; i < systemArray.Length; i++)
			if (systemArray[i] != HonoBehaviorSystem._instance)
				UnityEngine.Object.Destroy(systemArray[i].gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
			return;
		if (this._fastMode != this._nextFastMode)
		{
			if (this._nextFastMode)
				for (Int32 i = 0; i < this._behaviorList.Count; i++)
					this._behaviorList[i].HonoOnStartFastForwardMode();
			else
				for (Int32 i = 0; i < this._behaviorList.Count; i++)
					this._behaviorList[i].HonoOnStopFastForwardMode();
			this._fastMode = this._nextFastMode;
		}
		Boolean inField = SceneDirector.IsFieldScene();
		Boolean inWorld = SceneDirector.IsWorldScene();
		for (Int32 updateCount = 0; updateCount < FPSManager.MainLoopUpdateCount; updateCount++)
		{
			if (inField)
				SmoothFrameUpdater_Field.ResetState();
			else if (inWorld)
				SmoothFrameUpdater_World.ResetState();
			for (Int32 i = 0; i < this._behaviorList.Count; i++)
			{
				if (this._behaviorList[i].IsEnabled())
				{
					if (updateCount == 0)
						this._behaviorList[i].HonoFixedUpdate();
					this._behaviorList[i].HonoUpdate();
				}
			}
			while (this._justAddLists[0].Count != 0 || this._justAddLists[1].Count != 0)
			{
				List<HonoBehavior> list = this._GetCurrentJustAddList();
				this._SwapJustAddList();
				for (Int32 i = 0; i < list.Count; i++)
				{
					list[i].HonoStart();
					if (this.IsFastForwardModeActive())
						list[i].HonoOnStartFastForwardMode();
					else
						list[i].HonoOnStopFastForwardMode();
				}
				for (Int32 i = 0; i < list.Count; i++)
				{
					if (list[i].IsEnabled())
					{
						if (updateCount == 0)
							list[i].HonoFixedUpdate();
						list[i].HonoUpdate();
					}
					this._behaviorList.Add(list[i]);
				}
				list.Clear();
			}
			for (Int32 i = 0; i < this._behaviorList.Count; i++)
				if (this._behaviorList[i].IsEnabled())
					this._behaviorList[i].HonoLateUpdate();
			for (Int32 i = 0; i < this._justRemoveList.Count; i++)
			{
				this._justRemoveList[i].HonoOnDestroy();
				Int32 indexInList = this._behaviorList.IndexOf(this._justRemoveList[i]);
				if (indexInList != -1)
				{
					this._behaviorList.RemoveAt(indexInList);
					UnityEngine.Object.Destroy(this._justRemoveList[i]);
				}
			}
			this._justRemoveList.Clear();
			for (Int32 i = 0; i < this._justDisposeList.Count; i++)
				HonoBehaviorSystem.RemoveGameObject(this._justDisposeList[i]);
			this._justDisposeList.Clear();
			if (HonoBehaviorSystem.ExtraLoopCount > 0)
			{
				updateCount -= HonoBehaviorSystem.ExtraLoopCount;
				HonoBehaviorSystem.ExtraLoopCount = 0;
			}
			if (inField)
				SmoothFrameUpdater_Field.RegisterState();
			else if (inWorld)
				SmoothFrameUpdater_World.RegisterState();
		}
		if (inField)
			FPSManager.AddSmoothEffect(SmoothFrameUpdater_Field.Apply);
		else if (inWorld)
			FPSManager.AddSmoothEffect(SmoothFrameUpdater_World.Apply);
	}

	private void LateUpdate()
	{
		return;
		if (!HonoBehaviorSystem.FrameSkipEnabled)
			return;
		Single deltaTime = Time.deltaTime;
		HonoBehaviorSystem._cumulativeTime += deltaTime;
		Int32 num = Mathf.RoundToInt(HonoBehaviorSystem._cumulativeTime / HonoBehaviorSystem.TargetFrameTime);
		if (num == 0)
		{
			HonoBehaviorSystem.IdleLoopCount = 1;
		}
		else if (num == 1)
		{
			HonoBehaviorSystem._cumulativeTime -= HonoBehaviorSystem.TargetFrameTime;
		}
		else
		{
			HonoBehaviorSystem._cumulativeTime -= HonoBehaviorSystem.TargetFrameTime * (Single)num;
			HonoBehaviorSystem.ExtraLoopCount = num - 1;
		}
	}

	private void OnGUI()
	{
		if (PersistenSingleton<UIManager>.Instance.IsPause)
			return;
		for (Int32 i = 0; i < this._behaviorList.Count; i++)
			this._behaviorList[i].HonoOnGUI();
	}

	private void OnDestroy()
	{
		for (Int32 i = 0; i < this._justAddLists[0].Count; i++)
			this._justAddLists[0][i].HonoOnDestroy();
		this._justAddLists[0].Clear();
		for (Int32 j = 0; j < this._justAddLists[1].Count; j++)
			this._justAddLists[1][j].HonoOnDestroy();
		this._justAddLists[1].Clear();
		for (Int32 k = 0; k < this._behaviorList.Count; k++)
			this._behaviorList[k].HonoOnDestroy();
		this._behaviorList.Clear();
		HonoBehaviorSystem._instance = (HonoBehaviorSystem)null;
	}

	public static void AddBehavior(HonoBehavior h)
	{
		if (HonoBehaviorSystem.Instance._GetCurrentJustAddList().IndexOf(h) != -1)
			return;
		h.HonoAwake();
		HonoBehaviorSystem.Instance._GetCurrentJustAddList().Add(h);
	}

	public static void RemoveBehavior(HonoBehavior h, Boolean dispose = false)
	{
		if (HonoBehaviorSystem._instance == null)
			return;
		if (HonoBehaviorSystem.Instance._justRemoveList.IndexOf(h) != -1)
			return;
		HonoBehaviorSystem.Instance._justRemoveList.Add(h);
		if (dispose)
			HonoBehaviorSystem.Instance._justDisposeList.Add(h.gameObject);
	}

	private static void RemoveGameObject(GameObject gameObject)
	{
		if (gameObject == null)
			return;
		HonoBehavior[] componentsInChildren = gameObject.GetComponentsInChildren<HonoBehavior>();
		for (Int32 i = 0; i < componentsInChildren.Length; i++)
		{
			HonoBehavior honoBehavior = componentsInChildren[i];
			Int32 index = HonoBehaviorSystem.Instance._behaviorList.IndexOf(honoBehavior);
			if (index != -1)
			{
				HonoBehaviorSystem.Instance._behaviorList.RemoveAt(index);
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

	public static Single TargetFrameTime = 0.0333333351f;

	public static Single _cumulativeTime;
}
