using System;

namespace NPOI.Util;

internal class MutableBigInteger
{
	private int[] _value;

	private int intLen;

	private int offset;

	private static readonly MutableBigInteger One = new MutableBigInteger(1);

	private const long LONG_MASK = 4294967295L;

	private const long INFLATED = long.MinValue;

	public MutableBigInteger()
	{
		_value = new int[1];
		intLen = 0;
	}

	public MutableBigInteger(int val)
	{
		_value = new int[1];
		intLen = 1;
		_value[0] = val;
	}

	public MutableBigInteger(int[] val)
	{
		_value = val;
		intLen = val.Length;
	}

	public static int[] ArraysCopyOf(int[] original, int newLength)
	{
		int[] array = new int[newLength];
		Array.Copy(original, 0, array, 0, Math.Min(original.Length, newLength));
		return array;
	}

	public static long[] ArraysCopyOf(long[] original, int newLength)
	{
		long[] array = new long[newLength];
		Array.Copy(original, 0, array, 0, Math.Min(original.Length, newLength));
		return array;
	}

	public static int[] ArraysCopyOfRange(int[] original, int from, int to)
	{
		int num = to - from;
		if (num < 0)
		{
			throw new ArgumentException(from + " > " + to);
		}
		int[] array = new int[num];
		Array.Copy(original, from, array, 0, Math.Min(original.Length - from, num));
		return array;
	}

	public static long[] ArraysCopyOfRange(long[] original, int from, int to)
	{
		int num = to - from;
		if (num < 0)
		{
			throw new ArgumentException(from + " > " + to);
		}
		long[] array = new long[num];
		Array.Copy(original, from, array, 0, Math.Min(original.Length - from, num));
		return array;
	}

	private MutableBigInteger(BigInteger b)
	{
		intLen = b.mag.Length;
		_value = ArraysCopyOf(b.mag, intLen);
	}

	private MutableBigInteger(MutableBigInteger val)
	{
		intLen = val.intLen;
		_value = ArraysCopyOfRange(val._value, val.offset, val.offset + intLen);
	}

	private int[] getMagnitudeArray()
	{
		if (offset > 0 || _value.Length != intLen)
		{
			return ArraysCopyOfRange(_value, offset, offset + intLen);
		}
		return _value;
	}

	private long toLong()
	{
		if (intLen == 0)
		{
			return 0L;
		}
		long num = _value[offset] & 0xFFFFFFFFu;
		if (intLen != 2)
		{
			return num;
		}
		return (num << 32) | (_value[offset + 1] & 0xFFFFFFFFu);
	}

	public BigInteger toBigInteger(int sign)
	{
		if (intLen == 0 || sign == 0)
		{
			return BigInteger.ZERO;
		}
		return new BigInteger(getMagnitudeArray(), sign);
	}

	private void clear()
	{
		offset = (intLen = 0);
		int i = 0;
		for (int num = _value.Length; i < num; i++)
		{
			_value[i] = 0;
		}
	}

	private void reset()
	{
		offset = (intLen = 0);
	}

	private int compare(MutableBigInteger b)
	{
		int num = b.intLen;
		if (intLen < num)
		{
			return -1;
		}
		if (intLen > num)
		{
			return 1;
		}
		int[] value = b._value;
		int num2 = offset;
		int num3 = b.offset;
		while (num2 < intLen + offset)
		{
			int num4 = (int)(_value[num2] + 2147483648u);
			int num5 = (int)(value[num3] + 2147483648u);
			if (num4 < num5)
			{
				return -1;
			}
			if (num4 > num5)
			{
				return 1;
			}
			num2++;
			num3++;
		}
		return 0;
	}

	private int compareHalf(MutableBigInteger b)
	{
		int num = b.intLen;
		int num2 = intLen;
		if (num2 <= 0)
		{
			if (num > 0)
			{
				return -1;
			}
			return 0;
		}
		if (num2 > num)
		{
			return 1;
		}
		if (num2 < num - 1)
		{
			return -1;
		}
		int[] value = b._value;
		int num3 = 0;
		int num4 = 0;
		if (num2 != num)
		{
			if (value[num3] != 1)
			{
				return -1;
			}
			num3++;
			num4 = int.MinValue;
		}
		int[] value2 = _value;
		int num5 = offset;
		int num6 = num3;
		while (num5 < num2 + offset)
		{
			int num7 = value[num6++];
			long num8 = (Operator.UnsignedRightShift(num7, 1) + num4) & 0xFFFFFFFFu;
			long num9 = value2[num5++] & 0xFFFFFFFFu;
			if (num9 != num8)
			{
				if (num9 >= num8)
				{
					return 1;
				}
				return -1;
			}
			num4 = (num7 & 1) << 31;
		}
		if (num4 != 0)
		{
			return -1;
		}
		return 0;
	}

