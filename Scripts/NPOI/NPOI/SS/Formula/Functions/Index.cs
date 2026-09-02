using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Index : Function2Arg, Function, Function3Arg, Function4Arg
{
	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		TwoDEval twoDEval = ConvertFirstArg(arg0);
		int pColumnIx = 0;
		try
		{
			int num = ResolveIndexArg(arg1, srcRowIndex, srcColumnIndex);
			if (!twoDEval.IsColumn)
			{
				if (!twoDEval.IsRow)
				{
					return ErrorEval.REF_INVALID;
				}
				pColumnIx = num;
				num = 0;
			}
			return GetValueFromArea(twoDEval, num, pColumnIx);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2)
	{
		TwoDEval ae = ConvertFirstArg(arg0);
		try
		{
			int pColumnIx = ResolveIndexArg(arg2, srcRowIndex, srcColumnIndex);
			int pRowIx = ResolveIndexArg(arg1, srcRowIndex, srcColumnIndex);
			return GetValueFromArea(ae, pRowIx, pColumnIx);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2, ValueEval arg3)
	{
		throw new Exception("Incomplete code - don't know how to support the 'area_num' parameter yet)");
	}

	private static TwoDEval ConvertFirstArg(ValueEval arg0)
	{
		if (arg0 is RefEval)
		{
			return ((RefEval)arg0).Offset(0, 0, 0, 0);
		}
		if (arg0 is TwoDEval)
		{
			return (TwoDEval)arg0;
		}
		throw new Exception("Incomplete code - cannot handle first arg of type (" + arg0.GetType().Name + ")");
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		return args.Length switch
		{
			2 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1]), 
			3 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1], args[2]), 
			4 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1], args[2], args[3]), 
			_ => ErrorEval.VALUE_INVALID, 
		};
	}

	private static ValueEval GetValueFromArea(TwoDEval ae, int pRowIx, int pColumnIx)
	{
		TwoDEval twoDEval = ae;
		if (pRowIx != 0)
		{
			if (pRowIx > ae.Height)
			{
				throw new EvaluationException(ErrorEval.REF_INVALID);
			}
			twoDEval = twoDEval.GetRow(pRowIx - 1);
		}
		if (pColumnIx != 0)
		{
			if (pColumnIx > ae.Width)
			{
				throw new EvaluationException(ErrorEval.REF_INVALID);
			}
			twoDEval = twoDEval.GetColumn(pColumnIx - 1);
		}
		return twoDEval;
	}

	[Obsolete]
	private static ValueEval GetValueFromArea(AreaEval ae, int pRowIx, int pColumnIx, bool colArgWasPassed, int srcRowIx, int srcColIx)
	{
		bool flag = pRowIx == 0;
		bool flag2 = pColumnIx == 0;
		int num;
		int num2;
		if (ae.IsRow)
		{
			if (ae.IsColumn)
			{
				num = ((!flag) ? (pRowIx - 1) : 0);
				num2 = ((!flag2) ? (pColumnIx - 1) : 0);
			}
			else if (colArgWasPassed)
			{
				num = ((!flag) ? (pRowIx - 1) : 0);
				num2 = pColumnIx - 1;
			}
			else
			{
				num = 0;
				num2 = pRowIx - 1;
				flag2 = flag;
			}
		}
		else if (ae.IsColumn)
		{
			num = ((!flag) ? (pRowIx - 1) : (srcRowIx - ae.FirstRow));
			num2 = ((!flag2) ? ((!flag2) ? (pColumnIx - 1) : 0) : 0);
		}
		else
		{
			if (!colArgWasPassed)
			{
				throw new EvaluationException((pRowIx < 0) ? ErrorEval.VALUE_INVALID : ErrorEval.REF_INVALID);
			}
			num = ((!flag) ? (pRowIx - 1) : (srcRowIx - ae.FirstRow));
			num2 = ((!flag2) ? (pColumnIx - 1) : (srcColIx - ae.FirstColumn));
		}
		int width = ae.Width;
		int height = ae.Height;
		if ((!flag && num >= height) || (!flag2 && num2 >= width))
		{
			throw new EvaluationException(ErrorEval.REF_INVALID);
		}
		if (num < 0 || num2 < 0 || num >= height || num2 >= width)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		return ae.GetRelativeValue(num, num2);
	}

	private static int ResolveIndexArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		ValueEval singleValue = OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol);
		if (singleValue == MissingArgEval.instance)
		{
			return 0;
		}
		if (singleValue == BlankEval.instance)
		{
			return 0;
		}
		int num = OperandResolver.CoerceValueToInt(singleValue);
		if (num < 0)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		return num;
	}
}
