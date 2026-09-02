using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

namespace Toked.StatusEffect;

public abstract class StatusEffectScriptableObject : ScriptableObject
{
	[SerializeField]
	protected StatusEffectData _statusEffectData;

	[SerializeField]
	protected bool _cantClearEffectAfterFinishedMission;

	[SerializeField]
	protected bool _infiniteDuration;

	[SerializeField]
	private bool _customUpdateTime;

	[SerializeField]
	private float _customUpdateTimeSeconds = 1f;

	[SerializeField]
	private List<StatusEffectScriptableObject> _antiStatusEffectsList = new List<StatusEffectScriptableObject>();

	public StatusEffectData StatusEffectData => _statusEffectData;

	public float Duration
	{
		get
		{
			if (!_infiniteDuration)
			{
				return _statusEffectData.Duration;
			}
			return -1f;
		}
		set
		{
			_statusEffectData.Duration = value;
		}
	}

	public bool DestroyOnRemove { get; set; }

	public virtual bool CantClearEffectAfterFinishedMission => _cantClearEffectAfterFinishedMission;

	public bool InfiniteDuration => _infiniteDuration;

	public bool CustomUpdateTime => _customUpdateTime;

	public float CustomUpdateTimeSeconds => _customUpdateTimeSeconds;

	public abstract void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect);

	public abstract IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect);

	public abstract void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect);

	public virtual void AdditionalUpdateFunction(float elapsedTime)
	{
	}

	public virtual float GetTotalEffectDuration(StatusEffectController statusEffectController)
	{
		if (!_infiniteDuration)
		{
			return Duration;
		}
		return -1f;
	}

	public string GetStatusEffectLocalizationName(bool isUsingBrackets = false)
	{
		string text = LocalizationManager.GetTranslation(_statusEffectData.LocalizationName);
		string text2 = ColorUtility.ToHtmlStringRGB(_statusEffectData.NameColor);
		if (string.IsNullOrWhiteSpace(text))
		{
			text = string.Empty;
		}
		if (isUsingBrackets)
		{
			return "<color=#" + text2 + ">(" + text + ")</color>";
		}
		return "<color=#" + text2 + ">" + text + "</color>";
	}

	public virtual void AdditionalCloneSoData(StatusEffectScriptableObject sourceStatusEffect)
	{
	}

	public virtual void CheckHaveAntiStatusEffect(StatusEffectController.StatusEffect statusEffect)
	{
		statusEffect.HasAntiStatusEffect = false;
		foreach (StatusEffectScriptableObject antiStatusEffects in _antiStatusEffectsList)
		{
			if (statusEffect.statusEffectController.CheckContainEffectStatus(antiStatusEffects.StatusEffectData.BaseName))
			{
				statusEffect.HasAntiStatusEffect = true;
				break;
			}
		}
	}
}
