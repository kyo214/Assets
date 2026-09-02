using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFFormulaEvaluator : BaseFormulaEvaluator
{
	private static readonly Type[] VALUE_CONTRUCTOR_CLASS_ARRAY;

	private static readonly Type[] AREA3D_CONSTRUCTOR_CLASS_ARRAY;

	private static readonly Type[] REFERENCE_CONSTRUCTOR_CLASS_ARRAY;

	private static readonly Type[] REF3D_CONSTRUCTOR_CLASS_ARRAY;

	private static readonly Hashtable VALUE_EVALS_MAP;

	protected IRow row;

	protected ISheet sheet;

	protected IWorkbook _book;

	static HSSFFormulaEvaluator()
	{
		VALUE_CONTRUCTOR_CLASS_ARRAY = new Type[1] { typeof(Ptg) };
		AREA3D_CONSTRUCTOR_CLASS_ARRAY = new Type[2]
		{
			typeof(Ptg),
			typeof(ValueEval[])
		};
		REFERENCE_CONSTRUCTOR_CLASS_ARRAY = new Type[2]
		{
			typeof(Ptg),
			typeof(ValueEval)
		};
		REF3D_CONSTRUCTOR_CLASS_ARRAY = new Type[2]
		{
			typeof(Ptg),
			typeof(ValueEval)
		};
		VALUE_EVALS_MAP = new Hashtable();
		VALUE_EVALS_MAP[typeof(BoolPtg)] = typeof(BoolEval);
		VALUE_EVALS_MAP[typeof(IntPtg)] = typeof(NumberEval);
		VALUE_EVALS_MAP[typeof(NumberPtg)] = typeof(NumberEval);
		VALUE_EVALS_MAP[typeof(StringPtg)] = typeof(StringEval);
	}

	public HSSFFormulaEvaluator(IWorkbook workbook)
		: this(workbook, null)
	{
	}

	public HSSFFormulaEvaluator(IWorkbook workbook, IStabilityClassifier stabilityClassifier)
		: this(workbook, stabilityClassifier, null)
	{
	}

	public HSSFFormulaEvaluator(IWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
		: base(new WorkbookEvaluator(HSSFEvaluationWorkbook.Create(workbook), stabilityClassifier, udfFinder))
	{
		_book = workbook;
	}

	public static HSSFFormulaEvaluator Create(IWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
	{
		return new HSSFFormulaEvaluator(workbook, stabilityClassifier, udfFinder);
	}

	protected override IRichTextString CreateRichTextString(string str)
	{
		return new HSSFRichTextString(str);
	}

	public static void SetupEnvironment(string[] workbookNames, HSSFFormulaEvaluator[] evaluators)
	{
		BaseFormulaEvaluator.SetupEnvironment(workbookNames, evaluators);
	}

	public override void SetupReferencedWorkbooks(Dictionary<string, IFormulaEvaluator> evaluators)
	{
		CollaboratingWorkbooksEnvironment.SetupFormulaEvaluator(evaluators);
	}

	public override void NotifyUpdateCell(ICell cell)
	{
		_bookEvaluator.NotifyUpdateCell(new HSSFEvaluationCell(cell));
	}

	public override void NotifyDeleteCell(ICell cell)
	{
		_bookEvaluator.NotifyDeleteCell(new HSSFEvaluationCell(cell));
	}

	public override void NotifySetFormula(ICell cell)
	{
		_bookEvaluator.NotifyUpdateCell(new HSSFEvaluationCell(cell));
	}

	protected override CellValue EvaluateFormulaCellValue(ICell cell)
	{
		ValueEval valueEval = _bookEvaluator.Evaluate(new HSSFEvaluationCell((HSSFCell)cell));
		if (valueEval is BoolEval)
		{
			return CellValue.ValueOf(((BoolEval)valueEval).BooleanValue);
		}
		if (valueEval is NumberEval)
		{
			return new CellValue(((NumberEval)valueEval).NumberValue);
		}
		if (valueEval is StringEval)
		{
			return new CellValue(((StringEval)valueEval).StringValue);
		}
		if (valueEval is ErrorEval)
		{
			return CellValue.GetError(((ErrorEval)valueEval).ErrorCode);
		}
		throw new InvalidOperationException("Unexpected eval class (" + valueEval.GetType().Name + ")");
	}

	public override ICell EvaluateInCell(ICell cell)
	{
		if (cell == null)
		{
			return null;
		}
		if (cell.CellType == CellType.Formula)
		{
			CellValue cv = EvaluateFormulaCellValue(cell);
			SetCellValue(cell, cv);
			BaseFormulaEvaluator.SetCellType(cell, cv);
		}
		return cell;
	}

	public static void EvaluateAllFormulaCells(HSSFWorkbook wb)
	{
		BaseFormulaEvaluator.EvaluateAllFormulaCells(wb, new HSSFFormulaEvaluator(wb));
	}

	public new static void EvaluateAllFormulaCells(IWorkbook wb)
	{
		BaseFormulaEvaluator.EvaluateAllFormulaCells(wb);
	}

	public override void EvaluateAll()
	{
		BaseFormulaEvaluator.EvaluateAllFormulaCells(_book, this);
	}
}
