namespace UnityEngine.Rendering.Universal;

internal enum DecalSurfaceData
{
	[Tooltip("Decals will affect only base color and emission.")]
	Albedo = 0,
	[Tooltip("Decals will affect only base color, normal and emission.")]
	AlbedoNormal = 1,
	[Tooltip("Decals will affect base color, normal, metallic, ambient occlusion, smoothness and emission.")]
	AlbedoNormalMAOS = 2
}
