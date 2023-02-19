using System;
using Memoria;
using Memoria.Data;

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

	public UInt32 pad;

    public Character GetCharacter(Int32 index) => new Character(member[index]);
	public CharacterId GetCharacterId(Int32 index) => member[index] == null ? CharacterId.NONE : member[index].info.slot_no;
}
