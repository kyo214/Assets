using NPOI.Util;

namespace NPOI.HPSF;

internal class Blob
{
	private byte[] _value;

	public int Size => 4 + _value.Length;

	public Blob(byte[] data, int offset)
	{
		int num = LittleEndian.GetInt(data, offset);
		if (num == 0)
		{
			_value = new byte[0];
		}
		else
		{
			_value = LittleEndian.GetByteArray(data, offset + 4, num);
		}
	}
}
