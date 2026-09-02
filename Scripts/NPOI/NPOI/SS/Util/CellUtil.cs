using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Util;

public class CellUtil
{
	private class UnicodeMapping
	{
		public string entityName;

		public string resolvedValue;

		public UnicodeMapping(string pEntityName, string pResolvedValue)
		{
			entityName = "&" + pEntityName + ";";
			resolvedValue = pResolvedValue;
		}
	}

	public const string ALIGNMENT = "alignment";

	public const string BORDER_BOTTOM = "borderBottom";

	public const string BORDER_DIAGONAL = "borderDiagonal";

	public const string BORDER_LEFT = "borderLeft";

	public const string BORDER_RIGHT = "borderRight";

	public const string BORDER_TOP = "borderTop";

	public const string BOTTOM_BORDER_COLOR = "bottomBorderColor";

	public const string DATA_FORMAT = "dataFormat";

	public const string FILL_BACKGROUND_COLOR = "fillBackgroundColor";

	public const string FILL_FOREGROUND_COLOR = "fillForegroundColor";

	public const string FILL_PATTERN = "fillPattern";

	public const string FONT = "font";

	public const string HIDDEN = "hidden";

	public const string INDENTION = "indention";

	public const string LEFT_BORDER_COLOR = "leftBorderColor";

	public const string LOCKED = "locked";

	public const string RIGHT_BORDER_COLOR = "rightBorderColor";

	public const string ROTATION = "rotation";

	public const string SHRINK_TO_FIT = "shrinkToFit";

	public const string TOP_BORDER_COLOR = "topBorderColor";

	public const string VERTICAL_ALIGNMENT = "verticalAlignment";

	public const string WRAP_TEXT = "wrapText";

	private static ISet<string> shortValues;

	private static ISet<string> intValues;

	private static ISet<string> booleanValues;

	private static ISet<string> borderTypeValues;

	private static UnicodeMapping[] unicodeMappings;

	private CellUtil()
	{
	}

	public static ICell CopyCell(IRow row, int sourceIndex, int targetIndex)
	{
		if (sourceIndex == targetIndex)
		{
			throw new ArgumentException("sourceIndex and targetIndex cannot be same");
		}
		ICell cell = row.GetCell(sourceIndex);
		if (cell == null)
		{
			return null;
		}
		ICell cell2 = row.GetCell(targetIndex);
		if (cell2 == null)
		{
			cell2 = row.CreateCell(targetIndex);
		}
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
		return cell2;
	}

	public static IRow GetRow(int rowIndex, ISheet sheet)
	{
		IRow row = sheet.GetRow(rowIndex);
		if (row == null)
		{
			row = sheet.CreateRow(rowIndex);
		}
		return row;
	}

	public static ICell GetCell(IRow row, int columnIndex)
	{
		ICell cell = row.GetCell(columnIndex);
		if (cell == null)
		{
			cell = row.CreateCell(columnIndex);
		}
		return cell;
	}

	public static ICell CreateCell(IRow row, int column, string value, ICellStyle style)
	{
		ICell cell = GetCell(row, column);
		cell.SetCellValue(cell.Row.Sheet.Workbook.GetCreationHelper().CreateRichTextString(value));
		if (style != null)
		{
			cell.CellStyle = style;
		}
		return cell;
	}

	public static ICell CreateCell(IRow row, int column, string value)
	{
		return CreateCell(row, column, value, null);
	}

	[Obsolete("deprecated 3.15-beta2. Use {@link #SetAlignment(ICell, HorizontalAlignment)} instead.")]
	public static void SetAlignment(ICell cell, IWorkbook workbook, short align)
	{
		SetCellStyleProperty(cell, workbook, "alignment", align);
	}

	public static void SetAlignment(ICell cell, HorizontalAlignment align)
	{
		SetCellStyleProperty(cell, "alignment", align);
	}

