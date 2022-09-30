using System;
using UnityEngine;

public class CommonState : MonoBehaviour
{
	private void Awake()
	{
		this.Init();
	}

	public void Init()
	{
		this.FF9 = new FF9StateGlobal();
	}

	public FF9StateGlobal FF9;

	[NonSerialized]
	public Int32 PlayerCount = DEFAULT_PLAYER_COUNT;
	[NonSerialized]
	public Int32 PlayerPresetCount = DEFAULT_PRESET_COUNT;

	public const Int32 DEFAULT_PLAYER_COUNT = 12;
	public const Int32 DEFAULT_PRESET_COUNT = 16;
}
