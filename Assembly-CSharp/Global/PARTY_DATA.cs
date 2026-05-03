using Memoria;
using Memoria.Data;
using System;
using System.Linq;

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

    public CharacterId GetCharacterId(Int32 index) => index < 0 || index >= member.Length || member[index] == null ? CharacterId.NONE : member[index].info.slot_no;
    public Int32 MemberCount => member.Count(p => p != null);

    public Boolean IsInParty(CharacterId charId)
    {
        return charId != CharacterId.NONE && member.Any(p => p?.Index == charId);
    }
}
