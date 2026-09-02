using System;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public abstract class BaseXSSFFormulaEvaluator : BaseFormulaEvaluator
{
	protected BaseXSSFFormulaEvaluator(WorkbookEvaluator bookEvaluator)
		: base(bookEvaluator)
	{
	}

	protected override IRichTextString CreateRichTextString(string str)
	{
		return new XSSFRichTextString(str);
	}

	public override void NotifySetFormula(ICell cell)
	{
		_bookEvaluator.NotifyUpdateCell(new XSSFEvaluationCell((XSSFCell)cell));
	}

	public override void NotifyDeleteCell(ICell cell)
	{
		_bookEvaluator.NotifyDeleteCell(new XSSFEvaluationCell((XSSFCell)cell));
	}

	public override void NotifyUpdateCell(ICell cell)
	{
		_bookEvaluator.NotifyUpdateCell(new XSSFEvaluationCell((XSSFCell)cell));
	}

	protected abstract IEvaluationCell ToEvaluationCell(ICell cell);

	protected override CellValue EvaluateFormulaCellValue(ICell cell)
	{
		IEvaluationCell srcCell = ToEvaluationCell(cell);
		ValueEval valueEval = _bookEvaluator.Evaluate(srcCell);
		if (valueEval is NumberEval)
		{
			return new CellValue(((NumberEval)valueEval).NumberValue);
		}
		if (valueEval is BoolEval)
		{
			return CellValue.ValueOf(((BoolEval)valueEval).BooleanValue);
		}
		if (valueEval is StringEval)
		{
			return new CellValue(((StringEval)valueEval).StringValue);
		}
		if (valueEval is ErrorEval)
		{
			return CellValue.GetError(((ErrorEval)valueEval).ErrorCode);
		}
		throw new Exception("Unexpected eval class (" + valueEval.GetType().Name + ")");
	}
}
