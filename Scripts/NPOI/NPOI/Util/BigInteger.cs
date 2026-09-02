using System;
using System.Globalization;
using System.Text;

namespace NPOI.Util;

public class BigInteger : IComparable<BigInteger>
{
	private int _signum;

	internal int[] mag;

	private int bitCount;

	private int bitLength;

	private int firstNonzeroIntNum;

	public const long LONG_MASK = 4294967295L;

	public const long INFLATED = long.MinValue;

	public const int Min_RADIX = 2;

	public const int Max_RADIX = 36;

	private static BigInteger[] posConst;

	private static BigInteger[] negConst;

	private static readonly string[] zeros;

	public static readonly BigInteger ZERO;

	public static readonly BigInteger One;

	private static readonly BigInteger Two;

	public static readonly BigInteger TEN;

	private const int Max_CONSTANT = 16;

	private static readonly int[] digitsPerLong;

	private static readonly BigInteger[] longRadix;

	private static readonly long[] bitsPerDigit;

	private static readonly int[] digitsPerInt;

	private static readonly int[] intRadix;

	static BigInteger()
	{
		posConst = new BigInteger[17];
		negConst = new BigInteger[17];
		zeros = new string[64];
		ZERO = new BigInteger(new int[0], 0);
		One = ValueOf(1L);
		Two = ValueOf(2L);
		TEN = ValueOf(10L);
		digitsPerLong = new int[37]
		{
			0, 0, 62, 39, 31, 27, 24, 22, 20, 19,
			18, 18, 17, 17, 16, 16, 15, 15, 15, 14,
			14, 14, 14, 13, 13, 13, 13, 13, 13, 12,
			12, 12, 12, 12, 12, 12, 12
		};
		longRadix = new BigInteger[37]
		{
			null,
			null,
			ValueOf(4611686018427387904L),
			ValueOf(4052555153018976267L),
			ValueOf(4611686018427387904L),
			ValueOf(7450580596923828125L),
			ValueOf(4738381338321616896L),
			ValueOf(3909821048582988049L),
			ValueOf(1152921504606846976L),
			ValueOf(1350851717672992089L),
			ValueOf(1000000000000000000L),
			ValueOf(5559917313492231481L),
			ValueOf(2218611106740436992L),
			ValueOf(8650415919381337933L),
			ValueOf(2177953337809371136L),
			ValueOf(6568408355712890625L),
			ValueOf(1152921504606846976L),
			ValueOf(2862423051509815793L),
			ValueOf(6746640616477458432L),
			ValueOf(799006685782884121L),
			ValueOf(1638400000000000000L),
			ValueOf(3243919932521508681L),
			ValueOf(6221821273427820544L),
			ValueOf(504036361936467383L),
			ValueOf(876488338465357824L),
			ValueOf(1490116119384765625L),
			ValueOf(2481152873203736576L),
			ValueOf(4052555153018976267L),
			ValueOf(6502111422497947648L),
			ValueOf(353814783205469041L),
			ValueOf(531441000000000000L),
			ValueOf(787662783788549761L),
			ValueOf(1152921504606846976L),
			ValueOf(1667889514952984961L),
			ValueOf(2386420683693101056L),
			ValueOf(3379220508056640625L),
			ValueOf(4738381338321616896L)
		};
		bitsPerDigit = new long[37]
		{
			0L, 0L, 1024L, 1624L, 2048L, 2378L, 2648L, 2875L, 3072L, 3247L,
			3402L, 3543L, 3672L, 3790L, 3899L, 4001L, 4096L, 4186L, 4271L, 4350L,
			4426L, 4498L, 4567L, 4633L, 4696L, 4756L, 4814L, 4870L, 4923L, 4975L,
			5025L, 5074L, 5120L, 5166L, 5210L, 5253L, 5295L
		};
		digitsPerInt = new int[37]
		{
			0, 0, 30, 19, 15, 13, 11, 11, 10, 9,
			9, 8, 8, 8, 8, 7, 7, 7, 7, 7,
			7, 7, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 5
		};
		intRadix = new int[37]
		{
			0, 0, 1073741824, 1162261467, 1073741824, 1220703125, 362797056, 1977326743, 1073741824, 387420489,
			1000000000, 214358881, 429981696, 815730721, 1475789056, 170859375, 268435456, 410338673, 612220032, 893871739,
			1280000000, 1801088541, 113379904, 148035889, 191102976, 244140625, 308915776, 387420489, 481890304, 594823321,
			729000000, 887503681, 1073741824, 1291467969, 1544804416, 1838265625, 60466176
		};
		Init();
	}

