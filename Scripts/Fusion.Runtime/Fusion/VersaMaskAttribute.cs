using System;

namespace Fusion;

public class VersaMaskAttribute : PropertyAttribute
{
	public bool DefinesZero;

	public Type CastTo;

	public bool AlwaysExpanded;

	public bool ShowBitmask;

	public VersaMaskAttribute(bool definesZero = false, Type castTo = null)
	{
		DefinesZero = definesZero;
		CastTo = castTo;
	}
}
