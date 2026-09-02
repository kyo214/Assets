using System;
using System.Globalization;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula;

public class OperationEvaluationContext
{
	public static readonly FreeRefFunction UDF = UserDefinedFunction.instance;

	private IEvaluationWorkbook _workbook;

	private int _sheetIndex;

	private int _rowIndex;

	private int _columnIndex;

	private EvaluationTracker _tracker;

	private WorkbookEvaluator _bookEvaluator;

	public int RowIndex => _rowIndex;

	public int ColumnIndex => _columnIndex;

	public int SheetIndex => _sheetIndex;

	public OperationEvaluationContext(WorkbookEvaluator bookEvaluator, IEvaluationWorkbook workbook, int sheetIndex, int srcRowNum, int srcColNum, EvaluationTracker tracker)
	{
		_bookEvaluator = bookEvaluator;
		_workbook = workbook;
		_sheetIndex = sheetIndex;
		_rowIndex = srcRowNum;
		_columnIndex = srcColNum;
		_tracker = tracker;
	}

	public IEvaluationWorkbook GetWorkbook()
	{
		return _workbook;
	}

	private SheetRangeEvaluator CreateExternSheetRefEvaluator(IExternSheetReferenceToken ptg)
	{
		return CreateExternSheetRefEvaluator(ptg.ExternSheetIndex);
	}

	private SheetRangeEvaluator CreateExternSheetRefEvaluator(string firstSheetName, string lastSheetName, int externalWorkbookNumber)
	{
		ExternalSheet externalSheet = _workbook.GetExternalSheet(firstSheetName, lastSheetName, externalWorkbookNumber);
		return CreateExternSheetRefEvaluator(externalSheet);
	}

	private SheetRangeEvaluator CreateExternSheetRefEvaluator(int externSheetIndex)
	{
		ExternalSheet externalSheet = _workbook.GetExternalSheet(externSheetIndex);
		return CreateExternSheetRefEvaluator(externalSheet);
	}

	private SheetRangeEvaluator CreateExternSheetRefEvaluator(ExternalSheet externalSheet)
	{
		int num = -1;
		WorkbookEvaluator workbookEvaluator;
		int num2;
		if (externalSheet == null || externalSheet.WorkbookName == null)
		{
			workbookEvaluator = _bookEvaluator;
			num2 = ((externalSheet != null) ? _workbook.GetSheetIndex(externalSheet.SheetName) : 0);
			if (externalSheet is ExternalSheetRange)
			{
				string lastSheetName = ((ExternalSheetRange)externalSheet).LastSheetName;
				num = _workbook.GetSheetIndex(lastSheetName);
			}
		}
		else
		{
			string workbookName = externalSheet.WorkbookName;
			try
			{
				workbookEvaluator = _bookEvaluator.GetOtherWorkbookEvaluator(workbookName);
			}
			catch (WorkbookNotFoundException ex)
			{
				throw new RuntimeException(ex.Message, ex);
			}
			num2 = workbookEvaluator.GetSheetIndex(externalSheet.SheetName);
			if (externalSheet is ExternalSheetRange)
			{
				string lastSheetName2 = ((ExternalSheetRange)externalSheet).LastSheetName;
				num = workbookEvaluator.GetSheetIndex(lastSheetName2);
			}
			if (num2 < 0)
			{
				throw new Exception("Invalid sheet name '" + externalSheet.SheetName + "' in bool '" + workbookName + "'.");
			}
		}
		if (num == -1)
		{
			num = num2;
		}
		SheetRefEvaluator[] array = new SheetRefEvaluator[num - num2 + 1];
		for (int i = 0; i < array.Length; i++)
		{
			int sheetIndex = i + num2;
			array[i] = new SheetRefEvaluator(workbookEvaluator, _tracker, sheetIndex);
		}
		return new SheetRangeEvaluator(num2, num, array);
	}

