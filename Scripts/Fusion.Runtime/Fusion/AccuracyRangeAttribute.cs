using UnityEngine;

namespace Fusion;

internal class AccuracyRangeAttribute : UnityEngine.PropertyAttribute
{
	public float min;

	public float max;

	public int places;

	public bool logarithmic;

	public AccuracyRangeAttribute(AccuracyRangePreset preset = AccuracyRangePreset.Defaults)
	{
		places = 1;
		logarithmic = true;
		switch (preset)
		{
		case AccuracyRangePreset.Position:
			max = 1f;
			min = 0.0001f;
			break;
		case AccuracyRangePreset.Rotation:
			max = 1f;
			min = 0.0001f;
			break;
		case AccuracyRangePreset.Velocity:
			max = 1f;
			min = 0.0001f;
			break;
		case AccuracyRangePreset.AngularVelocity:
			max = 1f;
			min = 0.0001f;
			break;
		default:
			max = 1f;
			min = 0.0001f;
			break;
		}
	}

	public AccuracyRangeAttribute(float min, float max, bool logarithmic = true)
	{
		if (max > min)
		{
			this.min = min;
			this.max = max;
		}
		else
		{
			this.min = max;
			this.max = min;
		}
		places = 0;
		this.logarithmic = logarithmic;
	}

	public AccuracyRangeAttribute(float min, float max, int places = 1, bool logarithmic = true)
	{
		if (max > min)
		{
			this.min = min;
			this.max = max;
		}
		else
		{
			this.min = max;
			this.max = min;
		}
		this.places = places;
		this.logarithmic = logarithmic;
	}
}
