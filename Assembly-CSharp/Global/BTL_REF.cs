using System;

public class BTL_REF
{
    public Byte ScriptId;
    public Byte Power;
    public Byte Elements;
    public Byte Rate;

    public BTL_REF()
	{
		ScriptId = 0;
		Power = 0;
		Elements = 0;
		Rate = 0;
	}

	public BTL_REF(Byte scriptId, Byte power, Byte elements, Byte rate)
	{
		ScriptId = scriptId;
		Power = power;
		Elements = elements;
		Rate = rate;
	}
}
