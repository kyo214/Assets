using System;

namespace Doozy.Runtime.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RestoreDataAttribute : Attribute
{
	public string name { get; }

	public RestoreDataAttribute()
		: this("")
	{
	}

	public RestoreDataAttribute(string name)
	{
		this.name = name;
	}
}
