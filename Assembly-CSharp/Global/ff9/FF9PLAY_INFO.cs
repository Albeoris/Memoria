using System;
using Memoria.Data;

public class FF9PLAY_INFO
{
	public FF9PLAY_INFO()
	{
		this.Base = new PLAYER_BASE();
		this.sa = new UInt32[2];
	    this.equip = new CharacterEquipment();
	}

	public PLAYER_BASE Base;

	public UInt32[] sa;

	public UInt32 cur_hp;

	public UInt32 cur_mp;

	public CharacterEquipment equip;
}
