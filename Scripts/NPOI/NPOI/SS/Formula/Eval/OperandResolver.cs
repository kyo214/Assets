using System;
using System.Globalization;

namespace NPOI.SS.Formula.Eval;

public class OperandResolver
{
	private const string Digits = "\\d+";

	private const string Exp = "[eE][+-]?\\d+";

	private const string fpRegex = "[\\x00-\\x20]*[+-]?((((\\d+(\\.)?(\\d+?)([eE][+-]?\\d+)?)|(\\.(\\d+)([eE][+-]?\\d+)?))))[\\x00-\\x20]*";

	private OperandResolver()
	{
	}

	public static ValueEval GetSingleValue(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		ValueEval valueEval = ((arg is RefEval) ? ChooseSingleElementFromRef((RefEval)arg) : ((!(arg is AreaEval)) ? arg : ChooseSingleElementFromArea((AreaEval)arg, srcCellRow, srcCellCol)));
		if (valueEval is ErrorEval)
		{
			throw new EvaluationException((ErrorEval)valueEval);
		}
		return valueEval;
	}

	public static ValueEval ChooseSingleElementFromArea(AreaEval ae, int srcCellRow, int srcCellCol)
	{
		ValueEval valueEval = ChooseSingleElementFromAreaInternal(ae, srcCellRow, srcCellCol);
		if (valueEval is ErrorEval)
		{
			throw new EvaluationException((ErrorEval)valueEval);
		}
		return valueEval;
	}

	private static ValueEval ChooseSingleElementFromAreaInternal(AreaEval ae, int srcCellRow, int srcCellCol)
	{
		if (ae.IsColumn)
		{
			if (ae.IsRow)
			{
				return ae.GetRelativeValue(0, 0);
			}
			if (!ae.ContainsRow(srcCellRow))
			{
				throw EvaluationException.InvalidValue();
			}
			return ae.GetAbsoluteValue(srcCellRow, ae.FirstColumn);
		}
		if (!ae.IsRow)
		{
			if (ae.ContainsRow(srcCellRow) && ae.ContainsColumn(srcCellCol))
			{
				return ae.GetAbsoluteValue(ae.FirstRow, ae.FirstColumn);
			}
			throw EvaluationException.InvalidValue();
		}
		if (!ae.ContainsColumn(srcCellCol))
		{
			throw EvaluationException.InvalidValue();
		}
		return ae.GetAbsoluteValue(ae.FirstRow, srcCellCol);
	}

	private static ValueEval ChooseSingleElementFromRef(RefEval ref1)
	{
		return ref1.GetInnerValueEval(ref1.FirstSheetIndex);
	}

	public static int CoerceValueToInt(ValueEval ev)
	{
		if (ev == BlankEval.instance)
		{
			return 0;
		}
		return (int)Math.Floor(CoerceValueToDouble(ev));
	}

	public static double CoerceValueToDouble(ValueEval ev)
	{
		if (ev == BlankEval.instance)
		{
			return 0.0;
		}
		if (ev is NumericValueEval)
		{
			return ((NumericValueEval)ev).NumberValue;
		}
		if (ev is StringEval)
		{
			double num = ParseDouble(((StringEval)ev).StringValue);
			if (double.IsNaN(num))
			{
				throw EvaluationException.InvalidValue();
			}
			return num;
		}
		throw new Exception("Unexpected arg eval type (" + ev.GetType().Name + ")");
	}

	public static double ParseDouble(string pText)
	{
		try
		{
			double num = double.Parse(pText, CultureInfo.CurrentCulture);
			if (double.IsInfinity(num))
			{
				return double.NaN;
			}
			return num;
		}
		catch (Exception)
		{
			return double.NaN;
		}
	}

	public static string CoerceValueToString(ValueEval ve)
	{
		if (ve is StringValueEval)
		{
			return ((StringValueEval)ve).StringValue;
		}
		if (ve is BlankEval)
		{
			return "";
		}
		throw new ArgumentException("Unexpected eval class (" + ve.GetType().Name + ")");
	}

	public static bool? CoerceValueToBoolean(ValueEval ve, bool stringsAreBlanks)
	{
		if (ve == null || ve == BlankEval.instance)
		{
			return null;
		}
		if (ve is BoolEval)
		{
			return ((BoolEval)ve).BooleanValue;
		}
		if (ve is StringEval)
		{
			if (stringsAreBlanks)
			{
				return null;
			}
			string stringValue = ((StringEval)ve).StringValue;
			if (stringValue.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (stringValue.Equals("false", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		if (ve is NumericValueEval)
		{
			double numberValue = ((NumericValueEval)ve).NumberValue;
			if (double.IsNaN(numberValue))
			{
				throw new EvaluationException(ErrorEval.VALUE_INVALID);
			}
			return numberValue != 0.0;
		}
		if (ve is ErrorEval)
		{
			throw new EvaluationException((ErrorEval)ve);
		}
		throw new InvalidOperationException("Unexpected eval (" + ve.GetType().Name + ")");
	}
}
