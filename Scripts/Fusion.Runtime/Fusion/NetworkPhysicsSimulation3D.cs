#define DEBUG
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[OrderAfter(new Type[]
{
	typeof(NetworkRigidbody),
	typeof(NetworkRigidbodyObsolete)
})]
[DisallowMultipleComponent]
[SimulationBehaviour(Stages = (SimulationStages.Forward | SimulationStages.Resimulate))]
public class NetworkPhysicsSimulation3D : SimulationBehaviour, IBeforeTick
{
	private static bool _physicsAutoSimulationRestore;

	private static int _enabledRunnersCount;

	private static bool _pendingSyncTransforms;

	private bool _syncTransformsRequested;

	public virtual float PhysicsSimulationDeltaTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Runner.Simulation.DeltaTime;
		}
	}

	private void OnEnable()
	{
		if (_enabledRunnersCount == 0)
		{
			_physicsAutoSimulationRestore = Physics.autoSimulation;
		}
		Physics.autoSimulation = false;
		_enabledRunnersCount++;
	}

	internal void RequestPhysicsSyncTransform()
	{
		_pendingSyncTransforms = true;
		_syncTransformsRequested = true;
	}

	public override void FixedUpdateNetwork()
	{
		Assert.Check(Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics3D);
		if (Runner.Config.ServerPhysicsMode != NetworkProjectConfig.PhysicsModes.ServerOnly || Runner.Simulation.Stage != SimulationStages.Resimulate)
		{
			Simulate();
		}
	}

	public void Simulate()
	{
		if (Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			Assert.Check(Runner.SimulationUnityScene.IsValid(), $"{Runner} invalid scene");
			PhysicsScene physicsScene = Runner.SimulationUnityScene.GetPhysicsScene();
			if (physicsScene.IsValid())
			{
				Runner.InvokeOnBeforePhysicsStep();
				physicsScene.Simulate(PhysicsSimulationDeltaTime);
				Runner.InvokeOnAfterPhysicsStep();
			}
		}
		else
		{
			Runner.InvokeOnBeforePhysicsStep();
			Physics.Simulate(PhysicsSimulationDeltaTime);
			Runner.InvokeOnAfterPhysicsStep();
		}
	}

	void IBeforeTick.BeforeTick()
	{
		if (_pendingSyncTransforms)
		{
			Physics.SyncTransforms();
			_pendingSyncTransforms = false;
		}
		if (_syncTransformsRequested)
		{
			Runner.InvokeOnAfterPhysicsSyncTransforms3D();
			_syncTransformsRequested = false;
		}
	}

	private void OnDisable()
	{
		_enabledRunnersCount--;
		if (_enabledRunnersCount == 0)
		{
			Physics.autoSimulation = _physicsAutoSimulationRestore;
		}
	}
}
