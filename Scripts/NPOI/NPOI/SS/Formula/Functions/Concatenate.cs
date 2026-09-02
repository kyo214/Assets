using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Concatenate : TextFunction
{
	public override ValueEval EvaluateFunc(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int num = args.Length; i < num; i++)
		{
			stringBuilder.Append(TextFunction.EvaluateStringArg(args[i], srcCellRow, srcCellCol));
		}
		return new StringEval(stringBuilder.ToString());
	}
}