	public static void SetVerticalAlignment(ICell cell, VerticalAlignment align)
	{
		SetCellStyleProperty(cell, "verticalAlignment", align);
	}

	[Obsolete("deprecated 3.15-beta2. Use {@link #SetFont(ICell, IFont)} instead.")]
	public static void SetFont(ICell cell, IWorkbook workbook, IFont font)
	{
		short index = font.Index;
		if (!workbook.GetFontAt(index).Equals(font))
		{
			throw new ArgumentException("Font does not belong to this workbook");
		}
		SetCellStyleProperty(cell, workbook, "font", index);
	}

	public static void SetFont(ICell cell, IFont font)
	{
		IWorkbook workbook = cell.Sheet.Workbook;
		short index = font.Index;
		if (!workbook.GetFontAt(index).Equals(font))
		{
			throw new ArgumentException("Font does not belong to this workbook");
		}
		SetCellStyleProperty(cell, "font", index);
	}

	public static void SetCellStyleProperties(ICell cell, Dictionary<string, object> properties)
	{
		IWorkbook workbook = cell.Sheet.Workbook;
		ICellStyle cellStyle = cell.CellStyle;
		ICellStyle cellStyle2 = null;
		Dictionary<string, object> formatProperties = GetFormatProperties(cellStyle);
		PutAll(properties, formatProperties);
		int numCellStyles = workbook.NumCellStyles;
		for (int i = 0; i < numCellStyles; i++)
		{
			ICellStyle cellStyleAt = workbook.GetCellStyleAt(i);
			if (DictionaryEqual(GetFormatProperties(cellStyleAt), formatProperties, null))
			{
				cellStyle2 = cellStyleAt;
				break;
			}
		}
		if (cellStyle2 == null)
		{
			cellStyle2 = workbook.CreateCellStyle();
			cellStyle2.CloneStyleFrom(cellStyle);
			SetFormatProperties(cellStyle2, workbook, formatProperties);
		}
		cell.CellStyle = cellStyle2;
	}

