using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Countblank : Fixed1ArgFunction
{
	private class BlankPredicate : IMatchPredicate
	{
		public bool Matches(ValueEval valueEval)
		{
			if (valueEval != BlankEval.instance)
			{
				if (valueEval is StringEval)
				{
					return "".Equals(((StringEval)valueEval).StringValue);
				}
				return false;
			}
			return true;
		}
	}

	private static IMatchPredicate predicate = new BlankPredicate();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		double value;
		if (arg0 is RefEval)
		{
			value = CountUtils.CountMatchingCellsInRef((RefEval)arg0, predicate);
		}
		else
		{
			if (!(arg0 is ThreeDEval))
			{
				throw new ArgumentException("Bad range arg type (" + arg0.GetType().Name + ")");
			}
			value = CountUtils.CountMatchingCellsInArea((ThreeDEval)arg0, predicate);
		}
		return new NumberEval(value);
	}
}
