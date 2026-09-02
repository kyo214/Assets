using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field)]
public sealed class UnityResourcePathAttribute : PropertyAttribute
{
	public Type ResourceType { get; }

	public UnityResourcePathAttribute(Type resourceType)
	{
		ResourceType = resourceType;
	}
}
