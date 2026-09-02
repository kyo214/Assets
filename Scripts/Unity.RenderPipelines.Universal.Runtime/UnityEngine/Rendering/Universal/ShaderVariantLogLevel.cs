using System;

namespace UnityEngine.Rendering.Universal;

[Obsolete("This is obsolete, UnityEngine.Rendering.ShaderVariantLogLevel instead.", false)]
public enum ShaderVariantLogLevel
{
	Disabled = 0,
	[InspectorName("Only URP Shaders")]
	OnlyUniversalRPShaders = 1,
	[InspectorName("All Shaders")]
	AllShaders = 2
}
