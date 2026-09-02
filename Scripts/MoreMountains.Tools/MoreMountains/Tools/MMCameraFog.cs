using UnityEngine;

namespace MoreMountains.Tools;

[ExecuteAlways]
public class MMCameraFog : MonoBehaviour
{
	public FogSettings Settings;

	protected FogSettings _previousSettings;

	protected void Awake()
	{
		_previousSettings = new FogSettings();
	}

	protected virtual void OnPreRender()
	{
		_previousSettings.FogEnabled = RenderSettings.fog;
		_previousSettings.FogColor = RenderSettings.fogColor;
		_previousSettings.FogDensity = RenderSettings.fogDensity;
		_previousSettings.FogMode = RenderSettings.fogMode;
		RenderSettings.fog = Settings.FogEnabled;
		RenderSettings.fogColor = Settings.FogColor;
		RenderSettings.fogDensity = Settings.FogDensity;
		RenderSettings.fogMode = Settings.FogMode;
	}

	protected virtual void OnPostRender()
	{
		RenderSettings.fog = _previousSettings.FogEnabled;
		RenderSettings.fogColor = _previousSettings.FogColor;
		RenderSettings.fogDensity = _previousSettings.FogDensity;
		RenderSettings.fogMode = _previousSettings.FogMode;
	}
}
