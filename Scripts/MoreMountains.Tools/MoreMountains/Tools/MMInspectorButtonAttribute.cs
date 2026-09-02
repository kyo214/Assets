using System;
using UnityEngine;

namespace MoreMountains.Tools;

[AttributeUsage(AttributeTargets.Field)]
public class MMInspectorButtonAttribute : PropertyAttribute
{
	public readonly string MethodName;

	public MMInspectorButtonAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}
}
