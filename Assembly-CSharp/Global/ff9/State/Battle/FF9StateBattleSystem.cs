using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime.Collections;
using UnityEngine;

public class FF9StateBattleSystem
{
	public FF9StateBattleSystem()
	{
		this.p_mot = new String[][]
		{
			new String[34],
			new String[34],
			new String[34],
			new String[34]
		};
		this.enemy = new ENEMY[4];
		for (Int32 i = 0; i < (Int32)this.enemy.Length; i++)
		{
			this.enemy[i] = new ENEMY();
		}
		this.enemy_type = new ENEMY_TYPE[4];
		for (Int32 j = 0; j < (Int32)this.enemy_type.Length; j++)
		{
			this.enemy_type[j] = new ENEMY_TYPE();
		}
		this.btl_list = new BTL_DATA();
		this.btl_data = new BTL_DATA[8];
		for (Int32 k = 0; k < (Int32)this.btl_data.Length; k++)
		{
			this.btl_data[k] = new BTL_DATA();
		}
		this.cmd_buffer = new CMD_DATA[48]; // enemy_type.Length * 12
		for (Int32 l = 0; l < (Int32)this.cmd_buffer.Length; l++)
		{
			this.cmd_buffer[l] = new CMD_DATA();
		}
		this.cmd_escape = new CMD_DATA();
		this.cmd_queue = new CMD_DATA();
		this.cur_cmd = new CMD_DATA();
		this.fx_req = new BTL_VFX_REQ();
		this.seq_work_set = new SEQ_WORK_SET();
		this.btl_scene = new BTL_SCENE();
		this.btl2d_work_set = new BTL2D_WORK();
		this.status_data = EntryCollection.CreateWithDefaultElement<STAT_DATA>(32);
	    this.aa_data = EntryCollection.CreateWithDefaultElement<AA_DATA>(192);
	    this.add_status = EntryCollection.CreateWithDefaultElement<BattleStatusEntry>(64);
		this.map = new FF9StateBattleMap();
	}

	public UInt16 attr;

	public Char usage;

	public String[][] p_mot;

	public ENEMY[] enemy;

	public ENEMY_TYPE[] enemy_type;

	public AA_DATA[] enemy_attack;

	public BTL_DATA btl_list;

	public BTL_DATA[] btl_data;

	public Int32 btl_cnt;

	public Byte btl_phase;

	public Byte btl_seq;

	public Byte btl_fade_time;

	public Byte btl_escape_key;

	public SByte btl_escape_fade;

	public Byte btl_load_status;

	public SByte player_load_fade;

	public SByte enemy_load_fade;

	public CMD_DATA[] cmd_buffer;

	public CMD_DATA cmd_escape;

	public CMD_DATA cmd_queue;

	public CMD_DATA cur_cmd;

	public GameObject s_cur;

	public BTL_VFX_REQ fx_req;

	public UInt16 cmd_status;

	public Byte cmd_mode;

	public Byte phantom_no;

	public Int16 phantom_cnt;

	public UInt16 enemy_die;

	public FF9StateBattleMap map;

	public SEQ_WORK_SET seq_work_set;

	public BTL_SCENE btl_scene;

	public BTL2D_WORK btl2d_work_set;

	public EntryCollection<STAT_DATA> status_data;

	public EntryCollection<AA_DATA> aa_data;

	public EntryCollection<BattleStatusEntry> add_status;

    public IEnumerable<BattleUnit> EnumerateBattleUnits()
    {
        for (BTL_DATA data = FF9StateSystem.Battle.FF9Battle.btl_list.next; data != null; data = data.next)
            yield return new BattleUnit(data);
    }

    public BattleUnit GetUnit(Int32 index)
    {
        BTL_DATA data = btl_data[index];
        return new BattleUnit(data);
    }
}
