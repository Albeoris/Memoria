using System;
using UnityEngine;

public class MiniGameState : MonoBehaviour
{
	private void Awake()
	{
		Transform transform = base.transform;
		GameObject gameObject = new GameObject("QuadMistCardPool");
		this.cardPool = gameObject.AddComponent<CardPool>();
		gameObject.transform.parent = transform;
	}

	private void Start()
	{
		this.NewGame();
	}

	public Int32 GetNumberOfCards()
	{
		return this.SavedData.MiniGameCard.Count;
	}

	public void NewGame()
	{
		FF9StateSystem.Common.FF9.miniGameArg = 0;
		this.SavedData = new FF9SAVE_MINIGAME();
		QuadMistDatabase.LoadData();
	}

	public FF9SAVE_MINIGAME SavedData;

	private CardPool cardPool;
}
