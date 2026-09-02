using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using _Modules.Effects.StatusEffectsScripts;

namespace Toked.StatusEffect;

public class StatusEffectController : MonoBehaviour, IEffectable
{
	public enum StatusControllerType
	{
		PLAYER = 0,
		ENEMY = 1,
		OBJECT = 2
	}

	[Serializable]
	public class StatusEffect
	{
		public StatusEffectController statusEffectController;

		public PlayerController playerController;

		public StatusEffectScriptableObject statusEffectScriptableObject;

		public GameObject statusEffectGameObject;

		private float _timer;

		private int _stackCounter;

		private bool _isEffectApplied;

		private bool _hasAntiStatusEffect;

		private float _updatePerSecond;

		private Action<bool> onRemoveEvent;

		private Coroutine _statusEffectCoroutine;

		private TMP_Text _statusEffectDebugUI;

		public int StackCounter => _stackCounter;

		public bool HasAntiStatusEffect
		{
			get
			{
				return _hasAntiStatusEffect;
			}
			set
			{
				_hasAntiStatusEffect = value;
			}
		}

		public float Timer
		{
			get
			{
				return _timer;
			}
			set
			{
				_timer = value;
			}
		}

		public StatusEffect(StatusEffectController statusEffectController, PlayerController playerController, StatusEffectScriptableObject statusEffectScriptableObject, Action<bool> onRemoveAction = null)
		{
			this.statusEffectController = statusEffectController;
			this.playerController = playerController;
			this.statusEffectScriptableObject = statusEffectScriptableObject;
			_updatePerSecond = (statusEffectScriptableObject.CustomUpdateTime ? statusEffectScriptableObject.CustomUpdateTimeSeconds : 1f);
			onRemoveEvent = onRemoveAction;
		}

		public void SetEffectApplied(bool isEffectApplied)
		{
			_isEffectApplied = isEffectApplied;
		}

		public void SetRemainingTime(float time)
		{
			_timer = time;
		}

		public void ApplyStatus(bool initializeTimer = true)
		{
			StopStatusEffectCoroutine();
			_statusEffectCoroutine = statusEffectController?.StartCoroutine(ApplyStatusCoroutine(initializeTimer));
		}

		public void RemoveStatus(bool executeEvent = true)
		{
			onRemoveEvent?.Invoke(executeEvent);
			if (_isEffectApplied)
			{
				statusEffectScriptableObject.RemoveEffect(statusEffectController, this);
				_isEffectApplied = false;
			}
			_timer = 0f;
			_stackCounter = 0;
			StopStatusEffectCoroutine();
			RemoveStatusEffectDebugText();
			if (statusEffectScriptableObject.DestroyOnRemove)
			{
				UnityEngine.Object.Destroy(statusEffectScriptableObject);
			}
		}

		public void AddTime()
		{
			_timer = statusEffectScriptableObject.GetTotalEffectDuration(statusEffectController);
			_stackCounter++;
			CreateDebugText();
		}

		private IEnumerator ApplyStatusCoroutine(bool initializeTimer = true)
		{
			_isEffectApplied = false;
			if (initializeTimer)
			{
				AddTime();
			}
			else
			{
				CreateDebugText();
			}
			yield return statusEffectScriptableObject.OnApplyEffect(statusEffectController, this);
			_isEffectApplied = true;
			WaitForSeconds wait = new WaitForSeconds(_updatePerSecond);
			while (statusEffectScriptableObject.InfiniteDuration || _timer-- > 0f)
			{
				statusEffectScriptableObject.AdditionalUpdateFunction(_timer);
				OnUpdateDebugText();
				if ((!statusEffectScriptableObject.InfiniteDuration && _timer <= 0f) || statusEffectController.GetDeadStatus())
				{
					RemoveStatus();
					break;
				}
				statusEffectScriptableObject.ApplyEffect(playerController, statusEffectController, this);
				yield return wait;
			}
		}

		private void StopStatusEffectCoroutine()
		{
			if (_statusEffectCoroutine != null)
			{
				statusEffectController?.StopCoroutine(_statusEffectCoroutine);
				_statusEffectCoroutine = null;
			}
		}

		private void CreateDebugText()
		{
			if (GameModes.Instance.isDebug)
			{
				UpdateStatusEffectDebugText();
			}
		}

		private void RemoveStatusEffectDebugText()
		{
			if (GameModes.Instance.isDebug && _statusEffectDebugUI != null)
			{
				UnityEngine.Object.Destroy(_statusEffectDebugUI.gameObject);
			}
		}

		private void OnUpdateDebugText()
		{
			if (!GameModes.Instance.isDebug || !GameDebug.Instance.ShowStatusEffectDebug)
			{
				return;
			}
			if (statusEffectScriptableObject.InfiniteDuration)
			{
				if (_statusEffectDebugUI == null)
				{
					UpdateStatusEffectDebugText();
				}
			}
			else
			{
				UpdateStatusEffectDebugText();
			}
		}