	private SheetRefEvaluator CreateExternSheetRefEvaluator(string workbookName, string sheetName)
	{
		WorkbookEvaluator workbookEvaluator;
		if (workbookName == null)
		{
			workbookEvaluator = _bookEvaluator;
		}
		else
		{
			if (sheetName == null)
			{
				throw new ArgumentException("sheetName must not be null if workbookName is provided");
			}
			try
			{
				workbookEvaluator = _bookEvaluator.GetOtherWorkbookEvaluator(workbookName);
			}
			catch (WorkbookNotFoundException)
			{
				return null;
			}
		}
		int num = ((sheetName == null) ? _sheetIndex : workbookEvaluator.GetSheetIndex(sheetName));
		if (num < 0)
		{
			return null;
		}
		return new SheetRefEvaluator(workbookEvaluator, _tracker, num);
	}

	public SheetRangeEvaluator GetRefEvaluatorForCurrentSheet()
	{
		SheetRefEvaluator sheetEvaluator = new SheetRefEvaluator(_bookEvaluator, _tracker, _sheetIndex);
		return new SheetRangeEvaluator(_sheetIndex, sheetEvaluator);
	}

	public ValueEval GetDynamicReference(string workbookName, string sheetName, string refStrPart1, string refStrPart2, bool isA1Style)
	{
		if (!isA1Style)
		{
			throw new Exception("R1C1 style not supported yet");
		}
		SheetRefEvaluator sheetRefEvaluator = CreateExternSheetRefEvaluator(workbookName, sheetName);
		if (sheetRefEvaluator == null)
		{
			return ErrorEval.REF_INVALID;
		}
		SheetRangeEvaluator sheetRangeEvaluator = new SheetRangeEvaluator(_sheetIndex, sheetRefEvaluator);
		SpreadsheetVersion spreadsheetVersion = ((IFormulaParsingWorkbook)_workbook).GetSpreadsheetVersion();
		NameType nameType = ClassifyCellReference(refStrPart1, spreadsheetVersion);
		switch (nameType)
		{
		case NameType.BadCellOrNamedRange:
			return ErrorEval.REF_INVALID;
		case NameType.NamedRange:
		{
			IEvaluationName name = ((IFormulaParsingWorkbook)_workbook).GetName(refStrPart1, _sheetIndex);
			if (!name.IsRange)
			{
				throw new Exception("Specified name '" + refStrPart1 + "' is not a range as expected.");
			}
			return _bookEvaluator.EvaluateNameFormula(name.NameDefinition, this);
		}
		default:
		{
			if (refStrPart2 == null)
			{
				switch (nameType)
				{
				case NameType.Column:
				case NameType.Row:
					return ErrorEval.REF_INVALID;
				case NameType.Cell:
				{
					CellReference cellReference = new CellReference(refStrPart1);
					return new LazyRefEval(cellReference.Row, cellReference.Col, sheetRangeEvaluator);
				}
				default:
					throw new InvalidOperationException("Unexpected reference classification of '" + refStrPart1 + "'.");
				}
			}
			NameType nameType2 = ClassifyCellReference(refStrPart1, spreadsheetVersion);
			switch (nameType2)
			{
			case NameType.BadCellOrNamedRange:
				return ErrorEval.REF_INVALID;
			case NameType.NamedRange:
				throw new Exception("Cannot Evaluate '" + refStrPart1 + "'. Indirect Evaluation of defined names not supported yet");
			default:
			{
				if (nameType2 != nameType)
				{
					return ErrorEval.REF_INVALID;
				}
				int firstRowIndex;
				int lastRowIndex;
				int firstColumnIndex;
				int lastColumnIndex;
				switch (nameType)
				{
				case NameType.Column:
					firstRowIndex = 0;
					if (nameType2.Equals(NameType.Column))
					{
						lastRowIndex = spreadsheetVersion.LastRowIndex;
						firstColumnIndex = ParseRowRef(refStrPart1);
						lastColumnIndex = ParseRowRef(refStrPart2);
					}
					else
					{
						lastRowIndex = spreadsheetVersion.LastRowIndex;
						firstColumnIndex = ParseColRef(refStrPart1);
						lastColumnIndex = ParseColRef(refStrPart2);
					}
					break;
				case NameType.Row:
					firstColumnIndex = 0;
					if (nameType2.Equals(NameType.Row))
					{
						firstRowIndex = ParseColRef(refStrPart1);
						lastRowIndex = ParseColRef(refStrPart2);
						lastColumnIndex = spreadsheetVersion.LastColumnIndex;
					}
					else
					{
						lastColumnIndex = spreadsheetVersion.LastColumnIndex;
						firstRowIndex = ParseRowRef(refStrPart1);
						lastRowIndex = ParseRowRef(refStrPart2);
					}
					break;
				case NameType.Cell:
				{
					CellReference cellReference2 = new CellReference(refStrPart1);
					firstRowIndex = cellReference2.Row;
					firstColumnIndex = cellReference2.Col;
					CellReference cellReference3 = new CellReference(refStrPart2);
					lastRowIndex = cellReference3.Row;
					lastColumnIndex = cellReference3.Col;
					break;
				}
				default:
					throw new InvalidOperationException("Unexpected reference classification of '" + refStrPart1 + "'.");
				}
				return new LazyAreaEval(firstRowIndex, firstColumnIndex, lastRowIndex, lastColumnIndex, sheetRangeEvaluator);
			}
			}
		}
		}
	}

