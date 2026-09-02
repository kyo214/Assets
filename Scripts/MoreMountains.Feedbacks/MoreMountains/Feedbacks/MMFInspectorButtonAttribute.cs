using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Field)]
public class MMFInspectorButtonAttribute : PropertyAttribute
{
	public readonly string MethodName;

	public MMFInspectorButtonAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}
}
