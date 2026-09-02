using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Fusion;

internal static class NetworkRunnerUpdater
{
	private enum AddMode
	{
		FirstChild = 0,
		LastChild = 1,
		Before = 2,
		After = 3
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct NetworkRunnerUpdate
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct NetworkRunnerRender
	{
	}

	private static bool AddToPlayerLoop(ref PlayerLoopSystem parentSystem, Type referenceSystemType, AddMode addMode, Type ownerType, PlayerLoopSystem.UpdateFunction updateDelegate)
	{
		ref PlayerLoopSystem[] subSystemList = ref parentSystem.subSystemList;
		PlayerLoopSystem[] obj = subSystemList;
		int num = ((obj != null) ? obj.Length : 0);
		if (parentSystem.type == referenceSystemType)
		{
			switch (addMode)
			{
			case AddMode.FirstChild:
				InsertSystem(ref subSystemList, 0, ownerType, updateDelegate);
				break;
			case AddMode.LastChild:
				InsertSystem(ref subSystemList, num, ownerType, updateDelegate);
				break;
			default:
				throw new InvalidOperationException($"Unable to add with a mode {addMode} once a system has been entered");
			}
			return true;
		}
		for (int i = 0; i < num; i++)
		{
			PlayerLoopSystem playerLoopSystem = subSystemList[i];
			if (playerLoopSystem.type == referenceSystemType)
			{
				switch (addMode)
				{
				case AddMode.Before:
					InsertSystem(ref subSystemList, i, ownerType, updateDelegate);
					return true;
				case AddMode.After:
					InsertSystem(ref subSystemList, i + 1, ownerType, updateDelegate);
					return true;
				}
			}
			if (AddToPlayerLoop(ref subSystemList[i], referenceSystemType, addMode, ownerType, updateDelegate))
			{
				return true;
			}
		}
		return false;
	}

	private static bool RemoveFromPlayerLoop(ref PlayerLoopSystem parentSystem, Type type)
	{
		ref PlayerLoopSystem[] subSystemList = ref parentSystem.subSystemList;
		if (subSystemList == null)
		{
			return false;
		}
		for (int i = 0; i < subSystemList.Length; i++)
		{
			PlayerLoopSystem playerLoopSystem = subSystemList[i];
			if (playerLoopSystem.type == type)
			{
				for (int j = i + 1; j < subSystemList.Length; j++)
				{
					subSystemList[j - 1] = subSystemList[j];
				}
				Array.Resize(ref subSystemList, subSystemList.Length - 1);
				return true;
			}
			if (RemoveFromPlayerLoop(ref subSystemList[i], type))
			{
				return true;
			}
		}
		return false;
	}

	private static void InsertSystem(ref PlayerLoopSystem[] systems, int position, Type ownerType, PlayerLoopSystem.UpdateFunction updateDelegate)
	{
		PlayerLoopSystem[] obj = systems;
		int num = ((obj != null) ? obj.Length : 0);
		if (position < 0 || position > num)
		{
			throw new ArgumentOutOfRangeException("position");
		}
		PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
		{
			type = ownerType,
			updateDelegate = updateDelegate
		};
		Array.Resize(ref systems, num + 1);
		if (position < num)
		{
			Array.Copy(systems, position, systems, position + 1, systems.Length - position - 1);
		}
		systems[position] = playerLoopSystem;
	}

	private static void InvokeUpdate()
	{
		NetworkRunner.InvokeUpdate(Time.unscaledDeltaTime);
	}

	private static void InvokeRender()
	{
		NetworkRunner.InvokeRender();
	}

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		PlayerLoopSystem parentSystem = PlayerLoop.GetCurrentPlayerLoop();
		RemoveFromPlayerLoop(ref parentSystem, typeof(NetworkRunnerUpdate));
		RemoveFromPlayerLoop(ref parentSystem, typeof(NetworkRunnerRender));
		AddToPlayerLoop(ref parentSystem, typeof(Update.ScriptRunBehaviourUpdate), AddMode.Before, typeof(NetworkRunnerUpdate), InvokeUpdate);
		AddToPlayerLoop(ref parentSystem, typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), AddMode.Before, typeof(NetworkRunnerRender), InvokeRender);
		PlayerLoop.SetPlayerLoop(parentSystem);
	}
}
