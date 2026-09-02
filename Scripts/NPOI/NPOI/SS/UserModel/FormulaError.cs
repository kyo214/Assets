using System;
using System.Collections.Generic;

namespace NPOI.SS.UserModel;

public class FormulaError
{
	private static FormulaError[] _values;

	internal static readonly FormulaError _NO_ERROR;

	public static readonly FormulaError NULL;

	public static readonly FormulaError DIV0;

	public static readonly FormulaError VALUE;

	public static readonly FormulaError REF;

	public static readonly FormulaError NAME;

	public static readonly FormulaError NUM;

	public static readonly FormulaError NA;

	public static readonly FormulaError CIRCULAR_REF;

	public static readonly FormulaError FUNCTION_NOT_IMPLEMENTED;

	private byte type;

	private int longType;

	private string repr;

	private static Dictionary<string, FormulaError> smap;

	private static Dictionary<byte, FormulaError> bmap;

	private static Dictionary<int, FormulaError> imap;

	public string Name { get; private set; }

	public byte Code => type;

	public int LongCode => longType;

	public string String => repr;

	static FormulaError()
	{
		_NO_ERROR = new FormulaError(-1, "(no error)", "_NO_ERROR");
		NULL = new FormulaError(0, "#NULL!", "NULL");
		DIV0 = new FormulaError(7, "#DIV/0!", "DIV0");
		VALUE = new FormulaError(15, "#VALUE!", "VALUE");
		REF = new FormulaError(23, "#REF!", "REF");
		NAME = new FormulaError(29, "#NAME?", "NAME");
		NUM = new FormulaError(36, "#NUM!", "NUM");
		NA = new FormulaError(42, "#N/A", "NA");
		CIRCULAR_REF = new FormulaError(-60, "~CIRCULAR~REF~", "CIRCULAR_REF");
		FUNCTION_NOT_IMPLEMENTED = new FormulaError(-30, "~FUNCTION~NOT~IMPLEMENTED~", "FUNCTION_NOT_IMPLEMENTED");
		smap = new Dictionary<string, FormulaError>();
		bmap = new Dictionary<byte, FormulaError>();
		imap = new Dictionary<int, FormulaError>();
		_values = new FormulaError[9] { NULL, DIV0, VALUE, REF, NAME, NUM, NA, CIRCULAR_REF, FUNCTION_NOT_IMPLEMENTED };
		FormulaError[] values = _values;
		foreach (FormulaError formulaError in values)
		{
			bmap.Add(formulaError.Code, formulaError);
			imap.Add(formulaError.LongCode, formulaError);
			smap.Add(formulaError.String, formulaError);
		}
	}

	private FormulaError(int type, string repr, string name)
	{
		this.type = (byte)type;
		longType = type;
		this.repr = repr;
		Name = name;
	}

	public override string ToString()
	{
		return Name;
	}

	public static bool IsValidCode(int errorCode)
	{
		FormulaError[] values = _values;
		foreach (FormulaError formulaError in values)
		{
			if (formulaError.Code == errorCode)
			{
				return true;
			}
			if (formulaError.LongCode == errorCode)
			{
				return true;
			}
		}
		return false;
	}

	public static FormulaError ForInt(byte type)
	{
		if (bmap.ContainsKey(type))
		{
			return bmap[type];
		}
		throw new ArgumentException("Unknown error type: " + type);
	}

	public static FormulaError ForInt(int type)
	{
		if (imap.ContainsKey(type))
		{
			return imap[type];
		}
		if (bmap.ContainsKey((byte)type))
		{
			return bmap[(byte)type];
		}
		throw new ArgumentException("Unknown error type: " + type);
	}

	public static FormulaError ForString(string code)
	{
		if (smap.ContainsKey(code))
		{
			return smap[code];
		}
		throw new ArgumentException("Unknown error code: " + code);
	}
}
