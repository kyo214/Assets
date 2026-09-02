using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Vlookup : Var3or4ArgFunction
{
	private static ValueEval DEFAULT_ARG3 = BoolEval.TRUE;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2)
	{
		return Evaluate(srcRowIndex, srcColumnIndex, arg0, arg1, arg2, DEFAULT_ARG3);
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval lookup_value, ValueEval table_array, ValueEval col_index, ValueEval range_lookup)
	{
		try
		{
			ValueEval singleValue = OperandResolver.GetSingleValue(lookup_value, srcRowIndex, srcColumnIndex);
			TwoDEval tableArray = LookupUtils.ResolveTableArrayArg(table_array);
			int index = LookupUtils.LookupIndexOfValue(isRangeLookup: LookupUtils.ResolveRangeLookupArg(range_lookup, srcRowIndex, srcColumnIndex), lookupValue: singleValue, vector: LookupUtils.CreateColumnVector(tableArray, 0));
			int colIndex = LookupUtils.ResolveRowOrColIndexArg(col_index, srcRowIndex, srcColumnIndex);
			return CreateResultColumnVector(tableArray, colIndex).GetItem(index);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	private ValueVector CreateResultColumnVector(TwoDEval tableArray, int colIndex)
	{
		if (colIndex >= tableArray.Width)
		{
			throw EvaluationException.InvalidRef();
		}
		return LookupUtils.CreateColumnVector(tableArray, colIndex);
	}
}
