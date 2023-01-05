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
		Byte[] bytes = AssetManager.LoadBytes("BattleMap/BattleScene/" + name + "/dbfile0000.raw16");
		if (bytes == null)
			return;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes)))
		{
			this.header.Ver = binaryReader.ReadByte();
			this.header.PatCount = binaryReader.ReadByte();
			this.header.TypCount = binaryReader.ReadByte();
			this.header.AtkCount = binaryReader.ReadByte();
			this.header.Flags = binaryReader.ReadUInt16();
			this.PatAddr = new SB2_PATTERN[this.header.PatCount];
			this.MonAddr = new SB2_MON_PARM[this.header.TypCount];
			this.atk = new AA_DATA[this.header.AtkCount];
			binaryReader.BaseStream.Seek(8L, SeekOrigin.Begin);
			for (Int32 i = 0; i < this.header.PatCount; i++)
			{
				SB2_PATTERN sb2_PATTERN = this.PatAddr[i] = new SB2_PATTERN();
				sb2_PATTERN.Rate = binaryReader.ReadByte();
				sb2_PATTERN.MonsterCount = binaryReader.ReadByte();
				sb2_PATTERN.Camera = binaryReader.ReadByte();
				sb2_PATTERN.Pad0 = binaryReader.ReadByte();
				sb2_PATTERN.AP = binaryReader.ReadUInt32();
				for (Int32 j = 0; j < 4; j++)
				{
					SB2_PUT sb2_PUT = sb2_PATTERN.Monster[j] = new SB2_PUT();
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
			binaryReader.BaseStream.Seek(8 + 56 * this.header.PatCount, SeekOrigin.Begin);
			for (Int32 i = 0; i < this.header.TypCount; i++)
			{
				SB2_MON_PARM sb2_MON_PARM = this.MonAddr[i] = new SB2_MON_PARM();
				sb2_MON_PARM.ResistStatus = (BattleStatus)binaryReader.ReadUInt32();
				sb2_MON_PARM.AutoStatus = (BattleStatus)binaryReader.ReadUInt32();
				sb2_MON_PARM.InitialStatus = (BattleStatus)binaryReader.ReadUInt32();
				sb2_MON_PARM.MaxHP = binaryReader.ReadUInt16();
				sb2_MON_PARM.MaxMP = binaryReader.ReadUInt16();
				sb2_MON_PARM.WinGil = binaryReader.ReadUInt16();
				sb2_MON_PARM.WinExp = binaryReader.ReadUInt16();
				for (Int32 j = 0; j < 4; j++)
				{
					sb2_MON_PARM.WinItems[j] = (RegularItem)binaryReader.ReadByte();
					sb2_MON_PARM.WinItemRates[j] = SB2_MON_PARM.DefaultWinItemRates[j];
				}
				for (Int32 j = 0; j < 4; j++)
				{
					sb2_MON_PARM.StealItems[j] = (RegularItem)binaryReader.ReadByte();
					sb2_MON_PARM.StealItemRates[j] = SB2_MON_PARM.DefaultStealItemRates[j];
				}
				sb2_MON_PARM.Radius = binaryReader.ReadUInt16();
				sb2_MON_PARM.Geo = binaryReader.ReadUInt16();
				for (Int32 j = 0; j < 6; j++)
					sb2_MON_PARM.Mot[j] = binaryReader.ReadUInt16();
				for (Int32 j = 0; j < 2; j++)
					sb2_MON_PARM.Mesh[j] = binaryReader.ReadUInt16();
				sb2_MON_PARM.Flags = binaryReader.ReadUInt16();
				sb2_MON_PARM.AP = binaryReader.ReadUInt16();
				SB2_ELEMENT sb2_ELEMENT = sb2_MON_PARM.Element = new SB2_ELEMENT();
				sb2_ELEMENT.Speed = binaryReader.ReadByte();
				sb2_ELEMENT.Strength = binaryReader.ReadByte();
				sb2_ELEMENT.Magic = binaryReader.ReadByte();
				sb2_ELEMENT.Spirit = binaryReader.ReadByte();
				sb2_ELEMENT.pad = binaryReader.ReadByte();
				sb2_ELEMENT.trans = binaryReader.ReadByte();
				sb2_ELEMENT.cur_capa = binaryReader.ReadByte();
				sb2_ELEMENT.max_capa = binaryReader.ReadByte();
				sb2_MON_PARM.GuardElement = binaryReader.ReadByte();
				sb2_MON_PARM.AbsorbElement = binaryReader.ReadByte();
				sb2_MON_PARM.HalfElement = binaryReader.ReadByte();
				sb2_MON_PARM.WeakElement = binaryReader.ReadByte();
				sb2_MON_PARM.Level = binaryReader.ReadByte();
				sb2_MON_PARM.Category = binaryReader.ReadByte();
				sb2_MON_PARM.HitRate = binaryReader.ReadByte();
				sb2_MON_PARM.PhysicalDefence = binaryReader.ReadByte();
				sb2_MON_PARM.PhysicalEvade = binaryReader.ReadByte();
				sb2_MON_PARM.MagicalDefence = binaryReader.ReadByte();
				sb2_MON_PARM.MagicalEvade = binaryReader.ReadByte();
				sb2_MON_PARM.BlueMagic = binaryReader.ReadByte();
				for (Int32 j = 0; j < 4; j++)
					sb2_MON_PARM.Bone[j] = binaryReader.ReadByte();
				sb2_MON_PARM.DieSfx = binaryReader.ReadUInt16();
				sb2_MON_PARM.Konran = binaryReader.ReadByte();
				sb2_MON_PARM.MesCnt = binaryReader.ReadByte();
				for (Int32 j = 0; j < 6; j++)
					sb2_MON_PARM.IconBone[j] = binaryReader.ReadByte();
				for (Int32 j = 0; j < 6; j++)
					sb2_MON_PARM.IconY[j] = binaryReader.ReadSByte();
				for (Int32 j = 0; j < 6; j++)
					sb2_MON_PARM.IconZ[j] = binaryReader.ReadSByte();
				sb2_MON_PARM.StartSfx = binaryReader.ReadUInt16();
				sb2_MON_PARM.ShadowX = binaryReader.ReadUInt16();
				sb2_MON_PARM.ShadowZ = binaryReader.ReadUInt16();
				sb2_MON_PARM.ShadowBone = binaryReader.ReadByte();
				sb2_MON_PARM.WinCard = binaryReader.ReadByte();
				sb2_MON_PARM.WinCardRate = SB2_MON_PARM.DefaultWinCardRate;
				sb2_MON_PARM.ShadowOfsX = binaryReader.ReadInt16();
				sb2_MON_PARM.ShadowOfsZ = binaryReader.ReadInt16();
				sb2_MON_PARM.ShadowBone2 = binaryReader.ReadByte();
				sb2_MON_PARM.Pad0 = binaryReader.ReadByte();
				sb2_MON_PARM.Pad1 = binaryReader.ReadUInt16();
				sb2_MON_PARM.Pad2 = binaryReader.ReadUInt16();
			}
			binaryReader.BaseStream.Seek(8 + 56 * this.header.PatCount + 116 * this.header.TypCount, SeekOrigin.Begin);
			for (Int32 i = 0; i < this.header.AtkCount; i++)
			{
				AA_DATA aa_DATA = this.atk[i] = new AA_DATA();
				BattleCommandInfo cmd_INFO = aa_DATA.Info = new BattleCommandInfo();
				BTL_REF btl_REF = aa_DATA.Ref = new BTL_REF();
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
				btl_REF.ScriptId = binaryReader.ReadByte();
				btl_REF.Power = binaryReader.ReadByte();
				btl_REF.Elements = binaryReader.ReadByte();
				btl_REF.Rate = binaryReader.ReadByte();
				aa_DATA.Category = binaryReader.ReadByte();
				aa_DATA.AddStatusNo = (BattleStatusIndex)binaryReader.ReadByte();
				aa_DATA.MP = binaryReader.ReadByte();
				aa_DATA.Type = binaryReader.ReadByte();
				aa_DATA.Vfx2 = binaryReader.ReadUInt16();
				aa_DATA.Name = binaryReader.ReadUInt16().ToString();
			}
		}
		SetupSceneInfo();
		DataPatchers.ApplyBattlePatch(this);
		if (Configuration.Battle.SFXRework)
		{
			foreach (AA_DATA aa in this.atk)
			{
				if (!String.IsNullOrEmpty(aa.Info.SequenceFile))
				{
					String sequenceText = AssetManager.LoadString(aa.Info.SequenceFile);
					if (sequenceText != null)
						aa.Info.VfxAction = new UnifiedBattleSequencer.BattleAction(sequenceText);
				}
			}
		}
	}

	public static Int16 GetMonGeoID(Int32 pNum)
	{
		SB2_PUT sb2_PUT = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].Monster[pNum];
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
		UInt16 typeNo = (UInt16)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)patNum].Monster[0].TypeNo;
		return FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[(Int32)typeNo].StartSfx;
	}

	public static Int32 GetMonCount()
	{
		return (Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[(Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonsterCount;
	}

	private void SetupSceneInfo()
	{
		UInt16 flags = header.Flags;
		Info = new BTL_SCENE_INFO();
		Info.SpecialStart = (flags & 1) != 0;
		Info.BackAttack = (flags & 2) != 0;
		Info.NoGameOver = (flags & 4) != 0;
		Info.NoExp = (flags & 8) != 0;
		Info.WinPose = (flags & 16) == 0;
		Info.Runaway = (flags & 32) == 0;
		Info.NoNeighboring = (flags & 64) != 0;
		Info.NoMagical = (flags & 128) != 0;
		Info.ReverseAttack = (flags & 256) != 0;
		Info.FixedCamera1 = (flags & 512) != 0;
		Info.FixedCamera2 = (flags & 1024) != 0;
		Info.AfterEvent = (flags & 2048) != 0;
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
