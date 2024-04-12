public abstract class ISoundLoader
{
	public abstract void Initial();

	public abstract void Load(SoundProfile profile, ISoundLoader.ResultCallback callback, SoundDatabase soundDatabase);

	public delegate void ResultCallback(SoundProfile profile, SoundDatabase soundDatabase);
}
