using NPOI.Util;

namespace NPOI.HPSF;

public class UnicodeString
{
	private byte[] _value;

	public int Size => 4 + _value.Length;

	public byte[] Value => _value;

	public UnicodeString(byte[] data, int offset)
	{
		int num = LittleEndian.GetInt(data, offset);
		int offset2 = offset + 4;
		if (!validLength(num, data, offset2))
		{
			bool flag = false;
			int num2 = offset % 4;
			if (num2 != 0)
			{
				offset += num2;
				num = LittleEndian.GetInt(data, offset);
				offset2 = offset + 4;
				flag = validLength(num, data, offset2);
			}
			if (!flag)
			{
				throw new IllegalPropertySetDataException("UnicodeString started at offset #" + offset + " is not NULL-terminated");
			}
		}
		if (num == 0)
		{
			_value = new byte[0];
		}
		else
		{
			_value = LittleEndian.GetByteArray(data, offset2, num * 2);
		}
	}

	private bool validLength(int length, byte[] data, int offset)
	{
		if (length == 0)
		{
			return true;
		}
		int num = offset + length * 2;
		if (num <= data.Length && data[num - 1] == 0 && data[num - 2] == 0)
		{
			return true;
		}
		return false;
	}

	public string ToJavaString()
	{
		if (_value.Length == 0)
		{
			return null;
		}
		string fromUnicodeLE = StringUtil.GetFromUnicodeLE(_value, 0, _value.Length >> 1);
		int num = fromUnicodeLE.IndexOf('\0');
		if (num == -1)
		{
			return fromUnicodeLE;
		}
		_ = fromUnicodeLE.Length - 1;
		return fromUnicodeLE.Substring(0, num);
	}
}