	private int getLowestSetBit()
	{
		if (intLen == 0)
		{
			return -1;
		}
		int num = intLen - 1;
		while (num > 0 && _value[num + offset] == 0)
		{
			num--;
		}
		int num2 = _value[num + offset];
		if (num2 == 0)
		{
			return -1;
		}
		return (intLen - 1 - num << 5) + BigInteger.NumberOfTrailingZeros(num2);
	}

	private int getInt(int index)
	{
		return _value[offset + index];
	}

	private long getLong(int index)
	{
		return _value[offset + index] & 0xFFFFFFFFu;
	}

	private void normalize()
	{
		if (intLen == 0)
		{
			offset = 0;
			return;
		}
		int num = offset;
		if (_value[num] == 0)
		{
			int num2 = num + intLen;
			do
			{
				num++;
			}
			while (num < num2 && _value[num] == 0);
			int num3 = num - offset;
			intLen -= num3;
			offset = ((intLen != 0) ? (offset + num3) : 0);
		}
	}

	private void ensureCapacity(int len)
	{
		if (_value.Length < len)
		{
			_value = new int[len];
			offset = 0;
			intLen = len;
		}
	}

	private int[] toIntArray()
	{
		int[] array = new int[intLen];
		for (int i = 0; i < intLen; i++)
		{
			array[i] = _value[offset + i];
		}
		return array;
	}

	private void setInt(int index, int val)
	{
		_value[offset + index] = val;
	}

	private void setValue(int[] val, int length)
	{
		_value = val;
		intLen = length;
		offset = 0;
	}

	private void copyValue(MutableBigInteger src)
	{
		int num = src.intLen;
		if (_value.Length < num)
		{
			_value = new int[num];
		}
		Array.Copy(src._value, src.offset, _value, 0, num);
		intLen = num;
		offset = 0;
	}

	private void copyValue(int[] val)
	{
		int num = val.Length;
		if (_value.Length < num)
		{
			_value = new int[num];
		}
		Array.Copy(val, 0, _value, 0, num);
		intLen = num;
		offset = 0;
	}

	private bool isOne()
	{
		if (intLen == 1)
		{
			return _value[offset] == 1;
		}
		return false;
	}

	private bool isZero()
	{
		return intLen == 0;
	}

	private bool isEven()
	{
		if (intLen != 0)
		{
			return (_value[offset + intLen - 1] & 1) == 0;
		}
		return true;
	}

	private bool isOdd()
	{
		if (!isZero())
		{
			return (_value[offset + intLen - 1] & 1) == 1;
		}
		return false;
	}

	private bool isNormal()
	{
		if (intLen + offset > _value.Length)
		{
			return false;
		}
		if (intLen == 0)
		{
			return true;
		}
		return _value[offset] != 0;
	}

	public string toString()
	{
		return toBigInteger(1).ToString();
	}

	private void rightShift(int n)
	{
		if (intLen == 0)
		{
			return;
		}
		int num = Operator.UnsignedRightShift(n, 5);
		int num2 = n & 0x1F;
		intLen -= num;
		if (num2 != 0)
		{
			int num3 = BigInteger.BitLengthForInt(_value[offset]);
			if (num2 >= num3)
			{
				primitiveLeftShift(32 - num2);
				intLen--;
			}
			else
			{
				primitiveRightShift(num2);
			}
		}
	}

	private void leftShift(int n)
	{
		if (intLen == 0)
		{
			return;
		}
		int num = Operator.UnsignedRightShift(n, 5);
		int num2 = n & 0x1F;
		int num3 = BigInteger.BitLengthForInt(_value[offset]);
		if (n <= 32 - num3)
		{
			primitiveLeftShift(num2);
			return;
		}
		int num4 = intLen + num + 1;
		if (num2 <= 32 - num3)
		{
			num4--;
		}
		if (_value.Length < num4)
		{
			int[] array = new int[num4];
			for (int i = 0; i < intLen; i++)
			{
				array[i] = _value[offset + i];
			}
			setValue(array, num4);
		}
		else if (_value.Length - offset >= num4)
		{
			for (int j = 0; j < num4 - intLen; j++)
			{
				_value[offset + intLen + j] = 0;
			}
		}
		else
		{
			for (int k = 0; k < intLen; k++)
			{
				_value[k] = _value[offset + k];
			}
			for (int l = intLen; l < num4; l++)
			{
				_value[l] = 0;
			}
			offset = 0;
		}
		intLen = num4;
		if (num2 != 0)
		{
			if (num2 <= 32 - num3)
			{
				primitiveLeftShift(num2);
			}
			else
			{
				primitiveRightShift(32 - num2);
			}
		}
	}

