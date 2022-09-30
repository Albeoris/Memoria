using System;
using Memoria.Data;

public class FF9PARTY_INFO
{
	public Int32 party_ct;

	public CharacterId[] menu = new CharacterId[4];

	public CharacterId[] select = null;

	public Boolean[] fix = new Boolean[CommonState.DEFAULT_PLAYER_COUNT];
}
