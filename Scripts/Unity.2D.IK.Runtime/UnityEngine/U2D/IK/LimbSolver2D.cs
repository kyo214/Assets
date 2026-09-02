using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK;

[MovedFrom("UnityEngine.Experimental.U2D.IK")]
[Solver2DMenu("Limb")]
public sealed class LimbSolver2D : Solver2D
{
	[SerializeField]
	private IKChain2D m_Chain = new IKChain2D();

	[SerializeField]
	private bool m_Flip;

	private Vector3[] m_Positions = new Vector3[3];

	private float[] m_Lengths = new float[2];

	private float[] m_Angles = new float[2];

	public bool flip
	{
		get
		{
			return m_Flip;
		}
		set
		{
			m_Flip = value;
		}
	}

	protected override void DoInitialize()
	{
		m_Chain.transformCount = ((!(m_Chain.effector == null) && IKUtility.GetAncestorCount(m_Chain.effector) >= 2) ? 3 : 0);
		base.DoInitialize();
	}

	protected override int GetChainCount()
	{
		return 1;
	}

	public override IKChain2D GetChain(int index)
	{
		return m_Chain;
	}

	protected override void DoPrepare()
	{
		float[] lengths = m_Chain.lengths;
		m_Positions[0] = m_Chain.transforms[0].position;
		m_Positions[1] = m_Chain.transforms[1].position;
		m_Positions[2] = m_Chain.transforms[2].position;
		m_Lengths[0] = lengths[0];
		m_Lengths[1] = lengths[1];
	}

	protected override void DoUpdateIK(List<Vector3> targetPositions)
	{
		Vector3 position = targetPositions[0];
		Transform transform = m_Chain.transforms[0];
		Transform transform2 = m_Chain.transforms[1];
		Transform effector = m_Chain.effector;
		Vector2 vector = transform.InverseTransformPoint(position);
		position = transform.TransformPoint(vector);
		if (vector.sqrMagnitude > 0f && Limb.Solve(position, m_Lengths, m_Positions, ref m_Angles))
		{
			float angle = Vector2.SignedAngle(Vector2.right, vector) + Vector2.SignedAngle(transform2.localPosition, Vector2.right) + (flip ? (-1f) : 1f) * m_Angles[0];
			transform.localRotation *= Quaternion.AngleAxis(angle, Vector3.forward);
			float angle2 = Vector2.SignedAngle(Vector2.right, transform2.InverseTransformPoint(position)) + Vector2.SignedAngle(effector.localPosition, Vector2.right);
			m_Chain.transforms[1].localRotation *= Quaternion.AngleAxis(angle2, Vector3.forward);
		}
	}
}
