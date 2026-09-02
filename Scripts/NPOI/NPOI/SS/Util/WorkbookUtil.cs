using System;
using System.Text;
using NPOI.SS.UserModel;

namespace NPOI.SS.Util;

public class WorkbookUtil
{
	public static string CreateSafeSheetName(string nameProposal)
	{
		return CreateSafeSheetName(nameProposal, ' ');
	}

	public static string CreateSafeSheetName(string nameProposal, char replaceChar)
	{
		if (nameProposal == null)
		{
			return "null";
		}
		if (nameProposal.Length < 1)
		{
			return "empty";
		}
		int num = Math.Min(31, nameProposal.Length);
		StringBuilder stringBuilder = new StringBuilder(nameProposal.Substring(0, num));
		for (int i = 0; i < num; i++)
		{
			switch (stringBuilder[i])
			{
			case '\0':
			case '\u0003':
			case '*':
			case '/':
			case ':':
			case '?':
			case '[':
			case '\\':
			case ']':
				stringBuilder[i] = replaceChar;
				break;
			case '\'':
				if (i == 0 || i == num - 1)
				{
					stringBuilder[i] = replaceChar;
				}
				break;
			}
		}
		return stringBuilder.ToString();
	}

	public static void ValidateSheetName(string sheetName)
	{
		if (sheetName == null)
		{
			throw new ArgumentException("sheetName must not be null");
		}
		int length = sheetName.Length;
		if (length < 1 || length > 31)
		{
			throw new ArgumentException("sheetName '" + sheetName + "' is invalid - character count MUST be greater than or equal to 1 and less than or equal to 31");
		}
		for (int i = 0; i < length; i++)
		{
			char c = sheetName[i];
			switch (c)
			{
			case '*':
			case '/':
			case ':':
			case '?':
			case '[':
			case '\\':
			case ']':
				throw new ArgumentException("Invalid char (" + c + ") found at index (" + i + ") in sheet name '" + sheetName + "'");
			}
		}
		if (sheetName[0] == '\'' || sheetName[length - 1] == '\'')
		{
			throw new ArgumentException("Invalid sheet name '" + sheetName + "'. Sheet names must not begin or end with (').");
		}
	}

	public static void ValidateSheetState(SheetState state)
	{
		switch (state)
		{
		case SheetState.Visible:
		case SheetState.Hidden:
		case SheetState.VeryHidden:
			return;
		}
		throw new ArgumentException("Ivalid sheet state : " + state.ToString() + "\nSheet state must beone of the Workbook.SHEET_STATE_* constants");
	}
}
