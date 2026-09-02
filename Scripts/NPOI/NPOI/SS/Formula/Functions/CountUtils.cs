using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

internal class CountUtils
{
	private CountUtils()
	{
	}

	public static int CountMatchingCellsInRef(RefEval refEval, IMatchPredicate criteriaPredicate)
	{
		int num = 0;
		int firstSheetIndex = refEval.FirstSheetIndex;
		int lastSheetIndex = refEval.LastSheetIndex;
		for (int i = firstSheetIndex; i <= lastSheetIndex; i++)
		{
			ValueEval innerValueEval = refEval.GetInnerValueEval(i);
			if (criteriaPredicate.Matches(innerValueEval))
			{
				num++;
			}
		}
		return num;
	}

	public static int CountArg(ValueEval eval, IMatchPredicate criteriaPredicate)
	{
		if (eval == null)
		{
			throw new ArgumentException("eval must not be null");
		}
		if (eval is ThreeDEval)
		{
			return CountMatchingCellsInArea((ThreeDEval)eval, criteriaPredicate);
		}
		if (eval is TwoDEval)
		{
			throw new ArgumentException("Count requires 3D Evals, 2D ones aren't supported");
		}
		if (eval is RefEval)
		{
			return CountMatchingCellsInRef((RefEval)eval, criteriaPredicate);
		}
		if (!criteriaPredicate.Matches(eval))
		{
			return 0;
		}
		return 1;
	}

	public static int CountMatchingCellsInArea(ThreeDEval areaEval, IMatchPredicate criteriaPredicate)
	{
		int num = 0;
		int firstSheetIndex = areaEval.FirstSheetIndex;
		int lastSheetIndex = areaEval.LastSheetIndex;
		for (int i = firstSheetIndex; i <= lastSheetIndex; i++)
		{
			int height = areaEval.Height;
			int width = areaEval.Width;
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					ValueEval value = areaEval.GetValue(i, j, k);
					if ((!(criteriaPredicate is I_MatchAreaPredicate) || ((I_MatchAreaPredicate)criteriaPredicate).Matches(areaEval, j, k)) && criteriaPredicate.Matches(value))
					{
						num++;
					}
				}
			}
		}
		return num;
	}
}
