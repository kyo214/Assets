using System.Collections.Generic;

namespace NPOI.SS.UserModel;

public class RangeType
{
	public static RangeType NUMBER = new RangeType(1, "num");

	public static RangeType MIN = new RangeType(2, "min");

	public static RangeType MAX = new RangeType(3, "max");

	public static RangeType PERCENT = new RangeType(4, "percent");

	public static RangeType PERCENTILE = new RangeType(5, "percentile");

	public static RangeType UNALLOCATED = new RangeType(6, null);

	public static RangeType FORMULA = new RangeType(7, "formula");

	public static RangeType AUTOMIN = new RangeType(8, "autoMin");

	public static RangeType AUTOMAX = new RangeType(9, "autoMax");

	public int id;

	public string name;

	private static List<RangeType> values = new List<RangeType> { NUMBER, MIN, MAX, PERCENT, PERCENTILE, UNALLOCATED, FORMULA, AUTOMIN, AUTOMAX };

	public static List<RangeType> Values()
	{
		return values;
	}

	public override string ToString()
	{
		return id + " - " + name;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is RangeType))
		{
			return false;
		}
		RangeType rangeType = obj as RangeType;
		if (id == rangeType.id)
		{
			return name == rangeType.name;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return id.GetHashCode() ^ name.GetHashCode();
	}

	public static RangeType ById(int id)
	{
		return Values()[id - 1];
	}

	public static RangeType ByName(string name)
	{
		foreach (RangeType item in Values())
		{
			if (item.name.Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	private RangeType(int id, string name)
	{
		this.id = id;
		this.name = name;
	}
}
