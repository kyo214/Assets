namespace NPOI.DDF;

public class SysIndexSource
{
	public static SysIndexSource FILL_COLOR = new SysIndexSource(240);

	public static SysIndexSource LINE_OR_FILL_COLOR = new SysIndexSource(241);

	public static SysIndexSource LINE_COLOR = new SysIndexSource(242);

	public static SysIndexSource SHADOW_COLOR = new SysIndexSource(243);

	public static SysIndexSource CURRENT_OR_LAST_COLOR = new SysIndexSource(244);

	public static SysIndexSource FILL_BACKGROUND_COLOR = new SysIndexSource(245);

	public static SysIndexSource LINE_BACKGROUND_COLOR = new SysIndexSource(246);

	public static SysIndexSource FILL_OR_LINE_COLOR = new SysIndexSource(247);

	internal int value;

	internal static SysIndexSource[] Values()
	{
		return new SysIndexSource[7] { FILL_COLOR, LINE_OR_FILL_COLOR, SHADOW_COLOR, CURRENT_OR_LAST_COLOR, FILL_BACKGROUND_COLOR, LINE_BACKGROUND_COLOR, FILL_OR_LINE_COLOR };
	}

	internal SysIndexSource(int value)
	{
		this.value = value;
	}
}
