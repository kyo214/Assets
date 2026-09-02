using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class AverageIfs : FreeRefFunction
{
	public static FreeRefFunction instance = new AverageIfs();

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
			Sumifs.ValidateCriteria(array2);
			return new NumberEval(GetAvgFromMatchingCells(array, array2, areaEval));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	private void ValidateCriteriaRanges(AreaEval[] criteriaRanges, AreaEval avgRange)
	{
		foreach (AreaEval areaEval in criteriaRanges)
		{
			if (areaEval.Height != avgRange.Height || areaEval.Width != avgRange.Width)
			{
				throw EvaluationException.InvalidValue();
			}
		}
	}

	private static double GetAvgFromMatchingCells(AreaEval[] ranges, IMatchPredicate[] predicates, AreaEval aeAvg)
	{
		int height = aeAvg.Height;
		int width = aeAvg.Width;
		double num = 0.0;
		int num2 = 0;
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
					double? num3 = Accumulate(aeAvg, i, j);
					if (num3.HasValue)
					{
						num += num3.Value;
						num2++;
					}
				}
			}
		}
		if (num2 <= 0)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		return num / (double)num2;
	}

	private static double? Accumulate(AreaEval aeSum, int relRowIndex, int relColIndex)
	{
		ValueEval relativeValue = aeSum.GetRelativeValue(relRowIndex, relColIndex);
		if (relativeValue is NumberEval)
		{
			return ((NumberEval)relativeValue).NumberValue;
		}
		return null;
	}

	private static AreaEval ConvertRangeArg(ValueEval eval)
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
