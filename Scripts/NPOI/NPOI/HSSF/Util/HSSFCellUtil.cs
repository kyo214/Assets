using System;
using System.Collections.Generic;
using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.Util;

[Obsolete("deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil} instead.")]
public class HSSFCellUtil
{
	private HSSFCellUtil()
	{
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#getRow} instead.")]
	public static IRow GetRow(int rowIndex, HSSFSheet sheet)
	{
		return (HSSFRow)CellUtil.GetRow(rowIndex, sheet);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#getCell} instead.")]
	public static ICell GetCell(IRow row, int columnIndex)
	{
		return (HSSFCell)CellUtil.GetCell(row, columnIndex);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#createCell} instead.")]
	public static ICell CreateCell(IRow row, int column, string value, HSSFCellStyle style)
	{
		return (HSSFCell)CellUtil.CreateCell(row, column, value, style);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#createCell} instead.")]
	public static ICell CreateCell(IRow row, int column, string value)
	{
		return CreateCell(row, column, value, null);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#setAlignment} instead.")]
	public static void SetAlignment(ICell cell, HSSFWorkbook workbook, short align)
	{
		CellUtil.SetAlignment(cell, (HorizontalAlignment)align);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#setFont} instead.")]
	public static void SetFont(ICell cell, HSSFWorkbook workbook, HSSFFont font)
	{
		CellUtil.SetFont(cell, font);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#setCellStyleProperty} instead.")]
	public static void SetCellStyleProperty(ICell cell, HSSFWorkbook workbook, string propertyName, object propertyValue)
	{
		CellUtil.SetCellStyleProperty(cell, propertyName, propertyValue);
	}

	[Obsolete("@deprecated 3.15 beta2. Removed in 3.17. Use {@link org.apache.poi.ss.util.CellUtil#translateUnicodeValues} instead.")]
	public static ICell TranslateUnicodeValues(ICell cell)
	{
		CellUtil.TranslateUnicodeValues(cell);
		return cell;
	}

	private static void RemapCellStyle(HSSFCellStyle stylish, Dictionary<short, short> paletteMap)
	{
		if (paletteMap.ContainsKey(stylish.BorderDiagonalColor))
		{
			stylish.BorderDiagonalColor = paletteMap[stylish.BorderDiagonalColor];
		}
		if (paletteMap.ContainsKey(stylish.BottomBorderColor))
		{
			stylish.BottomBorderColor = paletteMap[stylish.BottomBorderColor];
		}
		if (paletteMap.ContainsKey(stylish.FillBackgroundColor))
		{
			stylish.FillBackgroundColor = paletteMap[stylish.FillBackgroundColor];
		}
		if (paletteMap.ContainsKey(stylish.FillForegroundColor))
		{
			stylish.FillForegroundColor = paletteMap[stylish.FillForegroundColor];
		}
		if (paletteMap.ContainsKey(stylish.LeftBorderColor))
		{
			stylish.LeftBorderColor = paletteMap[stylish.LeftBorderColor];
		}
		if (paletteMap.ContainsKey(stylish.RightBorderColor))
		{
			stylish.RightBorderColor = paletteMap[stylish.RightBorderColor];
		}
		if (paletteMap.ContainsKey(stylish.TopBorderColor))
		{
			stylish.TopBorderColor = paletteMap[stylish.TopBorderColor];
		}
	}

	public static void CopyCell(HSSFCell oldCell, HSSFCell newCell, IDictionary<int, HSSFCellStyle> styleMap, Dictionary<short, short> paletteMap, bool keepFormulas)
	{
		if (styleMap != null)
		{
			if (oldCell.CellStyle != null)
			{
				if (oldCell.Sheet.Workbook == newCell.Sheet.Workbook)
				{
					newCell.CellStyle = oldCell.CellStyle;
				}
				else
				{
					int hashCode = oldCell.CellStyle.GetHashCode();
					if (styleMap.ContainsKey(hashCode))
					{
						newCell.CellStyle = styleMap[hashCode];
					}
					else
					{
						HSSFCellStyle hSSFCellStyle = (HSSFCellStyle)newCell.Sheet.Workbook.CreateCellStyle();
						hSSFCellStyle.CloneStyleFrom(oldCell.CellStyle);
						RemapCellStyle(hSSFCellStyle, paletteMap);
						newCell.CellStyle = hSSFCellStyle;
						IFont font = hSSFCellStyle.GetFont(newCell.Sheet.Workbook);
						if (font.Color > 0 && paletteMap.ContainsKey(font.Color))
						{
							font.Color = paletteMap[font.Color];
						}
						styleMap.Add(hashCode, hSSFCellStyle);
					}
				}
			}
			else
			{
				newCell.CellStyle = null;
			}
		}
		switch (oldCell.CellType)
		{
		case CellType.String:
		{
			HSSFRichTextString hSSFRichTextString = oldCell.RichStringCellValue as HSSFRichTextString;
			newCell.SetCellValue(hSSFRichTextString);
			if (hSSFRichTextString != null)
			{
				for (int i = 0; i < hSSFRichTextString.NumFormattingRuns; i++)
				{
					short fontOfFormattingRun = hSSFRichTextString.GetFontOfFormattingRun(i);
					int indexOfFormattingRun = hSSFRichTextString.GetIndexOfFormattingRun(i);
					int num = 0;
					num = ((i + 1 != hSSFRichTextString.NumFormattingRuns) ? hSSFRichTextString.GetIndexOfFormattingRun(i + 1) : hSSFRichTextString.Length);
					FontRecord fontRecord = newCell.BoundWorkbook.CreateNewFont();
					fontRecord.CloneStyleFrom(oldCell.BoundWorkbook.GetFontRecordAt(fontOfFormattingRun));
					HSSFFont font2 = new HSSFFont((short)newCell.BoundWorkbook.GetFontIndex(fontRecord), fontRecord);
					newCell.RichStringCellValue.ApplyFont(indexOfFormattingRun, num, font2);
				}
			}
			break;
		}
		case CellType.Numeric:
			newCell.SetCellValue(oldCell.NumericCellValue);
			break;
		case CellType.Blank:
			newCell.SetCellType(CellType.Blank);
			break;
		case CellType.Boolean:
			newCell.SetCellValue(oldCell.BooleanCellValue);
			break;
		case CellType.Error:
			newCell.SetCellValue((int)oldCell.ErrorCellValue);
			break;
		case CellType.Formula:
			if (keepFormulas)
			{
				newCell.SetCellType(CellType.Formula);
				newCell.CellFormula = oldCell.CellFormula;
				break;
			}
			try
			{
				newCell.SetCellType(CellType.Numeric);
				newCell.SetCellValue(oldCell.NumericCellValue);
				break;
			}
			catch (Exception)
			{
				newCell.SetCellType(CellType.String);
				newCell.SetCellValue(oldCell.ToString());
				break;
			}
		}
	}
}
