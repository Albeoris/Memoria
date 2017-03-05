using System;
using FF9;
using Memoria.Data;
using UnityEngine;

namespace Memoria
{
    public class BattleUnit
    {
        public CalcFlag Flags;
        public Int16 HpDamage;
        public Int16 MpDamage;

        internal BTL_DATA Data;

        internal BattleUnit(BTL_DATA data)
        {
            Data = data;
        }

        public UInt16 Id => Data.btl_id;
        public Boolean IsPlayer => Data.bi.player != 0;
        public Boolean IsCovered => Data.bi.cover != 0;
        public Boolean IsSelected => Data.bi.target != 0;
        public CharacterPresetId PresetId => (CharacterPresetId)Data.bi.slot_no;

        public Byte Level => Data.level;
        public Byte Row => Data.bi.row;
        public UInt16 MaximumHp => Data.max.hp;
        public UInt16 CurrentHp
        {
            get { return Data.cur.hp; }
            set { Data.cur.hp = value; }
        }

        public Int16 MaximumMp => Data.max.mp;
        public Int16 CurrentMp
        {
            get { return Data.cur.mp; }
            set { Data.cur.mp = value; }
        }

        public Byte Strength
        {
            get { return Data.elem.str; }
            set { Data.elem.str = value; }
        }

        public Byte PhisicalDefence
        {
            get { return Data.defence.PhisicalDefence; }
            set { Data.defence.PhisicalDefence = value; }
        }

        public Byte PhisicalEvade => Data.defence.PhisicalEvade;

        public Byte Magic
        {
            get { return Data.elem.mgc; }
            set { Data.elem.mgc = value; }
        }

        public Byte MagicDefence
        {
            get { return Data.defence.MagicalDefence; }
            set { Data.defence.MagicalDefence = value; }
        }

        public Byte MagicEvade => Data.defence.MagicalEvade;

        public Byte Dexterity => Data.elem.dex;
        public Byte Will => Data.elem.wpr;

        public Byte Trance
        {
            get { return Data.trance; }
            set { Data.trance = value; }
        }

        public Int16 Fig
        {
            get { return Data.fig; }
            set { Data.fig = value; }
        }

        public Byte WeaponRate => Data.weapon.Ref.Rate;
        public Byte WeaponPower => Data.weapon.Ref.Power;
        public EffectElement WeaponElement => (EffectElement)Data.weapon.Ref.Elements;
        public BattleStatus WeaponStatus => FF9StateSystem.Battle.FF9Battle.add_status[Data.weapon.StatusIndex].Value;

        public Character Player => Character.Find(this);
        public CharacterCategory PlayerCategory => Player.Category;
        public ENEMY Enemy => btl_util.getEnemyPtr(Data);
        public ENEMY_TYPE EnemyType => btl_util.getEnemyTypePtr(Data);

        public BattleStatus CurrentStatus => (BattleStatus)Data.stat.cur;
        public BattleStatus PermanentStatus => (BattleStatus)Data.stat.permanent;
        public EffectElement BonusElement => (EffectElement)Data.p_up_attr;

        public EffectElement WeakElement => (EffectElement)Data.def_attr.weak;
        public EffectElement GuardElement => (EffectElement)Data.def_attr.invalid;
        public EffectElement AbsorbElement => (EffectElement)Data.def_attr.absorb;
        public EffectElement HalfElement => (EffectElement)Data.def_attr.half;

        public Boolean IsLevitate => HasCategory(EnemyCategory.Flight) || IsUnderStatus(BattleStatus.Float);
        public Boolean IsZombie => HasCategory(EnemyCategory.Undead) || IsUnderStatus(BattleStatus.Zombie);
        public Boolean HasLongReach => HasSupportAbility(SupportAbility1.LongReach) || HasCategory(WeaponCategory.LongRange);

        public WeaponItem Weapon => (WeaponItem)btl_util.getWeaponNumber(Data);
        public Boolean IsHealer => IsPlayer && (HasSupportAbility(SupportAbility1.Healer) || Weapon == WeaponItem.HealingRod);

        public Boolean IsUnderStatus(BattleStatus status)
        {
            return (CurrentStatus & status) != 0;
        }

        public Boolean IsUnderPermanentStatus(BattleStatus status)
        {
            return (PermanentStatus & status) != 0;
        }

        public Boolean HasCategory(EnemyCategory category)
        {
            return btl_util.CheckEnemyCategory(Data, (Byte)category);
        }

        public Boolean HasCategory(WeaponCategory category)
        {
            if (Data.weapon == null)
                return false;

            return ((WeaponCategory)Data.weapon.Category & category) != 0;
        }

        public Boolean HasSupportAbility(SupportAbility1 ability)
        {
            return (Data.sa[0] & (UInt32)ability) != 0;
        }

        public Boolean HasSupportAbility(SupportAbility2 ability)
        {
            return (Data.sa[1] & (UInt32)ability) != 0;
        }

        public Boolean TryRemoveStatuses(BattleStatus status)
        {
            return btl_stat.RemoveStatuses(Data, (UInt32)status) == 2U;
        }

        public void RemoveStatus(BattleStatus status)
        {
            btl_stat.RemoveStatus(Data, (UInt32)status);
        }

        public void AlterStatus(BattleStatus status)
        {
            btl_stat.AlterStatus(Data, (UInt32)status);
        }

        public void Kill()
        {
            Data.cur.hp = 0;
            Data.bi.death_f = 1;

            if (Data.bi.player == 0)
                btl_util.SetEnemyDieSound(Data, btl_util.getEnemyTypePtr(Data).die_snd_no);

            if (!btl_mot.checkMotion(Data, Data.bi.def_idle) && (Data.bi.player == 0 || !btl_mot.checkMotion(Data, 9)) && !btl_util.isCurCmdOwner(Data))
            {
                btl_mot.setMotion(Data, Data.bi.def_idle);
                Data.evt.animFrame = 0;
            }
        }

        public void Remove()
        {
            battle.btl_bonus.member_flag &= (Byte)~(1 << Data.bi.line_no);
            btl_cmd.ClearSysPhantom(Data);
            btl_cmd.KillCommand3(Data);
            btl_sys.SavePlayerData(Data, 1U);
            btl_sys.DelCharacter(Data);
            Data.SetDisappear(1);
            UIManager.Battle.DisplayInfomation();
            UIManager.Battle.RemovePlayerFromAction(Data.btl_id, true);
        }

        public void FaceTheEnemy()
        {
            FaceAsUnit(this);
        }

        public void FaceAsUnit(BattleUnit unit)
        {
            Int32 angle = btl_mot.setDirection(unit.Data);
            Data.evt.rotBattle.eulerAngles = new Vector3(Data.evt.rotBattle.eulerAngles.x, angle, Data.evt.rotBattle.eulerAngles.z);
            Data.rot.eulerAngles = new Vector3(Data.rot.eulerAngles.x, angle, Data.rot.eulerAngles.z);
        }

        public void ChangeRowToDefault()
        {
            if (IsPlayer && Row != Player.Row)
                btl_para.SwitchPlayerRow(Data);
        }

        public void ChangeRow()
        {
            btl_para.SwitchPlayerRow(Data);
        }

        public void Change(BattleUnit unit)
        {
            Data = unit.Data;
        }

        public void Libra()
        {
            UIManager.Battle.SetBattleLibra(Data);
        }

        public void Detect()
        {
            UIManager.Battle.SetBattlePeeping(Data);
        }
    }
}