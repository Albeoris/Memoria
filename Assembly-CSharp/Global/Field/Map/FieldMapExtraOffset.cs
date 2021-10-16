using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Assets;
using UnityEngine;

public class FieldMapExtraOffset
{
	public void Load()
	{
		this.offsetDict = new Dictionary<String, FieldMapExtraOffset.ExtraOffset[]>();
		this.notMoveDict = new Dictionary<String, FieldMapExtraOffset.ExtraOffset[]>();
		String name = "EmbeddedAsset/Manifest/FieldMap/mapExtraOffsetList.txt";
		String textAsset = AssetManager.LoadString(name, out _);
		StringReader stringReader = new StringReader(textAsset);
		String text;
		while ((text = stringReader.ReadLine()) != null)
		{
			String[] array = text.Split(new Char[]
			{
				','
			});
			String key = array[0];
			Int32 num = 0;
			Int32.TryParse(array[1], out num);
			if (num > 0)
			{
				FieldMapExtraOffset.ExtraOffset[] array2 = new FieldMapExtraOffset.ExtraOffset[num];
				for (Int32 i = 0; i < num; i++)
				{
					array2[i] = new FieldMapExtraOffset.ExtraOffset();
					Int32.TryParse(array[i * 3 + 2], out array2[i].overlayIndex);
					Int32.TryParse(array[i * 3 + 3], out array2[i].spriteIndex);
					Int32.TryParse(array[i * 3 + 4], out array2[i].zOffset);
				}
				this.offsetDict.Add(key, array2);
			}
			else
			{
				num *= -1;
				FieldMapExtraOffset.ExtraOffset[] array3 = new FieldMapExtraOffset.ExtraOffset[num];
				for (Int32 j = 0; j < num; j++)
				{
					array3[j] = new FieldMapExtraOffset.ExtraOffset();
					Int32.TryParse(array[j * 3 + 2], out array3[j].overlayIndex);
					Int32.TryParse(array[j * 3 + 3], out array3[j].spriteIndex);
					Int32.TryParse(array[j * 3 + 4], out array3[j].zOffset);
				}
				this.notMoveDict.Add(key, array3);
			}
		}
	}

	public void SetOffset(String name, List<BGOVERLAY_DEF> overlayList)
	{
		if (this.offsetDict.ContainsKey(name))
		{
			FieldMapExtraOffset.ExtraOffset[] array = this.offsetDict[name];
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				FieldMapExtraOffset.ExtraOffset extraOffset = array[i];
				if (extraOffset.spriteIndex == -1)
				{
					BGOVERLAY_DEF bgoverlay_DEF = overlayList[extraOffset.overlayIndex];
					bgoverlay_DEF.orgZ = (UInt16)(bgoverlay_DEF.orgZ + (UInt16)extraOffset.zOffset);
					BGOVERLAY_DEF bgoverlay_DEF2 = overlayList[extraOffset.overlayIndex];
					bgoverlay_DEF2.curZ = (UInt16)(bgoverlay_DEF2.curZ + (UInt16)extraOffset.zOffset);
				}
				else if (extraOffset.spriteIndex == -2)
				{
					overlayList[extraOffset.overlayIndex].orgZ = (UInt16)extraOffset.zOffset;
					overlayList[extraOffset.overlayIndex].curZ = (UInt16)extraOffset.zOffset;
				}
				else if (!(Localization.CurrentLanguage == "Japanese") || !(name == "FBG_N18_GTRE_MAP360_GT_GRD_0") || extraOffset.overlayIndex != 12 || extraOffset.spriteIndex != 12)
				{
					overlayList[extraOffset.overlayIndex].spriteList[extraOffset.spriteIndex].depth += extraOffset.zOffset;
				}
			}
		}
	}

	public void UpdateOverlayOffset(String name, Int32 overlayIndex, ref Int16 dz)
	{
		if (this.notMoveDict.ContainsKey(name))
		{
			for (Int32 i = 0; i < (Int32)this.notMoveDict[name].Length; i++)
			{
				if (this.notMoveDict[name][i].overlayIndex == overlayIndex)
				{
					dz = (Int16)this.notMoveDict[name][i].zOffset;
				}
			}
		}
	}

	public Dictionary<String, FieldMapExtraOffset.ExtraOffset[]> offsetDict;

	public Dictionary<String, FieldMapExtraOffset.ExtraOffset[]> notMoveDict;

	public class ExtraOffset
	{
		public Int32 overlayIndex;

		public Int32 spriteIndex;

		public Int32 zOffset;
	}
}
