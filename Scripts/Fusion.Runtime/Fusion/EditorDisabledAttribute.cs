using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field)]
public sealed class EditorDisabledAttribute : PropertyAttribute
{
	internal bool HideInRelease;

	public EditorDisabledAttribute(bool hideInRelease = false)
	{
		HideInRelease = hideInRelease;
	}
}
