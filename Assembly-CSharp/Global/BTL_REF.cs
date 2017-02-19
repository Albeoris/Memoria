using System;

public class BTL_REF
{
	public BTL_REF()
	{
		this.prog_no = 0;
		this.power = 0;
		this.attr = 0;
		this.rate = 0;
	}

	public BTL_REF(Byte prog_no, Byte power, Byte attr, Byte rate)
	{
		this.prog_no = prog_no;
		this.power = power;
		this.attr = attr;
		this.rate = rate;
	}

	public Byte prog_no;

	public Byte power;

	public Byte attr;

	public Byte rate;
}
