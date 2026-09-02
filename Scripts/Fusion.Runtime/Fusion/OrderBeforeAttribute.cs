using System;
using UnityEngine.Scripting;

namespace Fusion;

[Preserve]
public class OrderBeforeAttribute : OrderAttribute
{
	public Type[] Before { get; }

	public OrderBeforeAttribute(params Type[] types)
	{
		Before = types;
	}
}
