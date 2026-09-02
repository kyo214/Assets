using System;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Atp;

public class WorkdayCalculator
{
	public static WorkdayCalculator instance = new WorkdayCalculator();

	private WorkdayCalculator()
	{
	}

	public int CalculateWorkdays(double start, double end, double[] holidays)
	{
		int num = PastDaysOfWeek(start, end, DayOfWeek.Saturday);
		int num2 = PastDaysOfWeek(start, end, DayOfWeek.Sunday);
		int num3 = CalculateNonWeekendHolidays(start, end, holidays);
		return (int)(end - start + 1.0) - num - num2 - num3;
	}

	public DateTime CalculateWorkdays(double start, int workdays, double[] holidays)
	{
		DateTime javaDate = DateUtil.GetJavaDate(start);
		int num = ((workdays >= 0) ? 1 : (-1));
		DateTime dateTime = javaDate;
		double num2 = DateUtil.GetExcelDate(dateTime);
		while (workdays != 0)
		{
			dateTime = dateTime.AddDays(num);
			num2 += (double)num;
			if (dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(num2, holidays))
			{
				workdays -= num;
			}
		}
		return dateTime;
	}

	public int PastDaysOfWeek(double start, double end, DayOfWeek dayOfWeek)
	{
		int num = 0;
		int i = (int)Math.Floor((start < end) ? start : end);
		for (int num2 = (int)Math.Floor((end > start) ? end : start); i <= num2; i++)
		{
			if (DateUtil.GetJavaDate(i).DayOfWeek == dayOfWeek)
			{
				num++;
			}
		}
		if (!(start < end))
		{
			return -num;
		}
		return num;
	}

	private int CalculateNonWeekendHolidays(double start, double end, double[] holidays)
	{
		int num = 0;
		double start2 = ((start < end) ? start : end);
		double end2 = ((end > start) ? end : start);
		for (int i = 0; i < holidays.Length; i++)
		{
			if (IsInARange(start2, end2, holidays[i]) && !IsWeekend(holidays[i]))
			{
				num++;
			}
		}
		if (!(start < end))
		{
			return -num;
		}
		return num;
	}

	private bool IsWeekend(double aDate)
	{
		DateTime javaDate = DateUtil.GetJavaDate(aDate);
		if (javaDate.DayOfWeek != DayOfWeek.Saturday)
		{
			return javaDate.DayOfWeek == DayOfWeek.Sunday;
		}
		return true;
	}

	private bool IsHoliday(double aDate, double[] holidays)
	{
		for (int i = 0; i < holidays.Length; i++)
		{
			if (Math.Round(holidays[i]) == Math.Round(aDate))
			{
				return true;
			}
		}
		return false;
	}

	private int IsNonWorkday(double aDate, double[] holidays)
	{
		if (!IsWeekend(aDate) && !IsHoliday(aDate, holidays))
		{
			return 0;
		}
		return 1;
	}

	private bool IsInARange(double start, double end, double aDate)
	{
		if (aDate >= start)
		{
			return aDate <= end;
		}
		return false;
	}
}
