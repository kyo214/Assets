using NPOI.Util;

namespace NPOI.HPSF;

public class VariantBool
{
	public const int SIZE = 2;

	private bool _value;

	public bool Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public VariantBool(byte[] data, int offset)
	{
		switch (LittleEndian.GetShort(data, offset))
		{
		case 0:
			_value = false;
			break;
		case -1:
			_value = true;
			break;
		default:
			_value = true;
			break;
		}
	}
}
