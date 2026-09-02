using System;

namespace Doozy.Runtime.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DefineSymbolsAttribute : Attribute
{
	public string name { get; }

	public DefineSymbolsAttribute()
		: this("")
	{
	}

	public DefineSymbolsAttribute(string name)
	{
		this.name = name;
	}
}
