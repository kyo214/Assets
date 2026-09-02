using System;

namespace Doozy.Runtime.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RefreshDataAttribute : Attribute
{
	public string name { get; }

	public RefreshDataAttribute()
		: this("")
	{
	}

	public RefreshDataAttribute(string name)
	{
		this.name = name;
	}
}
