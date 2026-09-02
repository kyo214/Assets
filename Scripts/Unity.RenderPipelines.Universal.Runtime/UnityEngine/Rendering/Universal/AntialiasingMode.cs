namespace UnityEngine.Rendering.Universal;

public enum AntialiasingMode
{
	[InspectorName("No Anti-aliasing")]
	None = 0,
	[InspectorName("Fast Approximate Anti-aliasing (FXAA)")]
	FastApproximateAntialiasing = 1,
	[InspectorName("Subpixel Morphological Anti-aliasing (SMAA)")]
	SubpixelMorphologicalAntiAliasing = 2,
	[InspectorName("Temporal Anti-aliasing (TAA)")]
	TemporalAntiAliasing = 3
}
