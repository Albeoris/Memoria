using System;
using Memoria.Data;

public class SB2_MON_PARM
{
	public SB2_MON_PARM()
	{
		this.WinItems = new RegularItem[4];
		this.WinItemRates = new UInt16[4];
		this.StealItems = new RegularItem[4];
		this.StealItemRates = new UInt16[4];
		this.Mot = new UInt16[6];
		this.Mesh = new UInt16[2];
		this.Bone = new Byte[4];
		this.IconBone = new Byte[6];
		this.IconY = new SByte[6];
		this.IconZ = new SByte[6];
		this.MaxDamageLimit = ff9play.FF9PLAY_DAMAGE_MAX;
		this.MaxMpDamageLimit = ff9play.FF9PLAY_MPDAMAGE_MAX;
	}

	public const Byte MON_PRM_FLG_DEADATK = 1;
	public const Byte MON_PRM_FLG_DEADDMG = 2;

	public const Int32 Size = 116;

	public static readonly UInt16[] DefaultWinItemRates = { 256, 96, 32, 1 };
	public static readonly UInt16[] DefaultStealItemRates = { 256, 64, 16, 1 };
	public static readonly UInt16 DefaultWinCardRate = 32;

	[Memoria.PatchableFieldAttribute]
	public BattleStatus ResistStatus;
	[Memoria.PatchableFieldAttribute]
	public BattleStatus AutoStatus;
	[Memoria.PatchableFieldAttribute]
	public BattleStatus InitialStatus;

	[Memoria.PatchableFieldAttribute]
	public UInt32 MaxHP;

	[Memoria.PatchableFieldAttribute]
	public UInt32 MaxMP;

	[Memoria.PatchableFieldAttribute]
	public UInt32 WinGil;

	[Memoria.PatchableFieldAttribute]
	public UInt32 WinExp;

	[Memoria.PatchableFieldAttribute]
	public RegularItem[] WinItems;
	[Memoria.PatchableFieldAttribute]
	public UInt16[] WinItemRates;

	[Memoria.PatchableFieldAttribute]
	public RegularItem[] StealItems;
	[Memoria.PatchableFieldAttribute]
	public UInt16[] StealItemRates;

	public UInt16 Radius;

	public UInt16 Geo;

	public UInt16[] Mot;

	public UInt16[] Mesh;

	public UInt16 Flags;

	public UInt32 AP;

	public SB2_ELEMENT Element;

	[Memoria.PatchableFieldAttribute]
	public Byte GuardElement;
	[Memoria.PatchableFieldAttribute]
	public Byte AbsorbElement;
	[Memoria.PatchableFieldAttribute]
	public Byte HalfElement;
	[Memoria.PatchableFieldAttribute]
	public Byte WeakElement;
	[Memoria.PatchableFieldAttribute]
	public Byte BonusElement;

	[Memoria.PatchableFieldAttribute]
	public Byte Level;

	[Memoria.PatchableFieldAttribute]
	public Byte Category;

	[Memoria.PatchableFieldAttribute]
	public Byte HitRate;

	[Memoria.PatchableFieldAttribute]
	public Int32 PhysicalDefence;

	[Memoria.PatchableFieldAttribute]
	public Int32 PhysicalEvade;

	[Memoria.PatchableFieldAttribute]
	public Int32 MagicalDefence;

	[Memoria.PatchableFieldAttribute]
	public Int32 MagicalEvade;

	[Memoria.PatchableFieldAttribute]
	public Int32 BlueMagic;

	public Byte[] Bone;

	public UInt16 DieSfx;

	public Byte Konran;

	public Byte MesCnt;

	public Byte[] IconBone;

	public SByte[] IconY;

	public SByte[] IconZ;

	public UInt16 StartSfx;

	public UInt16 ShadowX;

	public UInt16 ShadowZ;

	public Byte ShadowBone;

	[Memoria.PatchableFieldAttribute]
	public TetraMasterCardId WinCard;
	[Memoria.PatchableFieldAttribute]
	public UInt16 WinCardRate;

	public Int16 ShadowOfsX;

	public Int16 ShadowOfsZ;

	public Byte ShadowBone2;

	public Byte Pad0;

	public UInt16 Pad1;

	public UInt16 Pad2;
    [Memoria.PatchableFieldAttribute]
    public Byte[] TranceGlowColor;

    [Memoria.PatchableFieldAttribute]
	public Boolean OutOfReach;

	[Memoria.PatchableFieldAttribute]
	public String[] TextureFiles;

	[Memoria.PatchableFieldAttribute]
	public String WeaponModel;
	[Memoria.PatchableFieldAttribute]
	public Int32 WeaponAttachment;

	[Memoria.PatchableFieldAttribute]
	public UInt32 MaxDamageLimit;
	[Memoria.PatchableFieldAttribute]
	public UInt32 MaxMpDamageLimit;
}
