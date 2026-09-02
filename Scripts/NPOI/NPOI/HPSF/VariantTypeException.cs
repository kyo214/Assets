using System;

namespace NPOI.HPSF;

[Serializable]
public abstract class VariantTypeException : HPSFException
{
	private object value;

	private long variantType;

	public long VariantType => variantType;

	public object Value => value;

	public VariantTypeException(long variantType, object value, string msg)
		: base(msg)
	{
		this.variantType = variantType;
		this.value = value;
	}
}
