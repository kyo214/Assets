using System;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(7)]
[DisallowMultipleComponent]
[OrderAfter(new Type[] { typeof(NetworkAreaOfInterestBehaviour) })]
public class NetworkTransformAnchor : NetworkAreaOfInterestBehaviour, IBeforeAllTicks, IAfterAllTicks, IRemotePrefabCreated, IBeforeCopyPreviousState
{
	protected const int PARENT_OFFSET = 0;

	protected const int TELE_PAR_OFFSET = 2;

	private const int POSITION_OFFSET = 4;

	protected const int ANCHOR_WORD_COUNT = 7;

	[InlineHelp]
	[SerializeField]
	[WarnIf("IsValidInterpolationTarget", false, "'Interpolation Target' should be a non-physics GameObject. Typically a child of this GameObject, or a separate object without colliders. The runner/simulation with State Authority will not interpolate if target is null or the same GameObject as a Rigidbody.")]
	protected Transform _interpolationTarget;

	[InlineHelp]
	[SerializeField]
	protected bool DetachInterpTarget;

	[InlineHelp]
	[SerializeField]
	protected bool SyncParent = true;

	[NonSerialized]
	[InlineHelp]
	private Transform _transform;

	public override int PositionWordOffset => 4;

	internal virtual bool IsValidInterpolationTarget => true;

	public Transform InterpolationTarget
	{
		get
		{
			return _interpolationTarget;
		}
		set
		{
			_interpolationTarget = value;
		}
	}

	protected Transform Transform => _transform ? _transform : (_transform = base.transform);

	protected virtual void OnEnable()
	{
	}

	protected virtual void Awake()
	{
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		if (Object.HasStateAuthority)
		{
			CopyEngine2Buffers();
		}
	}

	public virtual void RemotePrefabCreated()
	{
		CopyBuffers2Engine();
	}

	public virtual void BeforeAllTicks(bool resimulation, int tickCount)
	{
		if (!Object.HasStateAuthority)
		{
			CopyBuffers2Engine();
		}
	}

	public virtual void AfterAllTicks(bool resimulation, int tickCount)
	{
		CopyEngine2Buffers(!Object.HasStateAuthority);
	}

	public virtual void BeforeCopyPreviousState()
	{
		if (!Object.HasStateAuthority)
		{
			CopyEngine2Buffers(posRotOnly: true);
		}
	}

	internal virtual void CopyBuffers2Engine(bool posRotOnly = false)
	{
		if (SyncParent)
		{
			Copy2EngineAnchorState();
		}
	}

	internal virtual void CopyEngine2Buffers(bool posRotOnly = false)
	{
		if (SyncParent)
		{
			Copy2BufferAnchorState();
		}
	}

	protected unsafe bool Copy2EngineAnchorState()
	{
		NetworkBehaviour networkBehaviour = ReadWriteUtils.ReadNetworkBehaviourRef(Ptr, Runner, out var isValid);
		if (isValid)
		{
			Transform transform = (BehaviourUtils.IsAlive(networkBehaviour) ? networkBehaviour.transform : null);
			if (Transform.parent != transform)
			{
				Transform.SetParent(transform);
				return true;
			}
			return false;
		}
		return false;
	}

	protected unsafe void Copy2BufferAnchorState(int offset = 0)
	{
		Transform parent = Transform.parent;
		NetworkTransformAnchor component;
		if (parent == null)
		{
			ReadWriteUtils.WriteNullkBehaviourRef(Ptr + offset);
		}
		else if (parent.TryGetComponent<NetworkTransformAnchor>(out component))
		{
			ReadWriteUtils.WriteNetworkBehaviourRef(Ptr + offset, Runner, component);
		}
		else
		{
			ReadWriteUtils.WriteEmptyNetworkBehaviourRef(Ptr + offset);
		}
	}

	public override void Render()
	{
		if (base.InterpolationDataSource != InterpolationDataSources.NoInterpolation && GetInterpolationData(out var data) && SyncParent)
		{
			GetParentsForInterpolation(this, Runner, isTeleport: false, ref data, out var _, out var _);
		}
	}

	internal unsafe static (NetworkTransformAnchor parentFr, NetworkTransformAnchor parentTo) GetParentsForInterpolation(NetworkTransformAnchor nta, NetworkRunner runner, bool isTeleport, ref InterpolationData data, out bool fromParentIsValid, out bool toParentIsValid)
	{
		NetworkTransformAnchor networkTransformAnchor = (NetworkTransformAnchor)ReadWriteUtils.ReadNetworkBehaviourRef(data.From, runner, out fromParentIsValid);
		NetworkTransformAnchor networkTransformAnchor2 = (NetworkTransformAnchor)ReadWriteUtils.ReadNetworkBehaviourRef(data.To + (isTeleport ? 2 : 0), runner, out toParentIsValid);
		if (fromParentIsValid)
		{
			if (BehaviourUtils.IsAlive(networkTransformAnchor))
			{
				networkTransformAnchor.Render();
			}
		}
		else if ((bool)nta._interpolationTarget)
		{
			Transform parent = nta.Transform.parent;
			if ((bool)parent)
			{
				NetworkTransformAnchor componentInParent = parent.GetComponentInParent<NetworkTransformAnchor>();
				if (BehaviourUtils.IsAlive(componentInParent))
				{
					componentInParent.Render();
				}
			}
		}
		if (toParentIsValid)
		{
			if (BehaviourUtils.IsAlive(networkTransformAnchor2))
			{
				networkTransformAnchor2.Render();
			}
		}
		else if ((bool)nta._interpolationTarget)
		{
			Transform parent2 = nta.Transform.parent;
			if ((bool)parent2)
			{
				NetworkTransformAnchor componentInParent2 = parent2.GetComponentInParent<NetworkTransformAnchor>();
				if (BehaviourUtils.IsAlive(componentInParent2))
				{
					componentInParent2.Render();
				}
			}
		}
		return (parentFr: networkTransformAnchor, parentTo: networkTransformAnchor2);
	}
}
