#define DEBUG
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[OrderAfter(new Type[]
{
	typeof(NetworkRigidbody2D),
	typeof(NetworkRigidbodyObsolete2D)
})]
[DisallowMultipleComponent]
public class NetworkPhysicsSimulation2D : SimulationBehaviour, IBeforeTick
{
	private static SimulationMode2D _physicsSimulationModeRestore;

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
			_physicsSimulationModeRestore = Physics2D.simulationMode;
		}
		Physics2D.simulationMode = SimulationMode2D.Script;
		_enabledRunnersCount++;
	}

	internal void RequestPhysicsSyncTransform()
	{
		_pendingSyncTransforms = true;
		_syncTransformsRequested = true;
	}

	public override void FixedUpdateNetwork()
	{
		Assert.Check(Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics2D);
		if (Runner.Config.ServerPhysicsMode != NetworkProjectConfig.PhysicsModes.ServerOnly || Runner.Simulation.Stage != SimulationStages.Resimulate)
		{
			Simulate();
		}
	}

	public void Simulate()
	{
		Runner.InvokeOnBeforePhysicsStep();
		if (Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			Assert.Check(Runner.SimulationUnityScene.IsValid());
			PhysicsScene2D physicsScene2D = Runner.SimulationUnityScene.GetPhysicsScene2D();
			if (physicsScene2D.IsValid())
			{
				physicsScene2D.Simulate(PhysicsSimulationDeltaTime);
			}
		}
		else
		{
			Physics2D.Simulate(PhysicsSimulationDeltaTime);
		}
		Runner.InvokeOnAfterPhysicsStep();
	}

	void IBeforeTick.BeforeTick()
	{
		if (_pendingSyncTransforms)
		{
			Physics2D.SyncTransforms();
			_pendingSyncTransforms = false;
		}
		if (_syncTransformsRequested)
		{
			Runner.InvokeOnAfterPhysicsSyncTransforms2D();
			_syncTransformsRequested = false;
		}
	}

	private void OnDisable()
	{
		_enabledRunnersCount--;
		if (_enabledRunnersCount == 0)
		{
			Physics2D.simulationMode = _physicsSimulationModeRestore;
		}
	}
}
