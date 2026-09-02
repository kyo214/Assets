using System;
using System.Globalization;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class WeekNum : Fixed2ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new WeekNum();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval serialNumVE, ValueEval returnTypeVE)
	{
		double num = 0.0;
		try
		{
			num = NumericFunction.SingleOperandEvaluate(serialNumVE, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		DateTime javaDate = DateUtil.GetJavaDate(num, use1904windowing: false);
		int num2 = 0;
		try
		{
			num2 = OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(returnTypeVE, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException)
		{
			return ErrorEval.NUM_ERROR;
		}
		if (num2 != 1 && num2 != 2)
		{
			return ErrorEval.NUM_ERROR;
		}
		return new NumberEval(getWeekNo(javaDate, num2));
	}

	public int getWeekNo(DateTime dt, int weekStartOn)
	{
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		if (weekStartOn == 1)
		{
			return gregorianCalendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
		}
		return gregorianCalendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length == 2)
		{
			return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0], args[1]);
		}
		return ErrorEval.VALUE_INVALID;
	}
}
