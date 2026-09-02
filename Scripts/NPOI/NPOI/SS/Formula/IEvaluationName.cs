using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula;

public interface IEvaluationName
{
	string NameText { get; }

	bool IsFunctionName { get; }

	bool HasFormula { get; }

	Ptg[] NameDefinition { get; }

	bool IsRange { get; }

	NamePtg CreatePtg();
}
