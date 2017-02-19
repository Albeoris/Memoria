using System;

public class CMD_INFO
{
	public CMD_INFO()
	{
		this.cursor = 0;
		this.def_cur = 0;
		this.sub_win = 0;
		this.vfx_no = 0;
		this.sfx_no = 0;
		this.dead = 0;
		this.def_cam = 0;
		this.def_dead = 0;
	}

	public CMD_INFO(Byte cursor, Byte def_cur, Byte sub_win, Int16 vfx_no, Int16 sfx_no, Byte dead, Byte def_cam, Byte def_dead)
	{
		this.cursor = cursor;
		this.def_cur = def_cur;
		this.sub_win = sub_win;
		this.vfx_no = vfx_no;
		this.sfx_no = sfx_no;
		this.dead = dead;
		this.def_cam = def_cam;
		this.def_dead = def_dead;
	}

	public Byte cursor;

	public Byte def_cur;

	public Byte sub_win;

	public Int16 vfx_no;

	public Int16 sfx_no;

	public Byte dead;

	public Byte def_cam;

	public Byte def_dead;
}
