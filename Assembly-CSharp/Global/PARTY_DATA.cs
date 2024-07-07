using Memoria;
using Memoria.Data;
using System;

public class PARTY_DATA
{
    public PARTY_DATA()
    {
        this.member = new PLAYER[4];
    }

    public PLAYER[] member;

    public UInt16 escape_no;
    public UInt16 summon_flag;
    public UInt32 gil;
    public Int32 battle_no;

    public Character GetCharacter(Int32 index) => new Character(member[index]);
    public CharacterId GetCharacterId(Int32 index) => index < 0 || index >= member.Length || member[index] == null ? CharacterId.NONE : member[index].info.slot_no;
}
