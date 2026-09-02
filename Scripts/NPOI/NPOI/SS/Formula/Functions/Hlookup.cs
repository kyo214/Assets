using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Hlookup : Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		ValueEval rangeLookupArg = null;
		switch (args.Length)
		{
		case 4:
			rangeLookupArg = args[3];
			break;
		default:
			return ErrorEval.VALUE_INVALID;
		case 3:
			break;
		}
		try
		{
			ValueEval singleValue = OperandResolver.GetSingleValue(args[0], srcCellRow, srcCellCol);
			AreaEval tableArray = LookupUtils.ResolveTableArrayArg(args[1]);
			int index = LookupUtils.LookupIndexOfValue(isRangeLookup: LookupUtils.ResolveRangeLookupArg(rangeLookupArg, srcCellRow, srcCellCol), lookupValue: singleValue, vector: LookupUtils.CreateRowVector(tableArray, 0));
			int rowIndex = LookupUtils.ResolveRowOrColIndexArg(args[2], srcCellRow, srcCellCol);
			return CreateResultColumnVector(tableArray, rowIndex).GetItem(index);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	private ValueVector CreateResultColumnVector(AreaEval tableArray, int rowIndex)
	{
		if (rowIndex >= tableArray.Height)
		{
			throw EvaluationException.InvalidRef();
		}
		return LookupUtils.CreateRowVector(tableArray, rowIndex);
	}
}
