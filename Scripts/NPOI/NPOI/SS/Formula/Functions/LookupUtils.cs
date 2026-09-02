using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

internal class LookupUtils
{
	internal class RowVector : ValueVector
	{
		private AreaEval _tableArray;

		private int _size;

		private int _rowIndex;

		public int Size => _size;

		public RowVector(AreaEval tableArray, int rowIndex)
		{
			_rowIndex = rowIndex;
			int row = tableArray.FirstRow + rowIndex;
			if (!tableArray.ContainsRow(row))
			{
				int num = tableArray.LastRow - tableArray.FirstRow;
				throw new ArgumentException("Specified row index (" + rowIndex + ") is outside the allowed range (0.." + num + ")");
			}
			_tableArray = tableArray;
			_size = tableArray.Width;
		}

		public ValueEval GetItem(int index)
		{
			if (index > _size)
			{
				throw new IndexOutOfRangeException("Specified index (" + index + ") is outside the allowed range (0.." + (_size - 1) + ")");
			}
			return _tableArray.GetRelativeValue(_rowIndex, index);
		}
	}

	internal class ColumnVector : ValueVector
	{
		private AreaEval _tableArray;

		private int _size;

		private int _columnIndex;

		public int Size => _size;

		public ColumnVector(AreaEval tableArray, int columnIndex)
		{
			_columnIndex = columnIndex;
			int num = tableArray.FirstColumn + columnIndex;
			if (!tableArray.ContainsColumn((short)num))
			{
				int num2 = tableArray.LastColumn - tableArray.FirstColumn;
				throw new ArgumentException("Specified column index (" + columnIndex + ") is outside the allowed range (0.." + num2 + ")");
			}
			_tableArray = tableArray;
			_size = _tableArray.Height;
		}

		public ValueEval GetItem(int index)
		{
			if (index > _size)
			{
				throw new IndexOutOfRangeException("Specified index (" + index + ") is outside the allowed range (0.." + (_size - 1) + ")");
			}
			return _tableArray.GetRelativeValue(index, _columnIndex);
		}
	}

	private class SheetVector : ValueVector
	{
		private RefEval _re;

		private int _size;

		public int Size => _size;

		public SheetVector(RefEval re)
		{
			_size = re.NumberOfSheets;
			_re = re;
		}

		public ValueEval GetItem(int index)
		{
			if (index >= _size)
			{
				throw new IndexOutOfRangeException("Specified index (" + index + ") is outside the allowed range (0.." + (_size - 1) + ")");
			}
			int sheetIndex = _re.FirstSheetIndex + index;
			return _re.GetInnerValueEval(sheetIndex);
		}
	}

	private class StringLookupComparer : LookupValueComparerBase
	{
		private string _value;

		private Regex _wildCardPattern;

		private bool _matchExact;

		private bool _isMatchFunction;

		public StringLookupComparer(StringEval se, bool matchExact, bool isMatchFunction)
			: base(se)
		{
			_value = se.StringValue;
			_wildCardPattern = Countif.StringMatcher.GetWildCardPattern(_value);
			_matchExact = matchExact;
			_isMatchFunction = isMatchFunction;
		}

		protected override CompareResult CompareSameType(ValueEval other)
		{
			string stringValue = ((StringEval)other).StringValue;
			if (_wildCardPattern != null)
			{
				bool matches = _wildCardPattern.Matches(stringValue).Count > 0;
				if (_isMatchFunction || !_matchExact)
				{
					return CompareResult.ValueOf(matches);
				}
			}
			return CompareResult.ValueOf(string.Compare(_value, stringValue, ignoreCase: true));
		}

		protected override string GetValueAsString()
		{
			return _value;
		}
	}

	private class NumberLookupComparer : LookupValueComparerBase
	{
		private double _value;

		public NumberLookupComparer(NumberEval ne)
			: base(ne)
		{
			_value = ne.NumberValue;
		}

		protected override CompareResult CompareSameType(ValueEval other)
		{
			NumberEval numberEval = (NumberEval)other;
			return CompareResult.ValueOf(_value.CompareTo(numberEval.NumberValue));
		}

		protected override string GetValueAsString()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
	}

	public static ValueVector CreateRowVector(TwoDEval tableArray, int relativeRowIndex)
	{
		return new RowVector((AreaEval)tableArray, relativeRowIndex);
	}

	public static ValueVector CreateColumnVector(TwoDEval tableArray, int relativeColumnIndex)
	{
		return new ColumnVector((AreaEval)tableArray, relativeColumnIndex);
	}

	public static ValueVector CreateVector(TwoDEval ae)
	{
		if (ae.IsColumn)
		{
			return CreateColumnVector(ae, 0);
		}
		if (ae.IsRow)
		{
			return CreateRowVector(ae, 0);
		}
		return null;
	}

	public static ValueVector CreateVector(RefEval re)
	{
		return new SheetVector(re);
	}

