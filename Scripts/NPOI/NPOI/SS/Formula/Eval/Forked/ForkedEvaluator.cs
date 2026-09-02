using System;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Eval.Forked;

public class ForkedEvaluator
{
	private WorkbookEvaluator _evaluator;

	private ForkedEvaluationWorkbook _sewb;

	private ForkedEvaluator(IEvaluationWorkbook masterWorkbook, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
	{
		_sewb = new ForkedEvaluationWorkbook(masterWorkbook);
		_evaluator = new WorkbookEvaluator(_sewb, stabilityClassifier, udfFinder);
	}

	private static IEvaluationWorkbook CreateEvaluationWorkbook(IWorkbook wb)
	{
		if (wb is HSSFWorkbook)
		{
			return HSSFEvaluationWorkbook.Create((HSSFWorkbook)wb);
		}
		try
		{
			Type type = Type.GetType("NPOI.XSSF.UserModel.XSSFEvaluationWorkbook");
			Type type2 = Type.GetType("NPOI.XSSF.UserMode.XSSFWorkbook");
			return (IEvaluationWorkbook)type.GetMethod("create", new Type[1] { type2 }).Invoke(null, new object[1] { wb });
		}
		catch (Exception innerException)
		{
			throw new ArgumentException("Unexpected workbook type (" + wb.GetType().Name + ") - check for poi-ooxml and poi-ooxml schemas jar in the classpath", innerException);
		}
	}

	public static ForkedEvaluator Create(IWorkbook wb, IStabilityClassifier stabilityClassifier, UDFFinder udfFinder)
	{
		return new ForkedEvaluator(CreateEvaluationWorkbook(wb), stabilityClassifier, udfFinder);
	}

	public void UpdateCell(string sheetName, int rowIndex, int columnIndex, ValueEval value)
	{
		ForkedEvaluationCell orCreateUpdatableCell = _sewb.GetOrCreateUpdatableCell(sheetName, rowIndex, columnIndex);
		orCreateUpdatableCell.SetValue(value);
		_evaluator.NotifyUpdateCell(orCreateUpdatableCell);
	}

	public void CopyUpdatedCells(IWorkbook workbook)
	{
		_sewb.CopyUpdatedCells(workbook);
	}

	public ValueEval Evaluate(string sheetName, int rowIndex, int columnIndex)
	{
		IEvaluationCell evaluationCell = _sewb.GetEvaluationCell(sheetName, rowIndex, columnIndex);
		return evaluationCell.CellType switch
		{
			CellType.Boolean => BoolEval.ValueOf(evaluationCell.BooleanCellValue), 
			CellType.Error => ErrorEval.ValueOf(evaluationCell.ErrorCellValue), 
			CellType.Formula => _evaluator.Evaluate(evaluationCell), 
			CellType.Numeric => new NumberEval(evaluationCell.NumericCellValue), 
			CellType.String => new StringEval(evaluationCell.StringCellValue), 
			CellType.Blank => null, 
			_ => throw new InvalidOperationException("Bad cell type (" + evaluationCell.CellType.ToString() + ")"), 
		};
	}

	public static void SetupEnvironment(string[] workbookNames, ForkedEvaluator[] Evaluators)
	{
		WorkbookEvaluator[] array = new WorkbookEvaluator[Evaluators.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Evaluators[i]._evaluator;
		}
		CollaboratingWorkbooksEnvironment.Setup(workbookNames, array);
	}
}
