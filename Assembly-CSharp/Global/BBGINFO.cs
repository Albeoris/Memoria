using System;
using System.IO;
using UnityEngine;

public class BBGINFO
{
	public void ReadBattleInfo(String battleModelPath)
	{
		String name = "BattleMap/BattleInfo/" + battleModelPath.Replace("BBG", "INB") + ".inb";
		String[] battleInfo;
		Byte[] binAsset = AssetManager.LoadBytes(name, out battleInfo);
		if (binAsset == null)
		{
			global::Debug.LogWarning("Cannot find BattleInfo for : " + battleModelPath);
			return;
		}
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
		{
			this.bbgnumber = binaryReader.ReadInt16();
			this.texanim = binaryReader.ReadInt16();
			this.skyrotation = binaryReader.ReadInt16();
			this.fog = binaryReader.ReadInt16();
			this.objanim = binaryReader.ReadInt16();
			this.uvcount = binaryReader.ReadInt16();
			this.chr_r = binaryReader.ReadByte();
			this.chr_g = binaryReader.ReadByte();
			this.chr_b = binaryReader.ReadByte();
			this.shadow = binaryReader.ReadByte();
		}
	}

	public Int16 bbgnumber;

	public Int16 texanim;

	public Int16 skyrotation;

	public Int16 fog;

	public Int16 objanim;

	public Int16 uvcount;

	public Byte chr_r;

	public Byte chr_g;

	public Byte chr_b;

	public Byte shadow;
}
