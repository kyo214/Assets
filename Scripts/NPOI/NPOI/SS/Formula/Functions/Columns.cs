using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Columns : Fixed1ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		int num;
		if (arg0 is AreaEval)
		{
			num = ((AreaEval)arg0).Width;
		}
		else
		{
			if (!(arg0 is RefEval))
			{
				return ErrorEval.VALUE_INVALID;
			}
			num = 1;
		}
		return new NumberEval(num);
	}
}
