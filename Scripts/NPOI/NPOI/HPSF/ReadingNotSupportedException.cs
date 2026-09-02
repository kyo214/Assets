using System;

namespace NPOI.HPSF;

[Serializable]
public class ReadingNotSupportedException : UnsupportedVariantTypeException
{
	public ReadingNotSupportedException(long variantType, object value)
		: base(variantType, value)
	{
	}
}
