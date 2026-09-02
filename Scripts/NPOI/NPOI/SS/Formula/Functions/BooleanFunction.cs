using System;
using System.Globalization;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class BooleanFunction : Function
{
	protected abstract bool InitialResultValue { get; }

	protected abstract bool PartialEvaluate(bool cumulativeResult, bool currentValue);

	private bool Calculate(ValueEval[] args)
	{
		bool flag = InitialResultValue;
		bool flag2 = false;
		foreach (ValueEval valueEval in args)
		{
			if (valueEval is TwoDEval)
			{
				TwoDEval twoDEval = (TwoDEval)valueEval;
				int height = twoDEval.Height;
				int width = twoDEval.Width;
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < width; k++)
					{
						bool? flag3 = OperandResolver.CoerceValueToBoolean(twoDEval.GetValue(j, k), stringsAreBlanks: true);
						if (flag3.HasValue)
						{
							flag = PartialEvaluate(flag, Convert.ToBoolean(flag3, CultureInfo.InvariantCulture));
							flag2 = true;
						}
					}
				}
			}
			else if (valueEval is RefEval)
			{
				RefEval refEval = (RefEval)valueEval;
				int firstSheetIndex = refEval.FirstSheetIndex;
				int lastSheetIndex = refEval.LastSheetIndex;
				for (int l = firstSheetIndex; l <= lastSheetIndex; l++)
				{
					bool? flag3 = OperandResolver.CoerceValueToBoolean(refEval.GetInnerValueEval(l), stringsAreBlanks: true);
					if (flag3.HasValue)
					{
						flag = PartialEvaluate(flag, flag3.Value);
						flag2 = true;
					}
				}
			}
			else
			{
				bool? flag3 = ((valueEval != MissingArgEval.instance) ? OperandResolver.CoerceValueToBoolean(valueEval, stringsAreBlanks: false) : ((bool?)null));
				if (flag3.HasValue)
				{
					flag = PartialEvaluate(flag, Convert.ToBoolean(flag3, CultureInfo.InvariantCulture));
					flag2 = true;
				}
			}
		}
		if (!flag2)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		return flag;
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRow, int srcCol)
	{
		if (args.Length < 1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		bool b;
		try
		{
			b = Calculate(args);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return BoolEval.ValueOf(b);
	}
}
