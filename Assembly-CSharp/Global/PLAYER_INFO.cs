using System;

public class PLAYER_INFO
{
	public PLAYER_INFO(Byte index, Byte serial_no, Byte row, Byte win_pose, Byte party, Byte menu_type)
	{
		this.slot_no = index;
		this.serial_no = serial_no;
		this.row = row;
		this.win_pose = win_pose;
		this.party = party;
		this.menu_type = menu_type;
	}

	public Byte slot_no;

	public Byte serial_no;

	public Byte row;

	public Byte win_pose;

	public Byte party;

	public Byte menu_type;
}
