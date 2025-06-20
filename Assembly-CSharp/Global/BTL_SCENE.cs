using FF9;
using Memoria;
using Memoria.Data;
using System;
using System.Collections.Generic;
using System.IO;

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
                SB2_PATTERN pattern = this.PatAddr[i] = new SB2_PATTERN();
                pattern.Rate = binaryReader.ReadByte();
                pattern.MonsterCount = binaryReader.ReadByte();
                pattern.Camera = binaryReader.ReadByte();
                pattern.Pad0 = binaryReader.ReadByte();
                pattern.AP = binaryReader.ReadUInt32();
                for (Int32 j = 0; j < 4; j++)
                {
                    SB2_PUT placement = pattern.Monster[j] = new SB2_PUT();
                    placement.TypeNo = binaryReader.ReadByte();
                    placement.Flags = binaryReader.ReadByte();
                    placement.Pease = binaryReader.ReadByte();
                    placement.Pad = binaryReader.ReadByte();
                    placement.Xpos = binaryReader.ReadInt16();
                    placement.Ypos = binaryReader.ReadInt16();
                    placement.Zpos = binaryReader.ReadInt16();
                    placement.Rot = binaryReader.ReadInt16();
                }
            }
            binaryReader.BaseStream.Seek(8 + 56 * this.header.PatCount, SeekOrigin.Begin);
            for (Int32 i = 0; i < this.header.TypCount; i++)
            {
                SB2_MON_PARM monParam = this.MonAddr[i] = new SB2_MON_PARM();
                monParam.ResistStatus = (BattleStatus)binaryReader.ReadUInt32();
                monParam.AutoStatus = (BattleStatus)binaryReader.ReadUInt32();
                monParam.InitialStatus = (BattleStatus)binaryReader.ReadUInt32();
                monParam.MaxHP = binaryReader.ReadUInt16();
                monParam.MaxMP = binaryReader.ReadUInt16();
                monParam.WinGil = binaryReader.ReadUInt16();
                monParam.WinExp = binaryReader.ReadUInt16();
                for (Int32 j = 0; j < 4; j++)
                {
                    monParam.WinItems[j] = (RegularItem)binaryReader.ReadByte();
                    monParam.WinItemRates[j] = SB2_MON_PARM.DefaultWinItemRates[j];
                }
                for (Int32 j = 0; j < 4; j++)
                {
                    monParam.StealItems[j] = (RegularItem)binaryReader.ReadByte();
                    monParam.StealItemRates[j] = SB2_MON_PARM.DefaultStealItemRates[j];
                }
                monParam.Radius = binaryReader.ReadUInt16();
                monParam.Geo = binaryReader.ReadInt16();
                for (Int32 j = 0; j < 6; j++)
                    monParam.Mot[j] = binaryReader.ReadUInt16();
                for (Int32 j = 0; j < 2; j++)
                    monParam.Mesh[j] = binaryReader.ReadUInt16();
                monParam.Flags = binaryReader.ReadUInt16();
                monParam.AP = binaryReader.ReadUInt16();
                SB2_ELEMENT monStats = monParam.Element = new SB2_ELEMENT();
                monStats.Speed = binaryReader.ReadByte();
                monStats.Strength = binaryReader.ReadByte();
                monStats.Magic = binaryReader.ReadByte();
                monStats.Spirit = binaryReader.ReadByte();
                monStats.pad = binaryReader.ReadByte();
                monStats.trans = binaryReader.ReadByte();
                monStats.cur_capa = binaryReader.ReadByte();
                monStats.max_capa = binaryReader.ReadByte();
                monParam.GuardElement = binaryReader.ReadByte();
                monParam.AbsorbElement = binaryReader.ReadByte();
                monParam.HalfElement = binaryReader.ReadByte();
                monParam.WeakElement = binaryReader.ReadByte();
                monParam.Level = binaryReader.ReadByte();
                monParam.Category = binaryReader.ReadByte();
                monParam.HitRate = binaryReader.ReadByte();
                monParam.PhysicalDefence = binaryReader.ReadByte();
                monParam.PhysicalEvade = binaryReader.ReadByte();
                monParam.MagicalDefence = binaryReader.ReadByte();
                monParam.MagicalEvade = binaryReader.ReadByte();
                monParam.BlueMagic = binaryReader.ReadByte();
                for (Int32 j = 0; j < 4; j++)
                    monParam.Bone[j] = binaryReader.ReadByte();
                monParam.DieSfx = binaryReader.ReadUInt16();
                monParam.Konran = binaryReader.ReadByte();
                monParam.MesCnt = binaryReader.ReadByte();
                for (Int32 j = 0; j < 6; j++)
                    monParam.IconBone[j] = binaryReader.ReadByte();
                for (Int32 j = 0; j < 6; j++)
                    monParam.IconY[j] = binaryReader.ReadSByte();
                for (Int32 j = 0; j < 6; j++)
                    monParam.IconZ[j] = binaryReader.ReadSByte();
                monParam.StartSfx = binaryReader.ReadUInt16();
                monParam.ShadowX = binaryReader.ReadUInt16();
                monParam.ShadowZ = binaryReader.ReadUInt16();
                monParam.ShadowBone = binaryReader.ReadByte();
                monParam.WinCard = (TetraMasterCardId)binaryReader.ReadByte();
                monParam.WinCardRate = SB2_MON_PARM.DefaultWinCardRate;
                monParam.ShadowOfsX = binaryReader.ReadInt16();
                monParam.ShadowOfsZ = binaryReader.ReadInt16();
                monParam.ShadowBone2 = binaryReader.ReadByte();
                monParam.Pad0 = binaryReader.ReadByte();
                monParam.Pad1 = binaryReader.ReadUInt16();
                monParam.Pad2 = binaryReader.ReadUInt16();
                if (btl_eqp.EnemyBuiltInWeaponTable.TryGetValue(monParam.Geo, out Int32[] weaponBones))
                    monParam.WeaponAttachment = weaponBones;
            }
            binaryReader.BaseStream.Seek(8 + 56 * this.header.PatCount + 116 * this.header.TypCount, SeekOrigin.Begin);
            for (Int32 i = 0; i < this.header.AtkCount; i++)
            {
                AA_DATA monAbility = this.atk[i] = new AA_DATA();
                BattleCommandInfo abilInfo = monAbility.Info = new BattleCommandInfo();
                BTL_REF abilRef = monAbility.Ref = new BTL_REF();
                UInt32 input = binaryReader.ReadUInt32();
                Byte bitPos = 0;
                abilInfo.Target = (TargetType)BitUtil.ReadBits(input, ref bitPos, 4);
                abilInfo.DefaultAlly = BitUtil.ReadBits(input, ref bitPos, 1) != 0;
                abilInfo.DisplayStats = (TargetDisplay)BitUtil.ReadBits(input, ref bitPos, 3);
                abilInfo.VfxIndex = (Int16)BitUtil.ReadBits(input, ref bitPos, 9);
                /*cmd_INFO.sfx_no = (Int16)*/
                BitUtil.ReadBits(input, ref bitPos, 12);
                abilInfo.ForDead = BitUtil.ReadBits(input, ref bitPos, 1) != 0;
                abilInfo.DefaultCamera = BitUtil.ReadBits(input, ref bitPos, 1) != 0;
                abilInfo.DefaultOnDead = BitUtil.ReadBits(input, ref bitPos, 1) != 0;
                abilRef.ScriptId = binaryReader.ReadByte();
                abilRef.Power = binaryReader.ReadByte();
                abilRef.Elements = binaryReader.ReadByte();
                abilRef.Rate = binaryReader.ReadByte();
                monAbility.Category = binaryReader.ReadByte();
                monAbility.AddStatusNo = (StatusSetId)binaryReader.ReadByte();
                monAbility.MP = binaryReader.ReadByte();
                monAbility.Type = binaryReader.ReadByte();
                monAbility.Vfx2 = binaryReader.ReadUInt16();
                monAbility.Name = binaryReader.ReadUInt16().ToString();
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
        foreach (SB2_MON_PARM monParam in this.MonAddr)
        {
            if (!String.IsNullOrEmpty(monParam.SupportingAbilityFile))
            {
                String abilText = AssetManager.LoadString(monParam.SupportingAbilityFile);
                if (abilText != null)
                {
                    Dictionary<SupportAbility, SupportingAbilityFeature> saAsDictionary = new Dictionary<SupportAbility, SupportingAbilityFeature>();
                    ff9abil.LoadAbilityFeatureFile(ref saAsDictionary, abilText, monParam.SupportingAbilityFile);
                    monParam.SupportingAbilityFeatures = new List<SupportingAbilityFeature>(saAsDictionary.Values);
                }
            }
        }
    }

    public static Int16 GetMonGeoID(SB2_PUT placement)
    {
        if (placement.TypeNo > 0 && (placement.Flags & SB2_PUT.FLG_MULTIPART) != 0)
            return -1;
        return FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[placement.TypeNo].Geo;
    }

    public static List<Int32> EnemyGetAttackList(Int32 slotNo)
    {
        List<Int32> atkList = new List<Int32>();
        BTL_SCENE scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
        Int32 monsterIndex = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].Monster[slotNo].TypeNo;
        for (Int32 i = 0; i < scene.header.AtkCount; i++)
            if (btlseq.instance.GetEnemyIndexOfSequence(i) == monsterIndex)
                atkList.Add(i);
        return atkList;
    }

    public static UInt16 BtlGetStartSFX()
    {
        Byte patNum = FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum;
        UInt16 typeNo = (UInt16)FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[patNum].Monster[0].TypeNo;
        return FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[typeNo].StartSfx;
    }

    public static Int32 GetMonCount()
    {
        return FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum].MonsterCount;
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