	private static void Init()
	{
		if (zeros[63] == null)
		{
			for (int i = 1; i <= 16; i++)
			{
				int[] magnitude = new int[1] { i };
				posConst[i] = new BigInteger(magnitude, 1);
				negConst[i] = new BigInteger(magnitude, -1);
			}
			zeros[63] = "000000000000000000000000000000000000000000000000000000000000000";
			for (int j = 0; j < 63; j++)
			{
				zeros[j] = zeros[63].Substring(0, j);
			}
		}
	}

	public BigInteger(int[] magnitude, int signum)
	{
		_signum = ((magnitude.Length != 0) ? signum : 0);
		mag = magnitude;
	}

	public BigInteger(byte[] val)
	{
		if (val.Length == 0)
		{
			throw new ArgumentException("Zero length BigInteger");
		}
		if ((sbyte)val[0] < 0)
		{
			mag = makePositive(val);
			_signum = -1;
		}
		else
		{
			mag = stripLeadingZeroBytes(val);
			_signum = ((mag.Length != 0) ? 1 : 0);
		}
	}

	public BigInteger(int[] val)
	{
		if (val.Length == 0)
		{
			throw new ArgumentException("Zero length BigInteger");
		}
		if (val[0] < 0)
		{
			mag = makePositive(val);
			_signum = -1;
		}
		else
		{
			mag = TrustedStripLeadingZeroInts(val);
			_signum = ((mag.Length != 0) ? 1 : 0);
		}
	}

	public BigInteger(long val)
	{
		if (val < 0)
		{
			val = -val;
			_signum = -1;
		}
		else
		{
			_signum = 1;
		}
		int num = (int)Operator.UnsignedRightShift(val, 32);
		if (num == 0)
		{
			mag = new int[1];
			mag[0] = (int)val;
		}
		else
		{
			mag = new int[2];
			mag[0] = num;
			mag[1] = (int)val;
		}
	}

	public BigInteger(string val)
		: this(val, 10)
	{
	}

	public BigInteger(string val, int radix)
	{
		int i = 0;
		int length = val.Length;
		if (radix < 2 || radix > 36)
		{
			throw new FormatException("Radix out of range");
		}
		if (length == 0)
		{
			throw new FormatException("Zero length BigInteger");
		}
		int signum = 1;
		int num = val.LastIndexOf('-');
		int num2 = val.LastIndexOf('+');
		if (num + num2 <= -1)
		{
			if (num == 0 || num2 == 0)
			{
				i = 1;
				if (length == 1)
				{
					throw new FormatException("Zero length BigInteger");
				}
			}
			if (num == 0)
			{
				signum = -1;
			}
			for (; i < length && val[i] == '0'; i++)
			{
			}
			if (i == length)
			{
				_signum = 0;
				mag = ZERO.mag;
				return;
			}
			int num3 = length - i;
			_signum = signum;
			int num4 = Operator.UnsignedRightShift((int)(Operator.UnsignedRightShift(num3 * bitsPerDigit[radix], 10) + 1) + 31, 5);
			int[] array = new int[num4];
			int num5 = num3 % digitsPerInt[radix];
			if (num5 == 0)
			{
				num5 = digitsPerInt[radix];
			}
			array[num4 - 1] = int.Parse(val.Substring(i, i += num5), CultureInfo.InvariantCulture);
			if (array[num4 - 1] < 0)
			{
				throw new FormatException("Illegal digit");
			}
			int y = intRadix[radix];
			int num6 = 0;
			while (i < length)
			{
				string s = val.Substring(i, i += digitsPerInt[radix]);
				num6 = int.Parse(s, CultureInfo.InvariantCulture);
				if (num6 < 0)
				{
					throw new FormatException("Illegal digit");
				}
				DestructiveMulAdd(array, y, num6);
			}
			mag = TrustedStripLeadingZeroInts(array);
			return;
		}
		throw new FormatException("Illegal embedded sign character");
	}

	private static int[] TrustedStripLeadingZeroInts(int[] val)
	{
		int num = val.Length;
		int i;
		for (i = 0; i < num && val[i] == 0; i++)
		{
		}
		if (i != 0)
		{
			return Arrays.CopyOfRange(val, i, num);
		}
		return val;
	}

