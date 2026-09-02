using System;

namespace NPOI.HPSF;

[Serializable]
public class WritingNotSupportedException : UnsupportedVariantTypeException
{
	public WritingNotSupportedException(long variantType, object value)
		: base(variantType, value)
	{
	}
}
