using System;

namespace MoreMountains.Feedbacks;

[Serializable]
public struct MMCameraShakeProperties(float duration, float amplitude, float frequency, float amplitudeX = 0f, float amplitudeY = 0f, float amplitudeZ = 0f)
{
	public float Duration = duration;

	public float Amplitude = amplitude;

	public float Frequency = frequency;

	public float AmplitudeX = amplitudeX;

	public float AmplitudeY = amplitudeY;

	public float AmplitudeZ = amplitudeZ;
}