	private int divadd(int[] a, int[] result, int offset)
	{
		long num = 0L;
		for (int num2 = a.Length - 1; num2 >= 0; num2--)
		{
			long num3 = (a[num2] & 0xFFFFFFFFu) + (result[num2 + offset] & 0xFFFFFFFFu) + num;
			result[num2 + offset] = (int)num3;
			num = Operator.UnsignedRightShift(num3, 32);
		}
		return (int)num;
	}

	private int mulsub(int[] q, int[] a, int x, int len, int offset)
	{
		long num = x & 0xFFFFFFFFu;
		long num2 = 0L;
		offset += len;
		for (int num3 = len - 1; num3 >= 0; num3--)
		{
			long num4 = (a[num3] & 0xFFFFFFFFu) * num + num2;
			long num5 = q[offset] - num4;
			q[offset--] = (int)num5;
			num2 = Operator.UnsignedRightShift(num4, 32) + (((num5 & 0xFFFFFFFFu) > ((int)(~num4) & 0xFFFFFFFFu)) ? 1 : 0);
		}
		return (int)num2;
	}

	private void primitiveRightShift(int n)
	{
		int[] value = _value;
		int num = 32 - n;
		int num2 = offset + intLen - 1;
		int num3 = value[num2];
		while (num2 > offset)
		{
			int operand = num3;
			num3 = value[num2 - 1];
			value[num2] = (num3 << num) | Operator.UnsignedRightShift(operand, n);
			num2--;
		}
		value[offset] = Operator.UnsignedRightShift(value[offset], n);
	}

	private void primitiveLeftShift(int n)
	{
		int[] value = _value;
		int val = 32 - n;
		int i = offset;
		int num = value[i];
		for (int num2 = i + intLen - 1; i < num2; i++)
		{
			int num3 = num;
			num = value[i + 1];
			value[i] = (num3 << n) | Operator.UnsignedRightShift(num, val);
		}
		value[offset + intLen - 1] <<= n;
	}

	private void add(MutableBigInteger addend)
	{
		int num = intLen;
		int num2 = addend.intLen;
		int num3 = ((intLen > addend.intLen) ? intLen : addend.intLen);
		int[] array = ((_value.Length < num3) ? new int[num3] : _value);
		int num4 = array.Length - 1;
		long num5 = 0L;
		while (num > 0 && num2 > 0)
		{
			num--;
			num2--;
			long num6 = (_value[num + offset] & 0xFFFFFFFFu) + (addend._value[num2 + addend.offset] & 0xFFFFFFFFu) + num5;
			array[num4--] = (int)num6;
			num5 = Operator.UnsignedRightShift(num6, 32);
		}
		while (num > 0)
		{
			num--;
			if (num5 == 0L && array == _value && num4 == num + offset)
			{
				return;
			}
			long num6 = (_value[num + offset] & 0xFFFFFFFFu) + num5;
			array[num4--] = (int)num6;
			num5 = Operator.UnsignedRightShift(num6, 32);
		}
		while (num2 > 0)
		{
			num2--;
			long num6 = (addend._value[num2 + addend.offset] & 0xFFFFFFFFu) + num5;
			array[num4--] = (int)num6;
			num5 = Operator.UnsignedRightShift(num6, 32);
		}
		if (num5 > 0)
		{
			num3++;
			if (array.Length < num3)
			{
				int[] array2 = new int[num3];
				Array.Copy(array, 0, array2, 1, array.Length);
				array2[0] = 1;
				array = array2;
			}
			else
			{
				array[num4--] = 1;
			}
		}
		_value = array;
		intLen = num3;
		offset = array.Length - num3;
	}

