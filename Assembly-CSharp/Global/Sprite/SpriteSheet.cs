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
	public SpriteSheet.Info[] info; // Should be either null or same size as sheet
	public Boolean appendTexture;

	public class Info
	{
		public Vector4 padding;
		public Vector4 border;
	}
}
