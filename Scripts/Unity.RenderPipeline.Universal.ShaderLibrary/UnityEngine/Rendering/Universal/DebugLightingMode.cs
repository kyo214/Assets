namespace UnityEngine.Rendering.Universal;

[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@14.0.12\\ShaderLibrary\\Debug\\DebugViewEnums.cs")]
public enum DebugLightingMode
{
	None = 0,
	ShadowCascades = 1,
	LightingWithoutNormalMaps = 2,
	LightingWithNormalMaps = 3,
	Reflections = 4,
	ReflectionsWithSmoothness = 5
}
