using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field)]
public sealed class EditorDisabledGroupAttribute : PropertyAttribute
{
	public bool Begin { get; }

	public EditorDisabledGroupAttribute(bool begin)
	{
		Begin = begin;
	}
}
