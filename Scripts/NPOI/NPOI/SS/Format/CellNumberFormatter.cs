using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Format;

public class CellNumberFormatter : CellFormatter
{
	private class GeneralNumberFormatter : CellFormatter
	{
		private GeneralNumberFormatter()
			: base("General")
		{
		}

		public override void FormatValue(StringBuilder toAppendTo, object value)
		{
			if (value != null)
			{
				CellFormatter cellFormatter;
				if (Number.IsNumber(value))
				{
					double.TryParse(value.ToString(), out var result);
					cellFormatter = ((result % 1.0 == 0.0) ? SIMPLE_INT : SIMPLE_FLOAT);
				}
				else
				{
					cellFormatter = CellTextFormatter.SIMPLE_TEXT;
				}
				cellFormatter.FormatValue(toAppendTo, value);
			}
		}

		public override void SimpleValue(StringBuilder toAppendTo, object value)
		{
			FormatValue(toAppendTo, value);
		}
	}

	private class SimpleNumberCellFormatter : CellFormatter
	{
		public SimpleNumberCellFormatter(string format)
			: base(format)
		{
		}

		public override void FormatValue(StringBuilder toAppendTo, object value)
		{
			if (value == null)
			{
				return;
			}
			if (Number.IsNumber(value))
			{
				double.TryParse(value.ToString(), out var result);
				if (result % 1.0 == 0.0)
				{
					SIMPLE_INT.FormatValue(toAppendTo, value);
				}
				else
				{
					SIMPLE_FLOAT.FormatValue(toAppendTo, value);
				}
			}
			else
			{
				CellTextFormatter.SIMPLE_TEXT.FormatValue(toAppendTo, value);
			}
		}

		public override void SimpleValue(StringBuilder toAppendTo, object value)
		{
			FormatValue(toAppendTo, value);
		}
	}

	public class Special
	{
		internal char ch;

		internal int pos;

		public Special(char ch, int pos)
		{
			this.ch = ch;
			this.pos = pos;
		}

		public override string ToString()
		{
			return "'" + ch + "' @ " + pos;
		}
	}

	private string desc;

	private string printfFmt;

	private double scale;

	private Special decimalPoint;

	private Special slash;

	private Special exponent;

	private Special numerator;

	private Special afterInteger;

	private Special afterFractional;

	private bool integerCommas;

	private List<Special> specials = new List<Special>();

	private List<Special> integerSpecials = new List<Special>();

	private List<Special> fractionalSpecials = new List<Special>();

	private List<Special> numeratorSpecials = new List<Special>();

	private List<Special> denominatorSpecials = new List<Special>();

	private List<Special> exponentSpecials = new List<Special>();

	private List<Special> exponentDigitSpecials = new List<Special>();

	private int maxDenominator;

	private string numeratorFmt;

	private string denominatorFmt;

	private bool improperFraction;

	private DecimalFormat decimalFmt;

	private static List<Special> EmptySpecialList = new List<Special>();

	private static readonly CellFormatter SIMPLE_NUMBER = new SimpleNumberCellFormatter("General");

	private static readonly CellFormatter SIMPLE_INT = new CellNumberFormatter("#");

	private static readonly CellFormatter SIMPLE_FLOAT = new CellNumberFormatter("#.#");

