using Memoria.Data;
using System;
using System.Collections.Generic;

public class FF9PARTY_INFO
{
	public Int32 party_ct;

	public CharacterId[] menu = new CharacterId[4];

	public CharacterId[] select = null;

	public HashSet<CharacterId> fix = new HashSet<CharacterId>();
}
