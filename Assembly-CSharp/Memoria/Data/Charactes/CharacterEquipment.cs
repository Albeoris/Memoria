using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterEquipment : ICsvEntry
    {
        public const Int32 Length = 5;
        public const Byte EmptyItemId = Byte.MaxValue;

        public String Comment;
        public Int32 Id;

        public Byte Weapon;
        public Byte Head;
        public Byte Wrist;
        public Byte Armor;
        public Byte Accessory;

        public void Absorb(CharacterEquipment other)
        {
            Weapon = other.Weapon;
            Head = other.Head;
            Wrist = other.Wrist;
            Armor = other.Armor;
            Accessory = other.Accessory;
        }

        public CharacterEquipment Clone()
        {
            return new CharacterEquipment {Weapon = Weapon, Head = Head, Wrist = Wrist, Armor = Armor, Accessory = Accessory};
        }

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            Weapon = CsvParser.ByteOrMinusOne(raw[2]);
            Head = CsvParser.ByteOrMinusOne(raw[3]);
            Wrist = CsvParser.ByteOrMinusOne(raw[4]);
            Armor = CsvParser.ByteOrMinusOne(raw[5]);
            Accessory = CsvParser.ByteOrMinusOne(raw[6]);
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.String(Comment);
            writer.Int32(Id);

            writer.ByteOrMinusOne(Weapon);
            writer.ByteOrMinusOne(Head);
            writer.ByteOrMinusOne(Wrist);
            writer.ByteOrMinusOne(Armor);
            writer.ByteOrMinusOne(Accessory);
        }

        public Byte this[Int32 index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Weapon;
                    case 1:
                        return Head;
                    case 2:
                        return Wrist;
                    case 3:
                        return Armor;
                    case 4:
                        return Accessory;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Weapon = value;
                        break;
                    case 1:
                        Head = value;
                        break;
                    case 2:
                        Wrist = value;
                        break;
                    case 3:
                        Armor = value;
                        break;
                    case 4:
                        Accessory = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }
    }
}