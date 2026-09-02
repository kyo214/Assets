using System;
using NPOI.SS.Formula;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFFormulaEvaluator : BaseXSSFFormulaEvaluator
{
	private XSSFWorkbook _book;

	public XSSFFormulaEvaluator(IWorkbook workbook)
		: this(workbook as XSSFWorkbook, null, null)
	{
	}

	public XSSFFormulaEvaluator(XSSFWorkbook workbook)
		: this(workbook, null, null)
	{
	}

	private XSSFFormulaEvaluator(XSSFWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
		: this(workbook, new WorkbookEvaluator(XSSFEvaluationWorkbook.Create(workbook), stabilityClassifier, udfFinder))
	{
	}

	protected XSSFFormulaEvaluator(XSSFWorkbook workbook, WorkbookEvaluator bookEvaluator)
		: base(bookEvaluator)
	{
		_book = workbook;
	}

	public static XSSFFormulaEvaluator Create(XSSFWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
	{
		return new XSSFFormulaEvaluator(workbook, stabilityClassifier, udfFinder);
	}

	public static void EvaluateAllFormulaCells(XSSFWorkbook wb)
	{
		BaseFormulaEvaluator.EvaluateAllFormulaCells(wb);
	}

	public override void EvaluateAll()
	{
		BaseFormulaEvaluator.EvaluateAllFormulaCells(_book, this);
	}

	protected override IEvaluationCell ToEvaluationCell(ICell cell)
	{
		if (!(cell is XSSFCell))
		{
			throw new ArgumentException("Unexpected type of cell: " + cell.GetType().Name + ". Only XSSFCells can be evaluated.");
		}
		return new XSSFEvaluationCell((XSSFCell)cell);
	}
}
