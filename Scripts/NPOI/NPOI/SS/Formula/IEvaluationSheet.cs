namespace NPOI.SS.Formula;

public interface IEvaluationSheet
{
	IEvaluationCell GetCell(int rowIndex, int columnIndex);

	void ClearAllCachedResultValues();
}
