using System;

public class BubbleInputInfo
{
	public BubbleInputInfo(PosObj characterControl, Obj targetCollider, UInt32 buttonCode)
	{
		this.characterControl = characterControl;
		this.targetCollider = targetCollider;
		this.buttonCode = buttonCode;
	}

	public PosObj characterControl;

	public Obj targetCollider;

	public UInt32 buttonCode;
}
