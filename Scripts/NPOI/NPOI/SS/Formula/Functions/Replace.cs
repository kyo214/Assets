using System;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Replace : TextFunction
{
	public override ValueEval EvaluateFunc(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		if (args.Length != 4)
		{
			return ErrorEval.VALUE_INVALID;
		}
		string text = TextFunction.EvaluateStringArg(args[0], srcCellRow, srcCellCol);
		int num = TextFunction.EvaluateIntArg(args[1], srcCellRow, srcCellCol);
		int num2 = TextFunction.EvaluateIntArg(args[2], srcCellRow, srcCellCol);
		string value = TextFunction.EvaluateStringArg(args[3], srcCellRow, srcCellCol);
		if (num < 1 || num2 < 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		StringBuilder stringBuilder = new StringBuilder(text);
		if (num <= text.Length && num2 != 0)
		{
			stringBuilder.Remove(num - 1, Math.Min(num2, text.Length - num + 1));
		}
		if (num > stringBuilder.Length)
		{
			stringBuilder.Append(value);
		}
		else
		{
			stringBuilder.Insert(num - 1, value);
		}
		return new StringEval(stringBuilder.ToString());
	}
}
