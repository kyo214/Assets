using System;
using UnityEngine.Scripting;

namespace Fusion;

[Preserve]
public class OrderAfterAttribute : OrderAttribute
{
	public Type[] After { get; }

	public OrderAfterAttribute(params Type[] types)
	{
		After = types;
	}
}
