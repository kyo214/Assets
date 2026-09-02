using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class TmpAttribute : PropertyAttribute
{
	public readonly string header;

	public TmpAttribute(string header)
	{
		this.header = header;
	}
}
