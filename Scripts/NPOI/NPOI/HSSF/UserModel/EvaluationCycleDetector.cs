using System;
using System.Collections;
using System.Text;

namespace NPOI.HSSF.UserModel;

internal class EvaluationCycleDetector
{
	private class CellEvaluationFrame
	{
		private HSSFWorkbook _workbook;

		private HSSFSheet _sheet;

		private int _srcRowNum;

		private int _srcColNum;

		public CellEvaluationFrame(HSSFWorkbook workbook, HSSFSheet sheet, int srcRowNum, int srcColNum)
		{
			if (workbook == null)
			{
				throw new ArgumentException("workbook must not be null");
			}
			if (sheet == null)
			{
				throw new ArgumentException("sheet must not be null");
			}
			_workbook = workbook;
			_sheet = sheet;
			_srcRowNum = srcRowNum;
			_srcColNum = srcColNum;
		}

		public override bool Equals(object obj)
		{
			CellEvaluationFrame cellEvaluationFrame = (CellEvaluationFrame)obj;
			if (_workbook != cellEvaluationFrame._workbook)
			{
				return false;
			}
			if (_sheet != cellEvaluationFrame._sheet)
			{
				return false;
			}
			if (_srcRowNum != cellEvaluationFrame._srcRowNum)
			{
				return false;
			}
			if (_srcColNum != cellEvaluationFrame._srcColNum)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _workbook.GetHashCode() ^ _sheet.GetHashCode() ^ _srcRowNum ^ _srcColNum;
		}

		public string FormatAsString()
		{
			return "R=" + _srcRowNum + " C=" + _srcColNum + " ShIx=" + _workbook.GetSheetIndex(_sheet);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append(GetType().Name).Append(" [");
			stringBuilder.Append(FormatAsString());
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	private IList _evaluationFrames;

	public EvaluationCycleDetector()
	{
		_evaluationFrames = new ArrayList();
	}

	public bool StartEvaluate(HSSFWorkbook workbook, HSSFSheet sheet, int srcRowNum, int srcColNum)
	{
		CellEvaluationFrame value = new CellEvaluationFrame(workbook, sheet, srcRowNum, srcColNum);
		if (_evaluationFrames.Contains(value))
		{
			return false;
		}
		_evaluationFrames.Add(value);
		return true;
	}

	public void EndEvaluate(HSSFWorkbook workbook, HSSFSheet sheet, int srcRowNum, int srcColNum)
	{
		int count = _evaluationFrames.Count;
		if (count < 1)
		{
			throw new InvalidOperationException("Call to endEvaluate without matching call to startEvaluate");
		}
		count--;
		CellEvaluationFrame cellEvaluationFrame = (CellEvaluationFrame)_evaluationFrames[count];
		CellEvaluationFrame cellEvaluationFrame2 = new CellEvaluationFrame(workbook, sheet, srcRowNum, srcColNum);
		if (!cellEvaluationFrame2.Equals(cellEvaluationFrame))
		{
			throw new Exception("Wrong cell specified. Corresponding startEvaluate() call was for cell {" + cellEvaluationFrame.FormatAsString() + "} this endEvaluate() call Is for cell {" + cellEvaluationFrame2.FormatAsString() + "}");
		}
		_evaluationFrames.Remove(count);
	}
}
