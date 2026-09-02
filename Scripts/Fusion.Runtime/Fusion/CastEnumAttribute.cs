using System;
using UnityEngine;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field)]
public class CastEnumAttribute : UnityEngine.PropertyAttribute
{
	public string GetTypeMethodName;

	public Type CastToType;

	public CastEnumAttribute(string getTypeMethodName)
	{
		GetTypeMethodName = getTypeMethodName;
	}

	public CastEnumAttribute(Type castToType)
	{
		CastToType = castToType;
	}
}
