using System;

public class BTL_INFO
{
	public Byte player;

	public Byte row;

	public Byte slot_no;

	public Byte line_no;

	public Byte ofs;

	public Byte atb;

	public Byte dmg_mot_f;

	public Byte def_idle;

	public Byte cmd_idle;

	public Byte death_f;

	public Byte stop_anim;

	public Byte disappear;

	public Byte slave;

	public Byte shadow;

	public BTL_DATA cover_unit;

	public Byte cover => (Byte)(cover_unit != null ? 1 : 0);

	public Byte dodge;

	public Byte die_snd_f;

	public Byte target;

	public Byte select;

	public Byte t_gauge;

	public Byte reserve;
}
