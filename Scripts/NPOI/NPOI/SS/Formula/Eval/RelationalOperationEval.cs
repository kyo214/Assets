using System;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;

namespace NPOI.SS.Formula.Eval;

public abstract class RelationalOperationEval : Fixed2ArgFunction
{
	public static NPOI.SS.Formula.Functions.Function EqualEval = new EqualEval();

	public static NPOI.SS.Formula.Functions.Function NotEqualEval = new NotEqualEval();

	public static NPOI.SS.Formula.Functions.Function LessEqualEval = new LessEqualEval();

	public static NPOI.SS.Formula.Functions.Function LessThanEval = new LessThanEval();

	public static NPOI.SS.Formula.Functions.Function GreaterEqualEval = new GreaterEqualEval();

	public static NPOI.SS.Formula.Functions.Function GreaterThanEval = new GreaterThanEval();

	private static int DoCompare(ValueEval va, ValueEval vb)
	{
		if (va == BlankEval.instance)
		{
			return CompareBlank(vb);
		}
		if (vb == BlankEval.instance)
		{
			return -CompareBlank(va);
		}
		if (va is BoolEval)
		{
			if (vb is BoolEval)
			{
				BoolEval boolEval = (BoolEval)va;
				BoolEval boolEval2 = (BoolEval)vb;
				if (boolEval.BooleanValue == boolEval2.BooleanValue)
				{
					return 0;
				}
				if (!boolEval.BooleanValue)
				{
					return -1;
				}
				return 1;
			}
			return 1;
		}
		if (vb is BoolEval)
		{
			return -1;
		}
		if (va is StringEval)
		{
			if (vb is StringEval)
			{
				StringEval obj = (StringEval)va;
				return string.Compare(strB: ((StringEval)vb).StringValue, strA: obj.StringValue, comparisonType: StringComparison.OrdinalIgnoreCase);
			}
			return 1;
		}
		if (vb is StringEval)
		{
			return -1;
		}
		if (va is NumberEval && vb is NumberEval)
		{
			NumberEval numberEval = (NumberEval)va;
			NumberEval numberEval2 = (NumberEval)vb;
			if (numberEval.NumberValue == numberEval2.NumberValue)
			{
				return 0;
			}
			return NumberComparer.Compare(numberEval.NumberValue, numberEval2.NumberValue);
		}
		throw new ArgumentException("Bad operand types (" + va.GetType().Name + "), (" + vb.GetType().Name + ")");
	}

	private static int CompareBlank(ValueEval v)
	{
		if (v == BlankEval.instance)
		{
			return 0;
		}
		if (v is BoolEval)
		{
			if (!((BoolEval)v).BooleanValue)
			{
				return 0;
			}
			return -1;
		}
		if (v is NumberEval)
		{
			NumberEval numberEval = (NumberEval)v;
			return NumberComparer.Compare(0.0, numberEval.NumberValue);
		}
		if (v is StringEval)
		{
			if (((StringEval)v).StringValue.Length >= 1)
			{
				return -1;
			}
			return 0;
		}
		throw new ArgumentException("bad value class (" + v.GetType().Name + ")");
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		ValueEval singleValue;
		ValueEval singleValue2;
		try
		{
			singleValue = OperandResolver.GetSingleValue(arg0, srcRowIndex, srcColumnIndex);
			singleValue2 = OperandResolver.GetSingleValue(arg1, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		int cmpResult = DoCompare(singleValue, singleValue2);
		return BoolEval.ValueOf(ConvertComparisonResult(cmpResult));
	}

	public abstract bool ConvertComparisonResult(int cmpResult);
}
