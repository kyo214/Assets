using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NPOI.SS.Format;

public class CellNumberPartHandler : CellFormatPart.IPartHandler
{
	private char insertSignForExponent;

	private double scale = 1.0;

	private CellNumberFormatter.Special decimalPoint;

	private CellNumberFormatter.Special slash;

	private CellNumberFormatter.Special exponent;

	private CellNumberFormatter.Special numerator;

	private List<CellNumberFormatter.Special> specials = new List<CellNumberFormatter.Special>();

	private bool improperFraction;

	public double Scale => scale;

	public CellNumberFormatter.Special DecimalPoint => decimalPoint;

	public CellNumberFormatter.Special Slash => slash;

	public CellNumberFormatter.Special Exponent => exponent;

	public CellNumberFormatter.Special Numerator => numerator;

	public List<CellNumberFormatter.Special> Specials => specials;

	public bool IsImproperFraction => improperFraction;

	public string HandlePart(Match m, string part, CellFormatType type, StringBuilder descBuf)
	{
		int num = descBuf.Length;
		switch (part[0])
		{
		case 'E':
		case 'e':
			if (exponent == null && specials.Count > 0)
			{
				exponent = new CellNumberFormatter.Special('.', num);
				specials.Add(exponent);
				insertSignForExponent = part[1];
				return part.Substring(0, 1);
			}
			break;
		case '#':
		case '0':
		case '?':
		{
			if (insertSignForExponent != 0)
			{
				specials.Add(new CellNumberFormatter.Special(insertSignForExponent, num));
				descBuf.Append(insertSignForExponent);
				insertSignForExponent = '\0';
				num++;
			}
			for (int i = 0; i < part.Length; i++)
			{
				char ch = part[i];
				specials.Add(new CellNumberFormatter.Special(ch, num + i));
			}
			break;
		}
		case '.':
			if (decimalPoint == null && specials.Count > 0)
			{
				decimalPoint = new CellNumberFormatter.Special('.', num);
				specials.Add(decimalPoint);
			}
			break;
		case '/':
			if (slash == null && specials.Count > 0)
			{
				numerator = PreviousNumber();
				improperFraction |= numerator == FirstDigit(specials);
				slash = new CellNumberFormatter.Special('.', num);
				specials.Add(slash);
			}
			break;
		case '%':
			scale *= 100.0;
			break;
		default:
			return null;
		}
		return part;
	}

	private CellNumberFormatter.Special PreviousNumber()
	{
		for (int num = specials.Count - 1; num >= 0; num--)
		{
			CellNumberFormatter.Special special = specials[num];
			if (IsDigitFmt(special))
			{
				CellNumberFormatter.Special special2 = special;
				while (num >= 0)
				{
					special = specials[num];
					if (special2.pos - special.pos > 1 || !IsDigitFmt(special))
					{
						break;
					}
					special2 = special;
					num--;
				}
				return special2;
			}
		}
		return null;
	}

	private static bool IsDigitFmt(CellNumberFormatter.Special s)
	{
		if (s.ch != '0' && s.ch != '?')
		{
			return s.ch == '#';
		}
		return true;
	}

	private static CellNumberFormatter.Special FirstDigit(List<CellNumberFormatter.Special> specials)
	{
		foreach (CellNumberFormatter.Special special in specials)
		{
			if (IsDigitFmt(special))
			{
				return special;
			}
		}
		return null;
	}
}
