namespace UnityEngine.Rendering.Universal;

public enum VolumeFrameworkUpdateMode
{
	[InspectorName("Every Frame")]
	EveryFrame = 0,
	[InspectorName("Via Scripting")]
	ViaScripting = 1,
	[InspectorName("Use Pipeline Settings")]
	UsePipelineSettings = 2
}
