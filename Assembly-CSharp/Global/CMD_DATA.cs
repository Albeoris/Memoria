﻿using System;
using Memoria.Data;

public class CMD_DATA
{
	public CMD_DATA()
	{
		this.regist = new BTL_DATA();
		this.info = new CMD_DATA.SELECT_INFO();
		this.aa = new AA_DATA();
	}

	public CMD_DATA next;
	public BTL_DATA regist;
	public AA_DATA aa;
	public UInt16 tar_id;
	public BattleCommandId cmd_no;
	public Byte sub_no;
	public CMD_DATA.SELECT_INFO info;

	// Having duplicates allow to modify these fields (eg. SA features) without modifying the base AA's fields
	public Boolean IsShortRange;
	public Byte HitRate;
	public Byte Power;
	public Byte ScriptId;
	public EffectElement Element;
	public EffectElement ElementForBonus;
	public BattleStatus AbilityStatus;
	public Byte AbilityCategory;
	public Byte AbilityType;

	public void SetAAData(AA_DATA value)
	{
		aa = value;
		if (aa != null)
		{
			HitRate = aa.Ref.Rate;
			Power = aa.Ref.Power;
			ScriptId = aa.Ref.ScriptId;
			Element = (EffectElement)aa.Ref.Elements;
			ElementForBonus = Element;
			AbilityStatus = FF9StateSystem.Battle.FF9Battle.add_status[aa.AddNo].Value;
			AbilityCategory = aa.Category;
			AbilityType = aa.Type;
			IsShortRange = false;
		}
	}

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

		// Custom fields
		public Boolean IsZeroMP { get; set; }
		public Int32 CustomMPCost { get; set; }
		public Boolean ReflectNull { get; set; }

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
			CustomMPCost = -1;
			ReflectNull = false;
		}
	}
}
