using System;

public class AA_DATA
{
	public AA_DATA()
	{
		this.Info = new BattleCommandInfo();
		this.Ref = new BTL_REF();
	}

	public AA_DATA(BattleCommandInfo info, BTL_REF @ref, Byte category, Byte statusSetIndex, Byte mp, Byte type, UInt16 vfx2)
	{
		Info = info;
		Ref = @ref;
		Category = category;
		AddNo = statusSetIndex;
		MP = mp;
		Type = type;
		Vfx2 = vfx2;
	}

	public BattleCommandInfo Info;
	public BTL_REF Ref;
	public Byte Category;
	public Byte AddNo;
	public Byte MP;
    public Byte Type;
	public UInt16 Vfx2;

    // Delayed initialization
    public String Name;
}
