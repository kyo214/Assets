using System;

namespace UnityEngine.Rendering.Universal;

[Serializable]
internal class ScreenSpaceAmbientOcclusionSettings
{
	internal enum DepthSource
	{
		Depth = 0,
		DepthNormals = 1
	}

	internal enum NormalQuality
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	internal enum AOSampleOption
	{
		High = 0,
		Medium = 1,
		Low = 2
	}

	internal enum AOMethodOptions
	{
		BlueNoise = 0,
		InterleavedGradient = 1
	}

	internal enum BlurQualityOptions
	{
		High = 0,
		Medium = 1,
		Low = 2
	}

	[SerializeField]
	internal AOMethodOptions AOMethod;

	[SerializeField]
	internal bool Downsample;

	[SerializeField]
	internal bool AfterOpaque;

	[SerializeField]
	internal DepthSource Source = DepthSource.DepthNormals;

	[SerializeField]
	internal NormalQuality NormalSamples = NormalQuality.Medium;

	[SerializeField]
	internal float Intensity = 3f;

	[SerializeField]
	internal float DirectLightingStrength = 0.25f;

	[SerializeField]
	internal float Radius = 0.035f;

	[SerializeField]
	internal AOSampleOption Samples = AOSampleOption.Medium;

	[SerializeField]
	internal BlurQualityOptions BlurQuality;

	[SerializeField]
	internal float Falloff = 100f;

	[SerializeField]
	internal int SampleCount = -1;
}
