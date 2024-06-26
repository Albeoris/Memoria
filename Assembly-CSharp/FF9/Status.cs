using System;
using Memoria.Data;

namespace FF9
{
	public class Status
	{
	    public static Boolean checkCurStat(BTL_DATA btl, BattleStatus status)
	    {
	        return (btl.stat.cur & status) != 0u;
        }
	}
}