		private void UpdateStatusEffectDebugText()
		{
			string text = $"{statusEffectScriptableObject.StatusEffectData.Name} {_timer}";
			if (_statusEffectDebugUI == null)
			{
				_statusEffectDebugUI = statusEffectController.StatusEffectDebugUI?.CreateTextDebug(text);
			}
			else
			{
				_statusEffectDebugUI.text = text;
			}
		}
	}

	public static readonly string STATUSEFFECT_DEBUG_UI = "Assets/Prefabs/Character/StatusEffectDebug/StatusControllerDebugCanvas.prefab";

	[SerializeField]
	private StatusControllerType _statusControllerType;

	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	private EnemyController _enemyController;

	[SerializeField]
	private ObjectCollisionBullet _objectCollisionController;

	private Dictionary<string, StatusEffect> _statusEffectsList = new Dictionary<string, StatusEffect>();

	[SerializeField]
	private StatusEffectDebugUI _statusEffectDebugUIPrefab;

	private StatusEffectDebugUI _statusEffectDebugUI;

	public StatusControllerType ControllerType => _statusControllerType;

	public PlayerController PlayerController
	{
		get
		{
			return _playerController;
		}
		set
		{
			_playerController = value;
		}
	}

	public EnemyController EnemyController
	{
		get
		{
			return _enemyController;
		}
		set
		{
			_enemyController = value;
		}
	}

	public ObjectCollisionBullet ObjectCollisionController
	{
		get
		{
			return _objectCollisionController;
		}
		set
		{
			_objectCollisionController = value;
		}
	}

	public Dictionary<string, StatusEffect> StatusEffectsList => _statusEffectsList;

	public int StatusEffectCount => _statusEffectsList.Count;

	public StatusEffectDebugUI StatusEffectDebugUI
	{
		get
		{
			if (_statusEffectDebugUI == null)
			{
				InitStatusEffectDebug();
			}
			return _statusEffectDebugUI;
		}
	}

	public event Action<StatusEffect> OnAddedStatusEffectEvent;

	public event Action<string> OnRemoveStatusEffectEvent;

	public event Action<string, string> OnSwapStatusEffectEvent;

	public event Action OnSyncStatusEffectEvent;

	public bool GetDeadStatus()
	{
		return _statusControllerType switch
		{
			StatusControllerType.PLAYER => (bool)_playerController && _playerController.network.IsDead(), 
			StatusControllerType.ENEMY => (bool)_enemyController && _enemyController.network.IsDead(), 
			StatusControllerType.OBJECT => _objectCollisionController.GetStatusDestroy(), 
			_ => true, 
		};
	}

	public void ApplyStatus(PlayerController playerController, StatusEffectScriptableObject statusEffectScriptableObject, bool executeEvent = true)
	{
		if (!statusEffectScriptableObject || GetDeadStatus())
		{
			return;
		}
		string key = statusEffectScriptableObject.StatusEffectData.Name;
		if (!_statusEffectsList.ContainsKey(key))
		{
			StatusEffect statusEffect = new StatusEffect(this, playerController, statusEffectScriptableObject, (bool executeRemoveEvent) =>
			{
				RemoveStatus(statusEffectScriptableObject, executeRemoveEvent);
			});
			_statusEffectsList.Add(key, statusEffect);
			statusEffect.ApplyStatus();
			if (executeEvent)
			{
				OnAddedStatusEffectEvent?.Invoke(statusEffect);
			}
		}
		else
		{
			_statusEffectsList[key].AddTime();
		}
	}

	public StatusEffect ApplyStatusFromNetwork(PlayerController playerController, StatusEffectScriptableObject statusEffectScriptableObject, float remainingTime)
	{
		if (!statusEffectScriptableObject)
		{
			return null;
		}
		if (GetDeadStatus())
		{
			return null;
		}
		string key = statusEffectScriptableObject.StatusEffectData.Name;
		if (!_statusEffectsList.TryGetValue(key, out var value))
		{
			value = new StatusEffect(this, playerController, statusEffectScriptableObject, (bool executeRemoveEvent) =>
			{
				RemoveStatus(statusEffectScriptableObject, executeRemoveEvent);
			});
			_statusEffectsList.Add(key, value);
		}
		value.SetRemainingTime(remainingTime);
		value.ApplyStatus(initializeTimer: false);
		return value;
	}

	public void ChangeKeyStatusEffect(string oldKey, string newKey, bool isSwap, bool isSwapSameType)
	{
		ChangeKey(StatusEffectsList, oldKey, newKey, isSwap, isSwapSameType);
		if (!isSwapSameType)
		{
			OnSwapStatusEffectEvent?.Invoke(oldKey, newKey);
		}
	}

