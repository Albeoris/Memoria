using System;
using Memoria.Data;

public class PLAYER_INFO
{
	public PLAYER_INFO(CharacterId index, CharacterSerialNumber serial_no, Byte row, Byte win_pose, Byte party, CharacterPresetId menu_type)
	{
		this.slot_no = index;
		this.serial_no = serial_no;
		this.row = row;
		this.win_pose = win_pose;
		this.party = party;
		this.menu_type = menu_type;
		this.sub_replaced = index <= CharacterId.Freya || index >= CharacterId.Cinna;
	}

	public CharacterId slot_no;

	public CharacterSerialNumber serial_no;

	public Byte row;

	public Byte win_pose;

	public Byte party;

	public CharacterPresetId menu_type;

	public Boolean sub_replaced;
}
