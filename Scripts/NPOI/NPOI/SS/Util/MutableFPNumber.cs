using System;
using NPOI.Util;

namespace NPOI.SS.Util;

public class MutableFPNumber
{
	private class Rounder
	{
		private static BigInteger[] HALF_BITS;

		static Rounder()
		{
			BigInteger[] array = new BigInteger[33];
			long num = 1L;
			for (int i = 1; i < array.Length; i++)
			{
				array[i] = new BigInteger(num);
				num <<= 1;
			}
			HALF_BITS = array;
		}

		public static BigInteger Round(BigInteger bi, int nBits)
		{
			if (nBits < 1)
			{
				return bi;
			}
			return bi + HALF_BITS[nBits];
		}
	}

	private class TenPower
	{
		private static readonly BigInteger FIVE = new BigInteger(5L);

		private static TenPower[] _cache = new TenPower[350];

		public BigInteger _multiplicand;

		public BigInteger _divisor;

		public int _divisorShift;

		public int _multiplierShift;

		private TenPower(int index)
		{
			BigInteger bigInteger = FIVE.Pow(index);
			int num = bigInteger.BitLength();
			int num2 = 80 + num;
			BigInteger bigInteger2 = (BigInteger.One << num2) / bigInteger;
			int num3 = bigInteger2.BitLength() - 80;
			_divisor = bigInteger2 >> num3;
			num -= num3;
			_divisorShift = -(num + index + 80);
			int num4 = bigInteger.BitLength() - 68;
			if (num4 > 0)
			{
				_multiplierShift = index + num4;
				_multiplicand = bigInteger >> num4;
			}
			else
			{
				_multiplierShift = index;
				_multiplicand = bigInteger;
			}
		}

		public static TenPower GetInstance(int index)
		{
			TenPower tenPower = _cache[index];
			if (tenPower == null)
			{
				tenPower = new TenPower(index);
				_cache[index] = tenPower;
			}
			return tenPower;
		}
	}

	private static readonly BigInteger BI_MIN_BASE = new BigInteger(new int[2] { -1243209484, 2147477094 }, 1);

	private static readonly BigInteger BI_MAX_BASE = new BigInteger(new int[2] { -480270031, -1610620928 }, 1);

	private const int C_64 = 64;

	private const int MIN_PRECISION = 72;

	private BigInteger _significand;

	private int _binaryExponent;

	public MutableFPNumber(BigInteger frac, int binaryExponent)
	{
		_significand = frac;
		_binaryExponent = binaryExponent;
	}

	public MutableFPNumber Copy()
	{
		return new MutableFPNumber(_significand, _binaryExponent);
	}

	public void Normalise64bit()
	{
		int num = _significand.BitLength();
		int num2 = num - 64;
		if (num2 != 0)
		{
			if (num2 < 0)
			{
				throw new InvalidOperationException("Not enough precision");
			}
			_binaryExponent += num2;
			if (num2 > 32)
			{
				int num3 = (num2 - 1) & 0xFFFFE0;
				_significand >>= num3;
				num2 -= num3;
				num -= num3;
			}
			if (num2 < 1)
			{
				throw new InvalidOperationException();
			}
			_significand = Rounder.Round(_significand, num2);
			if (_significand.BitLength() > num)
			{
				num2++;
				_binaryExponent++;
			}
			_significand >>= num2;
		}
	}

	public int Get64BitNormalisedExponent()
	{
		return _binaryExponent + _significand.BitLength() - 64;
	}

	public bool IsBelowMaxRep()
	{
		int n = _significand.BitLength() - 64;
		return _significand.CompareTo(BI_MAX_BASE.ShiftLeft(n)) < 0;
	}

	public bool IsAboveMinRep()
	{
		int n = _significand.BitLength() - 64;
		return _significand.CompareTo(BI_MIN_BASE.ShiftLeft(n)) > 0;
	}

	public NormalisedDecimal CreateNormalisedDecimal(int pow10)
	{
		int num = _binaryExponent - 39;
		int fracPart = (_significand.IntValue() << num) & 0xFFFF80;
		return new NormalisedDecimal((_significand >> 64 - _binaryExponent - 1).LongValue(), fracPart, pow10);
	}

	public void multiplyByPowerOfTen(int pow10)
	{
		TenPower instance = TenPower.GetInstance(Math.Abs(pow10));
		if (pow10 < 0)
		{
			mulShift(instance._divisor, instance._divisorShift);
		}
		else
		{
			mulShift(instance._multiplicand, instance._multiplierShift);
		}
	}

	private void mulShift(BigInteger multiplicand, int multiplierShift)
	{
		_significand *= multiplicand;
		_binaryExponent += multiplierShift;
		int num = (_significand.BitLength() - 72) & -32;
		if (num > 0)
		{
			_significand >>= num;
			_binaryExponent += num;
		}
	}

	public ExpandedDouble CreateExpandedDouble()
	{
		return new ExpandedDouble(_significand, _binaryExponent);
	}
}