	private static void DestructiveMulAdd(int[] x, int y, int z)
	{
		long num = y & 0xFFFFFFFFu;
		long num2 = z & 0xFFFFFFFFu;
		int num3 = x.Length;
		long num4 = 0L;
		long num5 = 0L;
		for (int num6 = num3 - 1; num6 >= 0; num6--)
		{
			num4 = num * (x[num6] & 0xFFFFFFFFu) + num5;
			x[num6] = (int)num4;
			num5 = Operator.UnsignedRightShift(num4, 32);
		}
		long num7 = (x[num3 - 1] & 0xFFFFFFFFu) + num2;
		x[num3 - 1] = (int)num7;
		num5 = Operator.UnsignedRightShift(num7, 32);
		for (int num8 = num3 - 2; num8 >= 0; num8--)
		{
			num7 = (x[num8] & 0xFFFFFFFFu) + num5;
			x[num8] = (int)num7;
			num5 = Operator.UnsignedRightShift(num7, 32);
		}
	}

	public string ToString(int radix)
	{
		if (_signum == 0)
		{
			return "0";
		}
		if (radix < 2 || radix > 36)
		{
			radix = 10;
		}
		if (radix != 10)
		{
			throw new ArgumentException("Only support 10 radix rendering");
		}
		string[] array = new string[(4 * mag.Length + 6) / 7];
		BigInteger bigInteger = Abs();
		int num = 0;
		while (bigInteger._signum != 0)
		{
			BigInteger bigInteger2 = longRadix[radix];
			MutableBigInteger mutableBigInteger = new MutableBigInteger();
			MutableBigInteger mutableBigInteger2 = new MutableBigInteger(bigInteger.mag);
			MutableBigInteger b = new MutableBigInteger(bigInteger2.mag);
			MutableBigInteger mutableBigInteger3 = mutableBigInteger2.divide(b, mutableBigInteger);
			BigInteger bigInteger3 = mutableBigInteger.toBigInteger(bigInteger._signum * bigInteger2._signum);
			BigInteger bigInteger4 = mutableBigInteger3.toBigInteger(bigInteger._signum * bigInteger2._signum);
			array[num++] = bigInteger4.LongValue().ToString(CultureInfo.InvariantCulture);
			bigInteger = bigInteger3;
		}
		StringBuilder stringBuilder = new StringBuilder(num * digitsPerLong[radix] + 1);
		if (_signum < 0)
		{
			stringBuilder.Append('-');
		}
		stringBuilder.Append(array[num - 1]);
		for (int num2 = num - 2; num2 >= 0; num2--)
		{
			int num3 = digitsPerLong[radix] - array[num2].Length;
			if (num3 != 0)
			{
				stringBuilder.Append(zeros[num3]);
			}
			stringBuilder.Append(array[num2]);
		}
		return stringBuilder.ToString();
	}

	public static BigInteger ValueOf(long val)
	{
		Init();
		switch (val)
		{
		case 0L:
			return ZERO;
		case 1L:
		case 2L:
		case 3L:
		case 4L:
		case 5L:
		case 6L:
		case 7L:
		case 8L:
		case 9L:
		case 10L:
		case 11L:
		case 12L:
		case 13L:
		case 14L:
		case 15L:
		case 16L:
			return posConst[(int)val];
		default:
			if (val < 0 && val >= -16)
			{
				return negConst[(int)(-val)];
			}
			return new BigInteger(val);
		}
	}

	private static BigInteger ValueOf(int[] val)
	{
		if (val[0] <= 0)
		{
			return new BigInteger(val);
		}
		return new BigInteger(val, 1);
	}

	public static int BitLengthForInt(int n)
	{
		return 32 - NumberOfLeadingZeros(n);
	}

	public int BitLength()
	{
		int num = bitLength - 1;
		if (num == -1)
		{
			int num2 = mag.Length;
			if (num2 == 0)
			{
				num = 0;
			}
			else
			{
				int num3 = (num2 - 1 << 5) + BitLengthForInt(mag[0]);
				if (_signum < 0)
				{
					bool flag = BitCountForInt(mag[0]) == 1;
					for (int i = 1; (i < num2) & flag; i++)
					{
						flag = mag[i] == 0;
					}
					num = (flag ? (num3 - 1) : num3);
				}
				else
				{
					num = num3;
				}
			}
			bitLength = num + 1;
		}
		return num;
	}

	public int BitCount()
	{
		int num = bitCount - 1;
		if (num == -1)
		{
			num = 0;
			for (int i = 0; i < mag.Length; i++)
			{
				num += BitCountForInt(mag[i]);
			}
			if (_signum < 0)
			{
				int num2 = 0;
				int num3 = mag.Length - 1;
				while (mag[num3] == 0)
				{
					num2 += 32;
					num3--;
				}
				num2 += NumberOfTrailingZeros(mag[num3]);
				num += num2 - 1;
			}
			bitCount = num + 1;
		}
		return num;
	}

	public BigInteger Abs()
	{
		if (_signum < 0)
		{
			return Negate();
		}
		return this;
	}

	public BigInteger Negate()
	{
		return new BigInteger(mag, -_signum);
	}

