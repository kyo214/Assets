using System;
using System.Globalization;
using System.Text;
using NPOI.Util;

namespace NPOI.SS.Util;

public class NormalisedDecimal
{
	private const int EXPONENT_OFFSET = 14;

	private static readonly decimal BD_2_POW_24 = new decimal((BigInteger.One << 24).LongValue());

	private const int LOG_BASE_10_OF_2_TIMES_2_POW_20 = 315653;

	private const int C_2_POW_19 = 524288;

	private const int FRAC_HALF = 8388608;

	private const long MAX_REP_WHOLE_PART = 1000000000000000L;

	private int _relativeDecimalExponent;

	private long _wholePart;

	private int _fractionalPart;

	public static NormalisedDecimal Create(BigInteger frac, int binaryExponent)
	{
		int num = ((binaryExponent > 49 || binaryExponent < 46) ? (-(15204352 - binaryExponent * 315653 + 524288 >> 20)) : 0);
		MutableFPNumber mutableFPNumber = new MutableFPNumber(frac, binaryExponent);
		if (num != 0)
		{
			mutableFPNumber.multiplyByPowerOfTen(-num);
		}
		switch (mutableFPNumber.Get64BitNormalisedExponent())
		{
		case 46:
			if (mutableFPNumber.IsAboveMinRep())
			{
				break;
			}
			goto case 44;
		case 44:
		case 45:
			mutableFPNumber.multiplyByPowerOfTen(1);
			num--;
			break;
		case 49:
			if (mutableFPNumber.IsBelowMaxRep())
			{
				break;
			}
			goto case 50;
		case 50:
			mutableFPNumber.multiplyByPowerOfTen(-1);
			num++;
			break;
		default:
			throw new InvalidOperationException("Bad binary exp " + mutableFPNumber.Get64BitNormalisedExponent() + ".");
		case 47:
		case 48:
			break;
		}
		mutableFPNumber.Normalise64bit();
		return mutableFPNumber.CreateNormalisedDecimal(num);
	}

	public NormalisedDecimal RoundUnits()
	{
		long num = _wholePart;
		if (_fractionalPart >= 8388608)
		{
			num++;
		}
		int relativeDecimalExponent = _relativeDecimalExponent;
		if (num < 1000000000000000L)
		{
			return new NormalisedDecimal(num, 0, relativeDecimalExponent);
		}
		return new NormalisedDecimal(num / 10, 0, relativeDecimalExponent + 1);
	}

	public NormalisedDecimal(long wholePart, int fracPart, int decimalExponent)
	{
		_wholePart = wholePart;
		_fractionalPart = fracPart;
		_relativeDecimalExponent = decimalExponent;
	}

	public ExpandedDouble NormaliseBaseTwo()
	{
		MutableFPNumber mutableFPNumber = new MutableFPNumber(ComposeFrac(), 39);
		mutableFPNumber.multiplyByPowerOfTen(_relativeDecimalExponent);
		mutableFPNumber.Normalise64bit();
		return mutableFPNumber.CreateExpandedDouble();
	}

	public BigInteger ComposeFrac()
	{
		long wholePart = _wholePart;
		int fractionalPart = _fractionalPart;
		return new BigInteger(new byte[11]
		{
			(byte)(wholePart >> 56),
			(byte)(wholePart >> 48),
			(byte)(wholePart >> 40),
			(byte)(wholePart >> 32),
			(byte)(wholePart >> 24),
			(byte)(wholePart >> 16),
			(byte)(wholePart >> 8),
			(byte)wholePart,
			(byte)(fractionalPart >> 16),
			(byte)(fractionalPart >> 8),
			(byte)fractionalPart
		});
	}

	public string GetSignificantDecimalDigits()
	{
		return _wholePart.ToString(CultureInfo.InvariantCulture);
	}

	public string GetSignificantDecimalDigitsLastDigitRounded()
	{
		long value = _wholePart + 5;
		StringBuilder stringBuilder = new StringBuilder(24);
		stringBuilder.Append(value);
		stringBuilder[stringBuilder.Length - 1] = '0';
		return stringBuilder.ToString();
	}

	public int GetDecimalExponent()
	{
		return _relativeDecimalExponent + 14;
	}

	public int CompareNormalised(NormalisedDecimal other)
	{
		int num = _relativeDecimalExponent - other._relativeDecimalExponent;
		if (num != 0)
		{
			return num;
		}
		if (_wholePart > other._wholePart)
		{
			return 1;
		}
		if (_wholePart < other._wholePart)
		{
			return -1;
		}
		return _fractionalPart - other._fractionalPart;
	}

	public decimal GetFractionalPart()
	{
		return new decimal(_fractionalPart) / BD_2_POW_24;
	}

	private string GetFractionalDigits()
	{
		if (_fractionalPart == 0)
		{
			return "0";
		}
		return GetFractionalPart().ToString(CultureInfo.InvariantCulture).Substring(2);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append(" [");
		string text = _wholePart.ToString(CultureInfo.InvariantCulture);
		stringBuilder.Append(text[0]);
		stringBuilder.Append('.');
		stringBuilder.Append(text.Substring(1));
		stringBuilder.Append(' ');
		stringBuilder.Append(GetFractionalDigits());
		stringBuilder.Append("E");
		stringBuilder.Append(GetDecimalExponent());
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
