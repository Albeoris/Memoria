using System;

public class AA_DATA
{
	public AA_DATA()
	{
		this.Info = new BattleCommandInfo();
		this.Ref = new BTL_REF();
		Category = 0;
		AddStatusNo = 0;
		MP = 0;
		Type = 0;
		Vfx2 = 0;
	}

	public AA_DATA(BattleCommandInfo info, BTL_REF @ref, Byte category, Byte statusSetIndex, Byte mp, Byte type, UInt16 vfx2)
	{
		Info = info;
		Ref = @ref;
		Category = category;
		AddStatusNo = statusSetIndex;
		MP = mp;
		Type = type;
		Vfx2 = vfx2;
	}

	public BattleCommandInfo Info;
	public BTL_REF Ref;

	[Memoria.PatchableFieldAttribute]
	public Byte Category;
	[Memoria.PatchableFieldAttribute]
	public Byte AddStatusNo;
	[Memoria.PatchableFieldAttribute]
	public Byte MP;
	[Memoria.PatchableFieldAttribute]
	public Byte Type;
	[Memoria.PatchableFieldAttribute]
	public UInt16 Vfx2;

    // Delayed initialization
    public String Name;
}
