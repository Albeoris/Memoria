using System;

public class FF9PARTY_INFO
{
	public Int32 party_ct;

	public Byte[] menu = new Byte[4];

	public Byte[] select = new Byte[PartySettingUI.FF9PARTY_PLAYER_MAX];

	public Boolean[] fix = new Boolean[9];
}