	private static int ParseRowRef(string refStrPart)
	{
		return CellReference.ConvertColStringToIndex(refStrPart);
	}

	private static int ParseColRef(string refStrPart)
	{
		return int.Parse(refStrPart, CultureInfo.InvariantCulture) - 1;
	}

	private static NameType ClassifyCellReference(string str, SpreadsheetVersion ssVersion)
	{
		if (str.Length < 1)
		{
			return NameType.BadCellOrNamedRange;
		}
		return CellReference.ClassifyCellReference(str, ssVersion);
	}

	public FreeRefFunction FindUserDefinedFunction(string functionName)
	{
		return _bookEvaluator.FindUserDefinedFunction(functionName);
	}

	public ValueEval GetRefEval(int rowIndex, int columnIndex)
	{
		SheetRangeEvaluator refEvaluatorForCurrentSheet = GetRefEvaluatorForCurrentSheet();
		return new LazyRefEval(rowIndex, columnIndex, refEvaluatorForCurrentSheet);
	}

	public ValueEval GetRef3DEval(Ref3DPtg rptg)
	{
		SheetRangeEvaluator sre = CreateExternSheetRefEvaluator(rptg.ExternSheetIndex);
		return new LazyRefEval(rptg.Row, rptg.Column, sre);
	}

	public ValueEval GetRef3DEval(Ref3DPxg rptg)
	{
		SheetRangeEvaluator sre = CreateExternSheetRefEvaluator(rptg.SheetName, rptg.LastSheetName, rptg.ExternalWorkbookNumber);
		return new LazyRefEval(rptg.Row, rptg.Column, sre);
	}

	public ValueEval GetAreaEval(int firstRowIndex, int firstColumnIndex, int lastRowIndex, int lastColumnIndex)
	{
		SheetRangeEvaluator refEvaluatorForCurrentSheet = GetRefEvaluatorForCurrentSheet();
		return new LazyAreaEval(firstRowIndex, firstColumnIndex, lastRowIndex, lastColumnIndex, refEvaluatorForCurrentSheet);
	}

	public ValueEval GetArea3DEval(Area3DPtg aptg)
	{
		SheetRangeEvaluator evaluator = CreateExternSheetRefEvaluator(aptg.ExternSheetIndex);
		return new LazyAreaEval(aptg.FirstRow, aptg.FirstColumn, aptg.LastRow, aptg.LastColumn, evaluator);
	}

	public ValueEval GetArea3DEval(Area3DPxg aptg)
	{
		SheetRangeEvaluator evaluator = CreateExternSheetRefEvaluator(aptg.SheetName, aptg.LastSheetName, aptg.ExternalWorkbookNumber);
		return new LazyAreaEval(aptg.FirstRow, aptg.FirstColumn, aptg.LastRow, aptg.LastColumn, evaluator);
	}

