using System;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace NPOI.SS.Converter;

public class ExcelToHtmlUtils
{
	private const short EXCEL_COLUMN_WIDTH_FACTOR = 256;

	private const int UNIT_OFFSET_LENGTH = 7;

	public static void AppendAlign(StringBuilder style, HorizontalAlignment alignment)
	{
		switch (alignment)
		{
		case HorizontalAlignment.Center:
			style.Append("text-align: center; ");
			break;
		case HorizontalAlignment.CenterSelection:
			style.Append("text-align: center; ");
			break;
		case HorizontalAlignment.Justify:
			style.Append("text-align: justify; ");
			break;
		case HorizontalAlignment.Left:
			style.Append("text-align: left; ");
			break;
		case HorizontalAlignment.Right:
			style.Append("text-align: right; ");
			break;
		case HorizontalAlignment.General:
		case HorizontalAlignment.Fill:
			break;
		}
	}

	public static CellRangeAddress[][] BuildMergedRangesMap(ISheet sheet)
	{
		CellRangeAddress[][] array = new CellRangeAddress[1][];
		for (int i = 0; i < sheet.NumMergedRegions; i++)
		{
			CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);
			int num = mergedRegion.LastRow + 1;
			if (array.Length < num)
			{
				CellRangeAddress[][] array2 = new CellRangeAddress[num][];
				Array.Copy(array, 0, array2, 0, array.Length);
				array = array2;
			}
			for (int j = mergedRegion.FirstRow; j <= mergedRegion.LastRow; j++)
			{
				int num2 = mergedRegion.LastColumn + 1;
				CellRangeAddress[] array3 = array[j];
				if (array3 == null)
				{
					array3 = (array[j] = new CellRangeAddress[num2]);
				}
				else
				{
					int num3 = array3.Length;
					if (num3 < num2)
					{
						CellRangeAddress[] array4 = new CellRangeAddress[num2];
						Array.Copy(array3, 0, array4, 0, num3);
						array[j] = array4;
						array3 = array4;
					}
				}
				for (int k = mergedRegion.FirstColumn; k < mergedRegion.LastColumn + 1; k++)
				{
					array3[k] = mergedRegion;
				}
			}
		}
		return array;
	}

	public static string GetBorderStyle(BorderStyle xlsBorder)
	{
		switch (xlsBorder)
		{
		case BorderStyle.None:
			return "none";
		case BorderStyle.Dotted:
		case BorderStyle.Hair:
		case BorderStyle.DashDot:
		case BorderStyle.MediumDashDot:
		case BorderStyle.DashDotDot:
		case BorderStyle.MediumDashDotDot:
		case BorderStyle.SlantedDashDot:
			return "dotted";
		case BorderStyle.Dashed:
		case BorderStyle.MediumDashed:
			return "dashed";
		case BorderStyle.Double:
			return "double";
		default:
			return "solid";
		}
	}

	public static string GetBorderWidth(BorderStyle xlsBorder)
	{
		switch (xlsBorder)
		{
		case BorderStyle.MediumDashed:
		case BorderStyle.MediumDashDot:
		case BorderStyle.MediumDashDotDot:
			return "2pt";
		case BorderStyle.Thick:
			return "thick";
		default:
			return "thin";
		}
	}

	public static string GetColor(XSSFColor color)
	{
		StringBuilder stringBuilder = new StringBuilder(7);
		stringBuilder.Append('#');
		byte[] rGB = color.RGB;
		foreach (byte b in rGB)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		string text = stringBuilder.ToString();
		if (text.Equals("#ffffff"))
		{
			return "white";
		}
		if (text.Equals("#c0c0c0"))
		{
			return "silver";
		}
		if (text.Equals("#808080"))
		{
			return "gray";
		}
		if (text.Equals("#000000"))
		{
			return "black";
		}
		return text;
	}

	public static string GetColor(HSSFColor color)
	{
		StringBuilder stringBuilder = new StringBuilder(7);
		stringBuilder.Append('#');
		byte[] rGB = color.RGB;
		foreach (byte b in rGB)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		string text = stringBuilder.ToString();
		if (text.Equals("#ffffff"))
		{
			return "white";
		}
		if (text.Equals("#c0c0c0"))
		{
			return "silver";
		}
		if (text.Equals("#808080"))
		{
			return "gray";
		}
		if (text.Equals("#000000"))
		{
			return "black";
		}
		return text;
	}

	public static int GetColumnWidthInPx(int widthUnits)
	{
		int num = widthUnits / 256 * 7;
		int num2 = widthUnits % 256;
		return num + (int)Math.Round((float)num2 / 36.57143f);
	}

	public static CellRangeAddress GetMergedRange(CellRangeAddress[][] mergedRanges, int rowNumber, int columnNumber)
	{
		CellRangeAddress[] array = ((rowNumber < mergedRanges.Length) ? mergedRanges[rowNumber] : null);
		if (array == null || columnNumber >= array.Length)
		{
			return null;
		}
		return array[columnNumber];
	}

	public static HSSFWorkbook LoadXls(string xlsFile)
	{
		FileStream fileStream = File.Open(xlsFile, FileMode.Open);
		try
		{
			return new HSSFWorkbook(fileStream);
		}
		finally
		{
			fileStream?.Close();
			fileStream = null;
		}
	}
}