	public BigInteger Pow(int exponent)
	{
		if (exponent < 0)
		{
			throw new ArithmeticException("Negative exponent");
		}
		if (_signum == 0)
		{
			if (exponent != 0)
			{
				return this;
			}
			return One;
		}
		int signum = ((_signum >= 0 || (exponent & 1) != 1) ? 1 : (-1));
		int[] array = mag;
		int[] array2 = new int[1] { 1 };
		while (exponent != 0)
		{
			if ((exponent & 1) == 1)
			{
				array2 = MultiplyToLen(array2, array2.Length, array, array.Length, null);
				array2 = TrustedStripLeadingZeroInts(array2);
			}
			exponent = Operator.UnsignedRightShift(exponent, 1);
			if (exponent != 0)
			{
				array = squareToLen(array, array.Length, null);
				array = TrustedStripLeadingZeroInts(array);
			}
		}
		return new BigInteger(array2, signum);
	}

	private int[] MultiplyToLen(int[] x, int xlen, int[] y, int ylen, int[] z)
	{
		int num = xlen - 1;
		int num2 = ylen - 1;
		if (z == null || z.Length < xlen + ylen)
		{
			z = new int[xlen + ylen];
		}
		long num3 = 0L;
		int num4 = num2;
		int num5 = num2 + 1 + num;
		while (num4 >= 0)
		{
			long num6 = (y[num4] & 0xFFFFFFFFu) * (x[num] & 0xFFFFFFFFu) + num3;
			z[num5] = (int)num6;
			num3 = Operator.UnsignedRightShift(num6, 32);
			num4--;
			num5--;
		}
		z[num] = (int)num3;
		for (int num7 = num - 1; num7 >= 0; num7--)
		{
			num3 = 0L;
			int num8 = num2;
			int num9 = num2 + 1 + num7;
			while (num8 >= 0)
			{
				long num10 = (y[num8] & 0xFFFFFFFFu) * (x[num7] & 0xFFFFFFFFu) + (z[num9] & 0xFFFFFFFFu) + num3;
				z[num9] = (int)num10;
				num3 = Operator.UnsignedRightShift(num10, 32);
				num8--;
				num9--;
			}
			z[num7] = (int)num3;
		}
		return z;
	}

	private static int mulAdd(int[] output, int[] input, int offset, int len, int k)
	{
		long num = k & 0xFFFFFFFFu;
		long num2 = 0L;
		offset = output.Length - offset - 1;
		for (int num3 = len - 1; num3 >= 0; num3--)
		{
			long num4 = (input[num3] & 0xFFFFFFFFu) * num + (output[offset] & 0xFFFFFFFFu) + num2;
			output[offset--] = (int)num4;
			num2 = Operator.UnsignedRightShift(num4, 32);
		}
		return (int)num2;
	}

	private static int[] squareToLen(int[] x, int len, int[] z)
	{
		int num = len << 1;
		if (z == null || z.Length < num)
		{
			z = new int[num];
		}
		int num2 = 0;
		int i = 0;
		int num3 = 0;
		for (; i < len; i++)
		{
			long num4 = x[i] & 0xFFFFFFFFu;
			long num5 = num4 * num4;
			z[num3++] = (num2 << 31) | (int)Operator.UnsignedRightShift(num5, 33);
			z[num3++] = (int)Operator.UnsignedRightShift(num5, 1);
			num2 = (int)num5;
		}
		int num6 = len;
		int num7 = 1;
		while (num6 > 0)
		{
			int k = x[num6 - 1];
			k = mulAdd(z, x, num7, num6 - 1, k);
			addOne(z, num7 - 1, num6, k);
			num6--;
			num7 += 2;
		}
		PrimitiveLeftShift(z, num, 1);
		z[num - 1] |= x[len - 1] & 1;
		return z;
	}

	public static void PrimitiveLeftShift(int[] a, int len, int n)
	{
		if (len != 0 && n != 0)
		{
			int val = 32 - n;
			int i = 0;
			int num = a[i];
			for (int num2 = i + len - 1; i < num2; i++)
			{
				int num3 = num;
				num = a[i + 1];
				a[i] = (num3 << n) | Operator.UnsignedRightShift(num, val);
			}
			a[len - 1] <<= n;
		}
	}

	private static int addOne(int[] a, int offset, int mlen, int carry)
	{
		offset = a.Length - 1 - mlen - offset;
		long num = (a[offset] & 0xFFFFFFFFu) + (carry & 0xFFFFFFFFu);
		a[offset] = (int)num;
		if (num >> 32 == 0L)
		{
			return 0;
		}
		while (--mlen >= 0)
		{
			if (--offset < 0)
			{
				return 1;
			}
			a[offset]++;
			if (a[offset] != 0)
			{
				return 0;
			}
		}
		return 1;
	}

