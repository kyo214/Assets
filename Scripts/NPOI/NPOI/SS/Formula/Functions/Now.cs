using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class Now : Fixed0ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex)
	{
		return new NumberEval(DateUtil.GetExcelDate(DateTime.Now));
	}
}
