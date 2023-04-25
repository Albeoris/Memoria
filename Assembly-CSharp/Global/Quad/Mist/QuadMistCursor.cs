using System;
using UnityEngine;

public class QuadMistCursor : MonoBehaviour
{
	public Boolean Active
	{
		get
		{
			return cursor.enabled;
		}
		set
		{
			cursor.enabled = value;
			shadow.enabled = value;
		}
	}

	public Boolean Black
	{
		set
		{
			if (value)
			{
				cursor.color = new Color(0.5f, 0.5f, 0.5f, -1f);
			}
			else
			{
				cursor.color = new Color(1f, 1f, 1f, -1f);
			}
		}
	}

	private void Update()
	{
		if (clip != (UnityEngine.Object)null)
		{
			if (time >= Anim.TickToTime(6))
			{
				time = 0f;
				i = (i + 1) % 7;
				cursor.sprite = clip.sheet[i];
				shadow.sprite = clip.sheet[i];
			}
			time += Time.deltaTime;
		}
	}

	public SpriteRenderer cursor;

	public SpriteRenderer shadow;

	public SpriteSheet clip;

	private Single time;

	private Int32 i;
}
