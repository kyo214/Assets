using System;

namespace UnityEngine.Rendering;

public enum BuiltinShaderType
{
	DeferredShading = 0,
	DeferredReflections = 1,
	[Obsolete("LegacyDeferredLighting has been removed.", false)]
	LegacyDeferredLighting = 2,
	ScreenSpaceShadows = 3,
	DepthNormals = 4,
	MotionVectors = 5,
	LightHalo = 6,
	LensFlare = 7
}
