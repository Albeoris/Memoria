using System;
using UnityEngine;
using Memoria.Data;

public class ENEMY
{
	public ENEMY()
	{
		this.et = new ENEMY_TYPE();
		this.info = new ENEMY.ENEMY_INFO();
		this.bonus_item = new RegularItem[4];
		this.bonus_item_rate = new UInt16[4];
		this.steal_item = new RegularItem[4];
		this.steal_item_rate = new UInt16[4];
		this.trance_glowing_color = new Byte[3];
	}

	public ENEMY_TYPE et;
	public ENEMY.ENEMY_INFO info;

	public UInt32 bonus_gil;
	public UInt32 bonus_exp;
	public RegularItem[] bonus_item;
	public UInt16[] bonus_item_rate;
	public TetraMasterCardId bonus_card;
	public UInt16 bonus_card_rate;
	public RegularItem[] steal_item;
	public UInt16[] steal_item_rate;

	public Vector3 base_pos;
	public Byte[] trance_glowing_color;

	public Byte steal_unsuccessful_counter;

	public class ENEMY_INFO
	{
		public Byte die_fade_rate;
		public Byte die_atk;
		public Byte die_dmg;
		public Byte die_vulnerable;
        public Byte die_unused4;
        public Byte die_unused5;
        public Byte die_unused6;
        public Byte die_unused7;
        public Byte die_unused8;
        public Byte multiple;
		public Byte slave;
		public Int32 reserve;
		public UInt16 flags;
	}
}
