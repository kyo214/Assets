using System;
using DG.Tweening;
using Fusion;
using Toked;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(1)]
public class DestructibleObject : NetworkBehaviour
{
	[SerializeField]
	private string _objectTypeName;

	public Collider colliderObject;

	public GameObject normalObject;

	public GameObject destroyedObject;

	public ObjectCollisionBullet ObjectCollision;

	public byte HitByPlayerID;

	public SO_MissionObjective missionObjective;

	public EventOnDestroyDestructible EventOnDestroy;

	[SerializeField]
	[DefaultForProperty("isDestroyed", 0, 1)]
	private bool _isDestroyed;

	private static Changed<DestructibleObject> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<DestructibleObject> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<DestructibleObject> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[Networked(OnChanged = "OnDestroyObject")]
	[NetworkedWeaved(0, 1)]
	public unsafe bool isDestroyed
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing DestructibleObject.isDestroyed. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean((int*)((byte*)Ptr + 0));
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing DestructibleObject.isDestroyed. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean((int*)((byte*)Ptr + 0), value);
		}
	}

	public override void Spawned()
	{
		if (missionObjective != null && NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && missionObjective.Code != GameManagerPhoton.Instance.CurrentMission.MissionObjective.Code)
		{
			Runner.Despawn(Object);
		}
	}

	private void Start()
	{
		GameManager.Instance.arrDestructibleObject.Add(this);
		if (colliderObject == null)
		{
			colliderObject = ObjectCollision.ObjectCollider;
		}
	}

	[Preserve]
	private static void OnDestroyObject(Changed<DestructibleObject> changed)
	{
		if (NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance.CurrentMission && changed.Behaviour._objectTypeName == GameManagerPhoton.Instance.CurrentMission.MissionObjective.TargetType && GameManagerPhoton.Instance.TargetDestroyed < GameManagerPhoton.Instance.CurrentMission.MissionObjective.MinTargetDestroy)
		{
			GameManagerPhoton.Instance.TargetDestroyed++;
		}
		changed.Behaviour.EventOnDestroy?.Invoke(changed.Behaviour);
		GameManager.Instance.DestroyObjectGame(changed.Behaviour.ObjectCollision, changed.Behaviour.HitByPlayerID);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCSetExplode(byte playerID)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void DestructibleObject::RPCSetExplode(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void DestructibleObject::RPCSetExplode(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				data[num2] = playerID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		HitByPlayerID = playerID;
		isDestroyed = true;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCHitObject(byte playerID, int damage)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void DestructibleObject::RPCHitObject(System.Byte,System.Int32)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void DestructibleObject::RPCHitObject(System.Byte,System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
				data[num2] = playerID;
				num2 += 4 & -4;
				*(int*)(data + num2) = damage;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ObjectCollision.HitDestructibleObject(damage, NetworkGameManager.Instance.GetPlayer(playerID));
		DestructibleObject destructObject = ObjectCollision.destructObject;
		if (destructObject != null && ObjectCollision.isDisabled)
		{
			if (destructObject.normalObject != null)
			{
				destructObject.normalObject.SetActive(value: false);
			}
			if (destructObject.destroyedObject != null)
			{
				destructObject.destroyedObject.SetActive(value: true);
			}
			if (NetworkGameManager.Instance.isServer)
			{
				destructObject.isDestroyed = true;
			}
			else
			{
				destructObject.RPCSetExplode(playerID);
			}
		}
		if (ObjectCollision.SFXName != "")
		{
			AudioManager.PlaySFXTransform(ObjectCollision.SFXName, ObjectCollision.transform, isLocalPlayerTrigger: false);
		}
		if (ObjectCollision.isShaking)
		{
			ObjectCollision.transform.DOShakeRotation(0.3f, 10f);
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		isDestroyed = _isDestroyed;
	}

	public override void CopyStateToBackingFields()
	{
		_isDestroyed = isDestroyed;
	}

	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	protected unsafe static void RPCSetExplode_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte playerID = num2;
		behaviour.InvokeRpc = true;
		((DestructibleObject)behaviour).RPCSetExplode(playerID);
	}

	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	protected unsafe static void RPCHitObject_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte playerID = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		int damage = num3;
		behaviour.InvokeRpc = true;
		((DestructibleObject)behaviour).RPCHitObject(playerID, damage);
	}
}
