using System;

public class BTL_REF
{
	[Memoria.PatchableFieldAttribute]
	public Byte ScriptId;
	[Memoria.PatchableFieldAttribute]
	public Byte Power;
	[Memoria.PatchableFieldAttribute]
	public Byte Elements;
	[Memoria.PatchableFieldAttribute]
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
