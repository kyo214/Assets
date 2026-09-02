using UnityEngine;

namespace LuxURPEssentials;

[ExecuteAlways]
public class LuxURP_Tonemapping : MonoBehaviour
{
	[Space(5f)]
	[LuxURP_HelpBtn("h.zdqgjigbf0e4")]
	[Space(3f)]
	[Space(8f)]
	public bool _enableTonemapping;

	[Space(8f)]
	public ToneMappingModes _mode;

	[Header("Custom Tonemapping")]
	[Space(8f)]
	public bool _enableNeutral;

	[Space(4f)]
	[Range(-1f, 1f)]
	public float _gamma;

	[Range(-1f, 1f)]
	public float _contrast;

	[Range(-1f, 1f)]
	public float _hue;

	[Range(-1f, 1f)]
	public float _saturation;

	public Color _filter = Color.white;

	private static readonly int _LuxURP_EnableTonemapping = Shader.PropertyToID("_LuxURP_EnableTonemapping");

	private static readonly int _LuxURP_ToneMappingMode = Shader.PropertyToID("_LuxURP_ToneMappingMode");

	private static readonly int _LuxURP_EnableNeutral = Shader.PropertyToID("_LuxURP_EnableNeutral");

	private static readonly int _LuxURP_Gamma = Shader.PropertyToID("_LuxURP_Gamma");

	private static readonly int _LuxURP_Contrast = Shader.PropertyToID("_LuxURP_Contrast");

	private static readonly int _LuxURP_Saturation = Shader.PropertyToID("_LuxURP_Saturation");

	private static readonly int _LuxURP_Hue = Shader.PropertyToID("_LuxURP_Hue");

	private static readonly int _LuxURP_Filter = Shader.PropertyToID("_LuxURP_Filter");

	private void OnEnable()
	{
		UpdateSettings();
	}

	private void OnDisable()
	{
		Shader.SetGlobalFloat(_LuxURP_EnableTonemapping, 0f);
	}

	private void OnValidate()
	{
		UpdateSettings();
	}

	private void UpdateSettings()
	{
		Shader.SetGlobalFloat(_LuxURP_EnableTonemapping, _enableTonemapping ? 1f : 0f);
		Shader.SetGlobalFloat(_LuxURP_ToneMappingMode, (float)_mode);
		Shader.SetGlobalFloat(_LuxURP_EnableNeutral, _enableNeutral ? 1f : 0f);
		Shader.SetGlobalFloat(_LuxURP_Gamma, 1f + _gamma);
		Shader.SetGlobalFloat(_LuxURP_Contrast, 1f + _contrast);
		Shader.SetGlobalFloat(_LuxURP_Saturation, _saturation);
		Shader.SetGlobalFloat(_LuxURP_Hue, _hue * 0.5f);
		Shader.SetGlobalColor(_LuxURP_Filter, _filter);
	}
}
