using UnityEngine;

namespace Fusion.KCC;

public abstract class BaseKCCProcessor : MonoBehaviour, IKCCProcessor
{
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
}
