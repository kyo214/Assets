using System;
using JetBrains.Annotations;

namespace Sirenix.Serialization;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OdinSerializeAttribute : Attribute
{
}
