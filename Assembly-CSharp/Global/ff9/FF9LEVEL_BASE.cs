using System;

public class FF9LEVEL_BASE
{
	public FF9LEVEL_BASE(Byte dex, Byte str, Byte mgc, Byte wpr, Byte cap)
	{
		this.dex = dex;
		this.str = str;
		this.mgc = mgc;
		this.wpr = wpr;
		this.cap = cap;
	}

	public Byte dex;

	public Byte str;

	public Byte mgc;

	public Byte wpr;

	public Byte cap;

	public Byte pad;
}
