using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCTransientData
{
	public Vector3 JumpImpulse;

	public Vector3 ExternalVelocity;

	public Vector3 ExternalAcceleration;

	public Vector3 ExternalImpulse;

	public Vector3 ExternalForce;

	public void Store(KCC kcc, KCCData data)
	{
		JumpImpulse = data.JumpImpulse;
		ExternalVelocity = data.ExternalVelocity;
		ExternalAcceleration = data.ExternalAcceleration;
		ExternalImpulse = data.ExternalImpulse;
		ExternalForce = data.ExternalForce;
	}

	public void Restore(KCC kcc, KCCData data)
	{
		data.JumpImpulse -= JumpImpulse;
		data.ExternalVelocity -= ExternalVelocity;
		data.ExternalAcceleration -= ExternalAcceleration;
		data.ExternalImpulse -= ExternalImpulse;
		data.ExternalForce -= ExternalForce;
	}

	public void Clear()
	{
		JumpImpulse = default;
		ExternalVelocity = default;
		ExternalAcceleration = default;
		ExternalImpulse = default;
		ExternalForce = default;
	}
}
