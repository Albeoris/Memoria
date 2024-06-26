using System;
using Memoria;

public class WMPad
{
	public WMPad(WMPadType type)
	{
		this.Type = type;
	}

	public Boolean kPadLLeft { get; private set; }

	public Boolean kPadLRight { get; private set; }

	public Boolean kPadLUp { get; private set; }

	public Boolean kPadLDown { get; private set; }

	public Boolean kPadL1 { get; private set; }

	public Boolean kPadL2 { get; private set; }

	public Boolean kPadRLeft { get; private set; }

	public Boolean kPadRRight { get; private set; }

	public Boolean kPadRUp { get; private set; }

	public Boolean kPadRDown { get; private set; }

	public Boolean kPadR1 { get; private set; }

	public Boolean kPadR2 { get; private set; }

	public Boolean kPadSelect { get; private set; }

	public Boolean kPadStart { get; private set; }

	public void CollectInput()
	{
		if (this.Type == WMPadType.Normal)
		{
			this.OnUpdateNormal();
		}
		else if (this.Type == WMPadType.Push)
		{
			if (FF9StateSystem.World.IsBeeScene)
			{
				this.OnUpdatePush_DebugScene();
			}
			else
			{
				this.OnUpdatePush();
			}
		}
	}

	public void PurgeInput()
	{
		this.kPadLLeft = false;
		this.kPadLRight = false;
		this.kPadLUp = false;
		this.kPadLDown = false;
		this.kPadL1 = false;
		this.kPadL2 = false;
		this.kPadRLeft = false;
		this.kPadRRight = false;
		this.kPadRUp = false;
		this.kPadRDown = false;
		this.kPadR1 = false;
		this.kPadR2 = false;
		this.kPadSelect = false;
		this.kPadStart = false;
	}

	private void OnUpdateNormal()
	{
	}

	private void OnUpdatePush()
	{
		if (FPSManager.IsDelayedInputTrigger(EventInput.PL2 | EventInput.LL2))
			this.kPadL2 = true;
		if (FPSManager.IsDelayedInputTrigger(EventInput.PR2 | EventInput.LR2))
			this.kPadR2 = true;
	}

	private void OnUpdatePush_DebugScene()
	{
		if (FPSManager.IsDelayedInputTrigger(EventInput.PL2 | EventInput.LL2))
			this.kPadL2 = true;
		if (FPSManager.IsDelayedInputTrigger(EventInput.PR2 | EventInput.LR2))
			this.kPadR2 = true;
	}

	private WMPadType Type;
}
