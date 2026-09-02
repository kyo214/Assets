using System;
using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "IntoxicatedItemStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/IntoxicatedItemStatusEffectScriptableObject", order = 0)]
public class IntoxicatedItemStatusEffectScriptableObject : CurseItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private string _intoxicatedCategoryId = "Intoxicated";

	[SerializeField]
	private float _delayStatusEffect = 3f;

	[SerializeField]
	private ControlScrambler.ScrambleType _scrambleInputType = ControlScrambler.ScrambleType.Chaos;

	[SerializeField]
	private bool _onceOnly;

	[SerializeField]
	private float _intoxicatedFadeInDuration = 1f;

	[SerializeField]
	private float _intoxicatedFadeOutDuration = 1f;

	private CameraGame _cameraGame;

	private IScramble _inputScramble;

	private bool _initEffect;

	public IScramble InputScramble
	{
		get
		{
			return _inputScramble;
		}
		set
		{
			_inputScramble = value;
		}
	}

	public override void Init(int itemId, int uniqueItemId)
	{
		base.Init(itemId, uniqueItemId);
		if (_onceOnly && _inputScramble == null)
		{
			_inputScramble = ControlScrambler.GenerateScramble(_scrambleInputType);
		}
	}

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffect.HasAntiStatusEffect && _cameraGame == null)
		{
			SetIntoxicatedEffect(statusEffectController);
		}
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		_initEffect = false;
		yield return base.OnApplyEffect(statusEffectController, statusEffect);
		statusEffect.SetEffectApplied(isEffectApplied: true);
		_cameraGame = CameraGame.Instance;
		yield return new WaitForSeconds(_delayStatusEffect);
		if (!statusEffect.HasAntiStatusEffect)
		{
			statusEffectController.PlayerController.network.ShowBaloonChat(ChatType.MONOLOGUE, 22, -1, -1, -1, 10);
		}
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffect.HasAntiStatusEffect && !statusEffectController.CheckContainEffectStatus(_intoxicatedCategoryId, _statusEffectData.Name) && !statusEffectController.CheckContainEffectStatus(_intoxicatedCategoryId, _statusEffectData.Name))
		{
			SetIntoxicatedEffect(statusEffectController, () =>
			{
				SetScramblePlayerInput(statusEffectController.PlayerController);
			});
		}
		_initEffect = true;
	}

	private void OnApplyEffectStatus(StatusEffectController.StatusEffect statusEffect)
	{
		if (!statusEffect.HasAntiStatusEffect)
		{
			statusEffect.statusEffectController.PlayerController.network.ShowBaloonChat(ChatType.MONOLOGUE, 22, -1, -1, -1, 10);
		}
		SetIntoxicatedEffect(statusEffect.statusEffectController, () =>
		{
			SetScramblePlayerInput(statusEffect.statusEffectController.PlayerController);
		});
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		base.RemoveEffect(statusEffectController, statusEffect);
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffectController.CheckContainEffectStatus(_intoxicatedCategoryId))
		{
			RemoveIntoxicatedEffect(statusEffectController);
			statusEffectController.PlayerController.SetScrambleModifierInput(null);
		}
	}

	private void OnRemoveEffect(StatusEffectController.StatusEffect statusEffect)
	{
		RemoveIntoxicatedEffect(statusEffect.statusEffectController);
		statusEffect.statusEffectController.PlayerController.SetScrambleModifierInput(null);
	}

	private void SetIntoxicatedEffect(StatusEffectController statusEffectController, Action onCompleteAction = null)
	{
		if (statusEffectController.PlayerController.network.isLocalPlayer)
		{
			if (_cameraGame == null)
			{
				_cameraGame = CameraGame.Instance;
			}
			if (_cameraGame != null)
			{
				_cameraGame.SetStonedFilter(_intoxicatedFadeInDuration, onCompleteAction);
			}
		}
	}

	private void RemoveIntoxicatedEffect(StatusEffectController statusEffectController)
	{
		if (statusEffectController.PlayerController.network.isLocalPlayer)
		{
			if (_cameraGame == null)
			{
				_cameraGame = CameraGame.Instance;
			}
			_cameraGame?.RemoveStonedFilter(_intoxicatedFadeOutDuration);
		}
	}

	private void SetScramblePlayerInput(PlayerController playerController)
	{
		if (_onceOnly)
		{
			if (_inputScramble == null)
			{
				_inputScramble = ControlScrambler.GenerateScramble(_scrambleInputType);
			}
			playerController.SetScrambleModifierInput(_inputScramble);
		}
		else
		{
			playerController.SetScrambleModifierInput(_scrambleInputType);
		}
	}

	public override void AdditionalCloneSoData(StatusEffectScriptableObject sourceStatusEffect)
	{
		base.AdditionalCloneSoData(sourceStatusEffect);
		if (sourceStatusEffect is IntoxicatedItemStatusEffectScriptableObject intoxicatedItemStatusEffectScriptableObject)
		{
			IntoxicatedItemStatusEffectScriptableObject intoxicatedItemStatusEffectScriptableObject2 = intoxicatedItemStatusEffectScriptableObject;
			_inputScramble = intoxicatedItemStatusEffectScriptableObject2.InputScramble ?? (intoxicatedItemStatusEffectScriptableObject2.InputScramble = ControlScrambler.GenerateScramble(_scrambleInputType));
		}
	}

	public override void CheckHaveAntiStatusEffect(StatusEffectController.StatusEffect statusEffect)
	{
		base.CheckHaveAntiStatusEffect(statusEffect);
		if (statusEffect.statusEffectController.PlayerController.network.isLocalPlayer)
		{
			if (statusEffect.HasAntiStatusEffect)
			{
				OnRemoveEffect(statusEffect);
			}
			else if (_initEffect)
			{
				OnApplyEffectStatus(statusEffect);
			}
		}
	}
}
