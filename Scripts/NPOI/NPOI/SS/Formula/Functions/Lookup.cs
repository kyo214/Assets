using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Lookup : Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		switch (args.Length)
		{
		case 2:
			throw new Exception("Two arg version of LOOKUP not supported yet");
		default:
			return ErrorEval.VALUE_INVALID;
		case 3:
			try
			{
				ValueEval singleValue = OperandResolver.GetSingleValue(args[0], srcCellRow, srcCellCol);
				AreaEval ae = LookupUtils.ResolveTableArrayArg(args[1]);
				AreaEval ae2 = LookupUtils.ResolveTableArrayArg(args[2]);
				ValueVector valueVector = CreateVector(ae);
				ValueVector valueVector2 = CreateVector(ae2);
				if (valueVector.Size > valueVector2.Size)
				{
					throw new Exception("Lookup vector and result vector of differing sizes not supported yet");
				}
				int index = LookupUtils.LookupIndexOfValue(singleValue, valueVector, isRangeLookup: true);
				return valueVector2.GetItem(index);
			}
			catch (EvaluationException ex)
			{
				return ex.GetErrorEval();
			}
		}
	}

	private static ValueVector CreateVector(AreaEval ae)
	{
		ValueVector valueVector = LookupUtils.CreateVector(ae);
		if (valueVector != null)
		{
			return valueVector;
		}
		throw new InvalidOperationException("non-vector lookup or result areas not supported yet");
	}
}
