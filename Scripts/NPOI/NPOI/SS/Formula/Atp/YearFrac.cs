using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Atp;

internal class YearFrac : FreeRefFunction
{
	public static FreeRefFunction instance = new YearFrac();

	private YearFrac()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		int rowIndex = ec.RowIndex;
		int columnIndex = ec.ColumnIndex;
		double value;
		try
		{
			int basis = 0;
			switch (args.Length)
			{
			case 3:
				basis = EvaluateIntArg(args[2], rowIndex, columnIndex);
				break;
			default:
				return ErrorEval.VALUE_INVALID;
			case 2:
				break;
			}
			double pStartDateVal = EvaluateDateArg(args[0], rowIndex, columnIndex);
			double pEndDateVal = EvaluateDateArg(args[1], rowIndex, columnIndex);
			value = YearFracCalculator.Calculate(pStartDateVal, pEndDateVal, basis);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return new NumberEval(value);
	}

	private static double EvaluateDateArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		ValueEval singleValue = OperandResolver.GetSingleValue(arg, srcCellRow, (short)srcCellCol);
		if (singleValue is StringEval)
		{
			string stringValue = ((StringEval)singleValue).StringValue;
			double num = OperandResolver.ParseDouble(stringValue);
			if (!double.IsNaN(num))
			{
				return num;
			}
			return DateUtil.GetExcelDate(DateParser.ParseDate(stringValue), use1904windowing: false);
		}
		return OperandResolver.CoerceValueToDouble(singleValue);
	}

	private static int EvaluateIntArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		return OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol));
	}
}
