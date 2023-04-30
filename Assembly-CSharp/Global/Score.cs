using System;
using UnityEngine;

public class Score : MonoBehaviour
{
	public Int32 EnemyScore
	{
		set => enemy.Text = value == 10 ? "a" : value.ToString();
	}

	public Int32 PlayerScore
	{
		set => player.Text = value == 10 ? "a" : value.ToString();
	}

	public SpriteText player;
	public SpriteText enemy;
}
