using System;
using System.IO;
using Memoria;
using Memoria.Data;
using UnityEngine;

public class BTL_SCENE
{
	public void ReadBattleScene(String name)
	{
		nameIdentifier = "BSC_" + name;
		name = "EVT_BATTLE_" + name;
		this.header = new SB2_HEAD();
		String[] bsceneInfo;
		Byte[] bytes = AssetManager.LoadBytes("BattleMap/BattleScene/" + name + "/dbfile0000.raw16", out bsceneInfo);
		if (bytes == null)
			return;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes)))
		{
			this.header.Ver = binaryReader.ReadByte();
			this.header.PatCount = binaryReader.ReadByte();
			this.header.TypCount = binaryReader.ReadByte();
			this.header.AtkCount = binaryReader.ReadByte();
			this.header.Flags = binaryReader.ReadUInt16();
			this.PatAddr = new SB2_PATTERN[(Int32)this.header.PatCount];
			this.MonAddr = new SB2_MON_PARM[(Int32)this.header.TypCount];
			this.atk = new AA_DATA[(Int32)this.header.AtkCount];
			binaryReader.BaseStream.Seek(8L, SeekOrigin.Begin);
			for (Int32 i = 0; i < (Int32)this.header.PatCount; i++)
			{
				SB2_PATTERN sb2_PATTERN = this.PatAddr[i] = new SB2_PATTERN();
				sb2_PATTERN.Rate = binaryReader.ReadByte();
				sb2_PATTERN.MonCount = binaryReader.ReadByte();
				sb2_PATTERN.Camera = binaryReader.ReadByte();
				sb2_PATTERN.Pad0 = binaryReader.ReadByte();
				sb2_PATTERN.AP = binaryReader.ReadUInt32();
				for (Int32 j = 0; j < 4; j++)
				{
					SB2_PUT sb2_PUT = sb2_PATTERN.Put[j] = new SB2_PUT();
					sb2_PUT.TypeNo = binaryReader.ReadByte();
					sb2_PUT.Flags = binaryReader.ReadByte();
					sb2_PUT.Pease = binaryReader.ReadByte();
					sb2_PUT.Pad = binaryReader.ReadByte();
					sb2_PUT.Xpos = binaryReader.ReadInt16();
					sb2_PUT.Ypos = binaryReader.ReadInt16();
					sb2_PUT.Zpos = binaryReader.ReadInt16();
					sb2_PUT.Rot = binaryReader.ReadInt16();
				}
			}
			binaryReader.BaseStream.Seek((Int64)(8 + 56 * this.header.PatCount), SeekOrigin.Begin);
			for (Int32 k = 0; k < (Int32)this.header.TypCount; k++)
			{
				SB2_MON_PARM sb2_MON_PARM = this.MonAddr[k] = new SB2_MON_PARM();
				for (Int32 l = 0; l < 3; l++)
				{
					sb2_MON_PARM.Status[l] = (BattleStatus)binaryReader.ReadUInt32();
				}
				sb2_MON_PARM.MaxHP = binaryReader.ReadUInt16();
				sb2_MON_PARM.MaxMP = binaryReader.ReadUInt16();
				sb2_MON_PARM.WinGil = binaryReader.ReadUInt16();
				sb2_MON_PARM.WinExp = binaryReader.ReadUInt16();
				for (Int32 m = 0; m < 4; m++)
				{
					sb2_MON_PARM.WinItems[m] = binaryReader.ReadByte();
				}
				for (Int32 n = 0; n < 4; n++)
				{
					sb2_MON_PARM.StealItems[n] = binaryReader.ReadByte();
				}
				sb2_MON_PARM.Radius = binaryReader.ReadUInt16();
				sb2_MON_PARM.Geo = binaryReader.ReadUInt16();
				for (Int32 num = 0; num < 6; num++)
				{
					sb2_MON_PARM.Mot[num] = binaryReader.ReadUInt16();
				}
				for (Int32 num2 = 0; num2 < 2; num2++)
				{
					sb2_MON_PARM.Mesh[num2] = binaryReader.ReadUInt16();
				}
				sb2_MON_PARM.Flags = binaryReader.ReadUInt16();
				sb2_MON_PARM.AP = binaryReader.ReadUInt16();
				SB2_ELEMENT sb2_ELEMENT = sb2_MON_PARM.Element = new SB2_ELEMENT();
				sb2_ELEMENT.dex = binaryReader.ReadByte();
				sb2_ELEMENT.str = binaryReader.ReadByte();
				sb2_ELEMENT.mgc = binaryReader.ReadByte();
				sb2_ELEMENT.wpr = binaryReader.ReadByte();
				sb2_ELEMENT.pad = binaryReader.ReadByte();
				sb2_ELEMENT.trans = binaryReader.ReadByte();
				sb2_ELEMENT.cur_capa = binaryReader.ReadByte();
				sb2_ELEMENT.max_capa = binaryReader.ReadByte();
				for (Int32 num3 = 0; num3 < 4; num3++)
				{
					sb2_MON_PARM.Attr[num3] = binaryReader.ReadByte();
				}
				sb2_MON_PARM.Level = binaryReader.ReadByte();
				sb2_MON_PARM.Category = binaryReader.ReadByte();
				sb2_MON_PARM.HitRate = binaryReader.ReadByte();
				sb2_MON_PARM.P_DP = binaryReader.ReadByte();
				sb2_MON_PARM.P_AV = binaryReader.ReadByte();
				sb2_MON_PARM.M_DP = binaryReader.ReadByte();
				sb2_MON_PARM.M_AV = binaryReader.ReadByte();
				sb2_MON_PARM.Blue = binaryReader.ReadByte();
				for (Int32 num4 = 0; num4 < 4; num4++)
				{
					sb2_MON_PARM.Bone[num4] = binaryReader.ReadByte();
				}
				sb2_MON_PARM.DieSfx = binaryReader.ReadUInt16();
				sb2_MON_PARM.Konran = binaryReader.ReadByte();
				sb2_MON_PARM.MesCnt = binaryReader.ReadByte();
				for (Int32 num5 = 0; num5 < 6; num5++)
				{
					sb2_MON_PARM.IconBone[num5] = binaryReader.ReadByte();
				}
				for (Int32 num6 = 0; num6 < 6; num6++)
				{
					sb2_MON_PARM.IconY[num6] = binaryReader.ReadSByte();
				}
				for (Int32 num7 = 0; num7 < 6; num7++)
				{
					sb2_MON_PARM.IconZ[num7] = binaryReader.ReadSByte();
				}
				sb2_MON_PARM.StartSfx = binaryReader.ReadUInt16();
				sb2_MON_PARM.ShadowX = binaryReader.ReadUInt16();
				sb2_MON_PARM.ShadowZ = binaryReader.ReadUInt16();
				sb2_MON_PARM.ShadowBone = binaryReader.ReadByte();
				sb2_MON_PARM.Card = binaryReader.ReadByte();
				sb2_MON_PARM.ShadowOfsX = binaryReader.ReadInt16();
				sb2_MON_PARM.ShadowOfsZ = binaryReader.ReadInt16();
				sb2_MON_PARM.ShadowBone2 = binaryReader.ReadByte();
				sb2_MON_PARM.Pad0 = binaryReader.ReadByte();
				sb2_MON_PARM.Pad1 = binaryReader.ReadUInt16();
				sb2_MON_PARM.Pad2 = binaryReader.ReadUInt16();
			}
			binaryReader.BaseStream.Seek((Int64)(8 + 56 * this.header.PatCount + 116 * this.header.TypCount), SeekOrigin.Begin);
			for (Int32 num8 = 0; num8 < (Int32)this.header.AtkCount; num8++)
			{
				AA_DATA aa_DATA = this.atk[num8] = new AA_DATA();
				BattleCommandInfo cmd_INFO = aa_DATA.Info = new BattleCommandInfo();
				UInt32 input = binaryReader.ReadUInt32();
				Byte b = 0;
				cmd_INFO.Target = (TargetType)BitUtil.ReadBits(input, ref b, 4);
				cmd_INFO.DefaultAlly = BitUtil.ReadBits(input, ref b, 1) != 0;
				cmd_INFO.DisplayStats = (TargetDisplay)BitUtil.ReadBits(input, ref b, 3);
				cmd_INFO.VfxIndex = (Int16)BitUtil.ReadBits(input, ref b, 9);
				/*cmd_INFO.sfx_no = (Int16)*/ BitUtil.ReadBits(input, ref b, 12);
				cmd_INFO.ForDead = BitUtil.ReadBits(input, ref b, 1) != 0;
				cmd_INFO.DefaultCamera = BitUtil.ReadBits(input, ref b, 1) != 0;
				cmd_INFO.DefaultOnDead = BitUtil.ReadBits(input, ref b, 1) != 0;
				BTL_REF btl_REF = aa_DATA.Ref = new BTL_REF();
				btl_REF.ScriptId = binaryReader.ReadByte();
				btl_REF.Power = binaryReader.ReadByte();
				btl_REF.Elements = binaryReader.ReadByte();
				btl_REF.Rate = binaryReader.ReadByte();
				aa_DATA.Category = binaryReader.ReadByte();
				aa_DATA.AddNo = binaryReader.ReadByte();
				aa_DATA.MP = binaryReader.ReadByte();
				aa_DATA.Type = binaryReader.ReadByte();
				aa_DATA.Vfx2 = binaryReader.ReadUInt16();
				aa_DATA.Name = binaryReader.ReadUInt16().ToString();
			}
		}
		Int32 typIndex = -1;
		Int32 attIndex = -1;
		foreach (String s in bsceneInfo)
		{
			String[] bsceneCode = s.Split(' ');
			if (bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "Enemy") == 0)
				Int32.TryParse(bsceneCode[1], out typIndex);
			else if (bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "Attack") == 0)
				Int32.TryParse(bsceneCode[1], out attIndex);
			else if (typIndex >= 0 && typIndex < this.MonAddr.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "MaxHP") == 0)
				UInt32.TryParse(bsceneCode[1], out this.MonAddr[typIndex].MaxHP);
			else if (typIndex >= 0 && typIndex < this.MonAddr.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "MaxMP") == 0)
				UInt32.TryParse(bsceneCode[1], out this.MonAddr[typIndex].MaxMP);
			else if (typIndex >= 0 && typIndex < this.MonAddr.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "Gil") == 0)
				UInt32.TryParse(bsceneCode[1], out this.MonAddr[typIndex].WinGil);
			else if (typIndex >= 0 && typIndex < this.MonAddr.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "Exp") == 0)
				UInt32.TryParse(bsceneCode[1], out this.MonAddr[typIndex].WinExp);
			else if (typIndex >= 0 && typIndex < this.MonAddr.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "AttackPower") == 0)
				UInt32.TryParse(bsceneCode[1], out this.MonAddr[typIndex].AP); // AP is unused by default
			else if (typIndex >= 0 && typIndex < this.MonAddr.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "OutOfReach") == 0)
			{
				Boolean oor;
				if (Boolean.TryParse(bsceneCode[1], out oor))
					this.MonAddr[typIndex].OutOfReach = (SByte)(oor ? 1 : 0);
			}
			else if (attIndex >= 0 && attIndex < this.atk.Length && bsceneCode.Length >= 2 && String.Compare(bsceneCode[0], "Vfx") == 0)
			{
				if (Configuration.Battle.SFXRework)
				{
					String[] efInfo;
					String sequenceText = AssetManager.LoadString(bsceneCode[1], out efInfo);
					if (sequenceText != null)
						this.atk[attIndex].Info.VfxAction = new UnifiedBattleSequencer.BattleAction(sequenceText);
				}
			}
		}
	}

	public static Int16 GetMonGeoID(Int32 pNum)
	{
		SB2_PUT sb2_PUT = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].Put[pNum];
		Int16 result;
		if (pNum > 0 && (sb2_PUT.Flags & 2) != 0)
			result = -1;
		else
			result = (Int16)FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[(Int32)sb2_PUT.TypeNo].Geo;
		return result;
	}

	public static UInt16 BtlGetStartSFX()
	{
		Byte patNum = FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum;
		UInt16 typeNo = (UInt16)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)patNum].Put[0].TypeNo;
		return FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[(Int32)typeNo].StartSfx;
	}

	public static Int32 GetMonCount()
	{
		return (Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonCount;
	}

	public const Int32 FR_OFFSET = -400;

	public BTL_SCENE_INFO Info;

	public SB2_PATTERN[] PatAddr;

	public SB2_MON_PARM[] MonAddr;

	public Byte SeqAddr;

	public Byte CamAddr;

	public Byte PatNum;

	public Byte Pad0;

	public UInt16 StartSfx;

	public UInt32[] pad = new UInt32[4];

	public SB2_HEAD header;

	public AA_DATA[] atk;

	// Custom field
	public String nameIdentifier;
}
