using System;
using Memoria.Data;

public class STAT_INFO
{
	public STAT_INFO()
	{
		this.cnt = new STAT_CNT();
	}

	public BattleStatus invalid;

	public BattleStatus permanent;

	public BattleStatus cur;

	public STAT_CNT cnt;
}
