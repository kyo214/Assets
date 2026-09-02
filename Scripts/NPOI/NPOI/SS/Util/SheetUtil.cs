using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using NPOI.SS.UserModel;

namespace NPOI.SS.Util;

public class SheetUtil
{
	public class DummyEvaluator : IFormulaEvaluator
	{
		public bool IgnoreMissingWorkbooks { get; set; }

		public bool DebugEvaluationOutputForNextEval
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void ClearAllCachedResultValues()
		{
		}

		public void NotifySetFormula(ICell cell)
		{
		}

		public void NotifyDeleteCell(ICell cell)
		{
		}

		public void NotifyUpdateCell(ICell cell)
		{
		}

		public CellValue Evaluate(ICell cell)
		{
			return null;
		}

		public ICell EvaluateInCell(ICell cell)
		{
			return null;
		}

		public void SetupReferencedWorkbooks(Dictionary<string, IFormulaEvaluator> workbooks)
		{
		}

		public void EvaluateAll()
		{
		}

		public CellType EvaluateFormulaCell(ICell cell)
		{
			return cell.CachedFormulaResultType;
		}
	}

	private static char defaultChar = '0';

	private static IFormulaEvaluator dummyEvaluator = new DummyEvaluator();

	public static IRow CopyRow(ISheet sourceSheet, int sourceRowIndex, ISheet targetSheet, int targetRowIndex)
	{
		IRow row = targetSheet.GetRow(targetRowIndex);
		IRow row2 = sourceSheet.GetRow(sourceRowIndex);
		if (row != null)
		{
			targetSheet.RemoveRow(row);
		}
		row = targetSheet.CreateRow(targetRowIndex);
		if (row2 == null)
		{
			throw new ArgumentNullException("source row doesn't exist");
		}
		for (int i = row2.FirstCellNum; i < row2.LastCellNum; i++)
		{
			ICell cell = row2.GetCell(i);
			if (cell != null)
			{
				ICell cell2 = row.CreateCell(i);
				if (cell.CellStyle != null)
				{
					cell2.CellStyle = cell.CellStyle;
				}
				if (cell.CellComment != null)
				{
					cell2.CellComment = cell.CellComment;
				}
				if (cell.Hyperlink != null)
				{
					cell2.Hyperlink = cell.Hyperlink;
				}
				cell2.SetCellType(cell.CellType);
				switch (cell.CellType)
				{
				case CellType.Blank:
					cell2.SetCellValue(cell.StringCellValue);
					break;
				case CellType.Boolean:
					cell2.SetCellValue(cell.BooleanCellValue);
					break;
				case CellType.Error:
					cell2.SetCellErrorValue(cell.ErrorCellValue);
					break;
				case CellType.Formula:
					cell2.SetCellFormula(cell.CellFormula);
					break;
				case CellType.Numeric:
					cell2.SetCellValue(cell.NumericCellValue);
					break;
				case CellType.String:
					cell2.SetCellValue(cell.RichStringCellValue);
					break;
				}
			}
		}
		for (int j = 0; j < sourceSheet.NumMergedRegions; j++)
		{
			CellRangeAddress mergedRegion = sourceSheet.GetMergedRegion(j);
			if (mergedRegion != null && mergedRegion.FirstRow == row2.RowNum)
			{
				CellRangeAddress region = new CellRangeAddress(row.RowNum, row.RowNum + (mergedRegion.LastRow - mergedRegion.FirstRow), mergedRegion.FirstColumn, mergedRegion.LastColumn);
				targetSheet.AddMergedRegion(region);
			}
		}
		return row;
	}

