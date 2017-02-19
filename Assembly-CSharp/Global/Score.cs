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
				this.enemy.Text = "a";
			}
			else
			{
				this.enemy.Text = value.ToString();
			}
		}
	}

	public Int32 PlayerScore
	{
		set
		{
			if (value == 10)
			{
				this.player.Text = "a";
			}
			else
			{
				this.player.Text = value.ToString();
			}
		}
	}

	public SpriteText player;

	public SpriteText enemy;
}