	public static bool DictionaryEqual<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, IEqualityComparer<TValue> valueComparer)
	{
		if (first == second)
		{
			return true;
		}
		if (first == null || second == null)
		{
			return false;
		}
		if (first.Count != second.Count)
		{
			return false;
		}
		valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		foreach (KeyValuePair<TKey, TValue> item in first)
		{
			if (!second.TryGetValue(item.Key, out var value))
			{
				return false;
			}
			if (!valueComparer.Equals(item.Value, value))
			{
				return false;
			}
		}
		return true;
	}

	public static void SetCellStyleProperty(ICell cell, string propertyName, object propertyValue)
	{
		Dictionary<string, object> properties = new Dictionary<string, object> { { propertyName, propertyValue } };
		SetCellStyleProperties(cell, properties);
	}

	[Obsolete("deprecated 3.15-beta2. Use {@link #setCellStyleProperty(Cell, String, Object)} instead.")]
	public static void SetCellStyleProperty(ICell cell, IWorkbook workbook, string propertyName, object propertyValue)
	{
		if (cell.Sheet.Workbook != workbook)
		{
			throw new ArgumentException("Cannot set cell style property. Cell does not belong to workbook.");
		}
		Dictionary<string, object> properties = new Dictionary<string, object> { { propertyName, propertyValue } };
		SetCellStyleProperties(cell, properties);
	}

	private static void PutAll(Dictionary<string, object> src, Dictionary<string, object> dest)
	{
		foreach (string key in src.Keys)
		{
			if (shortValues.Contains(key))
			{
				dest[key] = GetShort(src, key);
			}
			else if (intValues.Contains(key))
			{
				dest[key] = GetInt(src, key);
			}
			else if (booleanValues.Contains(key))
			{
				dest[key] = GetBoolean(src, key);
			}
			else if (borderTypeValues.Contains(key))
			{
				dest[key] = GetBorderStyle(src, key);
			}
			else if ("alignment".Equals(key))
			{
				dest[key] = GetHorizontalAlignment(src, key);
			}
			else if ("verticalAlignment".Equals(key))
			{
				dest[key] = GetVerticalAlignment(src, key);
			}
			else if ("fillPattern".Equals(key))
			{
				dest[key] = GetFillPattern(src, key);
			}
		}
	}

	private static Dictionary<string, object> GetFormatProperties(ICellStyle style)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Put(dictionary, "alignment", style.Alignment);
		Put(dictionary, "verticalAlignment", style.VerticalAlignment);
		Put(dictionary, "borderBottom", style.BorderBottom);
		Put(dictionary, "borderLeft", style.BorderLeft);
		Put(dictionary, "borderRight", style.BorderRight);
		Put(dictionary, "borderTop", style.BorderTop);
		Put(dictionary, "bottomBorderColor", style.BottomBorderColor);
		Put(dictionary, "dataFormat", style.DataFormat);
		Put(dictionary, "fillPattern", style.FillPattern);
		Put(dictionary, "fillForegroundColor", style.FillForegroundColor);
		Put(dictionary, "fillBackgroundColor", style.FillBackgroundColor);
		Put(dictionary, "font", (int)style.FontIndex);
		Put(dictionary, "hidden", style.IsHidden);
		Put(dictionary, "indention", style.Indention);
		Put(dictionary, "leftBorderColor", style.LeftBorderColor);
		Put(dictionary, "locked", style.IsLocked);
		Put(dictionary, "rightBorderColor", style.RightBorderColor);
		Put(dictionary, "rotation", style.Rotation);
		Put(dictionary, "topBorderColor", style.TopBorderColor);
		Put(dictionary, "wrapText", style.WrapText);
		return dictionary;
	}

	private static void SetFormatProperties(ICellStyle style, IWorkbook workbook, Dictionary<string, object> properties)
	{
		style.Alignment = GetHorizontalAlignment(properties, "alignment");
		style.VerticalAlignment = GetVerticalAlignment(properties, "verticalAlignment");
		style.BorderBottom = GetBorderStyle(properties, "borderBottom");
		style.BorderLeft = GetBorderStyle(properties, "borderLeft");
		style.BorderRight = GetBorderStyle(properties, "borderRight");
		style.BorderTop = GetBorderStyle(properties, "borderTop");
		style.BottomBorderColor = GetShort(properties, "bottomBorderColor");
		style.DataFormat = GetShort(properties, "dataFormat");
		style.FillPattern = GetFillPattern(properties, "fillPattern");
		style.FillForegroundColor = GetShort(properties, "fillForegroundColor");
		style.FillBackgroundColor = GetShort(properties, "fillBackgroundColor");
		style.SetFont(workbook.GetFontAt(GetShort(properties, "font")));
		style.IsHidden = GetBoolean(properties, "hidden");
		style.Indention = GetShort(properties, "indention");
		style.LeftBorderColor = GetShort(properties, "leftBorderColor");
		style.IsLocked = GetBoolean(properties, "locked");
		style.RightBorderColor = GetShort(properties, "rightBorderColor");
		style.Rotation = GetShort(properties, "rotation");
		style.TopBorderColor = GetShort(properties, "topBorderColor");
		style.WrapText = GetBoolean(properties, "wrapText");
	}

	private static short GetShort(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		short result = 0;
		if (short.TryParse(obj.ToString(), out result))
		{
			return result;
		}
		return 0;
	}

	private static int GetInt(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		if (Number.IsNumber(obj))
		{
			return int.Parse(obj.ToString());
		}
		return 0;
	}

	private static BorderStyle GetBorderStyle(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		if (!(obj is BorderStyle result))
		{
			if (obj is short || obj is int)
			{
				return (BorderStyle)short.Parse(obj.ToString());
			}
			if (obj == null)
			{
				return BorderStyle.None;
			}
			throw new RuntimeException("Unexpected border style class. Must be BorderStyle or Short (deprecated).");
		}
		return result;
	}

	private static FillPattern GetFillPattern(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		if (!(obj is FillPattern result))
		{
			if (!(obj is FillPattern result2))
			{
				if (obj == null)
				{
					return FillPattern.NoFill;
				}
				throw new RuntimeException("Unexpected fill pattern style class. Must be FillPattern or Short (deprecated).");
			}
			return result2;
		}
		return result;
	}

	private static HorizontalAlignment GetHorizontalAlignment(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		if (!(obj is HorizontalAlignment result))
		{
			if (!(obj is HorizontalAlignment result2))
			{
				if (obj == null)
				{
					return HorizontalAlignment.General;
				}
				throw new RuntimeException("Unexpected horizontal alignment style class. Must be HorizontalAlignment or Short (deprecated).");
			}
			return result2;
		}
		return result;
	}

	private static VerticalAlignment GetVerticalAlignment(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		if (!(obj is VerticalAlignment result))
		{
			if (!(obj is VerticalAlignment result2))
			{
				if (obj == null)
				{
					return VerticalAlignment.Bottom;
				}
				throw new RuntimeException("Unexpected vertical alignment style class. Must be VerticalAlignment or Short (deprecated).");
			}
			return result2;
		}
		return result;
	}

	private static bool GetBoolean(Dictionary<string, object> properties, string name)
	{
		object obj = properties[name];
		bool result = false;
		if (bool.TryParse(obj.ToString(), out result))
		{
			return result;
		}
		return false;
	}

	private static void Put(Dictionary<string, object> properties, string name, object value)
	{
		properties[name] = value;
	}

	private static void PutShort(Dictionary<string, object> properties, string name, short value)
	{
		properties[name] = value;
	}

	private static void PutEnum(Dictionary<string, object> properties, string name, Enum value)
	{
		properties[name] = value;
	}

	private static void PutBoolean(Dictionary<string, object> properties, string name, bool value)
	{
		properties[name] = value;
	}

	public static ICell TranslateUnicodeValues(ICell cell)
	{
		string text = cell.RichStringCellValue.String;
		bool flag = false;
		string text2 = text.ToLower();
		UnicodeMapping[] array = unicodeMappings;
		foreach (UnicodeMapping unicodeMapping in array)
		{
			string entityName = unicodeMapping.entityName;
			if (text2.Contains(entityName))
			{
				text = text.Replace(entityName, unicodeMapping.resolvedValue);
				flag = true;
			}
		}
		if (flag)
		{
			cell.SetCellValue(cell.Row.Sheet.Workbook.GetCreationHelper().CreateRichTextString(text));
		}
		return cell;
	}

	static CellUtil()
	{
		shortValues = new HashSet<string>(new string[9] { "bottomBorderColor", "leftBorderColor", "rightBorderColor", "topBorderColor", "fillForegroundColor", "fillBackgroundColor", "indention", "dataFormat", "rotation" });
		intValues = new HashSet<string>(new string[1] { "font" });
		booleanValues = new HashSet<string>(new string[3] { "locked", "hidden", "wrapText" });
		borderTypeValues = new HashSet<string>(new string[4] { "borderBottom", "borderLeft", "borderRight", "borderTop" });
		unicodeMappings = new UnicodeMapping[15]
		{
			um("alpha", "α"),
			um("beta", "β"),
			um("gamma", "γ"),
			um("delta", "δ"),
			um("epsilon", "ε"),
			um("zeta", "ζ"),
			um("eta", "η"),
			um("theta", "θ"),
			um("iota", "ι"),
			um("kappa", "κ"),
			um("lambda", "λ"),
			um("mu", "μ"),
			um("nu", "ν"),
			um("xi", "ξ"),
			um("omicron", "ο")
		};
	}

	private static UnicodeMapping um(string entityName, string resolvedValue)
	{
		return new UnicodeMapping(entityName, resolvedValue);
	}
}
