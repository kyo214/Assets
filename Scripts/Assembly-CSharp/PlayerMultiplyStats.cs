using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerMultiplyStats : MonoBehaviour
{
	[SerializeField]
	private PlayerStatsSO _multiplyMeleePenaltyMove;

	[SerializeField]
	private PlayerStatsSO _multiplyRangePenaltyMove;

	[SerializeField]
	private PlayerStatsSO _multiplyAnimSpeedReload;

	[SerializeField]
	private PlayerStatsSO _multiplyStamina;

	[SerializeField]
	private PlayerStatsSO _multiplyStaminaSprintConsumption;

	[SerializeField]
	private PlayerStatsSO _multiplyStaminaMeleeConsumption;

	[SerializeField]
	private PlayerStatsSO _multiplyStaminaDashConsumption;

	[SerializeField]
	private PlayerStatsSO _multiplyHealthPotency;

	[SerializeField]
	private PlayerStatsSO _multiplyMeleeDamage;

	[SerializeField]
	private PlayerStatsSO _multiplyStaminaRecovery;

	[SerializeField]
	private PlayerStatsSO _multiplyTimeRevive;

	[SerializeField]
	private PlayerStatsSO _multiplyHealthRestored;

	[SerializeField]
	private PlayerStatsSO _multiplyDamageReduction;

	[SerializeField]
	private PlayerStatsSO _multiplyMovementSpeed;

	[SerializeField]
	private PlayerStatsSO _multiplySprintSpeed;

	[SerializeField]
	private PlayerStatsSO _multiplyTimerGunAccuracy;

	[SerializeField]
	private PlayerStatsSO _boundLowHpSlowWalk;

	[SerializeField]
	private PlayerStatsSO _healLowHpAmount;

	[SerializeField]
	private PlayerStatsSO _dashAttackDamage;

	[SerializeField]
	private PlayerStatsSO _multiplyDamageExplosion;

	[SerializeField]
	private PlayerStatsSO _reduceBurnDuration;

	[SerializeField]
	private List<PlayerStatsSO> _listStats = new List<PlayerStatsSO>();

	[SerializeField]
	private float _minMultiplyValue = 0.01f;

	public Action<PlayerStatsSO> OnPlayerStatsChangedEvents;

	public List<PlayerStatsSO> ListStats => _listStats;

	public float GetMultiplyMeleePenaltyMove()
	{
		return _multiplyMeleePenaltyMove.Value;
	}

	public float GetMultiplyRangePenaltyMove()
	{
		return _multiplyRangePenaltyMove.Value;
	}

	public float GetMultiplyAnimSpeedReload()
	{
		return _multiplyAnimSpeedReload.Value;
	}

	public float GetMultiplyStamina()
	{
		return _multiplyStamina.Value;
	}

	public float GetMultiplyStaminaSprintConsumption()
	{
		return _multiplyStaminaSprintConsumption.Value;
	}

	public float GetMultiplyStaminaMeleeConsumption()
	{
		return _multiplyStaminaMeleeConsumption.Value;
	}

	public float GetMultiplyStaminaDashConsumption()
	{
		return _multiplyStaminaDashConsumption.Value;
	}

	public float GetMultiplyHealthPotency()
	{
		return _multiplyHealthPotency.Value;
	}

	public float GetMultiplyMeleeDamage()
	{
		return _multiplyMeleeDamage.Value;
	}

	public float GetMultiplyStaminaRecovery()
	{
		return _multiplyStaminaRecovery.Value;
	}

	public float GetMultiplyTimeRevive()
	{
		return _multiplyTimeRevive.Value;
	}

	public float GetMultiplyHealthRestored()
	{
		return _multiplyHealthRestored.Value;
	}

	public float GetMultiplyDamageReduction()
	{
		return _multiplyDamageReduction.Value;
	}

	public float GetMultiplyMovementSpeed()
	{
		return _multiplyMovementSpeed.Value;
	}

	public float GetMultiplySprintSpeed()
	{
		return _multiplySprintSpeed.Value;
	}

	public float GetMultiplyTimerGunAccuracy()
	{
		return _multiplyTimerGunAccuracy.Value;
	}

	public float GetBoundLowHpSlowWalk()
	{
		return _boundLowHpSlowWalk.Value;
	}

	public float GetHealLowHpAmount()
	{
		return _healLowHpAmount.Value;
	}

	public float GetDashAttackDamage()
	{
		return _dashAttackDamage.Value;
	}

	public float GetMultiplyDamageExplosion()
	{
		return _multiplyDamageExplosion.Value;
	}

	public float GetBurnDuration()
	{
		return _reduceBurnDuration.Value;
	}

	private void Awake()
	{
		FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(this);
			if (value is PlayerStatsSO)
			{
				fieldInfo.SetValue(this, UnityEngine.Object.Instantiate(value as PlayerStatsSO));
				_listStats.Add(fieldInfo.GetValue(this) as PlayerStatsSO);
			}
		}
	}

	private void OnDestroy()
	{
		foreach (PlayerStatsSO listStat in _listStats)
		{
			if (listStat != null)
			{
				UnityEngine.Object.Destroy(listStat);
			}
		}
		_listStats.Clear();
	}

	private string TrimAfterKeyword(string input, string keyword)
	{
		int num = input.IndexOf(keyword, StringComparison.Ordinal);
		if (num != -1)
		{
			return input.Substring(0, num);
		}
		return input;
	}

	public void AddValue(string SOName, float value)
	{
		PlayerStatsSO playerStatsSo = GetPlayerStatsSo(SOName);
		if (!(playerStatsSo == null))
		{
			playerStatsSo.Value += value;
			playerStatsSo.Value = Mathf.Max(playerStatsSo.Value, _minMultiplyValue);
			OnPlayerStatsChangedEvents?.Invoke(playerStatsSo);
		}
	}

	public void SetValue(string SOName, float value)
	{
		PlayerStatsSO playerStatsSo = GetPlayerStatsSo(SOName);
		if (!(playerStatsSo == null))
		{
			playerStatsSo.Value = Mathf.Max(value, _minMultiplyValue);
			OnPlayerStatsChangedEvents?.Invoke(playerStatsSo);
		}
	}

	public PlayerStatsSO GetPlayerStatsSo(string soName)
	{
		foreach (PlayerStatsSO listStat in _listStats)
		{
			if (TrimAfterKeyword(listStat.name, "(Clone)") == soName)
			{
				return listStat;
			}
		}
		return null;
	}
}