	public static int ResolveRowOrColIndexArg(ValueEval rowColIndexArg, int srcCellRow, int srcCellCol)
	{
		if (rowColIndexArg == null)
		{
			throw new ArgumentException("argument must not be null");
		}
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(rowColIndexArg, srcCellRow, (short)srcCellCol);
		}
		catch (EvaluationException)
		{
			throw EvaluationException.InvalidRef();
		}
		if (singleValue is StringEval && double.IsNaN(OperandResolver.ParseDouble(((StringEval)singleValue).StringValue)))
		{
			throw EvaluationException.InvalidRef();
		}
		int num = OperandResolver.CoerceValueToInt(singleValue);
		if (num < 1)
		{
			throw EvaluationException.InvalidValue();
		}
		return num - 1;
	}

	public static AreaEval ResolveTableArrayArg(ValueEval eval)
	{
		if (eval is AreaEval)
		{
			return (AreaEval)eval;
		}
		if (eval is RefEval)
		{
			return ((RefEval)eval).Offset(0, 0, 0, 0);
		}
		throw EvaluationException.InvalidValue();
	}

	public static bool ResolveRangeLookupArg(ValueEval rangeLookupArg, int srcCellRow, int srcCellCol)
	{
		if (rangeLookupArg == null)
		{
			return true;
		}
		ValueEval singleValue = OperandResolver.GetSingleValue(rangeLookupArg, srcCellRow, srcCellCol);
		if (singleValue is BlankEval)
		{
			return false;
		}
		if (singleValue is BoolEval)
		{
			return ((BoolEval)singleValue).BooleanValue;
		}
		if (singleValue is StringEval)
		{
			string stringValue = ((StringEval)singleValue).StringValue;
			if (stringValue.Length < 1)
			{
				throw EvaluationException.InvalidValue();
			}
			bool? flag = Countif.ParseBoolean(stringValue);
			if (flag.HasValue)
			{
				if (flag != true)
				{
					return false;
				}
				return true;
			}
			throw EvaluationException.InvalidValue();
		}
		if (singleValue is NumericValueEval)
		{
			NumericValueEval numericValueEval = (NumericValueEval)singleValue;
			return 0.0 != numericValueEval.NumberValue;
		}
		throw new Exception("Unexpected eval type (" + singleValue.GetType().Name + ")");
	}

	public static int LookupIndexOfValue(ValueEval lookupValue, ValueVector vector, bool isRangeLookup)
	{
		LookupValueComparer lookupComparer = CreateLookupComparer(lookupValue, isRangeLookup, isMatchFunction: false);
		int num = ((!isRangeLookup) ? LookupIndexOfExactValue(lookupComparer, vector) : PerformBinarySearch(vector, lookupComparer));
		if (num < 0)
		{
			throw new EvaluationException(ErrorEval.NA);
		}
		return num;
	}

	private static int LookupIndexOfExactValue(LookupValueComparer lookupComparer, ValueVector vector)
	{
		int size = vector.Size;
		for (int i = 0; i < size; i++)
		{
			if (lookupComparer.CompareTo(vector.GetItem(i)).IsEqual)
			{
				return i;
			}
		}
		return -1;
	}

	private static int PerformBinarySearch(ValueVector vector, LookupValueComparer lookupComparer)
	{
		BinarySearchIndexes binarySearchIndexes = new BinarySearchIndexes(vector.Size);
		int num;
		while (true)
		{
			num = binarySearchIndexes.GetMidIx();
			if (num < 0)
			{
				return binarySearchIndexes.GetLowIx();
			}
			CompareResult compareResult = lookupComparer.CompareTo(vector.GetItem(num));
			if (compareResult.IsTypeMismatch)
			{
				int num2 = HandleMidValueTypeMismatch(lookupComparer, vector, binarySearchIndexes, num);
				if (num2 < 0)
				{
					continue;
				}
				num = num2;
				compareResult = lookupComparer.CompareTo(vector.GetItem(num));
			}
			if (compareResult.IsEqual)
			{
				break;
			}
			binarySearchIndexes.NarrowSearch(num, compareResult.IsLessThan);
		}
		return FindLastIndexInRunOfEqualValues(lookupComparer, vector, num, binarySearchIndexes.GetHighIx());
	}

	private static int HandleMidValueTypeMismatch(LookupValueComparer lookupComparer, ValueVector vector, BinarySearchIndexes bsi, int midIx)
	{
		int num = midIx;
		int highIx = bsi.GetHighIx();
		CompareResult compareResult;
		do
		{
			num++;
			if (num == highIx)
			{
				bsi.NarrowSearch(midIx, isLessThan: true);
				return -1;
			}
			compareResult = lookupComparer.CompareTo(vector.GetItem(num));
			if (compareResult.IsLessThan && num == highIx - 1)
			{
				bsi.NarrowSearch(midIx, isLessThan: true);
				return -1;
			}
		}
		while (compareResult.IsTypeMismatch);
		if (compareResult.IsEqual)
		{
			return num;
		}
		bsi.NarrowSearch(num, compareResult.IsLessThan);
		return -1;
	}

	private static int FindLastIndexInRunOfEqualValues(LookupValueComparer lookupComparer, ValueVector vector, int firstFoundIndex, int maxIx)
	{
		for (int i = firstFoundIndex + 1; i < maxIx; i++)
		{
			if (!lookupComparer.CompareTo(vector.GetItem(i)).IsEqual)
			{
				return i - 1;
			}
		}
		return maxIx - 1;
	}

	public static LookupValueComparer CreateLookupComparer(ValueEval lookupValue, bool matchExact, bool isMatchFunction)
	{
		if (lookupValue == BlankEval.instance)
		{
			return new NumberLookupComparer(NumberEval.ZERO);
		}
		if (lookupValue is StringEval)
		{
			return new StringLookupComparer((StringEval)lookupValue, matchExact, isMatchFunction);
		}
		if (lookupValue is NumberEval)
		{
			return new NumberLookupComparer((NumberEval)lookupValue);
		}
		if (lookupValue is BoolEval)
		{
			return new BooleanLookupComparer((BoolEval)lookupValue);
		}
		throw new ArgumentException("Bad lookup value type (" + lookupValue.GetType().Name + ")");
	}
}