	private int subtract(MutableBigInteger b)
	{
		MutableBigInteger mutableBigInteger = this;
		int[] array = _value;
		int num = mutableBigInteger.compare(b);
		if (num == 0)
		{
			reset();
			return 0;
		}
		if (num < 0)
		{
			MutableBigInteger mutableBigInteger2 = mutableBigInteger;
			mutableBigInteger = b;
			b = mutableBigInteger2;
		}
		int num2 = mutableBigInteger.intLen;
		if (array.Length < num2)
		{
			array = new int[num2];
		}
		long num3 = 0L;
		int num4 = mutableBigInteger.intLen;
		int num5 = b.intLen;
		int num6 = array.Length - 1;
		while (num5 > 0)
		{
			num4--;
			num5--;
			num3 = (mutableBigInteger._value[num4 + mutableBigInteger.offset] & 0xFFFFFFFFu) - (b._value[num5 + b.offset] & 0xFFFFFFFFu) - (int)(-(num3 >> 32));
			array[num6--] = (int)num3;
		}
		while (num4 > 0)
		{
			num4--;
			num3 = (mutableBigInteger._value[num4 + mutableBigInteger.offset] & 0xFFFFFFFFu) - (int)(-(num3 >> 32));
			array[num6--] = (int)num3;
		}
		_value = array;
		intLen = num2;
		offset = _value.Length - num2;
		normalize();
		return num;
	}

	private int difference(MutableBigInteger b)
	{
		MutableBigInteger mutableBigInteger = this;
		int num = mutableBigInteger.compare(b);
		if (num == 0)
		{
			return 0;
		}
		if (num < 0)
		{
			MutableBigInteger mutableBigInteger2 = mutableBigInteger;
			mutableBigInteger = b;
			b = mutableBigInteger2;
		}
		long num2 = 0L;
		int num3 = mutableBigInteger.intLen;
		int num4 = b.intLen;
		while (num4 > 0)
		{
			num3--;
			num4--;
			num2 = (mutableBigInteger._value[mutableBigInteger.offset + num3] & 0xFFFFFFFFu) - (b._value[b.offset + num4] & 0xFFFFFFFFu) - (int)(-(num2 >> 32));
			mutableBigInteger._value[mutableBigInteger.offset + num3] = (int)num2;
		}
		while (num3 > 0)
		{
			num3--;
			num2 = (mutableBigInteger._value[mutableBigInteger.offset + num3] & 0xFFFFFFFFu) - (int)(-(num2 >> 32));
			mutableBigInteger._value[mutableBigInteger.offset + num3] = (int)num2;
		}
		mutableBigInteger.normalize();
		return num;
	}

	private void multiply(MutableBigInteger y, MutableBigInteger z)
	{
		int num = intLen;
		int num2 = y.intLen;
		int num3 = num + num2;
		if (z._value.Length < num3)
		{
			z._value = new int[num3];
		}
		z.offset = 0;
		z.intLen = num3;
		long num4 = 0L;
		int num5 = num2 - 1;
		int num6 = num2 + num - 1;
		while (num5 >= 0)
		{
			long num7 = (y._value[num5 + y.offset] & 0xFFFFFFFFu) * (_value[num - 1 + offset] & 0xFFFFFFFFu) + num4;
			z._value[num6] = (int)num7;
			num4 = Operator.UnsignedRightShift(num7, 32);
			num5--;
			num6--;
		}
		z._value[num - 1] = (int)num4;
		for (int num8 = num - 2; num8 >= 0; num8--)
		{
			num4 = 0L;
			int num9 = num2 - 1;
			int num10 = num2 + num8;
			while (num9 >= 0)
			{
				long num11 = (y._value[num9 + y.offset] & 0xFFFFFFFFu) * (_value[num8 + offset] & 0xFFFFFFFFu) + (z._value[num10] & 0xFFFFFFFFu) + num4;
				z._value[num10] = (int)num11;
				num4 = Operator.UnsignedRightShift(num11, 32);
				num9--;
				num10--;
			}
			z._value[num8] = (int)num4;
		}
		z.normalize();
	}

	public void mul(int y, MutableBigInteger z)
	{
		switch (y)
		{
		case 1:
			z.copyValue(this);
			return;
		case 0:
			z.clear();
			return;
		}
		long num = y & 0xFFFFFFFFu;
		int[] array = ((z._value.Length < intLen + 1) ? new int[intLen + 1] : z._value);
		long num2 = 0L;
		for (int num3 = intLen - 1; num3 >= 0; num3--)
		{
			long num4 = num * (_value[num3 + offset] & 0xFFFFFFFFu) + num2;
			array[num3 + 1] = (int)num4;
			num2 = Operator.UnsignedRightShift(num4, 32);
		}
		if (num2 == 0L)
		{
			z.offset = 1;
			z.intLen = intLen;
		}
		else
		{
			z.offset = 0;
			z.intLen = intLen + 1;
			array[0] = (int)num2;
		}
		z._value = array;
	}

