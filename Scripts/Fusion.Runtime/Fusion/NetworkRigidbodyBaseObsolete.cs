#define DEBUG
using System;
using UnityEngine;

namespace Fusion;

[OrderAfter(new Type[] { typeof(NetworkCharacterController) })]
[Obsolete("This class has been replaced by a new NetworkRigidbodyBase class and is now obsolete.")]
public abstract class NetworkRigidbodyBaseObsolete : NetworkTransformObsolete, IBeforeCopyPreviousState
{
	internal abstract class Implementation
	{
		public NetworkRigidbodyBaseObsolete Nrb;

		public abstract void Spawned();

		public abstract void Render();

		public abstract void CopyBackingFieldsToState();

		public abstract void BeforeCopyPreviousState();

		public abstract void BeforeAllTicks(bool resimulation);

		public abstract void AfterAllTicks(bool resimulation);

		public abstract void StateAuthorityChanged();

		public abstract void RemotePrefabCreated();
	}

	internal class ClientSidePhysicsPrediction : Implementation
	{
		public override void Spawned()
		{
		}

		public override void Render()
		{
			if (Nrb.InterpolationDataSource != InterpolationDataSources.NoInterpolation)
			{
				bool flag = Nrb.InterpolationDataSource != InterpolationDataSources.Snapshots;
				if (Nrb.GetInterpolationData(out var data, flag))
				{
					Nrb.InterpolateTransform(ref data, flag);
				}
			}
		}

		public override void CopyBackingFieldsToState()
		{
			Assert.Check(Nrb.Object.HasStateAuthority);
			Nrb.CopyEngine2Buffers();
		}

		public override void RemotePrefabCreated()
		{
			Nrb.CopyBuffers2Engine(posRotOnly: true);
		}

		public override void BeforeCopyPreviousState()
		{
			if (!Nrb.Object.HasStateAuthority)
			{
				Nrb.CopyEngine2Buffers(posRotOnly: true);
			}
		}

		public override void BeforeAllTicks(bool resimulation)
		{
			if (resimulation)
			{
				Nrb.CopyBuffers2Engine();
			}
		}

		public override void AfterAllTicks(bool resimulation)
		{
			Nrb.CopyEngine2Buffers(!Nrb.Object.HasStateAuthority);
		}

		public override void StateAuthorityChanged()
		{
		}
	}

	internal class ServerPhysicsOnly : Implementation
	{
		public override void Spawned()
		{
			if (Nrb.Runner.IsClient)
			{
				Nrb.SetIsKinematic(value: true);
			}
		}

		public override void Render()
		{
			if (Nrb.GetInterpolationData(out var data, Nrb.Runner.IsServer))
			{
				Nrb.InterpolateTransform(ref data, Nrb.Runner.IsServer);
			}
		}

		public override void CopyBackingFieldsToState()
		{
			Assert.Check(Nrb.Object.HasStateAuthority);
			Nrb.CopyEngine2Buffers(posRotOnly: true);
		}

		public override void BeforeAllTicks(bool resimulation)
		{
			if (!Nrb.Object.HasStateAuthority)
			{
				Nrb.CopyBuffers2Engine();
			}
		}

		public override void AfterAllTicks(bool resimulation)
		{
			if (Nrb.Object.HasStateAuthority)
			{
				Nrb.CopyEngine2Buffers();
			}
		}

		public override void StateAuthorityChanged()
		{
		}

		public override void RemotePrefabCreated()
		{
			Nrb.CopyBuffers2Engine(posRotOnly: true);
		}

		public override void BeforeCopyPreviousState()
		{
		}
	}

	internal class SharedMode : Implementation
	{
		public override void Spawned()
		{
			if (!Nrb.Object.HasStateAuthority)
			{
				Nrb.SetIsKinematic(value: true);
			}
		}

		public override void Render()
		{
			if (Nrb.GetInterpolationData(out var data, Nrb.Object.HasStateAuthority))
			{
				Nrb.InterpolateTransform(ref data, Nrb.Object.HasStateAuthority);
			}
		}

