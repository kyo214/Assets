using System;

namespace DG.DemiLib.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DeScriptExecutionOrderAttribute : Attribute
{
	internal int order;

	public DeScriptExecutionOrderAttribute(int order)
	{
		this.order = order;
	}
}
