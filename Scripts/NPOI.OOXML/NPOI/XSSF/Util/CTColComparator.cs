using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.Util;

public class CTColComparator : Comparer<CT_Col>
{
	public class CTColComparatorByMinMax : CTColComparator
	{
		public override int Compare(CT_Col col1, CT_Col col2)
		{
			long num = col1.min;
			long num2 = col2.min;
			if (num >= num2)
			{
				if (num <= num2)
				{
					return BY_MAX.Compare(col1, col2);
				}
				return 1;
			}
			return -1;
		}
	}

	public class CTColComparatorByMax : CTColComparator
	{
		public override int Compare(CT_Col col1, CT_Col col2)
		{
			long num = col1.max;
			long num2 = col2.max;
			if (num >= num2)
			{
				if (num <= num2)
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}
	}

	public static IComparer<CT_Col> BY_MAX = new CTColComparatorByMax();

	public static IComparer<CT_Col> BY_MIN_MAX = new CTColComparatorByMinMax();

	public override int Compare(CT_Col o1, CT_Col o2)
	{
		if (o1.min < o2.min)
		{
			return -1;
		}
		if (o1.min > o2.min)
		{
			return 1;
		}
		if (o1.max < o2.max)
		{
			return -1;
		}
		if (o1.max > o2.max)
		{
			return 1;
		}
		return 0;
	}
}
