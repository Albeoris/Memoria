using System;

public class FieldMapInfo
{
    static FieldMapInfo()
    {
        FieldMapInfo.localizeAreaTitle.Load();
        FieldMapInfo.fieldmapExtraOffset = new FieldMapExtraOffset();
        FieldMapInfo.fieldmapExtraOffset.Load();
    }

    public static FieldMapLocalizeAreaTitle localizeAreaTitle = new FieldMapLocalizeAreaTitle();

    public static FieldMapExtraOffset fieldmapExtraOffset;
}