	public int divideOneWord(int divisor, MutableBigInteger quotient)
	{
		long num = divisor & 0xFFFFFFFFu;
		if (intLen == 1)
		{
			long num2 = _value[offset] & 0xFFFFFFFFu;
			int num3 = (int)(num2 / num);
			int result = (int)(num2 - num3 * num);
			quotient._value[0] = num3;
			quotient.intLen = ((num3 != 0) ? 1 : 0);
			quotient.offset = 0;
			return result;
		}
		if (quotient._value.Length < intLen)
		{
			quotient._value = new int[intLen];
		}
		quotient.offset = 0;
		quotient.intLen = intLen;
		int num4 = BigInteger.NumberOfLeadingZeros(divisor);
		int num5 = _value[offset];
		long num6 = num5 & 0xFFFFFFFFu;
		if (num6 < num)
		{
			quotient._value[0] = 0;
		}
		else
		{
			quotient._value[0] = (int)(num6 / num);
			num5 = (int)(num6 - quotient._value[0] * num);
			num6 = num5 & 0xFFFFFFFFu;
		}
		int num7 = intLen;
		int[] array = new int[2];
		while (--num7 > 0)
		{
			long num8 = (num6 << 32) | (_value[offset + intLen - num7] & 0xFFFFFFFFu);
			if (num8 >= 0)
			{
				array[0] = (int)(num8 / num);
				array[1] = (int)(num8 - array[0] * num);
			}
			else
			{
				divWord(array, num8, divisor);
			}
			quotient._value[intLen - num7] = array[0];
			num5 = array[1];
			num6 = num5 & 0xFFFFFFFFu;
		}
		quotient.normalize();
		if (num4 > 0)
		{
			return num5 % divisor;
		}
		return num5;
	}

	public MutableBigInteger divide(MutableBigInteger b, MutableBigInteger quotient)
	{
		if (b.intLen == 0)
		{
			throw new ArithmeticException("BigInteger divide by zero");
		}
		if (intLen == 0)
		{
			quotient.intLen = quotient.offset;
			return new MutableBigInteger();
		}
		int num = compare(b);
		if (num < 0)
		{
			quotient.intLen = (quotient.offset = 0);
			return new MutableBigInteger(this);
		}
		if (num == 0)
		{
			quotient._value[0] = (quotient.intLen = 1);
			quotient.offset = 0;
			return new MutableBigInteger();
		}
		quotient.clear();
		if (b.intLen == 1)
		{
			int num2 = divideOneWord(b._value[b.offset], quotient);
			if (num2 == 0)
			{
				return new MutableBigInteger();
			}
			return new MutableBigInteger(num2);
		}
		int[] divisor = ArraysCopyOfRange(b._value, b.offset, b.offset + b.intLen);
		return divideMagnitude(divisor, quotient);
	}

	public long divide(long v, MutableBigInteger quotient)
	{
		if (v == 0L)
		{
			throw new ArithmeticException("BigInteger divide by zero");
		}
		if (intLen == 0)
		{
			quotient.intLen = (quotient.offset = 0);
			return 0L;
		}
		if (v < 0)
		{
			v = -v;
		}
		int num = (int)Operator.UnsignedRightShift(v, 32);
		quotient.clear();
		if (num == 0)
		{
			return divideOneWord((int)v, quotient) & 0xFFFFFFFFu;
		}
		int[] divisor = new int[2]
		{
			num,
			(int)(v & 0xFFFFFFFFu)
		};
		return divideMagnitude(divisor, quotient).toLong();
	}

