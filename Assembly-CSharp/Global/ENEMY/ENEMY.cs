using Memoria.Data;
using System;
using UnityEngine;

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
        this.trance_glowing_color = new Int32[3];
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
    public Int32[] trance_glowing_color;

    public Byte steal_unsuccessful_counter;

    public class ENEMY_INFO
    {
        public const UInt16 FLG_DIE_ATK = 1;
        public const UInt16 FLG_DIE_DMG = 2;
        public const UInt16 FLG_NON_DYING_BOSS = 4;

        public Boolean die_atk => (flags & FLG_DIE_ATK) != 0;
        public Boolean die_dmg => (flags & FLG_DIE_DMG) != 0;

        public Byte die_fade_rate;
        public Byte multiple;
        public Byte slave;
        public Int32 reserve;
        public UInt16 flags;
    }
}
