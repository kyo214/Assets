using UnityEngine;

namespace DarkTonic.MasterAudio;

public static class SpatializerHelper
{
	private const string OculusSpatializer = "OculusSpatializer";

	private const string ResonanceAudioSpatializer = "Resonance Audio";

	public static bool IsSupportedSpatializer
	{
		get
		{
			string selectedSpatializer = SelectedSpatializer;
			if (!(selectedSpatializer == "OculusSpatializer"))
			{
				if (selectedSpatializer == "Resonance Audio")
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsOculusAudioSpatializer => SelectedSpatializer == "OculusSpatializer";

	public static bool IsResonanceAudioSpatializer => SelectedSpatializer == "Resonance Audio";

	public static string SelectedSpatializer => AudioSettings.GetSpatializerPluginName();

	public static void TurnOnSpatializerIfEnabled(AudioSource source)
	{
		if (MasterAudio.SafeInstance == null)
		{
			SetSpatializerToggleOnSource(source, enabled: false);
			return;
		}
		if (!MasterAudio.Instance.useSpatializer)
		{
			SetSpatializerToggleOnSource(source, enabled: false);
			return;
		}
		SetSpatializerToggleOnSource(source, enabled: true);
		if (ResonanceAudioHelper.ResonanceAudioOptionExists && MasterAudio.Instance.useSpatializerPostFX)
		{
			source.spatializePostEffects = true;
		}
	}

	private static void SetSpatializerToggleOnSource(AudioSource source, bool enabled)
	{
		if (enabled)
		{
			enabled = source.spatialBlend != 0f;
		}
		source.spatialize = enabled;
	}
}