	public int Signum()
	{
		return _signum;
	}

	public byte[] ToByteArray()
	{
		int num = BitLength() / 8 + 1;
		byte[] array = new byte[num];
		int num2 = num - 1;
		int num3 = 4;
		int num4 = 0;
		int num5 = 0;
		while (num2 >= 0)
		{
			if (num3 == 4)
			{
				num4 = GetInt(num5++);
				num3 = 1;
			}
			else
			{
				num4 = Operator.UnsignedRightShift(num4, 8);
				num3++;
			}
			array[num2] = (byte)num4;
			num2--;
		}
		return array;
	}

	private int intLength()
	{
		return Operator.UnsignedRightShift(BitLength(), 5) + 1;
	}

	private int signBit()
	{
		if (_signum >= 0)
		{
			return 0;
		}
		return 1;
	}

	private int signInt()
	{
		if (_signum >= 0)
		{
			return 0;
		}
		return -1;
	}

	private int GetInt(int n)
	{
		if (n < 0)
		{
			return 0;
		}
		if (n >= mag.Length)
		{
			return signInt();
		}
		int num = mag[mag.Length - n - 1];
		if (_signum < 0)
		{
			if (n > FirstNonzeroIntNum())
			{
				return ~num;
			}
			return -num;
		}
		return num;
	}

	private int FirstNonzeroIntNum()
	{
		int num = firstNonzeroIntNum - 2;
		if (num == -2)
		{
			num = 0;
			int num2 = mag.Length;
			int num3 = num2 - 1;
			while (num3 >= 0 && mag[num3] == 0)
			{
				num3--;
			}
			num = num2 - num3 - 1;
			firstNonzeroIntNum = num + 2;
		}
		return num;
	}

	private static int[] stripLeadingZeroBytes(byte[] a)
	{
		int num = a.Length;
		int i;
		for (i = 0; i < num && a[i] == 0; i++)
		{
		}
		int num2 = Operator.UnsignedRightShift(num - i + 3, 2);
		int[] array = new int[num2];
		int num3 = num - 1;
		for (int num4 = num2 - 1; num4 >= 0; num4--)
		{
			array[num4] = a[num3--] & 0xFF;
			int val = num3 - i + 1;
			int num5 = Math.Min(3, val);
			for (int j = 8; j <= num5 << 3; j += 8)
			{
				array[num4] |= (a[num3--] & 0xFF) << j;
			}
		}
		return array;
	}

	private static int[] makePositive(byte[] a)
	{
		int num = a.Length;
		int i;
		for (i = 0; i < num && (sbyte)a[i] == -1; i++)
		{
		}
		int j;
		for (j = i; j < num && a[j] == 0; j++)
		{
		}
		int num2 = ((j == num) ? 1 : 0);
		int num3 = (num - i + num2 + 3) / 4;
		int[] array = new int[num3];
		int num4 = num - 1;
		for (int num5 = num3 - 1; num5 >= 0; num5--)
		{
			array[num5] = a[num4--] & 0xFF;
			int num6 = Math.Min(3, num4 - i + 1);
			if (num6 < 0)
			{
				num6 = 0;
			}
			for (int k = 8; k <= 8 * num6; k += 8)
			{
				array[num5] |= (a[num4--] & 0xFF) << k;
			}
			int num7 = Operator.UnsignedRightShift(-1, 8 * (3 - num6));
			array[num5] = ~array[num5] & num7;
		}
		for (int num8 = array.Length - 1; num8 >= 0; num8--)
		{
			array[num8] = (int)((array[num8] & 0xFFFFFFFFu) + 1);
			if (array[num8] != 0)
			{
				break;
			}
		}
		return array;
	}

	private static int[] makePositive(int[] a)
	{
		int i;
		for (i = 0; i < a.Length && a[i] == -1; i++)
		{
		}
		int j;
		for (j = i; j < a.Length && a[j] == 0; j++)
		{
		}
		int num = ((j == a.Length) ? 1 : 0);
		int[] array = new int[a.Length - i + num];
		for (int k = i; k < a.Length; k++)
		{
			array[k - i + num] = ~a[k];
		}
		int num2 = array.Length - 1;
		while (++array[num2] == 0)
		{
			num2--;
		}
		return array;
	}