	public void ClearAllStatusEffect()
	{
		foreach (StatusEffect item in _statusEffectsList.Values.ToList())
		{
			if (!(item?.statusEffectScriptableObject) || !item.statusEffectScriptableObject.CantClearEffectAfterFinishedMission)
			{
				item?.RemoveStatus();
			}
		}
	}

	public void SyncStatusEffectController()
	{
		OnSyncStatusEffectEvent?.Invoke();
	}

	public bool CheckContainEffectStatus(string key, params string[] exceptions)
	{
		foreach (StatusEffect value2 in _statusEffectsList.Values)
		{
			string text = value2.statusEffectScriptableObject.StatusEffectData.Name;
			int num = 0;
			while (true)
			{
				if (num < exceptions.Length)
				{
					string value = exceptions[num];
					if (text.Contains(value))
					{
						break;
					}
					num++;
					continue;
				}
				if (!text.Contains(key))
				{
					break;
				}
				return true;
			}
		}
		return false;
	}

	public List<StatusEffect> GetAllStatusEffectsContainName(string key)
	{
		List<StatusEffect> list = new List<StatusEffect>();
		foreach (StatusEffect value in _statusEffectsList.Values)
		{
			if (value.statusEffectScriptableObject.StatusEffectData.Name.StartsWith(key))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void ClearStatus(StatusEffectScriptableObject statusEffectScriptableObject, bool executeEvent = true)
	{
		if (!(statusEffectScriptableObject == null))
		{
			ClearStatus(statusEffectScriptableObject.StatusEffectData.Name, executeEvent);
		}
	}

	public void ClearStatus(string key, bool executeEvent = true)
	{
		if (_statusEffectsList.ContainsKey(key))
		{
			_statusEffectsList[key]?.RemoveStatus(executeEvent);
		}
	}

	public void ClearStatusContains(string key, bool executeEvent = true)
	{
		foreach (StatusEffect item in GetAllStatusEffectsContainName(key))
		{
			item?.RemoveStatus(executeEvent);
		}
	}

	private void RemoveStatus(StatusEffectScriptableObject statusEffectScriptableObject, bool executeEvent)
	{
		string text = statusEffectScriptableObject.StatusEffectData.Name;
		if (!string.IsNullOrWhiteSpace(text))
		{
			_statusEffectsList.Remove(text);
			if (executeEvent)
			{
				OnRemoveStatusEffectEvent?.Invoke(text);
			}
		}
	}

	private void InitStatusEffectDebug()
	{
		if (GameModes.Instance.isDebug && !(_statusEffectDebugUIPrefab == null) && _statusControllerType == StatusControllerType.PLAYER)
		{
			if (_statusEffectDebugUI == null)
			{
				_statusEffectDebugUI = UnityEngine.Object.Instantiate(_statusEffectDebugUIPrefab, GetTransform());
			}
			_statusEffectDebugUI.gameObject.SetActive(GameDebug.Instance.ShowStatusEffectDebug);
		}
		Transform GetTransform()
		{
			return _statusControllerType switch
			{
				StatusControllerType.PLAYER => _playerController.characterRenderController.transform, 
				StatusControllerType.ENEMY => _enemyController.enemyCharacterRenderController.transform, 
				StatusControllerType.OBJECT => _objectCollisionController.transform, 
				_ => base.transform, 
			};
		}
	}

	public void ChangeKey<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey oldKey, TKey newKey, bool isSwap, bool isSwapSameType)
	{
		if (!isSwapSameType)
		{
			if (dict.ContainsKey(oldKey))
			{
				TValue value = dict[oldKey];
				dict.Remove(oldKey);
				dict[newKey] = value;
			}
		}
		else
		{
			TValue val = dict[newKey];
			TValue val2 = dict[oldKey];
			TValue val3 = (dict[oldKey] = val);
			val3 = (dict[newKey] = val2);
		}
	}

	public void SetReference(bool withDebugUi)
	{
		ResetData();
		if (base.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			_statusControllerType = StatusControllerType.PLAYER;
			_playerController = GetComponent<PlayerController>();
		}
		else if (base.gameObject.CompareTag(EnemyController.EMEMY_TAG))
		{
			_statusControllerType = StatusControllerType.ENEMY;
			_enemyController = GetComponent<EnemyController>();
		}
		else if (base.gameObject.CompareTag(ObjectCollisionBullet.DESTRUCTABLE_OBJECT_TAG))
		{
			_statusControllerType = StatusControllerType.OBJECT;
			_objectCollisionController = GetComponent<ObjectCollisionBullet>();
		}
		void ResetData()
		{
			_statusEffectDebugUIPrefab = null;
			_playerController = null;
			_enemyController = null;
			_objectCollisionController = null;
		}
	}
}
