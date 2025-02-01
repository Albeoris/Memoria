using FF9;
using Memoria;
using Memoria.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FF9StateBattleSystem
{
    public const Byte PHASE_INIT_SYSTEM = 0;
    public const Byte PHASE_EVENT = 1;
    public const Byte PHASE_ENTER = 2;
    public const Byte PHASE_MENU_ON = 3;
    public const Byte PHASE_NORMAL = 4;
    public const Byte PHASE_MENU_OFF = 5;
    public const Byte PHASE_VICTORY = 6;
    public const Byte PHASE_DEFEAT = 7;
    public const Byte PHASE_CLOSE = 8;

    public const Byte SEQ_DEFEATCLOSE_FADEOUT = 0;
    public const Byte SEQ_DEFEATCLOSE_WAIT = 1;
    public const Byte SEQ_MENU_OFF_VICTORY = 0;
    public const Byte SEQ_MENU_OFF_DEFEAT = 1;
    public const Byte SEQ_MENU_OFF_EVENT = 2;
    public const Byte SEQ_MENU_OFF_ESCAPE = 3;
    public const Byte SEQ_MENU_OFF_VICTORY_WITH_ENEMY = 4;
    public const Byte SEQ_VICTORY_CAMERA_MOVE = 0;
    public const Byte SEQ_VICTORY_FADEOUT = 1;
    public const Byte SEQ_VICTORY_WAIT = 2;

    public FF9StateBattleSystem()
    {
        this.enemy = new ENEMY[4];
        for (Int32 i = 0; i < this.enemy.Length; i++)
            this.enemy[i] = new ENEMY();
        this.enemy_type = new List<ENEMY_TYPE>();
        this.enemy_attack = new List<AA_DATA>();
        this.btl_list = new BTL_DATA();
        this.btl_data = new BTL_DATA[8];
        for (Int32 i = 0; i < this.btl_data.Length; i++)
            this.btl_data[i] = new BTL_DATA();
        this.cmd_buffer = new CMD_DATA[48]; // enemy_type.Length * 12, although unused
        for (Int32 i = 0; i < this.cmd_buffer.Length; i++)
            this.cmd_buffer[i] = new CMD_DATA();
        this.cmd_escape = new CMD_DATA();
        this.cmd_queue = new CMD_DATA();
        this.cur_cmd_list = new List<CMD_DATA>();
        this.seq_work_set = new SEQ_WORK_SET();
        this.btl_scene = new BTL_SCENE();
        this.btl2d_work_set = new BTL2D_WORK();
        this.status_data = new Dictionary<BattleStatusId, BattleStatusDataEntry>();
        this.aa_data = new Dictionary<BattleAbilityId, AA_DATA>();
        this.add_status = new Dictionary<StatusSetId, BattleStatusEntry>();
        this.map = new FF9StateBattleMap();
    }

    public ff9btl.ATTR attr;

    public Char usage;

    public ENEMY[] enemy;
    public List<ENEMY_TYPE> enemy_type;
    public List<AA_DATA> enemy_attack;

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
    public CMD_DATA cur_cmd
    {
        get
        {
            if (cur_cmd_list.Count == 0)
                return null;
            foreach (CMD_DATA cmd in cur_cmd_list)
                if (cmd.regist != null && cmd.regist.bi.player == 0)
                    return cmd;
            return cur_cmd_list[0];
        }
    }

    public List<CMD_DATA> cur_cmd_list;

    public GameObject s_cur;

    public UInt16 cmd_status; // FF9.Command.CMDSYS_STATUS_...
    public UInt16 enemy_die;

    public FF9StateBattleMap map;
    public SEQ_WORK_SET seq_work_set;
    public BTL_SCENE btl_scene;
    public BTL2D_WORK btl2d_work_set;

    public Dictionary<BattleStatusId, BattleStatusDataEntry> status_data;
    public Dictionary<BattleAbilityId, AA_DATA> aa_data;
    public Dictionary<StatusSetId, BattleStatusEntry> add_status;

    public IEnumerable<BattleUnit> EnumerateBattleUnits()
    {
        for (BTL_DATA data = btl_list.next; data != null; data = data.next)
            yield return new BattleUnit(data);
    }

    public BattleUnit GetUnit(Int32 index)
    {
        BTL_DATA data = btl_data[index];
        return new BattleUnit(data);
    }

    public IEnumerable<BattleUnit> EnumerateDeletedUnits()
    {
        HashSet<UInt16> presentList = new HashSet<UInt16>();
        for (BTL_DATA btl = btl_list.next; btl != null; btl = btl.next)
            presentList.Add(btl.btl_id);
        for (Int32 i = 0; i < 8; i++)
            if (btl_data[i].btl_id != 0 && !presentList.Contains(btl_data[i].btl_id))
                yield return new BattleUnit(btl_data[i]);
    }
}
