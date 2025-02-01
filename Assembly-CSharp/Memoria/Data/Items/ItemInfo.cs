using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class ItemInfo : ICsvEntry
    {
        public RegularItem Id;
        public UInt32 Price;
        public Int32 SellingPrice;
        public ItemCharacter CharacterMask;
        public Byte GraphicsId;
        public Byte ColorId;
        public Single Quality;
        public Int32 BonusId;
        public Int32[] AbilityIds;
        public ItemType TypeMask;
        public Single Order;
        public String UseCondition;
        public Int32 WeaponId;
        public Int32 ArmorId;
        public Int32 EffectId;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Boolean hasAuxIds = metadata.HasOption($"IncludeAuxiliaryIds");

            Id = metadata.HasOption($"Include{nameof(Id)}") ? (RegularItem)CsvParser.Int32(raw[index++]) : (RegularItem)(-1);
            WeaponId = hasAuxIds || metadata.HasOption($"Include{nameof(WeaponId)}") ? CsvParser.Int32(raw[index++]) : -1;
            ArmorId = hasAuxIds || metadata.HasOption($"Include{nameof(ArmorId)}") ? CsvParser.Int32(raw[index++]) : -1;
            EffectId = hasAuxIds || metadata.HasOption($"Include{nameof(EffectId)}") ? CsvParser.Int32(raw[index++]) : -1;

            Price = CsvParser.UInt32(raw[index++]);
            if (metadata.HasOption($"Include{nameof(SellingPrice)}"))
                SellingPrice = CsvParser.Int32(raw[index++]);
            else
                SellingPrice = (Int32)(Price / 2);
            GraphicsId = CsvParser.Byte(raw[index++]);
            ColorId = CsvParser.Byte(raw[index++]);
            Quality = CsvParser.Single(raw[index++]);
            BonusId = CsvParser.Int32(raw[index++]);
            AbilityIds = CsvParser.AnyAbilityArray(raw[index++]);

            Byte type = 0;
            for (Int32 i = 0; i < 8; i++)
            {
                type <<= 1;
                type |= CsvParser.Byte(raw[index++]);
            }
            TypeMask = (ItemType)type;

            Order = CsvParser.Single(raw[index++]);

            if (metadata.HasOption($"Include{nameof(UseCondition)}"))
                UseCondition = CsvParser.String(raw[index++]);
            else
                UseCondition = String.Empty;

            UInt64 equippable = 0;
            for (Int32 i = 0; i < 12; i++)
            {
                equippable <<= 1;
                equippable |= CsvParser.Byte(raw[index++]);
            }
            for (Int32 i = 12; index < raw.Length; i++)
                if (CsvParser.Byte(raw[index++]) != 0)
                    equippable |= 1ul << i;

            CharacterMask = (ItemCharacter)equippable;
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            Boolean hasAuxIds = metadata.HasOption($"IncludeAuxiliaryIds");
            if (metadata.HasOption($"Include{nameof(Id)}"))
                writer.Int32((Int32)Id);
            if (hasAuxIds || metadata.HasOption($"Include{nameof(WeaponId)}"))
                writer.Int32(WeaponId);
            if (hasAuxIds || metadata.HasOption($"Include{nameof(ArmorId)}"))
                writer.Int32(ArmorId);
            if (hasAuxIds || metadata.HasOption($"Include{nameof(EffectId)}"))
                writer.Int32(EffectId);

            writer.UInt32(Price);
            if (metadata.HasOption($"Include{nameof(SellingPrice)}"))
                writer.Int32(SellingPrice);
            writer.Byte(GraphicsId);
            writer.Byte(ColorId);
            writer.Single(Quality);
            writer.Int32(BonusId);
            writer.AnyAbilityArray(AbilityIds);

            writer.Boolean(Weapon);
            writer.Boolean(Armlet);
            writer.Boolean(Helmet);
            writer.Boolean(Armor);
            writer.Boolean(Accessory);
            writer.Boolean(Item);
            writer.Boolean(Gem);
            writer.Boolean(Usable);

            writer.Single(Order);

            if (metadata.HasOption($"Include{nameof(UseCondition)}"))
                writer.String(UseCondition);

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

        public FF9ITEM_DATA ToItemData()
        {
            return new FF9ITEM_DATA(Price, SellingPrice, (UInt64)CharacterMask, GraphicsId, ColorId, Quality, BonusId, AbilityIds, TypeMask, Order, UseCondition, WeaponId, ArmorId, EffectId);
        }

        public Boolean Weapon => (TypeMask & ItemType.Weapon) == ItemType.Weapon;
        public Boolean Armlet => (TypeMask & ItemType.Armlet) == ItemType.Armlet;
        public Boolean Helmet => (TypeMask & ItemType.Helmet) == ItemType.Helmet;
        public Boolean Armor => (TypeMask & ItemType.Armor) == ItemType.Armor;
        public Boolean Accessory => (TypeMask & ItemType.Accessory) == ItemType.Accessory;
        public Boolean Item => (TypeMask & ItemType.Item) == ItemType.Item;
        public Boolean Gem => (TypeMask & ItemType.Gem) == ItemType.Gem;
        public Boolean Usable => (TypeMask & ItemType.Usable) == ItemType.Usable;

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
