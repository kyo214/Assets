using System;
using I2.Loc;
using UnityEngine;

namespace _Modules.Localization.Scripts;

[Serializable]
public class StatsValueLocalization
{
	[SerializeField]
	private string _statsTag;

	[SerializeField]
	private UnityEngine.Object _statsValueLocalization;

	public string StatsTag => _statsTag;

	public UnityEngine.Object StatValueLocalization => _statsValueLocalization;

	public StatsValueLocalization(UnityEngine.Object statsValueLocalization)
	{
		_statsValueLocalization = statsValueLocalization;
	}

	public void SetStatsValueLocalization(LocalizationParamsManager localizationManager)
	{
		if (_statsValueLocalization is IStatsValueLocalization statsValueLocalization)
		{
			localizationManager.SetParameterValue(_statsTag, statsValueLocalization?.GetStatsValueLocalization());
		}
	}

	private bool IsIStatsValueLocalization(UnityEngine.Object so)
	{
		return so is IStatsValueLocalization;
	}
}