	public static int NumberOfLeadingZeros(int i)
	{
		if (i == 0)
		{
			return 32;
		}
		int num = 1;
		if (Operator.UnsignedRightShift(i, 16) == 0)
		{
			num += 16;
			i <<= 16;
		}
		if (Operator.UnsignedRightShift(i, 24) == 0)
		{
			num += 8;
			i <<= 8;
		}
		if (Operator.UnsignedRightShift(i, 28) == 0)
		{
			num += 4;
			i <<= 4;
		}
		if (Operator.UnsignedRightShift(i, 30) == 0)
		{
			num += 2;
			i <<= 2;
		}
		return num - Operator.UnsignedRightShift(i, 31);
	}

	public static int NumberOfTrailingZeros(int i)
	{
		if (i == 0)
		{
			return 32;
		}
		int num = 31;
		int num2 = i << 16;
		if (num2 != 0)
		{
			num -= 16;
			i = num2;
		}
		num2 = i << 8;
		if (num2 != 0)
		{
			num -= 8;
			i = num2;
		}
		num2 = i << 4;
		if (num2 != 0)
		{
			num -= 4;
			i = num2;
		}
		num2 = i << 2;
		if (num2 != 0)
		{
			num -= 2;
			i = num2;
		}
		return num - Operator.UnsignedRightShift(i << 1, 31);
	}

	public static int BitCountForInt(int i)
	{
		uint num = (uint)i;
		num -= (num >> 1) & 0x55555555;
		num = (num & 0x33333333) + ((num >> 2) & 0x33333333);
		num = (num + (num >> 4)) & 0xF0F0F0F;
		num += num >> 8;
		num += num >> 16;
		return (int)(num & 0x3F);
	}

	public int CompareTo(BigInteger val)
	{
		if (_signum == val._signum)
		{
			return _signum switch
			{
				1 => compareMagnitude(val), 
				-1 => val.compareMagnitude(this), 
				_ => 0, 
			};
		}
		if (_signum <= val._signum)
		{
			return -1;
		}
		return 1;
	}

	private int compareMagnitude(BigInteger val)
	{
		int[] array = mag;
		int num = array.Length;
		int[] array2 = val.mag;
		int num2 = array2.Length;
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		for (int i = 0; i < num; i++)
		{
			int num3 = array[i];
			int num4 = array2[i];
			if (num3 != num4)
			{
				if ((num3 & 0xFFFFFFFFu) >= (num4 & 0xFFFFFFFFu))
				{
					return 1;
				}
				return -1;
			}
		}
		return 0;
	}

