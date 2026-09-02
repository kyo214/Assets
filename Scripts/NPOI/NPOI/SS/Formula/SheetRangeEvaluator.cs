using System;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public class SheetRangeEvaluator : ISheetRange
{
	private int _firstSheetIndex;

	private int _lastSheetIndex;

	private SheetRefEvaluator[] _sheetEvaluators;

	public int FirstSheetIndex => _firstSheetIndex;

	public int LastSheetIndex => _lastSheetIndex;

	public string SheetNameRange
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetSheetName(_firstSheetIndex));
			if (_firstSheetIndex != _lastSheetIndex)
			{
				stringBuilder.Append(':');
				stringBuilder.Append(GetSheetName(_lastSheetIndex));
			}
			return stringBuilder.ToString();
		}
	}

	public SheetRangeEvaluator(int firstSheetIndex, int lastSheetIndex, SheetRefEvaluator[] sheetEvaluators)
	{
		if (firstSheetIndex < 0)
		{
			throw new ArgumentException("Invalid firstSheetIndex: " + firstSheetIndex + ".");
		}
		if (lastSheetIndex < firstSheetIndex)
		{
			throw new ArgumentException("Invalid lastSheetIndex: " + lastSheetIndex + " for firstSheetIndex: " + firstSheetIndex + ".");
		}
		_firstSheetIndex = firstSheetIndex;
		_lastSheetIndex = lastSheetIndex;
		_sheetEvaluators = (SheetRefEvaluator[])sheetEvaluators.Clone();
	}

	public SheetRangeEvaluator(int onlySheetIndex, SheetRefEvaluator sheetEvaluator)
		: this(onlySheetIndex, onlySheetIndex, new SheetRefEvaluator[1] { sheetEvaluator })
	{
	}

	public SheetRefEvaluator GetSheetEvaluator(int sheetIndex)
	{
		if (sheetIndex < _firstSheetIndex || sheetIndex > _lastSheetIndex)
		{
			throw new ArgumentException("Invalid SheetIndex: " + sheetIndex + " - Outside range " + _firstSheetIndex + " : " + _lastSheetIndex);
		}
		return _sheetEvaluators[sheetIndex - _firstSheetIndex];
	}

	public string GetSheetName(int sheetIndex)
	{
		return GetSheetEvaluator(sheetIndex).SheetName;
	}

	public ValueEval GetEvalForCell(int sheetIndex, int rowIndex, int columnIndex)
	{
		return GetSheetEvaluator(sheetIndex).GetEvalForCell(rowIndex, columnIndex);
	}
}
