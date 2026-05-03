using System;

public class SoundMetaDataProfile
{
    public SoundMetaDataProfile(String name, Int32 soundIndex, String type)
    {
        this.Name = name;
        this.SoundIndex = soundIndex;
        this.Type = type;
    }

    public String Name;

    public Int32 SoundIndex;

    public String Type;
}
