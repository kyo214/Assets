using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field)]
public class EnumMaskAttribute : PropertyAttribute
{
	public bool definesZero;

	public Type castTo;

	public EnumMaskAttribute(bool definesZero = false, Type castTo = null)
	{
		this.castTo = castTo;
		this.definesZero = definesZero;
	}
}