	public ValueEval GetNameXEval(NameXPtg nameXPtg)
	{
		ExternalSheet externalSheet = _workbook.GetExternalSheet(nameXPtg.SheetRefIndex);
		if (externalSheet == null || externalSheet.WorkbookName == null)
		{
			return GetLocalNameXEval(nameXPtg);
		}
		string workbookName = externalSheet.WorkbookName;
		ExternalName externalName = _workbook.GetExternalName(nameXPtg.SheetRefIndex, nameXPtg.NameIndex);
		return GetExternalNameXEval(externalName, workbookName);
	}

	public ValueEval GetNameXEval(NameXPxg nameXPxg)
	{
		ExternalSheet externalSheet = _workbook.GetExternalSheet(nameXPxg.SheetName, null, nameXPxg.ExternalWorkbookNumber);
		if (externalSheet == null || externalSheet.WorkbookName == null)
		{
			return GetLocalNameXEval(nameXPxg);
		}
		string workbookName = externalSheet.WorkbookName;
		ExternalName externalName = _workbook.GetExternalName(nameXPxg.NameName, nameXPxg.SheetName, nameXPxg.ExternalWorkbookNumber);
		return GetExternalNameXEval(externalName, workbookName);
	}

	private ValueEval GetLocalNameXEval(NameXPxg nameXPxg)
	{
		int sheetIndex = -1;
		if (nameXPxg.SheetName != null)
		{
			sheetIndex = _workbook.GetSheetIndex(nameXPxg.SheetName);
		}
		string nameName = nameXPxg.NameName;
		IEvaluationName name = _workbook.GetName(nameName, sheetIndex);
		if (name != null)
		{
			return new ExternalNameEval(name);
		}
		return new FunctionNameEval(nameName);
	}

	private ValueEval GetLocalNameXEval(NameXPtg nameXPtg)
	{
		string text = _workbook.ResolveNameXText(nameXPtg);
		int num = text.IndexOf('!');
		IEvaluationName evaluationName = null;
		if (num > -1)
		{
			string sheetName = text.Substring(0, num);
			string name = text.Substring(num + 1);
			evaluationName = _workbook.GetName(name, _workbook.GetSheetIndex(sheetName));
		}
		else
		{
			evaluationName = _workbook.GetName(text, -1);
		}
		if (evaluationName != null)
		{
			return new ExternalNameEval(evaluationName);
		}
		return new FunctionNameEval(text);
	}

	private ValueEval GetExternalNameXEval(ExternalName externName, string workbookName)
	{
		try
		{
			WorkbookEvaluator otherWorkbookEvaluator = _bookEvaluator.GetOtherWorkbookEvaluator(workbookName);
			IEvaluationName name = otherWorkbookEvaluator.GetName(externName.Name, externName.Ix - 1);
			if (name != null && name.HasFormula)
			{
				if (name.NameDefinition.Length > 1)
				{
					throw new Exception("Complex name formulas not supported yet");
				}
				OperationEvaluationContext operationEvaluationContext = new OperationEvaluationContext(otherWorkbookEvaluator, otherWorkbookEvaluator.Workbook, -1, -1, -1, _tracker);
				Ptg ptg = name.NameDefinition[0];
				if (ptg is Ref3DPtg)
				{
					Ref3DPtg rptg = (Ref3DPtg)ptg;
					return operationEvaluationContext.GetRef3DEval(rptg);
				}
				if (ptg is Ref3DPxg)
				{
					Ref3DPxg rptg2 = (Ref3DPxg)ptg;
					return operationEvaluationContext.GetRef3DEval(rptg2);
				}
				if (ptg is Area3DPtg)
				{
					Area3DPtg aptg = (Area3DPtg)ptg;
					return operationEvaluationContext.GetArea3DEval(aptg);
				}
				if (ptg is Area3DPxg)
				{
					Area3DPxg aptg2 = (Area3DPxg)ptg;
					return operationEvaluationContext.GetArea3DEval(aptg2);
				}
			}
			return ErrorEval.REF_INVALID;
		}
		catch (WorkbookNotFoundException)
		{
			return ErrorEval.REF_INVALID;
		}
	}
}
