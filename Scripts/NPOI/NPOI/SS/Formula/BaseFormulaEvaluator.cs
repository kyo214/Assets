using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula;

public abstract class BaseFormulaEvaluator : IFormulaEvaluator, IWorkbookEvaluatorProvider
{
	protected WorkbookEvaluator _bookEvaluator;

	public bool IgnoreMissingWorkbooks
	{
		get
		{
			return _bookEvaluator.IgnoreMissingWorkbooks;
		}
		set
		{
			_bookEvaluator.IgnoreMissingWorkbooks = value;
		}
	}

	public bool DebugEvaluationOutputForNextEval
	{
		get
		{
			return _bookEvaluator.DebugEvaluationOutputForNextEval;
		}
		set
		{
			_bookEvaluator.DebugEvaluationOutputForNextEval = value;
		}
	}

	protected BaseFormulaEvaluator(WorkbookEvaluator bookEvaluator)
	{
		_bookEvaluator = bookEvaluator;
	}

	public static void SetupEnvironment(string[] workbookNames, BaseFormulaEvaluator[] Evaluators)
	{
		WorkbookEvaluator[] array = new WorkbookEvaluator[Evaluators.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Evaluators[i]._bookEvaluator;
		}
		CollaboratingWorkbooksEnvironment.Setup(workbookNames, array);
	}

	public virtual void SetupReferencedWorkbooks(Dictionary<string, IFormulaEvaluator> evaluators)
	{
		CollaboratingWorkbooksEnvironment.SetupFormulaEvaluator(evaluators);
	}

	public WorkbookEvaluator GetWorkbookEvaluator()
	{
		return _bookEvaluator;
	}

	public void ClearAllCachedResultValues()
	{
		_bookEvaluator.ClearAllCachedResultValues();
	}

	public CellValue Evaluate(ICell cell)
	{
		if (cell == null)
		{
			return null;
		}
		return cell.CellType switch
		{
			CellType.Boolean => CellValue.ValueOf(cell.BooleanCellValue), 
			CellType.Error => CellValue.GetError(cell.ErrorCellValue), 
			CellType.Formula => EvaluateFormulaCellValue(cell), 
			CellType.Numeric => new CellValue(cell.NumericCellValue), 
			CellType.String => new CellValue(cell.RichStringCellValue.String), 
			CellType.Blank => null, 
			_ => throw new InvalidOperationException("Bad cell type (" + cell.CellType.ToString() + ")"), 
		};
	}

	public virtual ICell EvaluateInCell(ICell cell)
	{
		if (cell == null)
		{
			return null;
		}
		if (cell.CellType == CellType.Formula)
		{
			CellValue cv = EvaluateFormulaCellValue(cell);
			SetCellValue(cell, cv);
			SetCellType(cell, cv);
		}
		return cell;
	}

	protected abstract CellValue EvaluateFormulaCellValue(ICell cell);

	public CellType EvaluateFormulaCell(ICell cell)
	{
		return EvaluateFormulaCellEnum(cell);
	}

	public virtual CellType EvaluateFormulaCellEnum(ICell cell)
	{
		if (cell == null || cell.CellType != CellType.Formula)
		{
			return CellType.Unknown;
		}
		CellValue cellValue = EvaluateFormulaCellValue(cell);
		SetCellValue(cell, cellValue);
		return cellValue.CellType;
	}

	protected static void SetCellType(ICell cell, CellValue cv)
	{
		CellType cellType = cv.CellType;
		switch (cellType)
		{
		case CellType.Numeric:
		case CellType.String:
		case CellType.Boolean:
		case CellType.Error:
			cell.SetCellType(cellType);
			break;
		case CellType.Blank:
			throw new ArgumentException("This should never happen. Blanks eventually Get translated to zero.");
		case CellType.Formula:
			throw new ArgumentException("This should never happen. Formulas should have already been Evaluated.");
		default:
			throw new InvalidOperationException("Unexpected cell value type (" + cellType.ToString() + ")");
		}
	}

	protected abstract IRichTextString CreateRichTextString(string str);

	protected void SetCellValue(ICell cell, CellValue cv)
	{
		CellType cellType = cv.CellType;
		switch (cellType)
		{
		case CellType.Boolean:
			cell.SetCellValue(cv.BooleanValue);
			break;
		case CellType.Error:
			cell.SetCellErrorValue((byte)cv.ErrorValue);
			break;
		case CellType.Numeric:
			cell.SetCellValue(cv.NumberValue);
			break;
		case CellType.String:
			cell.SetCellValue(CreateRichTextString(cv.StringValue));
			break;
		default:
			throw new InvalidOperationException("Unexpected cell value type (" + cellType.ToString() + ")");
		}
	}

	public static void EvaluateAllFormulaCells(IWorkbook wb)
	{
		IFormulaEvaluator evaluator = wb.GetCreationHelper().CreateFormulaEvaluator();
		EvaluateAllFormulaCells(wb, evaluator);
	}

	protected static void EvaluateAllFormulaCells(IWorkbook wb, IFormulaEvaluator evaluator)
	{
		for (int i = 0; i < wb.NumberOfSheets; i++)
		{
			foreach (IRow item in wb.GetSheetAt(i))
			{
				foreach (ICell item2 in item)
				{
					if (item2.CellType == CellType.Formula)
					{
						evaluator.EvaluateFormulaCell(item2);
					}
				}
			}
		}
	}

	public abstract void NotifySetFormula(ICell cell);

	public abstract void NotifyDeleteCell(ICell cell);

	public abstract void NotifyUpdateCell(ICell cell);

	public abstract void EvaluateAll();
}