		public override void CopyBackingFieldsToState()
		{
			Nrb.CopyEngine2Buffers(posRotOnly: true);
			Nrb.Copy2BuffersFlags();
		}

		public override void RemotePrefabCreated()
		{
			Nrb.CopyBuffers2Engine(posRotOnly: true);
		}

		public override void BeforeCopyPreviousState()
		{
			if (Nrb.Object.HasStateAuthority)
			{
				Nrb.CopyEngine2Buffers(posRotOnly: true);
			}
		}

		public override void BeforeAllTicks(bool resimulation)
		{
			if (!Nrb.Object.HasStateAuthority)
			{
				Nrb.CopyBuffers2Engine(posRotOnly: true);
				Nrb.SetIsKinematic(value: true);
			}
		}

		public override void AfterAllTicks(bool resimulation)
		{
			if (Nrb.Object.HasStateAuthority)
			{
				Nrb.CopyEngine2Buffers(posRotOnly: true);
				Nrb.Copy2BuffersFlags();
			}
		}

		public unsafe override void StateAuthorityChanged()
		{
			bool isKinematic = !Nrb.Object.HasStateAuthority || (Nrb.Ptr[31] & 1) == 1;
			Nrb.SetIsKinematic(isKinematic);
		}
	}

	protected const int VELOCITY_OFFSET = 25;

	protected const int ANGVELOC_OFFSET = 28;

	protected const int BITFLAGS_OFFSET = 31;

	protected const int VEL_DRAG_OFFSET = 32;

	protected const int ANG_DRAG_OFFSET = 33;

	protected const int RBD_MASS_OFFSET = 34;

	protected const int BASICS_WORD_CNT = 32;

	protected const int EXTRAS_WORD_CNT = 35;

	protected const int FLAG_ISKINEMATIC = 1;

	protected const int FLAG_USE_GRAVITY = 2;

	protected const int FLAG_SLEEPING = 4;

	protected const int FLAG_SIMULATE = 8;

	protected const int CONSTRAINTS_SHIFT = 4;

	internal Implementation Impl;

	[InlineHelp]
	[SerializeField]
	protected bool SyncDragAndMass = false;

	public override int? DynamicWordCount => SyncDragAndMass ? 35 : 32;

	internal override bool IsValidInterpolationTarget
	{
		get
		{
			if (this == null)
			{
				return true;
			}
			return _interpolationTarget != null && _interpolationTarget != base.transform;
		}
	}

	internal abstract void SetIsKinematic(bool value);

	private void InitImpl()
	{
		if (Impl == null)
		{
			if (Runner.Config.Simulation.Topology == SimulationConfig.Topologies.Shared)
			{
				Impl = new SharedMode();
			}
			else if (Runner.Config.ServerPhysicsMode == NetworkProjectConfig.PhysicsModes.ClientPrediction)
			{
				Impl = new ClientSidePhysicsPrediction();
			}
			else
			{
				Impl = new ServerPhysicsOnly();
			}
			Impl.Nrb = this;
		}
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		InitImpl();
		Impl.CopyBackingFieldsToState();
	}

	public override void Spawned()
	{
		base.Spawned();
		Impl.Spawned();
	}

	internal abstract void Copy2BuffersFlags();

	public override void RemotePrefabCreated()
	{
		InitImpl();
		Impl.RemotePrefabCreated();
	}

	public override void BeforeAllTicks(bool resimulation, int tickCount)
	{
		Impl.BeforeAllTicks(resimulation);
	}

	public override void AfterAllTicks(bool resimulation, int tickCount)
	{
		Impl.AfterAllTicks(resimulation);
	}

	void IBeforeCopyPreviousState.BeforeCopyPreviousState()
	{
		Impl.BeforeCopyPreviousState();
	}

	public override void Render()
	{
		if (base.InterpolationDataSource != InterpolationDataSources.NoInterpolation && (bool)_interpolationTarget)
		{
			Impl?.Render();
		}
	}
}
