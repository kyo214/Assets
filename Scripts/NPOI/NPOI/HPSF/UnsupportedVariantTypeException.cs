using System;
using NPOI.Util;

namespace NPOI.HPSF;

[Serializable]
public abstract class UnsupportedVariantTypeException : VariantTypeException
{
	public UnsupportedVariantTypeException(long variantType, object value)
		: base(variantType, value, "HPSF does not yet support the variant type " + variantType + " (" + Variant.GetVariantName(variantType) + ", " + HexDump.ToHex(variantType) + "). If you want support for this variant type in one of the next POI releases please submit a request for enhancement (RFE) To <http://issues.apache.org/bugzilla/>! Thank you!")
	{
	}
}
