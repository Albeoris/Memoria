using System;
using UnityEngine;
using Memoria.Data;

public class SFXScreenShot : SFXMeshBase
{
	public SFXScreenShot()
	{
	}

	public SFXScreenShot(Int32 _rx, Int32 _ry, Int32 _x, Int32 _y)
	{
		SFXMeshBase.ssOffsetX = _rx;
		SFXMeshBase.ssOffsetY = _ry;
	}

	public override void Render(Int32 index)
	{
		if (screenshot == null)
		{
			screenshot = new Texture2D(256, 256, TextureFormat.ARGB32, false);
			screenshot.wrapMode = TextureWrapMode.Clamp;
			screenshot.filterMode = FilterMode.Bilinear;
		}
		// Take a screenshot then pretend the screen is 320x220 (full height)
		if (screenshotHD == null || Screen.width > screenshotHD.width || Screen.height > screenshotHD.height)
			screenshotHD = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
		screenshotHD.wrapMode = TextureWrapMode.Clamp;
		screenshotHD.filterMode = FilterMode.Bilinear;
		screenshotHD.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0); // Smaller area is not picked here because it doesn't seem to work well (Rect position ignored?)
		screenshotHD.Apply();
		// Pick the target 256x256 area of that 320x220 screenshot; the height is too small but hopefully the UV coordinates of visible vertices will not point to missing areas
		Single hdFactor = (Single)Screen.height / (Single)FieldMap.PsxScreenHeightNative;
		Int32 rx = (Int32)(hdFactor * SFXMeshBase.ssOffsetX + 0.5f * (Screen.width - FieldMap.PsxScreenWidthNative * Screen.height / FieldMap.PsxScreenHeightNative));
		Int32 ry = (Int32)(hdFactor * SFXMeshBase.ssOffsetY);
		for (Int32 x = 0; x < 256; x++)
			for (Int32 y = 0; y < 256; y++) // Also the screenshot texture is flipped vertically
				screenshot.SetPixel(x, y, screenshotHD.GetPixel((Int32)(rx + hdFactor * x), Screen.height - 1 - (Int32)(ry + hdFactor * y)));
		screenshot.Apply();
	}

	public static Texture2D screenshotHD = null;
	public static Texture2D screenshot = null;

	// The following is there a bit by default, not taking space somewhere else, but also because the Slow clock texture is related to the Slow screenshot as it gets twisted by it
	// Context: a clock sprite of the Slow SFX (as displayed in the PSX version) has been replaced by a purple glow in the port (most presumably the modification is in FF9SpecialEffectPlugin.dll)
	// We put that clock sprite back; the clock texture itself is still loaded in the Vram but the UV mapping is normally missing, so it's recreated here
	public static Boolean IsSpecialSlowTexture(UInt32 meshKey)
	{
		return SFX.currentEffectID == SpecialEffect.Slow && meshKey == 0x01000000u;
	}

	public static Vector2[] slowClockUV = new Vector2[]
	{
		new Vector2(108.8571f, 36.57143f),
		new Vector2(112f, 58.57143f),
		new Vector2(86.85714f, 42.85714f),
		new Vector2(90f, 58.57143f),
		new Vector2(96.28571f, 17.71428f),
		new Vector2(108.8571f, 36.57143f),
		new Vector2(80.57143f, 33.42857f),
		new Vector2(86.85714f, 42.85714f),
		new Vector2(77.42857f, 5.142857f),
		new Vector2(96.28571f, 17.71428f),
		new Vector2(68f, 27.14286f),
		new Vector2(80.57143f, 33.42857f),
		new Vector2(55.42857f, 2f),
		new Vector2(77.42857f, 5.142857f),
		new Vector2(55.42857f, 24f),
		new Vector2(68f, 27.14286f),
		new Vector2(36.57143f, 5.142857f),
		new Vector2(55.42857f, 2f),
		new Vector2(42.85714f, 27.14286f),
		new Vector2(55.42857f, 24f),
		new Vector2(17.71428f, 17.71428f),
		new Vector2(36.57143f, 5.142857f),
		new Vector2(33.42857f, 33.42857f),
		new Vector2(42.85714f, 27.14286f),
		new Vector2(5.142857f, 36.57143f),
		new Vector2(17.71428f, 17.71428f),
		new Vector2(24f, 42.85714f),
		new Vector2(33.42857f, 33.42857f),
		new Vector2(2f, 58.57143f),
		new Vector2(5.142857f, 36.57143f),
		new Vector2(24f, 58.57143f),
		new Vector2(24f, 42.85714f),
		new Vector2(5.142857f, 77.42857f),
		new Vector2(2f, 58.57143f),
		new Vector2(24f, 71.14285f),
		new Vector2(24f, 58.57143f),
		new Vector2(17.71428f, 96.28571f),
		new Vector2(5.142857f, 77.42857f),
		new Vector2(33.42857f, 80.57143f),
		new Vector2(24f, 71.14285f),
		new Vector2(36.57143f, 108.8571f),
		new Vector2(17.71428f, 96.28571f),
		new Vector2(42.85714f, 86.85714f),
		new Vector2(33.42857f, 80.57143f),
		new Vector2(55.42857f, 112f),
		new Vector2(36.57143f, 108.8571f),
		new Vector2(55.42857f, 90f),
		new Vector2(42.85714f, 86.85714f),
		new Vector2(77.42857f, 108.8571f),
		new Vector2(55.42857f, 112f),
		new Vector2(68f, 86.85714f),
		new Vector2(55.42857f, 90f),
		new Vector2(96.28571f, 96.28571f),
		new Vector2(77.42857f, 108.8571f),
		new Vector2(80.57143f, 80.57143f),
		new Vector2(68f, 86.85714f),
		new Vector2(108.8571f, 77.42857f),
		new Vector2(96.28571f, 96.28571f),
		new Vector2(86.85714f, 71.14285f),
		new Vector2(80.57143f, 80.57143f),
		new Vector2(112f, 58.57143f),
		new Vector2(108.8571f, 77.42857f),
		new Vector2(90f, 58.57143f),
		new Vector2(86.85714f, 71.14285f),
		new Vector2(86.85714f, 42.85714f),
		new Vector2(90f, 58.57143f),
		new Vector2(74.28571f, 49.14286f),
		new Vector2(74.28571f, 58.57143f),
		new Vector2(80.57143f, 33.42857f),
		new Vector2(86.85714f, 42.85714f),
		new Vector2(68f, 42.85714f),
		new Vector2(74.28571f, 49.14286f),
		new Vector2(68f, 27.14286f),
		new Vector2(80.57143f, 33.42857f),
		new Vector2(64.85714f, 39.71429f),
		new Vector2(68f, 42.85714f),
		new Vector2(55.42857f, 24f),
		new Vector2(68f, 27.14286f),
		new Vector2(55.42857f, 39.71429f),
		new Vector2(64.85714f, 39.71429f),
		new Vector2(42.85714f, 27.14286f),
		new Vector2(55.42857f, 24f),
		new Vector2(49.14286f, 39.71429f),
		new Vector2(55.42857f, 39.71429f),
		new Vector2(33.42857f, 33.42857f),
		new Vector2(42.85714f, 27.14286f),
		new Vector2(42.85714f, 42.85714f),
		new Vector2(49.14286f, 39.71429f),
		new Vector2(24f, 42.85714f),
		new Vector2(33.42857f, 33.42857f),
		new Vector2(39.71429f, 49.14286f),
		new Vector2(42.85714f, 42.85714f),
		new Vector2(24f, 58.57143f),
		new Vector2(24f, 42.85714f),
		new Vector2(36.57143f, 58.57143f),
		new Vector2(39.71429f, 49.14286f),
		new Vector2(24f, 71.14285f),
		new Vector2(24f, 58.57143f),
		new Vector2(39.71429f, 64.85714f),
		new Vector2(36.57143f, 58.57143f),
		new Vector2(33.42857f, 80.57143f),
		new Vector2(24f, 71.14285f),
		new Vector2(42.85714f, 71.14285f),
		new Vector2(39.71429f, 64.85714f),
		new Vector2(42.85714f, 86.85714f),
		new Vector2(33.42857f, 80.57143f),
		new Vector2(49.14286f, 74.28571f),
		new Vector2(42.85714f, 71.14285f),
		new Vector2(55.42857f, 90f),
		new Vector2(42.85714f, 86.85714f),
		new Vector2(55.42857f, 74.28571f),
		new Vector2(49.14286f, 74.28571f),
		new Vector2(68f, 86.85714f),
		new Vector2(55.42857f, 90f),
		new Vector2(64.85714f, 74.28571f),
		new Vector2(55.42857f, 74.28571f),
		new Vector2(80.57143f, 80.57143f),
		new Vector2(68f, 86.85714f),
		new Vector2(68f, 71.14285f),
		new Vector2(64.85714f, 74.28571f),
		new Vector2(86.85714f, 71.14285f),
		new Vector2(80.57143f, 80.57143f),
		new Vector2(74.28571f, 64.85714f),
		new Vector2(68f, 71.14285f),
		new Vector2(90f, 58.57143f),
		new Vector2(86.85714f, 71.14285f),
		new Vector2(74.28571f, 58.57143f),
		new Vector2(74.28571f, 64.85714f),
		new Vector2(74.28571f, 49.14286f),
		new Vector2(74.28571f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(68f, 42.85714f),
		new Vector2(74.28571f, 49.14286f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(64.85714f, 39.71429f),
		new Vector2(68f, 42.85714f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 39.71429f),
		new Vector2(64.85714f, 39.71429f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(49.14286f, 39.71429f),
		new Vector2(55.42857f, 39.71429f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(42.85714f, 42.85714f),
		new Vector2(49.14286f, 39.71429f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(39.71429f, 49.14286f),
		new Vector2(42.85714f, 42.85714f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(36.57143f, 58.57143f),
		new Vector2(39.71429f, 49.14286f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(39.71429f, 64.85714f),
		new Vector2(36.57143f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(42.85714f, 71.14285f),
		new Vector2(39.71429f, 64.85714f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(49.14286f, 74.28571f),
		new Vector2(42.85714f, 71.14285f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 74.28571f),
		new Vector2(49.14286f, 74.28571f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(64.85714f, 74.28571f),
		new Vector2(55.42857f, 74.28571f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(68f, 71.14285f),
		new Vector2(64.85714f, 74.28571f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(74.28571f, 64.85714f),
		new Vector2(68f, 71.14285f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(74.28571f, 58.57143f),
		new Vector2(74.28571f, 64.85714f),
		new Vector2(55.42857f, 58.57143f),
		new Vector2(55.42857f, 58.57143f)
	};
}
