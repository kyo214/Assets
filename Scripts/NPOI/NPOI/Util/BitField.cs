using System;

namespace NPOI.Util;

[Serializable]
public class BitField
{
	private int _mask;

	private int _shift_count;

	public BitField(int mask)
	{
		_mask = mask;
		int num = 0;
		int num2 = mask;
		if (num2 != 0)
		{
			while ((num2 & 1) == 0)
			{
				num++;
				num2 >>= 1;
			}
		}
		_shift_count = num;
	}

	public BitField(uint mask)
		: this((int)mask)
	{
	}

	public int Clear(int holder)
	{
		return holder & ~_mask;
	}

	public short ClearShort(short holder)
	{
		return (short)Clear(holder);
	}

	public int GetRawValue(int holder)
	{
		return holder & _mask;
	}

	public short GetShortRawValue(short holder)
	{
		return (short)GetRawValue(holder);
	}

	public short GetShortValue(short holder)
	{
		return (short)GetValue(holder);
	}

	public int GetValue(int holder)
	{
		return Operator.UnsignedRightShift(GetRawValue(holder), _shift_count);
	}

	public bool IsAllSet(int holder)
	{
		return (holder & _mask) == _mask;
	}

	public bool IsSet(int holder)
	{
		return (holder & _mask) != 0;
	}

	public int Set(int holder)
	{
		return holder | _mask;
	}

	public int SetBoolean(int holder, bool flag)
	{
		if (flag)
		{
			return Set(holder);
		}
		return Clear(holder);
	}

	public short SetShort(short holder)
	{
		return (short)Set(holder);
	}

	public short SetShortBoolean(short holder, bool flag)
	{
		if (flag)
		{
			return SetShort(holder);
		}
		return ClearShort(holder);
	}

	public short SetShortValue(short holder, short value)
	{
		return (short)SetValue(holder, value);
	}

	public int SetValue(int holder, int value)
	{
		return (holder & ~_mask) | ((value << _shift_count) & _mask);
	}

	public byte SetByteBoolean(byte holder, bool flag)
	{
		if (flag)
		{
			return SetByte(holder);
		}
		return ClearByte(holder);
	}

	public byte ClearByte(byte holder)
	{
		return (byte)Clear(holder);
	}

	public byte SetByte(byte holder)
	{
		return (byte)Set(holder);
	}
}
