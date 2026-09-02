using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class EDate : FreeRefFunction
{
	public static FreeRefFunction Instance = new EDate();

	internal EDate()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		try
		{
			double value = GetValue(args[0]);
			int months = (int)GetValue(args[1]);
			double excelDate = DateUtil.GetExcelDate(DateUtil.GetJavaDate(value).AddMonths(months));
			NumericFunction.CheckValue(excelDate);
			return new NumberEval(excelDate);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	private double GetValue(ValueEval arg)
	{
		if (arg is NumberEval)
		{
			return ((NumberEval)arg).NumberValue;
		}
		if (arg is BlankEval)
		{
			return 0.0;
		}
		if (arg is RefEval)
		{
			RefEval obj = (RefEval)arg;
			if (obj.NumberOfSheets > 1)
			{
				throw new EvaluationException(ErrorEval.VALUE_INVALID);
			}
			ValueEval innerValueEval = obj.GetInnerValueEval(obj.FirstSheetIndex);
			if (innerValueEval is NumberEval)
			{
				return ((NumberEval)innerValueEval).NumberValue;
			}
			if (innerValueEval is BlankEval)
			{
				return 0.0;
			}
		}
		throw new EvaluationException(ErrorEval.VALUE_INVALID);
	}
}
