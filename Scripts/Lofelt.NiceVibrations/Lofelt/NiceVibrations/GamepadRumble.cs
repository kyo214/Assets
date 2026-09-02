using System;
using UnityEngine;

namespace Lofelt.NiceVibrations;

[Serializable]
public struct GamepadRumble
{
	[SerializeField]
	public int[] durationsMs;

	[SerializeField]
	public int totalDurationMs;

	[SerializeField]
	public float[] lowFrequencyMotorSpeeds;

	[SerializeField]
	public float[] highFrequencyMotorSpeeds;

	public bool IsValid()
	{
		if (durationsMs != null && lowFrequencyMotorSpeeds != null && highFrequencyMotorSpeeds != null && durationsMs.Length == lowFrequencyMotorSpeeds.Length && durationsMs.Length == highFrequencyMotorSpeeds.Length)
		{
			return durationsMs.Length != 0;
		}
		return false;
	}
}
