using System;
using UnityEngine;

public class QuadMistCursor : MonoBehaviour
{
	public Boolean Active
	{
		get
		{
			return this.cursor.enabled;
		}
		set
		{
			this.cursor.enabled = value;
			this.shadow.enabled = value;
		}
	}

	public Boolean Black
	{
		set
		{
			if (value)
			{
				this.cursor.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			}
			else
			{
				this.cursor.color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}

	private void Update()
	{
		if (this.clip != (UnityEngine.Object)null)
		{
			if (this.time >= Anim.TickToTime(6))
			{
				this.time = 0f;
				this.i = (this.i + 1) % 7;
				this.cursor.sprite = this.clip.sheet[this.i];
				this.shadow.sprite = this.clip.sheet[this.i];
			}
			this.time += Time.deltaTime;
		}
	}

	public SpriteRenderer cursor;

	public SpriteRenderer shadow;

	public SpriteSheet clip;

	private Single time;

	private Int32 i;
}
