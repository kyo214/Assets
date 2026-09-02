using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class PPMT : NumericFunction
{
	protected override double Eval(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		if (args.Length < 4)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		ValueEval singleValue = OperandResolver.GetSingleValue(args[0], srcCellRow, srcCellCol);
		ValueEval singleValue2 = OperandResolver.GetSingleValue(args[1], srcCellRow, srcCellCol);
		ValueEval singleValue3 = OperandResolver.GetSingleValue(args[2], srcCellRow, srcCellCol);
		ValueEval singleValue4 = OperandResolver.GetSingleValue(args[3], srcCellRow, srcCellCol);
		double r = OperandResolver.CoerceValueToDouble(singleValue);
		int per = OperandResolver.CoerceValueToInt(singleValue2);
		int nper = OperandResolver.CoerceValueToInt(singleValue3);
		double pv = OperandResolver.CoerceValueToDouble(singleValue4);
		double result = Finance.PPMT(r, per, nper, pv);
		NumericFunction.CheckValue(result);
		return result;
	}
}