	public static IRow CopyRow(ISheet sheet, int sourceRowIndex, int targetRowIndex)
	{
		if (sourceRowIndex == targetRowIndex)
		{
			throw new ArgumentException("sourceIndex and targetIndex cannot be same");
		}
		IRow row = sheet.GetRow(targetRowIndex);
		IRow row2 = sheet.GetRow(sourceRowIndex);
		if (row != null)
		{
			sheet.ShiftRows(targetRowIndex, sheet.LastRowNum, 1);
		}
		row = sheet.CreateRow(targetRowIndex);
		row.Height = row2.Height;
		for (int i = row2.FirstCellNum; i < row2.LastCellNum; i++)
		{
			ICell cell = row2.GetCell(i);
			if (cell != null)
			{
				ICell cell2 = row.CreateCell(i);
				if (cell.CellStyle != null)
				{
					cell2.CellStyle = cell.CellStyle;
				}
				if (cell.CellComment != null)
				{
					cell2.CellComment = cell.CellComment;
				}
				if (cell.Hyperlink != null)
				{
					cell2.Hyperlink = cell.Hyperlink;
				}
				cell2.SetCellType(cell.CellType);
				switch (cell.CellType)
				{
				case CellType.Blank:
					cell2.SetCellValue(cell.StringCellValue);
					break;
				case CellType.Boolean:
					cell2.SetCellValue(cell.BooleanCellValue);
					break;
				case CellType.Error:
					cell2.SetCellErrorValue(cell.ErrorCellValue);
					break;
				case CellType.Formula:
					cell2.SetCellFormula(cell.CellFormula);
					break;
				case CellType.Numeric:
					cell2.SetCellValue(cell.NumericCellValue);
					break;
				case CellType.String:
					cell2.SetCellValue(cell.RichStringCellValue);
					break;
				}
			}
		}
		for (int j = 0; j < sheet.NumMergedRegions; j++)
		{
			CellRangeAddress mergedRegion = sheet.GetMergedRegion(j);
			if (mergedRegion != null && mergedRegion.FirstRow == row2.RowNum)
			{
				CellRangeAddress region = new CellRangeAddress(row.RowNum, row.RowNum + (mergedRegion.LastRow - mergedRegion.FirstRow), mergedRegion.FirstColumn, mergedRegion.LastColumn);
				sheet.AddMergedRegion(region);
			}
		}
		return row;
	}

