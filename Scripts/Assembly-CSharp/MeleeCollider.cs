using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
	public WeaponController weaponController;

	public EnemyData enemyData;

	public bool isPlayer = true;

	[SerializeField]
	private LayerMask obstacleMask;

	[SerializeField]
	private List<EnemyController> _arrEnemyHit = new List<EnemyController>();

	[SerializeField]
	private bool _isHitEnemy;

	[SerializeField]
	private DummyController dummyTarget;

	[SerializeField]
	private bool _noShake;

	private void Start()
	{
		if (isPlayer)
		{
			Physics.IgnoreCollision(GetComponent<Collider>(), weaponController.playerController.playerCollider.GetComponent<Collider>());
		}
	}

	private void OnDisable()
	{
		if (_isHitEnemy)
		{
			Vector3 myPos = base.transform.position;
			_arrEnemyHit.Sort((EnemyController a, EnemyController b) =>
			{
				float sqrMagnitude = (a.middlePos.position - myPos).sqrMagnitude;
				float sqrMagnitude2 = (b.middlePos.position - myPos).sqrMagnitude;
				return sqrMagnitude.CompareTo(sqrMagnitude2);
			});
			int num = 3;
			if (weaponController.playerController.isAttackMeleeSwing)
			{
				num = (weaponController.isMeleeCharging ? weaponController.MaxEnemyHitFullCharge : ((!weaponController.isHalfMeleeCharging) ? weaponController.MaxEnemyHit : weaponController.MaxEnemyHitHalfCharge));
			}
			else if (weaponController.playerController.isDashing)
			{
				num = 5;
			}
			for (int num2 = 0; num2 < num; num2++)
			{
				if (_arrEnemyHit.Count > num2)
				{
					weaponController.CheckDamageToEnemy(_arrEnemyHit[num2]);
				}
			}
			weaponController.CheckEnemyAggro(weaponController.transform, 3f);
		}
		_arrEnemyHit.Clear();
		_isHitEnemy = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isPlayer && other.CompareTag("EnemyCollider"))
		{
			_arrEnemyHit.Add(other.GetComponent<EnemyCollider>().enemyControler);
			_isHitEnemy = true;
		}
		if (other.CompareTag("PlayerCollider"))
		{
			PlayerController component = other.transform.parent.GetComponent<PlayerController>();
			if (component.network.GetHealth() > 0f && !component.isDashing && !component.network.playerPhoton.disconnected)
			{
				if (isPlayer && GameModes.Instance.friendlyFire)
				{
					weaponController.CheckDamageToOtherPlayer(component);
				}
				else if (!isPlayer && !component.isHurt && !GameManagerPhoton.Instance.IsWin && component.network.isLocalPlayer)
				{
					Vector3 normalized = (component.weaponPos.position - base.transform.parent.position).normalized;
					float maxDistance = Vector3.Distance(base.transform.parent.position, component.weaponPos.position);
					if (!Physics.Raycast(enemyData.enemyController.middlePos.position, normalized, maxDistance, obstacleMask))
					{
						if (enemyData.enemyController.network.IsSpecialAttacking() && enemyData.enemyController.attack.special1Type != "")
						{
							if (enemyData.enemyController.attack.special1Type == "Entangle")
							{
								enemyData.enemyController.attack.targetPlayer = component;
								if (NetworkGameManager.Instance.isServer)
								{
									enemyData.enemyController.attack.EventSpecialAttack1Effect.Invoke();
								}
								else
								{
									enemyData.enemyController.network.networkPhoton.RPCSetEntanglePlayer(component.network.GetIDX());
								}
							}
						}
						else
						{
							component.network.ExecHurtEffect(component.network.GetIDX());
							float num = enemyData.damage * component.PlayerMultiplyStatsData.GetMultiplyDamageReduction();
							if (enemyData.enemyController.allSpriteParts[0].color.a == 0f)
							{
								enemyData.enemyController.VisibleSprite();
							}
							if (component.isEntangled)
							{
								component.network.AddSubHealth(-Mathf.RoundToInt(num / 4f));
							}
							else
							{
								component.network.AddSubHealth(0f - num);
							}
							if (!_noShake)
							{
								CameraGame.Instance.CameraShake();
							}
						}
					}
				}
			}
		}
		if (isPlayer && other.CompareTag("BreakableObject"))
		{
			ObjectCollisionBullet breakObj = other.GetComponent<ObjectCollisionBullet>();
			if (!breakObj.IsCollisionDelayRunning)
			{
				breakObj.IsCollisionDelayRunning = true;
				UniTaskUtil.DelayedCall(breakObj, 0.1f, () =>
				{
					breakObj.IsCollisionDelayRunning = false;
				}).Forget();
				float num2 = ((weaponController.isHalfMeleeCharging && !weaponController.isMeleeCharging) ? weaponController.dmgWeaponHalfCharge0 : ((!weaponController.isMeleeCharging) ? weaponController.dmgWeapon0 : weaponController.dmgWeaponFullCharge0));
				if (num2 == 0f)
				{
					num2 = 1f;
				}
				weaponController.CheckDamageToBreakableObject(breakObj, isRange: false, default, num2);
			}
		}
		if (isPlayer && other.CompareTag("Dummy"))
		{
			if (dummyTarget == null || dummyTarget.MyCollider != other)
			{
				dummyTarget = other.GetComponent<DummyController>();
			}
			Vector3 direction = new Vector3(dummyTarget.transform.position.x, 0f, dummyTarget.transform.position.z) - new Vector3(weaponController.playerController.weaponPos.position.x, 0f, weaponController.playerController.weaponPos.position.z);
			dummyTarget.GetHit(direction, weaponController.isMeleeCharging, weaponController.idWeaponMelee);
		}
	}
}
