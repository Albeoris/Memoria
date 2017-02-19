using System;

public class AA_DATA
{
	public AA_DATA()
	{
		this.Info = new CMD_INFO();
		this.Ref = new BTL_REF();
	}

	public AA_DATA(CMD_INFO Info, BTL_REF Ref, Byte Category, Byte AddNo, Byte MP, Byte Type, UInt16 Vfx2, UInt16 Name)
	{
		this.Info = Info;
		this.Ref = Ref;
		this.Category = Category;
		this.AddNo = AddNo;
		this.MP = MP;
		this.Type = Type;
		this.Vfx2 = Vfx2;
	}

	public CMD_INFO Info;

	public BTL_REF Ref;

	public Byte Category;

	public Byte AddNo;

	public Byte MP;

	public Byte Type;

	public UInt16 Vfx2;

	public String Name;
}
