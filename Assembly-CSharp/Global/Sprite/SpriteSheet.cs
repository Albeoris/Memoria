using System;
using UnityEngine;

public class SpriteSheet : ScriptableObject
{
	public Sprite this[Int32 i]
	{
		get
		{
			return this.sheet[i];
		}
	}

	public Sprite[] sheet;
}
