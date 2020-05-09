﻿using System;
using Memoria.Data;

public class CMD_DATA
{
	public CMD_DATA()
	{
		this.regist = new BTL_DATA();
		this.aa = new AA_DATA();
		this.info = new CMD_DATA.SELECT_INFO();
	}

	public CMD_DATA next;
	public BTL_DATA regist;
	public AA_DATA aa;
	public UInt16 tar_id;
	public BattleCommandId cmd_no;
	public Byte sub_no;
	public CMD_DATA.SELECT_INFO info;

	public class SELECT_INFO
	{
		public Byte cursor;
		public Byte stat;
		public Byte priority;
		public Byte cover;
		public Byte dodge;
		public Byte reflec;
		public Byte meteor_miss;
		public Byte short_summon;
		public Byte mon_reflec;

		public Boolean IsZeroMP { get; set; }

		public void Reset()
		{
			cursor = 0;
			stat = 0;
			priority = 0;
			cover = 0;
			dodge = 0;
			reflec = 0;
			meteor_miss = 0;
			short_summon = 0;
			mon_reflec = 0;
			IsZeroMP = false;
		}
	}
}
