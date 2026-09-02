using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula;

public class SheetNameFormatter
{
	private const string BIFF8_LAST_COLUMN = "IV";

	private const int BIFF8_LAST_COLUMN_TEXT_LEN = 2;

	private static readonly string BIFF8_LAST_ROW = 65536.ToString(CultureInfo.InvariantCulture);

	private static readonly int BIFF8_LAST_ROW_TEXT_LEN = BIFF8_LAST_ROW.Length;

	private const char DELIMITER = '\'';

	private const string CELL_REF_PATTERN = "^([A-Za-z]+)([0-9]+)$";

	private SheetNameFormatter()
	{
	}

	public static string Format(string rawSheetName)
	{
		StringBuilder stringBuilder = new StringBuilder(rawSheetName.Length + 2);
		AppendFormat(stringBuilder, rawSheetName);
		return stringBuilder.ToString();
	}

	public static void AppendFormat(StringBuilder out1, string rawSheetName)
	{
		if (NeedsDelimiting(rawSheetName))
		{
			out1.Append('\'');
			AppendAndEscape(out1, rawSheetName);
			out1.Append('\'');
		}
		else
		{
			out1.Append(rawSheetName);
		}
	}

	public static void AppendFormat(StringBuilder out1, string workbookName, string rawSheetName)
	{
		if (NeedsDelimiting(workbookName) || NeedsDelimiting(rawSheetName))
		{
			out1.Append('\'');
			out1.Append('[');
			AppendAndEscape(out1, workbookName.Replace('[', '(').Replace(']', ')'));
			out1.Append(']');
			AppendAndEscape(out1, rawSheetName);
			out1.Append('\'');
		}
		else
		{
			out1.Append('[');
			out1.Append(workbookName);
			out1.Append(']');
			out1.Append(rawSheetName);
		}
	}

	private static void AppendAndEscape(StringBuilder sb, string rawSheetName)
	{
		int length = rawSheetName.Length;
		for (int i = 0; i < length; i++)
		{
			char c = rawSheetName[i];
			if (c == '\'')
			{
				sb.Append('\'');
			}
			sb.Append(c);
		}
	}

	private static bool NeedsDelimiting(string rawSheetName)
	{
		int length = rawSheetName.Length;
		if (length < 1)
		{
			throw new Exception("Zero Length string is an invalid sheet name");
		}
		if (char.IsDigit(rawSheetName[0]))
		{
			return true;
		}
		for (int i = 0; i < length; i++)
		{
			if (IsSpecialChar(rawSheetName[i]))
			{
				return true;
			}
		}
		if (char.IsLetter(rawSheetName[0]) && char.IsDigit(rawSheetName[length - 1]) && NameLooksLikePlainCellReference(rawSheetName))
		{
			return true;
		}
		if (NameLooksLikeBooleanLiteral(rawSheetName))
		{
			return true;
		}
		return false;
	}

	private static bool NameLooksLikeBooleanLiteral(string rawSheetName)
	{
		switch (rawSheetName[0])
		{
		case 'T':
		case 't':
			return "TRUE".Equals(rawSheetName, StringComparison.OrdinalIgnoreCase);
		case 'F':
		case 'f':
			return "FALSE".Equals(rawSheetName, StringComparison.OrdinalIgnoreCase);
		default:
			return false;
		}
	}

	private static bool IsSpecialChar(char ch)
	{
		if (char.IsLetterOrDigit(ch))
		{
			return false;
		}
		switch (ch)
		{
		case '.':
		case '_':
			return false;
		case '\t':
		case '\n':
		case '\r':
			throw new Exception("Illegal Char (0x" + StringUtil.ToHexString(ch) + ") found in sheet name");
		default:
			return true;
		}
	}

	public static bool CellReferenceIsWithinRange(string lettersPrefix, string numbersSuffix)
	{
		return CellReference.CellReferenceIsWithinRange(lettersPrefix, numbersSuffix, SpreadsheetVersion.EXCEL97);
	}

	public static bool NameLooksLikePlainCellReference(string rawSheetName)
	{
		Regex regex = new Regex("^([A-Za-z]+)([0-9]+)$");
		if (!regex.IsMatch(rawSheetName))
		{
			return false;
		}
		Match match = regex.Matches(rawSheetName)[0];
		string value = match.Groups[1].Value;
		string value2 = match.Groups[2].Value;
		return CellReferenceIsWithinRange(value, value2);
	}
}