	public CellNumberFormatter(string format)
		: base(format)
	{
		CellNumberPartHandler cellNumberPartHandler = new CellNumberPartHandler();
		StringBuilder stringBuilder = CellFormatPart.ParseFormat(format, CellFormatType.NUMBER, cellNumberPartHandler);
		exponent = cellNumberPartHandler.Exponent;
		specials.AddRange(cellNumberPartHandler.Specials);
		improperFraction = cellNumberPartHandler.IsImproperFraction;
		if ((cellNumberPartHandler.DecimalPoint != null || cellNumberPartHandler.Exponent != null) && cellNumberPartHandler.Slash != null)
		{
			slash = null;
			numerator = null;
		}
		else
		{
			slash = cellNumberPartHandler.Slash;
			numerator = cellNumberPartHandler.Numerator;
		}
		int num = interpretPrecision(cellNumberPartHandler.DecimalPoint, specials);
		if (cellNumberPartHandler.DecimalPoint != null)
		{
			if (num == 0)
			{
				specials.Remove(cellNumberPartHandler.DecimalPoint);
				decimalPoint = null;
			}
			else
			{
				decimalPoint = cellNumberPartHandler.DecimalPoint;
			}
		}
		else
		{
			decimalPoint = null;
		}
		if (decimalPoint != null)
		{
			afterInteger = decimalPoint;
		}
		else if (exponent != null)
		{
			afterInteger = exponent;
		}
		else if (numerator != null)
		{
			afterInteger = numerator;
		}
		else
		{
			afterInteger = null;
		}
		if (exponent != null)
		{
			afterFractional = exponent;
		}
		else if (numerator != null)
		{
			afterFractional = numerator;
		}
		else
		{
			afterFractional = null;
		}
		double[] array = new double[1] { cellNumberPartHandler.Scale };
		integerCommas = interpretIntegerCommas(stringBuilder, specials, decimalPoint, integerEnd(), fractionalEnd(), array);
		if (exponent == null)
		{
			scale = array[0];
		}
		else
		{
			scale = 1.0;
		}
		if (num != 0)
		{
			int num2 = specials.IndexOf(decimalPoint) + 1;
			fractionalSpecials.AddRange(specials.GetRange(num2, fractionalEnd() - num2));
		}
		if (exponent != null)
		{
			int num3 = specials.IndexOf(exponent);
			exponentSpecials.AddRange(specialsFor(num3, 2));
			exponentDigitSpecials.AddRange(specialsFor(num3 + 2));
		}
		if (slash != null)
		{
			if (numerator != null)
			{
				numeratorSpecials.AddRange(specialsFor(specials.IndexOf(numerator)));
			}
			denominatorSpecials.AddRange(specialsFor(specials.IndexOf(slash) + 1));
			if (denominatorSpecials.Count == 0)
			{
				numeratorSpecials.Clear();
				maxDenominator = 1;
				numeratorFmt = null;
				denominatorFmt = null;
			}
			else
			{
				maxDenominator = maxValue(denominatorSpecials);
				numeratorFmt = SingleNumberFormat(numeratorSpecials);
				denominatorFmt = SingleNumberFormat(denominatorSpecials);
			}
		}
		else
		{
			maxDenominator = 1;
			numeratorFmt = null;
			denominatorFmt = null;
		}
		integerSpecials.AddRange(specials.GetRange(0, integerEnd()));
		if (exponent == null)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			int repeatCount = calculateintPartWidth();
			stringBuilder2.Append('0', repeatCount).Append('.').Append('0', num);
			printfFmt = stringBuilder2.ToString();
			decimalFmt = null;
		}
		else
		{
			StringBuilder stringBuilder3 = new StringBuilder();
			bool flag = true;
			List<Special> list = integerSpecials;
			if (integerSpecials.Count == 1)
			{
				stringBuilder3.Append("0");
				flag = false;
			}
			else
			{
				foreach (Special item in list)
				{
					if (IsDigitFmt(item))
					{
						stringBuilder3.Append(flag ? '#' : '0');
						flag = false;
					}
				}
			}
			if (fractionalSpecials.Count > 0)
			{
				stringBuilder3.Append('.');
				foreach (Special fractionalSpecial in fractionalSpecials)
				{
					if (IsDigitFmt(fractionalSpecial))
					{
						if (!flag)
						{
							stringBuilder3.Append('0');
						}
						flag = false;
					}
				}
			}
			stringBuilder3.Append('E');
			placeZeros(stringBuilder3, exponentSpecials.GetRange(2, exponentSpecials.Count - 2));
			decimalFmt = new DecimalFormat(stringBuilder3.ToString());
			printfFmt = null;
		}
		desc = stringBuilder.ToString();
	}

	private static void placeZeros(StringBuilder sb, List<Special> specials)
	{
		foreach (Special special in specials)
		{
			if (IsDigitFmt(special))
			{
				sb.Append('0');
			}
		}
	}

	private static Special firstDigit(List<Special> specials)
	{
		foreach (Special special in specials)
		{
			if (IsDigitFmt(special))
			{
				return special;
			}
		}
		return null;
	}

	private static CellNumberStringMod insertMod(Special special, string toAdd, int where)
	{
		return new CellNumberStringMod(special, toAdd, where);
	}

	private static CellNumberStringMod deleteMod(Special start, bool startInclusive, Special end, bool endInclusive)
	{
		return new CellNumberStringMod(start, startInclusive, end, endInclusive);
	}

	private static CellNumberStringMod ReplaceMod(Special start, bool startInclusive, Special end, bool endInclusive, char withChar)
	{
		return new CellNumberStringMod(start, startInclusive, end, endInclusive, withChar);
	}

	private static string SingleNumberFormat(List<Special> numSpecials)
	{
		return "D" + numSpecials.Count;
	}

	private static int maxValue(List<Special> s)
	{
		return (int)Math.Round(Math.Pow(10.0, s.Count) - 1.0);
	}

	private List<Special> specialsFor(int pos, int takeFirst)
	{
		if (pos >= specials.Count)
		{
			return EmptySpecialList;
		}
		IEnumerator<Special> enumerator = specials.GetRange(pos + takeFirst, specials.Count - pos - takeFirst).GetEnumerator();
		enumerator.MoveNext();
		Special special = enumerator.Current;
		int num = pos + takeFirst;
		while (enumerator.MoveNext())
		{
			Special current = enumerator.Current;
			if (!IsDigitFmt(current) || current.pos - special.pos > 1)
			{
				break;
			}
			num++;
			special = current;
		}
		return specials.GetRange(pos, num + 1 - pos);
	}

	private List<Special> specialsFor(int pos)
	{
		return specialsFor(pos, 0);
	}

	private static bool IsDigitFmt(Special s)
	{
		if (s.ch != '0' && s.ch != '?')
		{
			return s.ch == '#';
		}
		return true;
	}

	private int calculateintPartWidth()
	{
		int num = 0;
		foreach (Special special in specials)
		{
			if (special == afterInteger)
			{
				break;
			}
			if (IsDigitFmt(special))
			{
				num++;
			}
		}
		return num;
	}

	private static int interpretPrecision(Special decimalPoint, List<Special> specials)
	{
		int num = specials.IndexOf(decimalPoint);
		int num2 = 0;
		if (num != -1)
		{
			IEnumerator<Special> enumerator = specials.GetRange(num + 1, specials.Count - num - 1).GetEnumerator();
			while (enumerator.MoveNext() && IsDigitFmt(enumerator.Current))
			{
				num2++;
			}
		}
		return num2;
	}

	private static bool interpretIntegerCommas(StringBuilder sb, List<Special> specials, Special decimalPoint, int integerEnd, int fractionalEnd, double[] scale)
	{
		List<Special> range = specials.GetRange(0, integerEnd);
		bool flag = true;
		bool result = false;
		for (int num = range.Count - 1; num >= 0; num--)
		{
			if (range[num].ch != ',')
			{
				flag = false;
			}
			else if (flag)
			{
				scale[0] /= 1000.0;
			}
			else
			{
				result = true;
			}
		}
		if (decimalPoint != null)
		{
			range = specials.GetRange(0, fractionalEnd);
			int num2 = range.Count - 1;
			while (num2 >= 0 && range[num2].ch == ',')
			{
				scale[0] /= 1000.0;
				num2--;
			}
		}
		IEnumerator<Special> enumerator = specials.GetEnumerator();
		int num3 = 0;
		List<Special> list = new List<Special>();
		while (enumerator.MoveNext())
		{
			Special current = enumerator.Current;
			current.pos -= num3;
			if (current.ch == ',')
			{
				num3++;
				list.Add(current);
				sb.Remove(current.pos, 1);
			}
		}
		foreach (Special item in list)
		{
			specials.Remove(item);
		}
		return result;
	}

	private int integerEnd()
	{
		if (afterInteger != null)
		{
			return specials.IndexOf(afterInteger);
		}
		return specials.Count;
	}

	private int fractionalEnd()
	{
		if (afterFractional != null)
		{
			return specials.IndexOf(afterFractional);
		}
		return specials.Count;
	}

	public override void FormatValue(StringBuilder toAppendTo, object valueObject)
	{
		double num = (double)valueObject;
		num *= scale;
		bool flag = num < 0.0;
		if (flag)
		{
			num = 0.0 - num;
		}
		double fractional = 0.0;
		if (slash != null)
		{
			if (improperFraction)
			{
				fractional = num;
				num = 0.0;
			}
			else
			{
				fractional = num % 1.0;
				num = (long)num;
			}
		}
		SortedList<CellNumberStringMod, object> sortedList = new SortedList<CellNumberStringMod, object>();
		StringBuilder stringBuilder = new StringBuilder(desc);
		if (exponent != null)
		{
			WriteScientific(num, stringBuilder, sortedList);
		}
		else if (improperFraction)
		{
			WriteFraction(num, null, fractional, stringBuilder, sortedList);
		}
		else
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(num.ToString(printfFmt));
			if (numerator == null)
			{
				WriteFractional(stringBuilder2, stringBuilder);
				Writeint(stringBuilder2, stringBuilder, integerSpecials, sortedList, integerCommas);
			}
			else
			{
				WriteFraction(num, stringBuilder2, fractional, stringBuilder, sortedList);
			}
		}
		specials.GetEnumerator();
		IEnumerator enumerator = sortedList.Keys.GetEnumerator();
		CellNumberStringMod cellNumberStringMod = (enumerator.MoveNext() ? ((CellNumberStringMod)enumerator.Current) : null);
		int num2 = 0;
		BitArray bitArray = new BitArray(1024);
		foreach (Special special in specials)
		{
			int num3 = special.pos + num2;
			if (!bitArray[special.pos] && stringBuilder[num3] == '#')
			{
				stringBuilder.Remove(num3, 1);
				num2--;
				bitArray.Set(special.pos, value: true);
			}
			while (cellNumberStringMod != null && special == cellNumberStringMod.GetSpecial())
			{
				int length = stringBuilder.Length;
				int num4 = special.pos + num2;
				switch (cellNumberStringMod.Op)
				{
				case 2:
					if (!cellNumberStringMod.ToAdd.Equals(",") || !bitArray.Get(special.pos))
					{
						stringBuilder.Insert(num4 + 1, cellNumberStringMod.ToAdd);
					}
					break;
				case 1:
					stringBuilder.Insert(num4, cellNumberStringMod.ToAdd);
					break;
				case 3:
				{
					int num5 = special.pos;
					if (!cellNumberStringMod.IsStartInclusive)
					{
						num5++;
						num4++;
					}
					while (bitArray.Get(num5))
					{
						num5++;
						num4++;
					}
					int num6 = cellNumberStringMod.End.pos;
					if (cellNumberStringMod.IsEndInclusive)
					{
						num6++;
					}
					int num7 = num6 + num2;
					if (num4 >= num7)
					{
						break;
					}
					if (cellNumberStringMod.ToAdd == "")
					{
						stringBuilder.Remove(num4, num7 - num4);
					}
					else
					{
						char value = cellNumberStringMod.ToAdd[0];
						for (int i = num4; i < num7; i++)
						{
							stringBuilder[i] = value;
						}
					}
					for (int j = num5; j < num6; j++)
					{
						bitArray.Set(j, value: true);
					}
					break;
				}
				default:
					throw new InvalidOperationException("Unknown op: " + cellNumberStringMod.Op);
				}
				num2 += stringBuilder.Length - length;
				cellNumberStringMod = ((!enumerator.MoveNext()) ? null : ((CellNumberStringMod)enumerator.Current));
			}
		}
		if (flag)
		{
			toAppendTo.Append('-');
		}
		toAppendTo.Append((object)stringBuilder);
	}

	private void WriteScientific(double value, StringBuilder output, SortedList<CellNumberStringMod, object> mods)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string pattern = decimalFmt.Pattern;
		int i;
		for (i = 0; pattern[i] == '#' || pattern[i] == '0'; i++)
		{
		}
		int num = i;
		if (pattern[0] == '#')
		{
			num--;
		}
		if (num >= 6 && value > 1.0)
		{
			pattern = pattern.Substring(1);
			stringBuilder.Append(value.ToString(pattern));
		}
		else
		{
			stringBuilder.Append(value.ToString("E"));
		}
		Writeint(stringBuilder, output, integerSpecials, mods, integerCommas);
		WriteFractional(stringBuilder, output);
		string text = stringBuilder.ToString();
		int num2 = text.IndexOf("E");
		int num3 = num2 + 1;
		char c = stringBuilder[num3];
		if (c != '-')
		{
			c = '+';
			if (text.IndexOf(c, num2) < 0)
			{
				stringBuilder.Insert(num3, '+');
			}
		}
		object obj = exponentSpecials.GetEnumerator();
		((IEnumerator)obj).MoveNext();
		((IEnumerator)obj).MoveNext();
		Special current = ((IEnumerator<Special>)obj).Current;
		char ch = current.ch;
		if (c == '-' || ch == '+')
		{
			mods.Add(ReplaceMod(current, startInclusive: true, current, endInclusive: true, c), null);
		}
		else
		{
			mods.Add(deleteMod(current, startInclusive: true, current, endInclusive: true), null);
		}
		StringBuilder stringBuilder2 = new StringBuilder(stringBuilder.ToString().Substring(num3 + 1));
		if (stringBuilder2.Length > 2 && stringBuilder2[0] == '0')
		{
			stringBuilder2.Remove(0, 1);
		}
		Writeint(stringBuilder2, output, exponentDigitSpecials, mods, ShowCommas: false);
	}

	private void WriteFraction(double value, StringBuilder result, double fractional, StringBuilder output, SortedList<CellNumberStringMod, object> mods)
	{
		if (!improperFraction)
		{
			if (fractional == 0.0 && !HasChar('0', numeratorSpecials))
			{
				Writeint(result, output, integerSpecials, mods, ShowCommas: false);
				Special start = lastSpecial(integerSpecials);
				Special end = lastSpecial(denominatorSpecials);
				if (HasChar('?', integerSpecials, numeratorSpecials, denominatorSpecials))
				{
					mods.Add(ReplaceMod(start, startInclusive: false, end, endInclusive: true, ' '), null);
				}
				else
				{
					mods.Add(deleteMod(start, startInclusive: false, end, endInclusive: true), null);
				}
				return;
			}
			bool flag = !HasChar('0', numeratorSpecials);
			bool flag2 = !HasChar('0', integerSpecials);
			bool flag3 = integerSpecials.Count == 0 || (integerSpecials.Count == 1 && HasChar('#', integerSpecials));
			bool flag4 = fractional == 0.0 && (flag3 | flag);
			bool flag5 = (fractional != 0.0) & flag2;
			if (value == 0.0 && (flag4 | flag5))
			{
				Special start2 = lastSpecial(integerSpecials);
				CellNumberStringMod key = (HasChar('?', integerSpecials, numeratorSpecials) ? ReplaceMod(start2, startInclusive: true, numerator, endInclusive: false, ' ') : deleteMod(start2, startInclusive: true, numerator, endInclusive: false));
				mods.Add(key, null);
			}
			else
			{
				Writeint(result, output, integerSpecials, mods, ShowCommas: false);
			}
		}
		try
		{
			int num;
			int num2;
			if (fractional == 0.0 || (improperFraction && fractional % 1.0 == 0.0))
			{
				num = (int)Math.Round(fractional);
				num2 = 1;
			}
			else
			{
				SimpleFraction simpleFraction = SimpleFraction.BuildFractionMaxDenominator(fractional, maxDenominator);
				num = simpleFraction.Numerator;
				num2 = simpleFraction.Denominator;
			}
			if (improperFraction)
			{
				num += (int)Math.Round(value * (double)num2);
			}
			WriteSingleint(numeratorFmt, num, output, numeratorSpecials, mods);
			WriteSingleint(denominatorFmt, num2, output, denominatorSpecials, mods);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.StackTrace);
		}
	}

	private static bool HasChar(char ch, params List<Special>[] numSpecials)
	{
		for (int i = 0; i < numSpecials.Length; i++)
		{
			foreach (Special item in numSpecials[i])
			{
				if (item.ch == ch)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void WriteSingleint(string fmt, int num, StringBuilder output, List<Special> numSpecials, SortedList<CellNumberStringMod, object> mods)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(num.ToString(fmt));
		Writeint(stringBuilder, output, numSpecials, mods, ShowCommas: false);
	}

	private void Writeint(StringBuilder result, StringBuilder output, List<Special> numSpecials, SortedList<CellNumberStringMod, object> mods, bool ShowCommas)
	{
		int num = result.ToString().IndexOf(".") - 1;
		if (num < 0)
		{
			num = ((exponent == null || numSpecials != integerSpecials) ? (result.Length - 1) : (result.ToString().IndexOf("E") - 1));
		}
		int i;
		for (i = 0; i < num; i++)
		{
			char c = result[i];
			if (c != '0' && c != ',')
			{
				break;
			}
		}
		Special special = null;
		int num2 = 0;
		for (int num3 = numSpecials.Count - 1; num3 >= 0; num3--)
		{
			char c2 = ((num < 0) ? '0' : result[num]);
			Special special2 = numSpecials[num3];
			bool num4 = ShowCommas && num2 > 0 && num2 % 3 == 0;
			bool flag = false;
			if (c2 != '0' || special2.ch == '0' || special2.ch == '?' || num >= i)
			{
				flag = special2.ch == '?' && num < i;
				output[special2.pos] = (flag ? ' ' : c2);
				special = special2;
			}
			if (num4)
			{
				mods.Add(insertMod(special2, flag ? " " : ",", 2), null);
			}
			num2++;
			num--;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (num < 0)
		{
			return;
		}
		num++;
		stringBuilder = new StringBuilder(result.ToString().Substring(0, num));
		if (ShowCommas)
		{
			while (num > 0)
			{
				if (num2 > 0 && num2 % 3 == 0)
				{
					stringBuilder.Insert(num, ',');
				}
				num2++;
				num--;
			}
		}
		mods.Add(insertMod(special, stringBuilder.ToString(), 1), null);
	}

	private void WriteFractional(StringBuilder result, StringBuilder output)
	{
		if (fractionalSpecials.Count <= 0)
		{
			return;
		}
		int num = result.ToString().IndexOf(".") + 1;
		int num2 = ((exponent == null) ? (result.Length - 1) : (result.ToString().IndexOf("E") - 1));
		while (num2 > num && result[num2] == '0')
		{
			num2--;
		}
		foreach (Special fractionalSpecial in fractionalSpecials)
		{
			if (num >= result.Length)
			{
				break;
			}
			char c = result[num];
			if (c != '0' || fractionalSpecial.ch == '0' || num < num2)
			{
				output[fractionalSpecial.pos] = c;
			}
			else if (fractionalSpecial.ch == '?')
			{
				output[fractionalSpecial.pos] = ' ';
			}
			num++;
		}
	}

	public override void SimpleValue(StringBuilder toAppendTo, object value)
	{
		SIMPLE_NUMBER.FormatValue(toAppendTo, value);
	}

	private static Special lastSpecial(List<Special> s)
	{
		return s[s.Count - 1];
	}
}
