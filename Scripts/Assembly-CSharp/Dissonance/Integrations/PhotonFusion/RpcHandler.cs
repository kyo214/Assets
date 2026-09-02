using System;
using Dissonance.Extensions;
using Fusion;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

namespace Dissonance.Integrations.PhotonFusion;

internal class RpcHandler : SimulationBehaviour
{
	private static FusionCommsNetwork _instance;

	private static NetworkRunner _runner;

	public static void Initialize(FusionCommsNetwork instance, NetworkRunner runner)
	{
		_instance = instance;
		_runner = runner;
	}

	[NotNull]
	private static T[] ConvertToArray<T>(ArraySegment<T> segment) where T : struct
	{
		T[] array = new T[segment.Count];
		segment.CopyToSegment(array);
		return array;
	}

	public static void SendToServer(FusionCommsNetwork instance, ArraySegment<byte> packet, bool reliable)
	{
		if (_instance != instance)
		{
			throw new InvalidOperationException("Cannot send from mismatched instance");
		}
		if (reliable)
		{
			SendToServerReliableRPC(_runner, ConvertToArray(packet));
		}
		else
		{
			SendToServerUnreliableRPC(_runner, ConvertToArray(packet));
		}
	}

	public static bool SendToClient(FusionCommsNetwork instance, FusionPeer dest, ArraySegment<byte> packet, bool reliable)
	{
		if (_instance != instance)
		{
			throw new InvalidOperationException("Cannot send from mismatched instance");
		}
		if (_runner.GetRpcTargetStatus(dest.PlayerRef) == RpcTargetStatus.Unreachable)
		{
			return false;
		}
		if (reliable)
		{
			SendToClientReliableRPC(_runner, ConvertToArray(packet), dest.PlayerRef);
		}
		else
		{
			SendToClientUnreliableRPC(_runner, ConvertToArray(packet), dest.PlayerRef);
		}
		return true;
	}

