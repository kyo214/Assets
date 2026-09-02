using System;
using System.Diagnostics;

namespace Fusion;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
[Conditional("FUSION_UNITY")]
internal sealed class UnityDummyAttribute : Attribute
{
	public UnityDummyAttribute()
	{
	}

	public UnityDummyAttribute(string str)
	{
	}
}
