using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Animation/Animator.h")]
public struct MatchTargetWeightMask(Vector3 positionXYZWeight, float rotationWeight)
{
	private Vector3 m_PositionXYZWeight = positionXYZWeight;

	private float m_RotationWeight = rotationWeight;

	public Vector3 positionXYZWeight
	{
		get
		{
			return m_PositionXYZWeight;
		}
		set
		{
			m_PositionXYZWeight = value;
		}
	}

	public float rotationWeight
	{
		get
		{
			return m_RotationWeight;
		}
		set
		{
			m_RotationWeight = value;
		}
	}
}
