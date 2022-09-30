using System;
using FF9;
using Memoria.Data;

namespace Memoria
{
    public sealed class Character
    {
        internal readonly PLAYER Data;

        internal Character(PLAYER data)
        {
            Data = data;
        }

        public PLAYER GetData => Data;

        public String Name => Data.Name;
        public CharacterId Index => Data.Index;
        public CharacterPresetId PresetId => Data.PresetId;
        public CharacterEquipment Equipment => Data.equip;
        public Byte Row => Data.info.row;
        public CharacterCategory Category => (CharacterCategory)Data.category;
        public Boolean IsMainCharacter => !Data.IsSubCharacter;
        public Boolean IsSubCharacter => Data.IsSubCharacter;

        public static Character Find(BattleUnit unit)
        {
            PLAYER data = FF9StateSystem.Common.FF9.GetPlayer(unit.PlayerIndex);
            return new Character(data);
        }
    }

    public sealed class Weapon
    {
        private readonly ItemAttack _data;

        internal Weapon(ItemAttack data)
        {
            _data = data;
        }

        public WeaponCategory Category => (WeaponCategory)_data.Category;
        public BattleStatus Status => FF9StateSystem.Battle.FF9Battle.add_status[_data.StatusIndex].Value;
        public UInt16 ModelId => _data.ModelId;
        public Byte ScriptId => _data.Ref.ScriptId;
        public Byte Power => _data.Ref.Power;
        public EffectElement Element => (EffectElement)_data.Ref.Elements;
        public Byte HitRate => _data.Ref.Rate;

        public static Weapon Find(Byte weaponId)
        {
            return new Weapon(ff9weap.WeaponData[btl_util.ff9WeaponNum(weaponId)]);
        }
    }
}