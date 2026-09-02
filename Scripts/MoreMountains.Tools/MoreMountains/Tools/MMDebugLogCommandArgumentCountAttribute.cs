using System;

namespace MoreMountains.Tools;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class MMDebugLogCommandArgumentCountAttribute : Attribute
{
	public readonly int ArgumentCount;

	public MMDebugLogCommandArgumentCountAttribute(int argumentCount)
	{
		ArgumentCount = argumentCount;
	}
}
