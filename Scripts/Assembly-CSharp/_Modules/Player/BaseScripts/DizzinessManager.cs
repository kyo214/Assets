using System;
using System.Collections;
using System.Collections.Generic;
using Toked.StatusEffect;
using UnityEngine;

namespace _Modules.Player.BaseScripts;

public class DizzinessManager : MonoBehaviour
{
	[SerializeField]
	private PlayerController _playerController;

	[Header("Settings")]
	[SerializeField]
	private int _minPoints;

	[SerializeField]
	private float _intoxicatedThresholdPercentage = 0.5f;

	[SerializeField]
	private float _intervalDecay = 0.5f;

	[SerializeField]
	private int _decayPerTick = 1;

	[SerializeField]
	private float _gainInterval = 0.1f;

	[SerializeField]
	private int _gainPerTick = 1;

	[SerializeField]
	private StatusEffectScriptableObject _statusEffectScriptableObject;

	[SerializeField]
	private List<StatusEffectScriptableObject> _antiStatusEffectsList = new List<StatusEffectScriptableObject>();

	private StatusEffectScriptableObject _playerStatusEffectScriptableObject;

	private int _targetPoints;

	private Coroutine _gainRoutine;

	private CameraGame _cameraGame;

	private Coroutine _decayRoutine;

	public float MaxPoints => _playerController.data.GetCurrentMaxStamina();

	public float MaxPointPercentage => 1f / MaxPoints;

	public int IntoxicatedThreshold => (int)(MaxPoints * _intoxicatedThresholdPercentage);

	public float IntoxicatedThresholdPercentage => _intoxicatedThresholdPercentage;

	public StatusEffectScriptableObject PlayerStatusEffectScriptableObject => _playerStatusEffectScriptableObject ?? (_playerStatusEffectScriptableObject = _statusEffectScriptableObject.CloneStatusEffectSO(destroyOnRemove: false));

	public int CurrentPoints { get; private set; }

	public float CurrentPointsPercentage => (float)CurrentPoints * MaxPointPercentage;

	public bool IsIntoxicated { get; private set; }

	private bool IsAddingPoints => CurrentPoints < _targetPoints;

	public event Action<int> OnPointsChanged;

	public event Action<bool> OnIntoxicatedStateChanged;

	private void Awake()
	{
		_targetPoints = CurrentPoints;
		UpdateIntoxicatedState();
	}

	public void AddPoints(int amount)
	{
		if (amount != 0)
		{
			if (amount < 0)
			{
				RemovePoints(-amount);
			}
			else if (!_playerController.network.IsDead() && !CheckHaveAntiStatusEffect())
			{
				_targetPoints = Mathf.Clamp(_targetPoints + amount, _minPoints, (int)MaxPoints);
				StartGainRoutineIfNeeded();
				UpdateDecayRoutine();
			}
		}
	}

	public void RemovePoints(int amount)
	{
		if (amount > 0)
		{
			int num = Mathf.Max(_minPoints, CurrentPoints - amount);
			_targetPoints = Mathf.Min(_targetPoints, num);
			SetPoints(num);
		}
	}

	public void ClearPoints()
	{
		_targetPoints = 0;
		if (_gainRoutine != null)
		{
			StopCoroutine(_gainRoutine);
			_gainRoutine = null;
		}
		SetPoints(0);
	}

	private void StartGainRoutineIfNeeded()
	{
		if (_gainRoutine == null)
		{
			_gainRoutine = StartCoroutine(GainRoutine());
		}
	}

	private IEnumerator GainRoutine()
	{
		if (_gainPerTick <= 0)
		{
			Debug.LogError("DizzinessManager: GainPerTick must be > 0");
			_gainRoutine = null;
			yield break;
		}
		WaitForSeconds wait = new WaitForSeconds(_gainInterval);
		while (CurrentPoints < _targetPoints)
		{
			yield return wait;
			int points = Mathf.Min(_targetPoints, CurrentPoints + _gainPerTick);
			SetPoints(points);
			OnGainPointsChangedAction();
		}
		_gainRoutine = null;
	}

	private void SetPoints(int value)
	{
		int num = Mathf.Clamp(value, _minPoints, (int)MaxPoints);
		if (num != CurrentPoints)
		{
			CurrentPoints = num;
			if (_targetPoints < CurrentPoints)
			{
				_targetPoints = CurrentPoints;
			}
			OnPointsChanged?.Invoke(CurrentPoints);
			UpdateIntoxicatedState();
			UpdateDecayRoutine();
		}
	}

	private void UpdateIntoxicatedState()
	{
		bool flag = CurrentPoints > IntoxicatedThreshold;
		if (flag != IsIntoxicated)
		{
			IsIntoxicated = flag;
			OnIntoxicatedStateChanged?.Invoke(IsIntoxicated);
			if (IsIntoxicated)
			{
				EnableIntoxicatedEffect();
			}
			else
			{
				DisableIntoxicatedEffect();
			}
		}
	}

	private void UpdateDecayRoutine()
	{
		if (CurrentPoints > 0 || _targetPoints > 0)
		{
			if (_decayRoutine == null)
			{
				_decayRoutine = StartCoroutine(DecayRoutine());
			}
		}
		else if (_decayRoutine != null)
		{
			StopCoroutine(_decayRoutine);
			_decayRoutine = null;
		}
	}

	private IEnumerator DecayRoutine()
	{
		WaitForSeconds wait = new WaitForSeconds(_intervalDecay);
		while (CurrentPoints > 0 || _targetPoints > 0)
		{
			yield return wait;
			if (!IsAddingPoints)
			{
				SetPoints(_targetPoints = Mathf.Max(_minPoints, CurrentPoints - _decayPerTick));
			}
		}
		_decayRoutine = null;
	}

	private void EnableIntoxicatedEffect()
	{
		if (_playerController.network.isLocalPlayer)
		{
			_playerController.StatusEffectController.ApplyStatus(_playerController, PlayerStatusEffectScriptableObject);
		}
	}

	private void DisableIntoxicatedEffect()
	{
		if (_playerController.network.isLocalPlayer)
		{
			_playerController.StatusEffectController.ClearStatus(PlayerStatusEffectScriptableObject);
		}
	}

	private bool CheckHaveAntiStatusEffect()
	{
		foreach (StatusEffectScriptableObject antiStatusEffects in _antiStatusEffectsList)
		{
			if (_playerController.StatusEffectController.CheckContainEffectStatus(antiStatusEffects.StatusEffectData.BaseName))
			{
				return true;
			}
		}
		return false;
	}

	private void OnGainPointsChangedAction()
	{
		if (_playerController.network.isLocalPlayer)
		{
			_playerController.network.playerPhoton.RpcSicknessEffect();
		}
	}
}
