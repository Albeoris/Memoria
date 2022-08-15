using System;

public class ENEMY_TYPE
{
	public ENEMY_TYPE()
	{
		this.max = new POINTS();
		this.mot = new String[6];
		this.cam_bone = new Byte[3];
		this.icon_bone = new Byte[6];
		this.icon_y = new SByte[6];
		this.icon_z = new SByte[6];
		this.bonus = new ENEMY_TYPE.ENEMY_BONUSES();
	}

	public String name;

	public Byte category;

	public Byte level;

	public POINTS max;

	public String[] mot;

	public UInt16 radius;

	public Byte p_atk_no;

	public Byte blue_magic_no;

	public Byte mes;

	public Byte[] cam_bone;

	public UInt32 cam_addr;

	public UInt16 die_snd_no;

	public Byte[] icon_bone;

	public SByte[] icon_y;

	public SByte[] icon_z;

	public ENEMY_TYPE.ENEMY_BONUSES bonus;

	public class ENEMY_BONUSES
	{
		public ENEMY_BONUSES()
		{
			this.item = new Byte[4];
			this.item_rate = new UInt16[4];
		}

		public UInt32 gil;

		public UInt32 exp;

		public Byte[] item;
		public UInt16[] item_rate;

		public UInt32 card;
		public UInt16 card_rate;
	}
}