	[Rpc(InvokeLocal = false, InvokeResim = false, TickAligned = false, Channel = RpcChannel.Reliable, HostMode = RpcHostMode.SourceIsHostPlayer)]
	private unsafe static void SendToServerReliableRPC(NetworkRunner runner, byte[] data, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
			if (_instance == null)
			{
				Debug.LogWarning("Received Dissonance message, but there is no Dissonance comms network");
			}
			else
			{
				_instance.DeliverMessageToServer(data, info.Source);
			}
			return;
		}
		if ((object)runner == null)
		{
			throw new ArgumentNullException("runner");
		}
		if (runner.Stage != SimulationStages.Resimulate)
		{
			int num = 8;
			num += (data.Length * 1 + 4 + 3) & -4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToServerReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.RpcInfo)", num);
			}
			else if (runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data2 = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToServerReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.RpcInfo)")), data2);
				*(int*)(data2 + num2) = data.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data2 + num2, data) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				ptr->SetNotTickAligned();
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
		}
	}

	[Rpc(InvokeLocal = false, InvokeResim = false, TickAligned = false, Channel = RpcChannel.Unreliable, HostMode = RpcHostMode.SourceIsHostPlayer)]
	private unsafe static void SendToServerUnreliableRPC(NetworkRunner runner, byte[] data, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
			if (_instance == null)
			{
				Debug.LogWarning("Received Dissonance message, but there is no Dissonance comms network");
			}
			else
			{
				_instance.DeliverMessageToServer(data, info.Source);
			}
			return;
		}
		if ((object)runner == null)
		{
			throw new ArgumentNullException("runner");
		}
		if (runner.Stage != SimulationStages.Resimulate)
		{
			int num = 8;
			num += (data.Length * 1 + 4 + 3) & -4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToServerUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.RpcInfo)", num);
			}
			else if (runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data2 = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToServerUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.RpcInfo)")), data2);
				*(int*)(data2 + num2) = data.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray(data2 + num2, data) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				ptr->SetUnreliable();
				ptr->SetNotTickAligned();
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
		}
	}

	[Rpc(InvokeLocal = false, InvokeResim = false, TickAligned = false, Channel = RpcChannel.Reliable, HostMode = RpcHostMode.SourceIsHostPlayer)]
	private unsafe static void SendToClientReliableRPC(NetworkRunner runner, byte[] data, [RpcTarget] PlayerRef player)
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
			if (_instance == null)
			{
				Debug.LogWarning("Received Dissonance message, but there is no Dissonance comms network");
			}
			else
			{
				_instance.DeliverMessageToClient(data);
			}
			return;
		}
		if ((object)runner == null)
		{
			throw new ArgumentNullException("runner");
		}
		if (runner.Stage == SimulationStages.Resimulate)
		{
			return;
		}
		switch (runner.GetRpcTargetStatus(player))
		{
		case RpcTargetStatus.Unreachable:
			NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)");
			return;
		case RpcTargetStatus.Self:
			NetworkBehaviourUtils.NotifyLocalTargetedRpcCulled(player, "System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)");
			return;
		}
		int num = 8;
		num += (data.Length * 1 + 4 + 3) & -4;
		if (!SimulationMessage.CanAllocateUserPayload(num))
		{
			NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)", num);
			return;
		}
		SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
		byte* data2 = SimulationMessage.GetData(ptr);
		int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)")), data2);
		*(int*)(data2 + num2) = data.Length;
		num2 += 4;
		num2 = ((Native.CopyFromArray(data2 + num2, data) + 3) & -4) + num2;
		ptr->Offset = num2 * 8;
		ptr->SetTarget(player);
		ptr->SetNotTickAligned();
		ptr->SetStatic();
		runner.SendRpc(ptr);
	}

	[Rpc(InvokeLocal = false, InvokeResim = false, TickAligned = false, Channel = RpcChannel.Unreliable, HostMode = RpcHostMode.SourceIsHostPlayer)]
	private unsafe static void SendToClientUnreliableRPC(NetworkRunner runner, byte[] data, [RpcTarget] PlayerRef player)
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
			if (_instance == null)
			{
				Debug.LogWarning("Received Dissonance message, but there is no Dissonance comms network");
			}
			else
			{
				_instance.DeliverMessageToClient(data);
			}
			return;
		}
		if ((object)runner == null)
		{
			throw new ArgumentNullException("runner");
		}
		if (runner.Stage == SimulationStages.Resimulate)
		{
			return;
		}
		switch (runner.GetRpcTargetStatus(player))
		{
		case RpcTargetStatus.Unreachable:
			NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)");
			return;
		case RpcTargetStatus.Self:
			NetworkBehaviourUtils.NotifyLocalTargetedRpcCulled(player, "System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)");
			return;
		}
		int num = 8;
		num += (data.Length * 1 + 4 + 3) & -4;
		if (!SimulationMessage.CanAllocateUserPayload(num))
		{
			NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)", num);
			return;
		}
		SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
		byte* data2 = SimulationMessage.GetData(ptr);
		int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)")), data2);
		*(int*)(data2 + num2) = data.Length;
		num2 += 4;
		num2 = ((Native.CopyFromArray(data2 + num2, data) + 3) & -4) + num2;
		ptr->Offset = num2 * 8;
		ptr->SetTarget(player);
		ptr->SetUnreliable();
		ptr->SetNotTickAligned();
		ptr->SetStatic();
		runner.SendRpc(ptr);
	}

	[NetworkRpcStaticWeavedInvoker("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToServerReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	protected unsafe static void SendToServerReliableRPC_0040Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsHostPlayer);
		NetworkBehaviourUtils.InvokeRpc = true;
		SendToServerReliableRPC(runner, array, info);
	}

	[NetworkRpcStaticWeavedInvoker("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToServerUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	protected unsafe static void SendToServerUnreliableRPC_0040Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsHostPlayer);
		NetworkBehaviourUtils.InvokeRpc = true;
		SendToServerUnreliableRPC(runner, array, info);
	}

	[NetworkRpcStaticWeavedInvoker("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientReliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)")]
	[Preserve]
	protected unsafe static void SendToClientReliableRPC_0040Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		PlayerRef target = message->Target;
		NetworkBehaviourUtils.InvokeRpc = true;
		SendToClientReliableRPC(runner, array, target);
	}

	[NetworkRpcStaticWeavedInvoker("System.Void Dissonance.Integrations.PhotonFusion.RpcHandler::SendToClientUnreliableRPC(Fusion.NetworkRunner,System.Byte[],Fusion.PlayerRef)")]
	[Preserve]
	protected unsafe static void SendToClientUnreliableRPC_0040Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray(array, data + num) + 3) & -4) + num;
		PlayerRef target = message->Target;
		NetworkBehaviourUtils.InvokeRpc = true;
		SendToClientUnreliableRPC(runner, array, target);
	}
}
