namespace UnityEngine.VFX;

internal enum VFXInstancingMode
{
	Disabled = -1,
	[InspectorName("Automatic batch capacity")]
	Auto = 0,
	[InspectorName("Custom batch capacity")]
	Custom = 1
}
