using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Format;

public class CellFormat
{
	public class GeneralCellFormat : CellFormat
	{
		public GeneralCellFormat()
			: base("General")
		{
		}

		public override CellFormatResult Apply(object value)
		{
			string text = new CellGeneralFormatter().Format(value);
			return new CellFormatResult(applies: true, text, Color.Empty);
		}
	}

	private string format;

	private CellFormatPart posNumFmt;

	private CellFormatPart zeroNumFmt;

	private CellFormatPart negNumFmt;

	private CellFormatPart textFmt;

	private int formatPartCount;

	private static readonly Regex ONE_PART = new Regex(CellFormatPart.FORMAT_PAT.ToString() + "(;|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

	private static readonly CellFormatPart DEFAULT_TEXT_FORMAT = new CellFormatPart("@");

	private const string INVALID_VALUE_FOR_FORMAT = "###############################################################################################################################################################################################################################################################";

	private const string QUOTE = "\"";

	private static readonly CellFormat GENERAL_FORMAT = new GeneralCellFormat();

	private static Dictionary<string, CellFormat> formatCache = new Dictionary<string, CellFormat>();

	public static CellFormat GetInstance(string format)
	{
		CellFormat cellFormat = null;
		if (formatCache.ContainsKey(format))
		{
			cellFormat = formatCache[format];
		}
		if (cellFormat == null)
		{
			cellFormat = ((!format.Equals("General") && !format.Equals("@")) ? new CellFormat(format) : GENERAL_FORMAT);
			formatCache.Add(format, cellFormat);
		}
		return cellFormat;
	}

	private CellFormat(string format)
	{
		this.format = format;
		MatchCollection matchCollection = ONE_PART.Matches(format);
		List<CellFormatPart> list = new List<CellFormatPart>();
		foreach (Match item in matchCollection)
		{
			try
			{
				string text = item.Groups[0].Value;
				if (text.EndsWith(";"))
				{
					text = text.Substring(0, text.Length - 1);
				}
				list.Add(new CellFormatPart(text));
			}
			catch (Exception)
			{
				list.Add(null);
			}
		}
		formatPartCount = list.Count;
		switch (formatPartCount)
		{
		case 1:
			posNumFmt = list[0];
			negNumFmt = null;
			zeroNumFmt = null;
			textFmt = DEFAULT_TEXT_FORMAT;
			break;
		case 2:
			posNumFmt = list[0];
			negNumFmt = list[1];
			zeroNumFmt = null;
			textFmt = DEFAULT_TEXT_FORMAT;
			break;
		case 3:
			posNumFmt = list[0];
			negNumFmt = list[1];
			zeroNumFmt = list[2];
			textFmt = DEFAULT_TEXT_FORMAT;
			break;
		default:
			posNumFmt = list[0];
			negNumFmt = list[1];
			zeroNumFmt = list[2];
			textFmt = list[3];
			break;
		}
	}

	public virtual CellFormatResult Apply(object value)
	{
		if (Number.IsNumber(value))
		{
			double.TryParse(value.ToString(), out var result);
			if (result < 0.0 && ((formatPartCount == 2 && !posNumFmt.HasCondition && !negNumFmt.HasCondition) || (formatPartCount == 3 && !negNumFmt.HasCondition) || (formatPartCount == 4 && !negNumFmt.HasCondition)))
			{
				return negNumFmt.Apply(0.0 - result);
			}
			return GetApplicableFormatPart(result).Apply(result);
		}
		if (value is DateTime)
		{
			double excelDate = DateUtil.GetExcelDate((DateTime)value);
			if (DateUtil.IsValidExcelDate(excelDate))
			{
				return GetApplicableFormatPart(excelDate).Apply(value);
			}
			throw new ArgumentException("value " + excelDate + " of date " + value?.ToString() + " is not a valid Excel date");
		}
		return textFmt.Apply(value);
	}

	private CellFormatResult Apply(DateTime date, double numericValue)
	{
		return GetApplicableFormatPart(numericValue).Apply(date);
	}

	public CellFormatResult Apply(ICell c)
	{
		switch (UltimateType(c))
		{
		case CellType.Blank:
			return Apply("");
		case CellType.Boolean:
			return Apply(c.BooleanCellValue);
		case CellType.Numeric:
		{
			double numericCellValue = c.NumericCellValue;
			if (GetApplicableFormatPart(numericCellValue).CellFormatType == CellFormatType.DATE)
			{
				if (DateUtil.IsValidExcelDate(numericCellValue))
				{
					return Apply(c.DateCellValue, numericCellValue);
				}
				return Apply("###############################################################################################################################################################################################################################################################");
			}
			return Apply(numericCellValue);
		}
		case CellType.String:
			return Apply(c.StringCellValue);
		default:
			return Apply("?");
		}
	}

	private CellFormatPart GetApplicableFormatPart(object value)
	{
		if (Number.IsNumber(value))
		{
			double.TryParse(value.ToString(), out var result);
			if (formatPartCount == 1)
			{
				if (!posNumFmt.HasCondition || (posNumFmt.HasCondition && posNumFmt.Applies(result)))
				{
					return posNumFmt;
				}
				return new CellFormatPart("General");
			}
			if (formatPartCount == 2)
			{
				if ((!posNumFmt.HasCondition && result >= 0.0) || (posNumFmt.HasCondition && posNumFmt.Applies(result)))
				{
					return posNumFmt;
				}
				if (!negNumFmt.HasCondition || (negNumFmt.HasCondition && negNumFmt.Applies(result)))
				{
					return negNumFmt;
				}
				return new CellFormatPart("\"###############################################################################################################################################################################################################################################################\"");
			}
			if ((!posNumFmt.HasCondition && result > 0.0) || (posNumFmt.HasCondition && posNumFmt.Applies(result)))
			{
				return posNumFmt;
			}
			if ((!negNumFmt.HasCondition && result < 0.0) || (negNumFmt.HasCondition && negNumFmt.Applies(result)))
			{
				return negNumFmt;
			}
			return zeroNumFmt;
		}
		throw new ArgumentException("value must be a Number");
	}

	public static CellType UltimateType(ICell cell)
	{
		CellType cellType = cell.CellType;
		if (cellType == CellType.Formula)
		{
			return cell.CachedFormulaResultType;
		}
		return cellType;
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj is CellFormat)
		{
			CellFormat cellFormat = (CellFormat)obj;
			return format.Equals(cellFormat.format);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return format.GetHashCode();
	}

	public override string ToString()
	{
		return format;
	}
}
