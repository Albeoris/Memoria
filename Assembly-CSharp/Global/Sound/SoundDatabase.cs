using System;
using System.Collections.Generic;

public class SoundDatabase
{
	public SoundDatabase()
	{
		this.soundIndexDictionary = new Dictionary<Int32, SoundProfile>();
	}

	public SoundProfile Create(SoundProfile profile)
	{
		if (this.soundIndexDictionary.ContainsKey(profile.SoundIndex))
		{
			return (SoundProfile)null;
		}
		this.soundIndexDictionary.Add(profile.SoundIndex, profile);
		return profile;
	}

	public SoundProfile Read(Int32 soundIndex)
	{
		if (this.soundIndexDictionary.ContainsKey(soundIndex))
		{
			return this.soundIndexDictionary[soundIndex];
		}
		return (SoundProfile)null;
	}

	public Dictionary<Int32, SoundProfile> ReadAll()
	{
		return this.soundIndexDictionary;
	}

	public SoundProfile Update(SoundProfile profile)
	{
		if (this.soundIndexDictionary.ContainsKey(profile.SoundIndex))
		{
			this.soundIndexDictionary[profile.SoundIndex] = profile;
			return profile;
		}
		return (SoundProfile)null;
	}

	public void Delete(SoundProfile profile)
	{
		if (this.soundIndexDictionary.ContainsKey(profile.SoundIndex))
		{
			this.soundIndexDictionary.Remove(profile.SoundIndex);
		}
	}

	public void DeleteAll()
	{
		this.soundIndexDictionary.Clear();
	}

	private Dictionary<Int32, SoundProfile> soundIndexDictionary;
}
