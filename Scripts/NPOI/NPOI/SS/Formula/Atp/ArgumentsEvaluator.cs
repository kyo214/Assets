using System.Collections.Generic;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Atp;

internal class ArgumentsEvaluator
{
	public static ArgumentsEvaluator instance = new ArgumentsEvaluator();

	private ArgumentsEvaluator()
	{
	}

	public double EvaluateDateArg(ValueEval arg, int srcCellRow, int srcCellCol)
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

	public double[] EvaluateDatesArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		if (arg == null)
		{
			return new double[0];
		}
		if (arg is StringEval)
		{
			return new double[1] { EvaluateDateArg(arg, srcCellRow, srcCellCol) };
		}
		if (arg is AreaEvalBase)
		{
			List<double> list = new List<double>();
			AreaEvalBase areaEvalBase = (AreaEvalBase)arg;
			for (int i = areaEvalBase.FirstRow; i <= areaEvalBase.LastRow; i++)
			{
				for (int j = areaEvalBase.FirstColumn; j <= areaEvalBase.LastColumn; j++)
				{
					list.Add(EvaluateDateArg(areaEvalBase.GetAbsoluteValue(i, j), i, j));
				}
			}
			double[] array = new double[list.Count];
			for (int k = 0; k < list.Count; k++)
			{
				array[k] = list[k];
			}
			return array;
		}
		return new double[1] { OperandResolver.CoerceValueToDouble(arg) };
	}

	public double EvaluateNumberArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		if (arg == null)
		{
			return 0.0;
		}
		return OperandResolver.CoerceValueToDouble(arg);
	}
}
