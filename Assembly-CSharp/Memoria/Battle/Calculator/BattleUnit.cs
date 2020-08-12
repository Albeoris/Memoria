using System;
using System.Linq;
using FF9;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;

namespace Memoria
{
    public class BattleUnit
    {
        public CalcFlag Flags;
        public Int32 HpDamage;
        public Int32 MpDamage;

        internal BTL_DATA Data;
        
        internal BattleUnit(BTL_DATA data)
        {
            Data = data;
        }
        
        public UInt16 Id => Data.btl_id;
        public Boolean IsPlayer => Data.bi.player != 0;
        public Boolean IsSelected => Data.bi.target != 0;
        public Boolean IsSlave => Data.bi.slave != 0;
        public Boolean IsOutOfReach
        {
            get => Data.out_of_reach;
            set => Data.out_of_reach = value;
        }
        public Boolean CanMove => Data.bi.atb != 0;
        public CharacterIndex PlayerIndex => Data.bi.slot_no;

        public Byte Level => Data.level;
        public Byte Row => Data.bi.row;
        public Byte Position => Data.bi.line_no;

        public Boolean IsCovered
        {
            get => Data.bi.cover != 0;
            set => Data.bi.cover = (Byte)(value ? 1 : 0);
        }

        public Boolean IsDodged
        {
            get => Data.bi.dodge != 0;
            set => Data.bi.dodge = (Byte)(value ? 1 : 0);
        }

        public UInt32 MaximumHp
        {
            get => btl_para.GetLogicalHP(Data, true);
            set => btl_para.SetLogicalHP(Data, value, true);
        }
        public UInt32 CurrentHp
        {
            get => btl_para.GetLogicalHP(Data, false);
            set => btl_para.SetLogicalHP(Data, value, false);
        }

        public UInt32 MaximumMp
        {
            get => Data.max.mp;
            set => Data.max.mp = value;
        }
        public UInt32 CurrentMp
        {
            get => Data.cur.mp;
            set => Data.cur.mp = value;
        }

        public Int16 MaximumAtb => Data.max.at;
        public Int16 CurrentAtb
        {
            get => Data.cur.at;
            set => Data.cur.at = value;
        }

        public Byte Strength
        {
            get => Data.elem.str;
            set => Data.elem.str = value;
        }

        public Byte PhisicalDefence
        {
            get => Data.defence.PhisicalDefence;
            set => Data.defence.PhisicalDefence = value;
        }

        public Byte PhisicalEvade
        {
            get => Data.defence.PhisicalEvade;
            set => Data.defence.PhisicalEvade = value;
        }

        public Byte Magic
        {
            get => Data.elem.mgc;
            set => Data.elem.mgc = value;
        }

        public Byte MagicDefence
        {
            get => Data.defence.MagicalDefence;
            set => Data.defence.MagicalDefence = value;
        }

        public Byte MagicEvade
        {
            get => Data.defence.MagicalEvade;
            set => Data.defence.MagicalEvade = value;
        }

        public Byte Dexterity
        {
            get => Data.elem.dex;
            set => Data.elem.dex = value;
        }
        public Byte Will
        {
            get => Data.elem.wpr;
            set => Data.elem.wpr = value;
        }

        public Boolean HasTrance => Data.bi.t_gauge != 0;
        public Boolean InTrance => (CurrentStatus & BattleStatus.Trance) != 0;
        public Byte Trance
        {
            get => Data.trance;
            set => Data.trance = value;
        }

        public Int32 Fig
        {
            get => Data.fig;
            set => Data.fig = value;
        }

        public Byte WeaponRate => Data.weapon != null ? Data.weapon.Ref.Rate : (Byte)0;
        public Byte WeaponPower => Data.weapon != null ? Data.weapon.Ref.Power : (Byte)0;
        public EffectElement WeaponElement => (EffectElement)(Data.weapon != null ? Data.weapon.Ref.Elements : 0);
        public BattleStatus WeaponStatus => Data.weapon != null ? FF9StateSystem.Battle.FF9Battle.add_status[Data.weapon.StatusIndex].Value : 0;

        public Character Player => Character.Find(this);
        public CharacterCategory PlayerCategory => Player.Category;
        public EnemyCategory Category => IsPlayer ? EnemyCategory.Humanoid : (EnemyCategory)btl_util.getEnemyTypePtr(Data).category;
        public WeaponCategory WeapCategory => (WeaponCategory)(Data.weapon != null ? Data.weapon.Category : 0);
        public BattleEnemy Enemy => new BattleEnemy(btl_util.getEnemyPtr(Data));
        public ENEMY_TYPE EnemyType => btl_util.getEnemyTypePtr(Data);
        public String Name => IsPlayer ? Player.Name : Enemy.Name;

