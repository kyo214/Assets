using System;

namespace BansheeGz.BGDatabase;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BGCalcUnitDefinitionAttribute : Attribute
{
	public string name { get; }

	public bool hidden { get; }

	public BGCalcUnitDefinitionAttribute(string name)
		: this(name, hidden: false)
	{
	}

	public BGCalcUnitDefinitionAttribute(string name, bool hidden)
	{
		this.name = name;
		this.hidden = hidden;
	}
}
