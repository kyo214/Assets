using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class Days360 : Var2or3ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		double value;
		try
		{
			double d = NumericFunction.SingleOperandEvaluate(arg0, srcRowIndex, srcColumnIndex);
			double d2 = NumericFunction.SingleOperandEvaluate(arg1, srcRowIndex, srcColumnIndex);
			value = Evaluate(d, d2, method: false);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return new NumberEval(value);
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2)
	{
		double value;
		try
		{
			double d = NumericFunction.SingleOperandEvaluate(arg0, srcRowIndex, srcColumnIndex);
			double d2 = NumericFunction.SingleOperandEvaluate(arg1, srcRowIndex, srcColumnIndex);
			bool? flag = OperandResolver.CoerceValueToBoolean(OperandResolver.GetSingleValue(arg2, srcRowIndex, srcColumnIndex), stringsAreBlanks: false);
			value = Evaluate(d, d2, flag.HasValue && flag.Value);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return new NumberEval(value);
	}

	private double Evaluate(double d0, double d1, bool method)
	{
		DateTime date = GetDate(d0);
		DateTime date2 = GetDate(d1);
		int[] startingDate = GetStartingDate(date, method);
		int[] endingDate = GetEndingDate(date2, date, method);
		return endingDate[0] * 360 + endingDate[1] * 30 + endingDate[2] - (startingDate[0] * 360 + startingDate[1] * 30 + startingDate[2]);
	}

	private DateTime GetDate(double date)
	{
		return DateUtil.GetJavaDate(date);
	}

	private int[] GetStartingDate(DateTime realStart, bool method)
	{
		DateTime date = realStart;
		int num = Math.Min(30, date.Day);
		if (!method && IsLastDayOfMonth(date))
		{
			num = 30;
		}
		return new int[3] { date.Year, date.Month, num };
	}

	private static int[] GetEndingDate(DateTime realEnd, DateTime realStart, bool method)
	{
		DateTime dateTime = realEnd;
		int year = dateTime.Year;
		int month = dateTime.Month;
		int num = Math.Min(30, dateTime.Day);
		if (!method && realEnd.Day == 31)
		{
			if (realStart.Day < 30)
			{
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);
				year = dateTime.Year;
				month = dateTime.Month;
				num = 1;
			}
			else
			{
				num = 30;
			}
		}
		return new int[3] { year, month, num };
	}

	private DateTime GetEndingDateAccordingToStartingDate(double date, DateTime startingDate, bool method)
	{
		DateTime dateTime = DateUtil.GetJavaDate(date, use1904windowing: false);
		if (IsLastDayOfMonth(dateTime) && startingDate.Day < 30)
		{
			dateTime = GetFirstDayOfNextMonth(dateTime);
		}
		return dateTime;
	}

	private bool IsLastDayOfMonth(DateTime date)
	{
		return date.AddDays(1.0).Month != date.Month;
	}

	private DateTime GetFirstDayOfNextMonth(DateTime date)
	{
		return (date.Month >= 12) ? new DateTime(date.Year + 1, 1, 1, date.Hour, date.Minute, date.Second) : new DateTime(date.Year, date.Month + 1, 1, date.Hour, date.Minute, date.Second);
	}
}
