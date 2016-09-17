using System;
using FF9;

namespace Memoria
{
    public sealed class Character
    {
        private readonly PLAYER _data;

        internal Character(PLAYER data)
        {
            _data = data;
        }

        public static Character Find(BattleUnit unit)
        {
            PLAYER data = FF9StateSystem.Common.FF9.player[(int)unit.PresetId];
            return new Character(data);
        }

        public Byte Row => _data.info.row;
        public PlayerCategory Category => (PlayerCategory)_data.category;
    }

    public sealed class Weapon
    {
        private readonly WEAPON _data;

        internal Weapon(WEAPON data)
        {
            _data = data;
        }

        public WeaponCategory Category => (WeaponCategory)_data.category;
        public BattleStatus Status => (BattleStatus)FF9StateSystem.Battle.FF9Battle.add_status[_data.add_no];
        public UInt16 ModelId => _data.model_no;
        public Byte ScriptId => _data.Ref.prog_no;
        public Byte Power => _data.Ref.power;
        public EffectElement Element => (EffectElement)_data.Ref.attr;
        public Byte HitRate => _data.Ref.rate;

        public static Weapon Find(Byte weaponId)
        {
            return new Weapon(ff9weap._FF9Weapon_Data[btl_util.ff9WeaponNum(weaponId)]);
        }
    }
}