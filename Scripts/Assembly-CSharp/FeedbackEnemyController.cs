using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;

public class FeedbackEnemyController : MonoBehaviour
{
	private static readonly int Brightness = Shader.PropertyToID("_Brightness");

	private static readonly int Tint = Shader.PropertyToID("_Tint");

	[SerializeField]
	private EnemyController enemy;

	public async UniTask Hurt(float stuntTime, byte animationType, float angleImpact, PlayerController player, bool isShowBlood = true, bool isForceKnockback = false)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		if (!(enemy.network.GetHealth() > 0f) || enemy.network.GetIsJumping())
		{
			return;
		}
		if (!enemy.isDead)
		{
			if (!enemy.isElite)
			{
				AudioManager.PlaySFXTransform("enemy0-hurt", enemy.middlePos, isLocalPlayerTrigger: false);
			}
			else
			{
				if (enemy.data.type == 100)
				{
					AudioManager.PlaySFXTransform("hairmaiden-hurt", enemy.middlePos, isLocalPlayerTrigger: false);
				}
				enemy.meleeHitAnim.gameObject.SetActive(value: true);
				enemy.meleeHitAnim.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
				if (!player.isAttackMeleeSwing || player.weaponController.isMeleeCharging || player.weaponController.isHalfMeleeCharging)
				{
					enemy.meleeHitAnim.Play("ChargeMeleeHit");
				}
				else
				{
					enemy.meleeHitAnim.Play("MeleeHit" + UnityEngine.Random.Range(1, 3));
				}
			}
			bool flag = false;
			if (animationType == 0 || animationType == 1 || (animationType == 2 && enemy.isElite) || animationType == 3)
			{
				if (!enemy.isElite && animationType != 3)
				{
					if (enemy.GetCurrentStateHash() == AnimatorHashManager.AttackingHash)
					{
						enemy.attack.timerTriggerAttack.PauseDuration();
						enemy.isAttacking = false;
					}
					else
					{
						if (!enemy.isDown)
						{
							if (NetworkGameManager.Instance.isServer)
							{
								enemy.network.SetAnimation("Hurt" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
							}
							else if (MathFunc.Distance(enemy.object2D.position, enemy.transform.position) <= 1f)
							{
								enemy.network.SetAnimation("Hurt" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
								UniTaskUtil.DelayedCall(this, 0.15f, () =>
								{
									enemy.AnimationHurtEnd();
								}).Forget();
							}
							else
							{
								enemy.AnimationHurtEnd();
								enemy.network.SetAnimation("Move" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
							}
						}
						enemy.isHurt = true;
						enemy.SetAISpeed(0.01f);
					}
				}
				if ((bool)enemy.headObj)
				{
					enemy.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
				}
				if (isShowBlood)
				{
					ObjectImpactPool objectImpactPool = ImpactSpawner.Instance.Get();
					objectImpactPool.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
					objectImpactPool.transform.position = new Vector3(enemy.animator.transform.position.x, objectImpactPool.transform.position.y, enemy.animator.transform.position.z);
					objectImpactPool.transform.localEulerAngles = new Vector3(0f, angleImpact, 0f);
					objectImpactPool.transform.parent = enemy.transform;
					if (player.isAttackMeleeSwing)
					{
						objectImpactPool.typeImpact = ObjectImpactPool.ImpactType.BloodOmni;
					}
					else
					{
						objectImpactPool.typeImpact = ObjectImpactPool.ImpactType.Blood;
					}
					objectImpactPool.initType();
				}
			}
			else if (animationType == 2)
			{
				if (!enemy.network.IsSpecialAttacking())
				{
					if (enemy.GetCurrentStateHash() == AnimatorHashManager.AttackingHash)
					{
						enemy.attack.timerTriggerAttack.PauseDuration();
					}
					enemy.isHurt = true;
				}
				if ((bool)enemy.headObj)
				{
					enemy.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
				}
				if (isShowBlood)
				{
					ObjectImpactPool objectImpactPool2 = ImpactSpawner.Instance.Get();
					objectImpactPool2.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
					objectImpactPool2.transform.localEulerAngles = new Vector3(0f, angleImpact, 0f);
					objectImpactPool2.transform.position = new Vector3(enemy.animator.transform.position.x, objectImpactPool2.transform.position.y, enemy.animator.transform.position.z);
					objectImpactPool2.transform.parent = enemy.transform;
					if (player.isAttackMeleeSwing)
					{
						objectImpactPool2.typeImpact = ObjectImpactPool.ImpactType.BloodOmni;
					}
					else
					{
						objectImpactPool2.typeImpact = ObjectImpactPool.ImpactType.Blood;
					}
					objectImpactPool2.initType();
				}
				Vector3 normalized = (player.transform.position - enemy.object2D.transform.position).normalized;
				normalized = new Vector3(normalized.x, 0f, normalized.z);
				if (!enemy.isElite)
				{
					enemy.isAttacking = false;
					if (!enemy.isDown)
					{
						enemy.movement.angleAnim = enemy.attack.AngleEnemy(normalized, enemy.movement.angleAnim);
					}
				}
				if ((player.weaponController.isMeleeCharging || player.weaponController.isOneHitKnockback) && !enemy.isElite && player.isAttackMeleeSwing && !isForceKnockback)
				{
					enemy.meleeHitAnim.gameObject.SetActive(value: true);
					enemy.meleeHitAnim.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
					enemy.meleeHitAnim.Play("MeleeHit" + UnityEngine.Random.Range(1, 3));
					if (!enemy.isDown)
					{
						enemy.network.SetAnimation("Dead2" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
						EnemyKnockDown();
					}
					enemy.meleeHitAnim.gameObject.SetActive(value: true);
					enemy.meleeHitAnim.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
					enemy.meleeHitAnim.Play("ChargeMeleeHit");
				}
				else if (!enemy.network.IsSpecialAttacking())
				{
					if (enemy.isElite)
					{
						enemy.network.SetAnimation("Hurt" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
					}
					else if (!enemy.isDown)
					{
						enemy.network.SetAnimation("Knock" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
					}
					enemy.SetAISpeed(0.01f);
					enemy.meleeHitAnim.gameObject.SetActive(value: true);
					enemy.meleeHitAnim.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
					if (player.weaponController.isHalfMeleeCharging)
					{
						enemy.meleeHitAnim.Play("ChargeMeleeHit" + UnityEngine.Random.Range(1, 3));
					}
					else
					{
						enemy.meleeHitAnim.Play("MeleeHit" + UnityEngine.Random.Range(1, 3));
					}
				}
				if (!enemy.isElite)
				{
					Vector3 position = enemy.bodyCollider.transform.position;
					if (!NetworkGameManager.Instance.isServer)
					{
						position = enemy.object2D.transform.position;
					}
					bool flag2 = Physics.Raycast(new Vector3(position.x, enemy.middlePos.position.y / 2f, position.z), -normalized, out var hitInfo, 1.2f, enemy.layerWallCollider);
					bool num = Physics.SphereCast(new Vector3(position.x, enemy.middlePos.position.y / 2f, position.z), enemy.bodyCollider.bounds.size.x * 0.5f, -normalized, out hitInfo, 1.2f, enemy.layerWallCollider);
					if (NetworkGameManager.Instance.isServer)
					{
						enemy.SetEnableAI(value: false);
					}
					if (!num && !flag2)
					{
						enemy.object2D.DOKill();
						enemy.object2D.transform.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0f);
						if (!enemy.isDown && player.network.isLocalPlayer)
						{
							GameManagerPhoton.Instance.RpcExecEnemyKnockback(enemy.network.GetIDX(), enemy.object2D.transform.position - normalized * enemy.data.knockBackDistanceMultiply);
							flag = true;
						}
					}
				}
			}
			if (NetworkGameManager.Instance.isServer && !flag)
			{
				GameManagerPhoton.Instance.RpcSetPosEnemy(enemy.network.GetIDX(), enemy.object2D.transform.position);
			}
			if (enemy.isFakeDead)
			{
				enemy.isFakeDead = false;
				enemy.timerStunt.StartDuration(0.1f);
			}
		}
		if (enemy.GetCurrentStateHash() == AnimatorHashManager.ChasingHash && enemy.aiTarget.target != null && !enemy.isElite && animationType != 3)
		{
			enemy.SetAISpeed(0.01f);
		}
		_ = enemy.aiPath.maxSpeed;
		_ = 0.01f;
		foreach (SpriteRenderer allSpritePart in enemy.allSpriteParts)
		{
			allSpritePart.GetPropertyBlock(enemy.MPB);
			enemy.MPB.SetFloat(Brightness, 0.7f);
			allSpritePart.SetPropertyBlock(enemy.MPB);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.029999999329447746), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		foreach (SpriteRenderer allSpritePart2 in enemy.allSpriteParts)
		{
			allSpritePart2.GetPropertyBlock(enemy.MPB);
			enemy.MPB.SetFloat(Brightness, -1.5f);
			enemy.MPB.SetColor(Tint, new Color(1.4f, 0f, 0f));
			allSpritePart2.SetPropertyBlock(enemy.MPB);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (enemy.attack.timerTriggerAttack.isPaused && !enemy.network.IsSpecialAttacking() && animationType != 3)
		{
			enemy.attack.timerTriggerAttack.ResumeDuration();
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		foreach (SpriteRenderer allSpritePart3 in enemy.allSpriteParts)
		{
			if (allSpritePart3.name != "effect" && allSpritePart3.name != "shadow")
			{
				allSpritePart3.GetPropertyBlock(enemy.MPB);
				enemy.MPB.SetFloat(Brightness, 0f);
				enemy.MPB.SetColor(Tint, new Color(0f, 0f, 0f));
				allSpritePart3.SetPropertyBlock(enemy.MPB);
			}
		}
	}

	public async UniTask HurtVFX()
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		enemy.meleeHitAnim.transform.position = enemy.hitPos[UnityEngine.Random.Range(0, enemy.hitPos.Count)].position;
		enemy.meleeHitAnim.gameObject.SetActive(value: true);
		enemy.meleeHitAnim.Play("MeleeHit" + UnityEngine.Random.Range(1, 3));
		if (!enemy.isElite)
		{
			AudioManager.PlaySFXTransform("enemy0-hurt", enemy.middlePos, isLocalPlayerTrigger: false);
		}
		else if (enemy.data.type == 100)
		{
			AudioManager.PlaySFXTransform("hairmaiden-hurt", enemy.middlePos, isLocalPlayerTrigger: false);
		}
		foreach (SpriteRenderer allSpritePart in enemy.allSpriteParts)
		{
			allSpritePart.GetPropertyBlock(enemy.MPB);
			enemy.MPB.SetFloat(Brightness, 0.7f);
			allSpritePart.SetPropertyBlock(enemy.MPB);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.029999999329447746), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		foreach (SpriteRenderer allSpritePart2 in enemy.allSpriteParts)
		{
			allSpritePart2.GetPropertyBlock(enemy.MPB);
			enemy.MPB.SetFloat(Brightness, -1.5f);
			enemy.MPB.SetColor(Tint, new Color(1.4f, 0f, 0f));
			allSpritePart2.SetPropertyBlock(enemy.MPB);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.15000000596046448), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		foreach (SpriteRenderer allSpritePart3 in enemy.allSpriteParts)
		{
			if (allSpritePart3.name != "effect" && allSpritePart3.name != "shadow")
			{
				allSpritePart3.GetPropertyBlock(enemy.MPB);
				enemy.MPB.SetFloat(Brightness, 0f);
				enemy.MPB.SetColor(Tint, new Color(0f, 0f, 0f));
				allSpritePart3.SetPropertyBlock(enemy.MPB);
			}
		}
	}

	public void EnemyKnockDown(bool isFromAttack = true)
	{
		enemy.shadow.color = new Color(enemy.shadow.color.r, enemy.shadow.color.r, enemy.shadow.color.b, 0f);
		enemy.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
		enemy.isDown = true;
		if (enemy.isFakeDead || !isFromAttack)
		{
			enemy.object2DSortingGroup.sortingLayerName = "Ground";
		}
		else if (NetworkGameManager.Instance.isServer)
		{
			enemy.timerStunt.StartDuration(UnityEngine.Random.Range(3.5f, 4.5f));
		}
		UniTaskUtil.DelayedCall(this, 0.2f, () =>
		{
			enemy.myrigidbody.isKinematic = true;
			enemy.bodyCollider.enabled = false;
		}).Forget();
		enemy.stoperCollider.enabled = false;
		enemy.enemyCollider.enabled = isFromAttack;
		foreach (SpriteRenderer item in enemy.eyeGlow)
		{
			item.enabled = false;
		}
	}
}
