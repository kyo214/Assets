using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMCameraShakerRotation : MMCameraShaker
{
	public override void ShakeCamera(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool useUnscaledTime)
	{
		if (amplitudeX != 0f || amplitudeY != 0f || amplitudeZ != 0f)
		{
			_wiggle.RotationWiggleProperties.AmplitudeMin.x = 0f - amplitudeX;
			_wiggle.RotationWiggleProperties.AmplitudeMin.y = 0f - amplitudeY;
			_wiggle.RotationWiggleProperties.AmplitudeMin.z = 0f - amplitudeZ;
			_wiggle.RotationWiggleProperties.AmplitudeMax.x = amplitudeX;
			_wiggle.RotationWiggleProperties.AmplitudeMax.y = amplitudeY;
			_wiggle.RotationWiggleProperties.AmplitudeMax.z = amplitudeZ;
		}
		else
		{
			_wiggle.RotationWiggleProperties.AmplitudeMin = Vector3.one * (0f - amplitude);
			_wiggle.RotationWiggleProperties.AmplitudeMax = Vector3.one * amplitude;
		}
		_wiggle.RotationWiggleProperties.UseUnscaledTime = useUnscaledTime;
		_wiggle.RotationWiggleProperties.FrequencyMin = frequency;
		_wiggle.RotationWiggleProperties.FrequencyMax = frequency;
		_wiggle.RotationWiggleProperties.NoiseFrequencyMin = frequency * Vector3.one;
		_wiggle.RotationWiggleProperties.NoiseFrequencyMax = frequency * Vector3.one;
		_wiggle.WiggleRotation(duration);
	}
}
