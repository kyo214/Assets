using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Atp;

public class YearFracCalculator
{
	private class SimpleDate
	{
		public const int JANUARY = 1;

		public const int FEBRUARY = 2;

		public int year;

		public int month;

		public int day;

		public long ticks;

		public SimpleDate(DateTime date)
		{
			year = date.Year;
			month = date.Month;
			day = date.Day;
			ticks = date.Ticks;
		}
	}

	private const int MS_PER_HOUR = 3600000;

	private const int MS_PER_DAY = 86400000;

	private const int DAYS_PER_NORMAL_YEAR = 365;

	private const int DAYS_PER_LEAP_YEAR = 366;

	private const int LONG_MONTH_LEN = 31;

	private const int SHORT_MONTH_LEN = 30;

	private const int SHORT_FEB_LEN = 28;

	private const int LONG_FEB_LEN = 29;

	public static double Calculate(double pStartDateVal, double pEndDateVal, int basis)
	{
		if (basis < 0 || basis >= 5)
		{
			throw new EvaluationException(ErrorEval.NUM_ERROR);
		}
		int num = (int)Math.Floor(pStartDateVal);
		int num2 = (int)Math.Floor(pEndDateVal);
		if (num == num2)
		{
			return 0.0;
		}
		if (num > num2)
		{
			int num3 = num;
			num = num2;
			num2 = num3;
		}
		return basis switch
		{
			0 => Basis0(num, num2), 
			1 => Basis1(num, num2), 
			2 => Basis2(num, num2), 
			3 => Basis3(num, num2), 
			4 => Basis4(num, num2), 
			_ => throw new InvalidOperationException("cannot happen"), 
		};
	}

	public static double Basis0(int startDateVal, int endDateVal)
	{
		SimpleDate simpleDate = CreateDate(startDateVal);
		SimpleDate simpleDate2 = CreateDate(endDateVal);
		int num = simpleDate.day;
		int num2 = simpleDate2.day;
		if (num == 31 && num2 == 31)
		{
			num = 30;
			num2 = 30;
		}
		else if (num == 31)
		{
			num = 30;
		}
		else if (num == 30 && num2 == 31)
		{
			num2 = 30;
		}
		else if (simpleDate.month == 2 && IsLastDayOfMonth(simpleDate))
		{
			num = 30;
			if (simpleDate2.month == 2 && IsLastDayOfMonth(simpleDate2))
			{
				num2 = 30;
			}
		}
		return CalculateAdjusted(simpleDate, simpleDate2, num, num2);
	}

	public static double Basis1(int startDateVal, int endDateVal)
	{
		SimpleDate simpleDate = CreateDate(startDateVal);
		SimpleDate simpleDate2 = CreateDate(endDateVal);
		double num = (IsGreaterThanOneYear(simpleDate, simpleDate2) ? AverageYearLength(simpleDate.year, simpleDate2.year) : ((!ShouldCountFeb29(simpleDate, simpleDate2)) ? 365.0 : 366.0));
		return DateDiff(simpleDate.ticks, simpleDate2.ticks) / num;
	}

	public static double Basis2(int startDateVal, int endDateVal)
	{
		return (double)(endDateVal - startDateVal) / 360.0;
	}

	public static double Basis3(double startDateVal, double endDateVal)
	{
		return (endDateVal - startDateVal) / 365.0;
	}

	public static double Basis4(int startDateVal, int endDateVal)
	{
		SimpleDate simpleDate = CreateDate(startDateVal);
		SimpleDate simpleDate2 = CreateDate(endDateVal);
		int num = simpleDate.day;
		int num2 = simpleDate2.day;
		if (num == 31)
		{
			num = 30;
		}
		if (num2 == 31)
		{
			num2 = 30;
		}
		return CalculateAdjusted(simpleDate, simpleDate2, num, num2);
	}

	private static double CalculateAdjusted(SimpleDate startDate, SimpleDate endDate, int date1day, int date2day)
	{
		return (double)((endDate.year - startDate.year) * 360 + (endDate.month - startDate.month) * 30 + (date2day - date1day)) / 360.0;
	}

	private static bool IsLastDayOfMonth(SimpleDate date)
	{
		if (date.day < 28)
		{
			return false;
		}
		return date.day == GetLastDayOfMonth(date);
	}

	private static int GetLastDayOfMonth(SimpleDate date)
	{
		switch (date.month)
		{
		case 1:
		case 3:
		case 5:
		case 7:
		case 8:
		case 10:
		case 12:
			return 31;
		case 4:
		case 6:
		case 9:
		case 11:
			return 30;
		default:
			if (IsLeapYear(date.year))
			{
				return 29;
			}
			return 28;
		}
	}

	private static bool ShouldCountFeb29(SimpleDate start, SimpleDate end)
	{
		bool flag = IsLeapYear(start.year);
		if (flag && start.year == end.year)
		{
			return true;
		}
		bool flag2 = IsLeapYear(end.year);
		if (!flag && !flag2)
		{
			return false;
		}
		if (flag)
		{
			int month = start.month;
			if ((uint)(month - 1) <= 1u)
			{
				return true;
			}
			return false;
		}
		if (flag2)
		{
			return end.month switch
			{
				1 => false, 
				2 => end.day == 29, 
				_ => true, 
			};
		}
		return false;
	}

	private static double DateDiff(long startDateTicks, long endDateTicks)
	{
		return new TimeSpan(endDateTicks - startDateTicks).TotalDays;
	}

	private static double AverageYearLength(int startYear, int endYear)
	{
		int num = 0;
		for (int i = startYear; i <= endYear; i++)
		{
			num += 365;
			if (IsLeapYear(i))
			{
				num++;
			}
		}
		double num2 = endYear - startYear + 1;
		return (double)num / num2;
	}

	private static bool IsLeapYear(int i)
	{
		if (i % 4 != 0)
		{
			return false;
		}
		if (i % 400 == 0)
		{
			return true;
		}
		if (i % 100 == 0)
		{
			return false;
		}
		return true;
	}

	private static bool IsGreaterThanOneYear(SimpleDate start, SimpleDate end)
	{
		if (start.year == end.year)
		{
			return false;
		}
		if (start.year + 1 != end.year)
		{
			return true;
		}
		if (start.month > end.month)
		{
			return false;
		}
		if (start.month < end.month)
		{
			return true;
		}
		return start.day < end.day;
	}

	private static SimpleDate CreateDate(int dayCount)
	{
		return new SimpleDate(DateUtil.SetCalendar(dayCount, 0, use1904windowing: false, roundSeconds: false));
	}
}
