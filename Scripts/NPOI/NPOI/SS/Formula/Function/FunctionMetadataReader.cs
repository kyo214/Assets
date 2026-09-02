using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace NPOI.SS.Formula.Function;

internal class FunctionMetadataReader
{
	private const string METADATA_FILE_NAME = "functionMetadata.txt";

	private const string ELLIPSIS = "...";

	private const string TAB_DELIM_PATTERN = "\\t";

	private const string SPACE_DELIM_PATTERN = "\\s";

	private static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];

	private static readonly string[] DIGIT_ENDING_FUNCTION_NAMES = new string[6] { "LOG10", "ATAN2", "DAYS360", "SUMXMY2", "SUMX2MY2", "SUMX2PY2" };

	private static List<string> DIGIT_ENDING_FUNCTION_NAMES_Set = new List<string>(DIGIT_ENDING_FUNCTION_NAMES);

	public static FunctionMetadataRegistry CreateRegistry()
	{
		using StreamReader streamReader = new StreamReader(typeof(FunctionMetadataReader).Assembly.GetManifestResourceStream("functionMetadata.txt"));
		FunctionDataBuilder functionDataBuilder = new FunctionDataBuilder(400);
		try
		{
			while (true)
			{
				string text = streamReader.ReadLine();
				if (text != null)
				{
					if (text.Length >= 1 && text[0] != '#' && text.Trim().Length >= 1)
					{
						ProcessLine(functionDataBuilder, text);
					}
					continue;
				}
				break;
			}
		}
		catch (IOException)
		{
			throw;
		}
		return functionDataBuilder.Build();
	}

	private static void ProcessLine(FunctionDataBuilder fdb, string line)
	{
		string[] array = new Regex("\\t").Split(line);
		if (array.Length != 8)
		{
			throw new Exception("Bad line format '" + line + "' - expected 8 data fields");
		}
		int functionIndex = ParseInt(array[0]);
		string functionName = array[1];
		int minParams = ParseInt(array[2]);
		int maxParams = ParseInt(array[3]);
		byte returnClassCode = ParseReturnTypeCode(array[4]);
		byte[] parameterClassCodes = ParseOperandTypeCodes(array[5]);
		bool hasFootnote = array[7].Length > 0;
		ValidateFunctionName(functionName);
		fdb.Add(functionIndex, functionName, minParams, maxParams, returnClassCode, parameterClassCodes, hasFootnote);
	}

	private static byte ParseReturnTypeCode(string code)
	{
		if (code.Length == 0)
		{
			return 0;
		}
		return ParseOperandTypeCode(code);
	}

	private static byte[] ParseOperandTypeCodes(string codes)
	{
		if (codes.Length < 1)
		{
			return EMPTY_BYTE_ARRAY;
		}
		if (IsDash(codes))
		{
			return EMPTY_BYTE_ARRAY;
		}
		string[] array = new Regex("\\s").Split(codes);
		int num = array.Length;
		if ("...".Equals(array[num - 1]))
		{
			num--;
		}
		byte[] array2 = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array2[i] = ParseOperandTypeCode(array[i]);
		}
		return array2;
	}

	private static bool IsDash(string codes)
	{
		if (codes.Length == 1 && codes[0] == '-')
		{
			return true;
		}
		return false;
	}

	private static byte ParseOperandTypeCode(string code)
	{
		if (code.Length != 1)
		{
			throw new Exception("Bad operand type code format '" + code + "' expected single char");
		}
		return code[0] switch
		{
			'V' => 32, 
			'R' => 0, 
			'A' => 64, 
			_ => throw new ArgumentException("Unexpected operand type code '" + code + "' (" + (int)code[0] + ")"), 
		};
	}

	private static void ValidateFunctionName(string functionName)
	{
		int num = functionName.Length - 1;
		if (char.IsDigit(functionName[num]))
		{
			while (num >= 0 && char.IsDigit(functionName[num]))
			{
				num--;
			}
			if (!DIGIT_ENDING_FUNCTION_NAMES_Set.Contains(functionName))
			{
				throw new Exception("Invalid function name '" + functionName + "' (is footnote number incorrectly Appended)");
			}
		}
	}

	private static int ParseInt(string valStr)
	{
		return int.Parse(valStr, CultureInfo.InvariantCulture);
	}
}
