using UnityEngine;

namespace Fusion.KCC;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkObject))]
[NetworkBehaviourWeaved(0)]
public abstract class NetworkAoIKCCProcessor : NetworkAreaOfInterestBehaviour, IKCCProcessor, IKCCProcessorProvider, IKCCInteractionProvider
{
	private static Changed<NetworkAoIKCCProcessor> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<NetworkAoIKCCProcessor> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<NetworkAoIKCCProcessor> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	public virtual float Priority => 0f;

	public virtual EKCCStages GetValidStages(KCC kcc, KCCData data)
	{
		return EKCCStages.All;
	}

	public virtual void SetInputProperties(KCC kcc, KCCData data)
	{
	}

	public virtual void SetDynamicVelocity(KCC kcc, KCCData data)
	{
	}

	public virtual void SetKinematicDirection(KCC kcc, KCCData data)
	{
	}

	public virtual void SetKinematicTangent(KCC kcc, KCCData data)
	{
	}

	public virtual void SetKinematicSpeed(KCC kcc, KCCData data)
	{
	}

	public virtual void SetKinematicVelocity(KCC kcc, KCCData data)
	{
	}

	public virtual void ProcessPhysicsQuery(KCC kcc, KCCData data)
	{
	}

	public virtual void OnEnter(KCC kcc, KCCData data)
	{
	}

	public virtual void OnExit(KCC kcc, KCCData data)
	{
	}

	public virtual void OnStay(KCC kcc, KCCData data)
	{
	}

	public virtual void OnInterpolate(KCC kcc, KCCData data)
	{
	}

	public virtual void ProcessUserLogic(KCC kcc, KCCData data, object userData)
	{
	}

	public virtual bool CanStartInteraction(KCC kcc, KCCData data)
	{
		return true;
	}

	public virtual bool CanStopInteraction(KCC kcc, KCCData data)
	{
		return true;
	}

	IKCCProcessor IKCCProcessorProvider.GetProcessor()
	{
		return this;
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		base.CopyBackingFieldsToState(P_0);
	}

	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
