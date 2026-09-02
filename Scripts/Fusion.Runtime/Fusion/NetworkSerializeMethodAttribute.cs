using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class NetworkSerializeMethodAttribute : Attribute
{
	public int MaxSize { get; set; }
}
