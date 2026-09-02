using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using Toked.StatusEffect;
using UnityEngine;

public class DeadhopAttackBehaviour : MonoBehaviour
{
	private EnemyController enemyController;

	private EnemyAttack enemyAttack;

	public List<GameObject> arrBullet = new List<GameObject>();

	[SerializeField]
	private Transform spawner;

	[SerializeField]
	private LayerMask layerObstacle;

	[SerializeField]
	private ToxinStatusEffectScriptableObject toxinStatusEffect;

	private void Start()
	{
		enemyController = base.transform.parent.GetComponent<EnemyController>();
		enemyAttack = base.transform.parent.GetComponent<EnemyAttack>();
	}

	public void Attack()
	{
		enemyController.isAttacking = true;
		if (enemyAttack.targetChasing != null)
		{
			enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
		}
		if (GetBullet() != null)
		{
			enemyController.network.SetAnimation("Attack" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		}
		enemyController.SetAISpeed(enemyController.data.GetSpeed());
		enemyAttack.timerTriggerAttack.StartDuration(1f);
		enemyController.network.networkPhoton.PosTarget = enemyAttack.targetChasing.position;
		enemyController.network.networkPhoton.AttackSeed = (int)DateTime.Now.Ticks;
		if (MathFunc.DistanceSameYPos(enemyController.network.networkPhoton.PosTarget, enemyController.middlePos.position) > 4f)
		{
			Vector3 normalized = (enemyController.network.networkPhoton.PosTarget - enemyController.middlePos.position).normalized;
			enemyController.network.networkPhoton.PosTarget = enemyController.middlePos.position + normalized * enemyController.data.distChasing / 2f;
		}
		enemyController.SetAISpeed(0f);
	}

	public void SpawnBullet()
	{
		UnityEngine.Random.InitState(enemyController.network.networkPhoton.AttackSeed);
		GameObject bullet = GetBullet();
		if (bullet != null)
		{
			bullet.transform.position = spawner.position;
			bullet.SetActive(value: true);
			Vector3 vector = new Vector3(spawner.position.x, enemyController.middlePos.position.y, spawner.position.z);
			Vector3 vector2 = new Vector3(enemyController.network.networkPhoton.PosTarget.x, enemyController.middlePos.position.y, enemyController.network.networkPhoton.PosTarget.z);
			bullet.transform.DOKill();
			if (Physics.SphereCast(vector, 0.5f, (vector2 - vector).normalized, out var hitInfo, MathFunc.Distance(vector2, vector), layerObstacle))
			{
				bullet.transform.DOJump(new Vector3(hitInfo.point.x, enemyController.network.networkPhoton.PosTarget.y, hitInfo.point.z), 1f, 1, 0.5f).OnComplete(() =>
				{
					OnBulletLand(bullet);
				});
			}
			else
			{
				bullet.transform.DOJump(enemyController.network.networkPhoton.PosTarget, 1f, 1, 0.5f).OnComplete(() =>
				{
					OnBulletLand(bullet);
				});
			}
		}
		if (NetworkGameManager.Instance.isServer && (bool)enemyAttack.targetChasing && MathFunc.Distance(base.transform.position, enemyAttack.targetChasing.position) > enemyController.data.distAttack)
		{
			enemyController.isAttacking = false;
			enemyController.attack.DisableAllTimer();
			enemyController.attack.fov.visibleTargets.Clear();
			enemyController.attack.timerTriggerAttack.StopDuration();
			enemyController.attack.timerDelayChasing.StopDuration();
			enemyController.attack.timerRandomIdleChasing.StopDuration();
			enemyController.attack.timerIdleChasing.StopDuration();
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				enemyController.SetState(EnemyState.Chasing);
				enemyController.network.SetAnimation("Move" + enemyController.movement.angleAnim);
			}).Forget();
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void Special()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			enemyController.network.SetAnimation("Special1-" + enemyController.movement.angleAnim);
			enemyController.network.networkPhoton.PosTarget = enemyAttack.targetChasing.position;
			enemyController.network.networkPhoton.AttackSeed = (int)DateTime.Now.Ticks;
			if (MathFunc.DistanceSameYPos(enemyController.network.networkPhoton.PosTarget, enemyController.middlePos.position) > 4f)
			{
				Vector3 normalized = (enemyController.network.networkPhoton.PosTarget - enemyController.middlePos.position).normalized;
				enemyController.network.networkPhoton.PosTarget = enemyController.middlePos.position + normalized * enemyController.data.distChasing / 2f;
			}
			enemyController.SetAISpeed(0f);
		}
	}

	public void SpawnSpecialBullet()
	{
		UnityEngine.Random.InitState(enemyController.network.networkPhoton.AttackSeed);
		for (int i = 0; i < 3; i++)
		{
			arrBullet[i].transform.position = spawner.position;
			arrBullet[i].SetActive(value: true);
			arrBullet[i].transform.DOKill();
			Vector3 vector = new Vector3(spawner.position.x, enemyController.middlePos.position.y, spawner.position.z);
			Vector3 vector2 = ((i != 0) ? new Vector3(base.transform.position.x + UnityEngine.Random.Range(-6f, 6f), enemyController.middlePos.position.y, base.transform.position.z + UnityEngine.Random.Range(-6f, 6f)) : new Vector3(enemyController.network.networkPhoton.PosTarget.x + UnityEngine.Random.Range(1f, 1f), enemyController.middlePos.position.y, enemyController.network.networkPhoton.PosTarget.z + UnityEngine.Random.Range(1f, 1f)));
			if (Physics.SphereCast(vector, 0.5f, (vector2 - vector).normalized, out var hitInfo, MathFunc.Distance(vector2, vector), layerObstacle))
			{
				arrBullet[i].transform.DOJump(new Vector3(hitInfo.point.x, enemyController.network.networkPhoton.PosTarget.y, hitInfo.point.z), 5f, 1, 1f);
			}
			else
			{
				arrBullet[i].transform.DOJump(new Vector3(vector2.x, enemyController.network.networkPhoton.PosTarget.y, vector2.z), 5f, 1, 1f);
			}
		}
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			for (int j = 0; j < 3; j++)
			{
				OnBulletLand(arrBullet[j]);
			}
		}).Forget();
		if (NetworkGameManager.Instance.isServer && MathFunc.Distance(base.transform.position, enemyAttack.targetChasing.position) > enemyController.data.distAttack)
		{
			enemyController.isAttacking = false;
			enemyController.attack.DisableAllTimer();
			enemyController.attack.fov.visibleTargets.Clear();
			enemyController.attack.timerTriggerAttack.StopDuration();
			enemyController.attack.timerDelayChasing.StopDuration();
			enemyController.attack.timerRandomIdleChasing.StopDuration();
			enemyController.attack.timerIdleChasing.StopDuration();
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				enemyController.SetState(EnemyState.Chasing);
				enemyController.network.SetAnimation("Move" + enemyController.movement.angleAnim);
			}).Forget();
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private GameObject GetBullet()
	{
		GameObject result = null;
		using (List<GameObject>.Enumerator enumerator = arrBullet.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				GameObject current = enumerator.Current;
				if (!current.activeSelf)
				{
					result = current;
				}
			}
		}
		return result;
	}

	private void OnBulletLand(GameObject bulletObj)
	{
		ToxinSpawner.Instance.GetItem(null, bulletObj.transform.position, -1f, toxinStatusEffect._damage);
		bulletObj.transform.DOKill();
		UniTaskUtil.DelayedCall(this, 0.5f, () =>
		{
			bulletObj.SetActive(value: false);
		}).Forget();
	}
}
