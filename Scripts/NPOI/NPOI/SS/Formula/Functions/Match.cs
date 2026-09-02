using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Match : Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		double num = 1.0;
		switch (args.Length)
		{
		case 3:
			try
			{
				num = EvaluateMatchTypeArg(args[2], srcCellRow, srcCellCol);
			}
			catch (EvaluationException)
			{
				return ErrorEval.REF_INVALID;
			}
			break;
		default:
			return ErrorEval.VALUE_INVALID;
		case 2:
			break;
		}
		bool matchExact = num == 0.0;
		bool findLargestLessThanOrEqual = num > 0.0;
		try
		{
			ValueEval singleValue = OperandResolver.GetSingleValue(args[0], srcCellRow, srcCellCol);
			ValueVector lookupRange = EvaluateLookupRange(args[1]);
			return new NumberEval(FindIndexOfValue(singleValue, lookupRange, matchExact, findLargestLessThanOrEqual) + 1);
		}
		catch (EvaluationException ex2)
		{
			return ex2.GetErrorEval();
		}
	}

	private static ValueVector EvaluateLookupRange(ValueEval eval)
	{
		if (eval is RefEval)
		{
			RefEval refEval = (RefEval)eval;
			if (refEval.NumberOfSheets == 1)
			{
				return new SingleValueVector(refEval.GetInnerValueEval(refEval.FirstSheetIndex));
			}
			return LookupUtils.CreateVector(refEval);
		}
		if (eval is TwoDEval)
		{
			return LookupUtils.CreateVector((TwoDEval)eval) ?? throw new EvaluationException(ErrorEval.NA);
		}
		if (eval is NumericValueEval)
		{
			throw new EvaluationException(ErrorEval.NA);
		}
		if (eval is StringEval)
		{
			if (double.IsNaN(OperandResolver.ParseDouble(((StringEval)eval).StringValue)))
			{
				throw new EvaluationException(ErrorEval.VALUE_INVALID);
			}
			throw new EvaluationException(ErrorEval.NA);
		}
		throw new Exception("Unexpected eval type (" + eval.GetType().Name + ")");
	}

	private static double EvaluateMatchTypeArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		ValueEval singleValue = OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol);
		if (singleValue is ErrorEval)
		{
			throw new EvaluationException((ErrorEval)singleValue);
		}
		if (singleValue is NumericValueEval)
		{
			return ((NumericValueEval)singleValue).NumberValue;
		}
		if (singleValue is StringEval)
		{
			double num = OperandResolver.ParseDouble(((StringEval)singleValue).StringValue);
			if (double.IsNaN(num))
			{
				throw new EvaluationException(ErrorEval.VALUE_INVALID);
			}
			return num;
		}
		if (singleValue is MissingArgEval)
		{
			return 1.0;
		}
		throw new Exception("Unexpected match_type type (" + singleValue.GetType().Name + ")");
	}

	private static int FindIndexOfValue(ValueEval lookupValue, ValueVector lookupRange, bool matchExact, bool FindLargestLessThanOrEqual)
	{
		LookupValueComparer lookupValueComparer = CreateLookupComparer(lookupValue, matchExact);
		int size = lookupRange.Size;
		if (matchExact)
		{
			for (int i = 0; i < size; i++)
			{
				if (lookupValueComparer.CompareTo(lookupRange.GetItem(i)).IsEqual)
				{
					return i;
				}
			}
			throw new EvaluationException(ErrorEval.NA);
		}
		if (FindLargestLessThanOrEqual)
		{
			if (lookupValue is NumericValueEval numericValueEval)
			{
				Dictionary<int, double> dictionary = new Dictionary<int, double>();
				for (int j = 0; j < size; j++)
				{
					NumericValueEval numericValueEval2 = lookupRange.GetItem(j) as NumericValueEval;
					if (lookupValueComparer.CompareTo(numericValueEval2).IsEqual)
					{
						return j;
					}
					dictionary.Add(j, numericValueEval2.NumberValue - numericValueEval.NumberValue);
				}
				return (from kv in dictionary
					where kv.Value < 0.0
					orderby kv.Value descending
					select kv).First().Key;
			}
			for (int num = size - 1; num >= 0; num--)
			{
				CompareResult compareResult = lookupValueComparer.CompareTo(lookupRange.GetItem(num));
				if (!compareResult.IsTypeMismatch && !compareResult.IsLessThan)
				{
					return num;
				}
			}
			throw new EvaluationException(ErrorEval.NA);
		}
		for (int num2 = 0; num2 < size; num2++)
		{
			CompareResult compareResult2 = lookupValueComparer.CompareTo(lookupRange.GetItem(num2));
			if (compareResult2.IsEqual)
			{
				return num2;
			}
			if (compareResult2.IsGreaterThan)
			{
				if (num2 < 1)
				{
					throw new EvaluationException(ErrorEval.NA);
				}
				return num2 - 1;
			}
		}
		return size - 1;
	}

	private static LookupValueComparer CreateLookupComparer(ValueEval lookupValue, bool matchExact)
	{
		return LookupUtils.CreateLookupComparer(lookupValue, matchExact, isMatchFunction: true);
	}
}
