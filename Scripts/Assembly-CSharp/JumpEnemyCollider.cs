using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DestroyIt;
using Toked;
using UnityEngine;

public class JumpEnemyCollider : MonoBehaviour
{
	public ItemInteractable barricade;

	[SerializeField]
	private Destructible glass;

	[SerializeField]
	private bool isGlassBroken;

	[SerializeField]
	private bool _isColliderJumpSpawingEnemy = true;

	[SerializeField]
	private List<Collider> _listColliderToPlayer = new List<Collider>();

	public GameObject ObstaclePath;

	public Transform _targetJump;

	private void Start()
	{
		Object.Destroy(GetComponent<MeshRenderer>());
		Object.Destroy(GetComponent<MeshFilter>());
		if (_listColliderToPlayer.Count > 0)
		{
			foreach (Collider item2 in _listColliderToPlayer.ToList())
			{
				if (item2 == null)
				{
					_listColliderToPlayer.Remove(item2);
				}
			}
			int count = _listColliderToPlayer.Count;
			for (int i = 0; i < count; i++)
			{
				Collider[] componentsInChildren = _listColliderToPlayer[i].gameObject.GetComponentsInChildren<Collider>();
				foreach (Collider item in componentsInChildren)
				{
					_listColliderToPlayer.Add(item);
				}
			}
			for (int k = 0; k < count; k++)
			{
				_listColliderToPlayer.RemoveAt(0);
			}
		}
		if (barricade != null)
		{
			barricade.JumpCollider = this;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("GlassWindow") && !isGlassBroken)
		{
			glass = other.GetComponent<Destructible>();
		}
		if (other.CompareTag("Enemy") && NetworkGameManager.Instance.isServer)
		{
			bool flag = false;
			EnemyController enemyController = other.GetComponent<EnemyController>();
			Vector3 vector = Vector3.zero;
			if ((bool)_targetJump)
			{
				vector = (_targetJump.position - base.transform.position).normalized * 2.5f;
			}
			else if ((bool)enemyController.LastPosEnemy && enemyController.LastPosEnemy.posEnter.Count > 0)
			{
				vector = (enemyController.LastPosEnemy.posEnter[0].position - enemyController.LastPosEnemy.transform.position).normalized * 2.5f;
			}
			if (barricade != null && barricade.Hp > 0)
			{
				flag = true;
				enemyController.barricadeCollider = this;
				enemyController.timerAttackBarricade.StartDuration(Random.Range(1, 2));
				enemyController.SetAISpeed(0f);
				enemyController.dirJump = vector;
			}
			if (enemyController.network.networkPhoton.doSpecialAttack1 && enemyController.data.type == 100)
			{
				enemyController.SetAISpeed(0f);
			}
			else if (!enemyController.network.GetIsJumping() && !flag && !enemyController.network.IsNonActive() && enemyController.network.GetHealth() > 0f)
			{
				bool flag2 = false;
				if (_isColliderJumpSpawingEnemy)
				{
					flag2 = true;
				}
				else if (!_isColliderJumpSpawingEnemy && enemyController.attack.targetChasing != null)
				{
					Bounds bounds = new Bounds(enemyController.attack.targetChasing.position, new Vector3(0.2f, 0.2f, 0.2f));
					bool flag3 = false;
					foreach (Collider item in _listColliderToPlayer)
					{
						if (item.bounds.Intersects(bounds))
						{
							flag3 = true;
							break;
						}
					}
					if (flag3)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					if (!enemyController.isElite)
					{
						AudioManager.PlaySFXTransform("enemy0-jump", enemyController.transform, isLocalPlayerTrigger: false);
					}
					enemyController.VisibleSprite();
					enemyController.network.SetIsJumping(value: true);
					if (NetworkGameManager.Instance.isServer)
					{
						enemyController.network.networkPhoton.isMoveToJump = false;
					}
					enemyController.enemyCollider.enabled = false;
					enemyController.stoperCollider.enabled = false;
					enemyController.isJumping = true;
					enemyController.movement.angleAnim = enemyController.attack.AngleEnemy((other.transform.position + vector - other.transform.position).normalized, enemyController.movement.angleAnim);
					if (enemyController.isElite)
					{
						enemyController.network.SetAnimation("Jump" + enemyController.movement.angleAnim);
					}
					else
					{
						enemyController.network.SetAnimation("Jump" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					}
					enemyController.myrigidbody.DOKill();
					enemyController.transform.DOKill();
					enemyController.myrigidbody.isKinematic = true;
					if (!_isColliderJumpSpawingEnemy)
					{
						_ = enemyController.transform.position + enemyController.aiPath.desiredVelocity.normalized * 2f;
						enemyController.transform.DOJump(enemyController.transform.position + enemyController.aiPath.desiredVelocity.normalized * 2f, 1.2f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
						{
							enemyController.movement.StartMove().Forget();
						});
					}
					else
					{
						Vector3 targetJump = enemyController.transform.position + vector;
						enemyController.transform.DOJump(enemyController.transform.position + vector, 1.2f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
						{
							if (Vector3.Distance(enemyController.myrigidbody.position, targetJump) > 0.1f)
							{
								enemyController.transform.position = targetJump;
							}
							enemyController.movement.StartMove().Forget();
						});
					}
					enemyController.SetEnableAI(value: false);
					enemyController.bodyCollider.enabled = false;
				}
			}
		}
		if (glass != null)
		{
			glass.ApplyDamage(1f);
			AudioManager.PlaySFXTransform("smashed-glass", base.transform, isLocalPlayerTrigger: false);
			isGlassBroken = true;
			glass = null;
		}
	}
}
