using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using Toked;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(0)]
public class HollowBabyAttackBehaviour : NetworkBehaviour
{
	private EnemyController enemyController;

	private EnemyAttack enemyAttack;

	private static Changed<HollowBabyAttackBehaviour> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<HollowBabyAttackBehaviour> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<HollowBabyAttackBehaviour> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private void Start()
	{
		enemyController = base.transform.parent.GetComponent<EnemyController>();
		enemyAttack = base.transform.parent.GetComponent<EnemyAttack>();
	}

	public void Attack()
	{
		if (!(enemyController.network.GetHealth() > 0f))
		{
			return;
		}
		PlayerController playerCont = NetworkGameManager.Instance.GetPlayerNearest(isHaveHealth: true, enemyController.transform.position);
		if (enemyAttack.targetChasing == null && playerCont != null)
		{
			playerCont.targetedPoint.position = new Vector3(playerCont.targetedPoint.position.x, enemyController.transform.position.y, playerCont.targetedPoint.position.z);
			enemyAttack.targetChasing = playerCont.targetedPoint;
		}
		enemyController.network.SetDoSpesialAttack(value: true);
		enemyController.isAttacking = true;
		enemyController.RVOController.enabled = false;
		if (enemyAttack.targetChasing != null)
		{
			enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
			enemyController.network.SetAngleDirection(enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim));
		}
		enemyController.network.SetAnimation("StartAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		enemyController.SetAISpeed(0f);
		UniTaskUtil.DelayedCall(this, Random.Range(0.1f, 0.2f), () =>
		{
			if (enemyController.network.GetHealth() > 0f)
			{
				if (playerCont != null)
				{
					playerCont.targetedPoint.position = new Vector3(playerCont.targetedPoint.position.x, enemyController.transform.position.y, playerCont.targetedPoint.position.z);
					enemyAttack.targetChasing = playerCont.targetedPoint;
				}
				enemyAttack.SetAttackTarget();
				RpcJumping(enemyAttack.targetChasing.position);
			}
		}).Forget();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	private unsafe void RpcJumping(Vector3 targetPosPlayer)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void HollowBabyAttackBehaviour::RpcJumping(UnityEngine.Vector3)", Object, 7);
				return;
			}
			int num = 8;
			num += 12;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void HollowBabyAttackBehaviour::RpcJumping(UnityEngine.Vector3)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, targetPosPlayer);
				num2 += 12;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		Vector3 targetPosPlayer2 = targetPosPlayer;
		if (!(enemyController.network.GetHealth() > 0f))
		{
			return;
		}
		if (enemyAttack.targetChasing != null)
		{
			enemyController.movement.angleAnim = enemyAttack.AngleEnemy((targetPosPlayer2 - base.transform.position).normalized, enemyController.movement.angleAnim);
		}
		if (NetworkGameManager.Instance.isServer)
		{
			enemyController.network.SetAnimation("Special1-" + enemyController.movement.angleAnim);
		}
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			if (enemyController.network.GetHealth() > 0f)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					Vector3 normalized = (targetPosPlayer2 - enemyController.transform.position).normalized;
					float maxDistance = MathFunc.Distance(enemyController.transform.position, enemyController.targetObj.transform.position) + 1.5f;
					Vector3 endValue = targetPosPlayer2 + normalized * 1.1f;
					if (Physics.Raycast(enemyController.bodyTransform.position, normalized, maxDistance, enemyController.layerWallCollider))
					{
						maxDistance = MathFunc.Distance(enemyController.transform.position, targetPosPlayer2);
						endValue = ((!Physics.Raycast(enemyController.bodyTransform.position, normalized, maxDistance, enemyController.layerWallCollider)) ? targetPosPlayer2 : enemyController.transform.position);
					}
					enemyController.isDown = true;
					enemyController.transform.DOMove(endValue, 0.5f);
				}
				else
				{
					Vector3 normalized2 = (targetPosPlayer2 - enemyController.transform.position).normalized;
					float maxDistance2 = MathFunc.Distance(enemyController.transform.position, targetPosPlayer2) + 1.5f;
					Vector3 endValue2 = targetPosPlayer2 + normalized2 * 1.1f;
					if (Physics.Raycast(enemyController.bodyTransform.position, normalized2, maxDistance2, enemyController.layerWallCollider))
					{
						maxDistance2 = MathFunc.Distance(enemyController.transform.position, targetPosPlayer2);
						endValue2 = ((!Physics.Raycast(enemyController.bodyTransform.position, normalized2, maxDistance2, enemyController.layerWallCollider)) ? targetPosPlayer2 : enemyController.transform.position);
					}
					enemyController.isDown = true;
					enemyController.object2D.transform.DOMove(endValue2, 0.5f);
				}
			}
		}).Forget();
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
	}

	public override void CopyStateToBackingFields()
	{
	}

	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	protected unsafe static void RpcJumping_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 targetPosPlayer = vector;
		behaviour.InvokeRpc = true;
		((HollowBabyAttackBehaviour)behaviour).RpcJumping(targetPosPlayer);
	}
}
