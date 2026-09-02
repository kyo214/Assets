using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Util;

namespace NPOI.SS.Formula.Functions;

public class DStarRunner : Function3Arg, Function
{
	public enum DStarAlgorithmEnum
	{
		DGET = 0,
		DMIN = 1
	}

	private enum Operator
	{
		largerThan = 0,
		largerEqualThan = 1,
		smallerThan = 2,
		smallerEqualThan = 3,
		equal = 4
	}

	private DStarAlgorithmEnum algoType;

	public DStarRunner(DStarAlgorithmEnum algorithm)
	{
		algoType = algorithm;
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		if (args.Length == 3)
		{
			return Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1], args[2]);
		}
		return ErrorEval.VALUE_INVALID;
	}

	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval database, ValueEval filterColumn, ValueEval conditionDatabase)
	{
		if (!(database is AreaEval) || !(conditionDatabase is AreaEval))
		{
			return ErrorEval.VALUE_INVALID;
		}
		AreaEval areaEval = (AreaEval)database;
		AreaEval cdb = (AreaEval)conditionDatabase;
		try
		{
			filterColumn = OperandResolver.GetSingleValue(filterColumn, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		int columnForName;
		try
		{
			columnForName = GetColumnForName(filterColumn, areaEval);
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (columnForName == -1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		IDStarAlgorithm iDStarAlgorithm = null;
		iDStarAlgorithm = algoType switch
		{
			DStarAlgorithmEnum.DGET => new DGet(), 
			DStarAlgorithmEnum.DMIN => new DMin(), 
			_ => throw new InvalidOperationException("Unexpected algorithm type " + algoType.ToString() + " encountered."), 
		};
		int height = areaEval.Height;
		for (int i = 1; i < height; i++)
		{
			bool flag = true;
			try
			{
				flag = FullFillsConditions(areaEval, i, cdb);
			}
			catch (EvaluationException)
			{
				return ErrorEval.VALUE_INVALID;
			}
			if (flag)
			{
				ValueEval eval = ResolveReference(areaEval, i, columnForName);
				if (!iDStarAlgorithm.ProcessMatch(eval))
				{
					break;
				}
			}
		}
		return iDStarAlgorithm.Result;
	}

	private static int GetColumnForName(ValueEval nameValueEval, AreaEval db)
	{
		string name = OperandResolver.CoerceValueToString(nameValueEval);
		return GetColumnForString(db, name);
	}

	private static int GetColumnForString(AreaEval db, string name)
	{
		int result = -1;
		int width = db.Width;
		for (int i = 0; i < width; i++)
		{
			ValueEval valueEval = ResolveReference(db, 0, i);
			if (!(valueEval is BlankEval) && !(valueEval is ErrorEval))
			{
				string value = OperandResolver.CoerceValueToString(valueEval);
				if (name.Equals(value))
				{
					result = i;
					break;
				}
			}
		}
		return result;
	}

	private static bool FullFillsConditions(AreaEval db, int row, AreaEval cdb)
	{
		int height = cdb.Height;
		for (int i = 1; i < height; i++)
		{
			bool flag = true;
			int width = cdb.Width;
			for (int j = 0; j < width; j++)
			{
				bool flag2 = true;
				ValueEval valueEval = null;
				valueEval = ResolveReference(cdb, i, j);
				if (valueEval is BlankEval)
				{
					continue;
				}
				ValueEval valueEval2 = ResolveReference(cdb, 0, j);
				if (!(valueEval2 is StringValueEval))
				{
					throw new EvaluationException(ErrorEval.VALUE_INVALID);
				}
				if (GetColumnForName(valueEval2, db) == -1)
				{
					flag2 = false;
				}
				if (flag2)
				{
					if (!testNormalCondition(ResolveReference(db, row, GetColumnForName(valueEval2, db)), valueEval))
					{
						flag = false;
						break;
					}
					continue;
				}
				if (string.IsNullOrEmpty(OperandResolver.CoerceValueToString(valueEval)))
				{
					throw new EvaluationException(ErrorEval.VALUE_INVALID);
				}
				throw new NotImplementedException("D* function with formula conditions");
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	private static bool testNormalCondition(ValueEval value, ValueEval condition)
	{
		if (condition is StringEval)
		{
			string stringValue = ((StringEval)condition).StringValue;
			if (stringValue.StartsWith("<"))
			{
				string text = stringValue.Substring(1);
				if (text.StartsWith("="))
				{
					text = text.Substring(1);
					return testNumericCondition(value, Operator.smallerEqualThan, text);
				}
				return testNumericCondition(value, Operator.smallerThan, text);
			}
			if (stringValue.StartsWith(">"))
			{
				string text2 = stringValue.Substring(1);
				if (text2.StartsWith("="))
				{
					text2 = text2.Substring(1);
					return testNumericCondition(value, Operator.largerEqualThan, text2);
				}
				return testNumericCondition(value, Operator.largerThan, text2);
			}
			if (stringValue.StartsWith("="))
			{
				string text3 = stringValue.Substring(1);
				if (string.IsNullOrEmpty(text3))
				{
					return value is BlankEval;
				}
				bool flag = false;
				try
				{
					int.Parse(text3);
					flag = true;
				}
				catch (FormatException)
				{
					try
					{
						double.Parse(text3);
						flag = true;
					}
					catch (FormatException)
					{
						flag = false;
					}
				}
				if (flag)
				{
					return testNumericCondition(value, Operator.equal, text3);
				}
				string value2 = ((value is BlankEval) ? "" : OperandResolver.CoerceValueToString(value));
				return text3.Equals(value2);
			}
			if (string.IsNullOrEmpty(stringValue))
			{
				return value is StringEval;
			}
			return ((value is BlankEval) ? "" : OperandResolver.CoerceValueToString(value)).StartsWith(stringValue);
		}
		if (condition is NumericValueEval)
		{
			double numberValue = ((NumericValueEval)condition).NumberValue;
			double? numberFromValueEval = GetNumberFromValueEval(value);
			if (!numberFromValueEval.HasValue)
			{
				return false;
			}
			return numberValue == numberFromValueEval;
		}
		if (condition is ErrorEval)
		{
			if (value is ErrorEval)
			{
				return ((ErrorEval)condition).ErrorCode == ((ErrorEval)value).ErrorCode;
			}
			return false;
		}
		return false;
	}

	private static bool testNumericCondition(ValueEval valueEval, Operator op, string condition)
	{
		if (!(valueEval is NumericValueEval))
		{
			return false;
		}
		double numberValue = ((NumericValueEval)valueEval).NumberValue;
		double num = 0.0;
		try
		{
			num = int.Parse(condition);
		}
		catch (FormatException)
		{
			try
			{
				num = double.Parse(condition);
			}
			catch (FormatException)
			{
				throw new EvaluationException(ErrorEval.VALUE_INVALID);
			}
		}
		int num2 = NumberComparer.Compare(numberValue, num);
		return op switch
		{
			Operator.largerThan => num2 > 0, 
			Operator.largerEqualThan => num2 >= 0, 
			Operator.smallerThan => num2 < 0, 
			Operator.smallerEqualThan => num2 <= 0, 
			Operator.equal => num2 == 0, 
			_ => false, 
		};
	}

	private static double? GetNumberFromValueEval(ValueEval value)
	{
		if (value is NumericValueEval)
		{
			return ((NumericValueEval)value).NumberValue;
		}
		if (value is StringValueEval)
		{
			string stringValue = ((StringValueEval)value).StringValue;
			try
			{
				return double.Parse(stringValue);
			}
			catch (FormatException)
			{
				return null;
			}
		}
		return null;
	}

	private static ValueEval ResolveReference(AreaEval db, int dbRow, int dbCol)
	{
		try
		{
			return OperandResolver.GetSingleValue(db.GetValue(dbRow, dbCol), db.FirstRow + dbRow, db.FirstColumn + dbCol);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}
}
