using System.Collections.Generic;

namespace NPOI.SS.UserModel;

public interface IFormulaEvaluator
{
	bool IgnoreMissingWorkbooks { get; set; }

	bool DebugEvaluationOutputForNextEval { get; set; }

	void ClearAllCachedResultValues();

	void NotifySetFormula(ICell cell);

	void NotifyDeleteCell(ICell cell);

	void NotifyUpdateCell(ICell cell);

	CellValue Evaluate(ICell cell);

	void EvaluateAll();

	CellType EvaluateFormulaCell(ICell cell);

	ICell EvaluateInCell(ICell cell);

	void SetupReferencedWorkbooks(Dictionary<string, IFormulaEvaluator> workbooks);
}
