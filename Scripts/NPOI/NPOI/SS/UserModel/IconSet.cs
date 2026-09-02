using System.Collections.Generic;

namespace NPOI.SS.UserModel;

public class IconSet
{
	public static IconSet GYR_3_ARROW = new IconSet(0, 3, "3Arrows");

	public static IconSet GREY_3_ARROWS = new IconSet(1, 3, "3ArrowsGray");

	public static IconSet GYR_3_FLAGS = new IconSet(2, 3, "3Flags");

	public static IconSet GYR_3_TRAFFIC_LIGHTS = new IconSet(3, 3, "3TrafficLights1");

	public static IconSet GYR_3_TRAFFIC_LIGHTS_BOX = new IconSet(4, 3, "3TrafficLights2");

	public static IconSet GYR_3_SHAPES = new IconSet(5, 3, "3Signs");

	public static IconSet GYR_3_SYMBOLS_CIRCLE = new IconSet(6, 3, "3Symbols");

	public static IconSet GYR_3_SYMBOLS = new IconSet(7, 3, "3Symbols2");

	public static IconSet GYR_4_ARROWS = new IconSet(8, 4, "4Arrows");

	public static IconSet GREY_4_ARROWS = new IconSet(9, 4, "4ArrowsGray");

	public static IconSet RB_4_TRAFFIC_LIGHTS = new IconSet(10, 4, "4RedToBlack");

	public static IconSet RATINGS_4 = new IconSet(11, 4, "4Rating");

	public static IconSet GYRB_4_TRAFFIC_LIGHTS = new IconSet(12, 4, "4TrafficLights");

	public static IconSet GYYYR_5_ARROWS = new IconSet(13, 5, "5Arrows");

	public static IconSet GREY_5_ARROWS = new IconSet(14, 5, "5ArrowsGray");

	public static IconSet RATINGS_5 = new IconSet(15, 5, "5Rating");

	public static IconSet QUARTERS_5 = new IconSet(16, 5, "5Quarters");

	protected static IconSet DEFAULT_ICONSET = GYR_3_TRAFFIC_LIGHTS;

	public int id;

	public int num;

	public string name;

	private static List<IconSet> values = new List<IconSet>
	{
		GYR_3_ARROW, GREY_3_ARROWS, GYR_3_FLAGS, GYR_3_TRAFFIC_LIGHTS, GYR_3_TRAFFIC_LIGHTS_BOX, GYR_3_SHAPES, GYR_3_SYMBOLS_CIRCLE, GYR_3_SYMBOLS, GYR_4_ARROWS, GREY_4_ARROWS,
		RB_4_TRAFFIC_LIGHTS, RATINGS_4, GYRB_4_TRAFFIC_LIGHTS, GYYYR_5_ARROWS, GREY_5_ARROWS, RATINGS_5, QUARTERS_5
	};

	public static List<IconSet> Values()
	{
		return values;
	}

	public override string ToString()
	{
		return id + " - " + name;
	}

	public static IconSet ById(int id)
	{
		return Values()[id];
	}

	public static IconSet ByName(string name)
	{
		foreach (IconSet item in Values())
		{
			if (item.name.Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	public static IconSet ByOOXMLName(string name)
	{
		if (name.StartsWith("Item"))
		{
			name = name.Remove(0, 4);
		}
		return ByName(name);
	}

	private IconSet(int id, int num, string name)
	{
		this.id = id;
		this.num = num;
		this.name = name;
	}
}