	public static double GetCellWidth(ICell cell, int defaultCharWidth, DataFormatter formatter, bool useMergedCells)
	{
		ISheet sheet = cell.Sheet;
		IWorkbook workbook = sheet.Workbook;
		IRow row = cell.Row;
		int columnIndex = cell.ColumnIndex;
		int colspan = 1;
		for (int i = 0; i < sheet.NumMergedRegions; i++)
		{
			CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);
			if (ContainsCell(mergedRegion, row.RowNum, columnIndex))
			{
				if (!useMergedCells)
				{
					return -1.0;
				}
				cell = row.GetCell(mergedRegion.FirstColumn);
				colspan = 1 + mergedRegion.LastColumn - mergedRegion.FirstColumn;
			}
		}
		ICellStyle cellStyle = cell.CellStyle;
		CellType cellType = cell.CellType;
		Font font = IFont2Font(workbook.GetFontAt(0));
		if (cellType == CellType.Formula)
		{
			cellType = cell.CachedFormulaResultType;
		}
		IFont fontAt = workbook.GetFontAt(cellStyle.FontIndex);
		double num = -1.0;
		using (Bitmap image = new Bitmap(1, 1))
		{
			using Graphics g = Graphics.FromImage(image);
			if (cellType == CellType.String)
			{
				IRichTextString richStringCellValue = cell.RichStringCellValue;
				string[] array = richStringCellValue.String.Split("\n".ToCharArray());
				for (int j = 0; j < array.Length; j++)
				{
					string str = array[j] + defaultChar;
					font = IFont2Font(fontAt);
					_ = richStringCellValue.NumFormattingRuns;
					_ = 0;
					num = GetCellWidth(defaultCharWidth, colspan, cellStyle, num, str, g, font, cell);
				}
			}
			else
			{
				string text = null;
				switch (cellType)
				{
				case CellType.Numeric:
					try
					{
						text = formatter.FormatCellValue(cell, dummyEvaluator);
					}
					catch (Exception)
					{
						text = cell.NumericCellValue.ToString();
					}
					break;
				case CellType.Boolean:
					text = cell.BooleanCellValue.ToString().ToUpper();
					break;
				}
				if (text != null)
				{
					string str2 = text + defaultChar;
					font = IFont2Font(fontAt);
					num = GetCellWidth(defaultCharWidth, colspan, cellStyle, num, str2, g, font, cell);
				}
			}
		}
		return num;
	}

	private static double GetCellWidth(int defaultCharWidth, int colspan, ICellStyle style, double width, string str, Graphics g, Font windowsFont, ICell cell)
	{
		double num4;
		if (style.Rotation != 0)
		{
			double num = (double)style.Rotation * 2.0 * Math.PI / 360.0;
			SizeF sizeF = g.MeasureString(str, windowsFont);
			double num2 = Math.Abs((double)sizeF.Height * Math.Sin(num));
			double num3 = Math.Abs((double)sizeF.Width * Math.Cos(num));
			num4 = Math.Round(num2 + num3, 0, MidpointRounding.ToEven);
		}
		else
		{
			num4 = Math.Round(g.MeasureString(str, windowsFont, int.MaxValue, StringFormat.GenericTypographic).Width, 0, MidpointRounding.ToEven);
		}
		width = Math.Max(width, num4 / (double)colspan / (double)defaultCharWidth + (double)cell.CellStyle.Indention);
		return width;
	}

	public static double GetColumnWidth(ISheet sheet, int column, bool useMergedCells)
	{
		return GetColumnWidth(sheet, column, useMergedCells, sheet.FirstRowNum, sheet.LastRowNum);
	}

	public static double GetColumnWidth(ISheet sheet, int column, bool useMergedCells, int firstRow, int lastRow, int maxRows = 0)
	{
		DataFormatter formatter = new DataFormatter();
		int defaultCharWidth = GetDefaultCharWidth(sheet.Workbook);
		if (maxRows > 0 && lastRow - firstRow > maxRows)
		{
			lastRow = firstRow + maxRows;
		}
		double num = -1.0;
		for (int i = firstRow; i <= lastRow; i++)
		{
			IRow row = sheet.GetRow(i);
			if (row != null)
			{
				double columnWidthForRow = GetColumnWidthForRow(row, column, defaultCharWidth, formatter, useMergedCells);
				num = Math.Max(num, columnWidthForRow);
			}
		}
		return num;
	}

	public static int GetDefaultCharWidth(IWorkbook wb)
	{
		IFont fontAt = wb.GetFontAt(0);
		int num = 0;
		Font font = IFont2Font(fontAt);
		using Bitmap image = new Bitmap(1, 1);
		using Graphics graphics = Graphics.FromImage(image);
		graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
		return (int)graphics.MeasureString(new string(defaultChar, 1), font, int.MaxValue, StringFormat.GenericTypographic).Width;
	}

	private static double GetColumnWidthForRow(IRow row, int column, int defaultCharWidth, DataFormatter formatter, bool useMergedCells)
	{
		if (row == null)
		{
			return -1.0;
		}
		ICell cell = row.GetCell(column);
		if (cell == null)
		{
			return -1.0;
		}
		return GetCellWidth(cell, defaultCharWidth, formatter, useMergedCells);
	}

	public static bool CanComputeColumnWidth(IFont font)
	{
		return true;
	}

	internal static Font IFont2Font(IFont font1)
	{
		FontStyle fontStyle = FontStyle.Regular;
		if (font1.IsBold)
		{
			fontStyle |= FontStyle.Bold;
		}
		if (font1.IsItalic)
		{
			fontStyle |= FontStyle.Italic;
		}
		if (font1.Underline == FontUnderlineType.Single)
		{
			fontStyle |= FontStyle.Underline;
		}
		return new Font(font1.FontName, (float)font1.FontHeightInPoints, fontStyle, GraphicsUnit.Point);
	}

	[Obsolete("deprecated 3.15 beta 2. Use {@link CellRangeAddressBase#isInRange(int, int)}.")]
	public static bool ContainsCell(CellRangeAddress cr, int rowIx, int colIx)
	{
		return cr.IsInRange(rowIx, colIx);
	}

	public static string GetUniqueSheetName(IWorkbook wb, string srcName)
	{
		if (wb.GetSheetIndex(srcName) == -1)
		{
			return srcName;
		}
		int num = 2;
		string text = srcName;
		int num2 = srcName.LastIndexOf('(');
		if (num2 > 0 && srcName.EndsWith(")"))
		{
			string text2 = srcName.Substring(num2 + 1, srcName.Length - num2 - 2);
			try
			{
				num = int.Parse(text2.Trim());
				num++;
				text = srcName.Substring(0, num2).Trim();
			}
			catch (FormatException)
			{
			}
		}
		string text4;
		do
		{
			string text3 = num++.ToString();
			text4 = ((text.Length + text3.Length + 2 >= 31) ? (text.Substring(0, 31 - text3.Length - 2) + "(" + text3 + ")") : (text + " (" + text3 + ")"));
		}
		while (wb.GetSheetIndex(text4) != -1);
		return text4;
	}

	public static ICell GetCellWithMerges(ISheet sheet, int rowIx, int colIx)
	{
		IRow row = sheet.GetRow(rowIx);
		if (row != null)
		{
			ICell cell = row.GetCell(colIx);
			if (cell != null)
			{
				return cell;
			}
		}
		for (int i = 0; i < sheet.NumMergedRegions; i++)
		{
			CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);
			if (mergedRegion.IsInRange(rowIx, colIx))
			{
				row = sheet.GetRow(mergedRegion.FirstRow);
				if (row != null)
				{
					return row.GetCell(mergedRegion.FirstColumn);
				}
			}
		}
		return null;
	}
}