	private MutableBigInteger divideMagnitude(int[] divisor, MutableBigInteger quotient)
	{
		MutableBigInteger mutableBigInteger = new MutableBigInteger(new int[intLen + 1]);
		Array.Copy(_value, offset, mutableBigInteger._value, 1, intLen);
		mutableBigInteger.intLen = intLen;
		mutableBigInteger.offset = 1;
		int num = mutableBigInteger.intLen;
		int num2 = divisor.Length;
		int num3 = num - num2 + 1;
		if (quotient._value.Length < num3)
		{
			quotient._value = new int[num3];
			quotient.offset = 0;
		}
		quotient.intLen = num3;
		int[] value = quotient._value;
		int num4 = BigInteger.NumberOfLeadingZeros(divisor[0]);
		if (num4 > 0)
		{
			BigInteger.PrimitiveLeftShift(divisor, num2, num4);
			mutableBigInteger.leftShift(num4);
		}
		if (mutableBigInteger.intLen == num)
		{
			mutableBigInteger.offset = 0;
			mutableBigInteger._value[0] = 0;
			mutableBigInteger.intLen++;
		}
		int num5 = divisor[0];
		long num6 = num5 & 0xFFFFFFFFu;
		int num7 = divisor[1];
		int[] array = new int[2];
		for (int i = 0; i < num3; i++)
		{
			int num8 = 0;
			int num9 = 0;
			bool flag = false;
			int num10 = mutableBigInteger._value[i + mutableBigInteger.offset];
			int num11 = (int)(num10 + 2147483648u);
			int num12 = mutableBigInteger._value[i + 1 + mutableBigInteger.offset];
			if (num10 == num5)
			{
				num8 = -1;
				num9 = num10 + num12;
				flag = num9 + 2147483648u < num11;
			}
			else
			{
				long num13 = ((long)num10 << 32) | (num12 & 0xFFFFFFFFu);
				if (num13 >= 0)
				{
					num8 = (int)(num13 / num6);
					num9 = (int)(num13 - num8 * num6);
				}
				else
				{
					divWord(array, num13, num5);
					num8 = array[0];
					num9 = array[1];
				}
			}
			if (num8 == 0)
			{
				continue;
			}
			if (!flag)
			{
				long num14 = mutableBigInteger._value[i + 2 + mutableBigInteger.offset] & 0xFFFFFFFFu;
				long two = ((num9 & 0xFFFFFFFFu) << 32) | num14;
				long num15 = (num7 & 0xFFFFFFFFu) * (num8 & 0xFFFFFFFFu);
				if (unsignedLongCompare(num15, two))
				{
					num8--;
					num9 = (int)((num9 & 0xFFFFFFFFu) + num6);
					if ((num9 & 0xFFFFFFFFu) >= num6)
					{
						num15 -= num7 & 0xFFFFFFFFu;
						two = ((num9 & 0xFFFFFFFFu) << 32) | num14;
						if (unsignedLongCompare(num15, two))
						{
							num8--;
						}
					}
				}
			}
			mutableBigInteger._value[i + mutableBigInteger.offset] = 0;
			if ((int)(mulsub(mutableBigInteger._value, divisor, num8, num2, i + mutableBigInteger.offset) + 2147483648u) > num11)
			{
				divadd(divisor, mutableBigInteger._value, i + 1 + mutableBigInteger.offset);
				num8--;
			}
			value[i] = num8;
		}
		if (num4 > 0)
		{
			mutableBigInteger.rightShift(num4);
		}
		quotient.normalize();
		mutableBigInteger.normalize();
		return mutableBigInteger;
	}

	private bool unsignedLongCompare(long one, long two)
	{
		return one + long.MinValue > two + long.MinValue;
	}

	private void divWord(int[] result, long n, int d)
	{
		long num = d & 0xFFFFFFFFu;
		if (num == 1)
		{
			result[0] = (int)n;
			result[1] = 0;
			return;
		}
		long num2 = Operator.UnsignedRightShift(n, 1) / Operator.UnsignedRightShift(num, 1);
		long num3 = n - num2 * num;
		while (num3 < 0)
		{
			num3 += num;
			num2--;
		}
		while (num3 >= num)
		{
			num3 -= num;
			num2++;
		}
		result[0] = (int)num2;
		result[1] = (int)num3;
	}

	private MutableBigInteger hybridGCD(MutableBigInteger b)
	{
		MutableBigInteger mutableBigInteger = this;
		MutableBigInteger quotient = new MutableBigInteger();
		while (b.intLen != 0)
		{
			if (Math.Abs(mutableBigInteger.intLen - b.intLen) < 2)
			{
				return mutableBigInteger.binaryGCD(b);
			}
			MutableBigInteger mutableBigInteger2 = mutableBigInteger.divide(b, quotient);
			mutableBigInteger = b;
			b = mutableBigInteger2;
		}
		return mutableBigInteger;
	}

