using System;
using UnityEngine.Scripting;

namespace Fusion;

[AttributeUsage(AttributeTargets.Class)]
[Preserve]
public abstract class OrderAttribute : Attribute
{
}
