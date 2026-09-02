using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feel;

public class Barbarian : MonoBehaviour
{
	[Header("Cooldown")]
	[Tooltip("a duration, in seconds, between two attacks, during which attacks are prevented")]
	public float CooldownDuration = 0.1f;

	[Header("Feedbacks")]
	[Tooltip("a feedback to call when the attack starts")]
	public MMFeedbacks AttackFeedback;

	[Tooltip("a feedback to call when each individual attack phase starts")]
	public MMFeedbacks IndividualAttackFeedback;

	[Tooltip("a feedback to call when trying to attack while in cooldown")]
	public MMFeedbacks DeniedFeedback;

	[Header("Attack settings")]
	public MMTween.MMTweenCurve AttackCurve = MMTween.MMTweenCurve.EaseInOutOverhead;

	public float AttackDuration = 2.5f;

	public float AttackPositionOffset = 0.3f;

	public float IntervalDecrement = 0.1f;

	protected List<Vector3> _targets;

	protected float _lastAttackStartedAt = -100f;

	protected Vector3 _initialPosition;

	protected Vector3 _initialLookAtTarget;

	protected Vector3 _lookAtTarget;

	protected BarbarianEnemy _enemy;

	protected virtual void Awake()
	{
		_initialPosition = base.transform.position;
		_initialLookAtTarget = base.transform.position + base.transform.forward * 10f;
		_lookAtTarget = _initialLookAtTarget;
	}

	protected virtual void Update()
	{
		HandleInput();
		LookAtTarget();
	}

	protected virtual void LookAtTarget()
	{
		Vector3 vector = _lookAtTarget - _initialPosition;
		base.transform.LookAt(_lookAtTarget + vector * 5f);
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			Attack();
		}
	}

	protected virtual void Attack()
	{
		if (Time.time - _lastAttackStartedAt < CooldownDuration + AttackDuration)
		{
			DeniedFeedback?.PlayFeedbacks();
			return;
		}
		AcquireTargets();
		StartCoroutine(AttackCoroutine());
		_lastAttackStartedAt = Time.time;
	}

	protected virtual void AcquireTargets()
	{
		_targets = new List<Vector3>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, 5f);
		foreach (Collider obj in array)
		{
			Vector3 position = obj.transform.position;
			Vector3 vector = base.transform.position - position;
			if (obj.GetComponent<BarbarianEnemy>() != null)
			{
				_targets.Add(position + vector * AttackPositionOffset);
			}
		}
		_targets.MMShuffle();
	}

	protected virtual IEnumerator AttackCoroutine()
	{
		float intervalDuration = AttackDuration / (float)_targets.Count;
		AttackFeedback?.PlayFeedbacks();
		int enemyCounter = 0;
		foreach (Vector3 target in _targets)
		{
			IndividualAttackFeedback?.PlayFeedbacks();
			MMTween.MoveTransform(this, base.transform, base.transform.position, target, null, 0f, intervalDuration, AttackCurve);
			_lookAtTarget = target;
			yield return MMCoroutine.WaitFor(intervalDuration - (float)enemyCounter * IntervalDecrement);
			enemyCounter++;
		}
		MMTween.MoveTransform(this, base.transform, base.transform.position, _initialPosition, null, 0f, intervalDuration, AttackCurve);
		_lookAtTarget = _initialLookAtTarget;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		_enemy = other.GetComponent<BarbarianEnemy>();
		if (_enemy != null)
		{
			int damage = Random.Range(50, 250);
			_enemy.TakeDamage(damage);
		}
	}
}
