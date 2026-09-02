namespace UnityEngine.Rendering.Universal;

public enum UpscalingFilterSelection
{
	[InspectorName("Automatic")]
	[Tooltip("Unity selects a filtering option automatically based on the Render Scale value and the current screen resolution.")]
	Auto = 0,
	[InspectorName("Bilinear")]
	Linear = 1,
	[InspectorName("Nearest-Neighbor")]
	Point = 2,
	[InspectorName("FidelityFX Super Resolution 1.0")]
	[Tooltip("If the target device does not support Unity shader model 4.5, Unity falls back to the Automatic option.")]
	FSR = 3
}
