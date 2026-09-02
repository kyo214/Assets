using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Roman : Fixed2ArgFunction
{
	public static int[] VALUES = new int[13]
	{
		1000, 900, 500, 400, 100, 90, 50, 40, 10, 9,
		5, 4, 1
	};

	public static string[] ROMAN = new string[13]
	{
		"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX",
		"V", "IV", "I"
	};

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE, ValueEval formVE)
	{
		int num = 0;
		try
		{
			num = OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(numberVE, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num < 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num > 3999)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num == 0)
		{
			return new StringEval("");
		}
		int num2 = 0;
		try
		{
			num2 = OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(formVE, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException)
		{
			return ErrorEval.NUM_ERROR;
		}
		if (num2 > 4 || num2 < 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		string text = integerToRoman(num);
		if (num2 == 0)
		{
			return new StringEval(text);
		}
		return new StringEval(MakeConcise(text, num2));
	}

	private string integerToRoman(int number)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 13; i++)
		{
			while (number >= VALUES[i])
			{
				number -= VALUES[i];
				stringBuilder.Append(ROMAN[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public string MakeConcise(string result, int form)
	{
		if (form > 0)
		{
			result = result.Replace("XLV", "VL");
			result = result.Replace("XCV", "VC");
			result = result.Replace("CDL", "LD");
			result = result.Replace("CML", "LM");
			result = result.Replace("CMVC", "LMVL");
		}
		if (form == 1)
		{
			result = result.Replace("CDXC", "LDXL");
			result = result.Replace("CDVC", "LDVL");
			result = result.Replace("CMXC", "LMXL");
			result = result.Replace("XCIX", "VCIV");
			result = result.Replace("XLIX", "VLIV");
		}
		if (form > 1)
		{
			result = result.Replace("XLIX", "IL");
			result = result.Replace("XCIX", "IC");
			result = result.Replace("CDXC", "XD");
			result = result.Replace("CDVC", "XDV");
			result = result.Replace("CDIC", "XDIX");
			result = result.Replace("LMVL", "XMV");
			result = result.Replace("CMIC", "XMIX");
			result = result.Replace("CMXC", "XM");
		}
		if (form > 2)
		{
			result = result.Replace("XDV", "VD");
			result = result.Replace("XDIX", "VDIV");
			result = result.Replace("XMV", "VM");
			result = result.Replace("XMIX", "VMIV");
		}
		if (form == 4)
		{
			result = result.Replace("VDIV", "ID");
			result = result.Replace("VMIV", "IM");
		}
		return result;
	}
}
