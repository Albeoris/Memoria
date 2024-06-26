using System;

public class SoundProfile
{
	public SoundProfile()
	{
		this.Code = String.Empty;
		this.Name = String.Empty;
		this.ResourceID = String.Empty;
		this.AkbBin = IntPtr.Zero;
		this.BankID = 0;
		this.SoundIndex = 0;
		this.SoundID = 0;
		this.SoundVolume = 1f;
		this.Panning = 0f;
		this.Pitch = 0f;
		this.SoundProfileState = SoundProfileState.Idle;
		this.SoundProfileType = SoundProfileType.Default;
		this.StartPlayTime = 0f;
    }

	public String Code;

	public String Name;

	public String ResourceID;

	public IntPtr AkbBin;

	public Int32 BankID;

	public Int32 SoundIndex;

	public Int32 SoundID;

	public Single SoundVolume;

	public Single Panning;

	public Single Pitch;

	public SoundProfileState SoundProfileState;

	public SoundProfileType SoundProfileType;

    public Single StartPlayTime;
}
