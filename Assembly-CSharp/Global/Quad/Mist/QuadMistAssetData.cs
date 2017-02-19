using System;
using UnityEngine;

public class QuadMistAssetData
{
	public QuadMistAssetData(String assetName, Int32 code, Sprite sprite)
	{
		this.AssetName = assetName;
		this.Code = code;
		this.Sprite = sprite;
	}

	public String AssetName;

	public Int32 Code;

	public Sprite Sprite;
}
