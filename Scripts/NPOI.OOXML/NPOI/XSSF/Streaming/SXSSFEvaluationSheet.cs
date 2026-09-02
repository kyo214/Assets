using NPOI.SS.Formula;

namespace NPOI.XSSF.Streaming;

public class SXSSFEvaluationSheet : IEvaluationSheet
{
	private SXSSFSheet _xs;

	public SXSSFEvaluationSheet(SXSSFSheet sheet)
	{
		_xs = sheet;
	}

	public SXSSFSheet GetSXSSFSheet()
	{
		return _xs;
	}

	public IEvaluationCell GetCell(int rowIndex, int columnIndex)
	{
		SXSSFRow sXSSFRow = (SXSSFRow)_xs.GetRow(rowIndex);
		if (sXSSFRow == null)
		{
			if (rowIndex <= _xs.LastFlushedRowNumber)
			{
				throw new RowFlushedException(rowIndex);
			}
			return null;
		}
		SXSSFCell sXSSFCell = (SXSSFCell)sXSSFRow.GetCell(columnIndex);
		if (sXSSFCell == null)
		{
			return null;
		}
		return new SXSSFEvaluationCell(sXSSFCell, this);
	}

	public void ClearAllCachedResultValues()
	{
	}
}
