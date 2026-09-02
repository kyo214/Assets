using System.Collections;

namespace NPOI.Util;

public class BitFieldFactory
{
	private static Hashtable instances = new Hashtable();

	public static BitField GetInstance(int mask)
	{
		BitField bitField = (BitField)instances[mask];
		if (bitField == null)
		{
			bitField = new BitField(mask);
			instances[mask] = bitField;
		}
		return bitField;
	}
}
