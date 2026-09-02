using System;
using NPOI.Util;

namespace NPOI.HPSF;

[Serializable]
public class IllegalVariantTypeException : VariantTypeException
{
	public IllegalVariantTypeException(long variantType, object value, string msg)
		: base(variantType, value, msg)
	{
	}

	public IllegalVariantTypeException(long variantType, object value)
		: this(variantType, value, "The variant type " + variantType + " (" + Variant.GetVariantName(variantType) + ", " + HexDump.ToHex(variantType) + ") is illegal in this context.")
	{
	}
}
