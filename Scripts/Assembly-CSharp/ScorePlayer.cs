using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(12)]
public class ScorePlayer : NetworkBehaviour
{
	[SerializeField]
	[DefaultForProperty("_scoreDataPerMission", 0, 6)]
	private ScoreDataNetwork __scoreDataPerMission;

	[SerializeField]
	[DefaultForProperty("_scoreDataTotal", 6, 6)]
	private ScoreDataNetwork __scoreDataTotal;

	public int TotalScore;

	private static Changed<ScorePlayer> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<ScorePlayer> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<ScorePlayer> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[SerializeField]
	[Networked]
	[NetworkedWeaved(0, 6)]
	private unsafe ScoreDataNetwork _scoreDataPerMission
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ScorePlayer._scoreDataPerMission. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ScoreDataNetwork*)((byte*)Ptr + 0);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ScorePlayer._scoreDataPerMission. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ScoreDataNetwork*)((byte*)Ptr + 0) = value;
		}
	}

	public ScoreDataNetwork ScoreDataPerMission => _scoreDataPerMission;

	[SerializeField]
	[Networked]
	[NetworkedWeaved(6, 6)]
	private unsafe ScoreDataNetwork _scoreDataTotal
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ScorePlayer._scoreDataTotal. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ScoreDataNetwork*)(Ptr + 6);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ScorePlayer._scoreDataTotal. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ScoreDataNetwork*)(Ptr + 6) = value;
		}
	}

	public ScoreDataNetwork ScoreDataTotal
	{
		get
		{
			return _scoreDataTotal;
		}
		set
		{
			_scoreDataTotal = value;
		}
	}

	public void ResetAllScore()
	{
		_scoreDataPerMission = default;
		_scoreDataTotal = default;
	}

	public void ResetScorePerMission()
	{
		_scoreDataPerMission = default;
	}

	public void IncreaseKill(bool isElite)
	{
		ScoreDataNetwork scoreDataPerMission = _scoreDataPerMission;
		if (isElite)
		{
			scoreDataPerMission.KillEliteCount++;
		}
		else
		{
			scoreDataPerMission.KillZombieCount++;
		}
		_scoreDataPerMission = scoreDataPerMission;
	}

	public void IncreaseDead()
	{
		ScoreDataNetwork scoreDataPerMission = _scoreDataPerMission;
		scoreDataPerMission.DeathCount++;
		_scoreDataPerMission = scoreDataPerMission;
	}

	public void IncreasePuzzleSolved()
	{
		ScoreDataNetwork scoreDataPerMission = _scoreDataPerMission;
		scoreDataPerMission.PuzzleSolved++;
		_scoreDataPerMission = scoreDataPerMission;
	}

	internal void IncreaseReviveOther()
	{
		ScoreDataNetwork scoreDataPerMission = _scoreDataPerMission;
		scoreDataPerMission.ReviveOtherPlayer++;
		_scoreDataPerMission = scoreDataPerMission;
	}

	public void SetTotalScoreFromScoreMission()
	{
		ScoreDataNetwork scoreDataTotal = _scoreDataTotal;
		scoreDataTotal.KillZombieCount += _scoreDataPerMission.KillZombieCount;
		scoreDataTotal.KillEliteCount += _scoreDataPerMission.KillEliteCount;
		scoreDataTotal.PuzzleSolved += _scoreDataPerMission.PuzzleSolved;
		scoreDataTotal.DeathCount += _scoreDataPerMission.DeathCount;
		scoreDataTotal.ReviveOtherPlayer += _scoreDataPerMission.ReviveOtherPlayer;
		scoreDataTotal.Life = (byte)GameManagerPhoton.Instance.Life;
		_scoreDataTotal = scoreDataTotal;
	}

	public int GetTotalKillPerMission()
	{
		return _scoreDataPerMission.KillEliteCount + _scoreDataPerMission.KillZombieCount;
	}

	public int GetTotalKill()
	{
		return _scoreDataTotal.KillEliteCount + _scoreDataTotal.KillZombieCount;
	}

	public void SetTotalScore(ScoreDataNetwork scoreData)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			_scoreDataTotal = scoreData;
		}
		else if (Object.HasInputAuthority)
		{
			Rpc_SetTotalScore(scoreData);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcAddKillEnemy(bool isElite)
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void ScorePlayer::RpcAddKillEnemy(System.Boolean)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void ScorePlayer::RpcAddKillEnemy(System.Boolean)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isElite);
					num2 += 4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		IncreaseKill(isElite);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RpcAddDead()
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void ScorePlayer::RpcAddDead()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void ScorePlayer::RpcAddDead()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		IncreaseDead();
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void Rpc_SetTotalScore(ScoreDataNetwork scoreData)
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
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void ScorePlayer::Rpc_SetTotalScore(ScoreDataNetwork)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 24;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void ScorePlayer::Rpc_SetTotalScore(ScoreDataNetwork)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
					*(ScoreDataNetwork*)(data + num2) = scoreData;
					num2 += 24;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		SetTotalScore(scoreData);
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		_scoreDataPerMission = __scoreDataPerMission;
		_scoreDataTotal = __scoreDataTotal;
	}

	public override void CopyStateToBackingFields()
	{
		__scoreDataPerMission = _scoreDataPerMission;
		__scoreDataTotal = _scoreDataTotal;
	}

	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	protected unsafe static void RpcAddKillEnemy_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		bool num2 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isElite = num2;
		behaviour.InvokeRpc = true;
		((ScorePlayer)behaviour).RpcAddKillEnemy(isElite);
	}

	[NetworkRpcWeavedInvoker(2, 7, 1)]
	[Preserve]
	protected unsafe static void RpcAddDead_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((ScorePlayer)behaviour).RpcAddDead();
	}

	[NetworkRpcWeavedInvoker(3, 2, 1)]
	[Preserve]
	protected unsafe static void Rpc_SetTotalScore_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		ScoreDataNetwork scoreDataNetwork = *(ScoreDataNetwork*)(data + num);
		num += 24;
		ScoreDataNetwork scoreData = scoreDataNetwork;
		behaviour.InvokeRpc = true;
		((ScorePlayer)behaviour).Rpc_SetTotalScore(scoreData);
	}
}
