using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Toked.StatusEffect;
using UnityEngine;

namespace Toked.Weapon.Throwable;

public class AreaImpactItemBase : MonoBehaviour
{
	[SerializeField]
	protected List<IEffectable> _effectableList = new List<IEffectable>();

	[SerializeField]
	protected PlayerController _playerController;

	[SerializeField]
	protected int _areaDps = 10;

	[SerializeField]
	protected int _timeDespawn = 5;

	[SerializeField]
	protected StatusEffectScriptableObject _statusEffectScriptableObject;

	protected bool _active;

	protected bool _isColliderActive;

	protected bool _counterActive;

	protected int _effectCount;

	protected float _timePassed;

	protected float _currentImpactDuration;

	protected float _currentImpactDps;

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

	private void OnDisable()
	{
		Reset();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isColliderActive)
		{
			IEffectable effectable = null;
			if (other.gameObject.CompareTag(PlayerController.PLAYER_COLLIDER_TAG))
			{
				effectable = other.GetComponentInParent<IEffectable>();
				AddToEffectList(effectable);
			}
			else if (other.gameObject.CompareTag(EnemyController.EMEMY_COLLIDER_TAG))
			{
				effectable = other.GetComponent<EnemyCollider>().StatusEffect;
				AddToEffectList(effectable);
			}
			else if (other.gameObject.CompareTag(ObjectCollisionBullet.DESTRUCTABLE_OBJECT_TAG))
			{
				effectable = other.GetComponent<IEffectable>();
				AddToEffectList(effectable);
			}
			if (_effectCount > 0)
			{
				_active = true;
			}
		}
		void AddToEffectList(IEffectable effectable2)
		{
			if (effectable2 != null && !_effectableList.Contains(effectable2))
			{
				_effectableList.Add(effectable2);
				_effectCount = _effectableList.Count;
			}
		}
	}

	private void Update()
	{
		if (_active)
		{
			for (int i = 0; i < _effectCount; i++)
			{
				ApplyStatusEffect(_effectableList[i]);
			}
		}
	}

	protected virtual void FixedUpdate()
	{
		if (!_counterActive)
		{
			return;
		}
		_timePassed += Time.deltaTime;
		if (_timePassed > _currentImpactDuration)
		{
			Reset();
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				_isColliderActive = false;
			}).Forget();
			UniTaskUtil.DelayedCall(this, 3f, () =>
			{
				Release();
			}).Forget();
		}
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag(PlayerController.PLAYER_COLLIDER_TAG) || other.gameObject.CompareTag(EnemyController.EMEMY_COLLIDER_TAG) || other.gameObject.CompareTag(ObjectCollisionBullet.DESTRUCTABLE_OBJECT_TAG))
		{
			IEffectable componentInParent = other.GetComponentInParent<IEffectable>();
			if (componentInParent != null && _effectableList.Contains(componentInParent))
			{
				_effectableList.Remove(componentInParent);
				_effectCount = _effectableList.Count;
			}
		}
		if (_effectCount <= 0)
		{
			_active = false;
		}
	}

	public virtual void Init(PlayerController playerController, float impactDuration = -1f, float impactDps = -1f)
	{
		Reset();
		_currentImpactDuration = ((impactDuration <= 0f) ? ((float)_timeDespawn) : impactDuration);
		_currentImpactDps = ((impactDps <= 0f) ? ((float)_areaDps) : impactDps);
		_playerController = playerController;
		base.gameObject.SetActive(value: true);
		_counterActive = true;
		_isColliderActive = true;
	}

	protected virtual void ApplyStatusEffect(IEffectable effectable)
	{
		if (!(effectable is StatusEffectController { ControllerType: StatusEffectController.StatusControllerType.PLAYER } statusEffectController) || !statusEffectController.PlayerController.isDashing)
		{
			effectable.ApplyStatus(_playerController, _statusEffectScriptableObject);
		}
	}

	protected virtual void Release()
	{
		base.gameObject.SetActive(value: false);
	}

	protected virtual void Reset()
	{
		_timePassed = 0f;
		_active = false;
		_counterActive = false;
		_effectCount = 0;
		_effectableList.Clear();
	}
}
