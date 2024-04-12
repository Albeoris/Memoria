public class FieldMapInfo
{
	static FieldMapInfo()
	{
		FieldMapInfo.localizeAreaTitle.Load();
		FieldMapInfo.fieldmapExtraOffset = new FieldMapExtraOffset();
		FieldMapInfo.fieldmapExtraOffset.Load();
		FieldMapInfo.fieldmapSPSExtraOffset = new FieldMapSPSExtraOffset();
		FieldMapInfo.fieldmapSPSExtraOffset.Load();
	}

	public static FieldMapLocalizeAreaTitle localizeAreaTitle = new FieldMapLocalizeAreaTitle();

	public static FieldMapExtraOffset fieldmapExtraOffset;

	public static FieldMapSPSExtraOffset fieldmapSPSExtraOffset;
}
