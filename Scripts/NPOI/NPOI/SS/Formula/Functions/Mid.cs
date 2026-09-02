using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Mid : TextFunction
{
	public override ValueEval EvaluateFunc(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		if (args.Length != 3)
		{
			return ErrorEval.VALUE_INVALID;
		}
		string text = TextFunction.EvaluateStringArg(args[0], srcCellRow, srcCellCol);
		int num = TextFunction.EvaluateIntArg(args[1], srcCellRow, srcCellCol);
		int num2 = TextFunction.EvaluateIntArg(args[2], srcCellRow, srcCellCol);
		int num3 = num - 1;
		if (num3 < 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num2 < 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		int length = text.Length;
		if (num2 < 0 || num3 > length)
		{
			return new StringEval("");
		}
		int num4 = Math.Min(num3 + num2, length);
		return new StringEval(text.Substring(num3, num4 - num3));
	}
}