        public BattleStatus CurrentStatus
        {
            get => Data.stat.cur;
            set => Data.stat.cur = value;
        }

        public BattleStatus PermanentStatus
        {
            get => Data.stat.permanent;
            set => Data.stat.permanent = value;
        }
         
        public BattleStatus ResistStatus
        {
            get => Data.stat.invalid;
            set => Data.stat.invalid = value;
        }

        public EffectElement BonusElement
        {
            get => (EffectElement)Data.p_up_attr;
            set => Data.p_up_attr = (Byte)value;
        }
        public EffectElement WeakElement
        {
            get => (EffectElement)Data.def_attr.weak;
            set => Data.def_attr.weak = (Byte)value;
        }
        public EffectElement GuardElement
        {
            get => (EffectElement)Data.def_attr.invalid;
            set => Data.def_attr.invalid = (Byte)value;
        }
        public EffectElement AbsorbElement
        {
            get => (EffectElement)Data.def_attr.absorb;
            set => Data.def_attr.absorb = (Byte)value;
        }
        public EffectElement HalfElement
        {
            get => (EffectElement)Data.def_attr.half;
            set => Data.def_attr.half = (Byte)value;
        }

        public Boolean IsLevitate => HasCategory(EnemyCategory.Flight) || IsUnderAnyStatus(BattleStatus.Float);
        public Boolean IsZombie => HasCategory(EnemyCategory.Undead) || IsUnderAnyStatus(BattleStatus.Zombie);
        public Boolean HasLongRangeWeapon => HasCategory(WeaponCategory.LongRange);

        public WeaponItem Weapon => (WeaponItem)btl_util.getWeaponNumber(Data);
        public Boolean IsHealingRod => IsPlayer && Weapon == WeaponItem.HealingRod;

        public Boolean[] StatModifier
        {
            get
            {
                return this.Data.stat_modifier;
            }
        }

        public UInt16 SummonCount => Data.summon_count;
        public Int16 CriticalRateBonus
        {
            get => Data.critical_rate_deal_bonus;
            set => Data.critical_rate_deal_bonus = value;
        }
        public Int16 CriticalRateWeakening
        {
            get => Data.critical_rate_receive_bonus;
            set => Data.critical_rate_receive_bonus = value;
        }

        public MutableBattleCommand AttackCommand => Commands[0];
        
        private MutableBattleCommand[] _commands;
        public MutableBattleCommand[] Commands => _commands ??= Data.cmd.Select(cmd => new MutableBattleCommand(cmd)).ToArray();

        public Boolean IsUnderStatus(BattleStatus status)
        {
            return (CurrentStatus & status) != 0;
        }

        public Boolean IsUnderPermanentStatus(BattleStatus status)
        {
            return (PermanentStatus & status) != 0;
        }

        public Boolean IsUnderAnyStatus(BattleStatus status)
        {
            return ((CurrentStatus | PermanentStatus) & status) != 0;
        }

        public Boolean HasCategory(CharacterCategory category)
        {
            return (PlayerCategory & category) != 0;
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
            return btl_stat.RemoveStatuses(Data, status) == 2U;
        }

        public void RemoveStatus(BattleStatus status)
        {
            btl_stat.RemoveStatus(Data, status);
        }

        public void AlterStatus(BattleStatus status)
        {
            btl_stat.AlterStatus(Data, status);
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
            btl_sys.SavePlayerData(Data, true);
            btl_sys.DelCharacter(Data);
            Data.SetDisappear(1);
            // The two following lines have been switched for fixing an UI bug (ATB bar glowing, etc... when an ally is snorted)
            // It seems to fix the bug without introducing another one (the HP/MP figures update strangely but that's because of how the UI cells are managed)
            UIManager.Battle.RemovePlayerFromAction(Data.btl_id, true);
            UIManager.Battle.DisplayParty();
        }

        public void FaceTheEnemy()
        {
            FaceAsUnit(this);
        }

        public void FaceAsUnit(BattleUnit unit)
        {
            Int32 angle = btl_mot.GetDirection(unit);
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
            UIManager.Battle.SetBattleLibra(this);
        }

        public void Detect()
        {
            UIManager.Battle.SetBattlePeeping(this);
        }

        public Int32 GetIndex()
        {
            Int32 index = 0;

            while (1 << index != Data.btl_id)
                ++index;

            return index;
        }
    }
}
