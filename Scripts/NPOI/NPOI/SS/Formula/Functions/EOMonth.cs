using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class EOMonth : FreeRefFunction
{
	public static FreeRefFunction instance = new EOMonth();

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		try
		{
			double num = NumericFunction.SingleOperandEvaluate(args[0], ec.RowIndex, ec.ColumnIndex);
			int months = (int)NumericFunction.SingleOperandEvaluate(args[1], ec.RowIndex, ec.ColumnIndex);
			if (num >= 0.0 && num < 1.0)
			{
				num = 1.0;
			}
			DateTime dateTime = DateUtil.GetJavaDate(num, use1904windowing: false).AddMonths(months).AddMonths(1);
			dateTime = new DateTime(dateTime.Year, dateTime.Month, 1).AddDays(-1.0);
			return new NumberEval(DateUtil.GetExcelDate(dateTime));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}
}
