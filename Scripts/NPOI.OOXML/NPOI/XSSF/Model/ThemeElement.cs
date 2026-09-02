using System.Collections.Generic;

namespace NPOI.XSSF.Model;

public class ThemeElement
{
	private static SortedDictionary<int, ThemeElement> values = new SortedDictionary<int, ThemeElement>();

	public static ThemeElement LT1 = new ThemeElement(0, "Lt1");

	public static ThemeElement DK1 = new ThemeElement(1, "Dk1");

	public static ThemeElement LT2 = new ThemeElement(2, "Lt2");

	public static ThemeElement DK2 = new ThemeElement(3, "Dk2");

	public static ThemeElement ACCENT1 = new ThemeElement(4, "Accent1");

	public static ThemeElement ACCENT2 = new ThemeElement(5, "Accent2");

	public static ThemeElement ACCENT3 = new ThemeElement(6, "Accent3");

	public static ThemeElement ACCENT4 = new ThemeElement(7, "Accent4");

	public static ThemeElement ACCENT5 = new ThemeElement(8, "Accent5");

	public static ThemeElement ACCENT6 = new ThemeElement(9, "Accent6");

	public static ThemeElement HLINK = new ThemeElement(10, "Hlink");

	public static ThemeElement FOLHLINK = new ThemeElement(11, "FolHlink");

	public static ThemeElement UNKNOWN = new ThemeElement(-1, null);

	public int idx;

	public string name;

	public static ThemeElement ById(int idx)
	{
		if (idx >= values.Count || idx < 0)
		{
			return UNKNOWN;
		}
		return values[idx];
	}

	private ThemeElement(int idx, string name)
	{
		this.idx = idx;
		this.name = name;
		values.Add(idx, this);
	}
}