	private MutableBigInteger binaryGCD(MutableBigInteger v)
	{
		MutableBigInteger mutableBigInteger = this;
		MutableBigInteger mutableBigInteger2 = new MutableBigInteger();
		int lowestSetBit = mutableBigInteger.getLowestSetBit();
		int lowestSetBit2 = v.getLowestSetBit();
		int num = ((lowestSetBit < lowestSetBit2) ? lowestSetBit : lowestSetBit2);
		if (num != 0)
		{
			mutableBigInteger.rightShift(num);
			v.rightShift(num);
		}
		bool num2 = num == lowestSetBit;
		MutableBigInteger mutableBigInteger3 = (num2 ? v : mutableBigInteger);
		int num3 = ((!num2) ? 1 : (-1));
		int lowestSetBit3;
		while ((lowestSetBit3 = mutableBigInteger3.getLowestSetBit()) >= 0)
		{
			mutableBigInteger3.rightShift(lowestSetBit3);
			if (num3 > 0)
			{
				mutableBigInteger = mutableBigInteger3;
			}
			else
			{
				v = mutableBigInteger3;
			}
			if (mutableBigInteger.intLen < 2 && v.intLen < 2)
			{
				int a = mutableBigInteger._value[mutableBigInteger.offset];
				int b = v._value[v.offset];
				a = binaryGcd(a, b);
				mutableBigInteger2._value[0] = a;
				mutableBigInteger2.intLen = 1;
				mutableBigInteger2.offset = 0;
				if (num > 0)
				{
					mutableBigInteger2.leftShift(num);
				}
				return mutableBigInteger2;
			}
			if ((num3 = mutableBigInteger.difference(v)) == 0)
			{
				break;
			}
			mutableBigInteger3 = ((num3 >= 0) ? mutableBigInteger : v);
		}
		if (num > 0)
		{
			mutableBigInteger.leftShift(num);
		}
		return mutableBigInteger;
	}

	private static int binaryGcd(int a, int b)
	{
		if (b == 0)
		{
			return a;
		}
		if (a == 0)
		{
			return b;
		}
		int num = BigInteger.NumberOfTrailingZeros(a);
		int num2 = BigInteger.NumberOfTrailingZeros(b);
		a = Operator.UnsignedRightShift(a, num);
		b = Operator.UnsignedRightShift(b, num2);
		int num3 = ((num < num2) ? num : num2);
		while (a != b)
		{
			if (a + 2147483648u > b + 2147483648u)
			{
				a -= b;
				a = Operator.UnsignedRightShift(a, BigInteger.NumberOfTrailingZeros(a));
			}
			else
			{
				b -= a;
				b = Operator.UnsignedRightShift(b, BigInteger.NumberOfTrailingZeros(b));
			}
		}
		return a << num3;
	}

	private MutableBigInteger mutableModInverse(MutableBigInteger p)
	{
		if (p.isOdd())
		{
			return modInverse(p);
		}
		if (isEven())
		{
			throw new ArithmeticException("BigInteger not invertible.");
		}
		int lowestSetBit = p.getLowestSetBit();
		MutableBigInteger mutableBigInteger = new MutableBigInteger(p);
		mutableBigInteger.rightShift(lowestSetBit);
		if (mutableBigInteger.isOne())
		{
			return modInverseMP2(lowestSetBit);
		}
		MutableBigInteger mutableBigInteger2 = modInverse(mutableBigInteger);
		MutableBigInteger mutableBigInteger3 = modInverseMP2(lowestSetBit);
		MutableBigInteger y = modInverseBP2(mutableBigInteger, lowestSetBit);
		MutableBigInteger y2 = mutableBigInteger.modInverseMP2(lowestSetBit);
		MutableBigInteger mutableBigInteger4 = new MutableBigInteger();
		MutableBigInteger mutableBigInteger5 = new MutableBigInteger();
		MutableBigInteger mutableBigInteger6 = new MutableBigInteger();
		mutableBigInteger2.leftShift(lowestSetBit);
		mutableBigInteger2.multiply(y, mutableBigInteger6);
		mutableBigInteger3.multiply(mutableBigInteger, mutableBigInteger4);
		mutableBigInteger4.multiply(y2, mutableBigInteger5);
		mutableBigInteger6.add(mutableBigInteger5);
		return mutableBigInteger6.divide(p, mutableBigInteger4);
	}

	private MutableBigInteger modInverseMP2(int k)
	{
		if (isEven())
		{
			throw new ArithmeticException("Non-invertible. (GCD != 1)");
		}
		if (k > 64)
		{
			return euclidModInverse(k);
		}
		int num = inverseMod32(_value[offset + intLen - 1]);
		if (k < 33)
		{
			num = ((k == 32) ? num : (num & ((1 << k) - 1)));
			return new MutableBigInteger(num);
		}
		long num2 = _value[offset + intLen - 1] & 0xFFFFFFFFu;
		if (intLen > 1)
		{
			num2 |= (long)_value[offset + intLen - 2] << 32;
		}
		long num3 = num & 0xFFFFFFFFu;
		num3 *= 2 - num2 * num3;
		num3 = ((k == 64) ? num3 : (num3 & ((1L << k) - 1)));
		MutableBigInteger mutableBigInteger = new MutableBigInteger(new int[2]);
		mutableBigInteger._value[0] = (int)Operator.UnsignedRightShift(num3, 32);
		mutableBigInteger._value[1] = (int)num3;
		mutableBigInteger.intLen = 2;
		mutableBigInteger.normalize();
		return mutableBigInteger;
	}

