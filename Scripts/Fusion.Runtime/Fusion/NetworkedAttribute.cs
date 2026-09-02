using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class NetworkedAttribute : Attribute
{
	public string OnChanged { get; set; }

	public OnChangedTargets OnChangedTargets { get; set; }

	public string Default { get; set; }

	public string Group { get; set; }

	public NetworkedAttribute()
	{
		OnChangedTargets = OnChangedTargets.All;
	}

	public NetworkedAttribute(string group)
		: this()
	{
		Group = group;
	}
}