	public override bool Equals(object x)
	{
		if (x == this)
		{
			return true;
		}
		if (!(x is BigInteger) || x == null)
		{
			return false;
		}
		BigInteger bigInteger = (BigInteger)x;
		if (bigInteger._signum != _signum)
		{
			return false;
		}
		int[] array = mag;
		int num = array.Length;
		int[] array2 = bigInteger.mag;
		if (num != array2.Length)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (array2[i] != array[i])
			{
				return false;
			}
		}
		return true;
	}

	public BigInteger Min(BigInteger val)
	{
		if (CompareTo(val) >= 0)
		{
			return val;
		}
		return this;
	}

	public BigInteger Max(BigInteger val)
	{
		if (CompareTo(val) <= 0)
		{
			return val;
		}
		return this;
	}

	public override int GetHashCode()
	{
		int num = 0;
		for (int i = 0; i < mag.Length; i++)
		{
			num = (int)(31 * num + (mag[i] & 0xFFFFFFFFu));
		}
		return num * _signum;
	}

	public int IntValue()
	{
		return GetInt(0);
	}

	public BigInteger ShiftLeft(int n)
	{
		if (_signum == 0)
		{
			return ZERO;
		}
		if (n == 0)
		{
			return this;
		}
		if (n < 0)
		{
			if (n == int.MinValue)
			{
				throw new ArithmeticException("Shift distance of Integer.Min_VALUE not supported.");
			}
			return ShiftRight(-n);
		}
		int num = Operator.UnsignedRightShift(n, 5);
		int num2 = n & 0x1F;
		int num3 = mag.Length;
		int[] array = null;
		if (num2 == 0)
		{
			array = new int[num3 + num];
			for (int i = 0; i < num3; i++)
			{
				array[i] = mag[i];
			}
		}
		else
		{
			int num4 = 0;
			int val = 32 - num2;
			int num5 = Operator.UnsignedRightShift(mag[0], val);
			if (num5 != 0)
			{
				array = new int[num3 + num + 1];
				array[num4++] = num5;
			}
			else
			{
				array = new int[num3 + num];
			}
			int num6 = 0;
			while (num6 < num3 - 1)
			{
				array[num4++] = (mag[num6++] << num2) | Operator.UnsignedRightShift(mag[num6], val);
			}
			array[num4] = mag[num6] << num2;
		}
		return new BigInteger(array, _signum);
	}

	public long LongValue()
	{
		long num = 0L;
		for (int num2 = 1; num2 >= 0; num2--)
		{
			num = (num << 32) + (GetInt(num2) & 0xFFFFFFFFu);
		}
		return num;
	}

	public BigInteger ShiftRight(int n)
	{
		if (n == 0)
		{
			return this;
		}
		if (n < 0)
		{
			if (n == int.MinValue)
			{
				throw new ArithmeticException("Shift distance of Integer.Min_VALUE not supported.");
			}
			return ShiftLeft(-n);
		}
		int num = Operator.UnsignedRightShift(n, 5);
		int num2 = n & 0x1F;
		int num3 = mag.Length;
		int[] array = null;
		if (num >= num3)
		{
			if (_signum < 0)
			{
				return negConst[1];
			}
			return ZERO;
		}
		if (num2 == 0)
		{
			int num4 = num3 - num;
			array = new int[num4];
			for (int i = 0; i < num4; i++)
			{
				array[i] = mag[i];
			}
		}
		else
		{
			int num5 = 0;
			int num6 = Operator.UnsignedRightShift(mag[0], num2);
			if (num6 != 0)
			{
				array = new int[num3 - num];
				array[num5++] = num6;
			}
			else
			{
				array = new int[num3 - num - 1];
			}
			int num7 = 32 - num2;
			int num8 = 0;
			while (num8 < num3 - num - 1)
			{
				array[num5++] = (mag[num8++] << num7) | Operator.UnsignedRightShift(mag[num8], num2);
			}
		}
		if (_signum < 0)
		{
			bool flag = false;
			int num9 = num3 - 1;
			int num10 = num3 - num;
			while (num9 >= num10 && !flag)
			{
				flag = mag[num9] != 0;
				num9--;
			}
			if (!flag && num2 != 0)
			{
				flag = mag[num3 - num - 1] << 32 - num2 != 0;
			}
			if (flag)
			{
				array = Increment(array);
			}
		}
		return new BigInteger(array, _signum);
	}

	private int[] Increment(int[] val)
	{
		int num = 0;
		int num2 = val.Length - 1;
		while (num2 >= 0 && num == 0)
		{
			num = ++val[num2];
			num2--;
		}
		if (num == 0)
		{
			val = new int[val.Length + 1];
			val[0] = 1;
		}
		return val;
	}

	public BigInteger and(BigInteger val)
	{
		int[] array = new int[Math.Max(intLength(), val.intLength())];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = GetInt(array.Length - i - 1) & val.GetInt(array.Length - i - 1);
		}
		return ValueOf(array);
	}

	public BigInteger Not()
	{
		int[] array = new int[intLength()];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ~GetInt(array.Length - i - 1);
		}
		return ValueOf(array);
	}

	public BigInteger Or(BigInteger val)
	{
		int[] array = new int[Math.Max(intLength(), val.intLength())];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = GetInt(array.Length - i - 1) | val.GetInt(array.Length - i - 1);
		}
		return ValueOf(array);
	}

	private BigInteger Multiply(long v)
	{
		if (v == 0L || _signum == 0)
		{
			return ZERO;
		}
		if (v == long.MinValue)
		{
			return Multiply(ValueOf(v));
		}
		int signum = ((v > 0) ? _signum : (-_signum));
		if (v < 0)
		{
			v = -v;
		}
		long num = Operator.UnsignedRightShift(v, 32);
		long num2 = v & 0xFFFFFFFFu;
		int num3 = mag.Length;
		int[] array = mag;
		int[] array2 = ((num == 0L) ? new int[num3 + 1] : new int[num3 + 2]);
		long num4 = 0L;
		int num5 = array2.Length - 1;
		for (int num6 = num3 - 1; num6 >= 0; num6--)
		{
			long num7 = (array[num6] & 0xFFFFFFFFu) * num2 + num4;
			array2[num5--] = (int)num7;
			num4 = Operator.UnsignedRightShift(num7, 32);
		}
		array2[num5] = (int)num4;
		if (num != 0L)
		{
			num4 = 0L;
			num5 = array2.Length - 2;
			for (int num8 = num3 - 1; num8 >= 0; num8--)
			{
				long num9 = (array[num8] & 0xFFFFFFFFu) * num + (array2[num5] & 0xFFFFFFFFu) + num4;
				array2[num5--] = (int)num9;
				num4 = Operator.UnsignedRightShift(num9, 32);
			}
			array2[0] = (int)num4;
		}
		if (num4 == 0L)
		{
			array2 = Arrays.CopyOfRange(array2, 1, array2.Length);
		}
		return new BigInteger(array2, signum);
	}

	public BigInteger Multiply(BigInteger val)
	{
		if (val._signum == 0 || _signum == 0)
		{
			return ZERO;
		}
		return new BigInteger(TrustedStripLeadingZeroInts(MultiplyToLen(mag, mag.Length, val.mag, val.mag.Length, null)), (_signum == val._signum) ? 1 : (-1));
	}

	public BigInteger Add(BigInteger val)
	{
		if (val._signum == 0)
		{
			return this;
		}
		if (_signum == 0)
		{
			return val;
		}
		if (val._signum == _signum)
		{
			return new BigInteger(add(mag, val.mag), _signum);
		}
		int num = compareMagnitude(val);
		if (num == 0)
		{
			return ZERO;
		}
		return new BigInteger(TrustedStripLeadingZeroInts((num > 0) ? Subtract(mag, val.mag) : Subtract(val.mag, mag)), (num == _signum) ? 1 : (-1));
	}

	private static int[] add(int[] x, int[] y)
	{
		if (x.Length < y.Length)
		{
			int[] array = x;
			x = y;
			y = array;
		}
		int num = x.Length;
		int num2 = y.Length;
		int[] array2 = new int[num];
		long num3 = 0L;
		while (num2 > 0)
		{
			num3 = (x[--num] & 0xFFFFFFFFu) + (y[--num2] & 0xFFFFFFFFu) + Operator.UnsignedRightShift(num3, 32);
			array2[num] = (int)num3;
		}
		bool flag = Operator.UnsignedRightShift(num3, 32) != 0;
		while ((num > 0) & flag)
		{
			flag = (array2[--num] = x[num] + 1) == 0;
		}
		while (num > 0)
		{
			array2[--num] = x[num];
		}
		if (flag)
		{
			int[] array3 = new int[array2.Length + 1];
			Array.Copy(array2, 0, array3, 1, array2.Length);
			array3[0] = 1;
			return array3;
		}
		return array2;
	}

	public BigInteger Subtract(BigInteger val)
	{
		if (val._signum == 0)
		{
			return this;
		}
		if (_signum == 0)
		{
			return val.Negate();
		}
		if (val._signum != _signum)
		{
			return new BigInteger(add(mag, val.mag), _signum);
		}
		int num = compareMagnitude(val);
		if (num == 0)
		{
			return ZERO;
		}
		return new BigInteger(TrustedStripLeadingZeroInts((num > 0) ? Subtract(mag, val.mag) : Subtract(val.mag, mag)), (num == _signum) ? 1 : (-1));
	}

	private static int[] Subtract(int[] big, int[] little)
	{
		int num = big.Length;
		int[] array = new int[num];
		int num2 = little.Length;
		long num3 = 0L;
		while (num2 > 0)
		{
			num3 = (big[--num] & 0xFFFFFFFFu) - (little[--num2] & 0xFFFFFFFFu) + (num3 >> 32);
			array[num] = (int)num3;
		}
		bool flag = num3 >> 32 != 0;
		while ((num > 0) & flag)
		{
			flag = (array[--num] = big[num] - 1) == -1;
		}
		while (num > 0)
		{
			array[--num] = big[num];
		}
		return array;
	}

	public BigInteger Divide(BigInteger val)
	{
		MutableBigInteger mutableBigInteger = new MutableBigInteger();
		MutableBigInteger mutableBigInteger2 = new MutableBigInteger(mag);
		MutableBigInteger b = new MutableBigInteger(val.mag);
		mutableBigInteger2.divide(b, mutableBigInteger);
		return mutableBigInteger.toBigInteger((_signum == val._signum) ? 1 : (-1));
	}

	public static BigInteger operator >>(BigInteger bi1, int shiftVal)
	{
		return bi1.ShiftRight(shiftVal);
	}

	public static BigInteger operator <<(BigInteger bi1, int shiftVal)
	{
		return bi1.ShiftLeft(shiftVal);
	}

	public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
	{
		return bi1.and(bi2);
	}

	public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Or(bi2);
	}

	public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Multiply(bi2);
	}

	public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Add(bi2);
	}

	public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Subtract(bi2);
	}

	public static bool operator <(BigInteger bi1, BigInteger bi2)
	{
		return bi1.CompareTo(bi2) < 0;
	}

	public static bool operator >(BigInteger bi1, BigInteger bi2)
	{
		return bi1.CompareTo(bi2) > 0;
	}

	public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Divide(bi2);
	}

	public static bool operator ==(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Equals(bi2);
	}

	public static bool operator !=(BigInteger bi1, BigInteger bi2)
	{
		return !(bi1 == bi2);
	}
}
