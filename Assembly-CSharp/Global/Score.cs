using System;
using UnityEngine;

public class Score : MonoBehaviour
{
	public Int32 EnemyScore
	{
		set
		{
			if (value == 10)
			{
				enemy.Text = "a";
			}
			else
			{
				enemy.Text = value.ToString();
			}
		}
	}

	public Int32 PlayerScore
	{
		set
		{
			if (value == 10)
			{
				player.Text = "a";
			}
			else
			{
				player.Text = value.ToString();
			}
		}
	}

	public SpriteText player;

	public SpriteText enemy;
}
