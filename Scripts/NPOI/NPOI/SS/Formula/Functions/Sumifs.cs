using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Sumifs : FreeRefFunction
{
	public static FreeRefFunction instance = new Sumifs();

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length < 3 || args.Length % 2 == 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		try
		{
			AreaEval areaEval = ConvertRangeArg(args[0]);
			AreaEval[] array = new AreaEval[(args.Length - 1) / 2];
			IMatchPredicate[] array2 = new IMatchPredicate[array.Length];
			int num = 1;
			int num2 = 0;
			while (num < args.Length)
			{
				array[num2] = ConvertRangeArg(args[num]);
				array2[num2] = Countif.CreateCriteriaPredicate(args[num + 1], ec.RowIndex, ec.ColumnIndex);
				num += 2;
				num2++;
			}
			ValidateCriteriaRanges(array, areaEval);
			ValidateCriteria(array2);
			return new NumberEval(CalcMatchingCells(array, array2, areaEval, 0.0, (double init, double? current) => init + (current.HasValue ? current.Value : 0.0)));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	internal static void ValidateCriteria(IMatchPredicate[] criteria)
	{
		foreach (IMatchPredicate matchPredicate in criteria)
		{
			if (matchPredicate is Countif.ErrorMatcher)
			{
				throw new EvaluationException(ErrorEval.ValueOf(((Countif.ErrorMatcher)matchPredicate).Value));
			}
		}
	}

	internal static void ValidateCriteriaRanges(AreaEval[] criteriaRanges, AreaEval sumRange)
	{
		foreach (AreaEval areaEval in criteriaRanges)
		{
			if (areaEval.Height != sumRange.Height || areaEval.Width != sumRange.Width)
			{
				throw EvaluationException.InvalidValue();
			}
		}
	}

	internal static double CalcMatchingCells(AreaEval[] ranges, IMatchPredicate[] predicates, AreaEval aeSum, double initialValue, Func<double, double?, double> calc)
	{
		int height = aeSum.Height;
		int width = aeSum.Width;
		double num = initialValue;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				bool flag = true;
				for (int k = 0; k < ranges.Length; k++)
				{
					AreaEval areaEval = ranges[k];
					if (!predicates[k].Matches(areaEval.GetRelativeValue(i, j)))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num = calc(num, ReadValue(aeSum, i, j));
				}
			}
		}
		return num;
	}

	private static double? ReadValue(AreaEval aeSum, int relRowIndex, int relColIndex)
	{
		ValueEval relativeValue = aeSum.GetRelativeValue(relRowIndex, relColIndex);
		if (relativeValue is NumberEval)
		{
			return ((NumberEval)relativeValue).NumberValue;
		}
		return null;
	}

	internal static AreaEval ConvertRangeArg(ValueEval eval)
	{
		if (eval is AreaEval)
		{
			return (AreaEval)eval;
		}
		if (eval is RefEval)
		{
			return ((RefEval)eval).Offset(0, 0, 0, 0);
		}
		throw new EvaluationException(ErrorEval.VALUE_INVALID);
	}
}
