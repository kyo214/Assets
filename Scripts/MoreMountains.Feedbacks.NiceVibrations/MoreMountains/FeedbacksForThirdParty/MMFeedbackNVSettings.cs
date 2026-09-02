using System;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[Serializable]
public class MMFeedbackNVSettings
{
	[Tooltip("whether or not to force this haptic to play on a specific gamepad")]
	public bool ForceGamepadID;

	[Tooltip("The ID of the gamepad on which to play this haptic")]
	public int GamepadID;

	[Tooltip("whether or not this haptic should play only if haptics are supported")]
	public bool OnlyPlayIfHapticsSupported = true;

	[Tooltip("whether or not this haptic should play only if advanced haptics requirements are met on the device")]
	public bool OnlyPlayIfAdvancedRequirementsMet;

	[Tooltip("whether or not this haptic should play only if the device supports amplitude modulation")]
	public bool OnlyPlayIfAmplitudeModulationSupported;

	[Tooltip("whether or not this haptic should play only if the device supports frequency modulation")]
	public bool OnlyPlayIfFrequencyModulationSupported;

	public virtual void SetGamepad()
	{
		if (ForceGamepadID)
		{
			GamepadRumbler.SetCurrentGamepad(GamepadID);
		}
	}

	public virtual bool CanPlay()
	{
		if (OnlyPlayIfAdvancedRequirementsMet && !DeviceCapabilities.meetsAdvancedRequirements)
		{
			return false;
		}
		if (OnlyPlayIfAmplitudeModulationSupported && !DeviceCapabilities.hasAmplitudeModulation)
		{
			return false;
		}
		if (OnlyPlayIfFrequencyModulationSupported && !DeviceCapabilities.hasFrequencyModulation)
		{
			return false;
		}
		return true;
	}
}
