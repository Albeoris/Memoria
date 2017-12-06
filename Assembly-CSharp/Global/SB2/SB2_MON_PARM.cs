using System;
using Memoria.Data;

public class SB2_MON_PARM
{
	public SB2_MON_PARM()
	{
		this.Status = new BattleStatus[3];
		this.WinItems = new Byte[4];
		this.StealItems = new Byte[4];
		this.Mot = new UInt16[6];
		this.Mesh = new UInt16[2];
		this.Attr = new Byte[4];
		this.Bone = new Byte[4];
		this.IconBone = new Byte[6];
		this.IconY = new SByte[6];
		this.IconZ = new SByte[6];
	}

	public const Byte MON_PRM_FLG_DEADATK = 1;

	public const Byte MON_PRM_FLG_DEADDMG = 2;

	public const Int32 Size = 116;

	public BattleStatus[] Status;

	public UInt16 MaxHP;

	public UInt16 MaxMP;

	public UInt16 WinGil;

	public UInt16 WinExp;

	public Byte[] WinItems;

	public Byte[] StealItems;

	public UInt16 Radius;

	public UInt16 Geo;

	public UInt16[] Mot;

	public UInt16[] Mesh;

	public UInt16 Flags;

	public UInt16 AP;

	public SB2_ELEMENT Element;

	public Byte[] Attr;

	public Byte Level;

	public Byte Category;

	public Byte HitRate;

	public Byte P_DP;

	public Byte P_AV;

	public Byte M_DP;

	public Byte M_AV;

	public Byte Blue;

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

	public Byte Card;

	public Int16 ShadowOfsX;

	public Int16 ShadowOfsZ;

	public Byte ShadowBone2;

	public Byte Pad0;

	public UInt16 Pad1;

	public UInt16 Pad2;
}
