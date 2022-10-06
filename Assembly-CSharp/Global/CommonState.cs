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

	public void ChangePlayerCount(Int32 newCount)
	{
		Int32 oldCount = this.FF9.player.Length;
		Array.Resize(ref this.FF9.player, newCount);
		PlayerCount = newCount;
		for (Int32 i = oldCount; i < newCount; i++)
			this.FF9.player[i] = new PLAYER();
	}

	public FF9StateGlobal FF9;

	[NonSerialized]
	public Int32 PlayerCount = DEFAULT_PLAYER_COUNT;
	[NonSerialized]
	public Int32 PlayerPresetCount = DEFAULT_PRESET_COUNT;

	public const Int32 DEFAULT_PLAYER_COUNT = 12;
	public const Int32 DEFAULT_PRESET_COUNT = 16;
}
