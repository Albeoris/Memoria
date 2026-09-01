using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class ItemInfo : ICsvEntry
    {
        public RegularItem Id;
        public FF9ITEM_DATA Data;

        public Int32 WeaponId
        {
            get => Data.weapon_id;
            set => Data.weapon_id = value;
        }

        public Int32 ArmorId
        {
            get => Data.armor_id;
            set => Data.armor_id = value;
        }

        public Int32 EffectId
        {
            get => Data.effect_id;
            set => Data.effect_id = value;
        }

        public static FF9ITEM_DATA GetExisting(RegularItem id)
        {
            if (ff9item._FF9Item_Data.TryGetValue(id, out FF9ITEM_DATA result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Boolean hasAuxIds = metadata.HasOption($"IncludeAuxiliaryIds");

            if (metadata.HasOption($"IncludeId") || metadata.IsAppendMode)
                Id = (RegularItem)CsvParser.Int32(raw[index++]);
            else
                Id = (RegularItem)(-1);

            Data = metadata.IsAppendMode ? GetExisting(Id) : new FF9ITEM_DATA();

            if (metadata.HasField("WeaponId")) WeaponId = hasAuxIds || metadata.HasOption($"IncludeWeaponId") ? CsvParser.Int32(raw[index++]) : -1;
            if (metadata.HasField("ArmorId")) ArmorId = hasAuxIds || metadata.HasOption($"IncludeArmorId") ? CsvParser.Int32(raw[index++]) : -1;
            if (metadata.HasField("EffectId")) EffectId = hasAuxIds || metadata.HasOption($"IncludeEffectId") ? CsvParser.Int32(raw[index++]) : -1;

            if (metadata.HasField("Price")) Data.price = CsvParser.UInt32(raw[index++]);
            if (!metadata.HasOption($"IncludeSellingPrice"))
                Data.selling_price = (Int32)(Data.price / 2);
            else if (metadata.HasField("SellingPrice"))
                Data.selling_price = CsvParser.Int32(raw[index++]);

            if (metadata.HasField("GraphicsId")) Data.shape = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("ColorId")) Data.color = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Quality")) Data.eq_lv = CsvParser.Single(raw[index++]);
            if (metadata.HasField("BonusId")) Data.bonus = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("AbilityIds")) Data.ability = CsvParser.AnyAbilityArray(raw[index++]);

            if (metadata.HasField("TypeMask"))
            {
                Byte type = 0;
                for (Int32 i = 0; i < 8; i++)
                {
                    type <<= 1;
                    type |= CsvParser.Byte(raw[index++]);
                }
                Data.type = (ItemType)type;
            }

            if (metadata.HasField("Order")) Data.sort = CsvParser.Single(raw[index++]);

            if (metadata.HasOption($"IncludeUseCondition") && metadata.HasField("UseCondition"))
                Data.use_condition = CsvParser.String(raw[index++]);
            else
                Data.use_condition = String.Empty;

            if (metadata.HasField("CharacterMask"))
            {
                UInt64 equippable = 0;
                for (Int32 i = 0; i < 12; i++)
                {
                    equippable <<= 1;
                    equippable |= CsvParser.Byte(raw[index++]);
                }
                for (Int32 i = 12; index < raw.Length; i++)
                    if (CsvParser.Byte(raw[index++]) != 0)
                        equippable |= 1ul << i;
                Data.equip = equippable;
            }
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            Boolean hasAuxIds = metadata.HasOption($"IncludeAuxiliaryIds");
            if (metadata.HasOption($"Include{nameof(Id)}"))
                writer.Int32((Int32)Id);
            if (hasAuxIds || metadata.HasOption($"IncludeWeaponId"))
                writer.Int32(Data.weapon_id);
            if (hasAuxIds || metadata.HasOption($"IncludeArmorId"))
                writer.Int32(Data.armor_id);
            if (hasAuxIds || metadata.HasOption($"IncludeEffectId"))
                writer.Int32(Data.effect_id);

            writer.UInt32(Data.price);
            if (metadata.HasOption($"IncludeSellingPrice"))
                writer.Int32(Data.selling_price);
            writer.Int32(Data.shape);
            writer.Int32(Data.color);
            writer.Single(Data.eq_lv);
            writer.Int32(Data.bonus);
            writer.AnyAbilityArray(Data.ability);

            writer.Boolean(Weapon);
            writer.Boolean(Armlet);
            writer.Boolean(Helmet);
            writer.Boolean(Armor);
            writer.Boolean(Accessory);
            writer.Boolean(Item);
            writer.Boolean(Gem);
            writer.Boolean(Usable);

            writer.Single(Data.sort);

            if (metadata.HasOption($"IncludeUseCondition"))
                writer.String(Data.use_condition);

            writer.Boolean(Zidane);
            writer.Boolean(Vivi);
            writer.Boolean(Garnet);
            writer.Boolean(Steiner);
            writer.Boolean(Freya);
            writer.Boolean(Quina);
            writer.Boolean(Eiko);
            writer.Boolean(Amarant);
            writer.Boolean(Cinna);
            writer.Boolean(Marcus);
            writer.Boolean(Blank);
            writer.Boolean(Beatrix);
        }

        public Boolean Weapon => (Data.type & ItemType.Weapon) == ItemType.Weapon;
        public Boolean Armlet => (Data.type & ItemType.Armlet) == ItemType.Armlet;
        public Boolean Helmet => (Data.type & ItemType.Helmet) == ItemType.Helmet;
        public Boolean Armor => (Data.type & ItemType.Armor) == ItemType.Armor;
        public Boolean Accessory => (Data.type & ItemType.Accessory) == ItemType.Accessory;
        public Boolean Item => (Data.type & ItemType.Item) == ItemType.Item;
        public Boolean Gem => (Data.type & ItemType.Gem) == ItemType.Gem;
        public Boolean Usable => (Data.type & ItemType.Usable) == ItemType.Usable;

        public ItemCharacter CharacterMask => (ItemCharacter)Data.equip;
        public Boolean Zidane => (CharacterMask & ItemCharacter.Zidane) == ItemCharacter.Zidane;
        public Boolean Vivi => (CharacterMask & ItemCharacter.Vivi) == ItemCharacter.Vivi;
        public Boolean Garnet => (CharacterMask & ItemCharacter.Garnet) == ItemCharacter.Garnet;
        public Boolean Steiner => (CharacterMask & ItemCharacter.Steiner) == ItemCharacter.Steiner;
        public Boolean Freya => (CharacterMask & ItemCharacter.Freya) == ItemCharacter.Freya;
        public Boolean Quina => (CharacterMask & ItemCharacter.Quina) == ItemCharacter.Quina;
        public Boolean Eiko => (CharacterMask & ItemCharacter.Eiko) == ItemCharacter.Eiko;
        public Boolean Amarant => (CharacterMask & ItemCharacter.Amarant) == ItemCharacter.Amarant;
        public Boolean Cinna => (CharacterMask & ItemCharacter.Cinna) == ItemCharacter.Cinna;
        public Boolean Marcus => (CharacterMask & ItemCharacter.Marcus) == ItemCharacter.Marcus;
        public Boolean Blank => (CharacterMask & ItemCharacter.Blank) == ItemCharacter.Blank;
        public Boolean Beatrix => (CharacterMask & ItemCharacter.Beatrix) == ItemCharacter.Beatrix;
    }

    [Flags]
    public enum ItemType : byte
    {
        Weapon = 128,
        Armlet = 64,
        Helmet = 32,
        Armor = 16,
        Accessory = 8,

        Item = 4,
        Gem = 2,
        Usable = 1,

        AnyEquipment = Weapon | Armlet | Helmet | Armor | Accessory,
        AnyItem = Item | Gem | Usable
    }

    [Flags]
    public enum ItemCharacter : UInt64
    {
        Zidane = 2048,
        Vivi = 1024,
        Garnet = 512,
        Steiner = 256,
        Freya = 128,
        Quina = 64,
        Eiko = 32,
        Amarant = 16,
        Cinna = 8,
        Marcus = 4,
        Blank = 2,
        Beatrix = 1
    }
}
