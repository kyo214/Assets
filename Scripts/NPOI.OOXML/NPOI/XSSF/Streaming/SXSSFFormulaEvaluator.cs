using System;
using NPOI.SS.Formula;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFFormulaEvaluator : BaseXSSFFormulaEvaluator
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(SXSSFFormulaEvaluator));

	private IWorkbook wb;

	public SXSSFFormulaEvaluator(SXSSFWorkbook workbook)
		: this(workbook, null, null)
	{
	}

	private SXSSFFormulaEvaluator(SXSSFWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
		: this(workbook, new WorkbookEvaluator(SXSSFEvaluationWorkbook.Create(workbook), stabilityClassifier, udfFinder))
	{
	}

	private SXSSFFormulaEvaluator(SXSSFWorkbook workbook, WorkbookEvaluator bookEvaluator)
		: base(bookEvaluator)
	{
		wb = workbook;
	}

	public static SXSSFFormulaEvaluator Create(SXSSFWorkbook workbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
	{
		return new SXSSFFormulaEvaluator(workbook, stabilityClassifier, udfFinder);
	}

	protected override IEvaluationCell ToEvaluationCell(ICell cell)
	{
		if (!(cell is SXSSFCell))
		{
			throw new ArgumentException("Unexpected type of cell: " + cell.GetType()?.ToString() + ". Only SXSSFCells can be evaluated.");
		}
		return new SXSSFEvaluationCell((SXSSFCell)cell);
	}

	public static void EvaluateAllFormulaCells(SXSSFWorkbook wb, bool skipOutOfWindow)
	{
		SXSSFFormulaEvaluator sXSSFFormulaEvaluator = new SXSSFFormulaEvaluator(wb);
		foreach (SXSSFSheet item in wb)
		{
			if (item.AllRowsFlushed)
			{
				throw new SheetsFlushedException();
			}
		}
		foreach (ISheet item2 in wb)
		{
			int lastFlushedRowNumber = ((SXSSFSheet)item2).LastFlushedRowNumber;
			if (lastFlushedRowNumber > -1)
			{
				if (!skipOutOfWindow)
				{
					throw new RowFlushedException(0);
				}
				logger.Log(3, "Rows up to " + lastFlushedRowNumber + " have already been flushed, skipping");
			}
			foreach (IRow item3 in item2)
			{
				foreach (ICell item4 in item3)
				{
					if (item4.CellType == CellType.Formula)
					{
						sXSSFFormulaEvaluator.EvaluateFormulaCell(item4);
					}
				}
			}
		}
	}

	public override void EvaluateAll()
	{
		EvaluateAllFormulaCells((SXSSFWorkbook)wb, skipOutOfWindow: false);
	}
}
