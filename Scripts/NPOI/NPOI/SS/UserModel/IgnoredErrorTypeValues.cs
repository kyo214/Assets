namespace NPOI.SS.UserModel;

public static class IgnoredErrorTypeValues
{
	public static IgnoredErrorType[] Values = new IgnoredErrorType[9]
	{
		IgnoredErrorType.CalculatedColumn,
		IgnoredErrorType.EmptyCellReference,
		IgnoredErrorType.EvaluationError,
		IgnoredErrorType.Formula,
		IgnoredErrorType.FormulaRange,
		IgnoredErrorType.ListDataValidation,
		IgnoredErrorType.NumberStoredAsText,
		IgnoredErrorType.TwoDigitTextYear,
		IgnoredErrorType.UnlockedFormula
	};
}
