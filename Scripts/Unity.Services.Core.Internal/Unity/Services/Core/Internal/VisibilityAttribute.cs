using System;
using UnityEngine;

namespace Unity.Services.Core.Internal;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class VisibilityAttribute : PropertyAttribute
{
	public string PropertyName { get; private set; }

	public object Value { get; private set; }

	public VisibilityAttribute(string propertyName, object value)
	{
		PropertyName = propertyName;
		Value = value;
	}
}
