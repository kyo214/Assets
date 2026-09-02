using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class TextFunction : Function
{
	protected static string EMPTY_STRING = "";

	public static readonly Function LEN = new Len();

	public static readonly Function LOWER = new Lower();

	public static readonly Function UPPER = new Upper();

	public static readonly Function TRIM = new Trim();

	public static readonly Function MID = new Mid();

	public static readonly Function LEFT = new LeftRight(isLeft: true);

	public static readonly Function RIGHT = new LeftRight(isLeft: false);

	public static readonly Function CONCATENATE = new Concatenate();

	public static readonly Function EXACT = new Exact();

	public static readonly Function TEXT = new Text();

	public static readonly Function FIND = new SearchFind(isCaseSensitive: true);

	public static readonly Function SEARCH = new SearchFind(isCaseSensitive: false);

	public static readonly Function CLEAN = new Clean();

	public static readonly Function CHAR = new CHAR();

	public static readonly Function PROPER = new Proper();

	public static string EvaluateStringArg(ValueEval eval, int srcRow, int srcCol)
	{
		return OperandResolver.CoerceValueToString(OperandResolver.GetSingleValue(eval, srcRow, srcCol));
	}

	public static int EvaluateIntArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		return OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol));
	}

	public static double EvaluateDoubleArg(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		return OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol));
	}

	public ValueEval Evaluate(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		try
		{
			return EvaluateFunc(args, srcCellRow, srcCellCol);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	internal static bool IsPrintable(char c)
	{
		return c >= ' ';
	}

	public abstract ValueEval EvaluateFunc(ValueEval[] args, int srcCellRow, int srcCellCol);
}
