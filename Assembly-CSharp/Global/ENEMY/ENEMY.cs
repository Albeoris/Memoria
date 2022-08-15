using System;
using UnityEngine;

public class ENEMY
{
	public ENEMY()
	{
		this.et = new ENEMY_TYPE();
		this.info = new ENEMY.ENEMY_INFO();
		this.steal_item = new Byte[4];
		this.steal_item_rate = new UInt16[4];
	}

	public const Byte ENEMY_CATEGORY_OTHER = 0;

	public const Byte ENEMY_CATEGORY_HUMANOID = 1;

	public const Byte ENEMY_CATEGORY_BEAST = 2;

	public const Byte ENEMY_CATEGORY_DEVIL = 4;

	public const Byte ENEMY_CATEGORY_DRAGON = 8;

	public const Byte ENEMY_CATEGORY_UNDEAD = 16;

	public const Byte ENEMY_CATEGORY_STONE = 32;

	public const Byte ENEMY_CATEGORY_SOUL = 64;

	public const Byte ENEMY_CATEGORY_FLIGHT = 128;

	public ENEMY_TYPE et;

	public ENEMY.ENEMY_INFO info;

	public Byte[] steal_item;
	public UInt16[] steal_item_rate;

	public Vector3 base_pos;

	public Byte steal_unsuccessful_counter;

	public class ENEMY_INFO
	{
		public Byte die_fade_rate;

		public Byte die_atk;

		public Byte die_dmg;

		public Byte multiple;

		public Byte slave;

		public Int32 reserve;

		public UInt16 flags;
	}
}