	private static int inverseMod32(int val)
	{
		int num = val;
		num *= 2 - val * num;
		num *= 2 - val * num;
		num *= 2 - val * num;
		return num * (2 - val * num);
	}

	private static MutableBigInteger modInverseBP2(MutableBigInteger mod, int k)
	{
		return fixup(new MutableBigInteger(1), new MutableBigInteger(mod), k);
	}

	private MutableBigInteger modInverse(MutableBigInteger mod)
	{
		throw new NotImplementedException("This method uses SignedMutableBigInteger class.");
	}

	private static MutableBigInteger fixup(MutableBigInteger c, MutableBigInteger p, int k)
	{
		MutableBigInteger mutableBigInteger = new MutableBigInteger();
		int num = -inverseMod32(p._value[p.offset + p.intLen - 1]);
		int i = 0;
		for (int num2 = k >> 5; i < num2; i++)
		{
			int y = num * c._value[c.offset + c.intLen - 1];
			p.mul(y, mutableBigInteger);
			c.add(mutableBigInteger);
			c.intLen--;
		}
		int num3 = k & 0x1F;
		if (num3 != 0)
		{
			int num4 = num * c._value[c.offset + c.intLen - 1];
			num4 &= (1 << num3) - 1;
			p.mul(num4, mutableBigInteger);
			c.add(mutableBigInteger);
			c.rightShift(num3);
		}
		while (c.compare(p) >= 0)
		{
			c.subtract(p);
		}
		return c;
	}

	private MutableBigInteger euclidModInverse(int k)
	{
		MutableBigInteger mutableBigInteger = new MutableBigInteger(1);
		mutableBigInteger.leftShift(k);
		MutableBigInteger mutableBigInteger2 = new MutableBigInteger(mutableBigInteger);
		MutableBigInteger mutableBigInteger3 = new MutableBigInteger(this);
		MutableBigInteger mutableBigInteger4 = new MutableBigInteger();
		MutableBigInteger mutableBigInteger5 = mutableBigInteger.divide(mutableBigInteger3, mutableBigInteger4);
		MutableBigInteger mutableBigInteger6 = mutableBigInteger;
		mutableBigInteger = mutableBigInteger5;
		MutableBigInteger mutableBigInteger7 = new MutableBigInteger(mutableBigInteger4);
		MutableBigInteger mutableBigInteger8 = new MutableBigInteger(1);
		MutableBigInteger mutableBigInteger9 = new MutableBigInteger();
		while (!mutableBigInteger.isOne())
		{
			MutableBigInteger mutableBigInteger10 = mutableBigInteger3.divide(mutableBigInteger, mutableBigInteger4);
			if (mutableBigInteger10.intLen == 0)
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}
			mutableBigInteger6 = mutableBigInteger10;
			mutableBigInteger3 = mutableBigInteger6;
			if (mutableBigInteger4.intLen == 1)
			{
				mutableBigInteger7.mul(mutableBigInteger4._value[mutableBigInteger4.offset], mutableBigInteger9);
			}
			else
			{
				mutableBigInteger4.multiply(mutableBigInteger7, mutableBigInteger9);
			}
			mutableBigInteger6 = mutableBigInteger4;
			mutableBigInteger4 = mutableBigInteger9;
			mutableBigInteger9 = mutableBigInteger6;
			mutableBigInteger8.add(mutableBigInteger4);
			if (mutableBigInteger3.isOne())
			{
				return mutableBigInteger8;
			}
			MutableBigInteger mutableBigInteger11 = mutableBigInteger.divide(mutableBigInteger3, mutableBigInteger4);
			if (mutableBigInteger11.intLen == 0)
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}
			mutableBigInteger6 = mutableBigInteger;
			mutableBigInteger = mutableBigInteger11;
			if (mutableBigInteger4.intLen == 1)
			{
				mutableBigInteger8.mul(mutableBigInteger4._value[mutableBigInteger4.offset], mutableBigInteger9);
			}
			else
			{
				mutableBigInteger4.multiply(mutableBigInteger8, mutableBigInteger9);
			}
			mutableBigInteger6 = mutableBigInteger4;
			mutableBigInteger4 = mutableBigInteger9;
			mutableBigInteger9 = mutableBigInteger6;
			mutableBigInteger7.add(mutableBigInteger4);
		}
		mutableBigInteger2.subtract(mutableBigInteger7);
		return mutableBigInteger2;
	}
}
